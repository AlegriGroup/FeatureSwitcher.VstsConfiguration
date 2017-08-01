using System;
using System.Collections.Generic;

namespace FeatureSwitcher.VstsConfiguration
{
    public class VstsSettings
    {
        public string WorkItemType { get; set; } = "Task";

        public string NameField { get; set; } = "System.Title";

        public string ValueField { get; set; } = "System.Description";

        public Dictionary<string, string> AdditionalFields { get; } = new Dictionary<string, string>
        {
            {  "System.Tags", "FeatureFlag" }
        };

        public string AdditionalQueryFilter { get; set; } = "and [System.Tags] Contains 'FeatureFlag'";

        public Uri Url { get; set; }

        public string PrivateAccessToken { get; set; }

        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromMinutes(10);
    }
}
