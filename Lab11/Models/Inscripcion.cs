using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab11.Models;

public class Inscripcion
{
    public int Id { get; set; }

    public int EventoId { get; set; }

    [ForeignKey(nameof(EventoId))]
    public Evento? Evento { get; set; }

    public int AsistenteId { get; set; }

    [ForeignKey(nameof(AsistenteId))]
    public Asistente? Asistente { get; set; }

    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required]
    [StringLength(10)]
    public string Estado { get; set; } = "Confirmada";
}
