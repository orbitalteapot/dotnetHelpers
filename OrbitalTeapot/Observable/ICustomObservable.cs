using System;

namespace OrbitalTeapot.Observable
{
    public interface ICustomObservable<out T>
    {
        IDisposable? Subscribe(ICustomObserver<T> observer);
    }
}