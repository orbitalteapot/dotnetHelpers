using System;
using System.Collections.Generic;

namespace OrbitalTeapot.Observable
{
    /// <summary>
    /// ModelTracker of type T, implements IObservable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelTracker<T> : ICustomObservable<T>
    {
        private readonly List<ICustomObserver<T>> _observers;

        public ModelTracker()
        {
            _observers = new List<ICustomObserver<T>>();
        }

        /// <summary>
        /// Do not USE!
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(ICustomObserver<T> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new UnSubscriber(_observers, observer);
        }

        private class UnSubscriber : IDisposable
        {
            private readonly List<ICustomObserver<T>> _observers;
            private readonly ICustomObserver<T> _observer;

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

        /// <summary>
        /// Updates the value of T and notifies OnNext
        /// </summary>
        /// <param name="loc"></param>
        public void Update(T loc)
        {
            foreach (var observer in _observers)
            {
                if (loc == null)
                    observer.OnError(new Exception("Value is null"));
                else
                    observer.OnNext(loc);
            }
        }
        /// <summary>
        /// Updates the value of T and notifies EventOnUpdate
        /// </summary>
        /// <param name="loc"></param>
        public void UpdateWithEvent(T loc)
        {
            foreach (var observer in _observers)
            {
                if (loc == null)
                    observer.OnError(new Exception("Value is null"));
                else
                    observer.EventOnUpdate(loc);
            }
        }

        /// <summary>
        /// Removes all Subscribers
        /// </summary>
        public void EndTransmission()
        {
            foreach (var observer in _observers.ToArray())
                if (_observers.Contains(observer))
                    observer.OnCompleted();

            _observers.Clear();
        }
    }
}