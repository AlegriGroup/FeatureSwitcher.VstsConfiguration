using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace FeatureSwitcher.VstsConfiguration.Tests
{
    [TestClass]
    public class VstsConfigTests
    {
        [TestMethod]
        public void VstsConfig_loads_data_async()
        {
            var clientMock = new Mock<IVstsClient>();

            clientMock.Setup(x => x.GetAsync()).ReturnsAsync(new Dictionary<string, string>
            {
                { "Feature1", "true" },
                { "Feature2", "false" }
            });


            var sut = new VstsConfig(new VstsSettings(), clientMock.Object);

            sut.SetupAsync().GetAwaiter().GetResult();

            sut.Cache.Count.Should().Be(2);
        }

        [TestMethod]
        public void VstsConfig_IsEnabled_returns_null()
        {
            var sut = new VstsConfig(new VstsSettings(), null);

            sut.IsEnabled(null).Should().Be(null);
        }

        [TestMethod]
        public void VstsConfig_IsEnabled_returns_value_from_cache()
        {
            var clientMock = new Mock<IVstsClient>();

            clientMock.Setup(x => x.GetAsync()).ReturnsAsync(new Dictionary<string, string>
            {
                { "Feature1", "true" },
                { "Feature2", "false" }
            });


            var sut = new VstsConfig(new VstsSettings(), clientMock.Object);
            sut.SetupAsync().GetAwaiter().GetResult();

            Feature.Name feature1 = new Feature.Name(typeof(Feature1), "Feature1");
            Feature.Name feature2 = new Feature.Name(typeof(Feature2), "Feature2");

            sut.IsEnabled(feature1).Should().BeTrue();
            sut.IsEnabled(feature2).Should().BeFalse();
        }

        [TestMethod]
        public void VstsConfig_IsEnabled_creates_missing_feature()
        {
            var clientMock = new Mock<IVstsClient>();

            clientMock.Setup(x => x.GetAsync()).ReturnsAsync(new Dictionary<string, string>
            {
                { "Feature1", "true" }
            });
            clientMock.Setup(x => x.PutAsync("Feature2", "False")).ReturnsAsync(true);


            var sut = new VstsConfig(new VstsSettings(), clientMock.Object);
            sut.SetupAsync().GetAwaiter().GetResult();

            Feature.Name feature2 = new Feature.Name(typeof(Feature2), "Feature2");

            sut.IsEnabled(feature2).Should().BeFalse();

            clientMock.Verify(x => x.PutAsync("Feature2", "False"));
        }
    }

    public class Feature1 : IFeature { }

    public class Feature2 : IFeature { }
}
