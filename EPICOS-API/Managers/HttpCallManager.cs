using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using EPICOS_API.Models.Wrappers;
using Newtonsoft.Json;

namespace EPICOS_API.Managers
{
    public class HttpCallManager: IHttpCallManager
    {
        private readonly string key;
        private readonly string baseURI;

        public HttpCallManager(string key, string baseURI)
        {
            this.key = key;
            this.baseURI = baseURI;
        }

        public async Task<bool> PostAuthURI(string url, HttpContent c)
        {
            var response = false;
            using (var client = new HttpClient())
            {
                string httpURI = $"{this.baseURI}{url}"; 
                Uri u = new Uri(httpURI);
                client.DefaultRequestHeaders.Add("X-Authorization", this.key);
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    if (await result.Content.ReadAsStringAsync() == "1")
                        response = true;            
                }
            }
            return response;
        }

        public async Task<ExternalPaginationResponse<T>> GetURI<T>(string url)
        {
            using (var client = new HttpClient())
            {
                string httpURI = $"{this.baseURI}{url}"; 
                Uri u = new Uri(httpURI);
                HttpResponseMessage response = await client.GetAsync(u);
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalPaginationResponse<T>>(jsonString);

            }
        }

        public async Task<ExternalPaginationResponse<T>> GetErpURI<T>(string url)
        {
            using (var client = new HttpClient())
            {
                string baseURI = "https://erp-api.kmc.solutions/api";
                string httpURI = $"{baseURI}{url}";
                Uri u = new Uri(httpURI);
                HttpResponseMessage response = await client.GetAsync(u);
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonString);
                return JsonConvert.DeserializeObject<ExternalPaginationResponse<T>>(jsonString);
            }
        }

        public async Task<T> GetSingleErpURI<T>(string url)
        {
            using (var client = new HttpClient())
            {
                string baseURI = "https://erp-api.kmc.solutions/api";
                string httpURI = $"{baseURI}{url}";
                Uri u = new Uri(httpURI);
                HttpResponseMessage response = await client.GetAsync(u);
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonString);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
        }

        public async Task<ExternalPaginationResponse<T>> QueryURI<S, T>(string url, S query)
        {
            using (var client = new HttpClient())
            {
                
                int i = 1;
                foreach(PropertyInfo propertyInfo in query.GetType().GetProperties()){
                    var name = propertyInfo.Name;
                    var value1 = propertyInfo.GetValue(query);
                    if(value1 != null){
                        if(!string.IsNullOrEmpty(value1.ToString())){
                            if(i == 1)
                                url += $"?{name.ToLower()}={value1}";
                            else
                                url += $"&{name.ToLower()}={value1}";
                        }
                    }
                    i++;
                }

                string baseURI = "https://erp-api.kmc.solutions/api";
                string httpURI = $"{baseURI}{url}";
                Console.WriteLine(url);
                Uri u = new Uri(httpURI);
                client.DefaultRequestHeaders.Add("X-Authorization", this.key);
                HttpResponseMessage response = await client.GetAsync(u);
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalPaginationResponse<T>>(jsonString);

            }
        }

        public async Task<ExternalPaginationResponse<T>> QueryErpURI<S, T>(string url, S query)
        {
            using (var client = new HttpClient())
            {

                int i = 1;
                foreach (PropertyInfo propertyInfo in query.GetType().GetProperties())
                {
                    var name = propertyInfo.Name;
                    var value1 = propertyInfo.GetValue(query);
                    if (value1 != null)
                    {
                        if (!string.IsNullOrEmpty(value1.ToString()))
                        {
                            if (i == 1)
                                url += $"?{name.ToLower()}={value1}";
                            else
                                url += $"&{name.ToLower()}={value1}";
                        }
                    }
                    i++;
                }
                string baseURI = "https://erp-api.kmc.solutions/api";
                string httpURI = $"{baseURI}{url}";
                Console.WriteLine(url);
                Uri u = new Uri(httpURI);
                client.DefaultRequestHeaders.Add("X-Authorization", this.key);
                HttpResponseMessage response = await client.GetAsync(u);
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalPaginationResponse<T>>(jsonString);

            }
        }
    }
}