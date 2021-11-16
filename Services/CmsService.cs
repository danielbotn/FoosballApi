using System.Threading.Tasks;
using FoosballApi.Helpers;
using FoosballApi.Models.Cms;
using Newtonsoft.Json;

namespace FoosballApi.Services
{
    public interface ICmsService
    {
        Task<HardcodedStrings> GetHardcodedStrings(string language);
    }
    public class CmsService : ICmsService
    {
        private readonly Secrets _secrets;

        public CmsService(Secrets secrets)
        {
            _secrets = secrets;
        }
        
        public async Task<HardcodedStrings> GetHardcodedStrings(string language)
        {
            HttpCaller httpCaller = new();

            string query = "{ hardcodedString(locale: "
                + language
                + ") {matches newGame quickActions lastTenMatches newGame statistics history leagues pricing settings about logout } }";

            var iCmsBody = new ICmsBody
            {
                query = query
            };

            string URL = "https://graphql.datocms.com/";
            string bodyParam = System.Text.Json.JsonSerializer.Serialize(iCmsBody);
            string data = await httpCaller.MakeApiCall(bodyParam, URL, _secrets);
            var resultToJson = JsonConvert.DeserializeObject<Root>(data);

            var hardcodedStrings = resultToJson.data.hardcodedString;

            return hardcodedStrings;
        }
    }
}