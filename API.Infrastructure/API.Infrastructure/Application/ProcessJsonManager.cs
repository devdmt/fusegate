using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Application
{
  public static class ProcessJsonManager
    {
        //  public   static async Task<>

        public enum methodtype
        {
            Get, Post
        }
        public enum TokenType
        {
            Basic, Bearer
        }
        public static async Task<string> ProcessJsonAsync(string? url, string token, string jsondata, methodtype methodtype = methodtype.Post, TokenType tokenType = TokenType.Bearer)
        {
            var response = new HttpResponseMessage();
            // "http://127.0.0.1:5000/api/B2BServices";
            // var json = JsonConvert.SerializeObject(param);
            string tokentyp = "Bearer";
            if (tokenType == TokenType.Bearer)
            {
                tokentyp = "Bearer";
            }
            else if (tokenType == TokenType.Basic)
            {
                tokentyp = "Basic";
            }
            var data = new StringContent(jsondata, Encoding.UTF8, "application/json");
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    //  Proxy = new WebProxy(_configs.ProxyIp, Convert.ToInt32(_configs.ProxyPort)),
                    UseProxy = false,

                };
                using var client = new HttpClient();

                if (methodtype == methodtype.Post)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue(tokentyp, token);
                    response = await client.PostAsync(url, data);

                }
                else if (methodtype == methodtype.Get)
                {
                    using (var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        requestMessage.Headers.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue(tokentyp, token);


                        response = await client.SendAsync(requestMessage);
                    }

                }
                return await response.Content.ReadAsStringAsync();

                //return JsonConvert.DeserializeObject<T>(result);
            }

            catch (Exception ex)
            {
                // return "";
            }

            return null;


        }


    }
}
