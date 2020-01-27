using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class TaskExtensions
    {
        [UsedImplicitly]
        public static async Task IgnoringCancellation([NotNull] this Task task, CancellationToken token)
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