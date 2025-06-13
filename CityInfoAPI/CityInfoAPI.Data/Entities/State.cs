using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Data.Entities;

public class State
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = $"{nameof(StateGuid)} is required.")]
    [MaxLength(50, ErrorMessage = $"{nameof(StateGuid)} cannot exceed 50 characters.")]
    public Guid StateGuid { get; set; }

    [Required(ErrorMessage = $"{nameof(StateCode)} is required.")]
    [MaxLength(2, ErrorMessage = $"{nameof(StateCode)} cannot exceed 2 characters.")]
    public string StateCode { get; set; }
}
