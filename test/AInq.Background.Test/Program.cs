// Copyright 2021 Anton Andryushchenko
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
using AInq.Background.Enumerable;
using AInq.Background.Helpers;
using AInq.Background.Interaction;
using AInq.Background.Tasks;
using AInq.Background.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

var host = new HostBuilder().ConfigureLogging(logging => logging.ClearProviders().AddDebug())
                            .ConfigureServices((context, services) =>
                            {
                                services.AddTransient<TestMachine>()
                                        .AddPriorityConveyor<int, int, TestMachine>(ReuseStrategy.Reuse, 3)
                                        .AddWorkScheduler()
                                        .AddWorkQueue()
                                        .AddStartupWork(WorkFactory.CreateWork(provider =>
                                        {
                                            provider.AddCronWork(WorkFactory.CreateWork(_ => DateTime.Now),
                                                        "0/10 * * * * *",
                                                        new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token)
                                                    .Subscribe(new TestObserver<DateTime>());
                                            provider.AddRepeatedWork(
                                                WorkFactory.CreateWork(_ => Console.WriteLine($"{DateTime.Now:T}\tRepeated work test")),
                                                TimeSpan.FromMinutes(1),
                                                TimeSpan.FromSeconds(15),
                                                execCount: 4);
                                            for (var index = 1; index <= 10; index++)
                                                provider.ProcessDataAsync<int, int>(index, priority: 50 + index);
                                            provider.AddScheduledAsyncQueueWork(WorkFactory.CreateAsyncWork(async (serviceProvider, _) =>
                                                {
                                                    using var source = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                                                    var tasks = Enumerable.Range(1, 10)
                                                                          .Select(index => serviceProvider.ProcessDataAsync<int, int>(index,
                                                                              source.Token))
                                                                          .ToList();
                                                    try
                                                    {
                                                        await Task.WhenAll(tasks);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex);
                                                    }
                                                    foreach (var task in tasks)
                                                        Console.WriteLine(
                                                            $"{tasks.IndexOf(task) + 1}\t{(task.IsCompletedSuccessfully ? task.Result.ToString() : "Canceled")}");
                                                }),
                                                TimeSpan.FromSeconds(20));
                                            provider.AddScheduledAsyncWork(WorkFactory.CreateAsyncWork(async (serviceProvider, cancel) =>
                                                {
                                                    using var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                                                    Console.WriteLine($"{DateTime.Now:T}\tDelayed start test");
                                                    _ = serviceProvider.EnqueueAsyncWork(
                                                        WorkFactory.CreateAsyncWork((_, token) => Task.Delay(TimeSpan.FromSeconds(8), token)),
                                                        cancel);
                                                    var test = serviceProvider.EnqueueWork(
                                                        WorkFactory.CreateWork(_ => $"{DateTime.Now:T}\tDelayed work test"),
                                                        cancel);
                                                    try
                                                    {
                                                        await serviceProvider.EnqueueWork(WorkFactory.CreateWork(_ => true), source.Token);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Console.WriteLine($"{DateTime.Now:T}\tWork cancellation test");
                                                    }
                                                    Console.WriteLine(await test);
                                                }),
                                                TimeSpan.FromSeconds(30));
                                            provider.AddScheduledAsyncWork(WorkFactory.CreateAsyncWork(async (serviceProvider, cancel) =>
                                                {
                                                    await foreach (var result in serviceProvider.ProcessDataAsync<int, int>(
                                                        Enumerable.Range(1, 10).ToAsyncEnumerable(),
                                                        cancel))
                                                        Console.WriteLine($"{DateTime.Now:T}\tEnumerator test {result}");
                                                }),
                                                TimeSpan.FromSeconds(60));
                                        }));
                            })
                            .Build();
var cancellation = new CancellationTokenSource();
var work = host.DoStartupWorkAsync(cancellation.Token)
               .ContinueWith(_ => Console.WriteLine($"{DateTime.Now}\t Starting host"))
               .ContinueWith(async _ => await host.RunAsync(cancellation.Token), cancellation.Token);
Console.ReadLine();
cancellation.Cancel();
Console.WriteLine($"{DateTime.Now:T}\tGeneral stop requested");
await work.Unwrap();
Console.WriteLine($"{DateTime.Now:T}\tStopped");
