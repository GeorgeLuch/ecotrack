using EcoTrackAPI.Models;
using EcoTrackAPI.ViewModels;

namespace EcoTrackAPI.Services
{
    public interface IComplianceService
    {
        Task<Compliance> RegistrarAsync(ComplianceViewModel model);
    }
}