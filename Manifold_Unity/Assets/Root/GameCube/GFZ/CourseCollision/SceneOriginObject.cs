using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// This structure points to an object which does not use a transform for placement within the scene.
    /// </summary>
    [Serializable]
    public class SceneOriginObject :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        [UnityEngine.SerializeField] private string nameCopy;

        // FIELDS
        public Pointer sceneInstanceReferencePtr;
        // REFERENCE FIELDS
        public SceneInstanceReference instanceReference;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string NameCopy => nameCopy;

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref sceneInstanceReferencePtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(sceneInstanceReferencePtr.IsNotNullPointer);
                reader.JumpToAddress(sceneInstanceReferencePtr);
                reader.ReadX(ref instanceReference, true);

                nameCopy = instanceReference.nameCopy;
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                sceneInstanceReferencePtr = instanceReference.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(sceneInstanceReferencePtr);
            }
            this.RecordEndAddress(writer);

        }

        public void ValidateReferences()
        {
            Assert.IsTrue(sceneInstanceReferencePtr.IsNotNullPointer);
        }


        public override string ToString()
        {
            return 
                $"{nameof(SceneOriginObject)}(" +
                $"Name: {nameCopy}" +
                $")";
        }
    }
}