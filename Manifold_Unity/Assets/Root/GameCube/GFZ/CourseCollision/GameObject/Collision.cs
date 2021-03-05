﻿using Manifold.IO;
using System.IO;
using UnityEngine;
using System;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Collision : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public uint unk_0x10;
        public int triCount;
        public int quadCount;
        public uint triAbsPtr;
        public uint quadAbsPtr;

        public CollisionTri[] tris;
        public CollisionQuad[] quads;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref triCount);
                reader.ReadX(ref quadCount);
                reader.ReadX(ref triAbsPtr);
                reader.ReadX(ref quadAbsPtr);
            }
            this.RecordEndAddress(reader);
            {
                if (triCount > 0)
                {
                    reader.BaseStream.Seek(triAbsPtr, SeekOrigin.Begin);
                    reader.ReadX(ref tris, triCount, true);
                }

                if (quadCount > 0)
                {
                    reader.BaseStream.Seek(quadAbsPtr, SeekOrigin.Begin);
                    reader.ReadX(ref quads, quadCount, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(unk_0x08);
            writer.WriteX(unk_0x0C);
            writer.WriteX(unk_0x10);
            writer.WriteX(triCount);
            writer.WriteX(quadCount);
            writer.WriteX(triAbsPtr);
            writer.WriteX(quadAbsPtr);

            // You need to implement saving the tris/quads
            throw new NotImplementedException();
        }


        #endregion

    }
}