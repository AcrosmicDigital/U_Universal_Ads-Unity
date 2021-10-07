using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U.Universal.Ads;
using UnityEngine.Advertisements;

namespace U.Universal.Ads.Test
{
    public class TestBanner : Banner<TestBanner>
    {
        protected override string gameId_Android => TestEnv.Ads.gameId_Android;
        protected override string gameId_IOS => TestEnv.Ads.gameId_IOS;


        protected override string adUnitId_Android => TestEnv.Ads.AdUnit.Banner_Android.ToString();
        protected override string adUnitId_IOS => TestEnv.Ads.AdUnit.Banner_iOS.ToString();


        protected override bool testMode => TestEnv.Ads.testMode;
        protected override bool noAdsMode => TestEnv.Ads.noAdsMode;

        protected override BannerPosition defaultPosition => BannerPosition.TOP_CENTER;

        protected override void OnClick()
        {
            Debug.Log("OnClick: " + name);
        }

        protected override void OnHide()
        {
            Debug.Log("OnHide: " + name);
        }

        protected override void OnLoad()
        {
            Debug.Log("OnLoad: " + name);
        }

        protected override void OnShow()
        {
            Debug.Log("OnShow: " + name);
        }

        protected override void OnError(string message)
        {
            Debug.Log("OnError: " + name + ", Error: " + message);
        }
    }
}
