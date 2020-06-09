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
                               .AddConveyor<int, int, TestMachine>(ReuseStrategy.Reuse, 3)
                               .AddWorkScheduler()
                               .AddWorkQueue()
                               .AddStartupWork(WorkFactory.CreateWork(provider =>
                               {
                                   var conveyor = provider.GetRequiredService<IConveyor<int, int>>();
                                   for (var index = 1; index <= 20; index++)
                                       conveyor.ProcessDataAsync(index);
                                   provider.GetRequiredService<IWorkScheduler>()
                                           .AddDelayedQueueWork(WorkFactory.CreateWork(serviceProvider =>
                                               {
                                                   var dataConveyor = serviceProvider.GetRequiredService<IConveyor<int, int>>();
                                                   for (var index = 1; index <= 20; index++)
                                                       dataConveyor.ProcessDataAsync(index);
                                               }),
                                               TimeSpan.FromMinutes(1));
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
