using UnityEngine;
using System.Collections;

public class FrameRateManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private bool useAdaptiveFrameRate = true;
    [SerializeField] private int minFrameRate = 30;
    [SerializeField] private float frameRateCheckInterval = 5f;
    [SerializeField] private int frameRateSampleSize = 10;

    private int[] fpsBuffer;
    private int fpsBufferIndex;

    private void Awake()
    {
        fpsBuffer = new int[frameRateSampleSize];
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0;
        if (useAdaptiveFrameRate)
        {
            StartCoroutine(AdaptiveFrameRateCoroutine());
        }
    }

    void Start()
    {
        // QualitySettings.vSyncCount = 0;
        // if (SystemInfo.processorFrequency < 2000)
        // {
        //     Application.targetFrameRate = 30;
        // }
        // else Application.targetFrameRate = 60;
    }

    private IEnumerator AdaptiveFrameRateCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(frameRateCheckInterval);
        while (true)
        {
            yield return wait;
            AdaptFrameRate();
        }
    }

    private void Update()
    {
        if (useAdaptiveFrameRate)
        {
            UpdateFPSBuffer();
        }
    }

    private void UpdateFPSBuffer()
    {
        fpsBuffer[fpsBufferIndex] = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
        fpsBufferIndex = (fpsBufferIndex + 1) % frameRateSampleSize;
    }

    private void AdaptFrameRate()
    {
        int averageFPS = CalculateAverageFPS();
        float batteryLevel = SystemInfo.batteryLevel;

        int newFrameRate;
        if (batteryLevel < 0.2f || averageFPS < minFrameRate)
        {
            newFrameRate = minFrameRate;
        }
        else if (averageFPS >= targetFrameRate)
        {
            newFrameRate = targetFrameRate;
        }
        else
        {
            newFrameRate = Mathf.Max(minFrameRate, averageFPS);
        }

        if (newFrameRate != Application.targetFrameRate)
        {
            Application.targetFrameRate = newFrameRate;
        }
    }

    private int CalculateAverageFPS()
    {
        int sum = 0;
        for (int i = 0; i < frameRateSampleSize; i++)
        {
            sum += fpsBuffer[i];
        }
        return sum / frameRateSampleSize;
    }
}