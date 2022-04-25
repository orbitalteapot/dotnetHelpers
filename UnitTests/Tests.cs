using System;
using System.Threading.Tasks;
using NUnit.Framework;
using OrbitalTeapot.Helpers;
using OrbitalTeapot.Observable;
using OrbitalTeapot.RestApi;
using OrbitalTeapot.RestApi.Models.Met;

namespace UnitTests
{
    public class Tests
    {
        private object? _data;
        [SetUp]
        public void Setup()
        {
            _data = new object();
        }

        [Test]
        public void StringExtensionsTests()
        {
            const string myString = "This is a test string";
            var foundChar = myString.ContainsChar('T');
            var lengthString = myString.ContainsNumberOfChar('t');
            Assert.IsTrue(foundChar);
            Assert.AreEqual(3, lengthString);
        }

        // Unsupported on non windows platforms
        //[Test]
        //public async Task GetRadarImage()
        //{
        //    var metRadarImageNorway = await GetApiData.GetMetRadarImageNorway();
        //    Assert.IsNotNull(metRadarImageNorway);
        //}

        [Test]
        public static async Task TestGetMetForecast()
        {
            var metForecast = await GetApiDataCollection.GetMetForecast(50,50.4f,7.3f);
            Assert.IsNotNull(metForecast);
        }

        [Test]
        public async Task GetDataFromApi()
        {
            var fromJson = await GetClassFromJson<Yr>.GetTypeFromJson("https://api.met.no/weatherapi/locationforecast/2.0/compact?altitude=20&lat=50&lon=7");
            Assert.IsNotNull(fromJson);
        }

        [Test]
        public async Task GetDataFromApiInterval()
        {
            var dataOnInterval = new GetClassFromJsonInterval<Yr>("https://api.met.no/weatherapi/locationforecast/2.0/compact?altitude=20&lat=50&lon=7", "Interval1", TimeSpan.FromSeconds(1));
            dataOnInterval.DataReceived += DataOnInterval_DataReceived;
            dataOnInterval.Start();
            
            await Task.Delay(2000);
            var testdata = _data as GetClassFromJsonInterval<Yr>.EventOfTypeT;
            dataOnInterval.Stop();
            Assert.IsNotNull(testdata);
        }

        private void DataOnInterval_DataReceived(object? sender, GetClassFromJsonInterval<Yr>.EventOfTypeT? e)
        {
            _data = e;
        }

        [Test]
        public void CheckObservableNullHandle()
        {
            var modelTracker = new ModelTracker<object>();
            var modelReporter = new ModelReporter<object>("MyTracker");
            modelReporter.Subscribe(modelTracker);

            var obj = new object();
            obj = null;
            try
            {
                modelTracker.Update(obj);
            }
            catch
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public void CheckObservable()
        {
            var modelTracker = new ModelTracker<object>();
            var modelReporter = new ModelReporter<object>("MyTracker");
            modelReporter.Subscribe(modelTracker);
            modelReporter.OnUpdatedTracker += ModelReporter_OnUpdatedTracker;
            var data = new Yr
            {
                type = default,
                geometry = default,
                properties = default
            };


            try
            {
                modelTracker.Update(data);
            }
            catch
            {
                Assert.Pass("Handles NULL input correctly");
            }
        }

        private void ModelReporter_OnUpdatedTracker(object? sender, ReporterEventArgs<object> e)
        {
            Console.WriteLine(e.Value);
            Assert.IsTrue(e.Name == "MyTracker" && e.Value != null);
            Assert.Pass();
        }


    }
}