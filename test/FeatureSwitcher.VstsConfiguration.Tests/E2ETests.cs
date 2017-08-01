using FeatureSwitcher;
using FeatureSwitcher.Configuration;
using FeatureSwitcher.VstsConfiguration.Tests.Properties;
using FluentAssertions;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

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
                .WithVSTSUrl(Settings.Default.Url)
                .WithPrivateAccessToken(IntegrationTests.GetPAT())
                .WithEnvironment(environmentKey)
                .PreloadedFeatures()
                .Wait();

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

            var testClient = new VstsClient(Settings.Default.Url, IntegrationTests.GetPAT(), testSettings);
            var task = testClient.PutAsync("Demo.DemoFeature", "True");
            task.Wait();

            var id = task.Result.Id;


            Features.Are
                .NamedBy
                .TypeFullName()
                .And
                .ConfiguredBy
                .VstsConfig()
                .WithVSTSUrl(Settings.Default.Url)
                .WithPrivateAccessToken(IntegrationTests.GetPAT())
                .WithEnvironment(environmentKey)
                .WithCacheTimeout(TimeSpan.FromMilliseconds(100))
                .PreloadedFeatures()
                .Wait();



            Feature<Demo.DemoFeature>.Is().Enabled.Should().BeTrue();

            // turn off the feature
            var doc = new JsonPatchDocument();
            doc.Add(new JsonPatchOperation
            {
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Replace,
                Path = "/fields/" + testSettings.ValueField,
                Value = "False"
            });

            var t = testClient.WorkItemTrackingHttpClient.UpdateWorkItemAsync(doc, id);
            t.Wait();

            // give the cache time to expire
            Thread.Sleep(100);

            // get value from cache
            Feature<Demo.DemoFeature>.Is().Enabled.Should().BeTrue();

            // give the operation in the background time to finish
            Thread.Sleep(2000);

            Feature<Demo.DemoFeature>.Is().Enabled.Should().BeFalse();

            testClient.WorkItemTrackingHttpClient.DeleteWorkItemAsync(id, destroy: true);
        }
    }
}

namespace Demo
{
    public class DemoFeature : IFeature { }
}
