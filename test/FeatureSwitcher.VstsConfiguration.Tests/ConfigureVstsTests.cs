using FeatureSwitcher.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FeatureSwitcher.VstsConfiguration.Tests
{
    [TestClass]
    public class ConfigureVstsTests
    {
        [TestMethod]
        public void ConfigureVsts_features_can_be_configured_by_VSTS()
        {
            var result = Features.Are.ConfiguredBy.VstsConfig();

            result.Should().BeOfType<ConfigureVsts>();
        }

        [TestMethod]
        public void ConfigureVsts_with_default_config()
        {
            var result = Features.Are.ConfiguredBy.VstsConfig();

            var expected = new VstsSettings();

            var actual = ((ConfigureVsts)result).Settings;

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ConfigureVsts_with_custom_settings()
        {
            var settings = new VstsSettings { NameField = "CustomSettings" };

            var result = Features.Are.ConfiguredBy.VstsConfig().WithSettings(settings);

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.NameField.Should().Be("CustomSettings");
        }

        [TestMethod]
        public void ConfigureVsts_with_VSTS_Url()
        {
            var settings = new VstsSettings { Url = new Uri("http://settingshost") };

            var expected = new Uri("http://explicithost");

            var result = Features.Are.ConfiguredBy
                .VstsConfig()
                .WithSettings(settings)
                .WithVSTSUrl(expected);

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.Url.Should().Be(expected);
        }

        [TestMethod]
        public void ConfigureVsts_with_PrivateAccessToken()
        {
            var settings = new VstsSettings { PrivateAccessToken = "settingspat" };

            var expected = "explicit pat";

            var result = Features.Are.ConfiguredBy
                .VstsConfig()
                .WithSettings(settings)
                .WithPrivateAccessToken(expected);

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.PrivateAccessToken.Should().Be(expected);
        }

        [TestMethod]
        public void ConfigureVsts_with_CacheTimeout()
        {
            var settings = new VstsSettings { CacheTimeout = TimeSpan.FromDays(1) };

            var expected = TimeSpan.FromMilliseconds(1);

            var result = Features.Are.ConfiguredBy
                .VstsConfig()
                .WithSettings(settings)
                .WithCacheTimeout(expected);

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.CacheTimeout.Should().Be(expected);
        }

        [TestMethod]
        public void ConfigureVsts_with_Environment()
        {
            var settings = new VstsSettings();

            var result = Features.Are.ConfiguredBy
                .VstsConfig()
                .WithSettings(settings)
                .WithEnvironment("XXX");

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.AdditionalQueryFilter.Should().Contain("XXX");
        }

        [TestMethod]
        public void ConfigureVsts_with_WorkItemType()
        {
            var settings = new VstsSettings() { WorkItemType = "Old" };

            var result = Features.Are.ConfiguredBy
                .VstsConfig()
                .WithSettings(settings)
                .WithWorkItemType("New");

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.WorkItemType.Should().Be("New");
        }

        [TestMethod]
        public void ConfigureVsts_with_NameField()
        {
            var settings = new VstsSettings() { NameField = "Old" };

            var result = Features.Are.ConfiguredBy
                .VstsConfig()
                .WithSettings(settings)
                .WithNameField("New");

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.NameField.Should().Be("New");
        }

        [TestMethod]
        public void ConfigureVsts_with_ValueField()
        {
            var settings = new VstsSettings() { ValueField = "Old" };

            var result = Features.Are.ConfiguredBy
                .VstsConfig()
                .WithSettings(settings)
                .WithValueField("New");

            result.Should().NotBeNull();
            ((ConfigureVsts)result).Settings.ValueField.Should().Be("New");
        }
    }
}