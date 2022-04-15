using System;
using System.Linq;
using System.Threading;
using OrbitalHelpers.RestApi;
using OrbitalHelpers.RestApi.Models.Met;

namespace ConsoleTestApp
{
    internal partial class Program
    {
        private static void Main()
        {
            CancellationTokenSource cancellationToken = new();
            var token = cancellationToken.Token;

            //PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            var tt = new GetApiDataOnInterval<Yr>("https://api.met.no/weatherapi/locationforecast/2.0/compact?altitude=20&lat=53&lon=8", "MyData", TimeSpan.FromSeconds(5), token);
            tt.DataReceived += TtDataReceived;
            tt.Start();

            void TtDataReceived(object sender, GetApiDataOnInterval<Yr>.EventOfTypeT e)
            {
                Console.Clear();
                var ff = e.Data.properties.timeseries.Single(a =>
                    a.time <= DateTime.Now && a.time >= DateTime.Now.AddHours(-1));
                Console.WriteLine($"EventName: {e.Name}\nTime: {ff.time} Current: {DateTime.Now}");
                ff.data.instant.details.GetType().GetProperties().ToList().ForEach(p =>
                    Console.WriteLine($"{p.Name}: {p.GetValue(ff.data.instant.details)}"));
            }


            Console.ReadLine();
            tt.DataReceived -= TtDataReceived;
            cancellationToken.Cancel();
        }
    }
}
