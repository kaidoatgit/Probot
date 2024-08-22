using System;

namespace ProPayments.Client.Dtos.ProductKey.Request;

public class UpdateProRaffleKeyRequest
{
    public string CurrentKey { get; set; }
    public string NewKey { get; set; }
}
