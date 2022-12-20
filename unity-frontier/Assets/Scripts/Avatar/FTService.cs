using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Utils.Injection;

namespace Avatar
{
    [Singleton]
    public class FTService : HTTPService
    {
        public async Task<int> GetData(string id)
        {
             
#if STANDALONE
            return 0;
#endif
            return int.Parse(await Get("http://localhost:8102/api/ft/" + id));
        }

        public async Task Transfer(string id, int amount)
        {
            await Post("http://localhost:8102/api/ft/" + id, JsonConvert.SerializeObject(
                new { amount }));
        }
    }

    public class HTTPService
    {
        private HttpClient _client;

        public HTTPService()
        {
            _client = new HttpClient();
        }

        protected async Task<string> Get(string url)
        {
            
            var request = UnityWebRequest.Get(url);
            await Task.Yield();
            request.SendWebRequest();
            while (!request.isDone)
                await Task.Yield();
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
                return null;
            }
            
            return request.downloadHandler.text;
        }

        protected async Task<string> Post(string url, string content)
        {
            var request = UnityWebRequest.Post(url, content);
            await Task.Yield();
            request.SendWebRequest();
            while (!request.isDone)
                await Task.Yield();
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
                return null;
            }
            
            return request.downloadHandler.text;
        }
    }
}