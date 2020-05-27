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
using System;
using System.Collections.Generic;

namespace AInq.Support.Background
{
    public static class BackgroundServicesDependencyInjection
    {
        public static IServiceCollection AddWorkQueue(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddPriorityWorkQueue(this IServiceCollection services, int maxPriority)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddDataConveyor<TData, TResult>(this IServiceCollection services, IDataConveyorMachine<TData, TResult> conveyorMachine)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddDataConveyor<TDataConveyorMachine, TData, TResult>(this IServiceCollection services, int maxActiveMachines = 1)
            where TDataConveyorMachine:IDataConveyorMachine<TData, TResult>
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddDataConveyor<TData, TResult>(this IServiceCollection services, Func<IServiceProvider, IDataConveyorMachine<TData, TResult>> conveyorMachineFabric, int maxActiveMachines = 1)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddDataConveyor<TData, TResult>(this IServiceCollection services, IEnumerable<IDataConveyorMachine<TData, TResult>> conveyorMachines)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, IDataConveyorMachine<TData, TResult> conveyorMachine)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddPriorityDataConveyor<TDataConveyorMachine, TData, TResult>(this IServiceCollection services, int maxPriority, int maxActiveMachines = 1)
            where TDataConveyorMachine:IDataConveyorMachine<TData, TResult>
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, Func<IServiceProvider, IDataConveyorMachine<TData, TResult>> conveyorMachineFabric, int maxActiveMachines = 1)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, IEnumerable<IDataConveyorMachine<TData, TResult>> conveyorMachines)
        {
            throw new NotImplementedException();
        }
    }
}