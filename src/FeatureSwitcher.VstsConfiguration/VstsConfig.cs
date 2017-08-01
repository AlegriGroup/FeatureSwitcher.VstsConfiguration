using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FeatureSwitcher.VstsConfiguration
{
    public class VstsConfig
    {
        public Feature.Behavior Default => IsEnabled;

        private readonly VstsSettings _settings;
        private IVstsClient _client;
        private readonly ConcurrentDictionary<string, bool?> _cache;
        private DateTime _cacheTimeout;

        public VstsConfig(VstsSettings settings, IVstsClient client)
        {
            _settings = settings;
            _client = client;
            _cache = new ConcurrentDictionary<string, bool?>();
            _cacheTimeout = DateTime.UtcNow.Add(_settings.CacheTimeout);
        }

        public VstsConfig(VstsSettings settings)
            : this(settings, null)
        {
        }

        public ConcurrentDictionary<string, bool?> Cache => _cache;

        private IVstsClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new VstsClient(_settings.Url, _settings.PrivateAccessToken, _settings);
                }

                return _client;
            }
        }

        public async Task SetupAsync()
        {
            var features = await Client.GetAsync();

            foreach (var feature in features)
            {
                bool? value = null;
                if (bool.TryParse(feature.Value, out var x))
                {
                    value = x;
                }

                _cache[feature.Key] = value;
            }

            _cacheTimeout = DateTime.UtcNow.Add(_settings.CacheTimeout);
        }


        public bool? IsEnabled(Feature.Name feature)
        {
            if (feature == null)
                return null;

            if (_cache.Count == 0 && DateTime.UtcNow > _cacheTimeout)
            {
                SetupAsync().Wait();
            }

            if (!_cache.ContainsKey(feature.Value))
            {
                return CreateFeatureFlagInBackground(feature.Value, defaultValue: false);
            }

            var result = _cache[feature.Value];

            if (DateTime.UtcNow > _cacheTimeout)
            {
                ReloadFeaturesInBackgound();
            }


            return result.Value;
        }


        private bool CreateFeatureFlagInBackground(string name, bool defaultValue)
        {
            _cache.AddOrUpdate(name, false, (key, item) => { return item; });

#pragma warning disable S1481 // Unused local variables should be removed

            // fire and forget
            //var t = CreateFeatureFlag(name, defaultValue);
            CreateFeatureFlag(name, defaultValue).Wait();

#pragma warning restore S1481 // Unused local variables should be removed

            return defaultValue;
        }

        private async Task CreateFeatureFlag(string feature, bool defaultValue)
        {
            await Client.PutAsync(feature, defaultValue.ToString());
        }

        private void ReloadFeaturesInBackgound()
        {
#pragma warning disable S1481 // Unused local variables should be removed

            // fire and forget
            var t = SetupAsync();

#pragma warning restore S1481 // Unused local variables should be removed

        }
    }
}
