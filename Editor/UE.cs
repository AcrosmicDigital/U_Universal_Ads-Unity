using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization;
using System.Xml;

namespace U.Universal.Ads.Editor
{
    internal static class UE
    {


        public static void CreateOverwriteSerializableClass<TResource>(string folderName, string fileName, TResource serializableResource, Func<string, string> FormatLog)

            where TResource : class

        {

            // Check if Env folder exist or create it
            if (!Directory.Exists(Application.dataPath + folderName))
            {
                Debug.Log(FormatLog("Creating Assets" + folderName + " directory"));
                Directory.CreateDirectory(Application.dataPath + folderName);
            }
            else
            {
                Debug.Log(FormatLog("Assets" + folderName + " already exist"));
            }


            if (serializableResource == null)
                Debug.LogError(FormatLog("Serializable object is null"));


            // Create the fullpath
            string path = Application.dataPath + folderName + fileName; // full path

            try
            {
                using (var writer = new FileStream(path, FileMode.Create))
                {

                    DataContractSerializer ser = new DataContractSerializer(typeof(TResource));

                    // Intentar serializar
                    ser.WriteObject(writer, serializableResource);

                    Debug.Log(FormatLog("Creating Asset" + folderName + fileName + " file"));

                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static TResource ReadSerializableResource<TResource>(string folderName, string fileName, Func<string, string> FormatLog)

           where TResource : class

        {

            string path = Application.dataPath + folderName + fileName; // full path

            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    using (var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                    {

                        var ser = new DataContractSerializer(typeof(TResource));

                        // Intentar desserializar
                        var file = (TResource)ser.ReadObject(reader, true);

                        return file;

                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }



        public static void CreateFile(string folderName, string fileName, string[] file, Func<string, string> FormatLog)
        {
            // Check if Env folder exist or create it
            if (!Directory.Exists(Application.dataPath + folderName))
            {
                Debug.Log(FormatLog("Creating Assets" + folderName + " directory"));
                Directory.CreateDirectory(Application.dataPath + folderName);
            }
            else
            {
                Debug.Log(FormatLog("Assets" + folderName + " already exist"));
            }

            // check if scenes file exist or create it
            if (!File.Exists(Application.dataPath + folderName + fileName))
            {
                Debug.Log(FormatLog("Creating Assets" + folderName + fileName + " file"));

                // Write the file
                File.WriteAllLines(Application.dataPath + folderName + fileName, file); // This should be async when is available

            }
            else
            {
                Debug.LogError(FormatLog("Assets" + folderName + fileName + " already exist"));
            }

        }

        public static void DeleteFile(string folderName, string fileName)
        {
            //Debug.Log($"Del: {Application.dataPath}{folderName}{fileName}");

            // Check if Env folder exist or create it
            if (!File.Exists(Application.dataPath + folderName + fileName))
            {
                //Debug.Log("noex");
                return;
            }

            File.Delete(Application.dataPath + folderName + fileName);

            if (File.Exists(Application.dataPath + folderName + fileName + ".meta"))
            {
                File.Delete(Application.dataPath + folderName + fileName + ".meta");
            }

        }

        public static void DeleteFolder(string folderName)
        {
            // Check if Env folder exist or create it
            if (!Directory.Exists(Application.dataPath + folderName))
            {
                return;
            }

            if (File.Exists(Application.dataPath + folderName.TrimEnd('/') + ".meta"))
            {
                File.Delete(Application.dataPath + folderName.TrimEnd('/') + ".meta");
            }

            Directory.Delete(Application.dataPath + folderName, true);

        }

        public static void CreateFileWithSaveFilePanelForceLocation(string folderName, string defaultFileName, Func<string, string[]> file, Func<string, string> FormatLog)
        {

            // Check if Env folder exist or create it
            if (!Directory.Exists(Application.dataPath + folderName))
            {
                Debug.Log(FormatLog("Creating Assets" + folderName + " directory"));
                Directory.CreateDirectory(Application.dataPath + folderName);
            }
            else
            {
                Debug.Log(FormatLog("Assets" + folderName + " already exist"));
            }

            // Read the File Name from the save file Panel
            var path = EditorUtility.SaveFilePanel(title: "Create " + defaultFileName, directory: Application.dataPath + folderName, defaultName: defaultFileName, extension: "cs");

            // Get only the filename
            var fileName = Path.GetFileName(path);
            var fileNameNoExtension = Path.GetFileNameWithoutExtension(path);

            // check if scenes file exist or create it
            if (!File.Exists(Application.dataPath + folderName + fileName))
            {
                Debug.Log(FormatLog("Creating Assets" + folderName + fileName + " file"));

                // Write the file
                File.WriteAllLines(Application.dataPath + folderName + fileName, file(fileNameNoExtension)); // This should be async when is available

            }
            else
            {
                Debug.LogError(FormatLog("Assets" + folderName + fileName + " already exist"));
            }

        }

        public static void CreateFileWithSaveFilePanel(string defaultFolderName, string defaultFileName, Func<string, string[]> file, Func<string, string> FormatLog)
        {
            // Check if Env folder exist or create it
            if (!Directory.Exists(Application.dataPath + defaultFolderName))
            {
                Debug.Log(FormatLog("Creating Assets" + defaultFolderName + " directory"));
                Directory.CreateDirectory(Application.dataPath + defaultFolderName);
            }
            else
            {
                Debug.Log(FormatLog("Assets" + defaultFolderName + " already exist"));
            }

            // Read the File Name from the save file Panel
            var path = EditorUtility.SaveFilePanel(title: "Create " + defaultFileName, directory: Application.dataPath, defaultName: defaultFileName, extension: "cs");

            // check if scenes file exist or create it
            if (!File.Exists(path))
            {
                Debug.Log(FormatLog("Creating " + path + " file"));

                // Write the file
                File.WriteAllLines(path, file(Path.GetFileNameWithoutExtension(path))); // This should be async when is available

            }
            else
            {
                Debug.LogError(FormatLog(path + " already exist"));
            }

        }

        public static void CreateFileWithSaveFilePanelAndCustomExtension(string defaultFolderName, string defaultFileName, Func<string, string[]> file, Func<string, string> FormatLog, string extension)
        {
            // Check if Env folder exist or create it
            if (!Directory.Exists(Application.dataPath + defaultFolderName))
            {
                Debug.Log(FormatLog("Creating Assets" + defaultFolderName + " directory"));
                Directory.CreateDirectory(Application.dataPath + defaultFolderName);
            }
            else
            {
                Debug.Log(FormatLog("Assets" + defaultFolderName + " already exist"));
            }

            // Read the File Name from the save file Panel
            var path = EditorUtility.SaveFilePanel(title: "Create " + defaultFileName, directory: Application.dataPath + defaultFolderName, defaultName: defaultFileName, extension: "cs");

            // Get only the filename
            var fileNameNoExtension = Path.GetFileNameWithoutExtension(path);
            var fileDirectory = Path.GetDirectoryName(path);

            var FilePath = fileDirectory + "/" + fileNameNoExtension + "." + extension + ".cs";

            // Create Files
            if (!File.Exists(FilePath))
            {
                Debug.Log(FormatLog("Creating " + FilePath + " file"));

                // Write the files
                File.WriteAllLines(FilePath, file(fileNameNoExtension)); // This should be async when is available

            }
            else
            {
                Debug.LogError(FormatLog(FilePath + " already exist"));
            }

        }

        public const string quote = "\"";

        public static string PrintBool(bool value)
        {
            if (value) return "true";
            else return "false";
        }

    }
}