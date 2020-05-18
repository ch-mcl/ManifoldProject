﻿using StarkTools.IO;
using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZX01.GMA
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_GMA + "GMA Importer")]
    public class GmaImporter : ExecutableScriptableObject,
        IImportable
    {
        [Header("Import Settings")]
        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        protected string searchPattern = "*.GMA*";

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;


        public override string ExecuteText => "Import GMA";

        public override void Execute() => Import();

        public void Import()
        {
            importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            var importFilesUncompressed = GFZX01Utility.DecompressEachLZ(importFiles);
            ImportUtility.ImportManyAs<GmaSobj>(importFilesUncompressed, importFrom, importTo);
        }

    }
}