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

internal static class ProcessorFactory
{
    public static ITaskProcessor<object?, TMetadata> CreateNullProcessor<TMetadata>(int maxParallelTasks = 1)
        => maxParallelTasks <= 1
            ? new SingleNullProcessor<TMetadata>() as ITaskProcessor<object?, TMetadata>
            : new MultipleNullProcessor<TMetadata>(maxParallelTasks);

    public static ITaskProcessor<TArgument, TMetadata> CreateProcessor<TArgument, TMetadata>(TArgument argument)
        => new SingleStaticProcessor<TArgument, TMetadata>(argument ?? throw new ArgumentNullException(nameof(argument)));

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

    public static ITaskProcessor<TArgument, TMetadata> CreateProcessor<TArgument, TMetadata>(IServiceProvider provider,
        Func<IServiceProvider, TArgument> argumentFactory, ReuseStrategy strategy, int maxArgumentsCount = 1)
    {
        if (argumentFactory == null)
            throw new ArgumentNullException(nameof(argumentFactory));
        if (strategy == ReuseStrategy.Static && provider == null)
            throw new ArgumentNullException(nameof(provider));
        return strategy switch
        {
            ReuseStrategy.Static => CreateProcessor<TArgument, TMetadata>(Enumerable
                                                                          .Repeat(0, maxArgumentsCount)
                                                                          .Select(_ => argumentFactory.Invoke(provider))),
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
