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

using AInq.Optional;
using System;

namespace AInq.Background.Test
{

public class TestObserver<T> : IObserver<Try<T>>
{
    public void OnCompleted()
        => Console.WriteLine($"{DateTime.Now:T}\tObservation completed");

    public void OnError(Exception error)
        => Console.WriteLine($"{DateTime.Now:T}\tObservation error\t{error}");

    public void OnNext(Try<T> value)
        => Console.WriteLine($"{DateTime.Now:T}\tObservation test\t{(value.Success ? $"Result {value.Value}" : $"Error {value.Error}")}");
}

}
