using System;

namespace OrbitalTeapot.Observable
{
    /// <summary>
    /// ModelReporter of type T, implements the Observer interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelReporter<T> : ICustomObserver<T>
    {
        private IDisposable? _unSubscriber;
        public event EventHandler<ReporterEventArgs<T>>? OnUpdatedTracker;
        private string Name { get; }

        public ModelReporter(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("The observer must be assigned a name.");
            this.Name = name;
        }

        /// <summary>
        /// Subscribe to IObservable of type T
        /// </summary>
        /// <param name="provider"></param>
        public virtual void Subscribe(ICustomObservable<T>? provider)
        {
            if (provider != null) _unSubscriber = provider.Subscribe(this);
        }

        /// <summary>
        /// Method called on end of subscription, this is called by the IObservable and Unsubscribes the IObserver
        /// </summary>
        public virtual void OnCompleted()
        {
            Console.WriteLine("The Location Tracker has completed transmitting data to {0}.", this.Name);
            this.Unsubscribe();
        }

        /// <summary>
        /// Method called of an error occurs, this is called by the IObservable
        /// </summary>
        public virtual void OnError(Exception e)
        {
            Console.WriteLine("Name: {1} Error: {0}", e.Message ,this.Name);
            throw new Exception(e.Message, e);
        }

        /// <summary>
        /// Method called by the IObservable when new data is revived
        /// </summary>
        /// <param name="value"></param>
        public virtual void OnNext(T value)
        {
            Console.WriteLine("{0}", this.Name);
        }
        /// <summary>
        /// Method called by the IObservable when new data is revived with Event
        /// </summary>
        /// <param name="value"></param>
        public void EventOnUpdate(T value)
        {
            OnUpdatedTracker?.Invoke(this, new ReporterEventArgs<T> { Name = this.Name, Value = value });
        }
        /// <summary>
        /// Unsubscribe to the IObservable
        /// </summary>
        protected virtual void Unsubscribe()
        {
            _unSubscriber?.Dispose();
        }
    }

    /// <summary>
    /// EventArgs for EventOnUpdate(T value)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReporterEventArgs<T>
    {
        public string? Name { get; set; }
        public T Value { get; set; } = default!;
    }
}