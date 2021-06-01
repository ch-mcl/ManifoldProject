using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownObjectAttributes2 :
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public Pointer collisionObjectReferencePtr;
        // FIELDS (deserialized from pointers)
        public UnknownObjectAttributes collisionObjectReference;


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
                reader.ReadX(ref collisionObjectReferencePtr);
            }
            this.RecordEndAddress(reader);
            {
                reader.JumpToAddress(collisionObjectReferencePtr);
                reader.ReadX(ref collisionObjectReference, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            // write
            {
                // TODO: comments
                collisionObjectReferencePtr = collisionObjectReference.SerializeWithReference(writer).GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(collisionObjectReferencePtr);
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