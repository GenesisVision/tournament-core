using System;

namespace GenesisVision.Tournament.Core.Models
{
    public static class InvokeOperations
    {
        public static OperationResult InvokeOperation(Action action)
        {
            try
            {
                action();
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return OperationResult.Failed(ex.Message);
            }
        }

        public static OperationResult<T> InvokeOperation<T>(Func<T> func)
        {
            try
            {
                var res = func();
                return OperationResult<T>.Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return OperationResult<T>.Failed(ex.Message);
            }
        }
    }
}
