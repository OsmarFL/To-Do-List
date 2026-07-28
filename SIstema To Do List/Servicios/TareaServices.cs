using SIstema_To_Do_List.Interfaces;
using SIstema_To_Do_List.Modelos;
using System.Text.Json;

namespace SIstema_To_Do_List.Servicios
{
    public class TareaServices : ITarea
    {
        // Ruta del archivo JSON donde se persisten las tareas.
        // Se guarda en la raíz del proyecto.
        private readonly string _archivoJson = "tasks.json";

        // ========================================
        // MÉTODOS PRIVADOS DE LECTURA Y ESCRITURA
        // ========================================

        // Lee el archivo JSON y retorna la lista de tareas.
        // Si el archivo no existe, lo crea vacío automáticamente.
        private List<Tarea> LeerArchivo()
        {
            if (!File.Exists(_archivoJson))
            {
                File.WriteAllText(_archivoJson, "[]");
                return new List<Tarea>();
            }

            var contenido = File.ReadAllText(_archivoJson);

            // Manejo de JSON corrupto: si falla la deserialización,
            // retorna lista vacía para no romper la aplicación.
            try
            {
                return JsonSerializer.Deserialize<List<Tarea>>(contenido) ?? new List<Tarea>();
            }
            catch
            {
                return new List<Tarea>();
            }
        }

        // Escribe la lista de tareas en el archivo JSON.
        // Usa WriteIndented para que el archivo sea legible.
        private void EscribirArchivo(List<Tarea> tareas)
        {
            var opciones = new JsonSerializerOptions { WriteIndented = true };
            var contenido = JsonSerializer.Serialize(tareas, opciones);
            File.WriteAllText(_archivoJson, contenido);
        }

        // Obtiene la lista de tareas (GET)
        public List<Tarea> GetTareas (string status)
        {
            var tareas = LeerArchivo();

            // Si se pasa un filtro de estado, filtra la lista
            if (!string.IsNullOrEmpty(status))
            {
                tareas = tareas
                    .Where(t => t.Estado.ToLower() == status.ToLower())
                    .ToList();
            }

            return tareas;
        }

        // Obtiene tareas por ID (GET)
        public Tarea? GetTareaById(int id)
        {
            var tareas = LeerArchivo();
            return tareas.FirstOrDefault(t => t.Id == id);
        }

        // (POST)
        public string SetTarea(Tarea model)
        {
            var tareas = LeerArchivo();

            // Genera un ID único basado en el máximo existente
            model.Id = tareas.Count > 0 ? tareas.Max(t => t.Id) + 1 : 1;

            model.FechaCreacion = DateTime.Now;

            model.Estado = "Pendiente";

            tareas.Add(model);
            EscribirArchivo(tareas);

            return "Tarea creada correctamente";
        }

        // (PUT)
        public string UpdateTarea(int id, Tarea model)
        {
            var tareas = LeerArchivo();
            var tarea = tareas.FirstOrDefault(t => t.Id == id);

            if (tarea == null)
                return "Tarea no encontrada";

            // Solo se permite modificar estos 3 campos.
            // La FechaCreacion nunca se toca.
            tarea.Titulo = model.Titulo;
            tarea.Descripcion = model.Descripcion;
            tarea.FechaLimite = model.FechaLimite;

            EscribirArchivo(tareas);
            return "Tarea actualizada correctamente";
        }

        // (PATCH)
        public string CompletarTarea(int id)
        {
            var tareas = LeerArchivo();
            var tarea = tareas.FirstOrDefault(t => t.Id == id);

            if (tarea == null)
                return "Tarea no encontrada";

            // No se permite completar una tarea ya completada
            if (tarea.Estado == "Completada")
                return "La tarea ya está completada";

            tarea.Estado = "Completada";
            EscribirArchivo(tareas);

            return "Tarea marcada como completada";
        }

        // (DELETE)
        public string DeleteTarea(int id)
        {
            var tareas = LeerArchivo();
            var tarea = tareas.FirstOrDefault(t => t.Id == id);

            if (tarea == null)
                return "Tarea no encontrada";

            tareas.Remove(tarea);
            EscribirArchivo(tareas);

            return "Tarea eliminada correctamente";
        }

    }
}
