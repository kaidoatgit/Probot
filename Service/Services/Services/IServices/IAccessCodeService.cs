using System;
using ProPayments.Service.Data.Entities;

namespace ProPayments.Service.Services.Services.IServices;

public interface IAccessCodeService
{
    Task<List<AccessCode>> GenerateCodes(List<PlanOption> planOptions);
}
