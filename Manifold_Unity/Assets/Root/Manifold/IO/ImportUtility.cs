﻿using StarkTools.IO;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;

namespace Manifold.IO
{
    public class ImportUtility
    {
        public static TSobj Create<TSobj>(string destinationDir, string fileName)
            where TSobj : ScriptableObject
        {

            var filePath = $"Assets/{destinationDir}/{fileName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<TSobj>(filePath);
            if (asset != null)
            {
                return asset;
            }
            else
            {
                var sobj = ScriptableObject.CreateInstance<TSobj>();
                AssetDatabase.CreateAsset(sobj, filePath);
                EditorUtility.SetDirty(sobj);
                return sobj;
            }
        }

        public static TSobj CreateFromBinary<TSobj>(string destinationDir, string fileName, BinaryReader reader)
            where TSobj : ScriptableObject, IBinarySerializable
        {
            var sobj = Create<TSobj>(destinationDir, fileName);
            sobj.Deserialize(reader);
            return sobj;
        }

        public static TSobj CreateFromBinaryFile<TSobj>(string destinationDir, string fileName, BinaryReader reader)
        where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var sobj = CreateFromBinary<TSobj>(destinationDir, fileName, reader);
            sobj.FileName = fileName;
            return sobj;
        }

        public static TSobj ImportAs<TSobj>(BinaryReader reader, string file, string importFrom, string importTo, out string filePath)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var unityPath = GetUnityOutputPath(file, importFrom, importTo);
            var fileName = Path.GetFileName(file);

            // GENERIC
            var sobj = CreateFromBinaryFile<TSobj>(unityPath, fileName, reader);
            sobj.FileName = fileName;

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            // Out params
            filePath = $"{unityPath}/{fileName}";

            return sobj;
        }

        public static TSobj[] ImportManyAs<TSobj>(string[] importFiles, string importPath, string importDest, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var count = 0;
            var total = importFiles.Length;
            var sobjs = new TSobj[total];

            foreach (var file in importFiles)
            {
                using (var fileStream = File.Open(file, mode, access, share))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        var filepath = string.Empty;
                        var sobj = ImportAs<TSobj>(reader, file, importPath, importDest, out filepath);
                        sobjs[count] = sobj;
                        ImportProgBar<TSobj>(count, total, filepath);
                    }
                }
                count++;
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();

            return sobjs;
        }

        public static string GetUnityOutputPath(string importFile, string importFrom, string importTo)
        {
            // Get path to root import folder
            var path = UnityPathUtility.GetUnityDirectory(UnityPathUtility.UnityFolder.Assets);
            var dest = UnityPathUtility.CombineSystemPath(path, importTo);

            // get path to file import folder
            // TODO: Regex instead of this hack
            // NOTE: The +1 is for the final / fwd slash assuming the "BrowseButtonField"
            // is used and does not return the final / on the path.
            var folder = importFile.Remove(0, importFrom.Length + 1);
            folder = Path.GetDirectoryName(folder);

            // (A) prevent null/empty && (B) prevent "/" or "\\"
            if (!string.IsNullOrEmpty(folder) && folder.Length > 1)
            {
                // dest = dest + folder;
                dest = $"{dest}/{folder}";
            }

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                Debug.Log($"Created path <i>{dest}</i>");
            }

            var unityPath = UnityPathUtility.ToUnityFolderPath(dest, UnityPathUtility.UnityFolder.Assets);
            unityPath = UnityPathUtility.EnforceUnitySeparators(unityPath);

            return unityPath;
        }

        public static void ImportProgBar<T>(int count, int total, string info)
        {
            // Progress bar update
            var digitCount = total.ToString().Length;
            var currentIndexStr = (count + 1).ToString().PadLeft(digitCount);
            var title = $"Importing {typeof(T).Name} ({currentIndexStr}/{total})";
            var progress = count / (float)total;
            EditorUtility.DisplayProgressBar(title, info, progress);
        }

        // path w/o Assets
        public static void EnsureAssetFolderExists(string folderPath)
        {
            var assetFolders = $"Assets/{folderPath}";
            var folders = assetFolders.Split('/');
            var numDirs = folders.Length;

            // Init parent folder
            var parentFolder = folders[0];

            // Append all directories to parent directory
            for (int i = 0; i < numDirs - 1; i++)
            {
                var nextFolder = folders[i+1];
                var fullPath = $"{parentFolder}/{nextFolder}";

                var doCreateFolder = !AssetDatabase.IsValidFolder(fullPath);
                if (doCreateFolder)
                {
                    AssetDatabase.CreateFolder(parentFolder, nextFolder);
                }

                parentFolder = fullPath;
            }
        }

    }
}