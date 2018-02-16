using GenesisVision.Tournament.Core.Models;

namespace GenesisVision.Tournament.Core.Services.Interfaces
{
    public interface IIpfsService
    {
        OperationResult<string> GetIpfsText(string hash);
        OperationResult<string> WriteIpfsText(string text);
        OperationResult<byte[]> GetIpfsFile(string hash);
    }
}