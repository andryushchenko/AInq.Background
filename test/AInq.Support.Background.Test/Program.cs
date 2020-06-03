using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AInq.Support.Background.Test
{
    internal static class Program
    {
        private static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<TestMachine>();
                    services.AddDataConveyor<int, int, TestMachine>(ReuseStrategy.Static, 3);
                    services.AddStartupWork(WorkFactory.CreateWork(provider =>
                    {
                        var conveyor = provider.GetRequiredService<IDataConveyor<int, int>>();
                        for (var index = 1; index <= 20; index++)
                            conveyor.ProcessDataAsync(index);
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