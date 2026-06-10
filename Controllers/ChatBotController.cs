using System.Threading.Tasks;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class ChatbotController : Controller
    {
        private readonly ChatbotService _chatbotService;

        public ChatbotController()
        {
            _chatbotService = new ChatbotService();
        }

        [HttpPost]
        public async Task<JsonResult> Enviar(string mensaje)
        {
            string respuesta = await _chatbotService.ObtenerRespuestaAsync(mensaje);
            bool exito = !respuesta.StartsWith("Error al conectar")
                && !respuesta.StartsWith("No pude conectar")
                && !respuesta.StartsWith("Gemini respondió con error")
                && !respuesta.StartsWith("La IA tardó demasiado")
                && !respuesta.StartsWith("No hay una clave");

            return Json(new { exito = exito, respuesta = respuesta });
        }
    }
}