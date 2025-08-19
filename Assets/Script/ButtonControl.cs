using DG.Tweening;
using UnityEngine;
using System.Collections;
// using UnityEngine.UI;

// using System;
// using System.Collections.Generic;
// using UnityEngine.SceneManagement;

// using System.Threading.Tasks;
// using UnityEngine.EventSystems;

public class ButtonControl : MonoBehaviour
{
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public RectTransform gameFooter;
    public AdsManager adsManager;
    public GameObject home;
    public GameObject rate;
    public GameObject game;
    public GameObject gameStage;
    public GameObject upBtn;
    public GameObject giftIcon;
    public GameObject homeBtn;
    public GameObject collecttionBtn;
    public GameObject downBtn;
    public GameObject setting;
    public GameObject settingBtn;
    public GameObject statistical;
    public GameObject ExitChallengeDialog;
    public GameObject SmallShop;
    public SmallCompassShop smallCompassShop;
    public GameObject MaskUI;
    public GameObject Compass;
    public ChallengeMode challengeMode;
    private bool enabledToClick = true;


    void Awake()
    {
        // Debug.Log(gameFooter.position.y);
        // -2.287579/-5.337684
        // gameFooter.position = new Vector3(gameFooter.position.x, -5.19f, gameFooter.position.z);
    }
    IEnumerator WaitFunction(float time)
    {
        enabledToClick = false;
        yield return new WaitForSeconds(time);
        enabledToClick = true;
    }

    public void OffBtn()
    {
        enabledToClick = false;
    }

    public void OnBtn()
    {
        enabledToClick = true;
    }


    public void MoveDownFooterInGame()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        enabledToClick = false;
        RectTransform gameFooterRect = gameFooter.GetComponent<RectTransform>();
        gameFooterRect.DOAnchorPos(new Vector2(gameFooterRect.anchoredPosition.x, -93f), 0.7f).SetEase(Ease.OutBack).OnComplete(() => {
            upBtn.SetActive(true);
            downBtn.SetActive(false);
            statistical.SetActive(true);
            enabledToClick = true;
        });
        // gameFooter.DOMove(new Vector3(gameFooter.position.x, gameFooter.position.y - Camera.main.orthographicSize * 0.2875f, gameFooter.position.z)
        // , 0.7f).SetEase(Ease.OutBack).OnComplete(() => {
        //     upBtn.SetActive(true);
        //     downBtn.SetActive(false);
        //     statistical.SetActive(true);
        // });

        MoveOut(homeBtn, -1);
        MoveOut(collecttionBtn, -1);
        MoveOut(giftIcon, 1);
        MoveOut(settingBtn, 1);

