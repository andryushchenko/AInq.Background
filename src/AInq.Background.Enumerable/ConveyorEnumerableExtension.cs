// Copyright 2020-2021 Anton Andryushchenko
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
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="conveyor" /> or <paramref name="data" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IConveyor<TData, TResult> conveyor, IEnumerable<TData> data,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        where TData : notnull
    {
        _ = conveyor ?? throw new ArgumentNullException(nameof(conveyor));
        var results = (data ?? throw new ArgumentNullException(nameof(data))).Select(item
            => conveyor.ProcessDataAsync(item, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IConveyor{TData,TResult},IEnumerable{TData},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IEnumerable<TData> data, IConveyor<TData, TResult> conveyor,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        where TData : notnull
        => (conveyor ?? throw new ArgumentNullException(nameof(conveyor))).ProcessDataAsync(data, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process data with giver <paramref name="priority" /> </summary>
    /// <param name="conveyor"> Conveyor instance </param>
    /// <param name="data"> Data to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="conveyor" /> or <paramref name="data" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IPriorityConveyor<TData, TResult> conveyor,
        IEnumerable<TData> data, int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1,
        bool enqueueAll = false)
        where TData : notnull
    {
        _ = conveyor ?? throw new ArgumentNullException(nameof(conveyor));
        var results = (data ?? throw new ArgumentNullException(nameof(data))).Select(item
            => conveyor.ProcessDataAsync(item, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IEnumerable<TData> data, IPriorityConveyor<TData, TResult> conveyor,
        int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        where TData : notnull
        => (conveyor ?? throw new ArgumentNullException(nameof(conveyor))).ProcessDataAsync(data, priority, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process data using registered conveyor with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="data"> Data to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> or <paramref name="data" /> is NULL </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if no conveyor for given <typeparamref name="TData" /> and <typeparamref name="TResult" /> is registered
    /// </exception>
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, IEnumerable<TData> data,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false, int priority = 0)
        where TData : notnull
    {
        var conveyor = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IConveyor<TData, TResult>>();
        return conveyor is IPriorityConveyor<TData, TResult> priorityConveyor
            ? priorityConveyor.ProcessDataAsync(data, priority, cancellation, attemptsCount, enqueueAll)
            : conveyor.ProcessDataAsync(data, cancellation, attemptsCount, enqueueAll);
    }

#endregion

#region AsyncEnumerable

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IConveyor{TData,TResult},IEnumerable{TData},CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IConveyor<TData, TResult> conveyor,
        IAsyncEnumerable<TData> data, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1)
        where TData : notnull
    {
        _ = conveyor ?? throw new ArgumentNullException(nameof(conveyor));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var item in data.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(conveyor.ProcessDataAsync(item, cancellation, attemptsCount), cancellation).ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
#if NET5_0_OR_GREATER
        await foreach (var result in reader.ReadAllAsync(cancellation).ConfigureAwait(false))
            yield return await result.ConfigureAwait(false);
#else
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IConveyor{TData,TResult},IEnumerable{TData},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IAsyncEnumerable<TData> data, IConveyor<TData, TResult> conveyor,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TData : notnull
        => (conveyor ?? throw new ArgumentNullException(nameof(conveyor))).ProcessDataAsync(data, cancellation, attemptsCount);

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IPriorityConveyor<TData, TResult> conveyor,
        IAsyncEnumerable<TData> data, int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1)
        where TData : notnull
    {
        _ = conveyor ?? throw new ArgumentNullException(nameof(conveyor));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var item in data.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(conveyor.ProcessDataAsync(item, priority, cancellation, attemptsCount), cancellation)
                                    .ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
#if NET5_0_OR_GREATER
        await foreach (var result in reader.ReadAllAsync(cancellation).ConfigureAwait(false))
            yield return await result.ConfigureAwait(false);
#else
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IPriorityConveyor{TData,TResult},IEnumerable{TData},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IAsyncEnumerable<TData> data,
        IPriorityConveyor<TData, TResult> conveyor, int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1)
        where TData : notnull
        => (conveyor ?? throw new ArgumentNullException(nameof(conveyor))).ProcessDataAsync(data, priority, cancellation, attemptsCount);

    /// <inheritdoc cref="ProcessDataAsync{TData,TResult}(IServiceProvider,IEnumerable{TData},CancellationToken,int,bool,int)" />
    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, IAsyncEnumerable<TData> data,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TData : notnull
    {
        var conveyor = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IConveyor<TData, TResult>>();
        return conveyor is IPriorityConveyor<TData, TResult> priorityConveyor
            ? priorityConveyor.ProcessDataAsync(data, priority, cancellation, attemptsCount)
            : conveyor.ProcessDataAsync(data, cancellation, attemptsCount);
    }

#endregion
}
