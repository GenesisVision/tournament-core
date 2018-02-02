using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Tournament.Core.Models
{
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }

        public static OperationResult Ok()
        {
            return new OperationResult
                   {
                       IsSuccess = true,
                       Errors = new List<string>()
                   };
        }

        public static OperationResult Failed()
        {
            return new OperationResult
                   {
                       IsSuccess = false,
                       Errors = new List<string>()
                   };
        }

        public static OperationResult Failed(string error)
        {
            return new OperationResult
                   {
                       IsSuccess = false,
                       Errors = new List<string> {error}
                   };
        }

        public static OperationResult Failed(IEnumerable<string> errors)
        {
            return new OperationResult
                   {
                       IsSuccess = false,
                       Errors = errors.ToList()
                   };
        }
    }

    public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }

        public static OperationResult<T> Ok()
        {
            return new OperationResult<T>
                   {
                       IsSuccess = true,
                       Errors = new List<string>()
                   };
        }

        public static OperationResult<T> Ok(T result)
        {
            return new OperationResult<T>
                   {
                       IsSuccess = true,
                       Errors = new List<string>(),
                       Data = result
                   };
        }

        public static OperationResult<T> Failed()
        {
            return new OperationResult<T>
                   {
                       IsSuccess = false,
                       Errors = new List<string>()
                   };
        }

        public static OperationResult<T> Failed(string error)
        {
            return new OperationResult<T>
                   {
                       IsSuccess = false,
                       Errors = new List<string> {error}
                   };
        }

        public static OperationResult<T> Failed(IEnumerable<string> errors)
        {
            return new OperationResult<T>
                   {
                       IsSuccess = false,
                       Errors = errors.ToList()
                   };
        }
    }
}
