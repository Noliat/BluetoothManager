using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;

public static class AsyncOperationExtensions
{
    public static TaskAwaiter<TResult> GetAwaiter<TResult>(this IAsyncOperation<TResult> operation)
    {
        var tcs = new TaskCompletionSource<TResult>();
        operation.Completed = (asyncOperation, status) =>
        {
            switch (status)
            {
                case AsyncStatus.Completed:
                    tcs.TrySetResult(asyncOperation.GetResults());
                    break;
                case AsyncStatus.Error:
                    tcs.TrySetException(asyncOperation.ErrorCode);
                    break;
                case AsyncStatus.Canceled:
                    tcs.TrySetCanceled();
                    break;
            }
        };
        return tcs.Task.GetAwaiter();
    }
}
