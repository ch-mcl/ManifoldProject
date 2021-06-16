﻿using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AnimationClip :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        /// <summary>
        /// Number of animation curves. Order: scale.xyz, rotation.xyz, position.xyz, unknown, unknown (light?)
        /// </summary>
        public const int kSizeCurvesPtrs = 11;
        const int kSizeZero_0x08 = 0x10;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public float unk_0x00;
        public float unk_0x04;
        public byte[] zeroes_0x08;
        public EnumFlags32 unk_layer_0x18;
        /// <summary>
        /// idx: 0,1,2: scale.xyz
        /// idx: 3,4,5: rotation.xyz
        /// idx: 6,7,8: position.xyz
        /// idx: 9: unused
        /// idx: 10: alpha channel
        /// </summary>
        public AnimationCurvePlus[] animationCurvePluses; // Written inline, not pointer refs


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
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref zeroes_0x08, kSizeZero_0x08);
                reader.ReadX(ref unk_layer_0x18);
                reader.ReadX(ref animationCurvePluses, kSizeCurvesPtrs, true);
            }
            this.RecordEndAddress(reader);
            {
                foreach (var zero in zeroes_0x08)
                    Assert.IsTrue(zero == 0);
            }
            // No jumping required
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                foreach (var zero in zeroes_0x08)
                    Assert.IsTrue(zero == 0);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(zeroes_0x08, false);
                writer.WriteX(unk_layer_0x18);
                writer.WriteX(animationCurvePluses, false);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(AnimationClip)}(" +
                $"{nameof(unk_0x00)}: {unk_0x00}, " +
                $"{nameof(unk_0x04)}: {unk_0x04}, " +
                $"{nameof(unk_layer_0x18)}: {unk_layer_0x18}" +
                $")";
        }
    }
}