using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avatar;
using Newtonsoft.Json;
using UnityEngine;
using Utils.Injection;

namespace CityBuilding
{
    public class BuildingPlacement
    {
        public int x;
        public int y;
        public BuildingType type;
    }


    public class SettlementState
    {
        public byte[,] cells;
        public BuildingPlacement[] buildings;
        public string[] characters;
    }

    [Singleton]
    public class BuilderService : HTTPService
    {
        public async Task<SettlementState> GetBuildingsData(string id)
        {
#if STANDALONE
            return new SettlementState()
            {
                buildings = Array.Empty<BuildingPlacement>()
            };
#endif
            return JsonConvert.DeserializeObject<SettlementState>(
                await Get($"http://localhost:8102/api/buildings/{id}"));
        }

        public async Task<SettlementState> PutBuilding(string id, int cellX, int cellY, BuildingType type)
        {
            return JsonConvert.DeserializeObject<SettlementState>(await Post(
                $"http://localhost:8102/api/buildings/{id}", JsonConvert.SerializeObject(
                    new { x = cellX, y = cellY, type })));
        }
    }
}