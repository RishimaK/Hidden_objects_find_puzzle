using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class OpenedGift : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;
    public AdsManager adsManager;
    // Start is called before the first frame update
    public GameObject AnimationGift;
    public GameObject ListGold;
    private bool enabledToClick = true;
    private int gift = 12;

    public RectTransform parentRectTransform; // RectTransform của đối tượng cha
    public RectTransform childRectTransform;  // RectTransform của đối tượng con
    private float speed = 2000f; // Tốc độ di chuyển

    private float minX;
    private float maxX;
    private bool movingRight = true;

    private bool continueGacha = true;
    public RectTransform checkPoint;
    private float checkPointMax;
    private float checkPointMin;
    public GameObject goldFisrtTarget;
    public ChallengeMode challengeMode;
    public TextMeshProUGUI goldReward;

    public TextMeshProUGUI goldInSmallShop;
    public TextMeshProUGUI goldInHome;
    public TextMeshProUGUI goldInOpenGift;
    void Start()
    {
        float parentWidth = parentRectTransform.rect.width;
        float childWidth = childRectTransform.rect.width;

        minX = (childWidth - parentWidth) / 2;
        maxX = parentWidth / 2 - childWidth / 2;
        childRectTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(minX, childRectTransform.GetComponent<RectTransform>().anchoredPosition.y);

        checkPointMax = -checkPoint.anchoredPosition.x + checkPoint.rect.width / 2;
        checkPointMin = -checkPoint.anchoredPosition.x - checkPoint.rect.width / 2;
        goldReward.text = $"{gift * 2}";
        goldInOpenGift.text = $"{(int)saveDataJson.GetData("Gold")}";
        // PlayAnimation();
    }

    void CloseThisDialog ()
    {
        Transform board = gameObject.transform.GetChild(1);
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            adsManager.ContinueCountDownInter();
            challengeMode.ContinueCountDown();
            gameObject.SetActive(false);
        });
    }

    void SaveGold(int val)
    {
        enabledToClick = false;
        PlayAnimationGold(val);
        int gold = (int)saveDataJson.GetData("Gold") + val;
        saveDataJson.SaveData("Gold", gold);

        audioManager.PlaySFX("coin_appear");
        goldInHome.text = $"{gold}";
        goldInSmallShop.text = $"{gold}";

    }

    void PlayAnimationGold(int val)
    {
        Vector3 fistPos = goldFisrtTarget.transform.position;
        if (val != 12) fistPos = goldReward.transform.parent.GetChild(1).position;
        Vector3 targetPos = goldInOpenGift.transform.parent.GetChild(0).position;
        int gold = (int)saveDataJson.GetData("Gold");
        int num = val / 6;
        for (int i = 0; i < 6; i++)
        {
            Transform child = ListGold.transform.GetChild(i);
            child.position = fistPos;
            child.gameObject.SetActive(true);
            bool isLast = false;
            if(i == 5) isLast = true;
            child.DOMove(targetPos, 2f).SetEase(Ease.InOutBack).OnComplete(() => {
                gold += num;
                goldInOpenGift.text = $"{gold}";
                child.gameObject.SetActive(false);
                if(isLast) CloseThisDialog();
            }).SetDelay(0.1f*i);
        }
    }

    public void PlayAnimation ()
    {
        challengeMode.PauseCountDown();
        adsManager.PauseCountDownInter();
        continueGacha = true;
        enabledToClick = false;
        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0,0,1);
        board.DOPause();
        AnimationGift.SetActive(false);
        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            SkeletonGraphic skeleton = AnimationGift.GetComponent<SkeletonGraphic>();
            skeleton.AnimationState.ClearTrack(0);
            skeleton.Skeleton.UpdateWorldTransform();
            AnimationGift.SetActive(true);
            audioManager.PlaySFX("gift_open");
            skeleton.AnimationState.SetAnimation(0, "animation", false);
            enabledToClick = true;
            StartCoroutine(PlayGacha());
        });
    }

    public void TakeGift()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        SaveGold(gift);
        // CloseThisDialog();
    }

    public void WatchAd()
    {
        if(!enabledToClick) return;
        bool isRewardReady = adsManager.CheckRewardAd();
        if(!isRewardReady) return;

        audioManager.PlaySFX("click");
        enabledToClick = false;

        continueGacha = false;
        Invoke("PlayAd", 1f);
    }

    void PlayAd()
    {
        adsManager.ShowRewardedAd("Gift");
    }

    public void GetReward ()
    {
        continueGacha = true;
        SaveGold(int.Parse(goldReward.text));
        // CloseThisDialog();
    }

    IEnumerator PlayGacha ()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if(continueGacha)
        {
            Vector3 currentPosition = childRectTransform.anchoredPosition;

            // Kiểm tra hướng di chuyển và cập nhật vị trí mới
            if (movingRight)
            {
                currentPosition.x += speed * Time.deltaTime;
                if (currentPosition.x >= maxX)
                {
                    currentPosition.x = maxX;
                    movingRight = false;
                }
            }
            else
            {
                currentPosition.x -= speed * Time.deltaTime;
                if (currentPosition.x <= minX)
                {
                    currentPosition.x = minX;
                    movingRight = true;
                }
            }

            // Cập nhật lại vị trí của đối tượng con
            childRectTransform.anchoredPosition = currentPosition;
            if(Math.Abs(currentPosition.x) >= checkPointMax) goldReward.text = $"{gift * 2}";
            else if (Math.Abs(currentPosition.x) < checkPointMin) goldReward.text = $"{gift * 10}";
            else goldReward.text = $"{gift * 5}";
            StartCoroutine(PlayGacha());
        }
    }
}

// public class coinReward : MonoBehaviour
// {
//     [SerializeField] private GameObject pileOfCoins;
//     [SerializeField] private TextMeshProUGUI counter;
//     [SerializeField] private Vector2[] initialPos;
//     [SerializeField] private Quaternion[] initialRotation;
//     [SerializeField] private int coinsAmount;
//     void Start()
//     {
        
//         if (coinsAmount == 0) 
//             coinsAmount = 10; // you need to change this value based on the number of coins in the inspector
        
//         initialPos = new Vector2[coinsAmount];
//         initialRotation = new Quaternion[coinsAmount];
        
//         for (int i = 0; i < pileOfCoins.transform.childCount; i++)
//         {
//             initialPos[i] = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
//             initialRotation[i] = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().rotation;
//         }
//     }


//    public void CountCoins()
//     {
//         pileOfCoins.SetActive(true);
//         var delay = 0f;
        
//         for (int i = 0; i < pileOfCoins.transform.childCount; i++)
//         {
//             pileOfCoins.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

//             pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(new Vector2(400f, 840f), 0.8f)
//                 .SetDelay(delay + 0.5f).SetEase(Ease.InBack);
             

//             pileOfCoins.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
//                 .SetEase(Ease.Flash);
            
            
//             pileOfCoins.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

//             delay += 0.1f;

//             counter.transform.parent.GetChild(0).transform.DOScale(1.1f, 0.1f).SetLoops(10,LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
//         }

//         StartCoroutine(CountDollars());
//     }
    
//     IEnumerator CountDollars()
//     {
//         yield return new WaitForSecondsRealtime(0.5f);
//         PlayerPrefs.SetInt("CountDollar",PlayerPrefs.GetInt("CountDollar") + 50 + PlayerPrefs.GetInt("BPrize"));
//         counter.text = PlayerPrefs.GetInt("CountDollar").ToString();
//         PlayerPrefs.SetInt("BPrize",0);
//     }