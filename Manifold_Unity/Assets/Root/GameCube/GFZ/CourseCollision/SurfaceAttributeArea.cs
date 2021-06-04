using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class SurfaceAttributeArea :
        IBinaryAddressable,
        IBinarySerializable
    {
        [UnityEngine.SerializeField] private AddressRange addressRange;

        public float lengthFrom;
        public float lengthTo;
        public float widthLeft;
        public float widthRight;
        public SurfaceAttribute surfaceAttribute;
        public byte trackBranchID;
        public byte zero_0x12;
        public byte zero_0x13;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref lengthFrom);
                reader.ReadX(ref lengthTo);
                reader.ReadX(ref widthLeft);
                reader.ReadX(ref widthRight);
                reader.ReadX(ref surfaceAttribute);
                reader.ReadX(ref trackBranchID);
                reader.ReadX(ref zero_0x12);
                reader.ReadX(ref zero_0x13);
            }
            this.RecordEndAddress(reader);

            Assert.IsTrue(zero_0x12 == 0);
            Assert.IsTrue(zero_0x13 == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            Assert.IsTrue(zero_0x12 == 0);
            Assert.IsTrue(zero_0x13 == 0);

            this.RecordStartAddress(writer);
            {
                writer.WriteX(lengthFrom);
                writer.WriteX(lengthTo);
                writer.WriteX(widthLeft);
                writer.WriteX(widthRight);
                writer.WriteX(surfaceAttribute);
                writer.WriteX(trackBranchID);
                writer.WriteX(zero_0x12);
                writer.WriteX(zero_0x13);
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }
    }
}
