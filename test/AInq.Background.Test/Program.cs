/*
 * Copyright 2020 Anton Andryushchenko
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                   .ConfigureServices((context, services) =>
                   {
                       services.AddTransient<TestMachine>()
                               .AddConveyor<TestMachine, int, int>(ReuseStrategy.Reuse, 3)
                               .AddWorkScheduler()
                               .AddWorkQueue()
                               .AddStartupWork(WorkFactory.CreateWork(provider =>
                               {
                                   for (var index = 1; index <= 10; index++)
                                       provider.ProcessData<int, int>(index);
                                   provider.AddDelayedQueueWork(WorkFactory.CreateWork(async serviceProvider =>
                                       {
                                           using var source = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                                           var tasks = Enumerable.Range(1, 10).Select(index => serviceProvider.ProcessData<int, int>(index, source.Token)).ToList();
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
