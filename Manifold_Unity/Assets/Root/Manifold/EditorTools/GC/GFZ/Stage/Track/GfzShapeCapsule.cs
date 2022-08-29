using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeCapsule : GfzSegmentShape
    {
        [Header("Pipe/Cylinder")]
        [SerializeField] private CapsuleStyle capsuleStyle;
        [SerializeField, Min(2)] private int lengthDistance = 25;
        [SerializeField, Min(4)] private int subdivideSemiCircle = 16;
        [SerializeField, Min(1)] private int subdivideLinee = 1;
        [SerializeField] private UnityEngine.AnimationCurve capsuleWidth = new(new(0, 1), new(1, 1));

        public int SubdivideSemiCircle => subdivideSemiCircle;
        public int SubdivideLine => subdivideLinee;

        public enum CapsuleStyle
        {
            Debug,
            MuteCity,
        }

        public override ShapeID ShapeIdentifier => ShapeID.pipe;

        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, TplTextureContainer tpl)
        {
            var tristripsCollections = GetTristrips(true);
            gcmfTemplates = GetGcmfTemplates();
            var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplates, tristripsCollections, tpl);
            return gcmf;
        }

        public GcmfTemplate[] GetGcmfTemplates()
        {
            {
                switch (capsuleStyle)
                {
                    case CapsuleStyle.MuteCity:
                        return new GcmfTemplate[]
                        {
                            GcmfTemplates.MuteCity.RoadTop(),
                            GcmfTemplates.MuteCityCOM.RoadTopNoDividers(),
                            GcmfTemplates.Debug.CreateLitVertexColored(),
                            GcmfTemplates.Debug.CreateLitVertexColored(),
                        };

                    default:
                        return new GcmfTemplate[]
                        {
                            GcmfTemplates.Debug.CreateLitVertexColored(),
                            GcmfTemplates.Debug.CreateLitVertexColored(),
                            GcmfTemplates.Debug.CreateLitVertexColored(),
                        };
                }
            }
        }

        public Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, lengthDistance);
            var matricesLeft = SemiCirclesMatrices(matrices, capsuleWidth, true);
            var matricesRight = SemiCirclesMatrices(matrices, capsuleWidth, false);
            var matricesTop = LineMatricesPosition(matrices, capsuleWidth, true);
            var matricesBottom = LineMatricesPosition(matrices, capsuleWidth, false);
            var segmentLength = GetRoot().GetMaxTime();

            switch (capsuleStyle)
            {
                case CapsuleStyle.MuteCity:
                    return new Tristrip[][]
                    {
                        TristripTemplates.CapsulePipe.SemiCirclesTex0(matricesLeft, matricesRight, this, segmentLength, isGfzCoordinateSpace, 12),
                        TristripTemplates.CapsulePipe.LinesTex0(matricesTop, matricesBottom, this, segmentLength, 8),
                        TristripTemplates.CapsulePipe.DebugOutside(matricesLeft, matricesRight, matricesTop, matricesBottom, this, isGfzCoordinateSpace),
                        TristripTemplates.CapsulePipe.DebugEndcap(matricesLeft, matricesRight, this),
                    };

                default:
                    return new Tristrip[][]
                    {
                        TristripTemplates.CapsulePipe.DebugInside(matricesLeft, matricesRight, matricesTop, matricesBottom, this, isGfzCoordinateSpace),
                        TristripTemplates.CapsulePipe.DebugOutside(matricesLeft, matricesRight, matricesTop, matricesBottom, this, isGfzCoordinateSpace),
                        TristripTemplates.CapsulePipe.DebugEndcap(matricesLeft, matricesRight, this),
                        //TristripTemplates.Pipe.DebugOutside(matrices, this, isGfzCoordinateSpace),
                        //TristripTemplates.Pipe.DebugRingEndcap(matrices, this),
                    };
            }
        }


        public override Mesh CreateMesh()
        {
            var tristripsColletion = GetTristrips(false);
            var tristrips = CombinedTristrips(tristripsColletion);
            var mesh = TristripsToMesh(tristrips);
            mesh.name = $"Auto Gen - {name}";
            return mesh;
        }

        public override TrackSegment CreateTrackSegment()
        {
            var capsuleTRS = new AnimationCurveTRS();
            capsuleTRS.Position.x = CreateWidthCurve(capsuleWidth, GetMaxTime());

            var capsule = new TrackSegment();
            capsule.OrderIndentifier = name + "_capsule";
            capsule.SegmentType = TrackSegmentType.IsEmbed;
            capsule.EmbeddedPropertyType = TrackEmbeddedPropertyType.IsCapsulePipe;
            capsule.BranchIndex = GetBranchIndex();
            capsule.FallbackPosition = new Unity.Mathematics.float3(0.5f, 0, 0);
            // 0.5f since this is the left/right position of the circle-ends radii centers.
            // This results in the track transform's scale.x being 1:1 with the width of the capsule.

            return capsule;
        }


        public override void UpdateTRS()
        {
            // do nothing :)
        }


        private static Matrix4x4[] SemiCirclesMatrices(Matrix4x4[] matrices, UnityEngine.AnimationCurve animationCurve, bool isLeftSide)
        {
            var offsetMatrices = new Matrix4x4[matrices.Length];
            float direction = isLeftSide ? 0.5f : -0.5f;
            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = matrix.lossyScale;

                Vector3 widthOffset = new Vector3(scale.x * direction, 0, 0);
                position += rotation * widthOffset;
                scale = new Vector3(scale.y, scale.y, 1);

                offsetMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return offsetMatrices;
        }
        private static Matrix4x4[] LineMatricesPosition(Matrix4x4[] matrices, UnityEngine.AnimationCurve animationCurve, bool isTop)
        {
            float direction = isTop ? 0.5f : -0.5f;
            Quaternion rotationOffset = isTop
                ? Quaternion.Euler(new Vector3(0, 0, 180))
                : Quaternion.identity;

            var offsetMatrices = new Matrix4x4[matrices.Length];
            for (int i = 0; i < matrices.Length; i++)
            {
                var matrix = matrices[i];
                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = matrix.lossyScale;

                Vector3 heightOffset = new Vector3(0, scale.y * direction, 0);

                position += rotation * heightOffset;
                rotation = rotation * rotationOffset;

                offsetMatrices[i] = Matrix4x4.TRS(position, rotation, scale);
            }
            return offsetMatrices;
        }
        private static UnityEngine.AnimationCurve CreateWidthCurve(UnityEngine.AnimationCurve animationCurve, float maxTime)
        {
            var keys = animationCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var newAnimationCurve = new UnityEngine.AnimationCurve(keys);
            return newAnimationCurve;
        }

    }
}
