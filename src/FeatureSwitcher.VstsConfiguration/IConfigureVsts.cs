using FeatureSwitcher.Configuration;
using System;
using System.Threading.Tasks;

namespace FeatureSwitcher.VstsConfiguration
{
    public interface IConfigureVsts : IConfigureFeatures
    {

        /// <summary>
        /// Configures the VSTS connection with the <see cref="VstsSettings"/>.
        /// </summary>
        /// <param name="settings">The <see cref="VstsSettings"/>.</param>
        /// <returns></returns>
        IConfigureVsts WithSettings(VstsSettings settings);

        /// <summary>
        /// Configures the VSTS connection to use the specified url.
        /// </summary>
        /// <param name="url">The url of your VSTS team project (https://youraccount.visualstudio.com/projectname)</param>
        /// <returns></returns>
        IConfigureVsts WithVSTSUrl(Uri url);

        /// <summary>
        /// Configures the VSTS connection to use the specified personal access token (PAT).
        /// </summary>
        /// <param name="pat">The personal access token (PAT). See <see cref="https://www.visualstudio.com/en-us/docs/setup-admin/team-services/use-personal-access-tokens-to-authenticate"/></param>
        /// <returns></returns>
        IConfigureVsts WithPrivateAccessToken(string pat);

        /// <summary>
        /// Configures the VSTS connection with the specified chache timeout.
        /// </summary>
        /// <param name="cacheTimeout">The timeout for the feature flag cache. Default are 10 minutes.</param>
        /// <returns></returns>
        IConfigureVsts WithCacheTimeout(TimeSpan cacheTimeout);

        /// <summary>
        /// Configures the VSTS connection with the specified environment. The environment is used to allow multiple flags with the same name.
        /// </summary>
        /// <param name="environment">A string representing the current enviornment</param>
        /// <returns></returns>
        IConfigureVsts WithEnvironment(string environment);

        /// <summary>
        /// Configures the VSTS connection to use the specified work item type to store feature flags.
        /// </summary>
        /// <param name="workItemType">The work item type to use (default: Task)</param>
        /// <returns></returns>
        IConfigureVsts WithWorkItemType(string workItemType);

        /// <summary>
        /// Configures the VSTS connection to use the specified field to store the name of the feature flags.
        /// </summary>
        /// <param name="nameField">The field to store the feature flag name in (Default: System.Title)</param>
        /// <returns></returns>
        IConfigureVsts WithNameField(string nameField);

        /// <summary>
        /// Configures the VSTS connection to use the specified field to store the value of the feature flags.
        /// </summary>
        /// <param name="valueField">The field to store the feature flag value in (Default: System.Description)</param>
        /// <returns></returns>
        IConfigureVsts WithValueField(string valueField);

        /// <summary>
        /// Loads all features from VSTS. 
        /// </summary>
        /// <returns></returns>
        Task PreloadedFeatures();

    }
}
