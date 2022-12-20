using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Injection;
using Utils.Signal;

namespace CityBuilding
{
    
    
    [Singleton]
    public class CharacterModel : InjectableObject<CharacterModel>
    {
        [Inject] private ConfigService _config;
        [Inject] private CharacterService _service;
        
        public readonly Signal Updated = new();

        public List<string> Characters { get; private set; } = new();

        public void Set(string[] value)
        {
            Characters = value.ToList(); 
            Updated.Dispatch();
        }

        public void Create(string type)
        {
            Characters.Add(type);
            Updated.Dispatch();
        }
    }
}