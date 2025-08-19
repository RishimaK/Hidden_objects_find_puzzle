using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization.Settings;

public class Setting : MonoBehaviour
{
    public AudioManager audioManager;
    public AdsManager adsManager;
    public SaveDataJson saveDataJson;

    public GameObject languageDilog;
    public GameObject pick;
    public GameObject listLanguage;
    public GameObject musicStatus;
    public GameObject soundStatus;
    public GameObject vibrationStatus;
    // public GameObject  exitLanguageDilog;
    private bool enabledToClick = true;


    IEnumerator WaitFunction(float time)
    {
        enabledToClick = false;
        yield return new WaitForSeconds(time);
        enabledToClick = true;
    }

    public void OpenLanguageDiaog()
    {
        audioManager.PlaySFX("click");
        languageDilog.SetActive(true);
    }

    public void closeLanguageDiaog()
    {
        audioManager.PlaySFX("click");
        languageDilog.SetActive(false);
    }

    public void ChangLanguage(Button btn)
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        pick.transform.position = btn.transform.position;
        int idLanguage = TakeIdLanguage(btn.name);

        StartCoroutine(SetLocale(idLanguage));
    }

    public int TakeIdLanguage (string txt)
    {
        int idLanguage = 0;
        switch (txt)
        {
            case "EN" :
                idLanguage = 0;
                break;
            case "FR" :
                idLanguage = 1;
                break;
            case "DE" :
                idLanguage = 2;
                break;
            case "JA" :
                idLanguage = 3;
                break;
            case "VI" :
                idLanguage = 4;
                break;
            default :
                idLanguage = 0;
                break;            
        }
        saveDataJson.SaveData("Language", txt);
        return idLanguage;
    }

    IEnumerator SetLocale (int id)
    {
        enabledToClick = false;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        enabledToClick = true;
    }

    public void ChangeMusic()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        bool status = ChangeState(musicStatus);
        // PlayerPrefs.SetString("Music", $"{status}");
        saveDataJson.SaveData("Music", status);
        audioManager.ChangeStatusOfMusic(status);
    }

    public void ChangeSound()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        bool status = ChangeState(soundStatus);
        saveDataJson.SaveData("Sound", status);
        audioManager.ChangeStatusOfSound(status);
    }

    public void ChangeVibration()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        bool status = ChangeState(vibrationStatus);
        saveDataJson.SaveData("Vibration", status);
        adsManager.ChangeStatusOfVibration(status);
    }

    bool ChangeState(GameObject e)
    {
        StartCoroutine(WaitFunction(0.2f));

        GameObject off = e.transform.GetChild(0).gameObject;
        GameObject on = e.transform.GetChild(1).gameObject;
        GameObject btn = e.transform.GetChild(2).gameObject;

        int i = off.activeSelf ? 1: -1;
        // .DOAnchorPos(currentContinuePosition, 1f)
        RectTransform btnRect = btn.GetComponent<RectTransform>();
        btnRect.DOAnchorPos(new Vector2(i * 27, btnRect.anchoredPosition.y), 0.2f).OnComplete(() => {
            off.SetActive(!off.activeSelf);
            on.SetActive(!on.activeSelf);
        });

        // btn.transform.DOMove(new Vector3(btn.transform.position.x + i * 0.45f, btn.transform.position.y, btn.transform.position.z), 0.2f).OnComplete(() => {
        //     off.SetActive(!off.activeSelf);
        //     on.SetActive(!on.activeSelf);
        // });

        return !e.transform.GetChild(1).gameObject.activeSelf;
    }

    void Start() 
    {
        StartCoroutine(WaitToStart(0.4f));
    }

    IEnumerator WaitToStart(float time)
    {
        yield return new WaitForSeconds(time);
        SetValue();
    }

    void SetValue()
    {
        PlayerData data = saveDataJson.GetData();
        
        int idLanguage = TakeIdLanguage((string)saveDataJson.GetData("Language"));
        pick.transform.position = listLanguage.transform.GetChild(idLanguage + 1).position;
        listLanguage.GetComponent<RectTransform>().anchoredPosition = new Vector3
        (
            0,
            y:Mathf.Clamp(-205 - pick.GetComponent<RectTransform>().anchoredPosition.y, 0, 110),
            0
        );

        if(!data.Music) ChangeState(musicStatus);
        if(!data.Sound) ChangeState(soundStatus);
        if(!data.Vibration) ChangeState(vibrationStatus);
    }
}
