using System.Globalization;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ExitChallengeDialog : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public ChallengeMode Game;
    public Complete complete;
    public Home home;
    public Button challengeButton;
    // public GameObject ChallengeList;
    private bool unlockedchallenge = false;

    void ShowAds()
    {
        adsManager.ShowInterstitialAd();
    }

    public void YesBtn()
    {
        if(!unlockedchallenge)
        {
            audioManager.PlaySFX("click");
            gameObject.SetActive(false);
            Game.PlayCloundAnimation("return home");
            Invoke("ShowAds", 2);
        }
        else
        {
            unlockedchallenge = false;
            gameObject.SetActive(false);
            complete.HomeBtn();
            home.ChangeMainPage(challengeButton);
        }
    }

    public void NoBtn()
    {
        audioManager.PlaySFX("click");
        Game.ContinueCountDown();
        gameObject.SetActive(false);
        unlockedchallenge = false;
    }

    public void SetMessage (string message, int num = 0)
    {
        if(message == "Notyet")
        {
            gameObject.transform.Find("Board/exit").gameObject.SetActive(true);
            gameObject.transform.Find("Board/Yes").gameObject.SetActive(false);
            gameObject.transform.Find("Board/No").gameObject.SetActive(false);
            gameObject.transform.Find("Board/Name").GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Challenge Locked");
            gameObject.transform.Find("Board/Status").GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"Openchallengemap{num}");
        }
        else if(message == "outchallenge")
        {
            gameObject.transform.Find("Board/exit").gameObject.SetActive(false);
            gameObject.transform.Find("Board/Yes").gameObject.SetActive(true);
            gameObject.transform.Find("Board/No").gameObject.SetActive(true);
            gameObject.transform.Find("Board/Name").GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Sure?");
            gameObject.transform.Find("Board/Status").GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Loseallcurrentprogress.");
        }
        else if(message == "unlockedchallenge")
        {
            // home.UnlockChallenge();
            unlockedchallenge = true;
            gameObject.transform.Find("Board/exit").gameObject.SetActive(false);
            gameObject.transform.Find("Board/Yes").gameObject.SetActive(true);
            gameObject.transform.Find("Board/No").gameObject.SetActive(true);
            gameObject.transform.Find("Board/Name").GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Challenge Unlocked");
            gameObject.transform.Find("Board/Status").GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Play Challenge");
        }
    }
}
