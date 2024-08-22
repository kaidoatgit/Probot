using System.Text.RegularExpressions;

namespace ProPayments.Client.Extensions;

public static class ValidationExtensions
{
    private const string ProductKeyPattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";
    private const string AlphabotKeyPattern = @"^u[a-zA-Z0-9]+-[a-zA-Z0-9]+$";

    public static bool IsProductKeyFormat(this string code)
    {
        code = code.Trim();
        return Regex.IsMatch(code, ProductKeyPattern);
    }

    public static bool IsAlphabotKeyFormat(this string key)
    {
        key = key.Trim();
        return Regex.IsMatch(key, AlphabotKeyPattern);
    }
}