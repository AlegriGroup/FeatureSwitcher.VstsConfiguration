using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureSwitcher.VstsConfiguration
{
    public interface IVstsClient
    {
        Task<IDictionary<string, string>> GetAsync();

        Task<dynamic> PutAsync(string name, string data);
    }
}