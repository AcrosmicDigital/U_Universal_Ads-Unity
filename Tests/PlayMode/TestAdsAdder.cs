using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U.Universal.Ads.Test;

public class TestAdsAdder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestIntertitial1.Initialize();
        TestIntertitial2.Initialize();
        TestRewarded1.Initialize();
        TestRewarded2.Initialize();
        TestBanner.Load();
    }


    private void OnDestroy()
    {
        TestIntertitial1.RemoveListener();
        TestIntertitial2.RemoveListener();
        TestRewarded1.RemoveListener();
        TestRewarded2.RemoveListener();
        TestBanner.Hide();
    }

    public void ShowInterstitial1()
    {
        if (TestIntertitial1.IsReady)
            TestIntertitial1.Show();
        else
            Debug.Log("Test intertitial1 is no ready");
    }

    public void ShowInterstitial2()
    {
        if (TestIntertitial2.IsReady)
            TestIntertitial2.Show();
        else
            Debug.Log("Test intertitial2 is no ready");
    }

    public void ShowTestRewarded1()
    {
        if (TestRewarded1.IsReady)
            TestRewarded1.Show();
        else
            Debug.Log("Test TestRewarded1 is no ready");
    }

    public void ShowTestRewarded2()
    {
        if (TestRewarded2.IsReady)
            TestRewarded2.Show();
        else
            Debug.Log("Test TestRewarded2 is no ready");
    }


    public void ShowBanner()
    {
        TestBanner.Show();
    }

    public void HideBanner()
    {
        TestBanner.Hide();
    }


    public async void ShowRewarded1Await()
    {
        // Show the add
        UnityEngine.Advertisements.ShowResult result;
        try
        {
            Debug.Log("Awiting for: " + TestRewarded1.name);
            result = await TestRewarded1.Show();
        }
        catch (System.Exception)
        {
            result = UnityEngine.Advertisements.ShowResult.Failed;
        }

        Debug.Log("Result: " + result);
    }

    public async void ShowRewarded2Await()
    {
        // Show the add
        UnityEngine.Advertisements.ShowResult result;
        try
        {
            Debug.Log("Awiting for: " + TestRewarded2.name);
            result = await TestRewarded2.Show();
        }
        catch (System.Exception)
        {
            result = UnityEngine.Advertisements.ShowResult.Failed;
        }

        // Choose action based in result
        Debug.Log("Result: " + result);
    }

    public async void ShowInterstitial1Await()
    {
        // Show the add
        UnityEngine.Advertisements.ShowResult result;
        try
        {
            Debug.Log("Awiting for: " + TestIntertitial1.name);
            result = await TestIntertitial1.Show();
        }
        catch (System.Exception)
        {
            result = UnityEngine.Advertisements.ShowResult.Failed;
        }

        Debug.Log("Result: " + result);
    }

    public async void ShowInterstitial2Await()
    {
        // Show the add
        UnityEngine.Advertisements.ShowResult result;
        try
        {
            Debug.Log("Awiting for: " + TestIntertitial2.name);
            result = await TestIntertitial2.Show();
        }
        catch (System.Exception)
        {
            result = UnityEngine.Advertisements.ShowResult.Failed;
        }

        Debug.Log("Result: " + result);
    }
}
