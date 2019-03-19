using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureSwitcher.VstsConfiguration.Tests
{
    public static class Settings
    {
        public static Uri Url { get; set; } = new Uri("https://alegristg.visualstudio.com/FeatureMaster");

        public static string PAT { get; set; } = "__PAT__";
    }

    [TestClass]
    public static class IntegrationTests
    {
        public static string GetPAT()
        {
            // to execute the intergration tests locally, store a private access token for your VSTS account in a file names pat.secret
            // the file will be ignored by git.
            // For the CI build the pat will be replaced in the app settings file.
            // See https://www.visualstudio.com/en-us/docs/setup-admin/team-services/use-personal-access-tokens-to-authenticate for details on how to create a PAT
            if (Settings.PAT == "__PAT__")
            {
                return File.ReadAllText("pat.secret");
            }
            else
            {
                return Settings.PAT;
            }
        }

        [AssemblyCleanup]
        public static async Task DeleteAllDefaultFeatureFlags()
        {
            var settings = new VstsSettings();
            var testClient = new VstsClient(Settings.Url, IntegrationTests.GetPAT(), settings);

            var query = new Wiql
            {
                Query = $"Select [System.Id] from WorkItems where [System.TeamProject] = '{Settings.Url.LocalPath.Trim('/')}' and [System.WorkItemType] = 'Task' or [System.WorkItemType] = 'FeatureFlag'"
            };

            var result = await testClient.WorkItemTrackingHttpClient.QueryByWiqlAsync(query);

            if (result.WorkItems.Any())
            {
                foreach (var wit in result.WorkItems)
                {
                    await testClient.WorkItemTrackingHttpClient.DeleteWorkItemAsync(wit.Id, destroy: true);
                }
            }
        }
    }
}
