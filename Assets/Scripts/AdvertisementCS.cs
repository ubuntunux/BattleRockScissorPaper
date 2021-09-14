using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

public class AdvertisementCS : MonoBehaviour
{
    BannerView _bannerView = null;
    InterstitialAd _interstitial = null;
    RewardedAd _rewardedAd = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitializeMobileAds()
    {
        MobileAds.Initialize(initStatus => { });

        _bannerView = CreateBannerView();
        _interstitial = CreateInterstitial();
        _rewardedAd = CreateRewardedAd();

        // TEST CODE
        RequestBanner();
    }

    public BannerView CreateBannerView()
    {
        // Create a 320x50 banner at the top of the screen.
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        return bannerView;
    }

    public InterstitialAd CreateInterstitial()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        InterstitialAd interstitialAd = new InterstitialAd(adUnitId);
        return interstitialAd;
    }

    public RewardedAd CreateRewardedAd()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        return rewardedAd;
    }

    // Request
    public void RequestBanner()
    {
        AdRequest request = new AdRequest.Builder().Build();
        _bannerView.LoadAd(request);
    }

    public void RequestInterstitial()
    {
        AdRequest request = new AdRequest.Builder().Build();
        _interstitial.LoadAd(request);
    }

    public void RequestRewardedAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        _rewardedAd.LoadAd(request);
    }

    // RewardedAD Callback
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToLoad event received.");
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToShow event received with message: " + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);
    }

    // Update is called once per frame
    public void Update()
    {
        if(_interstitial.IsLoaded())
        {
            _interstitial.Show();
        }

        if(_rewardedAd.IsLoaded())
        {
            _rewardedAd.Show();
        }
    }
}
