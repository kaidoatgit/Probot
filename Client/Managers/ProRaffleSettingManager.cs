using Probot.Client.Clients.ProRaffleApi;
using Probot.Client.Helpers;
using Probot.Client.Mappers;
using Probot.Client.Models;
using Probot.Shared.Enums;
using Probot.Shared.Helpers;

namespace Probot.Client.Managers;

public class ProRaffleSettingManager
{
    private readonly ProRaffleSettingClient _proRaffleSettingClient;
    private readonly Mapper _mapper;
    public ProRaffleSettingManager(ProRaffleSettingClient proRaffleSettingClient, Mapper mapper)
    {
        _proRaffleSettingClient = proRaffleSettingClient;
        _mapper = mapper;
    }

    internal async Task<(string content, IEnumerable<ProRaffleSetting> settings)>  GetUserSettingsAsync(ulong userId)
    {
        var apiResponse = await _proRaffleSettingClient.GetSettingsAsync(userId);
        if(apiResponse.Data == null)
        {
            return (string.Empty, Enumerable.Empty<ProRaffleSetting>().AsEnumerable());
        }
        string content = ProRaffleHelper.CreateNotificationMessageContent(apiResponse.Data);
        return (content, apiResponse.Data.Select(prs => _mapper.MapToProRaffleSetting(prs)).ToList());
    }

    internal async Task<(bool, string)> DisableAlertAsync(ulong settingId, RaffleAlertType raffleAlertType, ulong userId)
    {
        var apiResponse = await _proRaffleSettingClient.DisableAlertAsync(settingId, raffleAlertType);
        bool isModified = apiResponse.Data;
        if (!isModified)
        {              
            if(apiResponse.ExceptionResult == ExceptionResult.NotModified304)
            {
                return (false, MessageHelper.AlertAlreadyDisabled); 
            }
            else
            {
                return (false, MessageHelper.GenericErrorMessage()); 
            }
        }
        else
        {
            return (true, (await GetUserSettingsAsync(userId)).content);
        }
    }
}
