using System;

namespace ProPayments.Service.Dtos.ProRaffles.Request;

public class UpdateProRaffleKeyRequest
{
    public string CurrentKey { get; set; }
    public string NewKey { get; set; }
}
