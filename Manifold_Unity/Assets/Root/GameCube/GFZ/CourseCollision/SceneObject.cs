﻿using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Binds an object name to a loadable display model.
    /// </summary>
    [Serializable]
    public class SceneObject :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public uint zero_0x00;
        public Pointer namePtr;
        public uint zero_0x08;
        public float unk_0x0C; // LOD?
        // REFERENCE FIELDS
        public CString name;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref namePtr);
                reader.ReadX(ref zero_0x08);
                reader.ReadX(ref unk_0x0C);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x00 == 0);
                Assert.IsTrue(zero_0x08 == 0);

                reader.JumpToAddress(namePtr);
                reader.ReadX(ref name, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero_0x00 == 0);
                Assert.IsTrue(zero_0x08 == 0);

                namePtr = name.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zero_0x00);
                writer.WriteX(namePtr);
                writer.WriteX(zero_0x08);
                writer.WriteX(unk_0x0C);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object name.
            Assert.IsTrue(namePtr.IsNotNullPointer);
            Assert.IsTrue(name != null);
            Assert.ReferencePointer(name, namePtr);
            // HACK: this is because game data DOES have null names... why?
            //Assert.IsTrue(!string.IsNullOrEmpty(name.value));

            // Constants
            Assert.IsTrue(zero_0x00 == 0);
            Assert.IsTrue(zero_0x08 == 0);
        }

        public override string ToString()
        {
            return
                $"{nameof(SceneObject)}(" +
                $"{nameof(unk_0x0C)}: {unk_0x0C}, " +
                $"Name: {name}" +
                $")";
        }
    }
}