using Pilgrims.PersonalFinances.Models.DTOs;

namespace Pilgrims.PersonalFinances.Services
{
    public interface IExportService
    {
        Task ExportToPdfAsync(string reportType, object reportData, string fileName);
        Task ExportToCsvAsync(string reportType, object reportData, string fileName);
        Task ExportToImageAsync(string elementId, string fileName);
        Task<string> GenerateReportHtmlAsync(string reportType, object reportData);
    }
}