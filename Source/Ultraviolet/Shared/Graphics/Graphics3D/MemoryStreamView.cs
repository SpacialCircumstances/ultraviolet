using System;
using System.IO;
using Ultraviolet.Core;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents a read-only view into a larger <see cref="MemoryStream"/> object.
    /// </summary>
    internal sealed class MemoryStreamView : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStreamView"/> class.
        /// </summary>
        /// <param name="baseStream">The base stream which provides this stream's underlying data.</param>
        /// <param name="offset">The offset into <paramref name="baseStream"/> at which this stream begins reading data.</param>
        /// <param name="length">The number of bytes which this stream can read from <paramref name="baseStream"/>.</param>
        public MemoryStreamView(MemoryStream baseStream, Int64 offset, Int64 length)
        {
            Contract.Require(baseStream, nameof(baseStream));
            Contract.EnsureRange(offset >= 0, nameof(offset));
            Contract.EnsureRange(length >= 0, nameof(length));

            this.BaseStream = baseStream;
            this.offset = offset;
            this.length = length;
        }

        /// <inheritdoc/>
        public override Int64 Seek(Int64 offset, SeekOrigin origin)
        {
            Contract.EnsureNotDisposed(this, BaseStream == null);
            Contract.EnsureRange(offset >= 0 && offset <= length, nameof(offset));

            var seekpos = 0L;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    seekpos = offset;
                    break;

                case SeekOrigin.Current:
                    seekpos = position + offset;
                    break;

                case SeekOrigin.End:
                    seekpos = length + offset;
                    break;
            }

            if (seekpos < 0 || seekpos > length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            BaseStream.Seek(this.offset + seekpos, SeekOrigin.Begin);
            position = seekpos;
            return position;
        }

        /// <inheritdoc/>
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
        {
            Contract.EnsureNotDisposed(this, BaseStream == null);

            var remaining = length - position;
            if (remaining <= 0)
                return 0;

            if (remaining < count)
                count = (Int32)remaining;

            BaseStream.Seek(position, SeekOrigin.Begin);
            var read = BaseStream.Read(buffer, offset, count);
            position += read;

            return read;
        }

        /// <inheritdoc/>
        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            Contract.EnsureNotDisposed(this, BaseStream == null);

            BaseStream.Flush();
        }

        /// <inheritdoc/>
        public override void SetLength(Int64 value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Boolean CanRead => true;

        /// <inheritdoc/>
        public override Boolean CanWrite => false;

        /// <inheritdoc/>
        public override Boolean CanSeek => true;

        /// <inheritdoc/>
        public override Int64 Length
        {
            get
            {
                Contract.EnsureNotDisposed(this, BaseStream == null);

                return length;
            }
        }

        /// <inheritdoc/>
        public override Int64 Position
        {
            get
            {
                Contract.EnsureNotDisposed(this, BaseStream == null);

                return position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Gets the view's offset in bytes from the beginning of the underlying stream.
        /// </summary>
        public Int64 Offset => offset;

        /// <summary>
        /// Gets the base stream for this view.
        /// </summary>
        public MemoryStream BaseStream { get; private set; }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (BaseStream != null)
                {
                    BaseStream.Dispose();
                    BaseStream = null;
                }
            }
        }

        // Stream parameters.
        private readonly Int64 offset;
        private readonly Int64 length;
        private Int64 position;
    }
}