        StartCoroutine(WaitFunction(0.7f));
    }

    public void MoveOut(GameObject btn,int num)
    {
        btn.transform.DOMove(
            new Vector3(btn.transform.position.x + num * Camera.main.orthographicSize * 0.2875f, btn.transform.position.y, btn.transform.position.z), 0.7f).OnComplete(() => {
                // btn.SetActive(false);
            });
    }

    private void MoveIn(GameObject btn,int num)
    {
        // btn.SetActive(true);
        btn.transform.DOMove(
            new Vector3(btn.transform.position.x - num * Camera.main.orthographicSize * 0.2875f, btn.transform.position.y, btn.transform.position.z), 0.7f);
    }

    public void MoveUpFooterInGame()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        audioManager.PlaySFX("click");

        RectTransform gameFooterRect = gameFooter.GetComponent<RectTransform>();
        gameFooterRect.DOAnchorPos(new Vector2(gameFooterRect.anchoredPosition.x, 140), 0.7f).SetEase(Ease.OutBack).OnComplete(() => {
            upBtn.SetActive(false);
            downBtn.SetActive(true);
            enabledToClick = true;
        });
        // gameFooter.transform.DOMove(new Vector3(gameFooter.position.x, gameFooter.position.y + Camera.main.orthographicSize * 0.2875f, gameFooter.position.z)
        // , 0.7f).SetEase(Ease.OutBack).OnComplete(() => {
        //     upBtn.SetActive(false);
        //     downBtn.SetActive(true);
        // });
        statistical.SetActive(false);

        MoveIn(homeBtn, -1);
        MoveIn(collecttionBtn, -1);
        MoveIn(giftIcon, 1);
        MoveIn(settingBtn, 1);

        StartCoroutine(WaitFunction(0.7f));
    }

    public void OpenSetting()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        challengeMode.PauseCountDown();
        adsManager.PauseCountDownInter();
        float yy = setting.transform.GetChild(2).position.y;
        setting.transform.GetChild(2).position = new Vector3(setting.transform.position.x, yy - 11, setting.transform.position.z);
        setting.SetActive(true);
        setting.transform.GetChild(2).DOMove(new Vector3(setting.transform.position.x, yy,setting.transform.position.z),0.4f).SetEase(Ease.InQuad);
        StartCoroutine(WaitFunction(0.4f));
    }

    public void CloseSetting()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        float yy = setting.transform.GetChild(2).position.y;
        setting.transform.GetChild(2).DOMove(new Vector3(setting.transform.position.x, yy - 11 ,setting.transform.position.z),0.4f).SetEase(Ease.InQuad).OnComplete(() => 
        {
            adsManager.ContinueCountDownInter();
            challengeMode.ContinueCountDown();
            setting.SetActive(false);
            setting.transform.GetChild(2).position = new Vector3(setting.transform.position.x, yy, setting.transform.position.z);
        });
        StartCoroutine(WaitFunction(0.4f));
    }

    public void ComeBackHome()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        if(game.GetComponent<Game>().enabled) game.GetComponent<Game>().PlayCloundAnimation("return home");
        else if(game.GetComponent<SpecialGame>().enabled) game.GetComponent<SpecialGame>().PlayCloundAnimation("return home");
        else
        {
            ExitChallengeDialog.SetActive(true);
            challengeMode.PauseCountDown();
            ExitChallengeDialog.GetComponent<ExitChallengeDialog>().SetMessage("outchallenge");
        }
    }

    public void ReturnToHome(string txt = null)
    {
        game.SetActive(false);
        gameStage.SetActive(false);
        home.SetActive(true);
        home.GetComponent<Home>().CheckUnlockMap();
        if(!(bool)saveDataJson.GetData("Rate")) rate.SetActive(true);
    }

    public void OpenShop()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
    }

    public void UseHint()
    {
        if(!enabledToClick && !MaskUI.activeSelf) return;
        if(isTutCompass) return;
        audioManager.PlaySFX("click");
        int hint = (int)saveDataJson.GetData("Hint");
        if(hint == 0)
        {
            SmallShop.SetActive(true);
            // if((bool)saveDataJson.GetData("PlayTutorial")) SmallShop.GetComponent<SmallShop>().SetGold("tutorial");
            // else 
            SmallShop.GetComponent<SmallShop>().SetGold();
        }
        else
        {
            if (game.GetComponent<SpecialGame>().enabled)
            {
                game.GetComponent<SpecialGame>().FindRandomItem();
            }
            else
            {
                Game gameScript = game.GetComponent<Game>();
                gameScript.CloseMask("touch");
                gameScript.FindRandomItem();
                gameScript.StopBounceTool();
            }

            OffBtn();
            Invoke("OnBtn", 0.5f);
        }

    }

    private bool isTutCompass = false;
 
    public void AllowClickCompassTut()
    {
        isTutCompass = true;
    }

    public void DeclineClickCompassTut()
    {
        isTutCompass = false;
    }

    public void UseCompass()
    {
        if(!enabledToClick && !MaskUI.activeSelf) return;
        if(Compass.activeSelf) return;
        audioManager.PlaySFX("click");
        int compassValue = (int)saveDataJson.GetData("Compass");
        if(compassValue == 0)
        {
            smallCompassShop.SetGold();
        }
        else
        {
            if (game.GetComponent<SpecialGame>().enabled)
            {
                game.GetComponent<SpecialGame>().UseCompass();
            }
            else
            {
                Game gameScript = game.GetComponent<Game>();
                gameScript.UseCompass();
                gameScript.CloseMask("touch");
                gameScript.StopBounceTool();
            }

            OffBtn();
            Invoke("OnBtn", 0.5f);
        }

    }

    public void StartGame(GameObject btn)
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        game.SetActive(true);
        game.GetComponent<Game>().LoadGame(int.Parse(btn.name));
    }
}
