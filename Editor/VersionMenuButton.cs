using UnityEngine;
using UnityEditor;

namespace U.Universal.Ads.Editor
{
    public class VersionMenuButton : EditorWindow
    {

        [MenuItem("Universal/Ads/Version")]
        public static void PrintVersion()
        {

            Debug.Log(" U Framework: Universal Ads v1.0.0 for Unity");

        }
    }
}