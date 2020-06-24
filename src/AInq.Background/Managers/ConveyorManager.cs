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

using AInq.Background.Services;
using AInq.Background.Tasks;
using AInq.Background.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Managers
{

internal sealed class ConveyorManager<TData, TResult> : TaskManager<IConveyorMachine<TData, TResult>>, IConveyor<TData, TResult>
{
    private readonly int _maxAttempts;

    public ConveyorManager(int maxAttempts = int.MaxValue)
        => _maxAttempts = Math.Max(maxAttempts, 1);

    int IConveyor<TData, TResult>.MaxAttempts => _maxAttempts;

    Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
    {
        var dataWrapper = new ConveyorDataWrapper<TData, TResult>(data, cancellation, FixAttempts(attemptsCount));
        AddTask(dataWrapper);
        return dataWrapper.Result;
    }

    private int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));
}

}
