
namespace Backbone.Core.Services.Logging
{
    public interface ILogService
    {
        Task Log(string message, string level, string source, string? stackTrace);
    }
}
