using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public bool IsGlutenFree { get; set; }
}   