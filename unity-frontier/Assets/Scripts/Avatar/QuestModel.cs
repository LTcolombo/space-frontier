using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityBuilding;
using Utils.Injection;
using Utils.Signal;

namespace Avatar
{
    
    [Singleton]
    public class QuestModel : InjectableObject<QuestModel>
    {
        public Signal Updated = new();
        
        [Inject] private QuestService _service;
        [Inject] private AccountModel _account;
        private List<QuestInstance> _data;

        public async Task Load()
        {
             
#if STANDALONE
            _data = new List<QuestInstance>
            {
                new()
                {
                    id = "Team",
                    initiator = "Character_Medical_Male_01"
                },
                new()
                {
                    id = "Project",
                    initiator = "Character_Junky_Female_01"
                },
                new()
                {
                    id = "Tech",
                    initiator = "Character_Monk_Male_01"
                }
                
                
            };
            
            return;
#endif
            _data = await _service.GetData(_account.Id);
            Updated.Dispatch();
        }

        public List<QuestInstance> Get()
        {
            return _data;
        }

        public QuestInstance FindQuest(string prefabName)
        {
            return _data?.FirstOrDefault(q => q.initiator == prefabName);
        }

        public void Remove(QuestInstance value)
        {
            _data.Remove(value);
        }
    }

    public class QuestInstance
    {
        public string id;
        public string initiator;
    }
}