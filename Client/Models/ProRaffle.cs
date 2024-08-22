using System;

namespace ProPayments.Client.Models;

public class ProRaffle
{
    public string Key { get; set; }
    public bool IsPaused { get; set; }
    public Subscription Subscription { get; set; }
}