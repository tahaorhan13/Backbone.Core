using Backbone.Core.Services.Logging.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backbone.Core.Services.Logging
{
    public class LogService : ILogService
    {
        private readonly IMongoCollection<Logs> _logCollection;

        public LogService(IOptions<MongoDbSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _logCollection = database.GetCollection<Logs>(mongoSettings.Value.CollectionName);
        }

        public async Task Log(string message,string level,string source,string? stackTrace)
        {
            var appLog = new Logs
            {
                Message = message,
                Level = level,
                Source = source,
                StackTrace = stackTrace
            };

            await _logCollection.InsertOneAsync(appLog);
        }
    }

}
