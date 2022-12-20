using System;
using System.Net.Http;
using System.Threading.Tasks;
using Avatar;
using Newtonsoft.Json;
using Utils.Injection;

namespace CityBuilding
{
    [Singleton]
    public class CharacterService:HTTPService
    {

        private async Task<string[]> GetCharacterData(string id)
        {
            
#if STANDALONE
            return Array.Empty<string>();
#endif
            return JsonConvert.DeserializeObject<string[]>(await Get($"http://localhost:8102/api/characters/{id}"));
        }
    }
}