using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System;
using Unity.VisualScripting;

public class Home : MonoBehaviour
{
    public ButtonControl buttonControl;
    public SaveDataJson saveDataJson;
    private SaveDataJson.MapList MapData;

    public AdsManager adsManager;

    public ControlImageFromResources controlImage;

    public Setting setting;

    public GameObject InfoToOpenMap;

    public GameObject game;
    public GameObject gameScreen;
    public GameObject GoldTxt;
    public GameObject ListMap;
    public GameObject ListSpecialMap;
    public GameObject FirstMap;
    public GameObject FirstSpecialMap;
    public GameObject ListGold;

    public GameObject ChallengeList;
    public GameObject body;
    public GameObject menu;
    public GameObject WeeklyReward;

    public GameObject Challenge;
    public GameObject CollectionList;
    public GameObject CollectionBtn;
    public ExitChallengeDialog exitChallengeDialog;
    public CollectionInfo collectionInfo;

    public AudioManager audioManager;

    public TextMeshProUGUI goldInSmallShop;
    public TextMeshProUGUI goldInOpenGift;

    public WeeklyReward weeklyReward;

    private bool enabledToClick = true;

    void Awake()
    {
        // menu.onClick.AddListener(ShareText);
        Transform collection = body.transform.GetChild(0);
        Transform shop = body.transform.GetChild(1);
        Transform home = body.transform.GetChild(2);
        Transform challenge = body.transform.GetChild(3);
        Transform special = body.transform.GetChild(4);
        shop.gameObject.SetActive(true);
        challenge.gameObject.SetActive(true);
        home.gameObject.SetActive(true);
        collection.gameObject.SetActive(true);
        special.gameObject.SetActive(true);

        float XX = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;
        shop.GetComponent<RectTransform>().anchoredPosition = new Vector3(-XX, 0, shop.GetComponent<RectTransform>().localPosition.z);
        challenge.GetComponent<RectTransform>().anchoredPosition = new Vector3(XX, 0, shop.GetComponent<RectTransform>().localPosition.z);
        collection.GetComponent<RectTransform>().anchoredPosition = new Vector3(-XX * 2, 0, shop.GetComponent<RectTransform>().localPosition.z);
        special.GetComponent<RectTransform>().anchoredPosition = new Vector3(XX * 2, 0, shop.GetComponent<RectTransform>().localPosition.z);
    }

    // void Responsive() {
    //     Vector2 canvasSize = gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta;
    //     float check = canvasSize.y / canvasSize.x;
    //     if(check >= 2) {
    //         // cc.find("footer",this.node).scale = 0.9
    //         // cc.find('header', this.node).scale = 0.9
    //         // cc.find('body', this.node).scale = 0.9
    //     }
    // }

    void Start()
    {
        // saveDataJson.SaveData("Gold", 50000);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        // if (SystemInfo.processorFrequency < 2000)
        // {
        //     Application.targetFrameRate = 30;
        // }
        // else Application.targetFrameRate = 60;

        LoadHomeScreen();
        weeklyReward.CheckWeeklyReward();
    }

    public void PlayMusic()
    {
        audioManager.PlayMusic();
    }

    public void LoadHomeScreen()
    {
        MapData = saveDataJson.TakeMapData();
        CheckLanguage();
        SetChallenge();
        SetCollection();
        CreateMapList();
        CreateMapSpecialList();
        collectionInfo.SetPos();
    }

    public void UnlockChallenge(int map)
    {
        for(int i = 0; i < Challenge.transform.childCount; i++){
            if(i == map - 1) 
            {
                Challenge.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                break;
            }
        }
    }

    void CheckLanguage()
    {
        string language = (string)saveDataJson.GetData("Language");
        if(language == null || language == ""){
            StartCoroutine(CheckLocale());
        }else {
            int idLanguage = setting.TakeIdLanguage(language);
            StartCoroutine(SetLocale(idLanguage));
        }
    }

    IEnumerator CheckLocale ()
    {
        yield return LocalizationSettings.InitializationOperation;
        string txt = LocalizationSettings.SelectedLocale.ToString().Split(" (")[1].Split(")")[0].ToUpper();
        int idLanguage = setting.TakeIdLanguage(txt);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[idLanguage];
    }

