// using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RandomSpin : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public NewArea newArea;
    public GameObject LuckyReward;
    private bool enabledToClick = true;
    public GameObject Dial;
    private bool slowDown = false;


    [System.Serializable]
    public class Item
    {
        public int name;
        public float dropRate; // Tỉ lệ rơi (0-100)
        public string rewardItem;
        public int valueItem;
    }

    private List<Item> items = new List<Item>
    {
        new Item { name = 1, dropRate = 1f, rewardItem = "VIP"},
        new Item { name = 2, dropRate = 20f, rewardItem = "Hint", valueItem = 1},
        new Item { name = 3, dropRate = 20f, rewardItem = "Gold", valueItem = 100},
        new Item { name = 4, dropRate = 13f, rewardItem = "Hint", valueItem = 2},
        new Item { name = 5, dropRate = 80f, rewardItem = "Gold", valueItem = 10},
        new Item { name = 6, dropRate = 20f, rewardItem = "Hint", valueItem = 1},
        new Item { name = 7, dropRate = 40f, rewardItem = "Gold", valueItem = 50},
        new Item { name = 8, dropRate = 6f, rewardItem = "Hint", valueItem = 3},
    };
    public void PlayAnimation()
    {
        gameObject.SetActive(true);
        enabledToClick = false;
        slowDown = false;
        RectTransform rect = gameObject.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(0, gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y);
        Dial.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        rect.DOAnchorPos(new Vector2(0, 0), 0.5f).SetEase(Ease.OutCubic).OnComplete(() => {
            enabledToClick = true;
        });
    }

    public void Exit(string txt)
    {
        if(!enabledToClick) return;
        if(txt != "NoSound") audioManager.PlaySFX("click");
        enabledToClick = false;
        gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y), 0.5f).SetEase(Ease.OutCubic).OnComplete(() => {
            gameObject.SetActive(false);
            newArea.OnBtn();
        });
    }

    void CalculateTotalDropRate()
    {
        float totalDropRate = 0;
        foreach (var item in items)
        {
            totalDropRate += item.dropRate;
        }

        if (Mathf.Abs(totalDropRate - 100f) > 0.01f)
        {
            Debug.LogWarning($"Tổng tỉ lệ rơi ({totalDropRate}%) không bằng 100%!");
        }
    }

    public Item GetRandomItem()
    {
        double randomValue = System.Math.Round(Random.Range(0f, 200));

        float currentSum = 0;
        foreach (var item in items)
        {
            currentSum += item.dropRate;
            if (randomValue <= currentSum)
            {
                return item;
            }
        }

        // Trường hợp này không nên xảy ra nếu tổng tỉ lệ là 100%
        return items[4];
    }

    public void Rotation()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        // SpinWheel();
        adsManager.ShowRewardedAd("LuckyWheel");
        // GetReward();
    }

    public void GetReward()
    {
        enabledToClick = false;
        StartCoroutine(RotateDial(GetRandomItem().name));
    }

    IEnumerator RotateDial(int reward, float speed = 1, float eulerAnglesZ = 0)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if(speed >= 50 && !slowDown) 
        {
            slowDown = true;
            // Debug.Log(Dial.GetComponent<RectTransform>().eulerAngles);
            Dial.GetComponent<RectTransform>().eulerAngles = new Vector3(0 ,0 ,45 + 45 * (reward - 2));
        }
        
        if(slowDown) speed -= 0.2f;
        else speed += 2;

        Dial.GetComponent<RectTransform>().eulerAngles = new Vector3(0 ,0 ,Dial.GetComponent<RectTransform>().eulerAngles.z - speed);

        eulerAnglesZ += speed;
        if(eulerAnglesZ >= 45)
        { 
            audioManager.PlaySFX("tach");
            eulerAnglesZ -= 45;
        }
        
        if(speed > 0) StartCoroutine(RotateDial(reward, speed, eulerAnglesZ));
        else
        {
            CheckReward();
            enabledToClick = true;
            slowDown = false;
        }
    }

    void CheckReward()
    {
        float val = Dial.GetComponent<RectTransform>().eulerAngles.z;
        int priceNum = 0;
        if(val > 337.5 || val <= 22.5) priceNum = 1;
        else if (val > 292.5) priceNum = 8;
        else if (val > 247.5) priceNum = 7;
        else if (val > 202.5) priceNum = 6;
        else if (val > 157.5) priceNum = 5;
        else if (val > 112.5) priceNum = 4;
        else if (val > 67.5) priceNum = 3;
        else if (val > 22.5) priceNum = 2;

        audioManager.PlaySFX("collect_wheel");
        LuckyReward.GetComponent<LuckyReward>().SetReward(items[priceNum - 1]);
    }

    // [System.Serializable]
    // public class WheelSlot
    // {
    //     public string name;
    //     public float weight;
    //     public float angle;
    // }

    // public List<WheelSlot> wheelSlots = new List<WheelSlot>();
    // public float spinDuration = 5f;
    // public AnimationCurve spinCurve;

    // private float totalWeight;

    // void Start()
    // {
    //     CalculateTotalWeightAndAngles();
    // }

    // void CalculateTotalWeightAndAngles()
    // {
    //     totalWeight = 0;
    //     float currentAngle = 0;

    //     foreach (var slot in wheelSlots)
    //     {
    //         totalWeight += slot.weight;
    //     }

    //     foreach (var slot in wheelSlots)
    //     {
    //         float slotAngle = (slot.weight / totalWeight) * 360f;
    //         slot.angle = currentAngle + (slotAngle / 2f);
    //         currentAngle += slotAngle;
    //     }
    // }

    // public void SpinWheel()
    // {
    //     StartCoroutine(SpinCoroutine());
    // }

    // IEnumerator SpinCoroutine()
    // {
    //     float elapsedTime = 0f;
    //     float startRotation = transform.eulerAngles.z;
    //     float endRotation = CalculateTargetRotation();

    //     while (elapsedTime < spinDuration)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         float t = spinCurve.Evaluate(elapsedTime / spinDuration);
    //         float currentRotation = Mathf.Lerp(startRotation, endRotation, t);

    //         transform.rotation = Quaternion.Euler(0, 0, currentRotation);

    //         yield return null;
    //     }

    //     transform.rotation = Quaternion.Euler(0, 0, endRotation);
    //     Debug.Log("Wheel stopped at: " + GetResultSlot(endRotation).name);
    // }

    // float CalculateTargetRotation()
    // {
    //     float randomValue = Random.Range(0f, totalWeight);
    //     float weightSum = 0;

    //     foreach (var slot in wheelSlots)
    //     {
    //         weightSum += slot.weight;
    //         if (randomValue <= weightSum)
    //         {
    //             return -slot.angle + Random.Range(-10f, 10f) + 720f; // 720 degrees for two full rotations
    //         }
    //     }

    //     return 0f; // Shouldn't reach here
    // }

    // WheelSlot GetResultSlot(float angle)
    // {
    //     angle = NormalizeAngle(angle);
    //     foreach (var slot in wheelSlots)
    //     {
    //         if (NormalizeAngle(slot.angle) >= angle)
    //         {
    //             return slot;
    //         }
    //     }
    //     return wheelSlots[0]; // Fallback to first slot
    // }

    // float NormalizeAngle(float angle)
    // {
    //     while (angle < 0)
    //     {
    //         angle += 360;
    //     }
    //     return angle % 360;
    // }
}
