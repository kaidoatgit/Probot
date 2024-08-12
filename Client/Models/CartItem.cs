using System;
using System.Text;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Models;

public class CartItem
{
    public int ItemId { get; set; }
    public string SelectedPlan { get; set; } = string.Empty;
    public string SelectedDuration { get; set; } = string.Empty;
    public decimal Price { get; set;}
    public int PlanOptionId { get; set; }
    public override string ToString()
    {
        StringBuilder cartItem = new();
        cartItem.Append($"{ItemId.ToString().PadRight(3)}| ");
        cartItem.Append($"{SelectedPlan.PadRight(16)}| ");
        cartItem.Append($"{SelectedDuration.PadRight(14)}| ");
        cartItem.Append($"${Price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).PadRight(8)}");
        return cartItem.ToString();
    }
}
