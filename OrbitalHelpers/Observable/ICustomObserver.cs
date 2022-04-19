using System;

namespace OrbitalHelpers.Observable
{
    public interface ICustomObserver<in T> : IObserver<T>
    {
        void EventOnUpdate(T value);
    }
}