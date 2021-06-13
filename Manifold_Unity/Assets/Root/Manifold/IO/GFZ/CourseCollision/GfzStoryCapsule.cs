using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzStoryCapsule : MonoBehaviour,
        IGfzConvertable<CourseMetadataTrigger>
    {
        /// <summary>
        /// Capsule trigger scale (when compared to default Unity sphere).
        /// THIS IS STILL A GUESS.
        /// </summary>
        public const float scale = 20f; // maybe 27.5 like BBO?

        public enum CapsuleType
        {
            GxStory5 = 0,
            AxStory1 = 1,
        }

        // INSPECTOR FIELDS
        [SerializeField] private CapsuleType type;

        // PROPERTIES
        public CapsuleType Type
        {
            get => type;
            set => type = value;
        }

        // METHODS
        public CourseMetadataTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransform(this.transform);

            // Select which capsule type flag to use.
            CourseMetadataType metadataType = (CourseMetadataType)(-1);
            switch (type)
            {
                case CapsuleType.AxStory1:
                    metadataType = CourseMetadataType.Story1_CapsuleAX;
                    break;

                case CapsuleType.GxStory5:
                    metadataType = CourseMetadataType.Story5_Capsule;
                    break;

                default:
                    throw new System.ArgumentException("Bad initialization! Not a capsule type.");
            }

            // Construct type
            var value = new CourseMetadataTrigger
            {
                transform = transform,
                metadataType = metadataType,
            };

            return value;
        }

        public void ImportGfz(CourseMetadataTrigger value)
        {
            transform.CopyGfzTransform(value.transform);
            transform.localScale *= scale;

            // Select which capsule type flag to use.
            switch (value.metadataType)
            {
                case CourseMetadataType.Story1_CapsuleAX:
                    type = CapsuleType.AxStory1;
                    break;

                case CourseMetadataType.Story5_Capsule:
                    type = CapsuleType.GxStory5;
                    break;

                default:
                    throw new System.ArgumentException("Bad initialization! Not a capsule type.");
            }
        }
    }
}
