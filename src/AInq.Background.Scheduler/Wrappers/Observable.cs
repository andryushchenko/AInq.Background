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
using System.Linq;

namespace AInq.Background.Wrappers
{

internal class Observable<TResult> : IObservable<TResult>
{
    private readonly IList<IObserver<TResult>> _observers = new List<IObserver<TResult>>();

    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        if (!_observers.Contains(observer ?? throw new ArgumentNullException(nameof(observer))))
            _observers.Add(observer);
        return new Subscriber(_observers, observer);
    }

    public void Next(TResult item)
    {
        foreach (var observer in _observers.ToArray())
            if (_observers.Contains(observer))
                observer.OnNext(item);
    }

    public void Error(Exception ex)
    {
        foreach (var observer in _observers.ToArray())
            if (_observers.Contains(observer))
                observer.OnError(ex);
    }

    public void Complete()
    {
        foreach (var observer in _observers.ToArray())
            if (_observers.Contains(observer))
                observer.OnCompleted();
        _observers.Clear();
    }

    private class Subscriber : IDisposable
    {
        private readonly IObserver<TResult> _observer;
        private readonly IList<IObserver<TResult>> _observers;

        internal Subscriber(IList<IObserver<TResult>> observers, IObserver<TResult> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        void IDisposable.Dispose()
            => _observers.Remove(_observer);
    }
}

}
