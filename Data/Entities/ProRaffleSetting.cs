using System.ComponentModel.DataAnnotations;

namespace Probot.Data.Entities;
public class ProRaffleSetting : ProductSetting
{
    public string Key { get; set; } = null!;
    public bool IsPaused { get; set; } = false;
    [ConcurrencyCheck]
    public Guid Version { get; set; }

    public bool IsRegisteredAlertEnabled { get; set; } = false;
    public string RegisterAlertId { get; set; } = string.Empty;
    public string RegisterAlertToken { get; set; } = string.Empty;

    public bool IsErrorAlertEnabled { get; set; } = false;
    public string ErrorAlertId { get; set; } = string.Empty;
    public string ErrorAlertToken { get; set; } = string.Empty;
}