using System;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace.Model;
using Utils.Injection;
using Utils.Signal;

namespace CityBuilding
{
    [Singleton]
    public class BuilderModel : InjectableObject<BuilderModel>
    {
        [Inject] private BuilderService _service;
        [Inject] private InteractionModel _interaction;
        [Inject] private ConfigService _config;

        public readonly Signal Updated = new();

        public byte[,] OccupiedData { get; private set; }
        public Building Current { get; private set; }

        public BuildingPlacement[] Buildings { get; private set; }

        public BuildConfig Config { get; private set; }

        private static BuilderModel _instance;


        public async Task Load(string id)
        {
#if STANDALONE
            OccupiedData = new byte[20, 18];
            Buildings = Array.Empty<BuildingPlacement>();
            Config = new BuildConfig()
            {
                width = 20,
                height = 18,
                buildings = new Building[]
                {
                    new()
                    {
                        type = BuildingType.Generator,
                        width = 4,
                        height = 4,

                        prefab = "Buildings/Generator"
                    },

                    new()
                    {
                        type = BuildingType.Distributor,
                        width = 2,
                        height = 2,
                        prefab = "Buildings/Distributor"
                    }
                }
            };
#else

            Config = await _config.GetBuildConfig();
            var result = await _service.GetBuildingsData(id);

            OccupiedData = result.cells;
            Buildings = result.buildings;
#endif
        }


        public async void PutBuilding(string id, int x, int y)
        {
#if STANDALONE


#else
            var result = await _service.PutBuilding(id, x, y, Current.type);

            OccupiedData = result.cells;
            Buildings = result.buildings;
#endif
            Updated.Dispatch();
        }

        public void StartPlacement(BuildingType building)
        {
            Current = Config.buildings.First(b => b.type == building);
            _interaction.Set(InteractionState.Building);
        }

        public Building GetConfig(BuildingType type)
        {
            return Config.buildings.First(b => b.type == type);
        }

        public int GetEnergyGeneration()
        {
#if STANDALONE
            return 10;
#endif
            return Buildings.Sum(building => building.type switch
            {
                BuildingType.Distributor => 2,
                BuildingType.Generator => 8,
                _ => throw new ArgumentOutOfRangeException()
            });
        }
    }
}