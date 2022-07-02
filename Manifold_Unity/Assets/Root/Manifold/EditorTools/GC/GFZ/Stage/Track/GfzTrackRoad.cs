using GameCube.GFZ.Stage;
using Manifold.EditorTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackRoad : GfzSegmentShape
    {
        [Header("Gizmos")]
        [SerializeField]
        private bool doGizmos = true;

        [Header("Road Properties")]
        [Min(0f)]
        [SerializeField]
        private float railHeightLeft = 5f;

        [Min(0f)]
        [SerializeField]
        private float railHeightRight = 5f;


        private void OnDrawGizmos()
        {
            return;

            if (!doGizmos)
                return;

            Gizmos.color = Color.red;
            var mesh = Resources.GetBuiltinResource<Mesh>(UnityConst.Resources.Cube);

            var baseMtx = transform.localToWorldMatrix;
            var increment = 1f / 512f;
            for (float t = 0; t < 1f; t += increment)
            {
                var animMtx = AnimationCurveTRS.EvaluateMatrix(t);
                var mtx = baseMtx * animMtx;
                var p = mtx.GetPosition();
                var r = mtx.rotation;
                var s = mtx.lossyScale;
                Gizmos.DrawMesh(mesh, 0, p, r, s);
            }
        }

        public override Mesh[] GenerateMeshes()
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment GenerateTrackSegment()
        {
            var trackSegment = Segment.GetTrackSegment();
            var lastNode = trackSegment.Children[0];

            // Override the rail properies
            IO.Assert.IsTrue(lastNode.SegmentType == TrackSegmentType.IsTrack);

            // Rail height
            lastNode.SetRails(railHeightLeft, railHeightRight);

            //
            return trackSegment;
        }

    }
}
