using System.ComponentModel.DataAnnotations;

namespace Probot.Data.Entities;
public class ProRaffleSetting : ProductSetting
{
    public string Key { get; set; } = null!;
    public bool IsPaused { get; set; } = false;
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}