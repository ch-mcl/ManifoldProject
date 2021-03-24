﻿using GameCube.GFZ;
using GameCube.GFZ.Camera;
using UnityEngine;

namespace Manifold.IO.GFZ.Camera
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCamera + "livecam_stage Analyzer")]
    public class LiveCameraStageAnalyzer : ExecutableScriptableObject,
        IAnalyzable
    {
        #region MEMBERS

        [Header("Output")]
        [SerializeField, BrowseFolderField]
        protected string outputPath;
        [SerializeField, BrowseFolderField("Assets/"), Tooltip("Used with IOOption.allFromSourceFolder")]
        protected string[] searchFolders;

        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterAnalysis = true;

        [Header("Analysis Options")]
        [SerializeField]
        protected IOOption analysisOption = IOOption.allFromAssetDatabase;
        [SerializeField]
        protected LiveCameraStageSobj[] analysisSobjs;

        #endregion

        public override string ExecuteText => "Analyze Live Camera Stage";

        public void Analyze()
        {
            analysisSobjs = AssetDatabaseUtility.GetSobjByOption(analysisSobjs, analysisOption, searchFolders);

            var filePath = AnalyzerUtility.GetAnalysisFilePathTSV(outputPath, "livecam_stage stacked");
            WriteLivecamStageStacked(filePath);

            OSUtility.OpenDirectory(openFolderAfterAnalysis, filePath);
        }

        public override void Execute() => Analyze();

        public void WriteLivecamStageStacked(string filePath)
        {
            using (var writer = AnalyzerUtility.OpenWriter(filePath))
            {
                writer.WriteNextCol("File Name");
                writer.WriteNextCol("Stage Index");
                writer.WriteNextCol("Venue");
                writer.WriteNextCol("Stage");
                writer.WriteNextCol("Pan Index");
                writer.WriteColNicify(nameof(CameraPan.frameCount));
                writer.WriteColNicify(nameof(CameraPan.lerpSpeed));
                writer.WriteColNicify(nameof(CameraPan.zero_0x08));
                writer.WriteNextCol("Section");
                writer.WriteColNicify(nameof(CameraPan.from.cameraPosition));
                writer.WriteColNicify(nameof(CameraPan.from.lookatPosition));
                writer.WriteColNicify(nameof(CameraPan.from.fov));
                writer.WriteColNicify(nameof(CameraPan.from.rotation) + " Raw");
                writer.WriteColNicify(nameof(CameraPan.from.Rotation));
                writer.WriteColNicify(nameof(CameraPan.from.zero_0x1E));
                writer.WriteColNicify(nameof(CameraPan.from.interpolation));
                writer.WriteColNicify(nameof(CameraPan.from.zero_0x22));
                writer.WriteNextRow();

                foreach (var sobj in analysisSobjs)
                {
                    var liveCamStage = sobj.Value;

                    var panIndex = 0;
                    foreach (var pan in liveCamStage.Pans)
                    {

                        var stageIndexText = System.Text.RegularExpressions.Regex.Match(sobj.FileName, @"\d+").Value;
                        var stageIndex = int.Parse(stageIndexText);
                        var stageInfo = CourseUtility.GetCourseInfo(stageIndex);

                        var panArray = new CameraPanPoint[] { pan.from, pan.to };
                        for (int i = 0; i < panArray.Length; i++)
                        {
                            writer.WriteNextCol(sobj.FileName);
                            writer.WriteNextCol(stageIndex);
                            writer.WriteNextCol(stageInfo.Item1.GetDescription());
                            writer.WriteNextCol(stageInfo.Item3.GetDescription());
                            writer.WriteNextCol(panIndex);
                            writer.WriteNextCol(pan.frameCount);
                            writer.WriteNextCol(pan.lerpSpeed);
                            writer.WriteNextCol(pan.zero_0x08);

                            if (i == 0)
                            {
                                writer.WriteNextCol("FROM");
                            }
                            else
                            {
                                writer.WriteNextCol("TO");
                            }

                            writer.WriteNextCol(panArray[i].cameraPosition);
                            writer.WriteNextCol(panArray[i].lookatPosition);
                            writer.WriteNextCol(panArray[i].fov);
                            writer.WriteNextCol(panArray[i].rotation);
                            writer.WriteNextCol(panArray[i].Rotation);
                            writer.WriteNextCol(panArray[i].zero_0x1E);
                            writer.WriteNextCol(panArray[i].interpolation);
                            writer.WriteNextCol(panArray[i].zero_0x22);
                            writer.WriteNextRow();
                        }
                        panIndex++;
                    }
                }
            }
        }

        //public void WriteLivecamStageSingleLine(string filePath)
        //{
        //    using (var writer = AnalyzerUtility.OpenWriter(filePath))
        //    {
        //        writer.PushCol("File Name");
        //        writer.PushCol("Stage Index");
        //        writer.PushCol("Venue");
        //        writer.PushCol("Stage");
        //        writer.PushCol("Pan Index");
        //        writer.WriteColNicify(nameof(CameraPan.frameCount));
        //        writer.WriteColNicify(nameof(CameraPan.lerpSpeed));
        //        writer.WriteColNicify(nameof(CameraPan.zero_0x08));
        //        writer.PushCol("Section");
        //        writer.WriteColNicify(nameof(CameraPan.from.cameraPosition));
        //        writer.WriteColNicify(nameof(CameraPan.from.lookatPosition));
        //        writer.WriteColNicify(nameof(CameraPan.from.fov));
        //        writer.WriteColNicify(nameof(CameraPan.from.rotation));
        //        writer.WriteColNicify(nameof(CameraPan.from.unk_0x1D));
        //        writer.WriteColNicify(nameof(CameraPan.from.zero_0x1E));
        //        writer.WriteColNicify(nameof(CameraPan.from.interpolation));
        //        writer.WriteColNicify(nameof(CameraPan.from.zero_0x22));
        //        writer.PushRow();

        //        foreach (var sobj in analysisSobjs)
        //        {
        //            var liveCamStage = sobj.Value;

        //            var panIndex = 0;
        //            foreach (var pan in liveCamStage.Pans)
        //            {
        //                var stageIndexText = System.Text.RegularExpressions.Regex.Match(sobj.FileName, @"\d+").Value;
        //                var stageIndex = int.Parse(stageIndexText);
        //                var courseInfo = CourseUtility.GetCourseInfo(stageIndex);

        //                writer.PushCol(sobj.FileName);
        //                writer.PushCol(stageIndex);
        //                writer.PushCol(courseInfo.venue.GetDescription());
        //                writer.PushCol(courseInfo.name.GetDescription());
        //                writer.PushCol(panIndex);
        //                writer.PushCol(pan.frameCount);
        //                writer.PushCol(pan.lerpSpeed);
        //                writer.PushCol(pan.zero_0x08);
        //                // FROM
        //                writer.PushCol(pan.from.cameraPosition);
        //                writer.PushCol(pan.from.lookatPosition);
        //                writer.PushCol(pan.from.fov);
        //                writer.PushCol(pan.from.unk_0x1D);
        //                writer.WriteFlags(pan.from.interpolation);
        //                // TO
        //                writer.PushCol(pan.to.cameraPosition);
        //                writer.PushCol(pan.to.lookatPosition);
        //                writer.PushCol(pan.to.fov);
        //                writer.PushCol(pan.to.unk_0x1D);
        //                writer.WriteFlags(pan.to.interpolation);
        //                writer.PushRow();
        //                panIndex++;
        //            }
        //        }
        //    }
        //}
    }
}
