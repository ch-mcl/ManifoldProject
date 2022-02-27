﻿using Manifold;
using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// Appears to be a GX vertex meant for skinning. It is not part of a formal
    /// display list. The GXAttributes properly describe the intial position,
    /// normal, textureUV0, and textureUV1. The data afterwards is somewhat strange.
    /// The color values may not be true colors, but some kind of weight painting.
    /// </summary>
    public class SkinnedVertexB :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private float3 position;
        private float3 normal;
        private float2 textureUV0;
        private float2 textureUV1;
        private uint zero0x28;
        private uint zero0x2C;
        private uint color1; // RGBA. Appears to truly be a color.
        private uint unk0x34; // Magic bits: 00000000, 00010100, 02000002, 01000001, 03000003
        private uint color2; // RGBA. Color-looking, but does use alpha channel.
        private uint color3; // RGBA. Color-looking, but does use alpha channel. 00000004 is the only magic-bit looking value.

        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref normal);
                reader.ReadX(ref textureUV0);
                reader.ReadX(ref textureUV1);
                reader.ReadX(ref zero0x28);
                reader.ReadX(ref zero0x2C);
                reader.ReadX(ref color1);
                reader.ReadX(ref unk0x34);
                reader.ReadX(ref color2);
                reader.ReadX(ref color3);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero0x28 == 0);
                Assert.IsTrue(zero0x2C == 0);

                UnityEngine.Debug.LogError($"{nameof(color1)} {color1:x8}");
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                //writer.WriteX();
            }
            this.RecordEndAddress(writer);

            throw new System.NotImplementedException();
        }

    }

}