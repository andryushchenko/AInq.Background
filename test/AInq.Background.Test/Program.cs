﻿// Copyright 2020 Anton Andryushchenko
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

using AInq.Background;
using AInq.Background.Helpers;
using AInq.Background.Interaction;
using AInq.Background.Services;
using AInq.Background.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IWorkQueue? parallelQueue = null;
var host = Host.CreateDefaultBuilder(args)
               .ConfigureLogging(logging => logging.ClearProviders().AddDebug())
               .ConfigureServices(services =>
               {
                   services.AddTransient<TestMachine>()
                           .AddPriorityConveyor<int, int, TestMachine>(ReuseStrategy.Reuse, 3)
                           .AddWorkScheduler()
                           .AddWorkQueue()
                           .AddStartupWork(WorkFactory.CreateWork(StartupWork));
                   parallelQueue = services.CreateWorkQueue();
               })
               .Build();
var cancellationSource = new CancellationTokenSource();
var work = host.DoStartupWorkAsync(cancellationSource.Token)
               .ContinueWith(_ => Console.WriteLine($"{DateTime.Now}\t Starting host"))
               .ContinueWith(async _ => await host.RunAsync(cancellationSource.Token).ConfigureAwait(false), cancellationSource.Token)
               .Unwrap();
Console.ReadLine();
await (parallelQueue?.EnqueueWork(WorkFactory.CreateWork(_ => Console.WriteLine($"{DateTime.Now:T}\tParallel queue test"))) ?? Task.CompletedTask)
    .ConfigureAwait(false);
Console.ReadLine();
cancellationSource.Cancel();
Console.WriteLine($"{DateTime.Now:T}\tGeneral stop requested");
await work.ConfigureAwait(false);
Console.WriteLine($"{DateTime.Now:T}\tStopped");

void StartupWork(IServiceProvider provider)
{
    provider.AddCronWork(WorkFactory.CreateWork(_ => DateTime.Now),
                "0/10 * * * * *",
                cancellation: new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token)
            .Subscribe(new TestObserver<DateTime>());
    provider.AddRepeatedWork(WorkFactory.CreateWork(_ => Console.WriteLine($"{DateTime.Now:T}\tRepeated work test")),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromSeconds(15),
        4);
    provider.AddScheduledAsyncQueueWork(WorkFactory.CreateAsyncWork(ConveyorTestAsync), TimeSpan.FromSeconds(20));
    provider.AddScheduledAsyncWork(WorkFactory.CreateAsyncWork(DelayedWorkTestAsync), TimeSpan.FromSeconds(30));
    provider.AddScheduledAsyncWork(WorkFactory.CreateAsyncWork(EnumeratorTestAsync), TimeSpan.FromSeconds(60));
}

async Task DelayedWorkTestAsync(IServiceProvider provider, CancellationToken cancellation)
{
    using var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    Console.WriteLine($"{DateTime.Now:T}\tDelayed start test");
    _ = provider.EnqueueAsyncWork(WorkFactory.CreateAsyncWork((_, token) => Task.Delay(TimeSpan.FromSeconds(8), token)), cancellation: cancellation);
    var test = provider.EnqueueWork(WorkFactory.CreateWork(_ => $"{DateTime.Now:T}\tDelayed work test"), cancellation: cancellation);
    try
    {
        await provider.EnqueueWork(WorkFactory.CreateWork(_ => true), cancellation: source.Token).ConfigureAwait(false);
    }
    catch (Exception)
    {
        Console.WriteLine($"{DateTime.Now:T}\tWork cancellation test");
    }
    Console.WriteLine(await test.ConfigureAwait(false));
}

async Task EnumeratorTestAsync(IServiceProvider provider, CancellationToken cancellation)
{
    var conveyor = provider.GetRequiredService<IConveyor<int, int>>();
    conveyor = new ConveyorChain<int, int, int>(conveyor, conveyor);
    await foreach (var result in conveyor.ProcessDataAsync(Enumerable.Range(1, 10), cancellation: cancellation).ConfigureAwait(false))
        Console.WriteLine($"{DateTime.Now:T}\tEnumerator test {result}");
}

async Task ConveyorTestAsync(IServiceProvider provider, CancellationToken cancellation)
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    using var cancel = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellation);
    var tasks = Enumerable.Range(1, 10)
                          .Select(index => provider.ProcessDataAsync<int, int>(index, cancellation: cancel.Token, priority: 50 + index))
                          .ToList();
    try
    {
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
    foreach (var task in tasks)
        Console.WriteLine($"{tasks.IndexOf(task) + 1}\t{(task.IsCompletedSuccessfully ? task.Result.ToString() : "Canceled")}");
}
