using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Utils.Injection;

namespace Avatar
{
    public class NFTAttribute
    {
        public string trait_type;
        public string value;
    }

    public class ExtraMeta
    {
        public List<NFTAttribute> attributes;
    }

    public class NFTMetadata
    {
        public string title;
        public string description;
        public string media;

        public string issued_at;

        //nested deserialiser
        public ExtraMeta extraParsed;
        public string extra;
    }

    public class NFT
    {
        public string token_id;
        public string owner_id;
        public NFTMetadata metadata;
    }

    [Singleton]
    public class AvatarService:HTTPService
    {
        private NFT _data;

        public async Task<NFTMetadata> GetData(string id)
        {
             
#if STANDALONE
            return new NFTMetadata();
#endif
            
            return JsonConvert.DeserializeObject<NFTMetadata>(await Get("http://localhost:8102/api/avatar/" + id));
        }

        public async Task UpdateAttributeById(string accountId, string key, int value)
        {
            Debug.Log(await Post("http://localhost:8102/api/avatar/" + accountId, JsonConvert.SerializeObject(
                new { key, value })));
        }

    }
}