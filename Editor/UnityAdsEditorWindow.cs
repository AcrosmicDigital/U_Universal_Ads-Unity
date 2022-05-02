using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static U.Universal.Ads.Editor.UE;

namespace U.Universal.Ads.Editor
{
    public class UnityAdsEditorWindow : EditorWindow
    {

        #region Persistent File
        private static string PersistentFolderName => "/U.LocalScripts/UAds/";
        private static string PersistentFileName => "UAds.editorprefs.txt";
        #endregion Persistent File

        #region Startup File
        private static string StartupFolderName => "/U.LocalScripts/UAds/";
        private static string StartupFileName => "UAds.startup.cs";
        private static string[] StartupFile(IEnumerable<AdUnitSettings> adUnits)
        {
            var file = new List<string>();

            // FI1
            var fi1 = new string[]
            {
                "using System;",
                "",
                "namespace LocalScripts.UAds",
                "{",
                "    public static partial class Startup",
                "    {",
                "",
                "        public static void Initialize()",
                "        {",
                "",
                "            AdsInitializer.Initialize();",
                "            AdsInitializer.onInitializationComplete += OnInitializationComplete;",
                "",
                "        }",
                "",
                "        // Called when ads are Ready to be loaded",
                "        private static void OnInitializationComplete(object sender, EventArgs e)",
                "        {",
            };
            foreach (var line in fi1) file.Add(line);

            // FI2
            foreach (var adUnit in adUnits)
            {
                file.Add($"            {adUnit.className}AdUnit.Load();");
            }

            // FI3
            var fi3 = new string[]
            {
                "        }",
                "",
                "    }",
                "}",
            };
            foreach (var line in fi3) file.Add(line);

            // FILE
            return file.ToArray();
        }
        #endregion Startup File

        #region Initializer File
        private static string InitializerFolderName => "/U.LocalScripts/UAds/";
        private static string InitializerFileName => "AdsInitializer.cs";
        private readonly static string[] InitializerFile =
        {
            "using System;",
            "using System.Collections;",
            "using UnityEngine;",
            "using UnityEngine.Advertisements;",
            "",
            "namespace LocalScripts.UAds",
            "{",
            "    public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener",
            "    {",
            "",
            "        #region Singleton",
            "",
            "        public static event EventHandler onInitializationComplete;  // Event rised when ads are intitialized",
            "",
            "        // This is the gameObject where all related behaviours will be.",
            "        private static GameObject _host;",
            "        public static GameObject Host { get",
            "            {",
            "                if(_host == null)",
            "                {",
            "                    // Create the host",
            "                    _host = new GameObject("+quote+"AdsInitializer-Host"+quote+");",
            "                    DontDestroyOnLoad(_host);",
            "                }",
            "",
            "                return _host;",
            "            } ",
            "        }",
            "",
            "        // Initialize the ads",
            "        public static void Initialize()",
            "        {",
            "            // Add this component if the device is compatible with ads",
            "            if(Settings.IsSupported) Host.AddComponent<AdsInitializer>();",
            "        }",
            "",
            "#if UNITY_EDITOR",
            "        public static bool bannerIsVisible { get; set; } = false;  // Store banner current state",
            "#endif",
            "",
            "        #endregion Singleton",
            "",
            "",
            "        #region Monobehaviour",
            "",
            "        private string gameId;  // This will remain null for unsupported platforms.",
            "",
            "        private void Awake()",
            "        {",
            "",
            "            // Match the gameId to the current platform",
            "#if UNITY_ANDROID",
            "            gameId = Settings.androidGameId;",
            "#elif UNITY_IOS",
            "            gameId = Settings.iOSGameId;",
            "#endif",
            "",
            "            // Initialize the ads if is an compatible platform",
            "#if UNITY_ANDROID || UNITY_IOS",
            "",
            "            // Wait for the delay",
            "            StartCoroutine(WaitAndRun());",
            "",
            "            IEnumerator WaitAndRun()",
            "            {",
            "                yield return new WaitForSecondsRealtime(Settings.initializationDelay);",
            "                Advertisement.Initialize(gameId, Settings.testMode, this);",
            "            }",
            "",
            "            Advertisement.Initialize(gameId, Settings.testMode, this);",
            "#endif",
            "",
            "        }",
            "",
            "        public void OnInitializationComplete()",
            "        {",
            "            // Print log",
            "            if(Settings.showLogs) Debug.Log("+quote+"AdsInitializer.OnInitializationComplete: Unity Ads initialization complete."+quote+");",
            "",
            "            // Raise the event",
            "            onInitializationComplete?.Invoke(this, EventArgs.Empty);",
            "        }",
            "",
            "        public void OnInitializationFailed(UnityAdsInitializationError error, string message)",
            "        {",
            "            // Print log",
            "            if (Settings.showLogs) Debug.LogError($"+quote+"AdsInitializer.OnInitializationFailed: Unity Ads Initialization Failed: {error.ToString()} - {message}"+quote+");",
            "        }",
            "",
            "        #endregion Monobehaviour",
            "",
            "    }",
            "}",
        };
        #endregion Initializer File

