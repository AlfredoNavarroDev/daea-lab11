using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab11.Models;

public class Evento
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }

    [Required]
    [StringLength(100)]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000)]
    public int Aforo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Precio { get; set; }

    public int CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public Categoria? Categoria { get; set; }

    [Required]
    [StringLength(20)]
    public string Modalidad { get; set; } = "Presencial";

    public bool Activo { get; set; } = true;
}
