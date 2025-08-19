// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using System.Globalization;
using UnityEngine.Localization.Settings;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using System.Linq;
using System;

public class CompleteChallenge : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public ButtonControl buttonControl;
    public SaveDataJson saveDataJson;
    public GameObject MapImage;
    public GameObject game;
    public GameObject gameScreen;
    public GameObject challengeList;
    // public List<Sprite> ListImage;
    public ControlImageFromResources controlImage;

    private string challengeName;
    private int challengeLevel;


   public GameObject slogan;
    public GameObject Mess;
    private bool enabledToClick = true;
    public GameObject homeBtn;
    public GameObject continueBtn;
    public GameObject ChallengeList;

    Vector2 currentContinuePosition;
    Vector2 currentHomePosition;
    Vector2 currentSloganPosition;

    public ChangeScreenAnimation cloudAnimation;
    public SkeletonGraphic FireAnim;
    public void HomeBtn()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
        buttonControl.ReturnToHome();
        ChallengeList.SetActive(true);
    }

    public void Continue()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        audioManager.PlaySFX("click");
        cloudAnimation.PlayChangeScreenAnimation("ChallengeComplete");

    }

    public void ShowChallenge()
    {
        game.SetActive(true);
        gameObject.SetActive(false);

        gameScreen.SetActive(true);
        game.GetComponent<Game>().enabled = false;
        game.GetComponent<ChallengeMode>().enabled = true;
        game.GetComponent<ChallengeMode>().LoadChallenge(challengeName, challengeLevel);
    }

    // public void SetImage(int currentMap, string id, string mess)
    // {
    //     enabledToClick = false;
    //     challengeName = id;
    //     challengeLevel = currentMap;

    //     ChangeSlogan();
 
    //     if(mess == "open all maps"){
    //         challengeLevel--;
    //         MapImage.transform.GetChild(0).gameObject.SetActive(false);
    //         MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{currentMap}");
    //         Mess.GetComponent<LocalizeStringEvent>()
    //             .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Openallmaps");
    //         SetUpAnimation();
    //     }else if(mess == "new map"){
    //         MapImage.transform.GetChild(0).gameObject.SetActive(true);
    //         MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{currentMap + 1}");
    //         Mess.GetComponent<LocalizeStringEvent>()
    //             .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Newchallenge");
    //         SetUpAnimation();
    //     }else if(mess == "play again")
    //     {
    //         MapImage.transform.GetChild(0).gameObject.SetActive(false);
    //         MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{currentMap + 1}");
    //         Mess.GetComponent<LocalizeStringEvent>()
    //             .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Playagain");
    //         SetUpAnimation();
    //     }
    // }

    public void SetImage(SaveDataJson.Challenge ChallengeData)
    {
        enabledToClick = false;
        challengeName = ChallengeData.challengeID;
        challengeLevel = ChallengeData.level;

        adsManager.LogEvent($"{challengeName}_{ChallengeData.level}");
        saveDataJson.SaveData($"List{challengeName}", ChallengeData.level);
        SaveDataJson.Challenge[] challengeListData = saveDataJson.TakeChallengeData().ChallengeMap;
        int currentChallenge = (int)saveDataJson.GetData(challengeName);
        int countMap = 0;
        int challengeListDataLength = challengeListData.Length;
        SaveDataJson.Challenge[] listMap = {};
        for(int i = 0; i < challengeListDataLength; i++)
        {
            if(challengeListData[i].challengeID == challengeName)
            { 
                countMap++;
                int length = listMap.Length;
                Array.Resize(ref listMap, length + 1);
                listMap[length] = challengeListData[i];
            }
        }

        if(ChallengeData.level == countMap)
        {
            // hoàn thành toàn bộ map
            saveDataJson.SaveData(challengeName, ChallengeData.level + 1);
            challengeList.GetComponent<ChallengeList>().OpenNewMap(ChallengeData.level - 1);
            MapImage.transform.GetChild(0).gameObject.SetActive(false);
            MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{challengeLevel}");
            Mess.GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Openallmaps");
            SetUpAnimation();
        }
        else if(ChallengeData.level > currentChallenge)
        {
            if((bool)saveDataJson.GetData("LegendarySUB") || (saveDataJson.GetData("VIP3Day") != null && (string)saveDataJson.GetData("VIP3Day") != ""))
            {
                challengeLevel++;
                MapImage.transform.GetChild(0).gameObject.SetActive(false);
                MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{listMap[challengeLevel - 1].map}");
                Mess.GetComponent<LocalizeStringEvent>()
                    .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Playagain");
                challengeList.GetComponent<ChallengeList>().OpenNewMap(ChallengeData.level - 1);
                SetUpAnimation();
            }
            else
            {
                int[] listOpenedMap = (int[]) saveDataJson.GetData($"List{challengeName}");
                if(listOpenedMap.Contains(ChallengeData.level + 1))
                {
                    challengeLevel++;
                    MapImage.transform.GetChild(0).gameObject.SetActive(false);
                    MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{listMap[challengeLevel - 1].map}");
                    Mess.GetComponent<LocalizeStringEvent>()
                        .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Playagain");
                    // challengeList.GetComponent<ChallengeList>().OpenNewMap(ChallengeData.level - 1);
                    SetUpAnimation();
                }else
                {
                    challengeLevel = (int)saveDataJson.GetData($"{challengeName}");
                    MapImage.transform.GetChild(0).gameObject.SetActive(false);
                    MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{listMap[challengeLevel - 1].map}");
                    Mess.GetComponent<LocalizeStringEvent>()
                        .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Playalltoopen");
                    // challengeList.GetComponent<ChallengeList>().OpenNewMap(ChallengeData.level - 1);
                    SetUpAnimation();
                }
            }
        }
        else if(ChallengeData.level == currentChallenge)
        {
            // mở map mới
            int[] listOpenedMap = (int[]) saveDataJson.GetData($"List{challengeName}");
            int mapOpened = currentChallenge;
            int listMapLength = listMap.Length;
            for(int i = mapOpened + 1; i <= listMapLength; i++)
            {
                if(!listOpenedMap.Contains(i))
                {
                    mapOpened = i;
                    break;
                } else if (i == listMapLength) mapOpened = ChallengeData.level + 1;
            }
            
            challengeLevel = mapOpened;
            saveDataJson.SaveData(challengeName, mapOpened);
            challengeList.GetComponent<ChallengeList>().OpenNewMap(mapOpened - 1, ChallengeData.level - 1);

            MapImage.transform.GetChild(0).gameObject.SetActive(true);
            MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{listMap[mapOpened-1].map}");
            Mess.GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Newchallenge");
            SetUpAnimation();
        }
        else if(ChallengeData.level < currentChallenge)
        {
            // chơi lại map cũ
            challengeLevel++;
            MapImage.transform.GetChild(0).gameObject.SetActive(false);
            MapImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{listMap[challengeLevel - 1].map}");
            Mess.GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Playagain");
            SetUpAnimation();
        }
        
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
        continueBtn.GetComponent<RectTransform>().DOAnchorPos(currentContinuePosition, 1f).SetEase(Ease.OutCubic).SetDelay(1f);
        homeBtn.GetComponent<RectTransform>().DOAnchorPos(currentHomePosition, 1f).SetEase(Ease.OutCubic).SetDelay(1f).OnComplete(() => {
            enabledToClick = true;
            adsManager.ShowInterstitialAd();
        });
    }
}
