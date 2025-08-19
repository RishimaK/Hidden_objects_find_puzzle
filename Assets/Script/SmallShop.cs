// using System.Collections;
// using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmallShop : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AdsManager adsManager;
    public AudioManager audioManager;

    public TextMeshProUGUI gold;
    public TextMeshProUGUI goldInHome;
    public TextMeshProUGUI goldInOpenGift;

    public TextMeshProUGUI hintText;

    public ChallengeMode challengeMode;
    public GameObject boardNative;
    public GameObject BoardCoin;
    public Shop shop;

    private bool enabledToClick = true;
    // private bool isTutorial = false;

    void OpenAnimation()
    {
        Transform board = gameObject.transform.GetChild(3);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        board.DOPause();
        board.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            enabledToClick = true;
        });

        GameObject coinFrame = gold.transform.parent.gameObject;
        RectTransform coinFrameRect = coinFrame.GetComponent<RectTransform>();
        // RectTransform boardNativeRect = boardNative.GetComponent<RectTransform>();

        Vector2 currentCoinFramePosition = coinFrameRect.anchoredPosition;
        // Vector2 currentBoardNativePosition = boardNativeRect.anchoredPosition;

        float _width = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        // boardNativeRect.anchoredPosition = new Vector2(_width / 2 + boardNativeRect.sizeDelta.x / 2, boardNativeRect.anchoredPosition.y);
        coinFrameRect.anchoredPosition = new Vector2(-(_width / 2 + coinFrameRect.sizeDelta.x / 2), coinFrameRect.anchoredPosition.y);

        coinFrameRect.DOAnchorPos(currentCoinFramePosition, 1f).SetEase(Ease.OutCubic);
        // boardNativeRect.DOAnchorPos(currentBoardNativePosition, 1f).SetEase(Ease.OutCubic);
    }

    void CloseAnimation()
    {
        Transform board = gameObject.transform.GetChild(3);
        enabledToClick = false;
        board.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
            adsManager.ContinueCountDownInter();
            challengeMode.ContinueCountDown();
            gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

    public void SetGold(string val = "")
    {
        challengeMode.PauseCountDown();
        adsManager.PauseCountDownInter();
        OpenAnimation();
        // if(val == "tutorial")
        // {
        //     gold.text = "100";
        //     isTutorial = true;
        // }
        // else 
        gold.text = $"{(int)saveDataJson.GetData("Gold")}";
    }

    public void AdBtn()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        adsManager.ShowRewardedAd("Hint");
    }

    public void BuyBtn()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        int goldNum = (int)saveDataJson.GetData("Gold");

        if (goldNum >= 100)
        {
            // if(isTutorial) 
            // {
            //     gold.text = "0";
            //     isTutorial = false;
            // }
            // else{
            // }
            gold.text = $"{goldNum - 100}";
            goldInHome.text = gold.text;
            goldInOpenGift.text = gold.text;
            saveDataJson.SaveData("Gold", goldNum - 100);
            AddHint();
        }
        else
        {
            adsManager.ShowAdsMessage("Not Enough Gold");
            OpenBoardCoin();
        }
    }
    
    void OpenBoardCoin()
    {
        if (!enabledToClick) return;
        BoardCoin.SetActive(true);
        BoardCoin.transform.localScale = Vector3.zero;
        BoardCoin.transform.DOPause();
        enabledToClick = false;

        Transform board = gameObject.transform.GetChild(3);
        board.DOScale(Vector3.zero, 0.5f);
        gameObject.transform.GetChild(0).GetComponent<Button>().interactable = false;

        BoardCoin.transform.DOScale(Vector3.one, 1f).SetDelay(0.5f).SetEase(Ease.OutBounce).OnComplete(() => {
            board.gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

    public void CloseBoardCoin()
    {
        if (!enabledToClick) return;
        Transform board = gameObject.transform.GetChild(3);

        enabledToClick = false;
        BoardCoin.transform.DOScale(Vector3.zero, 0.5f);
        gameObject.transform.GetChild(0).GetComponent<Button>().interactable = true;

        board.gameObject.SetActive(true);

        board.DOScale(Vector3.one, 1f).SetDelay(0.5f).SetEase(Ease.OutBounce).OnComplete(() => {
            BoardCoin.SetActive(false);
            enabledToClick = true;
        });
    }

    public void Exit()
    {
        if (!enabledToClick) return;
        audioManager.PlaySFX("click");
        CloseAnimation();
    }

    public void AddHint(string txt = "")
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        Invoke("ActiveBtn", 1f);
        int hintNum = (int)saveDataJson.GetData("Hint") + 1;
        if(txt == "Hint+2") hintNum += 1;
        else if(txt == "hint_sale") hintNum += 2;

        hintText.text = $"{hintNum}";
        saveDataJson.SaveData("Hint", hintNum);
        shop.ChangeHintText(hintNum);
        if(txt != "Hint+2") CloseAnimation();
    }

    void ActiveBtn()
    {
        enabledToClick = true;
    }
}
