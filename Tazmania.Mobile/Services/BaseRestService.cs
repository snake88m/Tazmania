using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;

namespace Tazmania.Mobile.Services
{
    public class BaseRestService
    {
        readonly HttpClient client;
        readonly JsonSerializerOptions jsonOptions;
        readonly AuthenticationService authService;

        public string BaseUrl
        {
            get
            {
                //return "https://localhost:4747/api";
                return IsLAN() ? "https://192.168.1.4:4747/api" : "https://dementia.dyndns.org:4747/api";
            }
        }

        public async Task WaitWhenIsReady()
        {
            // su android l'header dell'autenticazione impiega del tempo a caricare
            // questa attesa serve per evitare eccezioni
            while (client.DefaultRequestHeaders.Authorization == null)
            {
                await Task.Delay(25);
            }
        }

        protected BaseRestService(AuthenticationService authService)
        {
            this.authService = authService;

            // l'ssl non è verificato, quindi bypasso il controllo
            var httpHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            };

            client = new HttpClient(httpHandler);

            jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            SetAuthHeader();
        }

        async void SetAuthHeader()
        {
            var user = await authService.GetCredential();
            string header = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Mail}:{user.Password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", header);
        }

        protected bool IsLAN()
        {
            //todo: provvisorio il comando successivo se lanciato su win scatena thread exception
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                return false;
            }    

            return Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi) ||
                   Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Ethernet);
        }

        protected async Task SendPost<REQUEST>(string partialUrl, REQUEST request)
        {
            string json = JsonSerializer.Serialize<REQUEST>(request, jsonOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await client.PostAsync(Path.Combine(BaseUrl, partialUrl), content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException(httpResponse.StatusCode.ToString());
            }
        }

        protected async Task<RESPONSE> SendPost<REQUEST, RESPONSE>(string partialUrl, REQUEST request)
        {
            string json = JsonSerializer.Serialize<REQUEST>(request, jsonOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await client.PostAsync(Path.Combine(BaseUrl, partialUrl), content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException(httpResponse.StatusCode.ToString());
            }

            string rawResponse = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RESPONSE>(rawResponse, jsonOptions);
        }
    }
}
