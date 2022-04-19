using System;

namespace OrbitalHelpers.Observable
{
    public interface ICustomObservable<out T>
    {
        IDisposable Subscribe(ICustomObserver<T> observer);
    }
}