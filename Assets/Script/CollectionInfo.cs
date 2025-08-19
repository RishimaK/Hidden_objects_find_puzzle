// using System.Collections;
// using System.Collections.Generic;
using System;
using System.Collections;
using System.Globalization;
using DG.Tweening;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class CollectionInfo : MonoBehaviour
{
    public ControlImageFromResources controlImage;

    public AudioManager audioManager;
    public ButtonControl buttonControl;
    public GameObject CollectionDialog;
    public GameObject collectionBtn;
    public Home home;

    public GameObject ListCollection;
    public GameObject hand;
    public GameObject CardInfo;
    public GameObject LightTut;

    public Game game;
    private bool enabledToClick = true;

    public void SetPos()
    {
        RectTransform rect = ListCollection.GetComponent<RectTransform>();
        RectTransform rectListCard = ListCollection.transform.GetChild(0).GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(140, 0);
        rect.localPosition = new Vector2(0, -413 + 344);
        rectListCard.localPosition = new Vector2(0, -344);
    }

    public void OpenCollectionInfor(string txt = "")
    {
        audioManager.PlaySFX("click");
        OpenAnimation(gameObject);
        string text = "Notfondcard";
        // if(txt == "gift") text = "Takereward";

        gameObject.transform.GetChild(1).GetChild(0).GetComponent<LocalizeStringEvent>()
            .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
    }

    public void TakeGiftOfCollection(Button button)
    {
        audioManager.PlaySFX("click");
        bool takeReward = true;
        foreach(Transform child in button.transform.parent.Find("ListCard"))
        {
            if(child.GetComponent<Image>().sprite.name == "Nocard") takeReward = false;
        }

        if(!takeReward)
        {
            OpenAnimation(gameObject);
            gameObject.transform.GetChild(1).GetChild(0).GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("Takereward");
        }
        else
        {
            // OpenAnimation(CollectionDialog);
            home.PlayAnimationGold(button.transform.parent.Find("Gift/Coin").gameObject);
        }
    }

    void OpenAnimation(GameObject target)
    {
        target.SetActive(true);
        Transform board = target.transform.GetChild(1);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        board.DOPause();
        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            enabledToClick = true;
        });
    }

    public void CloseAnimationCollectionInfo()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        Transform board = gameObject.transform.GetChild(1);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

        public void CloseAnimationCollectionDialog()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        Transform board = gameObject.transform.GetChild(1);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

    public void OffBtn()
    {
        enabledToClick = false;
    }

    public void OnBtn()
    {
        enabledToClick = true;
    }

    private Sequence currentSequence;
    private bool collectionListOpen = false;
    public void CollectionBtn()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        RectTransform rect = ListCollection.GetComponent<RectTransform>();
        RectTransform rectListCard = ListCollection.transform.GetChild(0).GetComponent<RectTransform>();
        if(hand.activeSelf) 
        {
            hand.SetActive(false);// Findallcards
            LightTut.SetActive(false);
            OpenAnimation(CardInfo);
        }
        currentSequence.Kill();
        if(collectionListOpen) 
        {
            collectionListOpen = false;
            // rect.sizeDelta = new Vector2(140, 688);
            // rect.localPosition = new Vector2(0, -413);
            // rectListCard.localPosition = new Vector2(0, 0);

            currentSequence = DOTween.Sequence();
            currentSequence.Join(rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, 0), 0.5f));
            currentSequence.Join(rect.DOAnchorPos(new Vector2(0, -69), 0.5f));
            currentSequence.Join(rectListCard.DOAnchorPos(new Vector2(0, - 344), 0.5f));
            currentSequence.Play().OnComplete(() => {
                ListCollection.SetActive(false);
            });
        }
        else 
        {
            collectionListOpen = true;
            // rect.sizeDelta = new Vector2(140, 0);
            // rect.localPosition = new Vector2(0, -413 + 344);
            // rectListCard.localPosition = new Vector2(0, -344);
            ListCollection.SetActive(true);

            currentSequence = DOTween.Sequence();
            currentSequence.Join(rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, 688), 0.5f));
            currentSequence.Join(rect.DOAnchorPos(new Vector2(0, -413), 0.5f));
            currentSequence.Join(rectListCard.DOAnchorPos(new Vector2(0, 0), 0.5f));
            currentSequence.Play();
        }

        

    }

    public void SetImage(string name)
    {
        ListCollection.transform.GetChild(0).GetChild(Convert.ToInt32(name) - 1).GetComponent<Image>().color = new Color(1,1,1);
    }

    public void CheckImage()
    {
        for(int i = 0; i < 4; i++)
        {
            if(ListCollection.transform.GetChild(0).GetChild(i).GetComponent<Image>().color == new Color(0.5f,0.5f,0.5f)) break;
            else if(i == 3)
            {
                float time = 0;
                if(collectionListOpen)
                {
                    enabledToClick = true;
                    CollectionBtn();
                    time = 0.5f;
                }
                Invoke("MoveICOut", time);

            }
        }
    }

    private  Vector3 curentPos;

    void MoveICOut()
    {
        curentPos = collectionBtn.transform.position;
        enabledToClick = false;
        buttonControl.MoveOut(collectionBtn, -1);
        Invoke("ReturnPos", 0.71f);
    }

    void ReturnPos()
    {
        // collectionBtn.transform.position = curentPos;
        collectionBtn.SetActive(false);
        enabledToClick = true;
    }

    public void SetCardImage(int mapNum)
    {
        for(int i = 0; i < 4; i++)
        {
            ListCollection.transform.GetChild(0).GetChild(i).GetComponent<Image>().sprite = controlImage.TakeImage(controlImage.ListCard, $"{mapNum}_{i + 1}");
            ListCollection.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
        }
    }

    public void CloseList()
    {
        ListCollection.SetActive(false);
        collectionListOpen = false;
    }

    public void CloseCardInfo()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        Transform board = CardInfo.transform.GetChild(1);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            CardInfo.SetActive(false);
            enabledToClick = true;
            game.AfterCardTutorial();
        });
    }

    public void LightTutAction()
    {
        LightTut.SetActive(true);
        LightAnimation(LightTut.transform.GetChild(1).gameObject);
                
    }

    void LightAnimation(GameObject target, int rotate = 0)
    {
        if(!LightTut.activeSelf) return;
        // target.transform.DOScale(new Vector3(1f , 1f, 1f), 0.5f).OnComplete(() => {
        //     if(!LightTut.activeSelf) return;
        //     target.transform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.5f).OnComplete(() => {
        //         LightAnimation(target);
        //     });
        // });

        // Tạo sequence để thực hiện các tween đồng thời
        Sequence sequence = DOTween.Sequence();
        rotate += 1;
        target.transform.DORotate(new Vector3(0, 0, rotate), 2f);

        // sequence.Join(target.transform.DORotate(new Vector3(0, 0, rotate), 0.5f));

        // float val = target.transform.localScale.z == 1.1f ? 1 : 1.1f;
        // sequence.Join(target.transform.DOScale(new Vector3(val, val, 1f), 0.5f));

        // Bắt đầu sequence
        sequence.Play().OnComplete(() => {
            LightAnimation(target, rotate);
        });
    }
}
