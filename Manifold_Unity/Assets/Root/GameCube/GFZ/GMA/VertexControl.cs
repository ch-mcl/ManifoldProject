﻿using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class VertexControlHeader : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        public const int kFifoPaddingSize = 12;

        [Header("Vertex Control Header")]
        [SerializeField]
        private AddressRange addressRange;

        [Space]
        [SerializeField, Hex("00", 8)]
        int vertexCount;

        [SerializeField, Hex("04", 8)]
        int vertexControlT1RelPtr;

        [SerializeField, Hex("08", 8)]
        int vertexControlT2RelPtr;

        [SerializeField, Hex("0C", 8)]
        int vertexControlT3RelPtr;

        [SerializeField, Hex("10", 8)]
        int vertexControlT4RelPtr;

        byte[] fifoPadding;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public int VertexCount
        {
            get => vertexCount;
            set => vertexCount = value;
        }

        public int VertexControlT1RelPtr
        {
            get => vertexControlT1RelPtr;
            set => vertexControlT1RelPtr = value;
        }

        public int VertexControlT2RelPtr
        {
            get => vertexControlT2RelPtr;
            set => vertexControlT2RelPtr = value;
        }

        public int VertexControlT3RelPtr
        {
            get => vertexControlT3RelPtr;
            set => vertexControlT3RelPtr = value;
        }

        public int VertexControlT4RelPtr
        {
            get => vertexControlT4RelPtr;
            set => vertexControlT4RelPtr = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref vertexCount);
                reader.ReadX(ref vertexControlT1RelPtr);
                reader.ReadX(ref vertexControlT2RelPtr);
                reader.ReadX(ref vertexControlT3RelPtr);
                reader.ReadX(ref vertexControlT4RelPtr);
                reader.ReadX(ref fifoPadding, kFifoPaddingSize);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(vertexCount);
            writer.WriteX(vertexControlT1RelPtr);
            writer.WriteX(vertexControlT2RelPtr);
            writer.WriteX(vertexControlT3RelPtr);
            writer.WriteX(vertexControlT4RelPtr);
            var align = writer.AlignTo(GameCube.GX.GXUtility.GX_FIFO_ALIGN);
            Assert.IsTrue(align == kFifoPaddingSize);
        }


        #endregion

    }
}
