using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utils.Injection;

namespace Avatar
{
    [Singleton]
    public class QuestService : HTTPService
    {
        public async Task<List<QuestInstance>> GetData(string id)
        {
            return JsonConvert.DeserializeObject<List<QuestInstance>>(await Get("http://localhost:8102/api/quests/" + id));
        }
    }
}