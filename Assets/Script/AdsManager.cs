using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.Localization.Components;
using System.Globalization;
using GoogleMobileAds.Ump.Api;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using Firebase.Extensions;

/* 
combo_classic 0.99
combo_plus 1.99
combo_premium 2.99
combo_deluxe 1.99
combo_special 2.99
removead 1.99
hiden_legendary


compass_classic 0.99
compass_plus 1.99
compass_premium 2.99
special_classic 3.99
special_plus 5.99
weekly_challenge 3.99
*/


// [System.Serializable]
// public class ExchangeRateResponse
// {
//     public Rates rates;
// }

// [System.Serializable]
// public class Rates
// {
//     public float AED, AFN, ALL, AMD, ANG, AOA, ARS, AUD, AWG, AZN, BAM, BBD, BDT, BGN, BHD, BIF, 
//         BMD, BND, BOB, BRL, BSD, BTN, BWP, BYN, BZD, CAD, CDF, CHF, CLP, CNY, COP, CRC, CUP, CVE, 
//         CZK, DJF, DKK, DOP, DZD, EGP, ERN, ETB, EUR, FJD, FKP, FOK, GBP, GEL, GGP, GHS, GIP, GMD, 
//         GNF, GTQ, GYD, HKD, HNL, HRK, HTG, HUF, IDR, ILS, IMP, INR, IQD, IRR, ISK, JEP, JMD, JOD, 
//         JPY, KES, KGS, KHR, KID, KMF, KRW, KWD, KYD, KZT, LAK, LBP, LKR, LRD, LSL, LYD, MAD, MDL, 
//         MGA, MKD, MMK, MNT, MOP, MRU, MUR, MVR, MWK, MXN, MYR, MZN, NAD, NGN, NIO, NOK, NPR, NZD, 
//         OMR, PAB, PEN, PGK, PHP, PKR, PLN, PYG, QAR, RON, RSD, RUB, RWF, SAR, SBD, SCR, SDG, SEK, 
//         SGD, SHP, SLL, SOS, SRD, SSP, STN, SYP, SZL, THB, TJS, TMT, TND, TOP, TRY, TTD, TVD, TWD, 
//         TZS, UAH, UGX, USD, UYU, UZS, VES, VND, VUV, WST, XAF, XCD, XOF, XPF, YER, ZAR, ZMW, ZWL;
    
// }

[Serializable]
public class ConsumableItem{
    public string Name;
    public string Id;
    public string description;
    public float price;
}

[Serializable]
public class NonConsumableItem{
    public string Name;
    public string Id;
    public string description;
    public float price;
}

[Serializable]
public class SubscriptionItem{
    public string Name;
    public string Id;
    public string description;
    public float price;
    public int timeDuration; // in Day
} 

public class AdsManager : MonoBehaviour
{

// #if UNITY_ANDROID
    // private string _appId = "ca-app-pub-5342144217301971~4597278331";

    private string _adOpenId = "ca-app-pub-5342144217301971/7131603028";
    private string _adBannerId = "ca-app-pub-5342144217301971/3699129638";
    private string _adRewardId = "ca-app-pub-5342144217301971/7207624641";
    private string _adInterstitialId = "ca-app-pub-5342144217301971/8520706314";
    private string NativeAdUnitId = "ca-app-pub-5342144217301971/3399746738";
// #elif UNITY_IPHONE
//     private string _appId = "ca-app-pub-5342144217301971~9244780337";

//     private string _adOpenId = "ca-app-pub-5342144217301971/7660235925";
//     private string _adBannerId = "ca-app-pub-5342144217301971/1561731436";
//     private string _adRewardId = "ca-app-pub-5342144217301971/1286399269";
//     private string _adInterstitialId = "ca-app-pub-5342144217301971/4474039075";
//     private string NativeAdUnitId = "ca-app-pub-5342144217301971/9248649768";
// #endif
    public static AdsManager Instance;

    public GameObject InterAds;
    public ChallengeMode game;
    public Loading loading;

    private NativeAd nativeAdSetting;
    public RawImage AdIconTextureSetting;
    public TextMeshProUGUI AdHeadlineSetting;
    public TextMeshProUGUI AdDescriptionSetting;
    public GameObject AdLoadedSetting;
    public GameObject AdLoadingSetting;
    public GameObject AdsMessage;
    public AudioManager audioManager;

    private NativeAd nativeAdNewArea;
    public RawImage AdIconTextureNewArea;
    public TextMeshProUGUI AdHeadlineNewArea;
    public TextMeshProUGUI AdDescriptionNewArea;
    public GameObject AdLoadedNewArea;
    public GameObject AdLoadingNewArea;

    private NativeAd nativeAdSmallShop;
    public RawImage AdIconTextureSmallShop;
    public TextMeshProUGUI AdHeadlineSmallShop;
    public TextMeshProUGUI AdDescriptionSmallShop;
    public GameObject AdLoadedSmallShop;
    public GameObject AdLoadingSmallShop;
    public GameObject Thanks;
    public GameObject CountDown;
    public GameObject BtnSub;
    public Home home;
    public VideoMoreHint videoMoreHint;


    private BannerView _bannerView;
    private AppOpenAd _appOpenAd;
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

    private int countOpenAd = 0;
    private bool stopCountDownInter = false;
    private bool pauseCountDownInter = false;
    private bool allowVibration;
    public ButtonControl buttonControl;

    ConsentForm _consentForm;

    public SaveDataJson saveDataJson;
    public SmallShop SmallShop;
    public SmallCompassShop smallCompassShop;
    public OpenedGift openedGift;
    public RandomSpin randomSpin;
    // public LuckyReward luckyReward;
    // public GameObject hintIconInGame;


    private string nameReward;

    private bool isFocus = false;
    private bool isProcessing = false;
    private bool showOpenAd = false;
    private bool loadedOpenAd = false;
    private bool stopShowOpenAd = false;
    private bool resetTimeInter = false;
    private bool allowToShowShopBannerHint = true;
    private bool allowToShowShopBanner = true;

    [Header("In App Purchase")]
    // StoreController storeController;
    StoreController storeController;
    public ConsumableItem cItemX5;
    public ConsumableItem cItemX15;
    public ConsumableItem cItemX30;

    public ConsumableItem CompassClassic;
    public ConsumableItem CompassPlus;
    public ConsumableItem CompassPremium;
    public ConsumableItem SpecialClassic;
    public ConsumableItem SpecialPlus;
    public ConsumableItem WeeklyChallenge;

    public NonConsumableItem SpecialMap;
    public NonConsumableItem SpecialMapSale;

    public ConsumableItem cDeluxe;
    public ConsumableItem cSpecial;

    public ConsumableItem CoinClassic;
    public ConsumableItem CoinPlus;
    public ConsumableItem CoinPremium;


    public NonConsumableItem ncItem;
    public SubscriptionItem sItem;
    // public GameObject AdsPurchasedWindow;
    public Shop shop;

    public Data data;
    public Payload payload;
    public PayloadData payloadData;

    public TextMeshProUGUI txtX5;
    public TextMeshProUGUI txtX15;
    public TextMeshProUGUI txtX30;
    public TextMeshProUGUI txtcSpecial;
    public TextMeshProUGUI txtcOldSpecial;
    public TextMeshProUGUI txtcDeluxe;
    public TextMeshProUGUI txtsItem;
    public TextMeshProUGUI txtCompassClassic;
    public TextMeshProUGUI txtCompassPlus;
    public TextMeshProUGUI txtsCompassPremium;
    public TextMeshProUGUI txtSpecialClassic;
    public TextMeshProUGUI txtSpecialPlus;
    public TextMeshProUGUI txtWeeklyChallenge;
    public TextMeshProUGUI specialMapPrice;
    public TextMeshProUGUI txtCoinClassic;
    public TextMeshProUGUI txtCoinPlus;
    public TextMeshProUGUI txtCoinPremium;
    public TextMeshProUGUI txtCoinClassic1;
    public TextMeshProUGUI txtCoinPlus1;
    public TextMeshProUGUI txtCoinPremium1;

