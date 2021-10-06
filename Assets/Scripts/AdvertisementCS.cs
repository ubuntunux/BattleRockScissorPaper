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
    MainSceneManagerCS _mainSceneManager = null;
    ResultCS _resultScene = null;
    BannerView _bannerView = null;

    InterstitialAd _interstitial = null;
    bool _showInterstitial = false;

    int _rewardScore = 0;
    SkinCardCS _rewardSkinCard = null;
    RewardedAd _rewardedAd = null;
    RewardedAd _rewardedAdNext = null;
    bool _showRewardedAd = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitializeMobileAds(MainSceneManagerCS mainSceneManager)
    {
        _mainSceneManager = mainSceneManager;

        MobileAds.Initialize(initStatus => { });

        _bannerView = CreateBannerView();
        _interstitial = CreateInterstitial();

        RequestBanner();
        RequestInterstitial();
        CreateAndRequestRewardedAd();
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
        bannerView.OnAdFailedToLoad += HandleOnBannerAdFailedToLoad;

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

        interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        interstitialAd.OnAdClosed += HandleOnAdClosed;

        return interstitialAd;
    }

    // Show Ad
    public bool GetShowInterstitial()
    {
        return _showInterstitial;
    }

    public void ShowInterstitial()
    {
        _showInterstitial = true;
    }

    public bool GetShowRewardedAd()
    {
        return _showRewardedAd;
    }

    public void ShowRewardedAd(SkinCardCS rewardedSkinCard, int rewardScore = 0, ResultCS resultScene = null)
    {
        _showRewardedAd = true;
        _resultScene = resultScene;

        if(null == _rewardedAd && null != _rewardedAdNext)
        {
            _rewardScore = rewardScore;
            _rewardSkinCard = rewardedSkinCard;
            _rewardedAd = _rewardedAdNext;
            _rewardedAdNext = null;
        }
        
        CreateAndRequestRewardedAd();
    }

    public void ShowFightRewardedAd(int rewardScore, ResultCS resultScene)
    {
        ShowRewardedAd(null, rewardScore, resultScene);
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

    public void CreateAndRequestRewardedAd()
    {
        if(null == _rewardedAdNext)
        {
            #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-3940256099942544/5224354917";
            #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/1712485313";
            #else
                string adUnitId = "unexpected_platform";
            #endif

            _rewardedAdNext = new RewardedAd(adUnitId);

            _rewardedAdNext.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            _rewardedAdNext.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            _rewardedAdNext.OnUserEarnedReward += HandleUserEarnedReward;
            _rewardedAdNext.OnAdClosed += HandleRewardedAdClosed;

            AdRequest request = new AdRequest.Builder().Build();
            _rewardedAdNext.LoadAd(request);
        }
    }

    // Banner Callback
    public void HandleOnBannerAdFailedToLoad(object sender, EventArgs args)
    {
        RequestBanner();
    }

    // Interstitial Callback
    public void HandleOnAdFailedToLoad(object sender, EventArgs args)
    {
        HandleOnAdClosed(sender, args);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        _showInterstitial = false;
        RequestInterstitial();
    }

    // RewardedAD Callback
    public void HandleRewardedAdFailedToLoad(object sender, EventArgs args)
    {
        HandleRewardedAdClosed(sender, args);
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        HandleRewardedAdClosed(sender, args);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        _showRewardedAd = false;
        _rewardScore = 0;
        _rewardSkinCard = null;
        _rewardedAd = null;
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        if(0 < _rewardScore)
        {
            if(null != _resultScene)
            {
                _resultScene.CallbackAdvertisement(_rewardScore);
            }
            _mainSceneManager.AddScore(_rewardScore);
            _rewardScore = 0;
        }

        if(null != _rewardSkinCard)
        {
            _rewardSkinCard.CallbackPurchaseSkinByAdvertisement();
            _rewardSkinCard = null;
        }

        // string type = args.Type;
        // double amount = args.Amount;
        // Debug.Log("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);
    }

    // Update is called once per frame
    public void Update()
    {
        if(_showInterstitial)
        {
            if(_interstitial.IsLoaded())
            {
                _interstitial.Show();
            }
        }

        if(_showRewardedAd)
        {
            if(null != _rewardedAd && _rewardedAd.IsLoaded())
            {
                _rewardedAd.Show();
            }
        }
    }
}
