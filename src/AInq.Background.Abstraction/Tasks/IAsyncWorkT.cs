﻿// Copyright 2020-2022 Anton Andryushchenko
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

namespace AInq.Background.Tasks;

/// <summary> Interface for asynchronous work with result of type <typeparamref name="TResult" /> </summary>
/// <typeparam name="TResult"> Work result type </typeparam>
public interface IAsyncWork<TResult>
{
    /// <summary> Asynchronous work action </summary>
    /// <param name="serviceProvider"> Service provider instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    Task<TResult> DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation = default);
}
