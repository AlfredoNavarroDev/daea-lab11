using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Models;

[Index(nameof(Nombre), IsUnique = true)]
public class Categoria
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
