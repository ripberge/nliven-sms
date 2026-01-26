using System.ComponentModel.DataAnnotations.Schema;

namespace SmsService.Core.Entities;

public class ProviderName
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public string KatsNewProperty { get; set; } = "DefaultValue";
}
