﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Net.Http;
using System.Text.Json;
using OrbitalTeapot.RestApi.Models.Met;

namespace OrbitalTeapot.RestApi
{
    /// <summary>
    /// Get API data of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class GetClassFromJson<T>
    {
        /// <summary>
        /// Returns Deserialized Json to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns>Result of type T</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<T> GetTypeFromJson(string url, string userAgent = "PostmanRuntime/7.29.0")
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            var response = await client.GetAsync(url);
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync()) ?? throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Get data from preconfigured API
    /// </summary>
    public static class GetApiDataCollection
    {
        /// <summary>
        /// Get Weather information
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<Yr> GetMetForecast(int altitude, float lat, float lon, string url = "https://api.met.no/weatherapi/locationforecast/2.0/compact?altitude=")
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PostmanRuntime/7.29.0");
            var response = await client.GetAsync($"{url}{altitude}&lat={lat}&lon={lon}");
            return await JsonSerializer.DeserializeAsync<Yr>(await response.Content.ReadAsStreamAsync()) ?? throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns radar image of norway, uses image class (windows only)
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<Image> GetMetRadarImageNorway(string url = "https://api.met.no/weatherapi/radar/2.0/?area=norway&type=5level_reflectivity")
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PostmanRuntime/7.29.0");
            var response = await client.GetAsync(url);
            return Image.FromStream(await response.Content.ReadAsStreamAsync());
        }
    }

    /// <summary>
    /// Get API data of type T with specified interval
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GetClassFromJsonInterval<T> where T : class
    {
        private string UserAgent;
        private string Url { get; }
        private string Name { get; }
        private TimeSpan TimeSpan { get; }
        private string AuthorizationHeaderName { get; }
        private string AuthorizationHeaderValue { get; }
        private readonly System.Timers.Timer _timer = new System.Timers.Timer();

        /// <summary>
        /// Get API data
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="timeSpan"></param>
        /// <param name="authorizationHeaderName"></param>
        /// <param name="authorizationHeaderValue"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public GetClassFromJsonInterval(string url, string name, TimeSpan timeSpan, string authorizationHeaderName = "", string authorizationHeaderValue = "", string userAgent = "PostmanRuntime/7.29.0")
        {
            UserAgent = userAgent;
            Url = url;
            Name = name;
            TimeSpan = timeSpan;
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
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            if (AuthorizationHeaderName.Length > 0) client.DefaultRequestHeaders.Add(AuthorizationHeaderName, AuthorizationHeaderValue);
            var response = await client.GetAsync(Url);
            OnDataReceived(new EventOfTypeT(await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync()) ?? throw new InvalidOperationException(), Name));
        }

        private void OnDataReceived(EventOfTypeT e)
        {
            var handler = DataReceived ?? throw new InvalidOperationException();
            handler?.Invoke(this, e);
        }
    }
}
