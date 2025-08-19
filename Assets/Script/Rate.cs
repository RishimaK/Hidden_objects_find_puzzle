using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Rate : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public GameObject ListStar;

    // void Start()
    // {
    //     // start.GetComponent<Button>().OnPointerEnter(() => rateStar();)
    // }

    public void PointDown(Button btn)
    {
        audioManager.PlaySFX("click");
        int index = btn.transform.GetSiblingIndex();
        for(int i = 0; i <= index; i++)
        {
            ListStar.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
    }

    public void PointUp(Button btn)
    {
        int index = btn.transform.GetSiblingIndex();

        if(index >= 3)
        {
            adsManager.Rate();
            saveDataJson.SaveData("Rate", true);
        }
        gameObject.SetActive(false);
        for(int i = 0; i <= index; i++)
        {
            ListStar.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }

    public void exit()
    {
        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
    }
    // public Button Btn1;
    // public Button Btn2;
    // public Button Btn3;
    // public Button Btn4;
    // public Button Btn5;
    // public float holdThreshold = 0.2f; // Thời gian tối thiểu để xem như đang giữ nút (giây)

    // private bool isPointerDown = false;
    // private bool isLongPress = false;
    // private Coroutine holdCoroutine;

    // void Start()
    // {
    //     Btn1 = GetComponent<Button>();
    //     Btn2 = GetComponent<Button>();
    //     Btn3 = GetComponent<Button>();
    //     Btn4 = GetComponent<Button>();
    //     Btn5 = GetComponent<Button>();
    //     // foreach(Transform child in ListStar.transform)
    //     // {
    //     //     child = 
    //     // }
    // }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     isPointerDown = true;
    //     holdCoroutine = StartCoroutine(HoldDetector());
    // }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     isPointerDown = false;
    //     if (holdCoroutine != null)
    //     {
    //         StopCoroutine(holdCoroutine);
    //     }
        
    //     if (!isLongPress)
    //     {
    //         Debug.Log(eventData);
    //         OnClick();
    //     }
    //     else
    //     {
    //         OnHoldRelease();
    //     }
        
    //     isLongPress = false;
    // }

    // private IEnumerator HoldDetector()
    // {
    //     yield return new WaitForSeconds(holdThreshold);
        
    //     if (isPointerDown)
    //     {
    //         isLongPress = true;
    //         OnHoldStart();
    //         while (isPointerDown)
    //         {
    //             OnHolding();
    //             yield return null;
    //         }
    //     }
    // }

    // // Các phương thức xử lý sự kiện
    // private void OnClick()
    // {
    //     Debug.Log("Button Clicked");
    //     // Xử lý khi nút được click bình thường
    // }

    // private void OnHoldStart()
    // {
    //     Debug.Log("Started Holding Button");
    //     // Xử lý khi bắt đầu giữ nút
    // }

    // private void OnHolding()
    // {
    //     Debug.Log("Holding Button");
    //     // Xử lý trong khi đang giữ nút
    // }

    // private void OnHoldRelease()
    // {
    //     Debug.Log("Released Hold");
    //     // Xử lý khi thả nút sau khi giữ
    // }
}
