using System.Threading.Tasks;
using Newtonsoft.Json;
using Utils.Injection;
using Utils.Signal;

namespace Avatar
{
    [Singleton]
    public class AvatarModel : InjectableObject<AvatarModel>
    {
        [Inject] private AccountModel _account;
        [Inject] private AvatarService _service;
        private NFTMetadata _data;
        public Signal Updated = new();

        public async Task Load()
        {
            _data = await _service.GetData(_account.Id);
            if (_data?.extra != null)
                _data.extraParsed = JsonConvert.DeserializeObject<ExtraMeta>(_data.extra);
            Updated.Dispatch();
        }

        public NFTMetadata Get()
        {
            return _data;
        }

        public string GetAttributeById(string value)
        {
            return _data?.extraParsed?.attributes.Find(a => a.trait_type == value).value;
        }

        public async Task UpdateAttributeById(string key, int value)
        {
            var attribute = _data.extraParsed.attributes.Find(a => a.trait_type == key);
            attribute.value = (int.Parse(attribute.value) + value).ToString();
            Updated.Dispatch();

            await _service.UpdateAttributeById(_account.Id, key, value);
        }
    }
}