using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AdsManager adsManager;
    public AudioManager audioManager;

    public GameObject LegendarySUB;
    public GameObject Sale50;
    public GameObject BtnSale;
    public BuySpecialMap buySpecialMap;

    public TextMeshProUGUI goldInHome;
    public TextMeshProUGUI goldInOpenGift;
    public TextMeshProUGUI goldSmallShop;
    public TextMeshProUGUI goldSmallCompassShop;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI hintInGame;

    public Timer timer;

    public TextMeshProUGUI compassInShop;
    public TextMeshProUGUI compassInGame;

    public WeeklyReward weeklyReward;

    private bool enabledToClick = true;

    public SmallShop smallShop;
    public SmallCompassShop smallCompassShop;

    void Start()
    {
        hintText.text = $"{(int)saveDataJson.GetData("Hint")}";
        compassInShop.text = $"{(int)saveDataJson.GetData("Compass")}";
        CheckVip3Day();
    }

    public void BuyBtn()
    {
        // if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        int goldNum = (int)saveDataJson.GetData("Gold");

        if(goldNum >= 100)
        {
            goldSmallShop.text = $"{goldNum - 100}";
            goldSmallCompassShop.text = goldSmallShop.text;
            goldInHome.text = goldSmallShop.text;
            goldInOpenGift.text = goldSmallShop.text;
            saveDataJson.SaveData("Gold", goldNum - 100);
            AddHint();
        }else adsManager.ShowAdsMessage("Not Enough Gold");
    }

    public void BuyCompassBtn()
    {
        // if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        int goldNum = (int)saveDataJson.GetData("Gold");

        if(goldNum >= 200)
        {
            AddMoreStuff(-200, 0, 1);
        }else adsManager.ShowAdsMessage("Not Enough Gold");
    }

    public void AddHint(string txt = "")
    {
        switch (txt)
        {
            case "combo_classic": AddMoreStuff(0, 5, 0); break;
            case "combo_plus": AddMoreStuff(0, 12, 0); break;
            case "combo_premium": AddMoreStuff(0, 25, 0); break;
            case "combo_special":
                AddMoreStuff(0, 30, 0);
                BtnSale.SetActive(false);
                CloseAnimation(Sale50);
                break;
            case "combo_deluxe": AddMoreStuff(0, 10, 0); break;
            case "hidenlegendary":
            case "hiden_legendary": AddMoreStuff(0, 10, 5); break;
            case "tutorial": AddMoreStuff(0, 4, 0); break;
            case "compassclassic":
            case "compass_classic": AddMoreStuff(0, 0, 2); break;
            case "compass_plus": AddMoreStuff(0, 0, 5); break;
            case "compass_premium": AddMoreStuff(0, 0, 10); break;
            case "mix_special": AddMoreStuff(500, 12, 5); break;
            case "special_plus": AddMoreStuff(2000, 50, 15); break;
            case "weekly_challenge":
                saveDataJson.SaveData("WeeklyVIP", true);
                weeklyReward.ExitVipRewardDialog("bought");
                break;
            case "special_map":
            case "special_map_sale": buySpecialMap.BuyMap(); break;
            case "coin_classic":
            case "coin_classic1":
                AddMoreStuff(500,0,0);
                if(smallShop.gameObject.activeSelf) smallShop.CloseBoardCoin();
                else smallCompassShop.CloseBoardCoin();
                break;
            case "coin_plus":
                AddMoreStuff(1000,0,0);
                if(smallShop.gameObject.activeSelf) smallShop.CloseBoardCoin();
                else smallCompassShop.CloseBoardCoin();
                break;
            case "coin_premium":
                AddMoreStuff(2500,0,0);
                if(smallShop.gameObject.activeSelf) smallShop.CloseBoardCoin();
                else smallCompassShop.CloseBoardCoin();
                break;
        }
    }

    public void AddMoreStuff(int gold , int hint, int compass, bool vip = false)
    {
        int times = vip ? 2 : 1;

        if(gold != 0)
        {
            SaveGold(gold * times);
        }
        if(hint != 0)
        {
            int hintNum = (int)saveDataJson.GetData("Hint") + hint * times;
            saveDataJson.SaveData("Hint", hintNum);
            hintText.text = $"{hintNum}";
            hintInGame.text = $"{hintNum}";
        }
        if(compass != 0)
        {
            int compassNum = (int)saveDataJson.GetData("Compass") + compass * times;
            saveDataJson.SaveData("Compass", compassNum);
            compassInShop.text = $"{compassNum}";
            compassInGame.text = $"{compassNum}";
        }
    }


    void SaveGold(int val)
    {
        // enabledToClick = false;
        // PlayAnimationGold(val);
        audioManager.PlaySFX("coin_appear");
        int gold = (int)saveDataJson.GetData("Gold") + val;
        saveDataJson.SaveData("Gold", gold);

        goldInHome.text = $"{gold}";
        goldSmallShop.text = $"{gold}";
        goldInOpenGift.text = $"{gold}";
        goldSmallCompassShop.text = $"{gold}";
    }

    public void ChangeHintText(int val)
    {
        audioManager.PlaySFX("click");
        hintText.text = $"{val}";
    }

    public void BuyHintSale()
    {
        audioManager.PlaySFX("click");
        adsManager.ShowRewardedAd("hint_sale");
    }

    public void BuyX5Hint()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("x5");
    }

    public void BuyX15Hint()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("x15");
    }

    public void BuyX30Hint()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("x30");
    }

    public void BuyCompassClassic()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("CompassClassic");
    }

    public void BuyCompassPlus()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("CompassPlus");
    }

    public void BuyCompassPremium()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("CompassPremium");
    }

    public void BuySpecialClassic()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("SpecialClassic");
    }

    public void BuySpecialPlus()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("SpecialPlus");
    }

    public void BuyWeeklyChallenge()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("WeeklyChallenge");
    }

    public void BuyComboDeluxe()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("ComboDeluxe");
    }

    public void BuyComboSpecial()
    {
        audioManager.PlaySFX("click");
        adsManager.ConnumableBtn("ComboSpecial");
    }

    public void OpenLegendarySub()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        enabledToClick = false;
        LegendarySUB.SetActive(true);
        Transform Btn = LegendarySUB.transform.GetChild(8);

        RectTransform rect = LegendarySUB.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, LegendarySUB.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y + 150);
        rect.DOAnchorPos(new Vector2(0, 0), 0.5f).SetEase(Ease.OutCubic).OnComplete(() => {
            Btn.localScale = Vector3.one;
            Btn.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            enabledToClick = true;
        });
    }

    public void ExitLegendarySub()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        enabledToClick = false;
        Transform Btn = LegendarySUB.transform.GetChild(8);
        LegendarySUB.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, LegendarySUB.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y + 150), 0.5f).SetEase(Ease.OutCubic).OnComplete(() => {
            Btn.DOKill();
            LegendarySUB.SetActive(false);
            enabledToClick = true;
        });
    }

    public void BuyLegendarySUB()
    {
        audioManager.PlaySFX("click");
        adsManager.Subscription();
    }

    public void RemoveAds()
    {
        audioManager.PlaySFX("click");
        adsManager.NonConnumableBtn();
    }

    public void OpenSaleDialog()
    {
        if(!enabledToClick) return;
        if(!timer.isCountingDown) return;
        audioManager.PlaySFX("click");
        Sale50.SetActive(true);
        OpenAnimation(Sale50);
    }

    public void OpenAnimation(GameObject target)
    {
        Transform board = target.transform.GetChild(1);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        Transform Btn = board.transform.GetChild(3);
        board.DOPause();
        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            Btn.localScale = Vector3.one;
            Btn.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            enabledToClick = true;
        });
    }

    public void CloseAnimation(GameObject target = null)
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        if(target == null) target = LegendarySUB;
        Transform board = target.transform.GetChild(1);
        Transform Btn = board.transform.GetChild(3);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            Btn.DOKill();
            target.SetActive(false);
            enabledToClick = true;
        });
    }

    string FormatDateTime(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss");
    public void VIP3Day()
    {
        DateTime currentTime = DateTime.Now;
        DateTime futureTime = currentTime.AddDays(3);
        string futureTimeFormatted = FormatDateTime(futureTime);
        saveDataJson.SaveData("VIP3Day", futureTimeFormatted);
        CheckVip3Day();
    }

    void CheckVip3Day()
    {
        if((string)saveDataJson.GetData("VIP3Day") == "" || saveDataJson.GetData("VIP3Day") == null) return;
        DateTime vip3Day = DateTime.ParseExact((string)saveDataJson.GetData("VIP3Day"), "dd/MM/yyyy HH:mm:ss", null);

        if(DateTime.Now <= vip3Day)
        { 
            adsManager.RemoveAds();
            saveDataJson.SaveData("OpenAllMaps", true);    
        }
        else if(DateTime.Now > vip3Day)
        { 
            saveDataJson.SaveData("VIP3Day", "");
            saveDataJson.SaveData("OpenAllMaps", false);    
        }
    }
}
