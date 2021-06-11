using CybersecHSG.Common.Models;
using CybersecHSG.Common.Utils;
using Microsoft.Research.SEAL;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CybersecHSG.ClientConsole
{
    public class FitnessTrackerClient
    {
        private static HttpClient _client = new HttpClient();
        private static readonly string BaseUri = "https://localhost:44357/";
        private static SEALContext _sealContext;
        private static KeyGenerator _keyGenerator;
        private static PublicKey publicKey;
        public static KeysModel GetKeys()
        {
            _sealContext = SEALUtils.GetContext();
            SecretKey key = new SecretKey();
            key.Load(_sealContext, new MemoryStream());

            _keyGenerator = new KeyGenerator(_sealContext);
            _keyGenerator.CreatePublicKey(out PublicKey publickKey);
            publicKey = publickKey;

            KeysModel keysModel = new KeysModel
            {
                PublicKey = SEALUtils.PublicKeyToBase64String(publicKey),
                SecretKey = SEALUtils.SecretKeyToBase64String(_keyGenerator.SecretKey)
            };
            return keysModel;
        }
        //public static async Task AddNewRunningDistance(RunItem metricsRequest)
        //{
        //    var metricsRequestAsJsonStr = JsonConvert.SerializeObject(metricsRequest);
      
        //    using (var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUri}/calculation"))
        //    using (var content = new StringContent(metricsRequestAsJsonStr, Encoding.UTF8, "application/json"))
        //    {
        //        request.Content = content;
        //        var response = await _client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //    }
        //}
        public static async Task<SummaryItem> GetMetrics()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUri}/calculation"))
            {
                var response = await _client.SendAsync(request);
                return JsonConvert.DeserializeObject<SummaryItem>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}