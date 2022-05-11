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

internal sealed class Subject<T> : IObservable<T>
{
    private Maybe<Subscriber[]> _state = Maybe.None<Subscriber[]>();

    IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
    {
        var subscriber = new Subscriber(observer ?? throw new ArgumentNullException(nameof(observer)), this);
        while (true)
        {
            var state = Volatile.Read(ref _state);
            if (!state.HasValue)
            {
                observer.OnCompleted();
                return new Subscriber(observer, null);
            }
            var subscribers = new Subscriber[state.Value.Length + 1];
            Array.Copy(state.Value, subscribers, state.Value.Length);
            subscribers[state.Value.Length] = subscriber;
            if (Interlocked.CompareExchange(ref _state, Maybe.Value(subscribers), state) == state)
                return subscriber;
        }
    }

    public void OnCompleted()
    {
        while (true)
        {
            var state = Volatile.Read(ref _state);
            if (!state.HasValue) return;
            if (Interlocked.CompareExchange(ref _state, Maybe.None<Subscriber[]>(), state) != state)
                continue;
            foreach (var subscriber in state.Value)
                subscriber.OnCompleted();
            return;
        }
    }

    public void OnNext(T value)
        => Volatile.Read(ref _state)
                   .Do((subscribers, argument) =>
                       {
                           foreach (var subscriber in subscribers)
                               subscriber.OnNext(argument);
                       },
                       value);

    private void Unsubscribe(Subscriber subscriber)
    {
        while (true)
        {
            var state = Volatile.Read(ref _state);
            if (state.SelectOrDefault(subscribers => subscribers.Length == 0, true)) return;
            var index = Array.IndexOf(state.Value, subscriber);
            if (index < 0) return;
            Subscriber[] subscribers;
            if (state.Value.Length == 1)
            {
                subscribers = Array.Empty<Subscriber>();
            }
            else
            {
                subscribers = new Subscriber[state.Value.Length - 1];
                Array.Copy(state.Value, subscribers, index);
                Array.Copy(state.Value, index + 1, subscribers, index, state.Value.Length - index - 1);
            }
            if (Interlocked.CompareExchange(ref _state, Maybe.Value(subscribers), state) == state) return;
        }
    }

    private sealed class Subscriber : IDisposable
    {
        private readonly IObserver<T> _observer;
        private Subject<T>? _owner;

        public Subscriber(IObserver<T> observer, Subject<T>? owner)
        {
            _observer = observer;
            _owner = owner;
        }

        void IDisposable.Dispose()
        {
            var owner = Interlocked.Exchange(ref _owner, null);
            owner?.Unsubscribe(this);
        }

        public void OnCompleted()
            => _observer.OnCompleted();

        public void OnNext(T value)
            => _observer.OnNext(value);
    }
}
