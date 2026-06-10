using System.Web.Mvc;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly ChatbotService _chatbotService;

        public ChatbotController()
        {
            _chatbotService = new ChatbotService();
        }

        [HttpPost]
        public JsonResult Enviar(string mensaje)
        {
            string respuesta = _chatbotService.ObtenerRespuesta(mensaje);
            bool exito = !respuesta.StartsWith("Error al conectar");
            return Json(new { exito = exito, respuesta = respuesta });
        }
    }
}