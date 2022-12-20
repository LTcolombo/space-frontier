using System.Threading.Tasks;
using Utils.Injection;
using Utils.Signal;

namespace Avatar
{
    
    [Singleton]
    public class FTModel : InjectableObject<FTModel>
    {
        public Signal Updated = new();
        
        [Inject] private FTService _service;
        [Inject] private AccountModel _account;
        private int _data;

        public async Task Update()
        {
            _data = await _service.GetData(_account.Id);
            Updated.Dispatch();
        }

        public int Get()
        {
            return _data;
        }

        public async Task Apply(int value)
        {
            _data += value;
            await _service.Transfer(_account.Id, value);
            Updated.Dispatch();
        }
    }
}