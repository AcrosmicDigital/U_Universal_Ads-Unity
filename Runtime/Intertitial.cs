using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;

namespace U.Universal.Ads
{
    public abstract class Intertitial<TInterstitial> where TInterstitial : Intertitial<TInterstitial>, new()
    {

        public static string name => new TInterstitial().GetType().Name;



        protected abstract string gameId_Android { get; }
        protected abstract string gameId_IOS { get; }

        protected abstract string adUnitId_Android { get; }
        protected abstract string adUnitId_IOS { get; }

        protected abstract bool testMode { get; }

        protected abstract bool noAdsMode { get; }


        //private static bool isStarted = false;
        private static AdListener listenerInstance;
        private static TaskCompletionSource<ShowResult> tks; // The task of the completation


        public static void Initialize()
        {

#if UNITY_ANDROID || UNITY_IOS

            // Remove the listener if exist
            RemoveListener();

            var instance = new TInterstitial();

            // Add the listener
            var listener = new AdListener(
#if UNITY_ANDROID
                instance.adUnitId_Android,
#elif UNITY_IOS
                instance.adUnitId_IOS,
#endif
                name,
                onUnityAdsDidError: (s) =>
                {
                    if(tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Failed);

                    instance.OnDidError(s);
                },
                onUnityAdsDidFinish: () =>
                {
                    if (tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Finished);

                    instance.OnDidFinish();
                },
                onUnityAdsDidSkip: () =>
                {
                    if (tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Skipped);

                    instance.OnDidSkip();
                },
                onUnityAdsDidFail: () =>
                {
                    if (tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Failed);

                    instance.OnDidFail();
                },
                instance.OnDidStart,
                instance.OnReady
                );

            listenerInstance = listener;
            AdListener.AddToInstancesList(listener);  // Add the llistener to the internal list

            // If no ads mode just execute is ready and return
            if (instance.noAdsMode)
            {
                //Debug.Log("Listener list is " + listenerInstance.GetHashCode() + " Count: " + listenerInstance);
                try { instance.OnReady(); } catch (Exception e) { Debug.LogError("Ads: Error in OnReady of " + name + ", " + e); }
                return;
            }
            Advertisement.AddListener(listener);  // Add the listener to the real list

            // Initialize the ads
            if (!Advertisement.isInitialized)
#if UNITY_ANDROID
                Advertisement.Initialize(instance.gameId_Android, instance.testMode);
#elif UNITY_IOS
                Advertisement.Initialize(instance.gameId_IOS, instance.testMode);
#endif

#else  // If no supported platform

            // Remove the listener if exist
            RemoveListener();

            var instance = new TInterstitial();

            // Add the listener
            var listener = new AdListener(
                "", // No Android or IOS id
                name,
                onUnityAdsDidError: (s) =>
                {
                    if(tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Failed);

                    instance.OnDidError(s);
                },
                onUnityAdsDidFinish: () =>
                {
                    if(tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Finished);

                    instance.OnDidFinish();
                },
                onUnityAdsDidSkip: () =>
                {
                    if(tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Skipped);

                    instance.OnDidSkip();
                },
                onUnityAdsDidFail: () =>
                {
                    if(tks != null)
                        if (!tks.Task.IsCompleted)
                            tks.SetResult(ShowResult.Failed);

                    instance.OnDidFail();
                },
                instance.OnDidStart,
                instance.OnReady
                );

            listenerInstance = listener;
            AdListener.AddToInstancesList(listener);  // Add the llistener to the internal list

            // Execute is ready
            try { instance.OnReady(); } catch (Exception e) { Debug.LogError("Ads: Error in OnReady of " + name + ", " + e); }
            
#endif

        }

        public static void RemoveListener()
        {
#if UNITY_ANDROID || UNITY_IOS

            // If listener exist remove it
            if (listenerInstance != null)
            {
                Advertisement.RemoveListener(listenerInstance);
                AdListener.RemoveFromInstancesList(listenerInstance);
                listenerInstance = null;
            }

#else

            // If listener exist remove it
            if (listenerInstance != null)
            {
                AdListener.RemoveFromInstancesList(listenerInstance);
                listenerInstance = null;
            }

#endif  // Of the function
        }

