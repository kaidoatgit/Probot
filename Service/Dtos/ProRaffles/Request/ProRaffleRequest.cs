using System;
using System.ComponentModel.DataAnnotations;
using ProPayments.Service.Dtos.UserSettings.Request;

namespace ProPayments.Service.Dtos.ProRaffles.Request;

public class ProRaffleRequest : UserSettingRequest
{
    [Required]
    public string AlphabotKey { get; set; }
}
