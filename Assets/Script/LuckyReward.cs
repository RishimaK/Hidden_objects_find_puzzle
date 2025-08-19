using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class LuckyReward : MonoBehaviour
{
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public RandomSpin randomSpin;
    public GameObject ListReward;
    private GameObject Reward;
    private bool enabledToClick = true;
    private RandomSpin.Item Item;

    public TextMeshProUGUI goldInHome;
    public TextMeshProUGUI goldReward;
    public TextMeshProUGUI goldInSmallShop;
    public TextMeshProUGUI goldInThisObject;
    public TextMeshProUGUI hintInThisObject;
    public TextMeshProUGUI hintInGame;
    public GameObject goldFisrtTarget;
    public TextMeshProUGUI goldInOpenGift;

    public Shop shop;
    public GameObject ListGold;
    public void SetReward(RandomSpin.Item item)
    {
        Item = item;
        gameObject.SetActive(true);
        string txt;
        goldInThisObject.text = $"{(int)saveDataJson.GetData("Gold")}";
        hintInThisObject.text = $"{(int)saveDataJson.GetData("Hint")}";
        if(item.rewardItem == "Hint")
        {
            Reward = ListReward.transform.GetChild(0).gameObject;
            Reward.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{item.valueItem}";
            if(item.valueItem == 1) txt = "Hintreward";
            else txt = "Hintsreward";
            Reward.transform.GetChild(0).GetComponent<LocalizeStringEvent>()
                .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt);
        }
        else if(item.rewardItem == "Gold")
        {
            Reward = ListReward.transform.GetChild(1).gameObject;
            Reward.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{item.valueItem}";
        }
        else if(item.rewardItem == "VIP")
        {
            Reward = ListReward.transform.GetChild(2).gameObject;
        }
        Reward.SetActive(true);
        OpenAnimation();
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

    void CloseAnimation()
    {

        Transform board = gameObject.transform.GetChild(1);

        enabledToClick = false;

        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
            foreach(Transform child in ListReward.transform)
            {
                child.gameObject.SetActive(false);
            }
        });
    }

    public void TakeReward()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        if(Item.rewardItem == "Gold")
        {
            SaveGold();
        }
        else if(Item.rewardItem == "Hint")
        {
            SaveHint();
        }
        else if(Item.rewardItem == "VIP")
        {
            shop.VIP3Day();
            CloseAnimation();
        }
    }

    void SaveGold()
    {
        PlayAnimationGold();
        int gold = (int)saveDataJson.GetData("Gold") + Item.valueItem;
        saveDataJson.SaveData("Gold", gold);

        audioManager.PlaySFX("coin_appear");
        goldInHome.text = $"{gold}";
        goldInSmallShop.text = $"{gold}";
        goldInOpenGift.text = $"{gold}";
    }

    public void SaveHint()
    {
        PlayAnimationGold();
        int Hint = (int)saveDataJson.GetData("Hint") + Item.valueItem;
        saveDataJson.SaveData("Hint", Hint);

        shop.ChangeHintText(Hint);
        audioManager.PlaySFX("");
    }

    void PlayAnimationGold()
    {
        Vector3 fistPos = goldFisrtTarget.transform.position;
        Transform target = goldInThisObject.transform.parent.GetChild(0);
        int total = (int)saveDataJson.GetData("Gold");
        int num = Item.valueItem > 10 ? 10 : 2;
        if(Item.rewardItem == "Hint") 
        {
            num = 1;
            target = hintInThisObject.transform.parent.GetChild(0);
            total = (int)saveDataJson.GetData("Hint");
        }
        Vector3 targetPos = target.position;
        int ListGoldLength = ListGold.transform.childCount;
        int max = Item.valueItem / num;

        for (int i = 0; i < max; i++)
        {
            Transform child;
            if(ListGoldLength - 1 < i) {
                child = Instantiate(ListGold.transform.GetChild(0) , Vector3.zero, Quaternion.identity);
                child.transform.SetParent(ListGold.transform);
                child.transform.localScale = new Vector3(1f,1f,1);
            } else child = ListGold.transform.GetChild(i);
            child.position = fistPos;
            child.GetComponent<Image>().sprite = target.GetComponent<Image>().sprite;
            child.gameObject.SetActive(true);
            bool isLast = false;
            if(i == max - 1) isLast = true;
            child.DOMove(targetPos, 2f).SetEase(Ease.InOutBack).OnComplete(() => {
                total += num;
                child.gameObject.SetActive(false);
                if(Item.rewardItem == "Hint") 
                {
                    hintInThisObject.text = $"{total}";
                    hintInGame.text = $"{total}";
                } else goldInThisObject.text = $"{total}";

                if(isLast) {
                    CloseAnimation();
                    randomSpin.Exit("NoSound");
                }
            }).SetDelay(0.1f*i);
        }
    }
}
