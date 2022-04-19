using System;
using System.Collections.Generic;

namespace OrbitalHelpers.Observable
{
    public class ModelTracker<T> : ICustomObservable<T> where T : struct
    {
        private List<ICustomObserver<T>> observers;

        public ModelTracker()
        {
            observers = new List<ICustomObserver<T>>();
        }

        public IDisposable Subscribe(ICustomObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new UnSubscriber(observers, observer);
        }

        private class UnSubscriber : IDisposable
        {
            private List<ICustomObserver<T>> _observers;
            private ICustomObserver<T> _observer;

            public UnSubscriber(List<ICustomObserver<T>> observers, ICustomObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void Update(T? loc)
        {
            foreach (var observer in observers)
            {
                if (!loc.HasValue)
                    observer.OnError(new UnknownDataException());
                else
                    observer.OnNext(loc.Value);
            }
        }

        public void UpdateWithEvent(T? loc)
        {
            foreach (var observer in observers)
            {
                if (!loc.HasValue)
                    observer.OnError(new UnknownDataException());
                else
                    observer.EventOnUpdate(loc.Value);
            }
        }

        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }
    }
}