        #region Settings File
        private static string SettingsFolderName => "/U.LocalScripts/UAds//";
        private static string SettingsFileName => "UAds.settings.cs";
        private static string[] SettingsFile(AdsSettings settings)
        {
            var file = new List<string>();

            // FI1
            var fi1 = new string[]
            {
                "",
                "namespace LocalScripts.UAds",
                "{",
                "    public static class Settings",
                "    {",
                "",
                "        // AdUnit Class",
                "        public class AddUnit",
                "        {",
                "            public string androidAdUnitId { get; set; }",
                "            public string iOSAdUnitId { get; set; }",
                "            public UnityEngine.Advertisements.BannerPosition possitionIfIsBanner {get; set;}",
                "        }",
                "",
                "",
                "        public static string androidGameId => "+quote+""+settings.androidGameId+""+quote+";",
                "        public static string iOSGameId => "+quote+""+settings.iOSGameId+""+quote+"; ",
                "        public static float initializationDelay => "+settings.initializationDelay+"f;",
                "        public static bool testMode => "+PrintBool(settings.testMode)+";",
                "        public static bool showLogs => "+PrintBool(settings.showLogs)+";",
                "        public static bool InNoSuportedDevicesAdsMustSucced => "+PrintBool(settings.inNoSuportedDevicesAdsMustSucced)+";",
                "",
                "        // Suported platforms must be true",
                "#if UNITY_ANDROID || UNITY_IOS",
                "        public static bool IsSupported => true;",
                "#else",
                "        public static bool IsSupported => false;",
                "#endif",
                "",
                "",
                "",
            };
            foreach (var line in fi1) file.Add(line);

            // FI2
            foreach (var adUnit in settings.adUnits)
            {
                file.Add($"        public static AddUnit {adUnit.className}Settings => new AddUnit");
                file.Add("        {");
                file.Add("            androidAdUnitId = "+quote+ "" + adUnit.androidAdUnitId + "" + quote + ", ");
                file.Add("            iOSAdUnitId = " + quote + "" + adUnit.iOSAdUnitId + "" + quote + ", ");
                if(adUnit.adFormat == AdFormat.Banner) file.Add($"            possitionIfIsBanner = UnityEngine.Advertisements.BannerPosition." + adUnit.bannerPosition + ",");
                file.Add("        };");
                file.Add($"");
            }

            // FI3
            var fi3 = new string[]
            {
                "    }",
                "",
                "}",
            };
            foreach (var line in fi3) file.Add(line);

            // FILE
            return file.ToArray();
        }
        #endregion Settings File

