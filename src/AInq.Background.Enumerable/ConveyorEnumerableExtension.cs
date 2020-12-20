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

using AInq.Background.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AInq.Background.Enumerable
{

public static class ConveyorEnumerableExtension
{
    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IConveyor<TData, TResult> conveyor, IEnumerable<TData> data,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        where TData : notnull
    {
        var priorityConveyor = (conveyor ?? throw new ArgumentNullException(nameof(conveyor))) as IPriorityConveyor<TData, TResult>;
        var results = (data ?? throw new ArgumentNullException(nameof(data)))
            .Select(item => priorityConveyor != null
                ? priorityConveyor.ProcessDataAsync(item, priority, cancellation, attemptsCount)
                : conveyor.ProcessDataAsync(item, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IEnumerable<TData> data, IConveyor<TData, TResult> conveyor,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        where TData : notnull
        => conveyor.ProcessDataAsync(data, cancellation, attemptsCount, priority, enqueueAll);

    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, IEnumerable<TData> data,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        where TData : notnull
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IConveyor<TData, TResult>)) as IConveyor<TData, TResult>
            ?? throw new InvalidOperationException($"No Conveyor service for {typeof(TData)} -> {typeof(TResult)} found"))
            .ProcessDataAsync(data, cancellation, attemptsCount, priority, enqueueAll);

    public static async IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IConveyor<TData, TResult> conveyor,
        IAsyncEnumerable<TData> data, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TData : notnull
    {
        var priorityConveyor = (conveyor ?? throw new ArgumentNullException(nameof(conveyor))) as IPriorityConveyor<TData, TResult>;
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var item in data.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(priorityConveyor != null
                                            ? priorityConveyor.ProcessDataAsync(item, priority, cancellation, attemptsCount)
                                            : conveyor.ProcessDataAsync(item, cancellation, attemptsCount),
                                        cancellation)
                                    .ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await writer.WriteAsync(Task.FromException<TResult>(ex), cancellation).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IAsyncEnumerable<TData> data, IConveyor<TData, TResult> conveyor,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TData : notnull
        => conveyor.ProcessDataAsync(data, cancellation, attemptsCount, priority);

    public static IAsyncEnumerable<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, IAsyncEnumerable<TData> data,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TData : notnull
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IConveyor<TData, TResult>)) as IConveyor<TData, TResult>
            ?? throw new InvalidOperationException($"No Conveyor service for {typeof(TData)} -> {typeof(TResult)} found"))
            .ProcessDataAsync(data, cancellation, attemptsCount, priority);
}

}
