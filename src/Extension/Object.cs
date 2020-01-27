using System;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class ObjectExtensions
    {
        #region ThrowIf

        /// <summary>
        ///     Throws an ArgumentNullException if the given data item is null.
        /// </summary>
        /// <param name="data">The item to check for nullity.</param>
        /// <param name="name">The name to use when throwing an exception, if necessary</param>
        /// <exception cref="ArgumentNullException"><paramref name="data" /> is <c>null</c>.</exception>
        [UsedImplicitly]
        [ContractAnnotation("data:null => halt")]
        public static void ThrowIfNull([CanBeNull] this object data, string name = null)
        {
            if (data == null)
            {
                throw name == null ? new ArgumentNullException() : new ArgumentNullException(name);
            }
        }

        #endregion
    }
}