using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class BuySpecialMap : MonoBehaviour
{
    private bool enabledToClick = true;
    private int mapNum;

    public Shop shop;
    public AudioManager audioManager;
    public AdsManager adsManager;
    public ControlImageFromResources controlImage;
    public SaveDataJson saveDataJson;
    public Home home;

    public void OpenAnimation(GameObject obj)
    {
        mapNum = int.Parse(obj.name);
        ChangePrice(mapNum);
        gameObject.transform.Find("Board/Image").GetComponent<Image>().sprite = 
            controlImage.TakeImage(controlImage.ListMapSpecialCanBuyImage, $"Special{mapNum}");

        gameObject.transform.Find("Board/Name").GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference =
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(obj.transform.Find("frName/Txt").GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference);

        gameObject.SetActive(true);
        Transform board = gameObject.transform.GetChild(1);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        board.DOPause();
        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            enabledToClick = true;
        });
    }

    void ChangePrice(int mapNum)
    {
        TextMeshProUGUI coinTxt = gameObject.transform.Find("Board/BuyByCoin/Txt").GetComponent<TextMeshProUGUI>();
        if(mapNum == 1) coinTxt.text = "1000";
        else coinTxt.text = "2000";
        adsManager.ChangeSpecialMapPrice(mapNum);
    }

    public void CloseAnimation()
    {
        Transform board = gameObject.transform.GetChild(1);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

    public void BuyByCoin()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        int coin = (int)saveDataJson.GetData("Gold");

        if((mapNum == 1 && coin < 1000) || (mapNum != 1 && coin < 2000))
        {
            adsManager.ShowAdsMessage("Not Enough Gold");
            return;
        } 

        if(mapNum == 1) shop.AddMoreStuff(-1000, 0, 0);
        else shop.AddMoreStuff(-2000, 0, 0);
        BuyMap();
    }

    public void BuyByMoney()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        if(mapNum == 1) adsManager.BuySpecialMapBtn("special_map_sale");
        else adsManager.BuySpecialMapBtn("special_map");
    }

    public void BuyMap()
    {
        saveDataJson.SaveData("ListOpenedSpecialMap", mapNum);
        home.OpenSpecialMap(mapNum);
        CloseAnimation();
    }
}
