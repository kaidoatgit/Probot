using Probot.Data.Entities;
using Probot.Shared.Dtos.ProRaffleSetting.Response;
using Riok.Mapperly.Abstractions;

namespace Probot.ProRaffleTool.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        public partial ProRaffleSettingResponse MapToProRaffleSettingResponse(ProRaffleSetting proRaffleSetting); 
    }
}
