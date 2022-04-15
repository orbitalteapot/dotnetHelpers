using System;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Net.Http;
using System.Text.Json;

namespace OrbitalHelpers.RestApi
{
    /// <summary>
    /// Get API data of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class GetApiData<T>
    {
        /// <summary>
        /// Returns Deserialized Json to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns>Result of type T</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<T> GetTypeFromJson(string url)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PostmanRuntime/7.29.0");
            var response = await client.GetAsync(url);
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync()) ?? throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Get API data of type T with specified interval
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GetApiDataOnInterval<T> where T : class
    {
        private string Url { get; }
        private string Name { get; }
        private TimeSpan TimeSpan { get; }
        private CancellationToken CancellationToken { get; }
        private string AuthorizationHeaderName { get; }
        private string AuthorizationHeaderValue { get; }
        private readonly System.Timers.Timer _timer = new System.Timers.Timer();

        /// <summary>
        /// Get API data
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="timeSpan"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="authorizationHeaderName"></param>
        /// <param name="authorizationHeaderValue"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public GetApiDataOnInterval(string url, string name, TimeSpan timeSpan, CancellationToken cancellationToken = default, string authorizationHeaderName = "", string authorizationHeaderValue = "")
        {
            Url = url;
            Name = name;
            TimeSpan = timeSpan;
            CancellationToken = cancellationToken;
            AuthorizationHeaderName = authorizationHeaderName;
            AuthorizationHeaderValue = authorizationHeaderValue;
        }

        public event EventHandler<EventOfTypeT>? DataReceived;

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

        /// <summary>
        /// Start receiving events
        /// </summary>
        public void Start()
        {
            if (DataReceived == null) return;
            _timer.Interval = TimeSpan.TotalMilliseconds;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += EventFromTimer;
        }

        /// <summary>
        /// Stop receiving events
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
            _timer.Enabled = false;
            _timer.Elapsed -= EventFromTimer;
        }

        private async void EventFromTimer(object sender, ElapsedEventArgs e)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PostmanRuntime/7.29.0");
            if (AuthorizationHeaderName.Length > 0) client.DefaultRequestHeaders.Add(AuthorizationHeaderName, AuthorizationHeaderValue);
            var response = await client.GetAsync(Url, CancellationToken);
            OnDataReceived(new EventOfTypeT(await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), cancellationToken: CancellationToken) ?? throw new InvalidOperationException(), Name));
        }

        private void OnDataReceived(EventOfTypeT e)
        {
            var handler = DataReceived ?? throw new InvalidOperationException();
            handler?.Invoke(this, e);
        }
    }
}
