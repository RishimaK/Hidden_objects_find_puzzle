using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using System.Globalization;
using System.Linq;

public class ChallengeList : MonoBehaviour
{
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public GameObject ChallengeMapList;
    public ChangeScreenAnimation cloudAnimation;

    public LocalizeStringEvent Name;
    public ControlImageFromResources controlImage;
    private string challengeName;
    private int challengeLevel;
    private bool enabledToClick = true;

    public GameObject Home;
    public GameObject Game;
    public GameObject GameScreen;

    public void SetValue(string val)
    {
        enabledToClick = false;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(-gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 0);
        StartCoroutine(ChangeNameArea(val));
        SetMapList(val);

        rect.DOAnchorPos(new Vector2(0, 0), 0.5f).SetEase(Ease.OutCubic).OnComplete(() => {
            enabledToClick = true;
            Home.GetComponent<Home>().TurnOnBtn();
        });
    }

    IEnumerator ChangeNameArea (string txt)
    {
        yield return LocalizationSettings.InitializationOperation;
        Name.StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt);
    }

    void SetMapList(string val)
    {
        challengeName = val;
        SaveDataJson.Challenge[] challengeList = saveDataJson.TakeChallengeData().ChallengeMap;
        int challengeListLength = challengeList.Length;
        int ChallengeMapListLength = ChallengeMapList.transform.childCount;
        int openedMap = (int)saveDataJson.GetData(val);

        Transform map;
        int level;
        RectTransform rect;
        int mapNumber = 0;
        bool isOpenAllMap = saveDataJson.GetData("OpenAllMaps") != null ? (bool)saveDataJson.GetData("OpenAllMaps") : false;

        int[] listOpenedMap = (int[]) saveDataJson.GetData($"List{val}");
        for(int i = 0; i < challengeListLength; i++)
        {
            SaveDataJson.Challenge child = challengeList[i];
            if(child.challengeID != val) continue;
            level = child.level;
            mapNumber++;
            if(ChallengeMapListLength >= level) map = ChallengeMapList.transform.GetChild(level - 1);
            else
            {
                map = Instantiate(ChallengeMapList.transform.GetChild(0), Vector3.zero, Quaternion.identity);
                map.SetParent(ChallengeMapList.transform);

                rect = map.GetComponent<RectTransform>();
                rect.DOPause();
                rect.localScale = new Vector3(1,1,1);
                rect.localPosition = new Vector2(355, -112 - 260 * (level - 1));
            }
            map.gameObject.SetActive(true);
            map.name = $"Map{level}";
    
            map.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListChallengeMapImage, $"Map{child.map}_Challenge");

            if(level <= openedMap || isOpenAllMap || (listOpenedMap != null && listOpenedMap.Contains(mapNumber))) 
            {   
                map.GetChild(0).gameObject.SetActive(false);
                if(mapNumber < openedMap || (listOpenedMap != null && listOpenedMap.Contains(mapNumber))) map.GetChild(1).gameObject.SetActive(true);
                else map.GetChild(1).gameObject.SetActive(false);
            }
            else 
            {
                map.GetChild(0).gameObject.SetActive(true);
                map.GetChild(1).gameObject.SetActive(false);
            }
        }
        // Debug.Log(mapNumber);
        ChallengeMapList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 260 * mapNumber);
        ChallengeMapList.GetComponent<RectTransform>().localPosition = Vector2.zero;
        // Debug.Log(112 + 250 * mapNumber);
    }

    void HideMapList()
    {
        int ChallengeMapListLength = ChallengeMapList.transform.childCount;
        for(int i = 0; i < ChallengeMapListLength; i++)
        {
            ChallengeMapList.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Exit()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        audioManager.PlaySFX("click");
        Home.GetComponent<Home>().TurnOffBtn();
        gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 0), 0.5f).SetEase(Ease.InCubic).OnComplete(() => {
            HideMapList();
            gameObject.SetActive(false);
            Home.GetComponent<Home>().TurnOnBtn();
        });
    }

    public void PlayChallenge(Button btn)
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        challengeLevel = Convert.ToInt32(btn.name.Split("ap")[1]);
        cloudAnimation.PlayChangeScreenAnimation("Challenge");
    }

    public void ShowChallenge()
    {
        gameObject.SetActive(false);
        Home.SetActive(false);
        Game.SetActive(true);
        GameScreen.SetActive(true);
        Game.GetComponent<Game>().enabled = false;
        Game.GetComponent<ChallengeMode>().enabled = true;
        Game.GetComponent<ChallengeMode>().LoadChallenge(challengeName, challengeLevel);
    }

    public void OpenNewMap(int map, int currentChallenge)
    {
        ChallengeMapList.transform.GetChild(map).GetChild(0).gameObject.SetActive(false);
        // ChallengeMapList.transform.GetChild(map).GetChild(1).gameObject.SetActive(false);
        ChallengeMapList.transform.GetChild(currentChallenge).GetChild(1).gameObject.SetActive(true);
    }

    public void OpenNewMap (int currentChallenge)
    {
        ChallengeMapList.transform.GetChild(currentChallenge).GetChild(1).gameObject.SetActive(true);
    }
}
