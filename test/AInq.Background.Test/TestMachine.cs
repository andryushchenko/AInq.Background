// Copyright 2020-2022 Anton Andryushchenko
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

namespace AInq.Background.Test;

internal sealed class TestMachine : IConveyorMachine<int, int>, IStartStoppable
{
    private readonly string _name;
    private bool _isRunning;

    public TestMachine()
    {
        _name = DateTime.Now.Ticks.ToString("X");
        Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} created");
    }

    async Task<int> IConveyorMachine<int, int>.ProcessDataAsync(int data, IServiceProvider provider, CancellationToken cancellation)
    {
        Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} checking provider {provider}");
        Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} processing {data}");
        await Task.Delay(3000, cancellation).ConfigureAwait(false);
        Console.WriteLine($"{DateTime.Now:T}\tMachine ID {_name} processed {data}");
        return data * data;
    }

    bool IStartStoppable.IsActive => _isRunning;

    async Task IStartStoppable.ActivateAsync(CancellationToken cancellation)
    {
        Console.WriteLine($"{DateTime.Now:T}\tStarting machine ID {_name}");
        await Task.Delay(2000, cancellation).ConfigureAwait(false);
        _isRunning = true;
        Console.WriteLine($"{DateTime.Now:T}\tMachine {_name} started");
    }

    async Task IStartStoppable.DeactivateAsync(CancellationToken cancellation)
    {
        Console.WriteLine($"{DateTime.Now:T}\tStopping machine ID {_name}");
        await Task.Delay(2000, cancellation).ConfigureAwait(false);
        _isRunning = false;
        Console.WriteLine($"{DateTime.Now:T}\tMachine {_name} stopped");
    }
}
