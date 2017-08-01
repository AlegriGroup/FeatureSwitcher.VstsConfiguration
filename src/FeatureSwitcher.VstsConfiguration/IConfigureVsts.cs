using FeatureSwitcher.Configuration;
using System;
using System.Threading.Tasks;

namespace FeatureSwitcher.VstsConfiguration
{
    public interface IConfigureVsts : IConfigureFeatures
    {
        IConfigureVsts WithSettings(VstsSettings settings);

        IConfigureVsts WithVSTSUrl(Uri url);

        IConfigureVsts WithPrivateAccessToken(string pat);

        IConfigureVsts WithCacheTimeout(TimeSpan cacheTimeout);

        IConfigureVsts WithEnvironment(string environment);

        IConfigureVsts WithWorkItemType(string workItemType);

        IConfigureVsts WithNameField(string nameField);

        IConfigureVsts WithValueField(string valueField);

        Task PreloadedFeatures();

    }
}
