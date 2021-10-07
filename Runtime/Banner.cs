using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace U.Universal.Ads
{
    public abstract class Banner<TBanner> where TBanner : Banner<TBanner>, new()
    {

        public static string name => new TBanner().GetType().Name;



        protected abstract string gameId_Android { get; }
        protected abstract string gameId_IOS { get; }

        protected abstract string adUnitId_Android { get; }
        protected abstract string adUnitId_IOS { get; }

        protected abstract bool testMode { get; }
        protected abstract bool noAdsMode { get; }

        protected abstract BannerPosition defaultPosition { get; }


        private static bool isHided = true;

        public static void Load()
        {
#if UNITY_ANDROID || UNITY_IOS

            // Create a new instance
            var instance = new TBanner();

            // If no ads just execute the function of load
            if (instance.noAdsMode)
            {
                instance.OnLoad();
                instance.OnShow();
                instance.OnHide();
                return;
            }

            // Show and Hide
            Show();
            Hide();
#else
            // Create a new instance
            var instance = new TBanner();

            // If no ads just execute the function of load
            instance.OnLoad();
            instance.OnShow();
            instance.OnHide();
#endif
        }

        public static void Show()
        {

#if UNITY_ANDROID || UNITY_IOS

            // Create a new instance
            var instance = new TBanner();

            // If no ads just execute the function of show
            if (instance.noAdsMode)
            {
                instance.OnLoad();
                instance.OnShow();
                return;
            }

            // Destroy other instances of banners running to avoid errors
            var activeList = Resources.FindObjectsOfTypeAll<BannerAdScript>();
            if(activeList != null)
            {
                foreach(var active in activeList)
                {
                    if (active == null)
                        continue;

                    UnityEngine.Object.Destroy(active.gameObject);
                }
            }

            // Create a gameObject
            var host = new GameObject("Host-Banner");

            // Put it on dont destroyon load
            UnityEngine.Object.DontDestroyOnLoad(host);

            // Add the behaviour
            var c = host.AddComponent<BannerAdScript>();

            // Set the properties to the behaviour
            c.testMode = instance.testMode;
            c.defaultPosition = instance.defaultPosition;
            c.errorCallback = instance.OnError;
            c.loadCallback = instance.OnLoad;
            c.clickCallback = instance.OnClick;
            c.showCallback = () => 
            {
                // When banner show but should be hidded, hide the banner
                if (isHided)
                {
                    Hide();
                }

                // Call the callback
                instance.OnShow();
            };
            c.hideCallback = () => 
            {
                // When the banner hide, but must be show, show the banner
                if (!isHided)
                {
                    Show();
                }

                // Call the callback
                instance.OnHide();
            };

            // Set platform specific properties
#if UNITY_ANDROID
            c.gameId = instance.gameId_Android;
            c.adUnitId = instance.adUnitId_Android;
#elif UNITY_IOS
            c.gameId = instance.gameId_IOS;
            c.adUnitId = instance.adUnitId_IOS;
#endif

            // Is not hidded
            isHided = false;

#else

            // Create a new instance
            var instance = new TBanner();

            // If no ads just execute the function of show
            instance.OnLoad();
            instance.OnShow();
            return;

#endif  // Of the function

        }


        public static void Hide()
        {
#if UNITY_ANDROID || UNITY_IOS

            // Create a new instance
            var instance = new TBanner();

            // If no ads just execute the function of show
            if (instance.noAdsMode && isHided)
            {
                instance.OnHide();
                return;
            }

            Advertisement.Banner.Hide();
            isHided = true;

#else

            // Just to use the var
            if (!isHided)
                isHided = true;

            // Create a new instance
            var instance = new TBanner();
            instance.OnHide();


#endif  // Of the function

        }


        public static void SetPosition(BannerPosition bannerPosition)
        {
#if UNITY_ANDROID || UNITY_IOS
            Advertisement.Banner.SetPosition(bannerPosition);
#endif  // Of the function
        }


        public static bool IsHided
        {
            get
            {
#if UNITY_ANDROID || UNITY_IOS
                return isHided;
#else
                return true;
#endif  // Of the function
            }
        }


        protected virtual void OnError(string message) { }
        protected virtual void OnLoad() { }
        protected virtual void OnHide() { }
        protected virtual void OnShow() { }
        protected virtual void OnClick() { }

    }
}

