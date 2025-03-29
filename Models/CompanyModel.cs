using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class CompanyModel
{
    [Key] // Marker CompanyID som primærnøgle
    public int CompanyID { get; set; } // Unikt ID for virksomheden

    public required string CompanyName { get; set; } // Tilføjet required

    public required string CVR { get; set; } // Tilføjet required
}