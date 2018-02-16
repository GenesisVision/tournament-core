using GenesisVision.Tournament.Core.Infrastructure;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using Ipfs.Api;
using System.IO;

namespace GenesisVision.Tournament.Core.Services
{
    public class IpfsService : IIpfsService
    {
        private readonly IpfsClient ipfs;

        public IpfsService()
        {
            ipfs = new IpfsClient(Constants.IpfsHost);
        }

        public OperationResult<string> GetIpfsText(string hash)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var data = ipfs.FileSystem.ReadAllTextAsync(hash).Result;
                return data;
            });
        }

        public OperationResult<string> WriteIpfsText(string text)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var res = ipfs.FileSystem.AddTextAsync(text).Result;
                return res.Id.Hash.ToString();
            });
        }

        public OperationResult<byte[]> GetIpfsFile(string hash)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                using (var stream = ipfs.FileSystem.ReadFileAsync(hash).Result)
                using (var data = new MemoryStream())
                {
                    stream.CopyTo(data);
                    return data.ToArray();
                }
            });
        }
    }
}
