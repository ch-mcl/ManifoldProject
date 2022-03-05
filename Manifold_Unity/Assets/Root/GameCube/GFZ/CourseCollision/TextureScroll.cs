﻿using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    // NOTES:
    // Indexes 0-3 ARE USED
    //  idx0: uv.xy scrolling (or, at least, on some models)
    //  idx1: ?
    //  idx2: ?
    //  idx3: ?
    // Indexes 4-11 unused, always (0f, 0f)

    /// <summary>
    /// Texture metadata. In some instasnces defines how a texture scrolls.
    /// </summary>
    [Serializable]
    public class TextureScroll :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // CONSTANTS
        public const int kCount = 12;


        // FIELDS
        public Pointer[] fieldPtrs;
        // REFERENCE FIELDS
        public TextureScrollField[] fields = new TextureScrollField[0];


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref fieldPtrs, kCount);
            }
            this.RecordEndAddress(reader);
            {
                fields = new TextureScrollField[kCount];
                for (int i = 0; i < kCount; i++)
                {
                    var pointer = fieldPtrs[i];
                    if (pointer.IsNotNull)
                    {
                        reader.JumpToAddress(pointer);
                        reader.ReadX(ref fields[i]);
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                fieldPtrs = fields.GetPointers();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(fieldPtrs);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Validate each field/pointer
            for (int i = 0; i < kCount; i++)
            {
                // reference can be to float2(0, 0)
                Assert.ReferencePointer(fields[i], fieldPtrs[i]);

                //if (fields[i] != null)
                //    Assert.IsTrue(fields[i].x != 0 && fields[i].y != 0);
            }
        }

        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(nameof(TextureScroll));
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] == null)
                    continue;
                stringBuilder.Append($"[{i}]{fields[i]}, ");
            }

            return stringBuilder.ToString();
        }
    }
}