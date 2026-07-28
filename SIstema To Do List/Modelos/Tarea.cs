using System.ComponentModel.DataAnnotations;

namespace SIstema_To_Do_List.Modelos
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }

        // [Required] indica que este campo (Titulo) es obligatorio.
        [Required]
        public string? Titulo { get; set; }

        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaLimite { get; set; }

        // Estado posible: "Pendiente" o "Completada"
        public string Estado { get; set; } = "Pendiente";
    }
}
