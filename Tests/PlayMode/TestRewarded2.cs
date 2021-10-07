using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U.Universal.Ads;

namespace U.Universal.Ads.Test
{
    public class TestRewarded2 : Intertitial<TestRewarded2>
    {
        protected override string gameId_Android => TestEnv.Ads.gameId_Android;
        protected override string gameId_IOS => TestEnv.Ads.gameId_IOS;


        protected override string adUnitId_Android => TestEnv.Ads.AdUnit.Rewarded_Android.ToString();
        protected override string adUnitId_IOS => TestEnv.Ads.AdUnit.Rewarded_iOS.ToString();


        protected override bool testMode => TestEnv.Ads.testMode;
        protected override bool noAdsMode => TestEnv.Ads.noAdsMode;



        protected override void OnReady()
        {
            Debug.Log("OnReady: " + name);
        }

        protected override void OnDidFail()
        {
            Debug.Log("OnDidFail: " + name);
        }

        protected override void OnDidFinish()
        {
            Debug.Log("OnDidFinish: " + name);
        }

        protected override void OnDidSkip()
        {
            Debug.Log("OnDidSkip: " + name);
        }

        protected override void OnDidStart()
        {
            Debug.Log("OnDidStart: " + name);
        }
    }
}
