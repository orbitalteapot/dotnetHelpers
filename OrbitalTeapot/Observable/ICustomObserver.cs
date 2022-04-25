using System;

namespace OrbitalTeapot.Observable
{
    public interface ICustomObserver<in T> : IObserver<T>
    {
        void EventOnUpdate(T value);
    }
}