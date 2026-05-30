using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventPlusWeb1.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly string _apiKey = "AIzaSyAxHnTng1KwvB58MbbAbVCVZ92T3lVhaQ8";
        private readonly string _modelo = "gemini-2.5-flash";

        // POST: Chatbot/Enviar
        [HttpPost]
        public JsonResult Enviar(string mensaje)
        {
            try
            {
                string promptSistema = @"Eres EventBot, el asistente virtual de EventPlus. 
Ayudas a los usuarios con información sobre eventos, inscripciones y el uso de la plataforma. 
Eres amable, conciso y profesional. Si te preguntan algo que no tiene que ver con eventos o la plataforma, 
responde amablemente que solo puedes ayudar con temas relacionados a EventPlus.";

                string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelo}:generateContent?key={_apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = promptSistema + "\n\nUsuario: " + mensaje }
                            }
                        }
                    }
                };

                string jsonRequest = JsonConvert.SerializeObject(requestBody);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonRequest);
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();

                        JObject resultado = JObject.Parse(jsonResponse);
                        string respuestaTexto = resultado["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                        if (string.IsNullOrEmpty(respuestaTexto))
                        {
                            respuestaTexto = "Lo siento, no pude procesar tu mensaje. Intenta de nuevo.";
                        }

                        return Json(new { exito = true, respuesta = respuestaTexto });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, respuesta = "Error al conectar con la IA: " + ex.Message });
            }
        }
    }
}
