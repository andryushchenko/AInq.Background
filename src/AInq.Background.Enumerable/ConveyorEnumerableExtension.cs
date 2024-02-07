// Copyright 2020 Anton Andryushchenko
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace AInq.Background;

/// <summary> <see cref="IConveyor{TData,TResult}" /> and <see cref="IPriorityConveyor{TData,TResult}" /> batch processing extension </summary>
public static class ConveyorEnumerableExtension
{
#region Enumerable

    /// <summary> Batch process data </summary>
    /// <param name="conveyor"> Conveyor instance </param>
    /// <param name="data"> Data to process </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="conveyor" /> or <paramref name="data" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IConveyor<TData, TResult> conveyor, IEnumerable<TData> data,
        int attemptsCount = 1, bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)
        where TData : notnull
    {
        _ = conveyor ?? throw new ArgumentNullException(nameof(conveyor));
        var results = (data ?? throw new ArgumentNullException(nameof(data)))
                      .Where(item => item != null)
                      .Select(item => conveyor.ProcessDataAsync(item, attemptsCount, cancellation));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IConveyor{TData,TResult},IEnumerable{TData},int,bool,CancellationToken)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IEnumerable<TData> data, IConveyor<TData, TResult> conveyor,
        int attemptsCount = 1, bool enqueueAll = false, CancellationToken cancellation = default)
        where TData : notnull
        => (conveyor ?? throw new ArgumentNullException(nameof(conveyor)))
            .ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), attemptsCount, enqueueAll, cancellation);

    /// <summary> Batch process data with giver <paramref name="priority" /> </summary>
    /// <param name="conveyor"> Conveyor instance </param>
    /// <param name="data"> Data to process </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="conveyor" /> or <paramref name="data" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IPriorityConveyor<TData, TResult> conveyor,
        IEnumerable<TData> data, int priority = 0, int attemptsCount = 1, bool enqueueAll = false,
        [EnumeratorCancellation] CancellationToken cancellation = default)
        where TData : notnull
    {
        _ = conveyor ?? throw new ArgumentNullException(nameof(conveyor));
        var results = (data ?? throw new ArgumentNullException(nameof(data)))
                      .Where(item => item != null)
                      .Select(item => conveyor.ProcessDataAsync(item, priority, attemptsCount, cancellation));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,int,bool,CancellationToken)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IEnumerable<TData> data, IPriorityConveyor<TData, TResult> conveyor,
        int priority = 0, int attemptsCount = 1, bool enqueueAll = false, CancellationToken cancellation = default)
        where TData : notnull
        => (conveyor ?? throw new ArgumentNullException(nameof(conveyor)))
            .ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), priority, attemptsCount, enqueueAll, cancellation);

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,int,bool,CancellationToken)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, IEnumerable<TData> data,
        int priority = 0, int attemptsCount = 1, bool enqueueAll = false, CancellationToken cancellation = default)
        where TData : notnull
    {
        var conveyor = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IConveyor<TData, TResult>>();
        return conveyor is IPriorityConveyor<TData, TResult> priorityConveyor
            ? priorityConveyor.ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)),
                priority,
                attemptsCount,
                enqueueAll,
                cancellation)
            : conveyor.ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), attemptsCount, enqueueAll, cancellation);
    }

#endregion

#region AsyncEnumerable

    private static async void PushData<TData, TResult>(ChannelWriter<Task<TResult>> writer, IConveyor<TData, TResult> conveyor,
        IAsyncEnumerable<TData> data, int attemptsCount, CancellationToken cancellation)
        where TData : notnull
    {
        try
        {
            await foreach (var item in data.WithCancellation(cancellation).ConfigureAwait(false))
                if (item != null)
                    await writer.WriteAsync(conveyor.ProcessDataAsync(item, attemptsCount, cancellation), cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await writer.WriteAsync(Task.FromException<TResult>(ex), CancellationToken.None).ConfigureAwait(false);
        }
        finally
        {
            writer.Complete();
        }
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IConveyor{TData,TResult},IEnumerable{TData},int,bool,CancellationToken)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IConveyor<TData, TResult> conveyor,
        IAsyncEnumerable<TData> data, int attemptsCount = 1, [EnumeratorCancellation] CancellationToken cancellation = default)
        where TData : notnull
    {
        _ = conveyor ?? throw new ArgumentNullException(nameof(conveyor));
        _ = data ?? throw new ArgumentNullException(nameof(data));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushData(channel.Writer, conveyor, data, attemptsCount, cancellation);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IConveyor{TData,TResult},IEnumerable{TData},int,bool,CancellationToken)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IAsyncEnumerable<TData> data, IConveyor<TData, TResult> conveyor,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TData : notnull
        => (conveyor ?? throw new ArgumentNullException(nameof(conveyor)))
            .ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), attemptsCount, cancellation);

    private static async void PushData<TData, TResult>(ChannelWriter<Task<TResult>> writer, IPriorityConveyor<TData, TResult> priorityConveyor,
        IAsyncEnumerable<TData> data, int priority, int attemptsCount, CancellationToken cancellation)
        where TData : notnull
    {
        try
        {
            await foreach (var item in data.WithCancellation(cancellation).ConfigureAwait(false))
                if (item != null)
                    await writer.WriteAsync(priorityConveyor.ProcessDataAsync(item, priority, attemptsCount, cancellation), cancellation)
                                .ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await writer.WriteAsync(Task.FromException<TResult>(ex), CancellationToken.None).ConfigureAwait(false);
        }
        finally
        {
            writer.Complete();
        }
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,int,bool,CancellationToken)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IPriorityConveyor<TData, TResult> priorityConveyor,
        IAsyncEnumerable<TData> data, int priority = 0, int attemptsCount = 1, [EnumeratorCancellation] CancellationToken cancellation = default)
        where TData : notnull
    {
        _ = priorityConveyor ?? throw new ArgumentNullException(nameof(priorityConveyor));
        _ = data ?? throw new ArgumentNullException(nameof(data));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushData(channel.Writer, priorityConveyor, data, priority, attemptsCount, cancellation);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,int,bool,CancellationToken)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IAsyncEnumerable<TData> data,
        IPriorityConveyor<TData, TResult> priorityConveyor, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TData : notnull
        => (priorityConveyor ?? throw new ArgumentNullException(nameof(priorityConveyor)))
            .ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), priority, attemptsCount, cancellation);

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,int,bool,CancellationToken)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, IAsyncEnumerable<TData> data,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TData : notnull
    {
        var conveyor = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IConveyor<TData, TResult>>();
        return conveyor is IPriorityConveyor<TData, TResult> priorityConveyor
            ? priorityConveyor.ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), priority, attemptsCount, cancellation)
            : conveyor.ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), attemptsCount, cancellation);
    }

#endregion
}
