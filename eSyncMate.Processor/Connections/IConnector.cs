using eSyncMate.Processor.Models;
using eSyncMate.Processor.Connections;

namespace eSyncMate.Processor.Connections
{
    public interface IConnector
    {
        public static string Token;

        public static string BaseURL;

        Task GetApiToken();
    }
}
