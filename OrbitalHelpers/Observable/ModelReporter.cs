using System;

namespace OrbitalHelpers.Observable
{
    public class ModelReporter<T> : ICustomObserver<T> where T : struct
    {
        private IDisposable _unSubscriber;
        public event EventHandler<ReporterEventArgs<T>> OnUpdatedTracker;
        private string Name { get; }

        public ModelReporter(string name)
        {
            this.Name = name;
        }

        public virtual void Subscribe(ICustomObservable<T> provider)
        {
            if (provider != null) _unSubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("The Location Tracker has completed transmitting data to {0}.", this.Name);
            this.Unsubscribe();
        }

        public virtual void OnError(Exception e)
        {
            Console.WriteLine("{0}: The location cannot be determined.", this.Name);
        }

        public virtual void OnNext(T value)
        {
            Console.WriteLine("{0}", this.Name);
        }

        public void EventOnUpdate(T value)
        {
            OnUpdatedTracker?.Invoke(this, new ReporterEventArgs<T> { Name = this.Name, Value = value });
        }
        public virtual void Unsubscribe()
        {
            _unSubscriber.Dispose();
        }
    }

    public class ReporterEventArgs<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }
}