using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [ExecuteInEditMode]
    public abstract class GfzSegmentNode : MonoBehaviour
    {

        // for editors
        [SerializeField] protected bool autoGenerateTRS = true;

        /// <summary>
        /// TODO: evaluate this method, see if it should only return a copy, name it sensically, etc
        /// </summary>
        /// <param name="isGfzCoordinateSpace"></param>
        /// <returns></returns>
        public abstract AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace);
        public abstract TrackSegment CreateTrackSegment();


        // TODO? Consider adding children to construction?
        public HierarchichalAnimationCurveTRS CreateHierarchichalAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var parentHacTRS = GetParentHacTRS(isGfzCoordinateSpace, out bool isRoot);
            //var staticMatrix = GetStaticMatrix(isGfzCoordinateSpace, isRoot);
            var animTRS = CopyAnimationCurveTRS(isGfzCoordinateSpace);

            var hacTRS = new HierarchichalAnimationCurveTRS()
            {
                Parent = parentHacTRS,
                //StaticMatrix = staticMatrix,
                AnimationCurveTRS = animTRS,
            };

            return hacTRS;
        }

        public abstract float GetMaxTime();

        public bool IsRoot()
        {
            var parent = GetParent();
            bool isRoot = parent == null;
            return isRoot;
        }

        public bool HasChildren()
        {
            var children = GetChildren();
            bool hasChildren = children.Length > 0;
            return hasChildren;
        }

        public GfzPathSegment GetRoot()
        {
            if (IsRoot())
            {
                bool isRootType = this is GfzPathSegment;
                if (!isRootType)
                    throw new System.Exception($"Root node is not of type {typeof(GfzPathSegment).Name}");

                var root = this as GfzPathSegment;
                return root;
            }
            else
            {
                // Recursive
                var parent = GetParent();
                var root = parent.GetRoot();
                return root;
            }
        }

        public TGfzTrackSegmentNode GetParent<TGfzTrackSegmentNode>() where TGfzTrackSegmentNode : GfzSegmentNode
        {
            // Note: if this fails, the transform might be null?
            bool hasParent = transform.parent != null;
            if (hasParent)
            {
                var parentSegment = transform.parent.GetComponent<TGfzTrackSegmentNode>();
                return parentSegment;
            }
            else
                return null;
        }

        public GfzSegmentNode GetParent()
        {
            var parent = GetParent<GfzSegmentNode>();
            return parent;
        }

        public TGfzTrackSegmentNode[] GetChildren<TGfzTrackSegmentNode>() where TGfzTrackSegmentNode : GfzSegmentNode
        {
            var children = new List<TGfzTrackSegmentNode>();
            foreach (var child in transform.GetChildren())
            {
                bool isActive = child.gameObject.activeInHierarchy;
                if (!isActive)
                    continue;

                var childSegment = child.GetComponent<TGfzTrackSegmentNode>();
                bool childIsSegment = childSegment != null;
                if (childIsSegment)
                    children.Add(childSegment);
            }
            return children.ToArray();
        }

        public GfzSegmentNode[] GetChildren()
        {
            var children = GetChildren<GfzSegmentNode>();
            return children;
        }

        public HierarchichalAnimationCurveTRS GetParentHacTRS(bool isGfzCoordinateSpace, out bool isRoot)
        {
            var parent = GetParent();
            isRoot = parent == null;
            var parentHacTRS = isRoot ? null : parent.CreateHierarchichalAnimationCurveTRS(isGfzCoordinateSpace);
            return parentHacTRS;
        }

        public byte GetBranchIndex()
        {
            // TODO: recurse up hierachy, stopping on branch node, getting ID
            return 0;
        }

        public TrackSegment[] CreateChildTrackSegments()
        {
            var children = GetChildren();
            var trackSegments = new TrackSegment[children.Length];
            for (int i = 0; i < trackSegments.Length; i++)
            {
                var trackSegment = children[i].CreateTrackSegment();
                trackSegments[i] = trackSegment;
            }
            return trackSegments;
        }

        public Vector3 GetPosition()
        {
            var position = IsRoot() ? transform.position : transform.localPosition;
            return position;
        }
        public Quaternion GetRotation()
        {
            var position = IsRoot() ? transform.rotation : transform.localRotation;
            return position;
        }
        public Vector3 GetScale()
        {
            var scale = IsRoot() ? transform.lossyScale : transform.localScale;
            return scale;
        }



        public GfzShape[] GetShapeNodes()
        {
            var shapes = GetComponentsInChildren<GfzShape>();
            return shapes;
        }
        public GfzPathPositionedBase[] GetUpdateables()
        {
            var updateables = GameObject.FindObjectsOfType<GfzPathPositionedBase>(false);
            return updateables;
        }

        public void UpdateShapeMeshes()
        {
            var shapes = GetShapeNodes();
            foreach (var shape in shapes)
                shape.UpdateMesh();
        }
        public void UpdateUpdateables()
        {
            var root = GetRoot();
            var updateables = root.GetUpdateables();
            foreach (var updateable in updateables)
            {
                var updateableRoot = updateable.SegmentNode.GetRoot();
                if (updateableRoot == root)
                {
                    updateable.OnUpdate();
                }
            }
        }

        public abstract void UpdateTRS();

        public virtual void InvokeUpdates()
        {
            UpdateTrackSegmentNodes();

            if (autoGenerateTRS)
                UpdateTRS();

            UpdateShapeMeshes();
            UpdateUpdateables();
        }
        protected virtual void Reset()
        {
            InvokeUpdates();
        }
        protected virtual void OnValidate()
        {
            if (autoGenerateTRS)
            {
                InvokeUpdates();
            }
        }
        private void Update()
        {
            bool isSelected = UnityEditor.Selection.activeGameObject == this.gameObject;
            if (isSelected && transform.hasChanged)
            {
                InvokeUpdates();
            }
        }

        private void UpdateTrackSegmentNodes()
        {
            var track = GetComponentInParent<GfzTrack>();
            if (track != null)
            {
                track.RefreshSegmentNodes();
            }
        }
    }
}
