using FeatureSwitcher.Configuration;

namespace FeatureSwitcher.VstsConfiguration
{
    public static class ByVstsConfig
    {
        /// <summary>
        /// Configure FeatureSwitcher to store the configuration in Visual Studio Team Services (VSTS).
        /// </summary>
        /// <param name="control">The IConfigureBehavior control that is passed in from FeatureSwitcher.</param>
        /// <returns>An object that implements <see cref="IConfigureVsts"/>.</returns>
        public static IConfigureVsts VstsConfig(this IConfigureBehavior control)
        {
            return new ConfigureVsts(control);
        }
    }
}
