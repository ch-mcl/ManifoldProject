using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// An object which does not use a transform for placement within the scene. The model's
    /// origin will align with the scene's origin.
    /// 
    /// This kind of object was used for all test objects in old AX scenes.
    /// </summary>
    [Serializable]
    public class SceneObjectStatic :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Pointer sceneObjectPtr;
        // REFERENCE FIELDS
        public SceneObject sceneObject;

        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string Name => sceneObject.Name;

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref sceneObjectPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(sceneObjectPtr.IsNotNullPointer);
                reader.JumpToAddress(sceneObjectPtr);
                reader.ReadX(ref sceneObject, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                sceneObjectPtr = sceneObject.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(sceneObjectPtr);
            }
            this.RecordEndAddress(writer);

        }

        public void ValidateReferences()
        {
            Assert.IsTrue(sceneObjectPtr.IsNotNullPointer);
        }


        public override string ToString()
        {
            return 
                $"{nameof(SceneObjectStatic)}(" +
                $"Name: {Name}" +
                $")";
        }
    }
}