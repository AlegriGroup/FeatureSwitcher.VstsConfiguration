using FeatureSwitcher;
using FeatureSwitcher.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FeatureSwitcher.VstsConfiguration.Tests
{
    [TestClass]
    public class E2ETests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void EndToEnd_flag_gets_created_automatically_for_environment()
        {
            var environmentKey = Guid.NewGuid().ToString();

            Features.Are
                .NamedBy
                .TypeFullName()
                .And
                .ConfiguredBy
                .VstsConfig()
                .WithVSTSUrl(Settings.Url)
                .WithPrivateAccessToken(IntegrationTests.GetPAT())
                .WithEnvironment(environmentKey)
                .PreloadedFeatures()
                .GetAwaiter().GetResult();

            // the flag should not exist and is created for the new environment
            var state = Feature<Demo.DemoFeature>.Is().Enabled;

            // the new flag should have the state "false" per default.
            state.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void EndToEnd_gets_the_correct_flag_for_environment()
        {
            var environmentKey = Guid.NewGuid().ToString();

            var testSettings = new VstsSettings();
            testSettings.AddEnvironment(environmentKey);

            var testClient = new VstsClient(Settings.Url, IntegrationTests.GetPAT(), testSettings);
            var task = testClient.PutAsync("Demo.DemoFeature", "True");
            task.GetAwaiter().GetResult();;

            var id = task.Result.Id;


            Features.Are
                .NamedBy
                .TypeFullName()
                .And
                .ConfiguredBy
                .VstsConfig()
                .WithVSTSUrl(Settings.Url)
                .WithPrivateAccessToken(IntegrationTests.GetPAT())
                .WithEnvironment(environmentKey)
                .WithCacheTimeout(TimeSpan.FromMilliseconds(1))
                .PreloadedFeatures()
                .GetAwaiter().GetResult();



            Feature<Demo.DemoFeature>.Is().Enabled.Should().BeTrue("The default flag should be false.");

            // turn off the feature
            var doc = new JsonPatchDocument();
            doc.Add(new JsonPatchOperation
            {
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Replace,
                Path = "/fields/" + testSettings.ValueField,
                Value = "False"
            });

            var t = testClient.WorkItemTrackingHttpClient.UpdateWorkItemAsync(doc, id);
            t.GetAwaiter().GetResult();

            Feature<Demo.DemoFeature>.Is().Enabled.Should().BeFalse("The feature was turned off.");

            testClient.WorkItemTrackingHttpClient.DeleteWorkItemAsync(id, destroy: true);
        }
    }
}

namespace Demo
{
    public class DemoFeature : IFeature { }
}
