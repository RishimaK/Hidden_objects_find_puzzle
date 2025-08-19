using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NewArea : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AdsManager adsManager;
    public AudioManager audioManager;
    public Game game;
    public Shop shop;
    public GameObject RandomSpin;
    public VideoMoreHint videoMoreHint;
    public Image areaImage;
    public Transform BtnPlay;
    // public List<Sprite> ListImage;
    public ControlImageFromResources controlImage;
    private bool enabledToClick = true;
    private int AreaNum;
    private int MapNum;
    private bool isSpecial = false;

    public void SetImage(int areaNum, int mapNum, string txt = "")
    {
        isSpecial = txt == "specialGame" ? true : false;
        MapNum = mapNum;
        AreaNum = areaNum;

        if(isSpecial) areaImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListNewAreaSpecialImage, $"SpcMap{mapNum}_{areaNum}");
        else areaImage.GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListNewAreaImage, $"Map{mapNum}_{areaNum}");
        OpenAnimation();
    }

    public void Exit()
    {
        if(!enabledToClick) return;
        // if(AreaNum >= 3 && (int)saveDataJson.GetData("OpenedMap") >= 2) videoMoreHint.ShowAds("StayCamera");
        else CloseAnimation("StayCamera");
    }

    public void Play()
    {
        if(!enabledToClick) return;
        // if(AreaNum >= 3 && (int)saveDataJson.GetData("OpenedMap") >= 2) videoMoreHint.ShowAds();
        else CloseAnimation();
    }

    public void OnBtn()
    {
        enabledToClick = true;
    }

    void OpenAnimation()
    {
        Transform board = gameObject.transform.GetChild(2);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        board.DOPause();

        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            adsManager.EndCountdownToInter(0);
            adsManager.PauseCountDownInter();
            BtnPlay.localScale = Vector3.one;
            BtnPlay.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            if (AreaNum == 2 && (MapNum >= 2 || (int)saveDataJson.GetData("OpenedMap") >= 2))
            {
                shop.OpenSaleDialog();
                enabledToClick = true;
            }
            else if (AreaNum == 3)
            {
                RandomSpin.GetComponent<RandomSpin>().PlayAnimation();
            }
            else enabledToClick = true;
        });
    }

    public void CloseAnimation(string txt = "")
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        Transform board = gameObject.transform.GetChild(2);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            adsManager.ContinueCountDownInter();
            gameObject.SetActive(false);
            BtnPlay.DOKill();
            if(isSpecial)
            {
                SpecialGame specialGame = game.GetComponent<SpecialGame>();
                if(txt == "") specialGame.PlayAnimationCloud(AreaNum);
                else specialGame.PlayAnimationCloud(AreaNum, "StayCamera");
            }
            else
            {
                if(txt == "") game.PlayAnimationCloud(AreaNum);
                else game.PlayAnimationCloud(AreaNum, "StayCamera");
            }
            // adsManager.ShowShopBanner();
        });
    }
}
