// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.Localization.Settings;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using DG.Tweening;
// using UnityEngine.UIElements;

public class ChallengeDialog : MonoBehaviour
{
    public AdsManager adsManager;
    public ButtonControl buttonControl;
    public AudioManager audioManager;
    public ChallengeMode game;
    public Image areaImage;
    public CompleteChallenge completeChallenge;
    public GameObject challengeName;
    public GameObject status;
    public GameObject messenger;
    public GameObject playBtn;
    public ChangeScreenAnimation cloudAnimation;
    public ControlImageFromResources controlImage;

    // private int AreaNum;
    private bool failed = false;
    private bool enabledToClick = true;
    [SerializeField] private LocalizedString localizedStringLevel;
    public void SetInfo(string name, int map, string item)
    {
        enabledToClick = true;
        gameObject.transform.GetChild(1).localScale = new Vector3(1,1,1);
        // Invoke("OpenAnimation", 1.167f);
        string txt = "";
        failed = false;
        if(name == "Challenge1") txt = "Hit the target";
        else if (name == "Challenge2") txt = "Find strange flowers";
        else if (name == "Challenge3") txt = name;
        status.SetActive(false);
        // areaImage.GetComponent<Image>().color = new Color(255,255,255);

        challengeName.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt);
        
        playBtn.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase("PLAY");
        
        Image itemImage = areaImage.transform.GetChild(0).GetComponent<Image>();
        itemImage.sprite = controlImage.TakeImage(controlImage.ListItem, item);
        itemImage.SetNativeSize();
        itemImage.rectTransform.sizeDelta *= 100;

        areaImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCompletedMapImage, $"Fr_Map{map}");
    }

    public void SetInfo()
    {
        OpenAnimation();
        status.SetActive(true);
        failed = true;
        // areaImage.GetComponent<Image>().color = new Color(255,255,255);
        playBtn.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Return");
    }

    void OpenAnimation()
    {
        Transform board = gameObject.transform.GetChild(1);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        board.DOPause();
        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            enabledToClick = true;
        });
    }

    public void CloseAnimation()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        enabledToClick = false;
        // if(failed) cloudAnimation.PlayChangeScreenAnimation("Idle", "ComeBackHome");
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
            Play();
        });
    }
    
    void UppdateText(string val)
    {
        messenger.GetComponent<TextMeshProUGUI>().text = val;
    }

    void Play()
    {
        if(!failed)
        {
            game.StartChallenge();
        } else
        {
            cloudAnimation.PlayChangeScreenAnimation("ComeBackHome");
            Invoke("ReturnToHome", 2f);
        }
    }

    void ReturnToHome()
    {
        adsManager.ShowInterstitialAd();
        game.ResetData();
        buttonControl.ReturnToHome();
    }
}
