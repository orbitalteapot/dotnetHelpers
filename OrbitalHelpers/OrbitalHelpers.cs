using System;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Net.Http;
using System.Text.Json;

namespace OrbitalHelpers
{
    public static class GetAPIData<T>
    {
        /// <summary>
        /// Returns Deserialized Json to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns>T</returns>
        /// <exception cref="NullReferenceException"
        public static async Task<T> GetTypeFromJson(string url)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PostmanRuntime/7.29.0");
            var response = await client.GetAsync(url);
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync());
        }
    }

    public class GetAPIDataOnInterval<T> where T : class
    {
        private string Url { get; }
        private string Name { get; }
        private TimeSpan TimeSpan { get; }
        private CancellationToken CancellationToken { get; }
        private string AuthorizationHeaderName { get; }
        private string AuthorizationHeaderValue { get; }

        readonly System.Timers.Timer Timer = new System.Timers.Timer();

        public GetAPIDataOnInterval(string url, string name, TimeSpan timeSpan, CancellationToken cancellationToken = default, string authorizationHeaderName = "", string authorizationHeaderValue = "")
        {
            Url = url;
            Name = name;
            TimeSpan = timeSpan;
            CancellationToken = cancellationToken;
            AuthorizationHeaderName = authorizationHeaderName;
            AuthorizationHeaderValue = authorizationHeaderValue;
        }

        public event EventHandler<EventOfTypeT>? DataRecived;

        public class EventOfTypeT : EventArgs
        {
            public EventOfTypeT(T data, string name)
            {
                Data = data;
                Name = name;
            }

            public T Data { get; set; }
            public string Name { get; set; }
        }

        public void Start()
        {
            Timer.Interval = TimeSpan.TotalMilliseconds;
            Timer.AutoReset = true;
            Timer.Enabled = true;
            Timer.Start();
            Timer.Elapsed += EventFromTimer;
        }

        public void Stop()
        {
            Timer.Stop();
            Timer.Elapsed -= EventFromTimer;
        }

        private async void EventFromTimer(object sender, ElapsedEventArgs e)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PostmanRuntime/7.29.0");
            if (AuthorizationHeaderName.Length == 0) client.DefaultRequestHeaders.Add(AuthorizationHeaderName, AuthorizationHeaderValue);
            var response = await client.GetAsync(Url);
            OnDataRecived(new EventOfTypeT(await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync()), Name));
        }

        protected virtual void OnDataRecived(EventOfTypeT e)
        {
            EventHandler<EventOfTypeT> handler = DataRecived;
            handler?.Invoke(this, e);
        }
    }
}
