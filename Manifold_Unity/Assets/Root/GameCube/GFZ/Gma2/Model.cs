﻿using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GFZ.Gma2
{
    public class Model :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private ShiftJisCString name;
        private Gcmf gcmf;
        
        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public Pointer GcmfPtr { get; set; }
        public Pointer NamePtr { get; set; }
        public ShiftJisCString Name { get => name; set => name = value; }
        public Gcmf Gcmf { get => gcmf; set => gcmf = value; }

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            // Pointers are assigned from outside the class.
            // If not set, Deserialization is not expected to be called.
            Assert.IsTrue(GcmfPtr.IsNotNull);
            Assert.IsTrue(NamePtr.IsNotNull);

            reader.JumpToAddress(NamePtr);
            reader.ReadX(ref name, true);

            reader.JumpToAddress(GcmfPtr);
            reader.ReadX(ref gcmf, true);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
