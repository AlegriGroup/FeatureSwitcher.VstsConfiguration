using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FeatureSwitcher.VstsConfiguration.Tests
{
    [TestClass]
    public class VstsClientTests
    {
        [TestMethod]
        public void VstsClient_validates_input()
        {
            Action act = () =>
            {
                new VstsClient(null, "***");
            };

            act.Should().Throw<ArgumentNullException>();

            act = () =>
            {
                new VstsClient(new Uri("http://localhost"), null);
            };

            act.Should().Throw<ArgumentNullException>();

            act = () =>
            {
                new VstsClient(new Uri("http://localhost"), "***");
            };

            act.Should().Throw<ArgumentException>().WithMessage("Invalid URL: The URL must have the format: 'https://<account>.visualstudio.com/<project>'.*");
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void VstsClient_can_create_feature_with_default_config()
        {
            var sut = new VstsClient(Settings.Url, IntegrationTests.GetPAT());

            var task = sut.PutAsync($"Test-Flag-{Guid.NewGuid()}", "true");
            task.GetAwaiter().GetResult();


            var result = task.Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id > 0);
            Assert.AreEqual("Task", result.Fields["System.WorkItemType"]);
            Assert.AreEqual("true", result.Fields["System.Description"]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void VstsClient_can_create_feature_with_special_config()
        {

            var config = new VstsSettings
            {
                WorkItemType = "FeatureFlag",
                ValueField = "FeatureFlag.Value"
            };

            config.AdditionalFields.Add("Microsoft.VSTS.Common.Priority", "1");

            var sut = new VstsClient(Settings.Url, IntegrationTests.GetPAT(), config);

            var task = sut.PutAsync($"Test-Flag-{Guid.NewGuid()}", "true");
            task.GetAwaiter().GetResult(); ;


            var result = task.Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id > 0);
            Assert.AreEqual("FeatureFlag", result.Fields["System.WorkItemType"]);
            Assert.AreEqual("true", result.Fields[config.ValueField]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void VstsClient_can_get_features_with_default_config()
        {
            var settings = new VstsSettings();
            settings.AddEnvironment(Guid.NewGuid().ToString());

            var sut = new VstsClient(Settings.Url, IntegrationTests.GetPAT(), settings);

            // make sure on eitem exists
            sut.PutAsync($"Test-Flag-{Guid.NewGuid()}", "true").GetAwaiter().GetResult();

            var task = sut.GetAsync();
            task.GetAwaiter().GetResult(); ;


            var result = task.Result;

            Assert.IsTrue(result.Any());

        }

        [TestMethod]
        [TestCategory("Integration")]
        public void VstsClient_can_get_empty_features()
        {

            var settings = new VstsSettings { AdditionalQueryFilter = "and [System.Tags] Contains 'ThisWillNotExist'" };

            var sut = new VstsClient(Settings.Url, IntegrationTests.GetPAT(), settings);

            var task = sut.GetAsync();
            task.GetAwaiter().GetResult(); ;


            var result = task.Result;

            Assert.IsFalse(result.Any());

        }

        [TestMethod]
        [TestCategory("Integration")]
        public void VstsClient_can_get_features_with_special_config()
        {
            var config = new VstsSettings
            {
                WorkItemType = "FeatureFlag",
                ValueField = "FeatureFlag.Value",
                AdditionalQueryFilter = "and [System.Tags] Contains 'FeatureFlag' and [Microsoft.VSTS.Common.Priority] = 1"
            };

            var sut = new VstsClient(Settings.Url, IntegrationTests.GetPAT(), config);

            var task = sut.GetAsync();
            task.GetAwaiter().GetResult(); ;


            var result = task.Result;

            Assert.IsTrue(result.Any());

        }
    }
}
