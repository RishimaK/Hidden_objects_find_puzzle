using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using System.Globalization;
using Spine.Unity;
using DG.Tweening;
using System.Collections;
using System.Linq;
public class Complete : MonoBehaviour
{
    public ButtonControl buttonControl;
    public AudioManager audioManager;
    public AdsManager adsManager;
    public Game game;
    public Home Home;
    public GameObject MapImage;
    public SaveDataJson saveDataJson;
    public ChangeScreenAnimation cloudAnimation;

    public GameObject slogan;
    public GameObject Mess;
    public GameObject homeBtn;
    public GameObject continueBtn;
    public GameObject RemoveAds;
    public GameObject UnlockNewChallenge;
    public ExitChallengeDialog exitChallengeDialog;

    public WeeklyReward weeklyReward;

    private int mapUnlockChallenge;
    private int continueMap;

    // public List<Sprite> ListImage;
    public ControlImageFromResources controlImage;
    private bool enabledToClick = true;

    Vector2 currentContinuePosition;
    Vector2 currentHomePosition;
    Vector2 currentSloganPosition;

    public SkeletonGraphic FireAnim;

    private bool openChallenge = false;
    private bool isSpecial = false;
    public void HomeBtn()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        enabledToClick = false;
        gameObject.SetActive(false);
        buttonControl.ReturnToHome();
    }

    public void Continue()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        enabledToClick = false;
        game.gameObject.SetActive(true);
        // gameObject.SetActive(false);
        if(isSpecial)
        {
            game.GetComponent<SpecialGame>().LoadGame(continueMap);
        }
        else
        {
            int openMap = continueMap;
            if(openMap == saveDataJson.TakeMapData().map.Length) openMap--;
            game.LoadGame(openMap);
        }
    }

    public void SetImage(int val, string txt = "")
    {
        enabledToClick = false;
        isSpecial = false;
        if(txt == "specialGame")
        {
            MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapSpecialImage, $"Fr_SpcMap{val}");
            Mess.GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Complete Special Map");
                MapImage.transform.GetChild(0).gameObject.SetActive(false);
            continueMap = val;
            isSpecial = true;
            SetUpAnimation();
            return;
        }
        saveDataJson.SaveData("ListOpenedMap", val);
        int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");
        int numberOfMaps = controlImage.ListCompletedMapImage.Length;

        if(val >= numberOfMaps)
        {
            // hoàn thành map cuối
            continueMap = numberOfMaps;
            MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{numberOfMaps}");
            Mess.GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Playagain");
            MapImage.transform.GetChild(0).gameObject.SetActive(false);
            // Debug.Log(val);
            // Debug.Log((int)saveDataJson.GetData("OpenedMap"));
            if((int)saveDataJson.GetData("OpenedMap") == val) adsManager.LogEvent($"Map_{val}");
            else adsManager.LogEvent($"Play_Again_Map{val}");
            saveDataJson.SaveData("OpenedMap", val + 1);
            SetUpAnimation();
        }
        else if(val > (int)saveDataJson.GetData("OpenedMap"))
        {
            // SUB
            MapImage.transform.GetChild(0).gameObject.SetActive(false);
            if((bool)saveDataJson.GetData("LegendarySUB") || (saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != ""))
            {
                continueMap = val + 1;
                MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{val + 1}");
                Mess.GetComponent<LocalizeStringEvent>()
                    .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Nextmap");
            }
            else
            {
                // int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");
                if(listOpenedMap.Contains(val + 1))
                {
                    continueMap = val + 1;
                    MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{val + 1}");
                    Mess.GetComponent<LocalizeStringEvent>()
                        .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Nextmap");
                }else
                {
                    continueMap = (int)saveDataJson.GetData("OpenedMap");
                    MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{continueMap}");
                    Mess.GetComponent<LocalizeStringEvent>()
                        .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Playalltoopen");
                }
            }
            adsManager.LogEvent($"Map_{val}");
            Home.CheckUnlockMap();
            SetUpAnimation();
        }
        else if(val < (int)saveDataJson.GetData("OpenedMap"))
        {
            // hoàn thành map đã hoàn thành
            continueMap = val + 1;
            MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{val + 1}");
            MapImage.transform.GetChild(0).gameObject.SetActive(false);
            Mess.GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Nextmap");
            SetUpAnimation();
            adsManager.LogEvent($"Play_Again_Map{val}");
        } 
        else if(val == (int)saveDataJson.GetData("OpenedMap"))
        {
            //hoàn thành map mới nhất đã mở
            openChallenge = true;
            mapUnlockChallenge = val;
            MapImage.transform.GetChild(0).gameObject.SetActive(true);
            Mess.GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Newmap");

            // int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");
            int mapOpened = val;
            for(int i = mapOpened + 1; i <= numberOfMaps; i++)
            {
                if(!listOpenedMap.Contains(i))
                {
                    mapOpened = i;
                    break;
                }
            }
            continueMap = mapOpened;
            MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{mapOpened}");

            saveDataJson.SaveData("OpenedMap", mapOpened);
            adsManager.LogEvent($"Map_{val}");
            Home.CheckUnlockMap();
            SetUpAnimation();
            // if((int)saveDataJson.GetData("OpenedMap") > 3 || (listOpenedMap != null && listOpenedMap.Contains(3)))
            // {
            //     adsManager.LoadBannerAd();
            // }
        }

        // int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");
    }

    void PlayFireWork()
    {
        FireAnim.gameObject.SetActive(true);
        FireAnim.AnimationState.ClearTrack(0);
        FireAnim.Skeleton.UpdateWorldTransform();
        FireAnim.AnimationState.SetAnimation(0, "animation", false);
        Invoke("SetActiveFireWork", 1.267f);
    }

    void SetActiveFireWork()
    {
        FireAnim.gameObject.SetActive(false);
    }

    void WaitforScreenAnimation(){
        if(cloudAnimation.gameObject.activeSelf) Invoke("WaitforScreenAnimation", Time.deltaTime);
        else 
        {
            audioManager.PlaySFX("level_completed");
            Invoke("PlayFireWork", 1.1f);

            if(MapImage.transform.GetChild(0).gameObject.activeSelf){
                SkeletonGraphic skeleton = MapImage.transform.GetChild(0).GetComponent<SkeletonGraphic>();
                skeleton.AnimationState.ClearTrack(0);
                skeleton.Skeleton.UpdateWorldTransform();
                skeleton.AnimationState.SetAnimation(0, "animation", false);
                StartCoroutine(PlayCompleteAnimation(2.767f));
            }else StartCoroutine(PlayCompleteAnimation(0));
        }
    }

    IEnumerator PlayCompleteAnimation(float time)
    {
        yield return new WaitForSeconds(time);

        slogan.GetComponent<RectTransform>().DOAnchorPos(currentSloganPosition, 1f).SetEase(Ease.OutBounce);
        continueBtn.GetComponent<RectTransform>().DOAnchorPos(currentContinuePosition, 1f).SetEase(Ease.OutCubic).SetDelay(1);
        homeBtn.GetComponent<RectTransform>().DOAnchorPos(currentHomePosition, 1f).SetEase(Ease.OutCubic).SetDelay(1).OnComplete(() => {
            enabledToClick = true;
            adsManager.ShowInterstitialAd();

            if(openChallenge)
            {
                if((int)saveDataJson.GetData("OpenedMap") == 2 &&!(bool)saveDataJson.GetData("RemoveAds")) RemoveAds.SetActive(true);
                else OpenChallengeDialog();
            }
        });
    }

    public void OpenChallengeDialog()
    {
        openChallenge = false;
        if((int)saveDataJson.GetData("OpenedMap") == 3) 
        {
            HomeBtn();
            weeklyReward.PlayTutorial();
            return;
        }
        UnlockNewChallenge.GetComponent<UnlockNewChallenge>().SetImage(mapUnlockChallenge);
    }

    void SetUpAnimation()
    {
        RectTransform continueRect = continueBtn.GetComponent<RectTransform>();
        RectTransform homeRect = homeBtn.GetComponent<RectTransform>();
        RectTransform sloganRect = slogan.GetComponent<RectTransform>();
        
        currentContinuePosition = continueRect.anchoredPosition;
        currentHomePosition = homeRect.anchoredPosition;
        currentSloganPosition = sloganRect.anchoredPosition;

        float _width = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;
        float _height = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y;
        
        continueRect.anchoredPosition = new Vector2(_width / 2 + continueRect.sizeDelta.x / 2, continueRect.anchoredPosition.y);
        homeRect.anchoredPosition = new Vector2(-(_width / 2 + homeRect.sizeDelta.x / 2), homeRect.anchoredPosition.y);
        sloganRect.anchoredPosition = new Vector2(sloganRect.anchoredPosition.x, _height / 2 + sloganRect.sizeDelta.y / 2);
        WaitforScreenAnimation();
    }
}
