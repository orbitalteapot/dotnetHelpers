using System;
using System.Threading.Tasks;
using NUnit.Framework;
using OrbitalHelpers;
using OrbitalHelpers.Observable;
using OrbitalHelpers.RestApi;
using OrbitalHelpers.RestApi.Models.Met;

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
        public async Task GetMetForecast()
        {
            var metRadarImageNorway = await GetApiData.GetMetForecast(50,50.4f,7.3f);
            Assert.IsNotNull(metRadarImageNorway);
        }

        [Test]
        public async Task GetDataFromApi()
        {
            var fromJson = await GetApiData<Yr>.GetTypeFromJson("https://api.met.no/weatherapi/locationforecast/2.0/compact?altitude=20&lat=50&lon=7");
            Assert.IsNotNull(fromJson);
        }

        [Test]
        public async Task GetDataFromApiInterval()
        {
            var dataOnInterval = new GetApiDataOnInterval<Yr>("https://api.met.no/weatherapi/locationforecast/2.0/compact?altitude=20&lat=50&lon=7", "Interval1", TimeSpan.FromSeconds(1));
            dataOnInterval.DataReceived += DataOnInterval_DataReceived;
            dataOnInterval.Start();
            
            await Task.Delay(2000);
            var testdata = _data as GetApiDataOnInterval<Yr>.EventOfTypeT;
            dataOnInterval.Stop();
            Assert.IsNotNull(testdata);
        }

        private void DataOnInterval_DataReceived(object? sender, GetApiDataOnInterval<Yr>.EventOfTypeT? e)
        {
            _data = e;
        }

        [Test]
        public void TestMethod1()
        {
            var modelTracker = new ModelTracker<int>();
            var modelReporter = new ModelReporter<int>("MyTracker");
            modelReporter.Subscribe(modelTracker);
            modelReporter.OnUpdatedTracker += ModelReporter_OnUpdatedTracker;
            var data = 5;
            modelTracker.Update(data);
            modelTracker.UpdateWithEvent(data);
            modelTracker.EndTransmission();
        }

        private void ModelReporter_OnUpdatedTracker(object sender, ReporterEventArgs<int> e)
        {
            Console.WriteLine(e.Value);
            Assert.IsTrue(e.Name == "MyTracker" && e.Value == 5);
        }
    }
}