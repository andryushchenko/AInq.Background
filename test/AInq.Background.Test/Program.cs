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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Test
{

internal static class Program
{
    private static async Task Main()
    {
        var host = new HostBuilder()
                   .ConfigureLogging(logging =>
                   {
                       logging.ClearProviders();
                       logging.AddDebug();
                   })
                   .ConfigureServices((context, services) =>
                   {
                       services.AddTransient<TestMachine>()
                               .AddPriorityConveyor<int, int, TestMachine>(ReuseStrategy.Reuse, 3)
                               .AddWorkScheduler()
                               .AddWorkQueue()
                               .AddStartupWork(WorkFactory.CreateWork(provider =>
                               {
                                   for (var index = 1; index <= 10; index++)
                                       provider.ProcessDataAsync<int, int>(index, priority: 50 - index);
                                   provider.AddDelayedAsyncQueueWork(WorkFactory.CreateAsyncWork(async (serviceProvider, cancel) =>
                                       {
                                           using var source = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                                           var tasks = Enumerable.Range(1, 10).Select(index => serviceProvider.ProcessDataAsync<int, int>(index, source.Token)).ToList();
                                           try
                                           {
                                               await Task.WhenAll(tasks);
                                           }
                                           catch (Exception ex)
                                           {
                                               Console.WriteLine(ex);
                                           }
                                           foreach (var task in tasks)
                                           {
                                               Console.WriteLine($"{tasks.IndexOf(task) + 1}\t{(task.IsCompletedSuccessfully ? task.Result.ToString() : "Canceled")}");
                                           }
                                       }),
                                       TimeSpan.FromSeconds(20));
                                   provider.AddDelayedAsyncWork(WorkFactory.CreateAsyncWork(async (serviceProvider, cancel) =>
                                       {
                                           using var source = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                                           Console.WriteLine($"Start\t{DateTime.Now:T}");
                                           _ = serviceProvider.EnqueueAsyncWork(WorkFactory.CreateAsyncWork(token => Task.Delay(TimeSpan.FromSeconds(8), token)), cancel);
                                           var test = serviceProvider.EnqueueWork(WorkFactory.CreateWork(_ => $"Test\t{DateTime.Now:T}"), cancel);
                                           try
                                           {
                                               await serviceProvider.EnqueueWork(WorkFactory.CreateWork(_ => true), source.Token);
                                           }
                                           catch (Exception)
                                           {
                                               Console.WriteLine($"Cancel\t{DateTime.Now:T}");
                                           }
                                           Console.WriteLine(await test);
                                       }),
                                       TimeSpan.FromSeconds(30));
                               }));
                   })
                   .Build();
        var cancellation = new CancellationTokenSource();
        var work = host.DoStartupWork(cancellation.Token)
                       .ContinueWith(async task => await host.RunAsync(cancellation.Token), cancellation.Token);
        Console.ReadLine();
        await work;
    }
}

}
