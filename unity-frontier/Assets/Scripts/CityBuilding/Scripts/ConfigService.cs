using System.Net.Http;
using System.Threading.Tasks;
using Avatar;
using Newtonsoft.Json;
using Utils.Injection;

namespace CityBuilding
{
    public class CharacterData
    {
        public string prefab;
    }

    public enum BuildingType
    {
        Generator,
        Distributor
    }

    public class BuildConfig
    {
        public int width;
        public int height;
        public Building[] buildings;
    }

    public class Building
    {
        public int width;
        public int height;
        public string prefab;
        public BuildingType type;
    }


    [Singleton]
    public class ConfigService : HTTPService
    {
        public async Task<BuildConfig> GetBuildConfig()
        {
#if STANDALONE
            return new BuildConfig();
#else
            return JsonConvert.DeserializeObject<BuildConfig>(await Get("http://localhost:8102/api/config/build"));
#endif
        }
    }
}