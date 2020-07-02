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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AInq.Background.Processors
{

/// <summary> Task processor factory class </summary>
public static class ProcessorFactory
{
    /// <summary> Create NULL-argument task processor (used in background work queue) </summary>
    /// <param name="maxParallelTasks"> Max allowed parallel tasks </param>
    /// <typeparam name="TMetadata"> Task metadata type </typeparam>
    /// <returns> Task processor instance </returns>
    public static ITaskProcessor<object?, TMetadata> CreateNullProcessor<TMetadata>(int maxParallelTasks = 1)
        => maxParallelTasks <= 1
            ? new SingleNullProcessor<TMetadata>() as ITaskProcessor<object?, TMetadata>
            : new MultipleNullProcessor<TMetadata>(maxParallelTasks);

    /// <summary> Create task processor with single static argument </summary>
    /// <param name="argument"> Task argument </param>
    /// <typeparam name="TArgument"> Task argument type </typeparam>
    /// <typeparam name="TMetadata"> Task metadata type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="argument"/> is NULL </exception>
    /// <returns> Task processor instance </returns>
    public static ITaskProcessor<TArgument, TMetadata> CreateProcessor<TArgument, TMetadata>(TArgument argument)
        => new SingleStaticProcessor<TArgument, TMetadata>(argument ?? throw new ArgumentNullException(nameof(argument)));

    /// <summary> Create task processor with static arguments </summary>
    /// <param name="arguments"> Task arguments collection </param>
    /// <typeparam name="TArgument"> Task argument type </typeparam>
    /// <typeparam name="TMetadata"> Task metadata type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="arguments"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="arguments"/> is empty collection </exception>
    /// <returns> Task processor instance </returns>
    public static ITaskProcessor<TArgument, TMetadata> CreateProcessor<TArgument, TMetadata>(IEnumerable<TArgument> arguments)
    {
        var args = arguments?.Where(argument => argument != null).ToList() ?? throw new ArgumentNullException(nameof(arguments));
        return args.Count switch
        {
            0 => throw new ArgumentException("Empty collection", nameof(arguments)),
            1 => new SingleStaticProcessor<TArgument, TMetadata>(args.First()),
            _ => new MultipleStaticProcessor<TArgument, TMetadata>(args)
        };
    }

    /// <summary> Create task processor with given argument reuse strategy </summary>
    /// <param name="argumentFactory"> Argument factory </param>
    /// <param name="strategy"> Argument reuse strategy </param>
    /// <param name="provider"> Service provider instance (used only for <see cref="ReuseStrategy.Static"/> strategy) </param>
    /// <param name="maxArgumentsCount"> Max allowed argument instances </param>
    /// <typeparam name="TArgument"> Task argument type </typeparam>
    /// <typeparam name="TMetadata"> Task metadata type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="argumentFactory"/> or <paramref name="provider"/> (if used) is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    /// <returns> Task processor instance </returns>
    public static ITaskProcessor<TArgument, TMetadata> CreateProcessor<TArgument, TMetadata>(Func<IServiceProvider, TArgument> argumentFactory,
        ReuseStrategy strategy, IServiceProvider? provider = null, int maxArgumentsCount = 1)
    {
        if (argumentFactory == null)
            throw new ArgumentNullException(nameof(argumentFactory));
        if (strategy == ReuseStrategy.Static && provider == null)
            throw new ArgumentNullException(nameof(provider));
        return strategy switch
        {
            ReuseStrategy.Static => CreateProcessor<TArgument, TMetadata>(Enumerable
                                                                          .Repeat(0, maxArgumentsCount)
                                                                          .Select(_ => argumentFactory.Invoke(provider!))),
            ReuseStrategy.Reuse => maxArgumentsCount <= 1
                ? new SingleReusableProcessor<TArgument, TMetadata>(argumentFactory) as ITaskProcessor<TArgument, TMetadata>
                : new MultipleReusableProcessor<TArgument, TMetadata>(argumentFactory, maxArgumentsCount),
            ReuseStrategy.OneTime => maxArgumentsCount <= 1
                ? new SingleOneTimeProcessor<TArgument, TMetadata>(argumentFactory) as ITaskProcessor<TArgument, TMetadata>
                : new MultipleOneTimeProcessor<TArgument, TMetadata>(argumentFactory, maxArgumentsCount),
            _ => throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy))
        };
    }
}

}