    private Firebase.FirebaseApp app;
    public GameObject ShopBanner;
    public GameObject ShopHintBanner;

    public GameObject BtrRemoveAdsInShop;
    public GameObject BtrRemoveAdsInSetting;
    public RemoveAds removeAds;
    // private string exchangeRateApiUrl = "https://api.exchangerate-api.com/v4/latest/USD";

    private bool waitOpenads = true;
    void Awake()
    {
        waitOpenads = true;
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        // InitializationOptions options = new InitializationOptions()
// #if UNITY_EDITOR || DEVELOPMENT_BUILD
//             .SetEnvironmentName("test");
// #else
//             .SetEnvironmentName("production");
// #endif
    }


    void Start()
    {
        // saveDataJson.SaveData("RemoveAds", true);

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        Vibration.Init();
        allowVibration = (bool)saveDataJson.GetData("Vibration");
        LoadAds();

        // SetupBuilder();

        InitializeIAP();

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
          var dependencyStatus = task.Result;
          if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
        
            // Set a flag here to indicate whether Firebase is ready to use by your app.
          } else {
            UnityEngine.Debug.LogError(System.String.Format(
              "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
          }
        });
    }

    // [Obsolete]
    void LoadAds()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // RequestNativeAd();

            if(!(bool)saveDataJson.GetData("PlayTutorial")) LoadAppOpenAd();
            
            CreateBannerView();

            LoadInterstitialAd();

            LoadRewardedAd();
        });
    }

    private void OnDestroy()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        storeController.OnPurchasePending -= OnPurchasePending;
        storeController.OnStoreDisconnected -= OnStoreDisconnected;
        storeController.OnProductsFetched -= OnProductsFetched;
        storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
        storeController.OnPurchasesFetched -= OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
        storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
        storeController.OnPurchaseDeferred -= OnPurchaseDeferred;
        storeController.OnPurchaseFailed -= OnPurchaseFailed;
    }


#region Banner

    public void CreateBannerView()
    {
        // Debug.LogWarning("Creating banner view"); 
        // If we already have a banner, destroy the old one.
        if((bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB")
        || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != "")) return;
        if (_bannerView != null) DestroyBannerAd();
        // Create a 320x50 banner at top of the screen
        // AdSize adSize = new AdSize(320, 100);
        _bannerView = new BannerView(_adBannerId, AdSize.Banner, AdPosition.Bottom);
        ListenToAdEvents();
    }

    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            // Debug.LogWarning("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void LoadBannerAd()
    {
        if((bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB")
        || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != "")) return;
        if(_bannerView == null) CreateBannerView();
        string uuid = Guid.NewGuid().ToString();
        // _bannerView = new BannerView(_adBannerId, AdSize.Banner, AdPosition.Bottom);
        var adRequest = new AdRequest();
        // adRequest.Extras.Add("collapsible", "bottom");
        // adRequest.Extras.Add("collapsible_request_id", uuid);
    
        _bannerView.LoadAd(adRequest);

        // Invoke("LoadNewBanner", 60f);
    }

    void LoadNewBanner() 
    {
        CreateBannerView();
        LoadBannerAd();
    }

    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            // Debug.LogWarning("Banner view loaded an ad with response : "
            //     + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            // Debug.LogWarning("Banner view failed to load an ad with error : "
            //     + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("Banner view paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            // Debug.LogWarning("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.LogWarning("Banner view full screen content closed.");
        };
    }

#endregion

#region Interstitial
    public void StopCountDownInter()
    {
        stopCountDownInter = true;
        CountDown.SetActive(false);
    }

    public void PauseCountDownInter()
    {
        pauseCountDownInter = true;
        CountDown.transform.GetChild(0).GetComponent<Slider>().DOPause();
    }

    public void ContinueCountDownInter()
    {
        pauseCountDownInter = false;
        CountDown.transform.GetChild(0).GetComponent<Slider>().DOPlay();
    }

    public void StartCountDownInter()
    {
        stopCountDownInter = false;
        // pauseCountDownInter = false;
        currentTime = 120;
        StartCoroutine(CountdownToInter());
    }
    int currentTime = 120;

    public void EndCountdownToInter(int time)
    {
        currentTime = time;
    }

    public IEnumerator CountdownToInter()
    {
        if (game.gameObject.activeSelf)
        {
            while (currentTime > 0)
            {
                // Debug.Log(currentTime);
                yield return new WaitForSeconds(1f);
                if (stopCountDownInter) break;
                if (!pauseCountDownInter) currentTime -= 1;
                if (resetTimeInter)
                {
                    currentTime = 120;
                    resetTimeInter = false;
                }
                // if(currentTime <= 5) ShowAdCountDown(currentTime);
                // Debug.Log(currentTime + " ?? " + Time.deltaTime);
            }

            if (!stopCountDownInter && !(bool)saveDataJson.GetData("RemoveAds") && !(bool)saveDataJson.GetData("LegendarySUB") &&
            ((string)saveDataJson.GetData("VIP3Day") == null || (string)saveDataJson.GetData("VIP3Day") == ""))
            {
                buttonControl.OffBtn();
                game.PauseCountDown();
                if (_interstitialAd != null && _interstitialAd.CanShowAd()) InterAds.SetActive(true);
                Invoke("ShowInterstitialAd", 1.5f);
            }
            else stopCountDownInter = false;
        }
    }

    void ShowAdCountDown (int time = 0)
    {
        if((bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB") || 
            ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != "")) return;
        RectTransform CountDownRect =  CountDown.GetComponent<RectTransform>();
        if(!CountDown.activeSelf)
        {
            if (_interstitialAd == null || !_interstitialAd.CanShowAd()) return;
            CountDown.transform.GetChild(0).GetComponent<Slider>().value = 1;
            CountDown.transform.GetChild(0).GetComponent<Slider>().DOPause();
            CountDown.transform.GetChild(0).GetComponent<Slider>().DOValue(0, 6, false);
            CountDownRect.anchoredPosition = new Vector2 (0, CountDownRect.sizeDelta.y / 2);
            CountDownRect.DOAnchorPos(new Vector2(0, -71), 0.5f);
            CountDown.SetActive(true);
        }
        CountDown.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{time}";
        if(time == 1) Invoke("ShowAdCountDown", 1);
        else if(time == 0) CountDownRect.DOAnchorPos(new Vector2(0, CountDownRect.sizeDelta.y / 2), 0.5f).OnComplete(() => CountDown.SetActive(false));
    }

    // void ShowInterAd ()
    // {
    //     ShowInterstitialAd();
    // }

    void ContinueGame(string txt = "")
    {
        InterAds.SetActive(false);
        if(txt == "thanks")
        {
            Thanks.SetActive(true);
            Invoke("WaitForThanks", 1);
        }else WaitForThanks();
    }

    void WaitForThanks()
    {
        Thanks.SetActive(false);
        game.ContinueCountDown();
        buttonControl.OnBtn();
        if((bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB")
        || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != "")) return;
        StartCountDownInter();
    }

    public void ShowInterstitialAd()
    {
        if((bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB")
        || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != "")) 
        {
            return;    
        }
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            // Debug.LogWarning("Showing interstitial ad.");
            audioManager.PauseAllAudio();
            _interstitialAd.Show();
        }
        else
        {
            // Debug.LogWarning("Interstitial ad is not ready yet.");
            LoadInterstitialAd();
            ContinueGame();
        }
    }

    public void LoadInterstitialAd()
    {
        if((bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB")
        || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != "")) return;
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
              _interstitialAd.Destroy();
              _interstitialAd = null;
        }

        // Debug.LogWarning("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        // adRequest.Keywords.Add("unity-admob-sample");
        // send the request to load the ad.
        InterstitialAd.Load(_adInterstitialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    // Debug.LogWarning("interstitial ad failed to load an ad " +
                    //                "with error : " + error);
                    return;
                }

                // Debug.LogWarning("Interstitial ad loaded with response : "
                //           + ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterEventHandlersInterstitial(_interstitialAd);
            });
    }

    private void RegisterEventHandlersInterstitial(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("Interstitial ad paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            // Debug.LogWarning("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            audioManager.UnPauseAllAudio();
            ContinueGame("thanks");

            // Debug.LogWarning("Interstitial ad full screen content closed.");
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            ContinueGame();
            
            // Debug.LogWarning("Interstitial ad failed to open full screen content " +
            //                "with error : " + error);
            LoadInterstitialAd();
        };
    }