    IEnumerator SetLocale (int id)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
    }

    void SetChallenge()
    {
        PlayerData data = saveDataJson.GetData();
        GoldTxt.GetComponent<TextMeshProUGUI>().text = $"{data.Gold}";
        int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");

        for(int i = 0; i < Challenge.transform.childCount; i++){
            if(i <= (int)saveDataJson.GetData("OpenedMap")- 2 || (listOpenedMap != null && listOpenedMap.Contains(i + 1)))
                Challenge.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
    }

    void SetCollection()
    {
        string[] collection = (string[]) saveDataJson.GetData("Collection");
        int[] collecionReward = (int[]) saveDataJson.GetData("CollecionReward");
        CollectionList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 270 * 9);

        if(collection != null && collection.Length > 0)
        {
            for(int j = 0; j < collection.Length; j++)
            {
                string name = collection[j];
                int map = Convert.ToInt32(name.Split("_")[0]);
                Transform image = CollectionList.transform.GetChild(map - 1).GetChild(1).GetChild(Convert.ToInt32(name.Split("_")[1]) - 1);
                image.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCard, name);
                // image.GetComponent<Button>().interactable = false;
                image.GetComponent<Button>().enabled = false;

                if(collecionReward.Contains(map))
                {
                    CollectionList.transform.GetChild(map - 1).GetChild(2).GetComponent<Button>().enabled = false;
                    CollectionList.transform.GetChild(map - 1).Find("Gift/tick").gameObject.SetActive(true);
                }
            }
        }

        for(int i = 0; i < 9; i++){
            CollectionList.transform.GetChild(i).GetChild(0).GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MapData.map[i + 1].AreaName);
            for(int z = 0; z < 4; z++)
            {
                if(CollectionList.transform.GetChild(i).GetChild(1).GetChild(z).GetComponent<Image>().sprite.name == "Nocard")
                {
                    break;
                }
                else if(z == 3 && !CollectionList.transform.GetChild(i).Find("Gift/tick").gameObject.activeSelf)
                {
                    CollectionList.transform.GetChild(i).GetChild(2).GetComponent<Image>().color = new Color(0,1,0);
                    GameObject warning = CollectionBtn.transform.GetChild(2).gameObject;
                    if(!warning.activeSelf)
                    {
                        warning.SetActive(true);
                        PlayWarning(CollectionBtn.transform.GetChild(2));
                    }
                }
            }
        }

    }

    public void OpenNewCollection(int map, int val)
    {
        Transform image = CollectionList.transform.GetChild(map - 1).GetChild(1).GetChild(val - 1);
        image.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCard, $"{map}_{val}");
        image.GetComponent<Button>().enabled = false;

        bool reward = true;
        foreach(Transform child in CollectionList.transform.GetChild(map - 1).GetChild(1))
        {
            if(child.GetComponent<Image>().sprite.name == "Nocard") 
            {
                reward = false;
                break;    
            }
        }

        if(reward)
        {
            CollectionList.transform.GetChild(map - 1).GetChild(2).GetComponent<Image>().color = new Color(0,1,0);

            GameObject warning = CollectionBtn.transform.GetChild(2).gameObject;
            if(!warning.activeSelf)
            {
                warning.SetActive(true);
                PlayWarning(CollectionBtn.transform.GetChild(2));
            }
        }
    }

    void PlayWarning(Transform warning)
    {
        if(!warning.gameObject.activeSelf) return;
        warning.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).OnComplete(() => {
            if(!warning.gameObject.activeSelf) return;
            warning.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.5f).OnComplete(() => {
                PlayWarning(warning);
            });
        });
    }

    void CheckRewardWarning()
    {
        for(int i = 0; i < 8; i++)
        {
            if(CollectionList.transform.GetChild(i).GetChild(2).GetComponent<Image>().color == new Color(0,1,0)) break;
            else if(i == 7) CollectionBtn.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void PlayAnimationGold(GameObject fisrtTarget)
    {
        Vector3 fistPos = fisrtTarget.transform.position;
        Vector3 targetPos = GoldTxt.transform.parent.GetChild(0).position;

        int gold = (int)saveDataJson.GetData("Gold");
        int num = 40;
        int ListGoldLength = ListGold.transform.childCount;
        int total = (int)saveDataJson.GetData("Gold") + 200;
        saveDataJson.SaveData("Gold", total);
        saveDataJson.SaveData("CollecionReward", fisrtTarget.transform.parent.parent.GetSiblingIndex() + 1);
        Debug.Log(fisrtTarget.transform.parent.parent.GetSiblingIndex() + 1);
        adsManager.LogEvent($"CollecionReward_{fisrtTarget.transform.parent.parent.GetSiblingIndex() + 1}");

        fisrtTarget.transform.parent.GetComponent<Button>().enabled = false;
        fisrtTarget.transform.parent.GetComponent<Image>().color = new Color(1,1,1);
        fisrtTarget.transform.parent.Find("tick").gameObject.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            Transform child;
            if(ListGoldLength - 1 < i) {
                child = Instantiate(ListGold.transform.GetChild(0) , Vector3.zero, Quaternion.identity);
                child.transform.SetParent(ListGold.transform);
                child.transform.localScale = new Vector3(1f,1f,1);
            } else child = ListGold.transform.GetChild(i);
            child.GetComponent<Image>().sprite = GoldTxt.transform.parent.GetChild(0).GetComponent<Image>().sprite;

            child.position = fistPos;
            child.gameObject.SetActive(true);
            audioManager.PlaySFX("coin_appear");

            child.DOMove(targetPos, 2f).SetEase(Ease.InOutBack).OnComplete(() => {
                gold += num;
                child.gameObject.SetActive(false); 
                GoldTxt.GetComponent<TextMeshProUGUI>().text = $"{gold}";
            }).SetDelay(0.1f*i);
        }
        goldInSmallShop.text = $"{total}";
        goldInOpenGift.text = $"{total}";
        CheckRewardWarning();
    }

    void CreateMapList()
    {
        int count = 0;
        int num = controlImage.ListMapImage.Length;
        int limit = num % 2 == 0 ? num / 2 : (num / 2) + 1;
        float xx = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x / 4;
        xx = xx - (xx - FirstMap.GetComponent<RectTransform>().sizeDelta.x / 2)/2;
        int openedMap = (int)saveDataJson.GetData("OpenedMap");
        bool isOpenAllMap = saveDataJson.GetData("OpenAllMaps") != null ? (bool)saveDataJson.GetData("OpenAllMaps") : false;
        int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");

        ListMap.GetComponent<RectTransform>().sizeDelta = new Vector2(ListMap.GetComponent<RectTransform>().sizeDelta.x, 400 * limit);
        for (int i = 0; i < limit; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                count++;
                int val = j == 1 ? 1 : -1;
                GameObject child = Instantiate(FirstMap , Vector3.zero, Quaternion.identity);
                child.transform.SetParent(ListMap.transform);
                child.transform.localScale = new Vector3(1f,1f,1);
                child.GetComponent<RectTransform>().anchoredPosition = new Vector2(xx * val, -186.5f - i * 400);
                child.name = $"{count}";

                if(count == num)
                {
                    child.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListMapImage, $"Fr_Coming");
                    child.transform.GetChild(1).gameObject.SetActive(false);
                    child.transform.GetChild(2).gameObject.SetActive(false);
                    child.transform.GetChild(5).gameObject.SetActive(true);
                    break;
                }
                else
                {
                    child.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListMapImage, $"Map{count}");
                    child.transform.GetChild(2).GetChild(0).GetComponent<LocalizeStringEvent>()
                        .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(MapData.map[count].AreaName);

                    if (count <= openedMap || isOpenAllMap || (listOpenedMap != null && listOpenedMap.Contains(count)))
                    {
                        child.transform.GetChild(1).gameObject.SetActive(false);
                        child.transform.GetChild(3).gameObject.SetActive(true);

                        if (count < openedMap || (listOpenedMap != null && listOpenedMap.Contains(count))) child.transform.GetChild(4).gameObject.SetActive(true);
                        else
                        {
                            SetProgressMap(child.transform.GetChild(0), count);
                            Transform Btn = child.transform.GetChild(3);
                            Btn.DOScale(new Vector3 (110, 110, 1), 0.5f).SetLoops(-1, LoopType.Yoyo);
                        }
                    }
                }
            }
        }

        FirstMap.SetActive(false);
    }

    void SetProgressMap(Transform obj, int map)
    {
        string[] itemsFinded = (string[])saveDataJson.GetData($"ItemMap{map}");
        int itemsFindedValue = itemsFinded != null ? itemsFinded.Length : 0;
        int[] milestones = (int[])saveDataJson.TakeMapData().map[map].Milestones;

        obj.gameObject.SetActive(true);
        if (itemsFindedValue > 0)
        {
            int count = 0;

            for (int i = 0; i < milestones.Length; i++)
            {
                if (milestones[i] <= itemsFindedValue) count++;
                else break;
            }
            if (count == 4) obj.gameObject.SetActive(false);
            else
            {
                if (count == 0) count++;
                obj.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{25 * (count + 1)}%";
                for (int i = 0; i <= count; i++)
                {
                    obj.GetChild(i).gameObject.SetActive(false);
                }
            }
            
        }
        else obj.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"0%";
    }

    public void CheckUnlockMap()
    {
        int num = ListMap.transform.childCount - 1;
        int openedMap = (int)saveDataJson.GetData("OpenedMap");
        bool isOpenAllMap = (bool)saveDataJson.GetData("OpenAllMaps");
        int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");
        for(int i = 1; i < num; i++)
        {
            if (i <= openedMap || isOpenAllMap || listOpenedMap.Contains(i))
            {
                Transform child = ListMap.transform.GetChild(i);
                child.GetChild(1).gameObject.SetActive(false);
                child.GetChild(3).gameObject.SetActive(true);

                if (i < openedMap || listOpenedMap.Contains(i))
                {
                    child.transform.GetChild(4).gameObject.SetActive(true);
                    child.GetChild(0).gameObject.SetActive(false);
                    Transform Btn = child.transform.GetChild(3);
                    Btn.DOKill();
                    Btn.localScale = new Vector3(100, 100, 1);
                }
                else
                {
                    SetProgressMap(child.GetChild(0), i);
                    Transform Btn = child.transform.GetChild(3);
                    Btn.DOScale(new Vector3 (110, 110, 1), 0.5f).SetLoops(-1, LoopType.Yoyo);
                }
            }
        }
    }

    void CreateMapSpecialList()
    {
        int count = 0;
        int num = controlImage.ListMapSpecialImage.Length;
        int limit = num % 2 == 0 ? num / 2 : (num / 2) + 1;
        float xx = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x / 4;
        xx = xx - (xx - FirstSpecialMap.GetComponent<RectTransform>().sizeDelta.x / 2)/2;

        int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedSpecialMap");

        ListSpecialMap.GetComponent<RectTransform>().sizeDelta = new Vector2(ListSpecialMap.GetComponent<RectTransform>().sizeDelta.x, 400 * limit);
        for (int i = 0; i < limit; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                count++;
                int val = j == 1 ? 1 : -1;
                GameObject child = Instantiate(FirstSpecialMap , Vector3.zero, Quaternion.identity);
                child.transform.SetParent(ListSpecialMap.transform);
                child.transform.localScale = new Vector3(1f,1f,1);
                child.GetComponent<RectTransform>().anchoredPosition = new Vector2(xx * val, -186.5f - i * 400);
                child.name = $"{count}";

                if(count == num)
                {
                    child.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListMapSpecialImage, $"Fr_Coming");
                    child.transform.GetChild(0).gameObject.SetActive(false);
                    child.transform.GetChild(1).gameObject.SetActive(false);
                    child.transform.GetChild(4).gameObject.SetActive(true);
                    break;
                }
                else
                {
                    child.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListMapSpecialImage, $"SpecialMap{count}");
                    child.transform.GetChild(1).GetChild(0).GetComponent<LocalizeStringEvent>()
                        .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(saveDataJson.TakeGameSpecialData().SpecialMap[count].AreaName);

                    if(count == 1) child.transform.GetChild(5).gameObject.SetActive(true);
                    
                    if(listOpenedMap != null && listOpenedMap.Contains(count)){
                        child.transform.GetChild(0).gameObject.SetActive(false);
                        child.transform.GetChild(2).gameObject.SetActive(true);
                        if(count == 1) child.transform.GetChild(5).gameObject.SetActive(false);
                        // if(listOpenedMap != null && listOpenedMap.Contains(count)) child.transform.GetChild(3).gameObject.SetActive(true);
                    }
                }
            }
        }

        FirstSpecialMap.SetActive(false);
    }

    public void CheckUnlockSpecialMap()
    {
        int num = ListSpecialMap.transform.childCount;
        int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedSpecialMap");
        for(int i = 1; i < num; i++)
        {
            if(listOpenedMap.Contains(i)) {
                Transform child = ListSpecialMap.transform.GetChild(i);
                child.GetChild(0).gameObject.SetActive(false);
                child.GetChild(2).gameObject.SetActive(true);

                if(listOpenedMap.Contains(i)) child.transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    public void ChangeMainPage(Button btn)
    {
        if(!enabledToClick) return;
        if(btn.gameObject.transform.GetChild(0).gameObject.activeSelf) return;
        audioManager.PlaySFX("click");
        foreach(Transform child in menu.transform)
        {
            if(child.name == "CurrentPage" || child.name == "BtnSUB" || child.name == "BtnSale") continue;
            if(child.GetChild(0).gameObject.activeSelf){
                child.GetChild(0).gameObject.SetActive(false);
                child.GetChild(1).gameObject.SetActive(true);
                // child.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(208,149,59);
                break;
            }
        }

        menu.transform.GetChild(0).DOPause();
            btn.transform.GetChild(0).gameObject.SetActive(true);
            btn.transform.GetChild(1).gameObject.SetActive(false);
        // menu.transform.GetChild(0).DOMove(btn.transform.position, 0.3f).SetEase(Ease.InOutCubic).OnComplete(() =>{
            // btn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(255,255,255);
        // });

        float targetX = 0;
        switch(btn.name){
            case "CollectionBtn":
                targetX = body.transform.GetChild(0).position.x;
                break;
            case "ShopBtn":
                targetX = body.transform.GetChild(1).position.x;
                break;
            case "HomeBtn":
                targetX = body.transform.GetChild(2).position.x;
                break;
            case "ChallengeBtn":
                targetX = body.transform.GetChild(3).position.x;
                break;
            case "SpecialBtn":
                targetX = body.transform.GetChild(4).position.x;
                break;
            default:
                break;
        }

        foreach(Transform child in body.transform)
        {
            child.transform.DOPause();
            child.transform.DOMove(new Vector3(child.transform.position.x - targetX, child.transform.position.y, child.transform.position.z), 0.3f).SetEase(Ease.InOutCubic);
        }


        // float targetX = 0;
        // switch(btn.name){
        //     case "ShopBtn":
        //         targetX = 6.63f;
        //         break;
        //     case "Homebtn":
        //         targetX = 0;
        //         break;
        //     case "Challenge":
        //         targetX = -6.63f;
        //         break;
        //     default:
        //         break;
        // }

        // body.transform.DOPause();
        // body.transform.DOMove(new Vector3(targetX, body.transform.position.y, body.transform.position.z), 0.3f);
    }

    public void PlayChallenge(Button btn)
    {
        if(!enabledToClick) return;
        TurnOffBtn();
        audioManager.PlaySFX("click");
        ChallengeList.SetActive(true);
        ChallengeList.GetComponent<ChallengeList>().SetValue(btn.name);
        // if((int)saveDataJson.GetData("OpenedMap") > 1)
        // {
        //     // cloudAnimation.PlayChangeScreenAnimation("Challenge");
        //     // challengeName = btn.name;
        //     ChallengeList.SetActive(true);
        //     ChallengeList.GetComponent<ChallengeList>().SetValue(btn.name);
        // }
        // else
        // {
        //     exitChallengeDialog.gameObject.SetActive(true);
        //     exitChallengeDialog.SetMessage("Notyet");
        // }
    }

    public void ShowHintToOpenChallenge(int num)
    {
        exitChallengeDialog.gameObject.SetActive(true);
        exitChallengeDialog.SetMessage("Notyet", num);
    }

    public void TurnOffBtn()
    {
        buttonControl.OffBtn();
        enabledToClick = false;
    }

    public void TurnOnBtn()
    {
        buttonControl.OnBtn();
        enabledToClick = true;
    }

    public void ToOpenMap()
    {
        audioManager.PlaySFX("click");
        InfoToOpenMap.SetActive(true);
        Image MessageImage = InfoToOpenMap.GetComponent<Image>();
        TextMeshProUGUI MessageText = InfoToOpenMap.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        MessageImage.DOPause();
        MessageText.DOPause();

        MessageImage.DOFade(0.83f, 0.5f);
        MessageText.DOFade(1f, 0.5f);
        MessageImage.DOFade(0f, 0.5f).SetDelay(2f);
        MessageText.DOFade(0f, 0.5f).SetDelay(2f).OnComplete(() => {InfoToOpenMap.SetActive(false);});
    }

    public void PlaySpecialGame(Button btn)
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        game.SetActive(true);
        game.GetComponent<SpecialGame>().LoadGame(int.Parse(btn.name));

    }

    public void OpenSpecialMap (int mapNum)
    {
        Transform map = ListSpecialMap.transform.GetChild(mapNum);
        map.GetChild(0).gameObject.SetActive(false);
        map.GetChild(2).gameObject.SetActive(true);
        // map.GetChild(3).gameObject.SetActive(true);

        if(mapNum == 1) map.GetChild(5).gameObject.SetActive(false);        
    }

    public void OpenWeeklyReward()
    {
        if(!enabledToClick) return;
        TurnOffBtn();
        audioManager.PlaySFX("click");
        WeeklyReward.GetComponent<WeeklyReward>().PlayInAnimation();
    }
}
