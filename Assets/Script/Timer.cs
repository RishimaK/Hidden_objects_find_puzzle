using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    // public float totalTime = 60f; // Tổng thời gian đếm ngược (giây)
    public TextMeshProUGUI countdownTextInBtn; // Reference đến Text UI để hiển thị thời gian
    public TextMeshProUGUI countdownTextInSaleDialog; // Reference đến Text UI để hiển thị thời gian

    private float timeRemaining;
    private DateTime deadLine;
    public bool isCountingDown = false;

    public GameObject Sale;

    void Start()
    {
        if((bool)saveDataJson.GetData("RemoveAds")){
            Sale.SetActive(false);
            return;
        }
        // timeRemaining = totalTime;
        DateTime currentTime = DateTime.Now;
        // string currentTimeFormatted = FormatDateTime(currentTime);

        string saleTime = (string)saveDataJson.GetData("SaleTime");
        string format = "dd/MM/yyyy HH:mm:ss";

        if(saleTime == null || saleTime == "" || DateTime.ParseExact(saleTime, format, null).Date < currentTime.Date)
        {
            // chưa set time hoặc set rồi mà khác ngày
            deadLine = currentTime.AddHours(1);
            string deadLineFormatted = FormatDateTime(deadLine);
            saveDataJson.SaveData("SaleTime", deadLineFormatted);
        }else if(DateTime.ParseExact(saleTime, format, null).Date == currentTime.Date) deadLine = DateTime.ParseExact(saleTime, format, null);
        
        TimeSpan timeRemaining = deadLine - currentTime;
        if(timeRemaining.Hours > 0 || timeRemaining.Minutes > 0 || timeRemaining.Seconds > 0)
        {
            //time đã set trùng ngày
            Sale.SetActive(true);
            UpdateCountdownDisplay();
            StartCountdown();
        }else if(timeRemaining.Hours <= 0 && timeRemaining.Minutes <= 0 && timeRemaining.Seconds <= 0) Sale.SetActive(false);
        // Debug.Log($"Hiện tại: {currentTimeFormatted}");
        // Debug.Log($"{35} giờ sau: {futureTimeFormatted}");
    }

    string FormatDateTime(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss");

    void Update()
    {
        if (isCountingDown)
        {
            // if (timeRemaining > 0)
            // {
                UpdateCountdownDisplay();
            // }
            // else
            // {
            //     FinishCountdown();
            // }
        }
    }

    void UpdateCountdownDisplay()
    {
        // if (timeRemaining < 0)
        //     timeRemaining = 0;

        // float minutes = Mathf.FloorToInt(timeRemaining / 60);
        // float seconds = Mathf.FloorToInt(timeRemaining % 60);
        

        // countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        DateTime currentTime = DateTime.Now;
        TimeSpan timeRemaining = deadLine - currentTime;
        string minutes = timeRemaining.Minutes > 9 ? $"{timeRemaining.Minutes}" : $"0{timeRemaining.Minutes}";
        string seconds = timeRemaining.Seconds > 9 ? $"{timeRemaining.Seconds}" : $"0{timeRemaining.Seconds}";

        countdownTextInBtn.text = $"{minutes}:{seconds}";
        countdownTextInSaleDialog.text = countdownTextInBtn.text;

        if (timeRemaining.Minutes <= 0 && timeRemaining.Seconds <= 0)
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
        // Debug.Log("Countdown finished!");
        isCountingDown = false;
        Sale.SetActive(false);
    }
}