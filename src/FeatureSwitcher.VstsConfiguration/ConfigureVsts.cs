using System;
using FeatureSwitcher.Configuration;
using System.Threading.Tasks;

namespace FeatureSwitcher.VstsConfiguration
{
    public partial class ConfigureVsts : IConfigureVsts, IConfigureFeatures
    {
        readonly IConfigureBehavior _control;
        readonly IConfigureFeatures _config;
        readonly VstsSettings _settings;
        readonly VstsConfig _target;

        public VstsSettings Settings => _settings;


        internal ConfigureVsts(IConfigureBehavior control)
            : this(control, new VstsSettings())
        {
        }

        private ConfigureVsts(IConfigureBehavior control, VstsSettings settings)
        {
            _control = control;
            _settings = settings;
            _target = new VstsConfig(_settings);
            _config = control.Custom(_target.IsEnabled);
        }

        IConfigureVsts IConfigureVsts.WithSettings(VstsSettings settings)
        {
            return new ConfigureVsts(_control, settings);
        }

        IConfigureVsts IConfigureVsts.WithVSTSUrl(System.Uri url)
        {
            _settings.Url = url;
            return new ConfigureVsts(_control, _settings);
        }

        IConfigureVsts IConfigureVsts.WithPrivateAccessToken(string pat)
        {
            _settings.PrivateAccessToken = pat;
            return new ConfigureVsts(_control, _settings);
        }

        IConfigureVsts IConfigureVsts.WithCacheTimeout(TimeSpan cacheTimeout)
        {
            _settings.CacheTimeout = cacheTimeout;
            return new ConfigureVsts(_control, _settings);
        }

        IConfigureVsts IConfigureVsts.WithEnvironment(string environment)
        {
            _settings.AddEnvironment(environment);
            return new ConfigureVsts(_control, _settings);
        }

        IConfigureVsts IConfigureVsts.WithWorkItemType(string workItemType)
        {
            _settings.WorkItemType = workItemType;
            return new ConfigureVsts(_control, _settings);
        }

        IConfigureVsts IConfigureVsts.WithNameField(string nameField)
        {
            _settings.NameField = nameField;
            return new ConfigureVsts(_control, _settings);
        }

        IConfigureVsts IConfigureVsts.WithValueField(string valueField)
        {
            _settings.ValueField = valueField;
            return new ConfigureVsts(_control, _settings);
        }

        async Task IConfigureVsts.PreloadedFeatures()
        {
            await _target.SetupAsync();
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class ConfigureVsts : IConfigureFeatures
    {
        IConfigureFeatures IConfigureFeatures.And => _config;

        IConfigureNaming IConfigureFeatures.NamedBy => _config.NamedBy;

        IConfigureBehavior IConfigureFeatures.ConfiguredBy => _config.ConfiguredBy;
    }
}
