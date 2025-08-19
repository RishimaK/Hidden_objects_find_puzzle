using System.Collections;
using TMPro;
using UnityEngine;

public class VideoMoreHint : MonoBehaviour
{
    public AdsManager adsManager;
    public TextMeshProUGUI timeTxt;
    public NewArea newArea;
    private string mess;

    private bool stopCountDown = false;

    public void ShowAds(string txt = "")
    {
        gameObject.SetActive(true);
        stopCountDown = false;
        mess = txt;
        timeTxt.text = "5s..";
        StartCoroutine(CountdownToAds(5));
    }  

    IEnumerator CountdownToAds(int timeRemaining)
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            if(stopCountDown) break;
            timeRemaining -= 1;
            timeTxt.text = $"{timeRemaining}s..";
        }

        if(!stopCountDown) adsManager.ShowRewardedAd("Hint+2");
    }

    public void Exit()
    {
        stopCountDown = true;
        gameObject.SetActive(false);
        newArea.CloseAnimation(mess);
    }   
}