#endregion

#region Reward
    private Coroutine closeHintBannerCoroutine;
    // public IEnumerator CountDownForHintSale()
    // {
    //     allowToShowShopBannerHint = true;
    //     yield return new WaitForSeconds(300); //300
    //     if(allowToShowShopBannerHint) ShowSHintBanner();
    // }

    public void StopCountDownForHintSale()
    {
        allowToShowShopBannerHint = false;
        if(closeHintBannerCoroutine != null) StopCoroutine(closeHintBannerCoroutine);
        StartCoroutine(CloseHintBanner(0));
    }

    public void ShowSHintBanner()
    {
        if(ShopBanner.activeSelf) 
        {
            Invoke("ShowSHintBanner", 1);
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // StartCoroutine(CountDownForComboDeluxe());
            return;
        };
        ShopHintBanner.SetActive(true);
        RectTransform ShopHintBannerRect =  ShopHintBanner.GetComponent<RectTransform>();

        ShopHintBannerRect.anchoredPosition = new Vector2 (-ShopHintBanner.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, -71);
        ShopHintBannerRect.DOPause();
        ShopHintBannerRect.DOAnchorPos(new Vector2(0, -71), 0.5f);
        
        closeHintBannerCoroutine = StartCoroutine(CloseHintBanner());
    }

    IEnumerator CloseHintBanner(int time = 10)
    {
        yield return new WaitForSeconds(time);
        if(ShopHintBanner.activeSelf)
        {
            RectTransform ShopHintBannerRect =  ShopHintBanner.GetComponent<RectTransform>();
            ShopHintBannerRect.DOPause();
            ShopHintBannerRect.DOAnchorPos(new Vector2(ShopHintBanner.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, -71), 0.5f).OnComplete(() => {
                ShopHintBanner.SetActive(false);
                if(allowToShowShopBannerHint) 
                {
                    StopCoroutine(closeHintBannerCoroutine);
                    // StartCoroutine(CountDownForHintSale());
                    StartCoroutine(CountDownForComboDeluxe());
                }
            });
        }
    }

    void GetReward ()
    {
        resetTimeInter = true;
        CountDown.transform.GetChild(0).GetComponent<Slider>().DOPause();
        CountDown.SetActive(false);
        audioManager.UnPauseAllAudio();
        if(nameReward == "Hint")
        {
            LogEvent("Hint");
            SmallShop.AddHint();
        }
        else if(nameReward == "Hint+2")
        {
            LogEvent("Hint+2");
            videoMoreHint.Exit();
            SmallShop.AddHint("Hint+2");
        }
        else if(nameReward == "Gift")
        {
            LogEvent("Gift");
            openedGift.GetReward();
        }
        else if(nameReward == "LuckyWheel")
        {
            LogEvent("LuckyWheel");
            randomSpin.GetReward();
        }
        else if (nameReward == "hint_sale")
        {
            LogEvent("hint_sale");
            SmallShop.AddHint("hint_sale");
            StartCoroutine(CloseHintBanner(0));
        }
        else if (nameReward == "BuyCompass")
        {
            LogEvent("Compass_AdReward");
            smallCompassShop.AddCompass();
        }
    }

    public bool CheckRewardAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd()) return true;
        ShowAdsMessage();
        return false;
    }

    public void ShowRewardedAd(string name)
    {
        // const string rewardMsg =
        //     "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            nameReward = name;
            audioManager.PauseAllAudio();
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                // Debug.LogWarning(String.Format(rewardMsg, reward.Type, reward.Amount));
                // GetReward(name);
            });
        }
        else {
            ShowAdsMessage();
            LoadRewardedAd();
        };
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
              _rewardedAd.Destroy();
              _rewardedAd = null;
        }

        // Debug.LogWarning("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adRewardId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    // Debug.LogWarning("Rewarded ad failed to load an ad " +
                    //                "with error : " + error);
                    return;
                }

                // Debug.LogWarning("Rewarded ad loaded with response : "
                //           + ad.GetResponseInfo());

                _rewardedAd = ad;
                RegisterEventHandlersReward(_rewardedAd);
            });
    }

    private void RegisterEventHandlersReward(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("Rewarded ad paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            // Debug.LogWarning("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            GetReward();
            // Debug.LogWarning("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // Debug.LogWarning("Rewarded ad failed to open full screen content " +
            //                "with error : " + error);
            LoadRewardedAd();
        };
    }

#endregion

#region OpenAds
    public void StopShowOpenAd()
    {
        stopShowOpenAd = true;
    }

    public void ShowAppOpenAd()
    {
        if (isPurchasing) return;
        if ((bool)saveDataJson.GetData("RemoveAds")
        || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != ""))
        {
            loading.ContinueLoading();
            return;
        }
        if (_appOpenAd != null && _appOpenAd.CanShowAd())
        {
            // Debug.LogWarning("Showing app open ad.");
            audioManager.PauseAllAudio();
            _appOpenAd.Show();
        }
        else
        {
            // Debug.LogWarning("App open ad is not ready yet");
            if(waitOpenads)
            {
                LoadAppOpenAd();
                showOpenAd = true;
            }
            else showOpenAd = false;
        }
    }

    void SetOpenAdStatus(bool status = true)
    {
        loadedOpenAd = status;
        loading.SetOpenAdStatus(status);
    }

    public void ChangeStatusStartGame ()
    {
        waitOpenads = false;
    }
    public void LoadAppOpenAd()
    {
        if (_appOpenAd != null)
        {
              _appOpenAd.Destroy();
              _appOpenAd = null;
        }
        SetOpenAdStatus(false);
        // Debug.LogWarning("Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(_adOpenId, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    // Debug.LogWarning("app open ad failed to load an ad " +
                    //                "with error : " + error);
                    countOpenAd++;
                    Debug.LogWarning(countOpenAd);
                    // if(countOpenAd <= 3) Invoke("LoadAppOpenAd", 1f);
                    // LoadAppOpenAd();
                    SetOpenAdStatus(true);
                    return;
                }

                // Debug.LogWarning("App open ad loaded with response : "
                //           + ad.GetResponseInfo());

                _appOpenAd = ad;
                RegisterEventHandlersOpenAd(ad);
                SetOpenAdStatus(true);
            });
    }

    private void RegisterEventHandlersOpenAd(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.LogWarning(String.Format("App open ad paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            // Debug.LogWarning("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            // Debug.LogWarning("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            // Debug.LogWarning("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            // Debug.LogWarning("App open ad full screen content closed.");
            audioManager.UnPauseAllAudio();
            LoadAppOpenAd();
            loading.ContinueLoading();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // Debug.LogWarning("App open ad failed to open full screen content " +
            //                "with error : " + error);
            if(waitOpenads) LoadAppOpenAd();
            // StartCoroutine(ShowAppOpenAdWithDelay());
        };
    }

    private void OnAppStateChanged(AppState state)
    {
        Debug.LogWarning("App State changed to : "+ state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            // if (IsAdAvailable)
            // {
                showOpenAd = true;
                // StartCoroutine(ShowAppOpenAdWithDelay());
            // }
            // else
        }
    }

    IEnumerator ShowAppOpenAdWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        ShowAppOpenAd();
    }

    void Update()
    {
        if(showOpenAd && loadedOpenAd && !stopShowOpenAd)
        {
            showOpenAd = false;
            ShowAppOpenAd();
        }
    }

