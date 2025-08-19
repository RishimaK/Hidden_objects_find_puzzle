using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeeklyReward : MonoBehaviour
{
    public GameObject Home;
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;

    private bool enabledToClick = true;
    private DateTime deadLine;
    public bool isCountingDown = false;

    public TextMeshProUGUI timer;
    public Slider RewardSlider;
    public Slider WeeklyItemSliderInHome;
    public Slider WeeklyItemSlider;

    public GameObject ListNormalReward;
    public GameObject ListVipReward;
    public GameObject WeeklyLock;

    public Shop shop;
    public GameObject BtnUnlock;
    public GameObject VipRewardDialog;

    public GameObject warning;
    public GameObject hand;
    public GameObject bg;

    public void PlayTutorial()
    {
        CheckWeeklyReward("FisrtOpen");
        WeeklyItemSliderInHome.transform.parent.gameObject.SetActive(true);
        WeeklyLock.SetActive(false);
        bg.SetActive(true);
        hand.SetActive(true);
        HandTutorialAnimation();
    }

    void HandTutorialAnimation()
    {
        if(!hand.activeSelf) return;
        hand.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).OnComplete(() => {
            if(!hand.activeSelf) return;
            hand.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.5f).OnComplete(() => {
                HandTutorialAnimation();
            });
        });
    }

    public void ChangeWeeklyItemSlider()
    {
        if((int)saveDataJson.GetData("OpenedMap") >= 3) WeeklyItemSliderInHome.transform.parent.gameObject.SetActive(true);

        float reward = ((string[])saveDataJson.GetData("SoulFireFound")).Length;
        for(int i = 0; i < 1;)
        {
            reward = reward >= 3 ? reward - 3 : reward;
            if(reward < 3) i++;
        }

        WeeklyItemSliderInHome.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{reward}/3";
        WeeklyItemSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{reward}/3";

        WeeklyItemSliderInHome.value = reward / 3;
        WeeklyItemSlider.value = reward / 3;
    }

    public void SetValue()
    {
        if((bool)saveDataJson.GetData("WeeklyVIP"))
        {
            BtnUnlock.SetActive(false);
            BtnUnlock.transform.parent.GetComponent<Button>().enabled = false;

        }
        ChangeRewardSlider();
    }

    public void ChangeRewardSlider()
    {
        int[] rewardTook = (int[])saveDataJson.GetData("RewardWeeklyTook");
        int[] rewardVipTook = (int[])saveDataJson.GetData("RewardWeeklyVipTook");
        int reward = ((string[])saveDataJson.GetData("SoulFireFound")).Length;
        bool isVIP = (bool)saveDataJson.GetData("WeeklyVIP");
        reward = reward / 3;

        for(int i = 1; i <= reward; i++)
        {
            Transform item = ListNormalReward.transform.GetChild(i);
            Transform itemVIP = ListVipReward.transform.GetChild(i);
            item.GetChild(1).gameObject.SetActive(false);
            if(rewardTook == null || !rewardTook.Contains(i)) 
            {
                item.GetComponent<Button>().enabled = true;
                item.transform.GetChild(3).gameObject.SetActive(true);
            }
            else item.GetChild(2).gameObject.SetActive(true);

            if(isVIP)
            { 
                itemVIP.GetChild(1).gameObject.SetActive(false);
                if(rewardTook == null || !rewardVipTook.Contains(i)) 
                {
                    itemVIP.GetComponent<Button>().enabled = true;
                    itemVIP.transform.GetChild(3).gameObject.SetActive(true);
                }
                else itemVIP.GetChild(2).gameObject.SetActive(true);
            }
        }

        float val = (float)reward / 21;
        if(reward != 0) val += 1f/42f;
        RewardSlider.value = val;
        CheckNotice();
    }

    void CheckNotice ()
    {
        bool isReward = false;

        foreach (Transform item in ListNormalReward.transform)
        {
            if(item.GetComponent<Button>().enabled)
            {
                isReward = true;
                break;
            }
        }

        if(!isReward && (bool)saveDataJson.GetData("WeeklyVIP"))
        {
            foreach (Transform item in ListVipReward.transform)
            {
                if(item.GetComponent<Button>().enabled && item.name != "BtnUnlock")
                {
                    isReward = true;
                    break;
                }
            } 
        }

        if(isReward)
        {
            warning.SetActive(true);
            PlayWarning();
        }
        else warning.SetActive(false);
    }

    void PlayWarning()
    {
        if(!warning.activeSelf) return;
        warning.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).OnComplete(() => {
            if(!warning.activeSelf) return;
            warning.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.5f).OnComplete(() => {
                PlayWarning();
            });
        });
    }

    void ResetWeeklyRewardsData (string deadLineFormatted)
    {
        saveDataJson.SaveData("SoulFireFound", null);
        saveDataJson.SaveData("RewardWeeklyVipTook", null);
        saveDataJson.SaveData("RewardWeeklyTook", null);
        saveDataJson.SaveData("WeeklyVIP", false);
    }

    string FormatDateTime(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss");

    public void CheckWeeklyReward (string txt = "")
    {
        if((int)saveDataJson.GetData("OpenedMap") < 3) 
        {
            WeeklyLock.SetActive(true);
            return;  
        };

        DateTime currentTime = DateTime.Now;

        string weeklyTime = (string)saveDataJson.GetData("EndWeeklyReward");
        string format = "dd/MM/yyyy HH:mm:ss";

        if(weeklyTime == null || weeklyTime == "" || DateTime.ParseExact(weeklyTime, format, null) < currentTime)
        {
            // chưa set time hoặc set rồi mà khác ngày
            deadLine = currentTime.AddDays(7);
            string deadLineFormatted = FormatDateTime(deadLine);
            saveDataJson.SaveData("EndWeeklyReward", deadLineFormatted);

            if(txt != "FisrtOpen") ResetWeeklyRewardsData(deadLineFormatted);
            weeklyTime = deadLineFormatted;
        }else if(DateTime.ParseExact(weeklyTime, format, null) >= currentTime) deadLine = DateTime.ParseExact(weeklyTime, format, null);

        if(DateTime.ParseExact(weeklyTime, format, null) >= currentTime)
        {
            UpdateCountdownDisplay();
            StartCountdown();
        }
        else if(DateTime.ParseExact(weeklyTime, format, null) < currentTime)
        {

        };
        ChangeWeeklyItemSlider();
        SetValue();
    }

    void Update()
    {
        if (isCountingDown)
        {
            UpdateCountdownDisplay();
        }
    }

    void UpdateCountdownDisplay()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeRemaining = deadLine - currentTime;
        timer.text = $"{timeRemaining.Minutes}:{timeRemaining.Seconds}";
        // string day = timeRemaining.Days > 9 ? $"{timeRemaining.Days}" : $"0{timeRemaining.Days}";
        string hours = timeRemaining.Hours > 9 ? $"{timeRemaining.Hours}" : $"0{timeRemaining.Hours}";
        string minutes = timeRemaining.Minutes > 9 ? $"{timeRemaining.Minutes}" : $"0{timeRemaining.Minutes}";
        string seconds = timeRemaining.Seconds > 9 ? $"{timeRemaining.Seconds}" : $"0{timeRemaining.Seconds}";

        if(timeRemaining.Days > 0) timer.text = $"{timeRemaining.Days}:{hours}:{minutes}:{seconds}";
        else if(timeRemaining.Days == 0) timer.text = $"{hours}:{minutes}:{seconds}";

        if (timeRemaining.Days <= 0 && timeRemaining.Hours <= 0 && timeRemaining.Minutes <= 0 && timeRemaining.Seconds <= 0)
        {
            PauseCountdown();
            FinishCountdown();
        }
    }

    public void StartCountdown()
    {
        isCountingDown = true;
    }

    public void PauseCountdown()
    {
        isCountingDown = false;

    }

    // public void ResetCountdown()
    // {
    //     isCountingDown = false;
    //     timeRemaining = totalTime;
    //     UpdateCountdownDisplay();
    // }

    void FinishCountdown()
    {
        // Thêm logic khi đếm ngược kết thúc ở đây
        CheckWeeklyReward();
    }

    public void PlayInAnimation()
    {
        if(hand.activeSelf)
        {
            hand.SetActive(false);
            bg.SetActive(false);
        }
        gameObject.SetActive(true);
        RectTransform rect = gameObject.GetComponent<RectTransform>();


        rect.anchoredPosition = new Vector2(-gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 0);

        rect.DOAnchorPos(new Vector2(0, 0), 0.5f).SetEase(Ease.OutCubic).OnComplete(() => {
            enabledToClick = true;
            Home.GetComponent<Home>().TurnOnBtn();
        });
    }

    public void Exit()
    {
        if(!enabledToClick) return;
        enabledToClick = false;
        audioManager.PlaySFX("click");
        Home.GetComponent<Home>().TurnOffBtn();
        gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 0), 0.5f).SetEase(Ease.InCubic).OnComplete(() => {
            gameObject.SetActive(false);
            Home.GetComponent<Home>().TurnOnBtn();
        });
    }

    public void TakeReward(GameObject item)
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        item.GetComponent<Button>().enabled = false;
        item.transform.GetChild(3).gameObject.SetActive(false);

        bool isVIP = false;
        // Debug.Log(item.transform.GetSiblingIndex());
        if(item.transform.parent.name == "Pass") 
        {
            isVIP = true;
            saveDataJson.SaveData("RewardWeeklyVipTook", item.transform.GetSiblingIndex());
        }
        else saveDataJson.SaveData("RewardWeeklyTook", item.transform.GetSiblingIndex());
        ChangeRewardSlider();
        
        switch (item.transform.GetSiblingIndex())
        {
            case 1: shop.AddMoreStuff(20,0,0, isVIP); break;
            case 2: shop.AddMoreStuff(0,1,0, isVIP); break;
            case 3: shop.AddMoreStuff(30,0,0, isVIP); break;
            case 4: shop.AddMoreStuff(0,1,0, isVIP); break;
            case 5: shop.AddMoreStuff(0,0,1, isVIP); break;
            case 6: shop.AddMoreStuff(30,0,0, isVIP); break;
            case 7: shop.AddMoreStuff(0,1,0, isVIP); break;
            case 8: shop.AddMoreStuff(40,0,0, isVIP); break;
            case 9: shop.AddMoreStuff(0,1,0, isVIP); break;
            case 10: shop.AddMoreStuff(0,2,1, isVIP); break;
            case 11: shop.AddMoreStuff(50,0,0, isVIP); break;
            case 12: shop.AddMoreStuff(0,2,0, isVIP); break;
            case 13: shop.AddMoreStuff(50,0,0, isVIP); break;
            case 14: shop.AddMoreStuff(0,2,0, isVIP); break;
            case 15: shop.AddMoreStuff(60,0,0, isVIP); break;
            case 16: shop.AddMoreStuff(0,2,0, isVIP); break;
            case 17: shop.AddMoreStuff(0,0,1, isVIP); break;
            case 18: shop.AddMoreStuff(80,0,0, isVIP); break;
            case 19: shop.AddMoreStuff(90,0,0, isVIP); break;
            case 20: shop.AddMoreStuff(100,3,2, isVIP); break;
        }
    }

    public void ExitVipRewardDialog(string txt = "")
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        Transform board = VipRewardDialog.transform.GetChild(1);
        enabledToClick = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.5f).OnComplete(() => {
            enabledToClick = true;
            VipRewardDialog.SetActive(false);
        });

        if(txt == "bought")
        {
            BtnUnlock.SetActive(false);
            BtnUnlock.transform.parent.GetComponent<Button>().enabled = false;
            SetValue();
        }
    }

    public void OpenVipRewardDialog()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");

        Transform board = VipRewardDialog.transform.GetChild(1);
        enabledToClick = false;
        board.localScale = new Vector3(0,0,1);
        VipRewardDialog.SetActive(true);
        board.DOPause();
        board.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            enabledToClick = true;
        });
    }
}
