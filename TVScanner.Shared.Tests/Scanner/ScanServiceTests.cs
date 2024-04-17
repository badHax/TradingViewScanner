namespace TVScanner.Shared.Tests.Scanner
{
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using TVScanner.Shared.Scanner;

    [TestFixture]
    public class ScanServiceTests
    {
        private ScanService _testClass;
        private IHttpClientFactory _httpClientFactory;

        private const string _testSymbol = "BTCUSDT";

        [SetUp]
        public void SetUp()
        {
            var httpClient = new HttpClient(new TestHttpMessageHandler());
            httpClient.BaseAddress = new Uri(TestHelper.ScanUrl);

            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _httpClientFactory.CreateClient(nameof(ScanService)).Returns(httpClient);

            _testClass = new ScanService(_httpClientFactory);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new ScanService(_httpClientFactory);

            // Assert
            instance.ShouldNotBeNull();
        }

        [Test]
        public void CannotConstructWithNullHttpClient()
        {
            Should.Throw<ArgumentNullException>(() => new ScanService(default(IHttpClientFactory)));
        }

        [Test]
        public async Task CanCallGetFilteredResult()
        {
            // Arrange
            var filter = new ScanFilter(ScanType.RelativeVolume);

            // Act
            var result = await _testClass.GetFilteredResult(filter);

            // Assert
            result.ShouldNotBeNull();
            result.Length.ShouldBe(1);
            result[0].BaseCurrencyLogoId.ShouldBe("BaseCurrencyLogoId");
            result[0].CurrencyLogoId.ShouldBe("CurrencyLogoId");
            result[0].Name.ShouldBe(_testSymbol);
            result[0].Close.ShouldBe(1.0f);
            result[0].Change.ShouldBe(2.0f);
            result[0].ATR.ShouldBe(3.0f);
            result[0].RelativeVolume10DCalc.ShouldBe(4.0f);
            result[0].Description.ShouldBe("Description");
            result[0].Type.ShouldBe("Type");
            result[0].Subtype.ShouldBe("Subtype");
            result[0].UpdateMode.ShouldBe("UpdateMode");
            result[0].Exchange.ShouldBe("Exchange");
            result[0].Pricescale.ShouldBe(5.0f);
            result[0].Minmov.ShouldBe(6.0f);
            result[0].Fractional.ShouldBeTrue();
            result[0].Minmove2.ShouldBe(7.0f);
            result[0].LastUpdated.ShouldBeInRange(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1));
            result[0].ScanFilter.ShouldBe(ScanType.RelativeVolume);
        }

        [Test]
        public async Task CannotCallGetFilteredResultWithNullFilter()
        {
            await Should.ThrowAsync<ArgumentNullException>(() => _testClass.GetFilteredResult(default(ScanFilter)));
        }
    }
}