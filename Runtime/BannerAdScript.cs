using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using System;

namespace U.Universal.Ads
{
    internal class BannerAdScript : MonoBehaviour
    {
        public string gameId = "";
        public string adUnitId = "";
        public bool testMode = true;
        public Action hideCallback;
        public Action showCallback;
        public Action<string> errorCallback;
        public Action clickCallback;
        public Action loadCallback;
        public BannerPosition defaultPosition = BannerPosition.BOTTOM_CENTER;

        void Start()
        {
            if(!Advertisement.isInitialized)
                Advertisement.Initialize(gameId, testMode);
            StartCoroutine(ShowBannerWhenInitialized());
        }

        IEnumerator ShowBannerWhenInitialized()
        {
            
            // Wait for initialize the Ads
            while (!Advertisement.isInitialized)
            {
                yield return new WaitForSeconds(0.2f);
            }
            
            // Set the positiion of the banner
            Advertisement.Banner.SetPosition(defaultPosition);

            // Load the banner
            Advertisement.Banner.Load(adUnitId, new BannerLoadOptions 
            {
                errorCallback = (e) => { this.errorCallback?.Invoke(e); },
                loadCallback = () => { this.loadCallback?.Invoke(); },
            });

            // Wait while is Loading 
            while (!Advertisement.Banner.isLoaded)
            {
                yield return new WaitForSeconds(0.2f);
            }

            // Show the banner and set the position
            Advertisement.Banner.Show(adUnitId, new BannerOptions 
            {
                hideCallback = () => { this.hideCallback?.Invoke(); },
                showCallback = () => { this.showCallback?.Invoke(); },
                clickCallback = () => { this.clickCallback?.Invoke(); },
            });


            // Destroy the GameObject when is Loaded
            Destroy(gameObject);
        }
    }
}