        public static Task<ShowResult> Show()
        {
#if UNITY_ANDROID || UNITY_IOS

            // If you are trying to call a new show when last is not completed
            if (tks != null)
                if (!tks.Task.IsCompleted)
                    tks.SetResult(ShowResult.Failed);
            // Create a new task
            tks = new TaskCompletionSource<ShowResult>();

            // Create a instance
            var instance = new TInterstitial();

            // Ckeck if initialized and try toinitialize
            if (listenerInstance == null)
                Initialize();

            // If no ads mode is enabled
            if (instance.noAdsMode)
            {
                // If listener exist execute
                if (listenerInstance != null)
                {
                    try { instance.OnDidStart(); } catch (Exception e) { Debug.LogError("Ads: Error in OnDidStart of " + name + ", " + e); }
                    try { instance.OnDidFinish(); } catch (Exception e) { Debug.LogError("Ads: Error in OnDidFinish of " + name + ", " + e); }
                }

                // Execute the onready for existing listeners
                foreach (var listener in AdListener.GetInstances())
                {
                    try { listener.onUnityAdsReady.Invoke(); } catch (Exception e) { Debug.LogError("Ads: Error in onUnityAdsReady of " + name + ", " + e); }
                }

                // Complete the task succesfully
                if (!tks.Task.IsCompleted)
                    tks.SetResult(ShowResult.Finished);
                return tks.Task;
            }

            // Ckeck if initialized and if listener exist
            if (!Advertisement.isInitialized || listenerInstance == null)
            {
                Debug.LogError("Ads are not initialized, Please try again later!");
                
                // Complete the task failed
                if (!tks.Task.IsCompleted)
                    tks.SetResult(ShowResult.Failed);
                return tks.Task;
            }

            // Check if listener is not subscribed in the list
            if (!AdListener.ExistInInstancesList(listenerInstance))
            {
                Debug.LogError("Ads are not initialized, Please try again later!");

                // Complete the task failed
                if (!tks.Task.IsCompleted)
                    tks.SetResult(ShowResult.Failed);
                return tks.Task;
            }


#if UNITY_IOS
            // If is realy show
            if (Advertisement.IsReady(instance.adUnitId_IOS))
            {
                AdListener.AddToShowingList(name); // Add to the list of showing ads
                Advertisement.Show(instance.adUnitId_IOS);
            }
#elif UNITY_ANDROID
            // If is realy show
            if (Advertisement.IsReady(instance.adUnitId_Android))
            {
                AdListener.AddToShowingList(name); // Add to the list of showing ads
                Advertisement.Show(instance.adUnitId_Android);
            }
#endif
            else
            {
                // Complete the task failed
                if (!tks.Task.IsCompleted)
                    tks.SetResult(ShowResult.Failed);

                Debug.LogError("Interstitial ad not ready at the moment! Please try again later!");
            }

            return tks.Task;

#else  // If no android or ios always return a completed task with isFinished

            // Create a instance
            var instance = new TInterstitial();

            // Ckeck if initialized and try toinitialize
            if (listenerInstance == null)
                Initialize();

            // If listener exist execute
            if (listenerInstance != null)
            {
                try { instance.OnDidStart(); } catch (Exception e) { Debug.LogError("Ads: Error in OnDidStart of " + name + ", " + e); }
                try { instance.OnDidFinish(); } catch (Exception e) { Debug.LogError("Ads: Error in OnDidFinish of " + name + ", " + e); }
            }

            // Execute the onready for existing listeners
            foreach (var listener in AdListener.GetInstances())
            {
                try { listener.onUnityAdsReady.Invoke(); } catch (Exception e) { Debug.LogError("Ads: Error in onUnityAdsReady of " + name + ", " + e); }
            }

            // Return a succesfully task
            return Task.FromResult(ShowResult.Finished);

#endif  // Of the function

        }

        public static bool IsReady { 
            get 
            {
#if UNITY_ANDROID || UNITY_IOS
                var instance = new TInterstitial();

                // If no ads mode is enabled
                if (instance.noAdsMode)
                    return true;

                if (!Advertisement.isInitialized)
                    return false;

#if UNITY_IOS
                return Advertisement.IsReady(instance.adUnitId_IOS);
#elif UNITY_ANDROID
                return Advertisement.IsReady(instance.adUnitId_Android);
#endif

#else  // Of the function
                return listenerInstance != null;
#endif  // Of the function
            }
        }


        protected virtual void OnDidError(string message) { }  // To all ads of any type
        protected virtual void OnDidFinish() { }  // name specific
        protected virtual void OnDidSkip() { }  // Name specific
        protected virtual void OnDidFail() { }  // Name specific
        protected virtual void OnDidStart() { }  // Name specific
        protected virtual void OnReady() { }  // To ads with the same UnitId, executed after an ad is showed

    }
}
