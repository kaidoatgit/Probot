using System.Text;
using Probot.Shared.Dtos.ProRaffleSetting.Response;

namespace Probot.Shared.Helpers;

public static class ProRaffleHelper
{
    /*
    * This method is used to create the content of a discord message with the updated notifications settings.
    * Is used by:
    * - ProRaffleTool after enable the register raffle alerts
    * - Probot client after sending a disable alert request for register or error
    */
    public static string CreateNotificationMessageContent(IEnumerable<ProRaffleSettingResponse> settings)
    {
        StringBuilder content = new("```ansi\n");
        foreach (var setting in settings)
        {
            string registeredAlertResult = setting.IsRegisteredAlertEnabled 
                ? $"\u001b[1;32m{EmojisHelper.Bell} Enabled\u001b[0m"
                : $"\u001b[1;31m{EmojisHelper.X} Disabled\u001b[0m";
            string errorAlertResult = setting.IsErrorAlertEnabled 
                ? $"\u001b[1;32m{EmojisHelper.Bell} Enabled\u001b[0m"
                : $"\u001b[1;31m{EmojisHelper.X} Disabled\u001b[0m";

            content.AppendLine($"\u001b[0;40m {EmojisHelper.User} {setting.Username} \u001b[0m");
            content.AppendLine($"Register Alerts: {registeredAlertResult}");
            content.AppendLine($"Error Alerts: {errorAlertResult}");
            content.AppendLine();
        }
        content.Append("```");
        return content.ToString();
    }
}
