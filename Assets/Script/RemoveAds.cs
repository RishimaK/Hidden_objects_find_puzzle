using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAds : MonoBehaviour
{
    // Start is called before the first frame update
    private bool enabledToClick = true;
    public AudioManager audioManager;
    public AdsManager adsManager;
    public Complete complete;
    public void Exit()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
        complete.OpenChallengeDialog();
    }

    public void RemoveAdsBtn()
    {
        if(!enabledToClick) return;
        // enabledToClick = false;
        audioManager.PlaySFX("click");
        adsManager.NonConnumableBtn();
    }
}
