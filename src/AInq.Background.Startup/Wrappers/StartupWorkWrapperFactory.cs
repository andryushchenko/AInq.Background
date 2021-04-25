// Copyright 2021 Anton Andryushchenko
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

using AInq.Background.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Wrappers
{

internal static class StartupWorkWrapperFactory
{
    public static IStartupWorkWrapper CreateStartupWorkWrapper(IWork work)
        => new StartupWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)));

    public static IStartupWorkWrapper CreateStartupWorkWrapper(IAsyncWork asyncWork)
        => new StartupWorkWrapper(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)));

    public static IStartupWorkWrapper CreateStartupWorkWrapper<T>(IWork<T> work)
        => new StartupWorkWrapper<T>(work ?? throw new ArgumentNullException(nameof(work)));

    public static IStartupWorkWrapper CreateStartupWorkWrapper<T>(IAsyncWork<T> asyncWork)
        => new StartupWorkWrapper<T>(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)));

    private class StartupWorkWrapper : IStartupWorkWrapper
    {
        private readonly IAsyncWork? _asyncWork;
        private readonly IWork? _work;

        internal StartupWorkWrapper(IWork work)
        {
            _work = work;
            _asyncWork = null;
        }

        internal StartupWorkWrapper(IAsyncWork asyncWork)
        {
            _work = null;
            _asyncWork = asyncWork;
        }

        async Task IStartupWorkWrapper.DoWorkAsync(IServiceProvider provider, ILogger? logger, CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested) return;
            try
            {
                if (_asyncWork == null)
                    _work!.DoWork(provider);
                else await _asyncWork.DoWorkAsync(provider, cancellation);
            }
            catch (OperationCanceledException)
            {
                logger?.LogWarning("Startup work {Work} canceled", _asyncWork as object ?? _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing startup work {Work}", _asyncWork as object ?? _work);
            }
        }
    }

    private class StartupWorkWrapper<T> : IStartupWorkWrapper
    {
        private readonly IAsyncWork<T>? _asyncWork;
        private readonly IWork<T>? _work;

        internal StartupWorkWrapper(IWork<T> work)
        {
            _work = work;
            _asyncWork = null;
        }

        internal StartupWorkWrapper(IAsyncWork<T> asyncWork)
        {
            _work = null;
            _asyncWork = asyncWork;
        }

        async Task IStartupWorkWrapper.DoWorkAsync(IServiceProvider provider, ILogger? logger, CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested) return;
            try
            {
                if (_asyncWork == null)
                    _work!.DoWork(provider);
                else await _asyncWork.DoWorkAsync(provider, cancellation);
            }
            catch (OperationCanceledException)
            {
                logger?.LogWarning("Startup work {Work} canceled", _asyncWork as object ?? _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing startup work {Work}", _asyncWork as object ?? _work);
            }
        }
    }
}

}
