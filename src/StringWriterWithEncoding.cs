using System;
using System.IO;
using System.Text;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public class StringWriterWithEncoding : StringWriter
    {
        public StringWriterWithEncoding(Encoding encoding)
        {
            encoding.ThrowIfNull(nameof(encoding));
            Encoding = encoding;
        }

        /// <summary>Initializes a new instance of the <see cref="StringWriter"></see> class with the specified format control.</summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"></see> object that controls formatting.</param>
        /// <param name="encoding">The encoding to report.</param>
        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding)
            : base(formatProvider)
        {
            encoding.ThrowIfNull(nameof(encoding));
            Encoding = encoding;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriter"></see> class that writes to the specified
        ///     <see cref="StringBuilder"></see>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"></see> object to write to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sb">sb</paramref> is null.</exception>
        /// <param name="encoding">The encoding to report.</param>
        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            encoding.ThrowIfNull(nameof(encoding));
            Encoding = encoding;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriter"></see> class that writes to the specified
        ///     <see cref="StringBuilder"></see> and has the specified format provider.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"></see> object to write to.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"></see> object that controls formatting.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sb">sb</paramref> is null.</exception>
        /// <param name="encoding">The encoding to report.</param>
        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider,
            Encoding encoding)
            : base(sb, formatProvider)
        {
            encoding.ThrowIfNull(nameof(encoding));
            Encoding = encoding;
        }
 public override Encoding Encoding { get; }
    }
}