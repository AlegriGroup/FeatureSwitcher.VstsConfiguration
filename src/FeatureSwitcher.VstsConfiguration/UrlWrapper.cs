using System;
using System.Linq;

namespace FeatureSwitcher.VstsConfiguration
{
    public class UrlWrapper
    {
        readonly Uri _input;

        public UrlWrapper(Uri uri)
        {
            _input = uri;
        }

        public string ProjectName
        {
            get
            {
                return _input.Segments.Last().Trim('/');
            }
        }

        public Uri ProjectCollectionUri
        {
            get
            {
                var collection = string.Join("", _input.Segments.Take(_input.Segments.Count() - 1));

                var port = _input.IsFile ? string.Empty : $":{_input.Port}";

                return new Uri($"{_input.Scheme}://{_input.DnsSafeHost}{port}{collection}".TrimEnd('/'));
            }
        }
    }
}