#endregion

#region NativeAds
    [Obsolete]
    private void RequestNativeAd()
    {
        if((bool)saveDataJson.GetData("RemoveAds")
        || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != "")) return;
        LoadSettingNative();
        LoadNewAreaNative();
        LoadSmallShopNative();
    }

    [Obsolete]
    private void LoadSettingNative()
    {
        AdLoader adLoader = new AdLoader.Builder(NativeAdUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoadedSetting;
        adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

        var adRequest = new AdRequest();
        adLoader.LoadAd(adRequest);
    }

    [Obsolete]
    private void LoadNewAreaNative()
    {
        AdLoader adLoader = new AdLoader.Builder(NativeAdUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoadedNewArea;
        adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

        var adRequest = new AdRequest();
        adLoader.LoadAd(adRequest);
    }

    [Obsolete]
    private void LoadSmallShopNative()
    {
        AdLoader adLoader = new AdLoader.Builder(NativeAdUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoadedSmallShop;
        adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

        var adRequest = new AdRequest();
        adLoader.LoadAd(adRequest);
    }

    // [System.Obsolete]
    [Obsolete]
    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // NativeAdEventArgs
        // Debug.LogWarning("Native ad failed to load: " + args.LoadAdError.GetMessage());
        // Invoke("RequestNativeAd", 10f);
    }


    private void HandleNativeAdLoadedSetting(object sender, NativeAdEventArgs args)
    {
        this.nativeAdSetting = args.nativeAd;

        AdIconTextureSetting.texture = nativeAdSetting.GetIconTexture();
        AdHeadlineSetting.text = nativeAdSetting.GetHeadlineText();
        AdDescriptionSetting.text = nativeAdSetting.GetBodyText();

        //register gameobjects with native ads api
        if (!nativeAdSetting.RegisterIconImageGameObject(AdIconTextureSetting.gameObject))
        {
            // Debug.LogWarning("error registering icon");
        }
        if (!nativeAdSetting.RegisterHeadlineTextGameObject(AdHeadlineSetting.gameObject))
        {
            // Debug.LogWarning("error registering headline");
        }
        if (!nativeAdSetting.RegisterBodyTextGameObject(AdDescriptionSetting.gameObject))
        {
            // Debug.LogWarning("error registering description");
        }

        //disable loading and enable ad object
        AdLoadedSetting.gameObject.SetActive(true);
        AdLoadingSetting.gameObject.SetActive(false);
    }

    private void HandleNativeAdLoadedNewArea(object sender, NativeAdEventArgs args)
    {
        this.nativeAdNewArea = args.nativeAd;

        AdIconTextureNewArea.texture = nativeAdNewArea.GetIconTexture();
        AdHeadlineNewArea.text = nativeAdNewArea.GetHeadlineText();
        AdDescriptionNewArea.text = nativeAdNewArea.GetBodyText();

        //register gameobjects with native ads api
        if (!nativeAdNewArea.RegisterIconImageGameObject(AdIconTextureNewArea.gameObject))
        {
            // Debug.LogWarning("error registering icon");
        }
        if (!nativeAdNewArea.RegisterHeadlineTextGameObject(AdHeadlineNewArea.gameObject))
        {
            // Debug.LogWarning("error registering headline");
        }
        if (!nativeAdNewArea.RegisterBodyTextGameObject(AdDescriptionNewArea.gameObject))
        {
            // Debug.LogWarning("error registering description");
        }

        //disable loading and enable ad object
        AdLoadedNewArea.gameObject.SetActive(true);
        AdLoadingNewArea.gameObject.SetActive(false);
    }

    private void HandleNativeAdLoadedSmallShop(object sender, NativeAdEventArgs args)
    {
        this.nativeAdSmallShop = args.nativeAd;

        AdIconTextureSmallShop.texture = nativeAdSmallShop.GetIconTexture();
        AdHeadlineSmallShop.text = nativeAdSmallShop.GetHeadlineText();
        AdDescriptionSmallShop.text = nativeAdSmallShop.GetBodyText();

        //register gameobjects with native ads api
        if (!nativeAdSmallShop.RegisterIconImageGameObject(AdIconTextureSmallShop.gameObject))
        {
            // Debug.LogWarning("error registering icon");
        }
        if (!nativeAdSmallShop.RegisterHeadlineTextGameObject(AdHeadlineSmallShop.gameObject))
        {
            // Debug.LogWarning("error registering headline");
        }
        if (!nativeAdSmallShop.RegisterBodyTextGameObject(AdDescriptionSmallShop.gameObject))
        {
            // Debug.LogWarning("error registering description");
        }

        //disable loading and enable ad object
        AdLoadedSmallShop.gameObject.SetActive(true);
        AdLoadingSmallShop.gameObject.SetActive(false);
    }

#endregion

#region IAP

    private Coroutine closeComboDeluxeCoroutine;
    int countComboDeluxe = 0;
    bool isPurchasing = false;
    
    public IEnumerator CountDownForComboDeluxe(string txt = "")
    {
        if (txt == "first") countComboDeluxe = 0;
        allowToShowShopBanner = true;
        yield return new WaitForSeconds(150); //60
        if (allowToShowShopBanner)
        {
            bool removeAds = (bool)saveDataJson.GetData("RemoveAds");
            if (!removeAds && countComboDeluxe < 2)
            {
                countComboDeluxe++;
                ShowShopBanner();
            }
            else
            {
                countComboDeluxe = 0;
                ShowSHintBanner();
            }

        }
    }

    public void StopCountDownForComboDeluxe()
    {
        allowToShowShopBanner = false;
        if(closeComboDeluxeCoroutine != null) StopCoroutine(closeComboDeluxeCoroutine);
        StartCoroutine(CloseShopBanner(0));
    }

    public void ShowShopBanner()
    {
        if((int)saveDataJson.GetData("OpenedMap") == 1) return;

        if(ShopHintBanner.activeSelf)
        { 
            Invoke("ShowShopBanner", 1);
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable || (bool)saveDataJson.GetData("RemoveAds")) {
            StartCoroutine(CountDownForComboDeluxe());
            return;
        };
        ShopBanner.SetActive(true);
        RectTransform ShopBannerRect =  ShopBanner.GetComponent<RectTransform>();

        ShopBannerRect.anchoredPosition = new Vector2 (-ShopBanner.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, -71);
        ShopBannerRect.DOPause();
        ShopBannerRect.DOAnchorPos(new Vector2(0, -71), 0.5f);
        
        closeComboDeluxeCoroutine = StartCoroutine(CloseShopBanner());
    }

    IEnumerator CloseShopBanner(int time = 10)
    {
        yield return new WaitForSeconds(time);
        if(ShopBanner.activeSelf)
        {
            RectTransform ShopBannerRect =  ShopBanner.GetComponent<RectTransform>();
            ShopBannerRect.DOPause();
            ShopBannerRect.DOAnchorPos(new Vector2(ShopBanner.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, -71), 0.5f).OnComplete(() => {
                ShopBanner.SetActive(false);
                if(allowToShowShopBanner) {
                    StopCoroutine(closeComboDeluxeCoroutine);
                    StartCoroutine(CountDownForComboDeluxe());
                }
            });
        }
    }

    public void ConnumableBtn(string val)
    {
        isPurchasing = true;
        if (val == "x5") storeController.PurchaseProduct(cItemX5.Id);
        else if (val == "x15") storeController.PurchaseProduct(cItemX15.Id);
        else if (val == "x30") storeController.PurchaseProduct(cItemX30.Id);
        else if (val == "ComboSpecial") storeController.PurchaseProduct(cSpecial.Id);
        else if (val == "ComboDeluxe") storeController.PurchaseProduct(cDeluxe.Id);
        else if (val == "CompassClassic") storeController.PurchaseProduct(CompassClassic.Id);
        else if (val == "CompassPlus") storeController.PurchaseProduct(CompassPlus.Id);
        else if (val == "CompassPremium") storeController.PurchaseProduct(CompassPremium.Id);
        else if (val == "SpecialClassic") storeController.PurchaseProduct(SpecialClassic.Id);
        else if (val == "SpecialPlus") storeController.PurchaseProduct(SpecialPlus.Id);
        else if (val == "WeeklyChallenge") storeController.PurchaseProduct(WeeklyChallenge.Id);
        else if (val == "CoinClassic") storeController.PurchaseProduct(CoinClassic.Id);
        else if (val == "CoinPlus") storeController.PurchaseProduct(CoinPlus.Id);
        else if (val == "CoinPremium") storeController.PurchaseProduct(CoinPremium.Id);

        // if (val == "x5") storeController.InitiatePurchase(cItemX5.Id);
        // else if (val == "x15") storeController.InitiatePurchase(cItemX15.Id);
        // else if (val == "x30") storeController.InitiatePurchase(cItemX30.Id);
        // else if (val == "ComboSpecial") storeController.InitiatePurchase(cSpecial.Id);
        // else if (val == "ComboDeluxe") storeController.InitiatePurchase(cDeluxe.Id);
        // else if (val == "CompassClassic") storeController.InitiatePurchase(CompassClassic.Id);
        // else if (val == "CompassPlus") storeController.InitiatePurchase(CompassPlus.Id);
        // else if (val == "CompassPremium") storeController.InitiatePurchase(CompassPremium.Id);
        // else if (val == "SpecialClassic") storeController.InitiatePurchase(SpecialClassic.Id);
        // else if (val == "SpecialPlus") storeController.InitiatePurchase(SpecialPlus.Id);
        // else if (val == "WeeklyChallenge") storeController.InitiatePurchase(WeeklyChallenge.Id);
        // // else if (val == "special_map") storeController.InitiatePurchase(SpecialMap.Id);
        // // else if (val == "special_map_sale") storeController.InitiatePurchase(SpecialMapSale.Id);
        // else if (val == "CoinClassic") storeController.InitiatePurchase(CoinClassic.Id);
        // else if (val == "CoinPlus") storeController.InitiatePurchase(CoinPlus.Id);
        // else if (val == "CoinPremium") storeController.InitiatePurchase(CoinPremium.Id);
    }

    public void NonConnumableBtn()
    {
        isPurchasing = true;
        storeController.PurchaseProduct(ncItem.Id);
    }

    public void BuySpecialMapBtn(string val)
    {
        isPurchasing = true;
        if (val == "special_map") storeController.PurchaseProduct(SpecialMap.Id);
        else if (val == "special_map_sale") storeController.PurchaseProduct(SpecialMapSale.Id);
    }

    public void Subscription()
    {
        isPurchasing = true;
        storeController.PurchaseProduct(sItem.Id);
    }

    async void InitializeIAP()
    {
        storeController = UnityIAPServices.StoreController();
        storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnStoreDisconnected += OnStoreDisconnected;
        storeController.OnPurchaseFailed += OnPurchaseFailed;
        storeController.OnPurchaseDeferred += OnPurchaseDeferred;
        await storeController.Connect();

        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnProductsFetchFailed += OnProductsFetchFailed;
        storeController.OnPurchasesFetched += OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

        var initialProductsToFetch = new List<ProductDefinition>
        {
            new(cItemX5.Id, ProductType.Consumable),
            new(cItemX15.Id, ProductType.Consumable),
            new(cItemX30.Id, ProductType.Consumable),

            new(CompassClassic.Id, ProductType.Consumable),
            new(CompassPlus.Id, ProductType.Consumable),
            new(CompassPremium.Id, ProductType.Consumable),
            new(SpecialClassic.Id, ProductType.Consumable),
            new(SpecialPlus.Id, ProductType.Consumable),
            new(WeeklyChallenge.Id, ProductType.Consumable),

            new(SpecialMap.Id, ProductType.NonConsumable),
            new(SpecialMapSale.Id, ProductType.NonConsumable),

            new(CoinClassic.Id, ProductType.Consumable),
            new(CoinPlus.Id, ProductType.Consumable),
            new(CoinPremium.Id, ProductType.Consumable),

            new(cSpecial.Id, ProductType.Consumable),
            new(cDeluxe.Id, ProductType.Consumable),
            new(ncItem.Id, ProductType.NonConsumable),
            new(sItem.Id, ProductType.Subscription),
        };

        storeController.FetchProducts(initialProductsToFetch);
        Debug.LogWarning(storeController);
    }

    void OnPurchasePending(PendingOrder pendingOrder)
    {
        Debug.LogWarning("OnPurchasePending_____________________");
        if (pendingOrder.Info.PurchasedProductInfo.Count > 0)
        {
            storeController.ConfirmPurchase(pendingOrder);
        }
        else
        {
            StartCoroutine(ConfirmWhenReady(pendingOrder));
        }
    }

    IEnumerator ConfirmWhenReady(PendingOrder pendingOrder)
    {
        float timeout = 5f; // tối đa 5 giây chờ
        float elapsed = 0f;

        while (pendingOrder.Info.PurchasedProductInfo.Count == 0 && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        storeController.ConfirmPurchase(pendingOrder);
    }

    void OnPurchaseConfirmed(Order order)
    {
        Debug.LogWarning("OnPurchaseConfirmed_____________________");

        isPurchasing = false;

        foreach (var purchasedProduct in order.Info.PurchasedProductInfo)
        {

            string productId = purchasedProduct.productId;
            // string productId = order.Info.PurchasedProductInfo[0].productId;

            if (productId == ncItem.Id)
            {
                saveDataJson.SaveData("RemoveAds", true);
                RemoveAds();
            }
            else if (productId == SpecialMapSale.Id || productId == SpecialMap.Id) shop.AddHint(productId);
            else if (productId == sItem.Id) //subscribed
            {
                saveDataJson.SaveData("LegendarySUB", true);
                saveDataJson.SaveData("OpenAllMaps", true);
                ActivateElitePass();
            }
            else
            {
                try
                {
                    var unifiedReceipt = JsonUtility.FromJson<UnifiedReceipt>(order.Info.Receipt);

                    // Parse lớp thứ 2 (Payload)
                    var googleWrapper = JsonUtility.FromJson<GooglePayloadWrapper>(unifiedReceipt.Payload);

                    // Parse lớp thứ 3 (json)
                    var purchaseData = JsonUtility.FromJson<GooglePurchaseData>(googleWrapper.json);

                    Debug.Log($"Product: {purchaseData.productId}, Quantity: {purchaseData.quantity}");

                    for (int i = 0; i < purchaseData.quantity; i++)
                    {
                        shop.AddHint(productId);
                        if (productId == "combo_deluxe" || productId == "combo_special")
                        {
                            saveDataJson.SaveData("RemoveAds", true);
                            RemoveAds();
                        }
                    }
                }
                catch
                {
                    shop.AddHint(productId);
                    if (productId == "combo_deluxe" || productId == "combo_special")
                    {
                        saveDataJson.SaveData("RemoveAds", true);
                        RemoveAds();
                    }
                }
            }
        }
    }

[Serializable]
public class UnifiedReceipt
{
    public string Store;
    public string TransactionID;
    public string Payload;
}

[Serializable]
public class GooglePayloadWrapper
{
    public string json;
    public string signature;
}

[Serializable]
public class GooglePurchaseData
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public int quantity; // Đây là cái bạn cần
}

    void OnPurchaseFailed(FailedOrder failedOrder)
    {
        isPurchasing = false;
        Debug.LogError("FailedOrder: " + failedOrder);
    }
    void OnPurchaseDeferred(DeferredOrder deferredOrder)
    {
        isPurchasing = false;
        Debug.LogWarning("DeferredOrder: " + deferredOrder);
    }


    void OnProductsFetched(List<Product> products)
    {
        storeController.FetchPurchases();
    }

    void OnProductsFetchFailed(ProductFetchFailed fail)
    {
        Debug.LogError($"Products fetch failed: {fail.FailureReason} - {fail.FailedFetchProducts}");
    }

    void OnPurchasesFetched(Orders orders)
    {
        Debug.LogWarning("OnPurchasesFetched: " + orders);
        ConvertPrice();
        // Debug.Log(storeController.GetProductById(sItem.Id));

        bool isRemoveAds = false;
        bool isSubscription = false;

        foreach (var order in orders.ConfirmedOrders)
        {
            foreach (var product in order.Info.PurchasedProductInfo)
            {
                if (product.productId == sItem.Id)
                {
                    isSubscription = true;
                }
                else if (product.productId == ncItem.Id)
                {
                    isRemoveAds = true;
                }
            }
            if (isSubscription && isRemoveAds) break;
        }

        if(isSubscription) ActivateElitePass();
        else DeActivateElitePass();

        if (isRemoveAds
            || (bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB")
            || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != ""))
        {
            RemoveAds();
        }
        else
        {
            ShowAds();
        }
    }

    void OnPurchasesFetchFailed(PurchasesFetchFailureDescription PurchasesFetchFailureDescription)
    {
        DeActivateElitePass();
        Debug.LogError("OnPurchasesFetchFailed: " + PurchasesFetchFailureDescription);
    }

    void OnStoreDisconnected(StoreConnectionFailureDescription storeConnectionFailureDescription)
    {
        isPurchasing = false;
        Debug.LogError("OnStoreDisconnected: " + storeConnectionFailureDescription);
    }

    void ConvertPrice()
    {
        foreach (Product product in storeController.GetProducts())
        {
            ConvertPriceToLocalCurrency(product, product.definition.id);
        }
    }

    void ConvertPriceToLocalCurrency(Product product, string id)
    {
        if(product == null) return;
        ProductMetadata metadata = product.metadata;
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        RegionInfo region = new RegionInfo(currentCulture.Name);

        string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
        if(id == cItemX5.Id) txtX5.text = txt;
        else if(id == cItemX15.Id) txtX15.text = txt;
        else if(id == cItemX30.Id) txtX30.text = txt;
        else if(id == cDeluxe.Id) txtcDeluxe.text = txt;
        else if(id == CompassClassic.Id) txtCompassClassic.text = txt;
        else if(id == CompassPlus.Id) txtCompassPlus.text = txt;
        else if(id == CompassPremium.Id) txtsCompassPremium.text = txt;
        else if(id == SpecialClassic.Id) txtSpecialClassic.text = txt;
        else if(id == SpecialPlus.Id) txtSpecialPlus.text = txt;
        else if(id == WeeklyChallenge.Id) txtWeeklyChallenge.text = txt;
        else if(id == CoinClassic.Id) { txtCoinClassic.text = txt; txtCoinClassic1.text = txt; }
        else if(id == CoinPlus.Id) { txtCoinPlus.text = txt; txtCoinPlus1.text = txt;}
        else if(id == CoinPremium.Id) { txtCoinPremium.text = txt; txtCoinPremium1.text = txt;}
        else if(id == sItem.Id) txtsItem.text = txt;
        else if(id == SpecialMapSale.Id || id == SpecialMap.Id) specialMapPrice.text = txt;
        else if (id == cSpecial.Id)
        {
            txtcSpecial.text = txt;
            txtcOldSpecial.text = $"{metadata.localizedPrice * 2}{region.CurrencySymbol}";
        }

    }

    // void ConvertPriceToLocalCurrency(NonConsumableItem item)
    // {
    //     Product product = storeController.GetProductById(item.Id);
    //     if(product == null) return;
    //     ProductMetadata metadata = product.metadata;
    //     CultureInfo currentCulture = CultureInfo.CurrentCulture;
    //     RegionInfo region = new RegionInfo(currentCulture.Name);

    //     string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
    //     specialMapPrice.text = txt;
    // }

    void SetupBuilder()
    {
        // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        // builder.AddProduct(cItemX5.Id, ProductType.Consumable);
        // builder.AddProduct(cItemX15.Id, ProductType.Consumable);
        // builder.AddProduct(cItemX30.Id, ProductType.Consumable);
    
        // builder.AddProduct(CompassClassic.Id, ProductType.Consumable);
        // builder.AddProduct(CompassPlus.Id, ProductType.Consumable);
        // builder.AddProduct(CompassPremium.Id, ProductType.Consumable);
        // builder.AddProduct(SpecialClassic.Id, ProductType.Consumable);
        // builder.AddProduct(SpecialPlus.Id, ProductType.Consumable);
        // builder.AddProduct(WeeklyChallenge.Id, ProductType.Consumable);

        // builder.AddProduct(SpecialMap.Id, ProductType.NonConsumable);
        // builder.AddProduct(SpecialMapSale.Id, ProductType.NonConsumable);

        // builder.AddProduct(CoinClassic.Id, ProductType.Consumable);
        // builder.AddProduct(CoinPlus.Id, ProductType.Consumable);
        // builder.AddProduct(CoinPremium.Id, ProductType.Consumable);

        // builder.AddProduct(cSpecial.Id, ProductType.Consumable);
        // builder.AddProduct(cDeluxe.Id, ProductType.Consumable);
        // builder.AddProduct(ncItem.Id, ProductType.NonConsumable);
        // builder.AddProduct(sItem.Id, ProductType.Subscription);
        
        // UnityPurchasing.Initialize(this, builder);
    }

    void CheckNonConsumable(string id) {
        // if (storeController!=null)
        // {
        //     var product = storeController.products.WithID(id);
        //     if (product!=null)
        //     { 
        //         if (product.hasReceipt || (bool)saveDataJson.GetData("RemoveAds") || (bool)saveDataJson.GetData("LegendarySUB")
        //         || ((string)saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != ""))//purchased
        //         {
        //             RemoveAds();
        //         }
        //         else {
        //             ShowAds();
        //         }
        //     }
        // }
    }

    public void RemoveAds()
    {
        DisplayAds(false);
    }

    void ShowAds()
    {
        DisplayAds(true);
    }

    void DisplayAds(bool x)
    {
        if (!x)
        {
            StartCoroutine(CloseShopBanner(0));

            AdLoadedNewArea.transform.parent.gameObject.SetActive(false);
            AdLoadedSetting.transform.parent.gameObject.SetActive(false);
            AdLoadedSmallShop.transform.parent.gameObject.SetActive(false);
            DestroyBannerAd();
            
            if((bool)saveDataJson.GetData("RemoveAds"))
            {
                BtrRemoveAdsInSetting.GetComponent<Image>().color = new Color(0.372f, 0.372f, 0.372f);
                BtrRemoveAdsInSetting.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.568f, 0.568f, 0.568f);
                BtrRemoveAdsInShop.GetComponent<Image>().color = new Color(0.372f, 0.372f, 0.372f);
                BtrRemoveAdsInShop.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.568f, 0.568f, 0.568f);
                BtrRemoveAdsInSetting.GetComponent<Button>().interactable = false;
                BtrRemoveAdsInShop.GetComponent<Button>().interactable = false;
            }

            if(removeAds.gameObject.activeSelf) removeAds.Exit();
        }
        else
        {
            saveDataJson.SaveData("RemoveAds", false);
            // AdsPurchasedWindow.SetActive(false);
            // adsBanner.SetActive(true);
        }
    }

    void ActivateElitePass()
    {
        setupElitePass(true);
    }

    void DeActivateElitePass()
    {
        setupElitePass(false);
    }

    string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    void setupElitePass(bool x)
    {
        if (x)// active
        {
            shop.ExitLegendarySub();
            home.CheckUnlockMap();
            RemoveAds();
            DateTime currentTime = DateTime.Now;
            string todayLegendarySub = (string)saveDataJson.GetData("TodayLegendarySUB");
            BtnSub.SetActive(false);
            if(todayLegendarySub == "" || DateTime.ParseExact(todayLegendarySub, "dd/MM/yyyy HH:mm:ss", null).Date < currentTime.Date)
            {
                string todayLegendarySubstring = FormatDateTime(currentTime);
                saveDataJson.SaveData("TodayLegendarySUB", todayLegendarySubstring);
                shop.AddHint(sItem.Id);
            }
        }
        else
        {
            saveDataJson.SaveData("LegendarySUB", false);
            saveDataJson.SaveData("TodayLegendarySUB", "");
            saveDataJson.SaveData("OpenAllMaps", false);
            LoadAds();
        }
    }

    // void CheckSubscription(string id) {

    //     var subProduct = storeController.products.WithID(id);
    //     if (subProduct != null)
    //     {
    //         try
    //         {
    //             if (subProduct.hasReceipt)
    //             {
    //                 var subManager = new SubscriptionManager(subProduct, null);
    //                 var info = subManager.getSubscriptionInfo();
    //                 /*print(info.getCancelDate());
    //                 print(info.getExpireDate());
    //                 print(info.getFreeTrialPeriod());
    //                 print(info.getIntroductoryPrice());
    //                 print(info.getProductId());
    //                 print(info.getPurchaseDate());
    //                 print(info.getRemainingTime());
    //                 print(info.getSkuDetails());
    //                 print(info.getSubscriptionPeriod());
    //                 print(info.isAutoRenewing());
    //                 print(info.isCancelled());
    //                 print(info.isExpired());
    //                 print(info.isFreeTrial());
    //                 print(info.isSubscribed());*/

    //                 if (info.isSubscribed() == Result.True)
    //                 {
    //                     print("We are subscribed");
    //                     ActivateElitePass();
    //                 }
    //                 else {
    //                     print("Un subscribed");
    //                     DeActivateElitePass();
    //                 }

    //             }
    //             else{
    //                 print("receipt not found !!");
    //                 DeActivateElitePass();
    //             }
    //         }
    //         catch (Exception)
    //         {
    //             print("It only work for Google store, app store, amazon store, you are using fake store!!");
    //         }
    //     }
    //     else {
    //         print("product not found !!");
    //     }
    // }

    // public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    // {
    //     throw new NotImplementedException();
    // }

    // public void OnInitializeFailed(InitializationFailureReason error)
    // {
    //     throw new NotImplementedException();
    // }

    // public void OnInitializeFailed(InitializationFailureReason error, string message)
    // {
    //     isPurchasing = false;
    //     throw new NotImplementedException();
    // }

    // public Purchase ProcessPurchase(PurchaseEventArgs purchaseEvent)
    // public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    // {
    //     //Retrive the purchased product
    //     var product = purchaseEvent.purchasedProduct;
    //     isPurchasing = false;
    //     // Debug.Log(product.definition.id);
    //     // print("Purchase Complete" + product.definition.id);
    //     if (product.definition.type == ProductType.NonConsumable)
    //     {
    //         if (product.definition.id == ncItem.Id)
    //         {
    //             saveDataJson.SaveData("RemoveAds", true);
    //             RemoveAds();
    //         }
    //         else shop.AddHint(product.definition.id);
    //     }
    //     // if (product.definition.id == ncItem.Id)//non consumable
    //     // {
    //     //     saveDataJson.SaveData("RemoveAds", true);
    //     //     RemoveAds();
    //     // }
    //     else if (product.definition.id == sItem.Id)//subscribed
    //     {
    //         saveDataJson.SaveData("LegendarySUB", true);
    //         saveDataJson.SaveData("OpenAllMaps", true);
    //         ActivateElitePass();
    //     }
    //     else
    //     // if (product.definition.id == cItemX5.Id || product.definition.id == cItemX15.Id || product.definition.id == cItemX30.Id ||
    //     //         product.definition.id == cDeluxe.Id ||product.definition.id == cSpecial.Id) 
    //     {
    //         string receipt = product.receipt;
    //         data = JsonUtility.FromJson<Data>(receipt);
    //         int quantity = 1;
    //         if (data.Payload != "ThisIsFakeReceiptData")
    //         {
    //             payload = JsonUtility.FromJson<Payload>(data.Payload);
    //             payloadData = JsonUtility.FromJson<PayloadData>(payload.json);
    //             quantity = payloadData.quantity;
    //         }

    //         for (int i = 0; i < quantity; i++)
    //         {
    //             shop.AddHint(product.definition.id);
    //             if (product.definition.id == "combo_deluxe" || product.definition.id == "combo_special")
    //             {
    //                 saveDataJson.SaveData("RemoveAds", true);
    //                 RemoveAds();
    //             }
    //         }
    //     }

    //     return PurchaseProcessingResult.Complete;
    // }

    // public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    // {
    //     throw new NotImplementedException();
    // }

    public void ChangeSpecialMapPrice(int val)
    {
        // if(val == 1) ConvertPriceToLocalCurrency(SpecialMapSale);
        // else ConvertPriceToLocalCurrency(SpecialMap);

        if(val == 1) ConvertPriceToLocalCurrency(storeController.GetProductById(SpecialMapSale.Id), SpecialMapSale.Id);
        else ConvertPriceToLocalCurrency(storeController.GetProductById(SpecialMap.Id), SpecialMap.Id);
    }

    // public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    // {
    //     storeController = controller;
    //     CheckNonConsumable(ncItem.Id);
    //     CheckSubscription(sItem.Id);

        // StartCoroutine(ConvertPriceToLocalCurrency(cItemX5.price));
        // StartCoroutine(ConvertPriceToLocalCurrency(cItemX15.price));
        // StartCoroutine(ConvertPriceToLocalCurrency(cItemX30.price));

        // ConvertPriceToLocalCurrency(cItemX5);
        // ConvertPriceToLocalCurrency(cItemX15);
        // ConvertPriceToLocalCurrency(cItemX30);
        // ConvertPriceToLocalCurrency(cSpecial);
        // ConvertPriceToLocalCurrency(cDeluxe);
        // ConvertPriceToLocalCurrency(sItem);

        // ConvertPriceToLocalCurrency(CompassClassic);
        // ConvertPriceToLocalCurrency(CompassPlus);
        // ConvertPriceToLocalCurrency(CompassPremium);
        // ConvertPriceToLocalCurrency(SpecialClassic);
        // ConvertPriceToLocalCurrency(SpecialPlus);
        // ConvertPriceToLocalCurrency(WeeklyChallenge);

        // ConvertPriceToLocalCurrency(CoinClassic);
        // ConvertPriceToLocalCurrency(CoinPlus);
        // ConvertPriceToLocalCurrency(CoinPremium);
    // }

    // void ConvertPriceToLocalCurrency(ConsumableItem item)
    // {
    //     Product product = storeController.GetProductById(item.Id);
    //     if(product == null) return;
    //     ProductMetadata metadata = product.metadata;
    //     CultureInfo currentCulture = CultureInfo.CurrentCulture;
    //     RegionInfo region = new RegionInfo(currentCulture.Name);

    //     string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
    //     if(item == cItemX5) txtX5.text = txt;
    //     else if(item == cItemX15) txtX15.text = txt;
    //     else if(item == cItemX30) txtX30.text = txt;
    //     else if(item == cDeluxe) txtcDeluxe.text = txt;
    //     else if(item == CompassClassic) txtCompassClassic.text = txt;
    //     else if(item == CompassPlus) txtCompassPlus.text = txt;
    //     else if(item == CompassPremium) txtsCompassPremium.text = txt;
    //     else if(item == SpecialClassic) txtSpecialClassic.text = txt;
    //     else if(item == SpecialPlus) txtSpecialPlus.text = txt;
    //     else if(item == WeeklyChallenge) txtWeeklyChallenge.text = txt;
    //     else if(item == CoinClassic) { txtCoinClassic.text = txt; txtCoinClassic1.text = txt; }
    //     else if(item == CoinPlus) { txtCoinPlus.text = txt; txtCoinPlus1.text = txt;}
    //     else if(item == CoinPremium) { txtCoinPremium.text = txt; txtCoinPremium1.text = txt;}
    //     else if(item == cSpecial)
    //     { 
    //         txtcSpecial.text = txt;
    //         txtcOldSpecial.text = $"{metadata.localizedPrice * 2}{region.CurrencySymbol}";
    //     }

    // }

    // void ConvertPriceToLocalCurrency(NonConsumableItem item)
    // {
    //     Product product = storeController.GetProductById(item.Id);
    //     if(product == null) return;
    //     ProductMetadata metadata = product.metadata;
    //     CultureInfo currentCulture = CultureInfo.CurrentCulture;
    //     RegionInfo region = new RegionInfo(currentCulture.Name);

    //     string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
    //     specialMapPrice.text = txt;
    // }

    // void ConvertPriceToLocalCurrency(SubscriptionItem item)
    // {
    //     Product product = storeController.GetProductById(item.Id);
    //     if(product == null) return;
    //     ProductMetadata metadata = product.metadata;
    //     CultureInfo currentCulture = CultureInfo.CurrentCulture;
    //     RegionInfo region = new RegionInfo(currentCulture.Name);

    //     string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
    //     if(item == sItem) txtsItem.text = txt;
    // }

