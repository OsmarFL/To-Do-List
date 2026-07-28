using Microsoft.AspNetCore.Mvc;
using SIstema_To_Do_List.Interfaces;
using SIstema_To_Do_List.Modelos;

namespace SIstema_To_Do_List.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareaController : ControllerBase
    {
        private readonly ITarea service;

        public TareaController(ITarea service)
        {
            this.service = service;
        }

        // GET /api/tarea
        // GET /api/tarea?status=Completada
        [HttpGet]
        public IActionResult GetTareas([FromQuery] string? status)
        {
            var tareas = service.GetTareas(status);
            return Ok(tareas); // 200
        }

        // GET /api/tarea/{id}
        [HttpGet("{id}")]
        public IActionResult GetTareaById(int id)
        {
            var tarea = service.GetTareaById(id);

            // Si no existe retorna 404
            if (tarea == null)
                return NotFound(new { mensaje = "Tarea no encontrada" }); // 404

            return Ok(tarea); // 200
        }

        // POST /api/tarea
        [HttpPost]
        public IActionResult SetTarea([FromBody] Tarea model)
        {
            // Si el modelo no pasa las validaciones ([Required]) retorna 400
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            var resultado = service.SetTarea(model);
            return CreatedAtAction(nameof(GetTareaById), new { id = model.Id }, resultado); // 201
        }

        // PUT /api/tarea/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTarea(int id, [FromBody] Tarea model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            var resultado = service.UpdateTarea(id, model);

            if (resultado == "Tarea no encontrada")
                return NotFound(new { mensaje = resultado }); // 404

            return Ok(new { mensaje = resultado }); // 200
        }

        // PATCH /api/tarea/{id}/complete
        [HttpPatch("{id}/complete")]
        public IActionResult CompletarTarea(int id)
        {
            var resultado = service.CompletarTarea(id);

            if (resultado == "Tarea no encontrada")
                return NotFound(new { mensaje = resultado }); // 404

            if (resultado == "La tarea ya está completada")
                return BadRequest(new { mensaje = resultado }); // 400

            return Ok(new { mensaje = resultado }); // 200
        }

        // DELETE /api/tarea/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTarea(int id)
        {
            var resultado = service.DeleteTarea(id);

            if (resultado == "Tarea no encontrada")
                return NotFound(new { mensaje = resultado }); // 404

            return NoContent(); // 204
        }
    }
}
