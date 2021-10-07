using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U.Universal.Ads.Test
{
    public class TestEnv
    {
        public class Ads
        {
            public static readonly string gameId_Android = "4142127";  // Tha game Id For Android 

            public static readonly string gameId_IOS = "4142126";  // The game Id for Ios

            public static readonly bool testMode = true;  // If testmode for ads is enabled

            public static readonly bool noAdsMode = false;

            public enum AdUnit  // The id of the adds
            {
                Interstitial_Android,
                Interstitial_iOS,
                Banner_Android,
                Banner_iOS,
                Rewarded_Android,
                Rewarded_iOS,
                // ...
            }
        }
    }
}

