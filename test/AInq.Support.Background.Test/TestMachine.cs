using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Test
{
    public class TestMachine : IDataConveyorMachine<int, int>, IStoppableTaskMachine
    {
        private bool _isRunning;
        private readonly string _name;

        public TestMachine()
        {
            _name = DateTime.Now.Ticks.ToString("X");
            Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} created");
        }

        async Task<int> IDataConveyorMachine<int, int>.ProcessDataAsync(int data, IServiceProvider provider, CancellationToken cancellation)
        {
         Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} checking provider {provider}");   
            Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} processing {data}");
            await Task.Delay(3000, cancellation);
            Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} processed {data}");
            return data;
        }

        bool IStoppableTaskMachine.IsRunning => _isRunning;

        async Task IStoppableTaskMachine.StartMachineAsync(CancellationToken cancellation)
        {
            Console.WriteLine($"{DateTime.Now:T}\tStarting machine ID {_name}");
            await Task.Delay(2000, cancellation);
            _isRunning = true;
            Console.WriteLine($"{DateTime.Now:T}\tMachine {_name} started");
        }

        async Task IStoppableTaskMachine.StopMachineAsync(CancellationToken cancellation)
        {
            Console.WriteLine($"{DateTime.Now:T}\tStopping machine ID {_name}");
            await Task.Delay(2000, cancellation);
            _isRunning = false;
            Console.WriteLine($"{DateTime.Now:T}\tMachine {_name} stopped");
        }
    }
}