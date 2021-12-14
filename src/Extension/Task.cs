using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class TaskExtensions
    {
        public static async Task IgnoringCancellation(this Task task, CancellationToken token)
        {
            task.ThrowIfNull(nameof(task));

            try
            {
                await task;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}