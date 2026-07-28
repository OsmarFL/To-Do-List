using SIstema_To_Do_List.Modelos;

namespace SIstema_To_Do_List.Interfaces
{
    public interface ITarea
    {
        // Obtener todas las tareas, con filtro opcional por estado
        List<Tarea> GetTareas(string? status);

        // Obtener una tarea por su ID
        Tarea? GetTareaById(int id);

        // Crear una nueva tarea
        string SetTarea(Tarea model);

        // Actualizar título, descripción y fecha límite
        string UpdateTarea(int id, Tarea model);

        // Marcar una tarea como completada
        string CompletarTarea(int id);

        // Eliminar una tarea
        string DeleteTarea(int id);
    }
}