        #region AdUnit File
        private static string AdUnitFolderName => "/U.LocalScripts/UAds/AdUnits/";
        private static string AdUnitFileName(AdUnitSettings adUnit) => $"{adUnit.className}AdUnit.cs";
        private static string[] AdUnitInterstitialOrRewardedFile(AdUnitSettings unitSettings) => new string[]
        {
            "using System.Threading.Tasks;",
            "using UnityEngine;",
            "using UnityEngine.Advertisements;",
            "",
            "namespace LocalScripts.UAds",
            "{",
            "    public class "+unitSettings.className+"AdUnit : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener",
            "    {",
            "",
            "        #region Singleton",
            "",
            "        // This is the behaviour where all functions will be executed.",
            "        private static "+unitSettings.className+"AdUnit _host;",
            "        private static "+unitSettings.className+"AdUnit Host",
            "",
            "        {",
            "            get",
            "            {",
            "                if (_host == null)",
            "                {",
            "                    // Create the host",
            "                    _host = AdsInitializer.Host.AddComponent<"+unitSettings.className+"AdUnit>();",
            "                    DontDestroyOnLoad(_host);",
            "                }",
            "",
            "                return _host;",
            "            }",
            "        }",
            "",
            "",
            "        private static TaskCompletionSource<bool> tks;  // The task of the ad",
            "        public static bool IsReady { get; private set; } = false;  // If add is initialized and loaded",
            "",
            "",
            "#if UNITY_EDITOR",
            "        private static float lastTimeScale = 1;",
            "        //private static bool lastBannerState = false;",
            "#endif",
            "",
            "",
            "        // Load the ad",
            "        public static void Load()",
            "        {",
            "            // Is no ready",
            "            IsReady = false;",
            "",
            "            if (!Settings.IsSupported) return;",
            "",
            "            if (!Advertisement.isInitialized)",
            "            {",
            "                if (Settings.showLogs) Debug.LogError($"+quote+""+unitSettings.className+"AdUnit.Load: Ads are not initialized"+quote+");",
            "                return;",
            "            }",
            "",
            "            if (Advertisement.isShowing)",
            "            {",
            "                if (Settings.showLogs) Debug.LogError($"+quote+""+unitSettings.className+"AdUnit.Load: An ad is showing, wait for it to finish"+quote+");",
            "                return;",
            "            }",
            "",
            "            // Add this component and Load",
            "            Host.LoadAd();",
            "",
            "        }",
            "",
            "",
            "        // Show the ad",
            "        public static Task<bool> Show()",
            "        {",
            "            // If a task exist, finish it",
            "            if (tks != null)",
            "                if (!tks.Task.IsCompleted)",
            "                    tks.SetException(new System.InvalidOperationException("+quote+"A new add was called before this one ended"+quote+"));",
            "",
            "            // Create a new Task",
            "            tks = new TaskCompletionSource<bool>();",
            "",
            "            // If is not suported",
            "            if (!Settings.IsSupported)",
            "            {",
            "                if (Settings.InNoSuportedDevicesAdsMustSucced) tks.SetResult(true);",
            "                else tks.SetException(new System.InvalidOperationException("+quote+"This device is not compatible with ads"+quote+"));",
            "",
            "                return tks.Task;",
            "            }",
            "",
            "            // If not ready , return",
            "            if (!IsReady)",
            "            {",
            "                if (Settings.showLogs) Debug.Log("+quote+""+unitSettings.className+"AdUnit: Ad is no Ready"+quote+");",
            "                tks.SetException(new System.InvalidOperationException("+quote+"Add is no Ready"+quote+"));",
            "                return tks.Task;",
            "            }",
            "",
            "            Host.ShowAd();",
            "",
            "            return tks.Task;",
            "",
            "        }",
            "",
            "        #endregion Singleton",
            "",
            "",
            "        #region MonoBehaviour",
            "",
            "        private string adUnitId;  // This will remain null for unsupported platforms.",
            "",
            "",
            "        private void Awake()",
            "        {",
            "#if UNITY_ANDROID",
            "            adUnitId = Settings.RewardedSettings.androidAdUnitId;",
            "#elif UNITY_IOS",
            "            adUnitId = Settings.RewardedAd.iOSAdUnitId;",
            "#endif",
            "        }",
            "",
            "",
            "        // Load content to the Ad Unit and add callbacks",
            "        private void LoadAd()",
            "        {",
            "            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).",
            "            if (Settings.showLogs) Debug.Log("+quote+""+unitSettings.className+"AdUnit.LoadAd: Loading Ad: "+quote+" + adUnitId);",
            "            Advertisement.Load(adUnitId, this);",
            "        }",
            "",
            "",
            "        // Show the content and add callbacks ",
            "        private void ShowAd()",
            "        {",
            "            // Note that if the ad content wasn't previously loaded, this method will fail",
            "            if (Settings.showLogs) Debug.Log("+quote+""+unitSettings.className+"AdUnit.ShowAd: Showing Ad: "+quote+" + adUnitId);",
            "            Advertisement.Show(adUnitId, this);",
            "        }",
            "",
            "",
            "        #region Load Callbacks",
            "",
            "        public void OnUnityAdsAdLoaded(string placementId)",
            "        {",
            "            if (!placementId.Equals(adUnitId)) return;",
            "",
            "            // When is loaded, set is ready",
            "            IsReady = true;",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnUnityAdsAdLoaded: Ad {placementId} is Ready"+quote+");",
            "        }",
            "",
            "        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)",
            "        {",
            "            if (!placementId.Equals(adUnitId)) return;",
            "",
            "            // When is loaded, set is ready",
            "            IsReady = false;",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnUnityAdsFailedToLoad: Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}"+quote+");",
            "        }",
            "",
            "        #endregion Load Callbacks",
            "",
            "",
            "        #region Show Callbacks",
            "",
            "        public void OnUnityAdsShowClick(string placementId)",
            "        {",
            "            if (!placementId.Equals(adUnitId)) return;",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnUnityAdsShowClick: On ad {placementId}"+quote+");",
            "        }",
            "",
            "        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)",
            "        {",
            "            if (!placementId.Equals(adUnitId)) return;",
            "",
            "",
            "            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED) tks.SetResult(true);",
            "            else tks.SetException(new System.InvalidOperationException($"+quote+"OnUnityAdsShowComplete: {showCompletionState}"+quote+"));",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnUnityAdsShowComplete: {placementId} , {showCompletionState}"+quote+");",
            "",
            "            // Load other add",
            "            Load();",
            "",
            "#if UNITY_EDITOR",
            "            // Resume in Editor, in final devices is not necesary",
            "            Time.timeScale = lastTimeScale;",
            "            //if (lastBannerState) Advertisement.Banner.();",
            "#endif",
            "",
            "        }",
            "",
            "        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)",
            "        {",
            "            if (!placementId.Equals(adUnitId)) return;",
            "",
            "            tks.SetException(new System.InvalidOperationException($"+quote+"OnUnityAdsShowFailure: {error}, {message}"+quote+"));",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnUnityAdsShowFailure: Error showing Ad {adUnitId}: {error.ToString()} - {message}"+quote+");",
            "",
            "            // Load other add",
            "            Load();",
            "",
            "#if UNITY_EDITOR",
            "            // Resume in Editor, in final devices is not necesary",
            "            Time.timeScale = lastTimeScale;",
            "            //if (lastBannerState) Advertisement.Banner.Show();",
            "#endif",
            "",
            "        }",
            "",
            "        public void OnUnityAdsShowStart(string placementId)",
            "        {",
            "            if (!placementId.Equals(adUnitId)) return;",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnUnityAdsShowStart: Ad {placementId} is starting"+quote+");",
            "",
            "#if UNITY_EDITOR",
            "            // Pause In editor, in final devices is not necesary",
            "            lastTimeScale = Time.timeScale;",
            "            Time.timeScale = 0;",
            "",
            "            // Hide banner in editor, in final devices is not necesary",
            "            //lastBannerState = AdsInitializer.bannerIsVisible;",
            "            //Advertisement.Banner.Hide();",
            "#endif",
            "",
            "        }",
            "",
            "",
            "        #endregion Show Callbacks",
            "",
            "",
            "        #endregion MonoBehaviour",
            "",
            "    }",
            "}",
        };
        private static string[] AdUnitBannerFile(AdUnitSettings unitSettings) => new string[]
        {
            "using UnityEngine;",
            "using UnityEngine.Advertisements;",
            "",
            "namespace LocalScripts.UAds",
            "{",
            "    public class "+unitSettings.className+"AdUnit : MonoBehaviour",
            "    {",
            "",
            "        #region Singleton",
            "",
            "        // This is the behaviour where all functions will be executed.",
            "        private static "+unitSettings.className+"AdUnit _host;",
            "        private static "+unitSettings.className+"AdUnit Host",
            "",
            "        {",
            "            get",
            "            {",
            "                if (_host == null)",
            "                {",
            "                    // Create the host",
            "                    _host = AdsInitializer.Host.AddComponent<"+unitSettings.className+"AdUnit>();",
            "                    DontDestroyOnLoad(_host);",
            "                }",
            "",
            "                return _host;",
            "            }",
            "        }",
            "",
            "        public static bool IsReady { get; private set; } = false;  // If add is initialized and loaded",
            "",
            "",
            "        // Load the banner",
            "        public static void Load()",
            "        {",
            "            // Is no ready",
            "            IsReady = false;",
            "",
            "            if (!Settings.IsSupported) return;",
            "",
            "            if (!Advertisement.isInitialized)",
            "            {",
            "                if (Settings.showLogs) Debug.LogError($"+quote+""+unitSettings.className+"AdUnit.Load: Ads are not initialized"+quote+");",
            "                return;",
            "            }",
            "",
            "            // Add this component and Load",
            "            Host.LoadAd();",
            "",
            "        }",
            "",
            "",
            "        // Show the banner",
            "        public static void Show()",
            "        {",
            "            if (!Settings.IsSupported) return;",
            "",
            "            // If not ready , return",
            "            if (!IsReady)",
            "            {",
            "                if (Settings.showLogs) Debug.Log("+quote+""+unitSettings.className+"AdUnit: Ad is no Ready"+quote+");",
            "                return;",
            "            }",
            "",
            "            Host.ShowAd();",
            "",
            "        }",
            "",
            "",
            "        // Hide the banner",
            "        public static void Hide()",
            "        {",
            "            if (!Settings.IsSupported) return;",
            "",
            "            Advertisement.Banner.Hide();",
            "        }",
            "",
            "",
            "        // Change banner position",
            "        public static void ChangePosition(BannerPosition position)",
            "        {",
            "            if (!Settings.IsSupported) return;",
            "",
            "            Advertisement.Banner.SetPosition(position);",
            "        }",
            "",
            "        #endregion Singleton",
            "",
            "",
            "        #region MonoBehaviour",
            "",
            "        private BannerPosition bannerPosition;  // Default banner position",
            "        private string adUnitId;  // This will remain null for unsupported platforms.",
            "",
            "        private void Awake()",
            "        {",
            "            if (!Settings.IsSupported) return;",
            "",
            "#if UNITY_ANDROID",
            "            adUnitId = Settings.BannerSettings.androidAdUnitId;",
            "#elif UNITY_IOS",
            "        adUnitId = Settings."+unitSettings.className+"AdUnit.iOSAdUnitId;",
            "#endif",
            "",
            "            // Set the banner default Position",
            "            bannerPosition = Settings.BannerSettings.possitionIfIsBanner;",
            "        }",
            "",
            "",
            "        // Load content to the Ad Unit and add callbacks",
            "        private void LoadAd()",
            "        {",
            "            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).",
            "            if (Settings.showLogs) Debug.Log("+quote+""+unitSettings.className+"AdUnit.LoadAd: Loading Ad: "+quote+" + adUnitId);",
            "",
            "            // Load the banner with custom props",
            "            Advertisement.Banner.SetPosition(bannerPosition);",
            "            Advertisement.Banner.Load(adUnitId, new BannerLoadOptions",
            "            {",
            "                loadCallback = OnBannerLoaded,",
            "                errorCallback = OnBannerError,",
            "            });",
            "        }",
            "",
            "",
            "        // Show the content and add callbacks ",
            "        private void ShowAd()",
            "        {",
            "            // Note that if the ad content wasn't previously loaded, this method will fail",
            "            if (Settings.showLogs) Debug.Log("+quote+""+unitSettings.className+"AdUnit.ShowAd: Showing Ad: "+quote+" + adUnitId);",
            "",
            "            Advertisement.Banner.Show(adUnitId, new BannerOptions",
            "            {",
            "                clickCallback = OnBannerClicked,",
            "                hideCallback = OnBannerHidden,",
            "                showCallback = OnBannerShown,",
            "            });",
            "        }",
            "",
            "",
            "        #region Load Callbacks",
            "",
            "        private void OnBannerError(string message)",
            "        {",
            "            IsReady = false;",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnBannerError: Error loading banner: {adUnitId}"+quote+");",
            "        }",
            "",
            "        private void OnBannerLoaded()",
            "        {",
            "            IsReady = true;",
            "",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnBannerLoaded: Loaded Banner: {adUnitId}"+quote+");",
            "        }",
            "",
            "        #endregion Load Callbacks",
            "",
            "",
            "        #region Show Callbacks",
            "",
            "        private void OnBannerShown()",
            "        {",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnBannerShown: Shown Banner: {adUnitId}"+quote+");",
            "",
            "",
            "#if UNITY_EDITOR",
            "            AdsInitializer.bannerIsVisible = true;",
            "#endif",
            "",
            "        }",
            "",
            "        private void OnBannerHidden()",
            "        {",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnBannerHidden: Hidden Banner: {adUnitId}"+quote+");",
            "",
            "#if UNITY_EDITOR",
            "            AdsInitializer.bannerIsVisible = false;",
            "#endif",
            "",
            "        }",
            "",
            "        private void OnBannerClicked()",
            "        {",
            "            if (Settings.showLogs) Debug.Log($"+quote+""+unitSettings.className+"AdUnit.OnBannerClicked: Clicked Banner: {adUnitId}"+quote+");",
            "        }",
            "",
            "",
            "        #endregion Show Callbacks",
            "",
            "",
            "        #endregion MonoBehaviour",
            "",
            "    }",
            "}",
        };
        #endregion AdUnit File



        private static string FormatLog(string text) => "UnivesalAds: " + text;


        static AdsSettings volatileFile = new AdsSettings();

        [MenuItem("Universal/Ads/Unity Ads")]
        public static void ShowWindow()
        {
            try
            {
                volatileFile = UE.ReadSerializableResource<AdsSettings>(PersistentFolderName, PersistentFileName, FormatLog);
                if (volatileFile == null) volatileFile = new AdsSettings();
            }
            catch (System.Exception)
            {
                volatileFile = new AdsSettings();
            }
            

            GetWindow<UnityAdsEditorWindow>("Ad Units");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            volatileFile.androidGameId = EditorGUILayout.TextField("Android Game Id: ", volatileFile.androidGameId);
            volatileFile.iOSGameId = EditorGUILayout.TextField("iOS Game Id: ", volatileFile.iOSGameId);
            volatileFile.initializationDelay = EditorGUILayout.FloatField("Initialization Delay: ", volatileFile.initializationDelay);
            volatileFile.testMode = EditorGUILayout.Toggle("Test Mode", volatileFile.testMode);
            volatileFile.showLogs = EditorGUILayout.Toggle("Show Logs", volatileFile.showLogs);
            volatileFile.inNoSuportedDevicesAdsMustSucced = EditorGUILayout.Toggle("Unsuported Succed", volatileFile.inNoSuportedDevicesAdsMustSucced);

            GUILayout.Space(15);
            GUILayout.Label("AdUnits");

            if (GUILayout.Button("Add"))  // Save data to the permanent file
            {
                volatileFile.adUnits.Add(new AdUnitSettings());
            }
            if (GUILayout.Button("Delete"))  // Save data to the permanent file
            {
                if (volatileFile.adUnits.Count <= 0) return;

                volatileFile.adUnits.RemoveAt(volatileFile.adUnits.Count - 1);
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < volatileFile.adUnits.Count; i++)
            {
                GUILayout.Space(10);

                var e = volatileFile.adUnits[i];

                e.className = EditorGUILayout.TextField($"{i}. Class Name:", e.className);
                e.adFormat = (AdFormat)EditorGUILayout.EnumPopup($"{i}. Ad Format:", e.adFormat);
                e.androidAdUnitId = EditorGUILayout.TextField($"{i}. Android AdUnit Id: ", e.androidAdUnitId);
                e.iOSAdUnitId = EditorGUILayout.TextField($"{i}. iOS AdUnit Id: ", e.iOSAdUnitId);
                if(e.adFormat == AdFormat.Banner)
                    e.bannerPosition = (BannerPosition)EditorGUILayout.EnumPopup($"{i}. Banner Position:", e.bannerPosition);
                
            }
            EditorGUI.indentLevel--;


            GUILayout.Space(10);
            GUILayout.Label("Settings");

            if (GUILayout.Button("Save"))  // Save data to the permanent file
            {
                SaveSettingsFile();

                // Compile
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Reset"))  // Delete persistent info
            {
                DeleteSettingsFile();
                volatileFile = new AdsSettings();

                // Compile
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);
            GUILayout.Label("C# classes");

            if (GUILayout.Button("Create"))  // Create the classes with permanent file Info
            {
                // Delete old files
                DeleteAllFiles();

                CreateFile(InitializerFolderName, InitializerFileName, InitializerFile, FormatLog);  // Initializer file
                CreateFile(StartupFolderName, StartupFileName, StartupFile(volatileFile.adUnits), FormatLog);  // Startup file
                CreateFile(SettingsFolderName, SettingsFileName, SettingsFile(volatileFile), FormatLog);  // Settings file

                foreach (var adUnit in volatileFile.adUnits)
                {
                    if (adUnit.adFormat == AdFormat.Banner) CreateFile(AdUnitFolderName, AdUnitFileName(adUnit), AdUnitBannerFile(adUnit), FormatLog);  // Adunit file
                    else CreateFile(AdUnitFolderName, AdUnitFileName(adUnit), AdUnitInterstitialOrRewardedFile(adUnit), FormatLog);  // Adunit file
                }

                // Close the window
                this.Close();

                // Compile
                AssetDatabase.Refresh();

            }

            if (GUILayout.Button("Delete"))  // Delete clases, but not the permanent file info
            {
                DeleteAllFiles();

                // Close the window
                this.Close();

                // Compile
                AssetDatabase.Refresh();
            }

        }


        private void DeleteAllFiles()
        {
            DeleteFile(StartupFolderName, StartupFileName);
            DeleteFile(SettingsFolderName, SettingsFileName);
            DeleteFile(InitializerFolderName, InitializerFileName);
            DeleteFolder(AdUnitFolderName);
        }

        private void DeleteSettingsFile()
        {
            DeleteFile(PersistentFolderName, PersistentFileName);
        }

        private void SaveSettingsFile()
        {
            CreateOverwriteSerializableClass<AdsSettings>(PersistentFolderName, PersistentFileName, volatileFile, FormatLog);
        }

    }


    public enum AdFormat
    {
        Interstitial,
        Rewarded,
        Banner,
    }

    public enum BannerPosition
    {
        BOTTOM_CENTER,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
        CENTER,
        TOP_CENTER,
        TOP_LEFT,
        TOP_RIGHT,
    }

    [System.Serializable]
    public class AdsSettings
    {
        public string androidGameId = "";
        public string iOSGameId = "";
        public float initializationDelay = 0.3f;
        public bool testMode = true;
        public bool showLogs = true;
        public bool inNoSuportedDevicesAdsMustSucced = true;
        public List<AdUnitSettings> adUnits = new List<AdUnitSettings>();
    }

    [System.Serializable]
    public class AdUnitSettings
    {
        public string className = "";
        public AdFormat adFormat = AdFormat.Interstitial;
        public string androidAdUnitId = "";
        public string iOSAdUnitId = "";
        public BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;
    }


}