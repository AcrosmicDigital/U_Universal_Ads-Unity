using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;

namespace U.Universal.Ads
{
    internal class AdListener : IUnityAdsListener
    {

        // List
        private static List<string> showingList = new List<string>();  // List of adds that are showing, here because Intetitial class is generic

        internal static void AddToShowingList(string name)
        {
            if (String.IsNullOrEmpty(name))
                return;

            showingList.Add(name);
        }

        private static List<AdListener> instancesList = new List<AdListener>();

        internal static void AddToInstancesList(AdListener instance)
        {
            if (instance == null)
                return;

            instancesList.Add(instance);
        }

        internal static void RemoveFromInstancesList(AdListener instance)
        {
            if (instance == null)
                return;

            instancesList.Remove(instance);
        }

        internal static bool ExistInInstancesList(AdListener instance)
        {
            if (instance == null)
                return false;

            return instancesList.Contains(instance);
        }

        internal static AdListener[] GetInstances()
        {
            if (instancesList == null)
                return new AdListener[] { };

            return instancesList.Where(i => i != null).ToArray();

        }

        // /List

        internal string adUnitId { get; private set; } = "";
        private string name = "";
        private Action<string> onUnityAdsDidError;
        private Action onUnityAdsDidFinish;
        private Action onUnityAdsDidSkip;
        private Action onUnityAdsDidFail;
        private Action onUnityAdsDidStart;
        internal Action onUnityAdsReady;

        public AdListener(
            string adUnitId,
            string name,
            Action<string> onUnityAdsDidError,
            Action onUnityAdsDidFinish,
            Action onUnityAdsDidSkip,
            Action onUnityAdsDidFail,
            Action onUnityAdsDidStart,
            Action onUnityAdsReady
            )
        {
            this.adUnitId = adUnitId;
            this.name = name;
            this.onUnityAdsDidError = onUnityAdsDidError;
            this.onUnityAdsDidFinish = onUnityAdsDidFinish;
            this.onUnityAdsDidSkip = onUnityAdsDidSkip;
            this.onUnityAdsDidFail = onUnityAdsDidFail;
            this.onUnityAdsDidStart = onUnityAdsDidStart;
            this.onUnityAdsReady = onUnityAdsReady;
        }

        public void OnUnityAdsDidError(string message)
        {
            // Debug.LogError("UnityAds: Did error: " + message);
            onUnityAdsDidError?.Invoke(message);
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            // Only if is the placement id that have this class
            if (placementId != adUnitId)
                return;

            // Only if the name of the parent intettitla class is is the showing list, that means that is waiting for this ad to be showed
            if (!showingList.Contains(name))
                return;

            // Execute the delegate
            try
            {
                //Debug.Log("Ad Finnish: " + placementId + " with code: " + showResult + " with name: " + name);
                if (showResult == ShowResult.Finished)
                {
                    onUnityAdsDidFinish?.Invoke();
                }
                else if (showResult == ShowResult.Skipped)
                {
                    onUnityAdsDidSkip?.Invoke();
                }
                else if (showResult == ShowResult.Failed)
                {
                    onUnityAdsDidFail?.Invoke();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Intertitial: Error in callback functions, " + e);
            }

            // Remove the name from the list because is executed
            showingList.Remove(name);


        }

        public void OnUnityAdsDidStart(string placementId)
        {
            // Only if is the placement id that have this class
            if (placementId != adUnitId)
                return;

            // Only if the name of the parent intettitla class is is the showing list, that means that is waiting for this ad to be showed
            if (!showingList.Contains(name))
                return;

            // Invoke the delegate
            onUnityAdsDidStart?.Invoke();
        }

        public void OnUnityAdsReady(string placementId)
        {
            // Only if is the placement id that have this class
            if (placementId != adUnitId)
                return;

            onUnityAdsReady?.Invoke();
        }
    }
}
