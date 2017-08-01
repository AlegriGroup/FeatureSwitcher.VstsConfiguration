using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureSwitcher.VstsConfiguration.Tests
{
    [TestClass]
    public class VstsEnvironmentsTests
    {
        [TestMethod]
        public void VstsEnvironments_can_add_environment_to_default_settings()
        {
            var settings = new VstsSettings();

            settings.AddEnvironment("XXX");

            settings.AdditionalFields["System.Tags"].Should().Be("FeatureFlag,XXX");
            settings.AdditionalQueryFilter.Should().Be("and [System.Tags] Contains 'FeatureFlag' and [System.Tags] Contains 'XXX'");
        }

        [TestMethod]
        public void VstsEnvironments_can_add_environment_to_settings_without_tags()
        {
            var settings = new VstsSettings();
            settings.AdditionalFields.Clear();
            settings.AdditionalQueryFilter = "";

            settings.AddEnvironment("XXX");

            settings.AdditionalFields["System.Tags"].Should().Be("XXX");
            settings.AdditionalQueryFilter.Should().Be("and [System.Tags] Contains 'XXX'");
        }
    }
}
