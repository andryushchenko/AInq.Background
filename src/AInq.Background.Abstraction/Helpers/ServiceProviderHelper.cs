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

namespace AInq.Background.Helpers;

/// <summary> <see cref="IServiceProvider" /> helper utility </summary>
public static class ServiceProviderHelper
{
    /// <summary> Get service of type <typeparamref name="T" /> from the <see cref="IServiceProvider" /> </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <typeparam name="T"> Service type </typeparam>
    /// <returns> Service instance or NULL if service not exists </returns>
    public static T? Service<T>(this IServiceProvider provider)
        => provider.GetService(typeof(T)) is T service ? service : default;

    /// <summary> Get service of type <typeparamref name="T" /> from the <see cref="IServiceProvider" /> </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <typeparam name="T"> Service type </typeparam>
    /// <returns> Service instance </returns>
    /// <exception cref="InvalidOperationException"> Thrown if service not exists </exception>
    public static T RequiredService<T>(this IServiceProvider provider)
        => provider.GetService(typeof(T)) is T service ? service : throw new InvalidOperationException("Service not exists");
}
