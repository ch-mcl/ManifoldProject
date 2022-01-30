﻿using GameCube.GFZ.CourseCollision;
using System;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    public class GfzSceneObject : MonoBehaviour,
        IGfzConvertable<SceneObjectDefinition>,
        IEquatable<GfzSceneObject>
    {
        [SerializeField] private LodRenderFlags lodRenderFlags;
        [SerializeField] private GfzSceneObjectLODs sceneObjectLODs;
        [SerializeField] private GfzColliderMesh colliderMesh;

        public GfzSceneObjectLODs GfzSceneObjectLODs => sceneObjectLODs;
        public GfzColliderMesh ColliderMesh => colliderMesh;


        private SceneObjectDefinition reference;

        public void InitSharedReference()
        {
            reference = new SceneObjectDefinition();

            reference.lodRenderFlags = lodRenderFlags;
            reference.lods = sceneObjectLODs.ExportGfz();

            if (colliderMesh != null)
                reference.colliderMesh = colliderMesh.ExportGfz();
        }

        public SceneObjectDefinition ExportGfz()
        {
            // If null, you have not init the shared reference value
            return reference;
        }

        public void ImportGfz(SceneObjectDefinition sceneObject)
        {
            lodRenderFlags = sceneObject.lodRenderFlags;

            sceneObjectLODs = this.gameObject.AddComponent<GfzSceneObjectLODs>();
            sceneObjectLODs.ImportGfz(sceneObject.lods);

            if (sceneObject.colliderMesh != null)
            {
                colliderMesh = this.gameObject.AddComponent<GfzColliderMesh>();
                colliderMesh.ImportGfz(sceneObject.colliderMesh);
            }
        }

        public bool Equals(GfzSceneObject other)
        {
            var hasFlags = other.lodRenderFlags == lodRenderFlags;
            var hasLODs = other.sceneObjectLODs.Equals(sceneObjectLODs);
            //var hasCollider = other.colliderMesh.Equals(colliderMesh);

            return hasFlags && hasLODs; //&& hasCollider;
        }
    }
}
