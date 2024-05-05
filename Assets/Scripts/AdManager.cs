using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UIElements;

//This script is intended to act as a persistent ad manager object so that the ad banner can be affected
// in multiple scenes.
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; } //Create persistent admanager

    private BannerView _bannerView; //google ad banner
    private bool adEnabled = true; // Initially set to true

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Denote urrent gameObject as persistent game object so this script
        }                                  // works across multiple scenes, not just Introduction
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize the ad when starting the game, create the ad banner and load ad from Google
    public void InitializeAd()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            CreateBannerView();
            LoadAd();
        });
    }

    // These ad units are configured to always serve test ads.
    // saved as variable to be input into the script later
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";//ca-app-pub-3940256099942544/6300978111
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string _adUnitId = "unused";
#endif

    //Assigned to a button in main scene, this function allows the user to toggle the ad banner 
    // off and back on as they wish. This function is only available in the main scene through the
    // use of the 'X' ad button at the bottom of the screen
    public void ToggleAdVisibility()
    {
        adEnabled = !adEnabled;
        if (adEnabled)
        {
            _bannerView.Show();
        }
        else
        {
            _bannerView.Hide();
        }
    }

    //Create the banner view at the bottom of the screen. First called when the game starts up
    // at the introduction scene
    private void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at bottom of the screen
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

        ListenToAdEvents();
    }

    //Calls for an ad from google. Needs the ad banner to be set up first otherwise it can't be displayed
    private void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    
    // Event listeners for banner view events
    private void ListenToAdEvents()
    {
        // Add your event listeners here
        // Event listeners for banner view events
        //On successful ad load
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // If ad load is unsuccessful
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // If ad banner gives some currency
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // For ad impressions 
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // When the ad is clicked
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // When the banner content is opened
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // When the banner content is closed
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    //Destroy the ad banner at the bottom of the screen.
    //Try to avoid using repeatedly, can cause issues. 
    //Use Hide() instead if needed to be used repeatedly
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;

        }
    }

}
