using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NSE.WebApp.MVC.Services
{
    public abstract class ServiceHttp
    {
        
        protected StringContent ObterConteudo(object dado)
        {
            return new StringContent(JsonSerializer.Serialize(dado),
                                     Encoding.UTF8,
                                     "application/json");
        }

        protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage responseMessage)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        }

        public bool TratarErrosResponse(HttpResponseMessage httpResponseMessage)
        {
            switch ((int)httpResponseMessage.StatusCode )
            {
                case 401:
                case 403:
                case 404:
                case 500:
                    throw new CustomHttpRequestException(httpResponseMessage.StatusCode);

                case 400:
                    return false;
            }

            httpResponseMessage.EnsureSuccessStatusCode();
            return true;
        }

        public ResponseResult RetornoOk()
        {
            return new ResponseResult();
        }
    }
}
