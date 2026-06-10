using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventPlusWeb1.Services
{
    public class ChatbotService
    {
        private readonly string _apiKey;
        private readonly string _modelo = "gemini-2.5-flash";
        private readonly string _promptSistema = @"Eres EventBot, el asistente virtual de EventPlus. 
        Ayudas a los usuarios con información sobre eventos, inscripciones y el uso de la plataforma. 
        Eres amable, conciso y profesional. Si te preguntan algo que no tiene que ver con eventos o la plataforma, 
        responde amablemente que solo puedes ayudar con temas relacionados a EventPlus.";

        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        public ChatbotService()
        {
            _apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
        }

        public string ObtenerRespuesta(string mensaje)
        {
            try
            {
                return ObtenerRespuestaAsync(mensaje).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return "Error al conectar con la IA: " + ex.Message;
            }
        }

        public async Task<string> ObtenerRespuestaAsync(string mensaje)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelo}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = _promptSistema + "\n\nUsuario: " + mensaje }
                        }
                    }
                }
            };

            string jsonRequest = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject resultado = JObject.Parse(jsonResponse);
            string respuestaTexto = resultado["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            if (string.IsNullOrEmpty(respuestaTexto))
            {
                return "Lo siento, no pude procesar tu mensaje. Intenta de nuevo.";
            }

            return respuestaTexto;
        }
    }
}