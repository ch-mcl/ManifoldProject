using System;
using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryReaderExtensions
    {
        public static void JumpToAddress(this BinaryReader reader, Pointer pointer, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            reader.BaseStream.Seek(pointer.address, seekOrigin);
        }

        public static void JumpToAddress(this BinaryReader reader, ArrayPointer arrayPointer, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            reader.BaseStream.Seek(arrayPointer.address, seekOrigin);
        }

        public static Pointer GetPositionAsPointer(this BinaryReader reader)
        {
            return new Pointer(reader.BaseStream.Position);
        }
    }
}
