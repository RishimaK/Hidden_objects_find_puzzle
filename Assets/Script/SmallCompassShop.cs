using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmallCompassShop : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AdsManager adsManager;
    public AudioManager audioManager;

    public TextMeshProUGUI gold;

    // public TextMeshProUGUI compassText;

    public ChallengeMode challengeMode;
    public Shop shop;
    public GameObject BoardCoin;

    private bool enabledToClick = true;
    // private bool isTutorial = false;

    void OpenAnimation()
    {
        Transform board = gameObject.transform.GetChild(2);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        board.DOPause();
        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            enabledToClick = true;
        });

        GameObject coinFrame = gold.transform.parent.gameObject;
        RectTransform coinFrameRect = coinFrame.GetComponent<RectTransform>();

        Vector2 currentCoinFramePosition = coinFrameRect.anchoredPosition;

        float _width = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        coinFrameRect.anchoredPosition = new Vector2(-(_width / 2 + coinFrameRect.sizeDelta.x / 2), coinFrameRect.anchoredPosition.y);

        coinFrameRect.DOAnchorPos(currentCoinFramePosition, 1f).SetEase(Ease.OutCubic);
    }

    void CloseAnimation()
    {
        Transform board = gameObject.transform.GetChild(2);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            adsManager.ContinueCountDownInter();
            challengeMode.ContinueCountDown();
            gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

    public void SetGold(string val = "")
    {
        gameObject.SetActive(true);
        challengeMode.PauseCountDown();
        adsManager.PauseCountDownInter();
        OpenAnimation();

        gold.text = $"{(int)saveDataJson.GetData("Gold")}";
    }

    public void AdBtn()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        adsManager.ShowRewardedAd("BuyCompass");
    }

    public void BuyBtn()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        int goldNum = (int)saveDataJson.GetData("Gold");

        if (goldNum >= 200)
        {
            goldNum -= 200;
            gold.text = $"{goldNum}";
            shop.AddMoreStuff(-200, 0, 0);
            AddCompass();
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

        Transform board = gameObject.transform.GetChild(2);
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
        Transform board = gameObject.transform.GetChild(2);

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

    public void AddCompass()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        Invoke("ActiveBtn", 1f);
        // int compassNum = (int)saveDataJson.GetData("Compass") + 1;
        shop.AddMoreStuff(0,0,1);

        // compassText.text = $"{compassNum}";
        CloseAnimation();
    }

    void ActiveBtn()
    {
        enabledToClick = true;
    }
}
