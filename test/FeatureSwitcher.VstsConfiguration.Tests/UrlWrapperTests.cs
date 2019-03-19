using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FeatureSwitcher.VstsConfiguration.Tests
{
    [TestClass]
    public class UrlWrapperTests
    {
        [DataTestMethod]
        [DataRow("https://alegristg.visualstudio.com/FeatureMaster/", "FeatureMaster")]
        [DataRow("https://alegristg.visualstudio.com/FeatureMaster", "FeatureMaster")]
        [DataRow("http://104.40.219.231:8080/tfs/DefaultCollection/FeatureFlags/", "FeatureFlags")]
        [DataRow("http://tfs2017:8080/tfs/DefaultCollection/FeatureFlags", "FeatureFlags")]
        [DataRow("https://tfs2017/tfs/CustomCollection/FeatureFlags", "FeatureFlags")]
        public void UrlWrapperTests_can_perse_projectName(string input, string projectName)
        {
            var sut = new UrlWrapper(new Uri(input));

            sut.ProjectName.Should().Be(projectName);
        }

        [DataTestMethod]
        [DataRow("https://alegristg.visualstudio.com/FeatureMaster/", "https://alegristg.visualstudio.com")]
        [DataRow("https://alegristg.visualstudio.com/FeatureMaster", "https://alegristg.visualstudio.com")]
        [DataRow("http://104.40.219.231:8080/tfs/DefaultCollection/FeatureFlags/", "http://104.40.219.231:8080/tfs/DefaultCollection")]
        [DataRow("http://104.40.219.231/tfs/DefaultCollection/FeatureFlags", "http://104.40.219.231/tfs/DefaultCollection")]
        [DataRow("https://tfs2017/tfs/CustomCollection/FeatureFlags", "https://tfs2017/tfs/CustomCollection")]
        public void UrlWrapperTests_can_parse_collection(string input, string collection)
        {
            var sut = new UrlWrapper(new Uri(input));

            sut.ProjectCollectionUri.Should().Be(new Uri(collection));
        }
    }
}
