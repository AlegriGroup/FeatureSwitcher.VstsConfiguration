using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureSwitcher.VstsConfiguration
{
    public class VstsClient : IVstsClient
    {
        readonly WorkItemTrackingHttpClient _client;
        readonly string _projectName;
        readonly VstsSettings _settings;

        public VstsClient(Uri projectUrl, string pat, VstsSettings settings)
        {
            if (projectUrl == null)
                throw new ArgumentNullException("projectUrl");

            if (pat == null)
                throw new ArgumentNullException("pat");

            if (projectUrl.LocalPath == "/")
                throw new ArgumentException("Invalid URL: The URL must have the format: 'https://<account>.visualstudio.com/<project>'.", "projectUrl");

            var urlWrapper = new UrlWrapper(projectUrl);
            _projectName = urlWrapper.ProjectName;
            var baseUrl = urlWrapper.ProjectCollectionUri;

            _settings = settings;

            VssCredentials credentials = new VssBasicCredential(string.Empty, pat);

            var connection = new VssConnection(baseUrl, credentials);
            _client = connection.GetClient<WorkItemTrackingHttpClient>();
        }

        public VstsClient(Uri projectUrl, string pat)
            : this(projectUrl, pat, new VstsSettings())
        {
        }

        public VstsClient(VstsSettings settings)
            : this(settings.Url, settings.PrivateAccessToken, settings)
        {
        }

        public WorkItemTrackingHttpClient WorkItemTrackingHttpClient => _client;

        public async Task<IDictionary<string, string>> GetAsync()
        {
            var query = new Wiql
            {
                Query = $"Select [System.Id] from WorkItems where [System.TeamProject] = '{_projectName}' and [System.WorkItemType] = '{_settings.WorkItemType}'"
            };

            if (!string.IsNullOrWhiteSpace(_settings.AdditionalQueryFilter))
            {
                query.Query += " " + _settings.AdditionalQueryFilter;
            }


            var result = await _client.QueryByWiqlAsync(query);

            if (!result.WorkItems.Any())
                return new Dictionary<string, string>();

            var fields = new string[]
            {
                "System.Id",
                _settings.NameField,
                _settings.ValueField
            };

            var workItems = await _client.GetWorkItemsAsync(result.WorkItems.Select(x => x.Id), fields);

            return workItems
                .Select(x => new
                {
                    key = x.Fields[_settings.NameField].ToString(),
                    value = x.Fields[_settings.ValueField].ToString()
                })
            .AsEnumerable()
            .ToDictionary(p => p.key, p => p.value);
        }

        public async Task<dynamic> PutAsync(string name, string data)
        {
            var doc = new JsonPatchDocument();
            doc.Add(new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = $"/fields/{_settings.NameField}",
                Value = name
            });

            doc.Add(new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = $"/fields/{_settings.ValueField}",
                Value = data
            });

            foreach (var item in _settings.AdditionalFields)
            {
                doc.Add(new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = $"/fields/{item.Key}",
                    Value = item.Value
                });
            }

            return await _client.CreateWorkItemAsync(doc, _projectName, _settings.WorkItemType);
        }
    }
}