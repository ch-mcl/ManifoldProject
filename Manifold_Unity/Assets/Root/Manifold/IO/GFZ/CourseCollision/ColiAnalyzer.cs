﻿using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Analyzer")]
    public class ColiAnalyzer : ExecutableScriptableObject,
        IAnalyzable
    {
        #region MEMBERS

        [Header("Output")]
        [SerializeField, BrowseFolderField]
        protected string outputPath;
        [SerializeField, BrowseFolderField("Assets/"), Tooltip("Used with IOOption.allFromSourceFolder")]
        protected string[] searchFolders;

        [SerializeField]
        protected bool
            animations = true,
            coliUnk5 = true,
            headers = true,
            sceneObjects = true,
            storyObjectTriggers = true,
            topologyParameters = true,
            trackTransform = true,
            transformsComparison = true,
            unknownAnimationData = true,
            unknownTrigger1 = true,
            venueMetadataObject = true,
            staticCollider = true,
            sceneObjectInstanceReferences = true;

        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterAnalysis = true;

        [Header("Analysis Options")]
        [SerializeField]
        protected IOOption analysisOption = IOOption.allFromAssetDatabase;
        [SerializeField]
        protected ColiSceneSobj[] sceneSobjs;

        #endregion

        #region PROPERTIES

        public override string ExecuteText => "Analyze COLI";

        #endregion

        public void Analyze()
        {
            sceneSobjs = AssetDatabaseUtility.GetSobjByOption(sceneSobjs, analysisOption, searchFolders);
            var time = AnalyzerUtility.GetFileTimestamp();


            if (animations)
            {
                // ANIMATIONS
                {
                    var filePath = $"{time} COLI Animations.tsv";
                    filePath = Path.Combine(outputPath, filePath);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                    AnalyzeGameObjectAnimations(filePath);
                }

                // ANIMATIONS INDIVIDUALIZED
                {
                    var count = GameCube.GFZ.CourseCollision.AnimationClip.kSizeCurvesPtrs;
                    for (int i = 0; i < count; i++)
                    {
                        var filePath = $"{time} COLI Animations {i}.tsv";
                        filePath = Path.Combine(outputPath, filePath);
                        EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / count);
                        AnalyzeGameObjectAnimationsIndex(filePath, i);
                    }
                }
            }

            //
            if (transformsComparison)
            {
                string fileName = $"{time} COLI Comapre Transforms.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeSceneObjectTransforms(filePath);
            }

            if (coliUnk5)
            {
                string fileName = $"{time} COLI {nameof(UnknownStageData1)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeUnknownStageData1(filePath);
            }

            //
            if (headers)
            {
                string filePath = string.Empty;

                filePath = $"{time} COLI Headers.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeHeaders(filePath);
            }

            if (storyObjectTriggers)
            {
                string fileName = $"{time} COLI {nameof(StoryObjectTrigger)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeStoryObjectTrigger(filePath);
            }

            //GAME OBJECTS
            if (sceneObjects)
            {
                string filePath = string.Empty;

                filePath = $"{time} COLI {nameof(SceneObject)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .1f);
                AnalyzeGameObjects(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} {nameof(UnknownSceneObjectData)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .3f);
                AnalyzeGameObjectsUnk1(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} {nameof(SkeletalAnimator)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeGameObjectsSkeletalAnimator(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} {nameof(ColliderTriangle)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .7f);
                AnalyzeGameObjectsCollisionTri(filePath);

                filePath = $"{time} COLI {nameof(SceneObject)} {nameof(ColliderQuad)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .9f);
                AnalyzeGameObjectsCollisionQuad(filePath);
            }


            if (sceneObjectInstanceReferences)
            {
                string filePath = string.Empty;

                filePath = $"{time} COLI {nameof(SceneObjectReference)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .33f);
                AnalyzeSceneObjectReferences(filePath);

                filePath = $"{time} COLI {nameof(SceneInstanceReference)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .66f);
                AnalyzeSceneInstanceReferences(filePath);

                filePath = $"{time} COLI {nameof(SceneInstanceReference)} and {nameof(SceneObjectReference)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, 1f);
                AnalyzeSceneInstanceAndObjectReferences(filePath);
            }


            // TOPOLOGY PARAMETERS
            if (topologyParameters)
            {
                var count = TopologyParameters.kCurveCount;
                for (int i = 0; i < count; i++)
                {
                    var filePath = $"{time} COLI {nameof(TopologyParameters)} {i + 1}.tsv";
                    filePath = Path.Combine(outputPath, filePath);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, (float)(i + 1) / TopologyParameters.kCurveCount);
                    AnalyzeTrackKeyables(filePath, i);
                }

                {
                    var filePath = $"{time} COLI {nameof(TopologyParameters)} ALL.tsv";
                    filePath = Path.Combine(outputPath, filePath);
                    EditorUtility.DisplayProgressBar(ExecuteText, filePath, 0.95f);
                    AnalyzeTrackKeyablesAll(filePath);
                }
            }

            // TRACK TRANSFORMS
            if (trackTransform)
            {
                var filePath = $"{time} COLI {nameof(TrackTransform)}.tsv";
                filePath = Path.Combine(outputPath, filePath);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeTrackTransforms(filePath);
            }

            if (unknownAnimationData)
            {
                string fileName = $"{time} COLI {nameof(UnknownStageData2)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeUnknownAnimationData(filePath);
            }

            if (unknownTrigger1)
            {
                string fileName = $"{time} COLI {nameof(UnknownTrigger1)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeUnknownTrigger1(filePath);
            }

            if (venueMetadataObject)
            {
                string fileName = $"{time} COLI {nameof(StoryObjectTrigger)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeStoryObjectTrigger(filePath);
            }


            //
            if (staticCollider)
            {
                string fileName = $"{time} COLI {nameof(StaticColliderMeshes)}.tsv";
                string filePath = Path.Combine(outputPath, fileName);
                EditorUtility.DisplayProgressBar(ExecuteText, filePath, .5f);
                AnalyzeSceneStaticCollider(filePath);
            }


            // OPEN FOLDER after analysis
            if (openFolderAfterAnalysis)
            {
                OSUtility.OpenDirectory(outputPath + "/");
            }
            EditorUtility.ClearProgressBar();
        }

        public override void Execute()
            => Analyze();

        #region Track Data / Transforms

        public void AnalyzeTrackKeyablesAll(string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Game");

                writer.WriteNextCol(nameof(TrackTransform.topologyMetadata));
                writer.WriteNextCol(nameof(TrackTransform.trackProperty));
                writer.WriteNextCol(nameof(TrackTransform.perimeterOptions));
                writer.WriteNextCol(nameof(TrackTransform.pipeCylinderOptions));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x38));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x3A));

                writer.WriteNextCol("TrackTransform Index");
                writer.WriteNextCol("Keyable /9");
                writer.WriteNextCol("Keyable Index");
                writer.WriteNextCol("Keyable Order");
                writer.WriteNextCol("Nested Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.time));
                writer.WriteNextCol(nameof(KeyableAttribute.value));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentOut));
                writer.WriteNextRow();

                // foreach File
                foreach (var sobj in sceneSobjs)
                {
                    // foreach Transform
                    int trackIndex = 0;
                    foreach (var trackTransform in sobj.Value.rootTrackTransforms)
                    {
                        for (int keyablesIndex = 0; keyablesIndex < TopologyParameters.kCurveCount; keyablesIndex++)
                        {
                            WriteTrackKeyableAttributeRecursive(writer, sobj, 0, keyablesIndex, ++trackIndex, trackTransform);
                        }
                    }
                }

                writer.Flush();
            }
        }

        public void AnalyzeTrackKeyables(string filename, int keyablesSet)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("FileName");
                writer.WriteNextCol("Game");

                writer.WriteNextCol(nameof(TrackTransform.topologyMetadata));
                writer.WriteNextCol(nameof(TrackTransform.trackProperty));
                writer.WriteNextCol(nameof(TrackTransform.perimeterOptions));
                writer.WriteNextCol(nameof(TrackTransform.pipeCylinderOptions));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x38));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x3A));

                writer.WriteNextCol("TrackTransform Index");
                writer.WriteNextCol("Keyable /9");
                writer.WriteNextCol("Keyable Index");
                writer.WriteNextCol("Keyable Order");
                writer.WriteNextCol("Nested Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.time));
                writer.WriteNextCol(nameof(KeyableAttribute.value));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentOut));
                writer.WriteNextRow();

                // foreach File
                foreach (var sobj in sceneSobjs)
                {
                    // foreach Transform
                    int trackTransformIndex = 0;
                    foreach (var trackTransform in sobj.Value.rootTrackTransforms)
                    {
                        WriteTrackKeyableAttributeRecursive(writer, sobj, nestedDepth: 0, keyablesSet, trackTransformIndex++, trackTransform);
                    }
                }

                writer.Flush();
            }
        }
        public void WriteTrackKeyableAttributeRecursive(StreamWriter writer, ColiSceneSobj sobj, int nestedDepth, int animationCurveIndex, int trackTransformIndex, TrackTransform trackTransform)
        {
            var animationCurves = trackTransform.trackTopology.animationCurves;
            var keyableIndex = 1; // 0-n, depends on number of keyables in array
            int keyableTotal = animationCurves[animationCurveIndex].Length;

            // Animation data of this curve
            foreach (var keyables in animationCurves[animationCurveIndex].keyableAttributes)
            {
                WriteKeyableAttribute(writer, sobj, nestedDepth + 1, keyableIndex++, keyableTotal, animationCurveIndex, trackTransformIndex, keyables, trackTransform);
            }

            // Go to track transform children, write their anim data (calls this function)
            foreach (var child in trackTransform.children)
                WriteTrackKeyableAttributeRecursive(writer, sobj, nestedDepth + 1, animationCurveIndex, trackTransformIndex, child);
        }
        public void WriteKeyableAttribute(StreamWriter writer, ColiSceneSobj sobj, int nestedDepth, int keyableIndex, int keyableTotal, int keyablesSet, int trackTransformIndex, KeyableAttribute param, TrackTransform tt)
        {
            string gameId = sobj.Value.IsFileGX ? "GX" : "AX";

            writer.WriteNextCol(sobj.FileName);
            writer.WriteNextCol(gameId);

            writer.WriteNextCol(tt.topologyMetadata);
            writer.WriteNextCol(tt.trackProperty);
            writer.WriteNextCol(tt.perimeterOptions);
            writer.WriteNextCol(tt.pipeCylinderOptions);
            writer.WriteNextCol(tt.unk_0x38);
            writer.WriteNextCol(tt.unk_0x3A);

            writer.WriteNextCol(trackTransformIndex);
            writer.WriteNextCol(keyablesSet);
            writer.WriteNextCol(keyableIndex);
            writer.WriteNextCol($"[{keyableIndex}/{keyableTotal}]");
            writer.WriteNextCol($"{nestedDepth}");
            writer.WriteNextCol(param.StartAddressHex());
            writer.WriteNextCol(param.easeMode);
            writer.WriteNextCol((int)param.easeMode);
            writer.WriteNextCol(param.time);
            writer.WriteNextCol(param.value);
            writer.WriteNextCol(param.zTangentIn);
            writer.WriteNextCol(param.zTangentOut);
            writer.WriteNextRow();
        }


        // Kicks off recursive write
        private static int s_order;
        public void AnalyzeTrackTransforms(string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                writer.WriteNextCol("Filename");
                writer.WriteNextCol("Order");
                writer.WriteNextCol("Root Index");
                writer.WriteNextCol("Transform Depth");
                writer.WriteNextCol("Address");
                writer.WriteNextCol(nameof(TrackTransform.topologyMetadata));
                writer.WriteNextCol(nameof(TrackTransform.trackProperty));
                writer.WriteNextCol(nameof(TrackTransform.perimeterOptions));
                writer.WriteNextCol(nameof(TrackTransform.pipeCylinderOptions));
                writer.WriteNextCol(nameof(TrackTransform.generalTopologyPtr));
                writer.WriteNextCol(nameof(TrackTransform.hairpinCornerTopologyPtr));
                writer.WriteNextCol(nameof(TrackTransform.childrenPtrs));
                writer.WriteNextCol(nameof(TrackTransform.localScale));
                writer.WriteNextCol(nameof(TrackTransform.localRotation));
                writer.WriteNextCol(nameof(TrackTransform.localPosition));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x38));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x39));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x3A));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x3B));
                writer.WriteNextCol(nameof(TrackTransform.railHeightRight));
                writer.WriteNextCol(nameof(TrackTransform.railHeightLeft));
                writer.WriteNextCol(nameof(TrackTransform.zero_0x44));
                writer.WriteNextCol(nameof(TrackTransform.zero_0x48));
                writer.WriteNextCol(nameof(TrackTransform.unk_0x4C));
                //
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x00));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x04));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x08));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x0C));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x10));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x14));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x18));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x1C));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x20));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x24));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x28));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.unk_0x2C));
                writer.WriteNextColNicify(nameof(TrackCornerTopology.unkRotation));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.const_0x34));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.zero_0x35));
                writer.WriteNextColNicify(nameof(TrackCornerTopology.perimeterOptions));
                //writer.WriteNextColNicify(nameof(Track90DegreeCorner.zero_0x37));
                //
                writer.WriteNextRow();

                // RESET static variable
                s_order = 0;

                foreach (var sobj in sceneSobjs)
                {
                    var scene = sobj.Value;
                    var index = 0;
                    var total = scene.rootTrackTransforms.Length;
                    foreach (var trackTransform in scene.rootTrackTransforms)
                    {
                        WriteTrackTransformRecursive(writer, sobj, 0, ++index, total, trackTransform);
                    }
                }

                writer.Flush();
            }
        }
        // Writes self and children
        public void WriteTrackTransformRecursive(StreamWriter writer, ColiSceneSobj sobj, int depth, int index, int total, TrackTransform trackTransform)
        {
            // Write Parent
            WriteTrackTransform(writer, sobj, depth, index, total, trackTransform);

            // Write children
            foreach (var child in trackTransform.children)
            {
                WriteTrackTransformRecursive(writer, sobj, depth + 1, index, total, child);
            }
        }
        // The actual writing to file
        public void WriteTrackTransform(StreamWriter writer, ColiSceneSobj sobj, int depth, int index, int total, TrackTransform trackTransform)
        {
            writer.WriteNextCol(sobj.FileName);
            writer.WriteNextCol($"{s_order++}");
            writer.WriteNextCol($"[{index}/{total}]");
            writer.WriteNextCol($"{depth}");
            writer.WriteNextCol(trackTransform.StartAddressHex());
            writer.WriteNextCol(trackTransform.topologyMetadata);
            writer.WriteNextCol(trackTransform.trackProperty);
            writer.WriteNextCol(trackTransform.perimeterOptions);
            writer.WriteNextCol(trackTransform.pipeCylinderOptions);
            writer.WriteNextCol(trackTransform.generalTopologyPtr);
            writer.WriteNextCol(trackTransform.hairpinCornerTopologyPtr);
            writer.WriteNextCol(trackTransform.childrenPtrs);
            writer.WriteNextCol(trackTransform.localScale);
            writer.WriteNextCol(trackTransform.localRotation);
            writer.WriteNextCol(trackTransform.localPosition);
            writer.WriteNextCol(trackTransform.unk_0x38);
            writer.WriteNextCol(trackTransform.unk_0x39);
            writer.WriteNextCol(trackTransform.unk_0x3A);
            writer.WriteNextCol(trackTransform.unk_0x3B);
            writer.WriteNextCol(trackTransform.railHeightRight);
            writer.WriteNextCol(trackTransform.railHeightLeft);
            writer.WriteNextCol(trackTransform.zero_0x44);
            writer.WriteNextCol(trackTransform.zero_0x48);
            writer.WriteNextCol(trackTransform.unk_0x4C);
            //
            if (trackTransform.hairpinCornerTopologyPtr.IsNotNullPointer)
            {
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x00);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x04);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x08);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x0C);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x10);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x14);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x18);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x1C);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x20);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x24);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x28);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.unk_0x2C);
                writer.WriteNextCol(trackTransform.hairpinCornerTopology.unkRotation);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.const_0x34);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.zero_0x35);
                writer.WriteNextCol(trackTransform.hairpinCornerTopology.perimeterOptions);
                //writer.WriteNextCol(trackTransform.track90DegreeCorner.zero_0x37);
            }
            //
            writer.WriteNextRow();
        }


        #endregion

        #region GameObject Animations

        public void AnalyzeGameObjectAnimations(string filename)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Anim Addr");
                writer.WriteNextCol("Key Addr");
                writer.WriteNextCol("Anim Index [0-10]");
                writer.WriteNextCol("Unk_0x00");
                writer.WriteNextCol("Time");
                writer.WriteNextCol("Value");
                writer.WriteNextCol("Unk_0x0C");
                writer.WriteNextCol("Unk_0x10");
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        if (gameObject.animation == null)
                            continue;

                        int animIndex = 0;
                        foreach (var animationCurvePlus in gameObject.animation.animationCurvePluses)
                        {
                            foreach (var keyable in animationCurvePlus.animationCurve.keyableAttributes)
                            {
                                writer.WriteNextCol(file.FileName);
                                writer.WriteNextCol(gameObjectIndex);
                                writer.WriteNextCol(gameObject.nameCopy);
                                writer.WriteNextCol(animationCurvePlus.StartAddressHex());
                                writer.WriteNextCol(keyable.StartAddressHex());
                                writer.WriteNextCol(animIndex);
                                writer.WriteNextCol(keyable.easeMode);
                                writer.WriteNextCol(keyable.time);
                                writer.WriteNextCol(keyable.value);
                                writer.WriteNextCol(keyable.zTangentIn);
                                writer.WriteNextCol(keyable.zTangentOut);
                                writer.WriteNextRow();
                            }
                            animIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectAnimationsIndex(string filename, int index)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filename))
            {
                // Write header
                writer.WriteNextCol("File Path");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Anim Addr");
                writer.WriteNextCol("Key Addr");
                writer.WriteNextCol("Anim Index [0-10]");
                writer.WriteNextCol("Unk_0x00");
                writer.WriteNextCol("Time");
                writer.WriteNextCol("Value");
                writer.WriteNextCol("Unk_0x0C");
                writer.WriteNextCol("Unk_0x10");
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        int animIndex = 0;
                        foreach (var animationCurvePlus in gameObject.animation.animationCurvePluses)
                        {
                            foreach (var keyable in animationCurvePlus.animationCurve.keyableAttributes)
                            {
                                /// HACK, write each anim index as separate file
                                if (animIndex != index)
                                    continue;

                                writer.WriteNextCol(file.FileName);
                                writer.WriteNextCol(gameObjectIndex);
                                writer.WriteNextCol(gameObject.nameCopy);
                                writer.WriteNextCol(animationCurvePlus.StartAddressHex());
                                writer.WriteNextCol(keyable.StartAddressHex());
                                writer.WriteNextCol(animIndex);
                                writer.WriteNextCol(keyable.easeMode);
                                writer.WriteNextCol(keyable.time);
                                writer.WriteNextCol(keyable.value);
                                writer.WriteNextCol(keyable.zTangentIn);
                                writer.WriteNextCol(keyable.zTangentOut);
                                writer.WriteNextRow();
                            }
                            animIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        #endregion

        #region GameObjects

        public void AnalyzeGameObjects(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol(nameof(SceneObject.lodFar));
                writer.WriteNextCol(nameof(SceneObject.lodFar));
                writer.WriteNextCol(nameof(SceneObject.lodNear));
                writer.WriteNextCol(nameof(SceneObject.lodNear));
                writer.WriteNextCol(nameof(SceneObject.instanceReferencePtr));
                writer.WriteNextCol(nameof(SceneObject.transform.Position));
                writer.WriteNextCol(nameof(SceneObject.transform.RotationEuler));
                writer.WriteNextCol(nameof(SceneObject.transform.Scale));
                writer.WriteNextCol(nameof(SceneObject.zero_0x2C));
                writer.WriteNextCol(nameof(SceneObject.animationPtr));
                writer.WriteNextCol(nameof(SceneObject.unkPtr_0x34));
                writer.WriteNextCol(nameof(SceneObject.skeletalAnimatorPtr));
                writer.WriteNextCol(nameof(SceneObject.transformPtr));
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        writer.WriteNextCol(file.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.nameCopy);
                        writer.WriteNextCol(sceneObject.lodFar.data32);
                        writer.WriteNextCol($"0x{sceneObject.lodFar.data32:x8}");
                        writer.WriteNextCol(sceneObject.lodNear.data32);
                        writer.WriteNextCol($"0x{sceneObject.lodNear.data32:x8}");
                        writer.WriteNextCol(sceneObject.instanceReferencePtr.HexAddress);
                        writer.WriteNextCol(sceneObject.transform.Position);
                        writer.WriteNextCol(sceneObject.transform.RotationEuler);
                        writer.WriteNextCol(sceneObject.transform.Scale);
                        writer.WriteNextCol(sceneObject.zero_0x2C);
                        writer.WriteNextCol(sceneObject.animationPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.unkPtr_0x34.HexAddress);
                        writer.WriteNextCol(sceneObject.skeletalAnimatorPtr.HexAddress);
                        writer.WriteNextCol(sceneObject.transformPtr.HexAddress);
                        writer.WriteNextRow();

                        sceneObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsUnk1(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol("Unknown 1 Index");
                writer.WriteNextColNicify(nameof(UnknownSceneObjectFloatPair.unk_0x00));
                writer.WriteNextColNicify(nameof(UnknownSceneObjectFloatPair.unk_0x04));
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        if (sceneObject.unk1 == null)
                            continue;

                        int unkIndex = 0;
                        foreach (var unk1 in sceneObject.unk1.unk)
                        {
                            writer.WriteNextCol(file.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(sceneObject.nameCopy);
                            writer.WriteNextCol(unkIndex);
                            writer.WriteNextCol(unk1.unk_0x00);
                            writer.WriteNextCol(unk1.unk_0x04);
                            writer.WriteNextRow();
                            unkIndex++;
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsSkeletalAnimator(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextColNicify(nameof(SkeletalAnimator.zero_0x00));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.zero_0x04));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.one_0x08));
                writer.WriteNextColNicify(nameof(SkeletalAnimator.propertiesPtr));

                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x00));
                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x04));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextColNicify(nameof(SkeletalProperties.unk_0x08));
                writer.WriteFlagNames<EnumFlags32>();
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x0C));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x10));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x14));
                writer.WriteNextColNicify(nameof(SkeletalProperties.zero_0x18));
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        if (!sceneObject.skeletalAnimator.propertiesPtr.IsNotNullPointer)
                        {
                            continue;
                        }

                        writer.WriteNextCol(file.FileName);
                        writer.WriteNextCol(gameObjectIndex);
                        writer.WriteNextCol(sceneObject.nameCopy);

                        writer.WriteNextCol(sceneObject.skeletalAnimator.zero_0x00);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.zero_0x04);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.one_0x08);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.propertiesPtr);

                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.unk_0x00);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteFlags(sceneObject.skeletalAnimator.properties.unk_0x04);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteFlags(sceneObject.skeletalAnimator.properties.unk_0x08);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x0C);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x10);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x14);
                        writer.WriteNextCol(sceneObject.skeletalAnimator.properties.zero_0x18);
                        writer.WriteNextRow();

                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsCollisionTri(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Tri Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderTriangle.unk_0x00));
                writer.WriteNextColNicify(nameof(ColliderTriangle.normal));
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex0));
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex1));
                writer.WriteNextColNicify(nameof(ColliderTriangle.vertex2));
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed0));
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed1));
                writer.WriteNextColNicify(nameof(ColliderTriangle.precomputed2));

                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        if (gameObject.instanceReference.colliderGeometry.triCount == 0)
                        {
                            continue;
                        }

                        int triIndex = 0;
                        foreach (var tri in gameObject.instanceReference.colliderGeometry.tris)
                        {
                            writer.WriteNextCol(file.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(gameObject.nameCopy);

                            writer.WriteNextCol(triIndex++);
                            writer.WriteStartAddress(tri);

                            writer.WriteNextCol(tri.unk_0x00);
                            writer.WriteNextCol(tri.normal);
                            writer.WriteNextCol(tri.vertex0);
                            writer.WriteNextCol(tri.vertex1);
                            writer.WriteNextCol(tri.vertex2);
                            writer.WriteNextCol(tri.precomputed0);
                            writer.WriteNextCol(tri.precomputed1);
                            writer.WriteNextCol(tri.precomputed2);

                            writer.WriteNextRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeGameObjectsCollisionQuad(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");

                writer.WriteNextCol("Quad Index");
                writer.WriteNextCol("Addr");

                writer.WriteNextColNicify(nameof(ColliderQuad.unk_0x00));
                writer.WriteNextColNicify(nameof(ColliderQuad.normal));
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex0));
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex1));
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex2));
                writer.WriteNextColNicify(nameof(ColliderQuad.vertex3));
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed0));
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed1));
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed2));
                writer.WriteNextColNicify(nameof(ColliderQuad.precomputed3));

                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int gameObjectIndex = 0;
                    foreach (var gameObject in file.Value.sceneObjects)
                    {
                        if (gameObject.instanceReference.colliderGeometry.quadCount == 0)
                        {
                            continue;
                        }

                        int quadIndex = 0;
                        foreach (var quad in gameObject.instanceReference.colliderGeometry.quads)
                        {
                            writer.WriteNextCol(file.FileName);
                            writer.WriteNextCol(gameObjectIndex);
                            writer.WriteNextCol(gameObject.nameCopy);

                            writer.WriteNextCol(quadIndex++);
                            writer.WriteStartAddress(quad);

                            writer.WriteNextCol(quad.unk_0x00);
                            writer.WriteNextCol(quad.normal);
                            writer.WriteNextCol(quad.vertex0);
                            writer.WriteNextCol(quad.vertex1);
                            writer.WriteNextCol(quad.vertex2);
                            writer.WriteNextCol(quad.vertex3);
                            writer.WriteNextCol(quad.precomputed0);
                            writer.WriteNextCol(quad.precomputed1);
                            writer.WriteNextCol(quad.precomputed2);
                            writer.WriteNextCol(quad.precomputed3);

                            writer.WriteNextRow();
                        }
                        gameObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        #endregion


        public void AnalyzeHeaders(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(Header.unk_0x00) + " a");
                writer.WriteNextCol(nameof(Header.unk_0x00) + " b");
                writer.WriteNextCol(nameof(Header.trackNodesPtr));
                writer.WriteNextCol(nameof(Header.trackNodesPtr));
                writer.WriteNextCol(nameof(Header.surfaceAttributeAreasPtr));
                writer.WriteNextCol(nameof(Header.surfaceAttributeAreasPtr));
                writer.WriteNextCol(nameof(Header.boostPadsActive));
                writer.WriteNextCol(nameof(Header.surfaceAttributeMeshTablePtr));
                writer.WriteNextCol(nameof(Header.zeroes0x20_Ptr));
                writer.WriteNextCol(nameof(Header.trackMinHeightPtr));
                writer.WriteNextCol(nameof(Header.zero_0x28));
                writer.WriteNextCol(nameof(Header.sceneObjectCount));
                writer.WriteNextCol(nameof(Header.unk_sceneObjectCount1));
                writer.WriteNextCol(nameof(Header.unk_sceneObjectCount2));
                writer.WriteNextCol(nameof(Header.sceneObjectsPtr));
                writer.WriteNextCol(nameof(Header.unkBool32_0x58));
                writer.WriteNextCol(nameof(Header.unknownSolsTriggerPtrs));
                writer.WriteNextCol(nameof(Header.unknownSolsTriggerPtrs));
                writer.WriteNextCol(nameof(Header.sceneInstancesListPtrs));
                writer.WriteNextCol(nameof(Header.sceneInstancesListPtrs));
                writer.WriteNextCol(nameof(Header.sceneOriginObjectsListPtrs));
                writer.WriteNextCol(nameof(Header.sceneOriginObjectsListPtrs));
                writer.WriteNextCol(nameof(Header.unused_0x74_0x78));
                writer.WriteNextCol(nameof(Header.unused_0x74_0x78));
                writer.WriteNextCol(nameof(Header.circuitType));
                writer.WriteNextCol(nameof(Header.unknownStageData2Ptr));
                writer.WriteNextCol(nameof(Header.unknownStageData1Ptr));
                writer.WriteNextCol(nameof(Header.unused_0x88_0x8C));
                writer.WriteNextCol(nameof(Header.unused_0x88_0x8C));
                writer.WriteNextCol(nameof(Header.trackLengthPtr));
                writer.WriteNextCol(nameof(Header.unknownTrigger1sPtr));
                writer.WriteNextCol(nameof(Header.unknownTrigger1sPtr));
                writer.WriteNextCol(nameof(Header.visualEffectTriggersPtr));
                writer.WriteNextCol(nameof(Header.visualEffectTriggersPtr));
                writer.WriteNextCol(nameof(Header.courseMetadataTriggersPtr));
                writer.WriteNextCol(nameof(Header.courseMetadataTriggersPtr));
                writer.WriteNextCol(nameof(Header.arcadeCheckpointTriggersPtr));
                writer.WriteNextCol(nameof(Header.arcadeCheckpointTriggersPtr));
                writer.WriteNextCol(nameof(Header.storyObjectTriggersPtr));
                writer.WriteNextCol(nameof(Header.storyObjectTriggersPtr));
                writer.WriteNextCol(nameof(Header.trackCheckpointTable8x8Ptr));
                // Structure
                writer.WriteNextCol(nameof(Header.courseBounds) + "." + nameof(Header.courseBounds.x));
                writer.WriteNextCol(nameof(Header.courseBounds) + "." + nameof(Header.courseBounds.z));
                writer.WriteNextCol(nameof(Header.courseBounds) + "." + nameof(Header.courseBounds.unk_0x08));
                writer.WriteNextCol(nameof(Header.courseBounds) + "." + nameof(Header.courseBounds.unk_0x0C));
                writer.WriteNextCol(nameof(Header.courseBounds) + "." + nameof(Header.courseBounds.unk_0x10));
                writer.WriteNextCol(nameof(Header.courseBounds) + "." + nameof(Header.courseBounds.unk_0x14));
                // 
                writer.WriteNextCol(nameof(Header.zero_0xD8));
                writer.WriteNextRow();

                foreach (var sceneSobj in sceneSobjs)
                {
                    var scene = sceneSobj.Value;

                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(CourseUtility.GetVenueID(scene.ID).GetDescription());
                    writer.WriteNextCol(((CourseIndexAX)scene.ID).GetDescription());
                    writer.WriteNextCol(scene.IsFileGX ? "GX" : "AX");

                    writer.WriteNextCol(scene.unk_0x00.a);
                    writer.WriteNextCol(scene.unk_0x00.b);
                    writer.WriteNextCol(scene.trackNodesPtr.Length);
                    writer.WriteNextCol(scene.trackNodesPtr.HexAddress);
                    writer.WriteNextCol(scene.surfaceAttributeAreasPtr.Length);
                    writer.WriteNextCol(scene.surfaceAttributeAreasPtr.HexAddress);
                    writer.WriteNextCol(scene.boostPadsActive);
                    writer.WriteNextCol(scene.staticColliderMeshTablePtr.HexAddress);
                    writer.WriteNextCol(scene.zeroes0x20Ptr.HexAddress);
                    writer.WriteNextCol(scene.trackMinHeightPtr.HexAddress);
                    writer.WriteNextCol(0);// coliHeader.zero_0x28);
                    writer.WriteNextCol(scene.sceneObjectCount);
                    if (scene.IsFileGX)
                    {
                        writer.WriteNextCol(scene.unk_sceneObjectCount1);
                    }
                    else // is AX
                    {
                        writer.WriteNextCol();
                    }
                    writer.WriteNextCol(scene.unk_sceneObjectCount2);
                    writer.WriteNextCol(scene.sceneObjectsPtr.HexAddress);
                    writer.WriteNextCol(scene.unkBool32_0x58);
                    writer.WriteNextCol(scene.unknownSolsTriggerPtrs.Length);
                    writer.WriteNextCol(scene.unknownSolsTriggerPtrs.HexAddress);
                    writer.WriteNextCol(scene.sceneInstancesListPtrs.Length);
                    writer.WriteNextCol(scene.sceneInstancesListPtrs.HexAddress);
                    writer.WriteNextCol(scene.sceneOriginObjectsListPtrs.Length);
                    writer.WriteNextCol(scene.sceneOriginObjectsListPtrs.HexAddress);
                    writer.WriteNextCol(scene.zero0x74);
                    writer.WriteNextCol(scene.zero0x78);
                    writer.WriteNextCol(scene.circuitType);
                    writer.WriteNextCol(scene.unknownStageData2Ptr.HexAddress);
                    writer.WriteNextCol(scene.unknownStageData1Ptr.HexAddress);
                    writer.WriteNextCol(scene.zero0x88);
                    writer.WriteNextCol(scene.zero0x8C);
                    writer.WriteNextCol(scene.trackLengthPtr.HexAddress);
                    writer.WriteNextCol(scene.unknownTrigger1sPtr.Length);
                    writer.WriteNextCol(scene.unknownTrigger1sPtr.HexAddress);
                    writer.WriteNextCol(scene.visualEffectTriggersPtr.Length);
                    writer.WriteNextCol(scene.visualEffectTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.courseMetadataTriggersPtr.Length);
                    writer.WriteNextCol(scene.courseMetadataTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.arcadeCheckpointTriggersPtr.Length);
                    writer.WriteNextCol(scene.arcadeCheckpointTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.storyObjectTriggersPtr.Length);
                    writer.WriteNextCol(scene.storyObjectTriggersPtr.HexAddress);
                    writer.WriteNextCol(scene.trackCheckpointTablePtr.HexAddress);
                    // Structure
                    writer.WriteNextCol(scene.courseBounds.x);
                    writer.WriteNextCol(scene.courseBounds.z);
                    writer.WriteNextCol(scene.courseBounds.unk_0x08);
                    writer.WriteNextCol(scene.courseBounds.unk_0x0C);
                    writer.WriteNextCol(scene.courseBounds.unk_0x10);
                    writer.WriteNextCol(scene.courseBounds.unk_0x14);
                    //
                    writer.WriteNextCol(0);// coliHeader.zero_0xD8);
                    writer.WriteNextCol(scene.trackMinHeight.value);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        public void AnalyzeUnknownTrigger1(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("Start");
                writer.WriteNextCol("End");
                //
                writer.WriteNextCol(nameof(UnknownTrigger1.unk_0x20));
                writer.WriteNextCol(nameof(UnknownTrigger1.unk_0x20));
                //
                writer.WriteNextCol("Order");
                writer.WriteNextCol("Index");
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    int count = 0;
                    int total = scene.unknownTrigger1s.Length;
                    foreach (var item in scene.unknownTrigger1s)
                    {
                        count++;

                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);

                        writer.WriteNextCol(item.StartAddressHex());
                        writer.WriteNextCol(item.EndAddressHex());

                        writer.WriteNextCol(item.unk_0x20);
                        writer.WriteNextCol($"0x{(int)item.unk_0x20:X8}");

                        writer.WriteNextCol(count);
                        writer.WriteNextCol($"[{count}/{total}]");

                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        //public void AnalyzeVenueMetadataObjects(string fileName)
        //{
        //    using (var writer = AnalyzerUtility.OpenWriter(fileName))
        //    {
        //        // Write header
        //        writer.PushCol("File");
        //        writer.PushCol("Index");
        //        writer.PushCol("Stage");
        //        writer.PushCol("Venue");
        //        writer.PushCol("AX/GX");
        //        //
        //        writer.PushCol(nameof(VenueMetadataObject.position));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x0C));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x0E));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x10));
        //        writer.PushCol(nameof(VenueMetadataObject.unk_0x12));
        //        writer.PushCol(nameof(VenueMetadataObject.positionOrScale));
        //        writer.PushCol(nameof(VenueMetadataObject.venue));
        //        //
        //        writer.PushRow();

        //        foreach (var file in coliSobjs)
        //        {
        //            var scene = file.Value;
        //            var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
        //            var courseID = ((CourseIDEx)scene.ID).GetDescription();
        //            var isAxGx = scene.IsFileGX ? "GX" : "AX";

        //            foreach (var item in scene.venueMetadataObjects)
        //            {
        //                writer.PushCol(scene.FileName);
        //                writer.PushCol(scene.ID);
        //                writer.PushCol(venueID);
        //                writer.PushCol(courseID);
        //                writer.PushCol(isAxGx);
        //                //
        //                writer.PushCol(item.position);
        //                writer.PushCol(item.unk_0x0C);
        //                writer.PushCol(item.unk_0x0E);
        //                writer.PushCol(item.unk_0x10);
        //                writer.PushCol(item.unk_0x12);
        //                writer.PushCol(item.positionOrScale);
        //                writer.PushCol(item.venue);
        //                //
        //                writer.PushRow();
        //            }
        //            writer.Flush();
        //        }
        //    }
        //}

        public void AnalyzeStoryObjectTrigger(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(StoryObjectTrigger.zero_0x00));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rockGroupOrderIndex));
                writer.WriteNextCol(nameof(StoryObjectTrigger.RockGroup));
                writer.WriteNextCol(nameof(StoryObjectTrigger.Difficulty));
                writer.WriteNextCol(nameof(StoryObjectTrigger.story2RockScale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.storyObjectPathPtr));
                writer.WriteNextCol(nameof(StoryObjectTrigger.scale));
                writer.WriteNextCol(nameof(StoryObjectTrigger.rotation));
                writer.WriteNextCol(nameof(StoryObjectTrigger.position));
                //
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var item in scene.storyObjectTriggers)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(item.zero_0x00);
                        writer.WriteNextCol(item.rockGroupOrderIndex);
                        writer.WriteNextCol(item.RockGroup);
                        writer.WriteNextCol(item.Difficulty);
                        writer.WriteNextCol(item.story2RockScale);
                        writer.WriteNextCol(item.storyObjectPathPtr);
                        writer.WriteNextCol(item.scale);
                        writer.WriteNextCol(item.rotation);
                        writer.WriteNextCol(item.position);
                        //
                        writer.WriteNextRow();
                    }
                    writer.Flush();
                }
            }
        }

        public void AnalyzeUnknownAnimationData(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("Index");
                //
                writer.WriteNextCol(nameof(KeyableAttribute.easeMode));
                writer.WriteNextCol(nameof(KeyableAttribute.time));
                writer.WriteNextCol(nameof(KeyableAttribute.value));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentIn));
                writer.WriteNextCol(nameof(KeyableAttribute.zTangentOut));
                //
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    var totalD1 = scene.unknownStageData2.animationCurves.Length;
                    var countD1 = 0;
                    foreach (var animationCurve in scene.unknownStageData2.animationCurves)
                    {
                        countD1++;
                        foreach (var keyableAttribute in animationCurve.keyableAttributes)
                        {
                            writer.WriteNextCol(scene.FileName);
                            writer.WriteNextCol(scene.ID);
                            writer.WriteNextCol(venueID);
                            writer.WriteNextCol(courseID);
                            writer.WriteNextCol(isAxGx);
                            //
                            writer.WriteNextCol($"[{countD1}/{totalD1}]");
                            //
                            writer.WriteNextCol(keyableAttribute.easeMode);
                            writer.WriteNextCol(keyableAttribute.time);
                            writer.WriteNextCol(keyableAttribute.value);
                            writer.WriteNextCol(keyableAttribute.zTangentIn);
                            writer.WriteNextCol(keyableAttribute.zTangentOut);
                            //
                            writer.WriteNextRow();
                        }
                    }
                    writer.Flush();
                }
            }
        }

        public void AnalyzeUnknownStageData1(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x00));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x04) + " " + nameof(UnknownStageData1.unk_0x04.a));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x04) + " " + nameof(UnknownStageData1.unk_0x04.b));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x0C));
                writer.WriteNextCol(nameof(UnknownStageData1.unk_0x18));
                //
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    var scene = file.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    writer.WriteNextCol(scene.FileName);
                    writer.WriteNextCol(scene.ID);
                    writer.WriteNextCol(venueID);
                    writer.WriteNextCol(courseID);
                    writer.WriteNextCol(isAxGx);
                    //
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x00);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x04.a);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x04.b);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x0C);
                    writer.WriteNextCol(scene.unknownStageData1.unk_0x18);
                    //
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }


        public void AnalyzeSceneObjectTransforms(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Game Object #");
                writer.WriteNextCol("Game Object");
                writer.WriteNextCol($"matrix.x");
                writer.WriteNextCol($"matrix.y");
                writer.WriteNextCol($"matrix.z");
                writer.WriteNextCol($"euler.x");
                writer.WriteNextCol($"euler.y");
                writer.WriteNextCol($"euler.z");
                writer.WriteNextCol("Decomposed phi");
                writer.WriteNextCol("Decomposed theta");
                writer.WriteNextCol("Decomposed psi");
                writer.WriteNextCol(nameof(UnknownTransformOption));
                writer.WriteNextCol(nameof(ObjectActiveOverride));
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int sceneObjectIndex = 0;
                    foreach (var sceneObject in file.Value.sceneObjects)
                    {
                        if (!sceneObject.transformPtr.IsNotNullPointer)
                            continue;

                        writer.WriteNextCol(file.FileName);
                        writer.WriteNextCol(sceneObjectIndex);
                        writer.WriteNextCol(sceneObject.nameCopy);

                        // Matrix
                        var matrix = sceneObject.transformMatrix3x4.rotationEuler;
                        writer.WriteNextCol(matrix.x);
                        writer.WriteNextCol(matrix.y);
                        writer.WriteNextCol(matrix.z);

                        // euler
                        var euler = sceneObject.transform.DecomposedRotation.EulerAngles;
                        writer.WriteNextCol(euler.x);
                        writer.WriteNextCol(euler.y);
                        writer.WriteNextCol(euler.z);

                        var decomposed = sceneObject.transform.DecomposedRotation;
                        writer.WriteNextCol(decomposed.phi);
                        writer.WriteNextCol(decomposed.theta);
                        writer.WriteNextCol(decomposed.psi);

                        writer.WriteNextCol(sceneObject.transform.UnknownOption);
                        writer.WriteNextCol(sceneObject.transform.ObjectActiveOverride);

                        writer.WriteNextRow();
                        sceneObjectIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeTrackNodes(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Track Node");
                writer.WriteNextCol("Track Point");
                writer.WriteNextColNicify(nameof(TrackPoint.unk_0x00));
                writer.WriteNextColNicify(nameof(TrackPoint.unk_0x04));
                writer.WriteNextColNicify(nameof(TrackPoint.trackDistanceStart));
                writer.WriteNextColNicify(nameof(TrackPoint.tangentStart));
                writer.WriteNextColNicify(nameof(TrackPoint.positionStart));
                writer.WriteNextColNicify(nameof(TrackPoint.trackDistanceEnd));
                writer.WriteNextColNicify(nameof(TrackPoint.tangentEnd));
                writer.WriteNextColNicify(nameof(TrackPoint.positionEnd));
                writer.WriteNextColNicify(nameof(TrackPoint.transformDistanceEnd));
                writer.WriteNextColNicify(nameof(TrackPoint.transformDistanceStart));
                writer.WriteNextColNicify(nameof(TrackPoint.trackWidth));
                writer.WriteNextColNicify(nameof(TrackPoint.isTrackContinuousStart));
                writer.WriteNextColNicify(nameof(TrackPoint.isTrackContinuousEnd));
                writer.WriteNextColNicify(nameof(TrackPoint.zero_0x4E));
                writer.WriteNextRow();

                foreach (var file in sceneSobjs)
                {
                    int nodeLength = file.Value.trackNodes.Length;
                    int nodeIndex = 0;
                    foreach (var trackNode in file.Value.trackNodes)
                    {
                        int pointLength = trackNode.points.Length;
                        int pointIndex = 0;
                        foreach (var trackPoint in trackNode.points)
                        {
                            writer.WriteNextCol(file.name);
                            writer.WriteNextCol($"[{nodeIndex}/{nodeLength}]");
                            writer.WriteNextCol($"[{pointIndex}/{pointLength}]");

                            writer.WriteNextCol(trackPoint.unk_0x00);
                            writer.WriteNextCol(trackPoint.unk_0x04);
                            writer.WriteNextCol(trackPoint.trackDistanceStart);
                            writer.WriteNextCol(trackPoint.tangentStart);
                            writer.WriteNextCol(trackPoint.positionStart);
                            writer.WriteNextCol(trackPoint.trackDistanceEnd);
                            writer.WriteNextCol(trackPoint.tangentEnd);
                            writer.WriteNextCol(trackPoint.positionEnd);
                            writer.WriteNextCol(trackPoint.transformDistanceEnd);
                            writer.WriteNextCol(trackPoint.transformDistanceStart);
                            writer.WriteNextCol(trackPoint.trackWidth);
                            writer.WriteNextCol(trackPoint.isTrackContinuousStart);
                            writer.WriteNextCol(trackPoint.isTrackContinuousEnd);
                            writer.WriteNextCol(trackPoint.zero_0x4E);
                            writer.WriteNextRow();

                            pointIndex++;
                        }
                        nodeIndex++;
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeSceneStaticCollider(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.collisionTrisPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.collisionTriIndexesPtr));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.x));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.z));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x08));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x0C));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x10));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x14));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.collisionQuadsPtr));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.collisionQuadIndexesPtr));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.x));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.z));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x08));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x0C));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x10));
                writer.WriteNextColNicify(nameof(GameCube.GFZ.CourseCollision.Bounds.unk_0x14));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.colliderTriangles));
                writer.WriteNextColNicify(nameof(StaticColliderMeshes.colliderQuads));
                writer.WriteNextRow();

                int index = 0;

                foreach (var file in sceneSobjs)
                {
                    var scene = file.Value;
                    var table = scene.staticColliderMeshTable;

                    writer.WriteNextCol(file.name);
                    writer.WriteNextCol(index++);
                    writer.WriteNextCol(table.collisionTrisPtr.HexAddress);
                    writer.WriteNextCol(table.collisionTriIndexesPtr.Length);
                    writer.WriteNextCol(table.meshBounds.x);
                    writer.WriteNextCol(table.meshBounds.z);
                    writer.WriteNextCol(table.meshBounds.unk_0x08);
                    writer.WriteNextCol(table.meshBounds.unk_0x0C);
                    writer.WriteNextCol(table.meshBounds.unk_0x10);
                    writer.WriteNextCol(table.meshBounds.unk_0x14);
                    writer.WriteNextCol(table.collisionQuadsPtr.HexAddress);
                    writer.WriteNextCol(table.collisionQuadIndexesPtr.Length);
                    writer.WriteNextCol(table.ununsedMeshBounds.x);
                    writer.WriteNextCol(table.ununsedMeshBounds.z);
                    writer.WriteNextCol(table.ununsedMeshBounds.unk_0x08);
                    writer.WriteNextCol(table.ununsedMeshBounds.unk_0x0C);
                    writer.WriteNextCol(table.ununsedMeshBounds.unk_0x10);
                    writer.WriteNextCol(table.ununsedMeshBounds.unk_0x14);
                    writer.WriteNextCol(table.colliderTriangles.Length);
                    writer.WriteNextCol(table.colliderQuads.Length);
                    writer.WriteNextRow();
                }
                writer.Flush();
            }
        }

        public void AnalyzeSceneObjectReferences(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol("Object Type");
                writer.WriteNextCol(nameof(SceneObjectReference.zero_0x00));
                writer.WriteNextCol(nameof(SceneObjectReference.namePtr));
                writer.WriteNextCol(nameof(SceneObjectReference.zero_0x08));
                writer.WriteNextCol(nameof(SceneObjectReference.unk_0x0C));
                //
                writer.WriteNextRow();

                foreach (var sceneSobj in sceneSobjs)
                {
                    var scene = sceneSobj.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var objectsList = new List<(SceneObjectReference sor, string category)>();
                    foreach (var sceneObject in scene.sceneInstancesList)
                    {
                        var sceneObjectReference = sceneObject.objectReference;
                        objectsList.Add((sceneObject.objectReference, "Instance"));
                    }
                    foreach (var sceneOriginObject in scene.sceneOriginObjectsList)
                    {
                        var sceneObjectReference = sceneOriginObject.instanceReference.objectReference;
                        objectsList.Add((sceneObjectReference, "Origin"));
                    }

                    // iterate
                    foreach (var sceneObjectReference in objectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObjectReference.sor.name);
                        writer.WriteNextCol(sceneObjectReference.category);
                        writer.WriteNextCol(sceneObjectReference.sor.zero_0x00);
                        writer.WriteNextCol(sceneObjectReference.sor.namePtr);
                        writer.WriteNextCol(sceneObjectReference.sor.zero_0x08);
                        writer.WriteNextCol(sceneObjectReference.sor.unk_0x0C);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeSceneInstanceReferences(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol("Object Type");
                writer.WriteNextCol(nameof(SceneInstanceReference.unk_0x00));
                writer.WriteNextCol(nameof(SceneInstanceReference.unk_0x04));
                writer.WriteNextCol(nameof(SceneInstanceReference.objectReferencePtr));
                writer.WriteNextCol(nameof(SceneInstanceReference.colliderGeometryPtr));
                //
                writer.WriteNextRow();

                foreach (var sceneSobj in sceneSobjs)
                {
                    var scene = sceneSobj.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    // Get all the scene object references
                    var objectsList = new List<(SceneInstanceReference sir, string category)>();
                    foreach (var sceneInstance in scene.sceneInstancesList)
                    {
                        objectsList.Add((sceneInstance, "Instance"));
                    }
                    foreach (var sceneOriginObject in scene.sceneOriginObjectsList)
                    {
                        var sceneInstance = sceneOriginObject.instanceReference;
                        objectsList.Add((sceneInstance, "Origin"));
                    }

                    // iterate
                    foreach (var sceneObjectReference in objectsList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        writer.WriteNextCol(sceneObjectReference.sir.objectReference.name);
                        writer.WriteNextCol(sceneObjectReference.category);
                        writer.WriteNextCol(sceneObjectReference.sir.unk_0x00);
                        writer.WriteNextCol(sceneObjectReference.sir.unk_0x04);
                        writer.WriteNextCol(sceneObjectReference.sir.objectReferencePtr);
                        writer.WriteNextCol(sceneObjectReference.sir.colliderGeometryPtr);
                        //
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

        public void AnalyzeSceneInstanceAndObjectReferences(string fileName)
        {
            using (var writer = AnalyzerUtility.OpenWriter(fileName))
            {
                // Write header
                writer.WriteNextCol("File");
                writer.WriteNextCol("Index");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("AX/GX");
                //
                writer.WriteNextCol("name");
                writer.WriteNextCol(nameof(SceneInstanceReference.unk_0x00));
                writer.WriteNextCol(nameof(SceneInstanceReference.unk_0x04));
                writer.WriteNextCol(nameof(SceneInstanceReference.objectReferencePtr));
                writer.WriteNextCol(nameof(SceneInstanceReference.colliderGeometryPtr));
                writer.WriteNextCol(nameof(SceneObjectReference.zero_0x00));
                writer.WriteNextCol(nameof(SceneObjectReference.namePtr));
                writer.WriteNextCol(nameof(SceneObjectReference.zero_0x08));
                writer.WriteNextCol(nameof(SceneObjectReference.unk_0x0C));
                //
                writer.WriteNextRow();

                foreach (var sceneSobj in sceneSobjs)
                {
                    var scene = sceneSobj.Value;
                    var venueID = CourseUtility.GetVenueID(scene.ID).GetDescription();
                    var courseID = ((CourseIndexAX)scene.ID).GetDescription();
                    var isAxGx = scene.IsFileGX ? "GX" : "AX";

                    foreach (var sceneInstance in scene.sceneInstancesList)
                    {
                        writer.WriteNextCol(scene.FileName);
                        writer.WriteNextCol(scene.ID);
                        writer.WriteNextCol(venueID);
                        writer.WriteNextCol(courseID);
                        writer.WriteNextCol(isAxGx);
                        //
                        var objectReference = sceneInstance.objectReference;
                        writer.WriteNextCol(objectReference.name);
                        writer.WriteNextCol(sceneInstance.unk_0x00);
                        writer.WriteNextCol(sceneInstance.unk_0x04);
                        writer.WriteNextCol(sceneInstance.objectReferencePtr);
                        writer.WriteNextCol(sceneInstance.colliderGeometryPtr);
                        writer.WriteNextCol(objectReference.zero_0x00);
                        writer.WriteNextCol(objectReference.namePtr);
                        writer.WriteNextCol(objectReference.zero_0x08);
                        writer.WriteNextCol(objectReference.unk_0x0C);
                        writer.WriteNextRow();
                    }
                }
                writer.Flush();
            }
        }

    }
}