[Serializable]
public class SkuDetails
{
    public string productId;
    public string type;
    public string title;
    public string name;
    public string iconUrl;
    public string description;
    public string price;
    public long price_amount_micros;
    public string price_currency_code;
    public string skuDetailsToken;
}

[Serializable]
public class PayloadData
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
    public int quantity;
    public bool acknowledged;
}

[Serializable]
public class Payload
{
    public string json;
    public string signature;
    public List<SkuDetails> skuDetails;
    public PayloadData payloadData;
}

[Serializable]
public class Data
{
    public string Payload;
    public string Store;
    public string TransactionID;
}
#endregion

#region  GDPR Consent Message
    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
        }
    }


    void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // Handle dismissal by reloading form.
        LoadConsentForm();
    }
#endregion

#region Log Events
    public void LogEvent(string name, string val)
    {
        if(app != null) Firebase.Analytics.FirebaseAnalytics.LogEvent("Completed_Map", name, val);
        // if(app != null) Firebase.Analytics.FirebaseAnalytics.LogEvent("Completed_Map", new Firebase.Analytics.Parameter(name, val));
    }

    public void LogEvent(string name)
    {
        if(app != null) Firebase.Analytics.FirebaseAnalytics.LogEvent(name);
    }
#endregion

#region OtherTasks
    public void ShowAdsMessage(string txt = "No Ads")
    {
        AdsMessage.SetActive(true);
        Image AdsMessageImage = AdsMessage.GetComponent<Image>();
        TextMeshProUGUI AdsMessageText = AdsMessage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        AdsMessage.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt);

        AdsMessageImage.DOPause();
        AdsMessageText.DOPause();

        AdsMessageImage.DOFade(0.83f, 0.5f);
        AdsMessageText.DOFade(1f, 0.5f);
        AdsMessageImage.DOFade(0f, 0.5f).SetDelay(2f);
        AdsMessageText.DOFade(0f, 0.5f).SetDelay(2f).OnComplete(() => {AdsMessage.SetActive(false);});
    }

    public void ChangeStatusOfVibration(bool status)
    {
        allowVibration = status;
    }

    public void VibrationDevice (long milliseconds = 250)
    {
        if(allowVibration)
        // Use Vibration.Vibrate(); for a classic default ~400ms vibration

        // Pop vibration: weak boom (For iOS: only available with the haptic engine. iPhone 6s minimum or Android)
        // Vibration.VibratePop();

        // Peek vibration: strong boom (For iOS: only available on iOS with the haptic engine. iPhone 6s minimum or Android)
        // Vibration.VibratePeek();

        // Nope vibration: series of three weak booms (For iOS: only available with the haptic engine. iPhone 6s minimum or Android)
        // Vibration.VibrateNope();
    
        Vibration.VibratePop();
    }

    public void Rate()
    {
        audioManager.PlaySFX("click");
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects");
#else
        Application.OpenURL("https://apps.apple.com/app/id6736607907");
#endif
    }

    public void ShareLink()
    {
#if UNITY_ANDROID

        if (!isProcessing)
        {
            audioManager.PlaySFX("click");
            StartCoroutine(ShareTextInAnroid());
        }

#else
		Debug.Log("No sharing set up for this platform.");
#endif
    }

    public void MoreGame()
    {
        Application.OpenURL("https://play.google.com/store/apps/developer?id=Vnstart+LLC");
    }

    void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }


#if UNITY_ANDROID
    public IEnumerator ShareTextInAnroid()
    {
        var shareSubject = "Play Find It on your phone"; //Subject text
        var shareMessage = "Get game from this link: " + //Message text
                           "https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects"; //Your link

        isProcessing = true;

        if (!Application.isEditor)
        {
            //Create intent for action send
            AndroidJavaClass intentClass =
                new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject =
                new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>
                ("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //put text and subject extra
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");

            intentObject.Call<AndroidJavaObject>
                ("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>
                ("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

            //call createChooser method of activity class
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            AndroidJavaObject currentActivity =
                unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser =
                intentClass.CallStatic<AndroidJavaObject>
                ("createChooser", intentObject, "Share your high score");
            currentActivity.Call("startActivity", chooser);
        }

        yield return new WaitUntil(() => isFocus);
        isProcessing = false;
    }
#endif


    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://vnstart.com/privacy-policy.html");
    }

    public void OpenTermsOfService()
    {
        Application.OpenURL("https://policies.google.com/terms");
    }

#endregion
}
