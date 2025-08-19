using System.Collections;
using System.Linq;
using DG.Tweening;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;
    public Game game;
    public GameObject home;
    public AdsManager adsManager;
    public GameObject MaskUI;

    public GameObject UpdateVersion;
    public GameObject LegendarySUB;

    private bool isStartGame = false;
    private bool loadedOpenAd = true;
    private bool checkUpdate = false;

    void Start()
    {
        StartCoroutine(ChekcStartGame(8f));

        MaskUI.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = gameObject.transform.parent.parent.GetComponent<RectTransform>().sizeDelta;
        checkUpdate = true;
        // if(!Application.isEditor) StartCoroutine(GetVersion());
        home.SetActive(true);
        // home.GetComponent<Home>().LoadHomeScreen();
    }

    IEnumerator GetVersion()
    {
        checkUpdate = false;
        UnityWebRequest www = UnityWebRequest.Get("https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects");
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("web request failed");
            checkUpdate = true;
        }
        else
        {
            string responseText = www.downloadHandler.text;
            // if(www.downloadHandler.text == Application.version)
            if (responseText.Contains(Application.version))
            {
                Debug.Log("right version");
                checkUpdate = true;
            }
            else
            {
                Debug.Log("wrong version");
                UpdateVersion.SetActive(true);
            }
        }
    }

    public void UpdateVersionOfGame()
    {
        audioManager.PlaySFX("click");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private IEnumerator ChekcStartGame(float currentTime)
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f); // Đợi 1 giây
            currentTime -= 1f;
            // Debug.Log(currentTime);
            if(isStartGame) break;
        }

        if(!isStartGame){
            StartGame();
        }
    }

    public void SetOpenAdStatus(bool status)
    {
        loadedOpenAd = status;
    }

    public void ContinueLoading()
    {
        Invoke("StartGame", 1f);
    }

    void StartGame()
    {
        if(!checkUpdate){
            Invoke("StartGame", Time.deltaTime);
            return;
        }

        // adsManager.StopShowOpenAd();
        isStartGame = true;
        gameObject.SetActive(false);
        home.GetComponent<Home>().PlayMusic();
        int[] listOpenedMap = (int[]) saveDataJson.GetData("ListOpenedMap");
        adsManager.ChangeStatusStartGame();
        // if((int)saveDataJson.GetData("OpenedMap") >= 3 || (listOpenedMap != null && listOpenedMap.Contains(3)))
        // {
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                adsManager.LoadBannerAd();
            });
        // }

        if ((int)saveDataJson.GetData("OpenedMap") >= 2)
        {
            LegendarySUB.SetActive(true);
            Transform Btn = LegendarySUB.transform.GetChild(8);
            Btn.localScale = Vector3.one;
            Btn.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
