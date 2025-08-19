using TMPro;
using System;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System.Linq;
// using System.Linq;
// using Unity.VisualScripting;
// using UnityEditor.Localization.Plugins.XLIFF.V12;
// using UnityEngine.Localization.Settings;
// using System.Xml.Serialization;
// using Unity.VisualScripting;

public class ChallengeMode : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    private SaveDataJson.Challenge ChallengeData;

    public Camera cam;

    public ButtonControl buttonControl;

    public Canvas canvas;

    private GameObject ChallengeItem;
    public GameObject listMap;
    public GameObject clock;
    public GameObject clockStatistical;
    public GameObject ListItemToFind;
    public GameObject totalInTrophy;
    public GameObject totalInStatistical;
    public GameObject mapGame;
    public GameObject listCloud;
    public GameObject hand;
    public GameObject btnDown;
    public GameObject info;
    public GameObject GameScreen;
    public GameObject CompleteChallenge;
    public GameObject ChallengeDialog;
    public GameObject ExitChallengeDialog;
    public GameObject wrongItem;
    public GameObject InterAds;
    public GameObject Limited;
    public GameObject areaName;
    public GameObject challengeList;
    public ChangeScreenAnimation cloudAnimation;
    public ControlImageFromResources controlImage;

    private float leftLimit = -4.184f;
    private float rightLimit = 4.184f;
    private float bottomLimit = -6.34f;
    private float upperLimit = 10.34f;

    private float leftLimit2 = 0;
    private float bottomLimit2 = 0;

    private int totalCurent = 0;

    private bool onSpecialTop = false;
    private bool beTouched = true;

    // biến điều khiển screen
    private bool _isDragActive = false;
    private Vector2 _screenPosition;
    private Vector3 _worldPosition;
    private Vector3 touchPos;

    private dragable _lastDragged; 
    // private bool _draggMove = false;
    private Vector2 _checkPos;
    private bool _isScreenChange = false;
    private Vector3 offset;
    private Vector3 offset2;
    private float zoomMin = 3;
    private float zoomMax = 7;
    private int fistFingerId = 0;
    private bool moveAllowed;
    private bool stopCountDown = false;
    private bool pauseCountDown = false;

    private Vector3 testPos;
    public TextMeshProUGUI hint;
    public TextMeshProUGUI compassTxt;
    public WeeklyReward weeklyReward;
    private GameObject SoulFireList;

    public GameObject Compass;


    void Awake()
    {
        ChallengeMode[] controllers = FindObjectsOfType<ChallengeMode>();
        if(controllers.Length > 1)
        {
            Destroy(gameObject);
        }        
    }

    public Vector3 GetNewPositionLimit(Vector3 newPosition)
    {
        newPosition = new Vector3
        (
            x:Mathf.Clamp(value:newPosition.x, min:leftLimit, max:rightLimit),
            y:Mathf.Clamp(value:newPosition.y, min:bottomLimit, max:upperLimit),
            newPosition.z
        );
        return newPosition;
    }

    void ShowFooter()
    {
        Transform footer = btnDown.transform.parent.parent;
        footer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 140);

        btnDown.SetActive(true);
        footer.Find("Status/Up").gameObject.SetActive(false);
        footer.Find("Statistical").gameObject.SetActive(false);

        GameObject btnSetting = GameScreen.transform.Find("Bt_Setting").gameObject;
        GameObject btnHome = GameScreen.transform.Find("Bt_home").gameObject;
        GameObject giftIcon = GameScreen.transform.Find("GiftIcon").gameObject;
        GameObject btnCollection = GameScreen.transform.Find("CollectionIC").gameObject;
        btnSetting.SetActive(true);
        btnHome.SetActive(true);
        btnSetting.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, btnSetting.GetComponent<RectTransform>().anchoredPosition.y);
        giftIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, giftIcon.GetComponent<RectTransform>().anchoredPosition.y);
        btnHome.GetComponent<RectTransform>().anchoredPosition = new Vector2(85, btnHome.GetComponent<RectTransform>().anchoredPosition.y);
        btnCollection.GetComponent<RectTransform>().anchoredPosition = new Vector2(85, btnCollection.GetComponent<RectTransform>().anchoredPosition.y);
    }

    void CheckSoulFifeList()
    {
        SoulFireList.SetActive(true);
        string[] soulFireFound = (string[])saveDataJson.GetData("SoulFireFound");
        if(soulFireFound == null) return;
        int size = SoulFireList.transform.childCount;
        for(int i = 0; i < size; i++)
        {
            Transform child = SoulFireList.transform.GetChild(i);
            if(soulFireFound.Contains(child.name)) child.gameObject.SetActive(false);
            else child.gameObject.SetActive(true);
        }
    }

    int MapNum;
    public void LoadChallenge(string mapName, int level)
    {
        MapNum = int.Parse(mapName.Split("Challenge")[1]);
        stopCountDown = false;
        pauseCountDown = false;
        SaveDataJson.Challenge[] challengeListData = saveDataJson.TakeChallengeData().ChallengeMap;
        int challengeListDataLength = challengeListData.Length;
        // int currentChallenge = (int)saveDataJson.GetData(name);
        for (int i = 0; i < challengeListDataLength; i++)
        {
            if (challengeListData[i].challengeID == mapName && challengeListData[i].level == level)
            {
                ChallengeData = challengeListData[i];
                break;
            }
        }
        StartAllSkeletonAnimation();
        gameObject.GetComponent<Game>().SetChallengeData(ChallengeData);

        float currentTime = ChallengeData.time;
        UpdateTimerDisplay(currentTime);
        StartCoroutine(ChangeNameArea(saveDataJson.TakeMapData().map[ChallengeData.map].AreaName));
        SetChallengeValue();
        ChallengeDialog.SetActive(true);
        ChallengeDialog.GetComponent<ChallengeDialog>().SetInfo(ChallengeData.challengeID, ChallengeData.map, ChallengeData.item);
    }

    IEnumerator ChangeNameArea (string txt)
    {
        yield return LocalizationSettings.InitializationOperation;

        areaName.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt);
        clock.transform.parent.GetChild(1).GetChild(1).GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ChallengeData.item);
    }

    public void StartChallenge()
    {
        // checkLimited();
        OpenChallengeArea();
    }

    void SetChallengeValue()
    {
        listMap.transform.Find($"Map{ChallengeData.map}").gameObject.SetActive(true);
        ChallengeItem = GameObject.Find($"Game/MainGame/Map/Map{ChallengeData.map}/ChallengeItem/{ChallengeData.challengeID}");
        SoulFireList = GameObject.Find($"Game/MainGame/Map/Map{ChallengeData.map}/SoulFifeChallengeList/{ChallengeData.challengeID}");

        clock.transform.parent.gameObject.SetActive(true);
        ChallengeItem.SetActive(true);
        ListItemToFind.SetActive(false);
        clockStatistical.SetActive(true);
        
        totalCurent = ChallengeData.quantity;
        totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"0/{totalCurent}";
        totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"0/{totalCurent}";
        gameObject.GetComponent<Game>().SetProgressBar(0, totalCurent, "set");

        hint.text = $"{(int)saveDataJson.GetData("Hint")}";
        compassTxt.text = $"{(int)saveDataJson.GetData("Compass")}";

        if((int)saveDataJson.GetData("OpenedMap") >= 2) Compass.SetActive(true);
        CheckSoulFifeList();
    }

    public void ResetData()
    {
        StopAllSkeletonAnimation();
        ShowFooter();
        leftLimit = -4.184f;
        rightLimit = 4.184f;
        bottomLimit = -6.34f;
        upperLimit = 10.34f;
        buttonControl.OnBtn();
        cam.orthographicSize = 7;
        info.SetActive(false);
        hand.SetActive(false);
        wrongItem.SetActive(false);
        clock.transform.parent.gameObject.SetActive(false);
        ListItemToFind.SetActive(true);
        clockStatistical.SetActive(false);
        SoulFireList.SetActive(false);
        Compass.SetActive(false);

        isCountTimeTakeItem = false;
        stopCountTimeTakeItem = true;
        resetCountTimeTakeIte = false;
        pauseCountTimeTakeItem = false;

        mapGame.transform.position = new Vector3(0,0,mapGame.transform.position.z);
        foreach (Transform child in ChallengeItem.transform)
        {
            if(child.GetComponent<SkeletonAnimation>())
                child.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0,"Idle",true);
            child.gameObject.SetActive(true);
            child.GetComponent<dragable>().finded = false;
        }
        ChallengeItem.SetActive(false);
        listMap.transform.Find($"Map{ChallengeData.map}").gameObject.SetActive(false);
        gameObject.GetComponent<Game>().enabled = true;
        gameObject.GetComponent<ChallengeMode>().enabled = false;
    }

    void OpenChallengeArea()
    {
        int num = ChallengeData.area.Length;
        string txt = "";
        for (int i = num - 1; i >= 0; i--)
        {
            txt = i == num - 1 ? "first" : "";
            PlayAnimationCloud(ChallengeData.area[i] - 1, txt);
        }

        float _height = 2f * cam.orthographicSize;
        float _width = _height * cam.aspect;

        if(num == 2){
            if( ChallengeData.area[1] == 2)
            {
                rightLimit = Limited.transform.GetChild(1).position.x - _width / 2;
                leftLimit = - rightLimit;
                bottomLimit = 6f;
                upperLimit = 10.34f;
                leftLimit2 = 0f;
                bottomLimit2 = 0f;
            }
            else if(ChallengeData.area[1] == 4)
            {
                rightLimit = Limited.transform.GetChild(1).position.x - _width / 2;
                leftLimit = - rightLimit;
                bottomLimit = -(Limited.transform.GetChild(2).position.y - _height / 2);
                upperLimit = -2.34f;
                leftLimit2 = 0f;
                bottomLimit2 = 0f;
            }
        }
        else if (num == 1)
        {
            num = ChallengeData.area[0];
            if(num == 1)
            {
                leftLimit = 3f;
                rightLimit = Limited.transform.GetChild(1).position.x - _width / 2;
                bottomLimit = 6f;
                upperLimit = 10.34f;
                leftLimit2 = 0f;
                bottomLimit2 = 0f;
            }
            else if(num == 2)
            {
                leftLimit = -(Limited.transform.GetChild(1).position.x - _width / 2);
                rightLimit = -3f;
                bottomLimit = 6f;
                upperLimit = 10.34f;
                leftLimit2 = 0f;
                bottomLimit2 = 0f;
            }
            else if(num == 3){
                leftLimit = 3f;
                rightLimit = Limited.transform.GetChild(1).position.x - _width / 2;
                bottomLimit = -(Limited.transform.GetChild(2).position.y - _height / 2);
                upperLimit = -2.34f;
                leftLimit2 = 0f;
                bottomLimit2 = 0f;
            }
            else
            {
                leftLimit = -(Limited.transform.GetChild(1).position.x - _width / 2);
                rightLimit = -3f;
                bottomLimit = -(Limited.transform.GetChild(2).position.y - _height / 2);
                upperLimit = -2.34f;
                leftLimit2 = 0f;
                bottomLimit2 = 0f;
            }
        }

        if(cam.orthographicSize != 5){
            cam.orthographicSize = 5;
        }
        ChangeLimited(7);
    }

    void checkLimited()
    {
        
    }

    void SetLimited(GameObject obj)
    {
        float left = leftLimit;
        float bottom = bottomLimit;

        if(leftLimit2 != 0 && obj.transform.position.x < leftLimit2 && !onSpecialTop) bottom = bottomLimit2;
        else if(bottomLimit2 != 0 && obj.transform.position.y < bottomLimit2)
        {
            left = leftLimit2;
            onSpecialTop = true;
        }
        else onSpecialTop = false;

        obj.transform.position = new Vector3
        (
            x:Mathf.Clamp(value:obj.transform.position.x, min:left, max:rightLimit),
            y:Mathf.Clamp(value:obj.transform.position.y, min:bottom, max:upperLimit),
            obj.transform.position.z
        );
    }

    public void ContinueCountDown()
    {
        pauseCountDown = false;
    }

    public void PauseCountDown()
    {
        pauseCountDown = true;
    }

    private IEnumerator Countdown(float currentTime)
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f); // Đợi 1 giây
            if(stopCountDown) break;

            if(!pauseCountDown) currentTime -= 1f; // Giảm thời gian còn lại đi 1 giây
            UpdateTimerDisplay(currentTime); // Cập nhật hiển thị đồng hồ
        }
        if(!stopCountDown){
            // Khi thời gian kết thúc, thực hiện hành động tùy ý, ví dụ: Hiển thị thông báo, chuyển cảnh, v.v.
            PlayCloundAnimation("fail");
        }
    }

    private void UpdateTimerDisplay(float currentTime)
    {
        // Format thời gian còn lại thành dạng phút:giây
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Cập nhật văn bản trên UI
        clock.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = timerString;
        clockStatistical.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = timerString;
    }

    public void PlayAnimationCloud(int areaNum = 0, string txt = "")
    {
        Vector3 pos = new Vector3(3.75f, 6.675f, mapGame.transform.position.z);

        if(areaNum == 1) pos = new Vector3(-3.75f, 6.675f, mapGame.transform.position.z);
        else if(areaNum == 2) pos = new Vector3(3.75f, -6.675f, mapGame.transform.position.z);
        else if(areaNum == 3) pos = new Vector3(-3.75f, -6.675f, mapGame.transform.position.z);

        pos = new Vector3
        (
            x:Mathf.Clamp(value:pos.x, min:leftLimit, max:rightLimit),
            y:Mathf.Clamp(value:pos.y, min:bottomLimit, max:upperLimit),
            pos.z
        );

        mapGame.transform.DOMove(pos, 0.5f).SetDelay(0.5f).OnComplete(() => CloudAnimation(areaNum, txt));
    }

    void CloudAnimation(int areaNum, string txt)
    {
        beTouched = false;
        buttonControl.OffBtn();
        Transform clouds = listCloud.transform.GetChild(areaNum);
        audioManager.PlaySFX("cloud_open");
        for(int i = 0; i < 3; i++)
        {
            SkeletonAnimation cloud = clouds.GetChild(i).GetComponent<SkeletonAnimation>();
            cloud.timeScale = 1;
            clouds.GetChild(i).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Tan", false);
            // if(i == 0 && txt == "first") clouds.GetChild(i).GetComponent<SkeletonAnimation>().AnimationState.Complete += AnimationCompleteHandlerChallenge;
        }
        if(txt == "first") Invoke("AnimationCompleteHandlerChallenge", 1.167f);
    }

    void AnimationCompleteHandlerChallenge()
    {
        beTouched = true;
        buttonControl.OnBtn();
        float currentTime = ChallengeData.time;
        // StartCoroutine(adsManager.CountDownForHintSale());
        StartCoroutine(adsManager.CountDownForComboDeluxe("first"));

        StartCoroutine(Countdown(currentTime));
        
        stopCountTimeTakeItem = false;
        if (!isCountTimeTakeItem)
        {
            isCountTimeTakeItem = true;
            StartCoroutine(CountTimeTakeItem());
        }
    }

    bool isCountTimeTakeItem = false;
    bool stopCountTimeTakeItem = true;
    bool resetCountTimeTakeIte = false;
    bool pauseCountTimeTakeItem = false;

    IEnumerator CountTimeTakeItem(int currentTime = 90)
    {
        Transform hintBtn = hint.transform.parent;
        // Transform compassBtn = compassTxt.transform.parent;
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            if (stopCountTimeTakeItem) break;
            if (!pauseCountTimeTakeItem) currentTime -= 1;
            if (resetCountTimeTakeIte)
            {
                currentTime = 90;
                resetCountTimeTakeIte = false;
            }
        }

        if (hintBtn.gameObject.activeSelf)
        {
            hintBtn.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void StopBounceTool()
    {
        Transform hintBtn = hint.transform.parent;
        if (DOTween.IsTweening(hintBtn))
        {
            hintBtn.DOKill();
            hintBtn.localScale = Vector3.one;

            resetCountTimeTakeIte = false;
            StartCoroutine(CountTimeTakeItem());
        } else resetCountTimeTakeIte = true;
    }

    void StopAllSkeletonAnimation()
    {
        GameObject AnimationList = GameObject.Find($"Game/MainGame/Map/Map{MapNum}/AnimationList");

        foreach (Transform child in AnimationList.transform)
        {
            child.GetComponent<SkeletonAnimation>().AnimationState.ClearTracks();
        }
    }

    void StartAllSkeletonAnimation()
    {
        GameObject AnimationList = GameObject.Find($"Game/MainGame/Map/Map{MapNum}/AnimationList");

        foreach (Transform child in AnimationList.transform)
        {
            child.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "animation", true);
        }
    }

    void Update()
    {
        // CheckHiddenItem();
        if(!beTouched) return;
        // if(Input.touchCount > 0) infoItem.SetActive(false);
        HandleTouchItems();
        HandleZoom();
    }

    void HandleTouchItems()
    {
        if(_isDragActive && (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
        {
            Drop();
            return;
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            _screenPosition = new Vector2(mousePos.x, mousePos.y);
        }
        else if (Input.touchCount > 0) _screenPosition = Input.GetTouch(0).position;
        else return;

        _worldPosition = Camera.main.ScreenToWorldPoint(_screenPosition);
        if(_isDragActive) Drag();
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(_worldPosition, Vector2.zero);
            if(hit.collider != null)
            {
                dragable dragable = hit.transform.gameObject.GetComponent<dragable>();
                if(dragable != null)
                {
                    _lastDragged = dragable;
                    InitDrag();
                }
            }
            else
            {
                if(Input.GetTouch(0).phase == TouchPhase.Ended && !_isScreenChange){
                    wrongItem.SetActive(true);
                    StartCoroutine(PlayAnimationWrongItem());
                }
            }
        }
    }

    IEnumerator PlayAnimationWrongItem()
    {
        wrongItem.transform.DOPause();
        wrongItem.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        Vector3 pos = cam.ScreenToWorldPoint((Vector3)Input.GetTouch(0).position);
        wrongItem.transform.position = new Vector3(pos.x, pos.y, wrongItem.transform.position.z);
        yield return new WaitForSeconds(0.2f);
        wrongItem.transform.DOScale(new Vector3(0f,0f,1f), 0.5f).SetEase(Ease.InBounce).OnComplete(() => wrongItem.SetActive(false));
    }

    void InitDrag()
    {
        _isDragActive = true;
        // _draggMove = false;
        _checkPos = Input.GetTouch(0).position;
    }

    void Drag()
    {   
        // if(_checkPos != Input.GetTouch(0).position) _draggMove = true;
    }

    void Drop()
    {
        dragable item = _lastDragged;
        if(!_isScreenChange && !item.finded)
        {
            if(item.transform.parent.parent.name == "SoulFifeChallengeList")
            {
                FoundSoulFire(item);
            }
            else if(item.transform.parent.name == ChallengeData.challengeID)
            {
                audioManager.PlaySFX("click");
                adsManager.VibrationDevice();
                if(hand.name == item.name) StopHandAnimation();

                SkeletonAnimation itemAnimation = item.GetComponent<SkeletonAnimation>();
                float waitTime = 0.5f;
                int currentOrder = item.GetComponent<Renderer>().sortingOrder;
                item.finded = true;
                item.GetComponent<Renderer>().sortingOrder = 110;
                if(itemAnimation != null) 
                {
                    itemAnimation.AnimationState.SetAnimation(0,"animation",false);
                    waitTime = item.GetComponent<SkeletonAnimation>().Skeleton.Data.FindAnimation("animation").Duration;
                }
                item.transform.DOScale(new Vector3(1.5f,1.5f,1.5f), 0.5f);

                StartCoroutine(CompleteAnimationItem(item, currentOrder, waitTime));

                ShowInfoScore(item);

                string itemName = item.name.Split(" (")[0];
                checkValue(item, itemName);
            }
        }
        _isDragActive = false;
        // _draggMove = false;
    }

    IEnumerator CompleteAnimationItem (dragable item, int currentOrder, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        item.GetComponent<Renderer>().sortingOrder = currentOrder;
        item.gameObject.SetActive(false);
        item.transform.localScale = new Vector3(1,1,1);
    }

    void FoundSoulFire (dragable item)
    {
        if(item.finded) return;
        audioManager.PlaySFX("collect_item");
        item.finded = true;
        saveDataJson.SaveData("SoulFireFound", item.name);
        weeklyReward.ChangeWeeklyItemSlider();
        weeklyReward.ChangeRewardSlider();

        int currentOrder = item.GetComponent<Renderer>().sortingOrder;
        item.GetComponent<Renderer>().sortingOrder = 110;
        item.transform.DOScale(new Vector3(1.2f,1.2f,1), 0.5f).OnComplete(() => {
            item.transform.DOScale(new Vector3(0,0,1), 0.5f).SetDelay(1f).OnComplete(() => {
                item.gameObject.SetActive(false);
                item.GetComponent<Renderer>().sortingOrder = currentOrder;

                item.transform.localScale = new Vector3(1,1,1);
            });
        });
    }

    void HandleZoom()
    {
        if(Input.touchCount > 0)
        {
            if(Input.touchCount == 2){
                _isScreenChange = true;
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                TakeOffset(touch1, "2");

                if(EventSystem.current &&
                    (EventSystem.current.IsPointerOverGameObject(touch1.fingerId)
                    || EventSystem.current.IsPointerOverGameObject(touch0.fingerId))) return;

                Vector2 touch0LastPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1LastPos = touch1.position - touch1.deltaPosition;

                float distTouch = (touch0LastPos - touch1LastPos).magnitude;
                float curretDistTouch = (touch0.position - touch1.position).magnitude;

                float difference = curretDistTouch - distTouch;

                Zoom(difference * 0.003f);
                if(touch0.phase == TouchPhase.Ended && touch1.phase == TouchPhase.Ended) _isScreenChange = false;
            }
            else
            {
                // infoItem.SetActive(false);
                Touch touch = Input.GetTouch(0);
                // Debug.Log(touch.fingerId);
                if(touch.fingerId != fistFingerId) {
                    // Debug.Log(touch.fingerId + "/" + fistFingerId);
                    offset = offset2;
                    fistFingerId = touch.fingerId;
                };
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if(EventSystem.current && 
                            EventSystem.current.IsPointerOverGameObject(touch.fingerId)) moveAllowed = false;
                        else moveAllowed = true;
                        _isScreenChange = false;
                        fistFingerId = touch.fingerId;
                        testPos = cam.ScreenToWorldPoint(touch.position);
                        TakeOffset(touch);
                        break;
                    case TouchPhase.Moved:
                        if(moveAllowed)
                        {
                            if(Vector3.Distance(testPos, cam.ScreenToWorldPoint(touch.position)) >= 0.1) _isScreenChange = true;

                            Vector3 currentPos = cam.ScreenToWorldPoint(touch.position);
                            currentPos.z = cam.transform.position.z; // Đảm bảo cùng một lớp z
                            mapGame.transform.position = currentPos - offset;
                            SetLimited(mapGame);
                        }
                        break;
                    case TouchPhase.Ended:
                        _isScreenChange = false;
                        break;
                    default: 
                    break;
                }                
            }
        }
    }

    void TakeOffset(Touch touch, string val = null)
    {
        touchPos = cam.ScreenToWorldPoint((Vector3)touch.position);
        touchPos.z = cam.transform.position.z; // Đảm bảo cùng một lớp z
        // offset = touchPos - mapGame.transform.position;
        if(val == null) offset = touchPos - mapGame.transform.position;
        else offset2 = touchPos - mapGame.transform.position;
    }

    private void checkValue(dragable item, string itemName) 
    {
        StopBounceTool();

        int currentTotal = 
            Convert.ToInt32(totalInTrophy.GetComponent<TextMeshProUGUI>().text.Split("/")[0]) + 1;

        totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"{currentTotal}/{totalCurent}";
        totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"{currentTotal}/{totalCurent}";
        gameObject.GetComponent<Game>().SetProgressBar(currentTotal, totalCurent);
    }

    void ShowInfoScore(dragable item)
    {
        Image obj = info.GetComponent<Image>();
        Image obj1 = info.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI obj2 = info.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI obj3 = info.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        info.transform.position = item.transform.position + new Vector3(0,1f,0);
        info.SetActive(true);

        string txt = item.name.Split(" (")[0];
        info.transform.GetChild(1).GetComponent<LocalizeStringEvent>()
            .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ChallengeData.item);

        obj1.sprite = controlImage.TakeImage(controlImage.ListItem, txt);
        obj1.SetNativeSize();
        obj1.transform.localScale = new Vector3(0.9f,0.9f,1);

        TextMeshProUGUI targetTxt = totalInTrophy.GetComponent<TextMeshProUGUI>();
        int currentQuantity = Convert.ToInt32(targetTxt.text.Split("/")[0]) + 1;
        obj3.text = $"{currentQuantity}/{totalCurent}";
        obj.DOPause();
        obj1.DOPause();
        obj2.DOPause();
        obj3.DOPause();

        obj.DOFade(1f, 0.1f);
        obj1.DOFade(1f, 0.1f);
        obj2.DOFade(1f, 0.1f);
        obj3.DOFade(1f, 0.1f).OnComplete(() => {
            obj.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj1.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj2.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj3.DOFade(0f, 0.5f).SetDelay(0.7f).OnComplete(() => info.SetActive(false));
        });

        if(currentQuantity == totalCurent){
            PlayCloundAnimation("win");
        }
    }

    void StopHandAnimation()
    {
        hand.SetActive(false);
    }

    private void Zoom(float increment)
    {
        float currentCamSize = cam.orthographicSize;
        // cam.GetComponent<ViewportHandler>().ChangeUnitsSize(increment, zoomMin, zoomMax);
        cam.orthographicSize = Mathf.Clamp(value:cam.orthographicSize - increment, zoomMin, zoomMax);
        
        ChangeLimited(currentCamSize);
    }

    public void ChangeLimited(float currentCamSize, float val = 0)
    {
        if (val == 0) val = cam.orthographicSize;
        upperLimit += currentCamSize - val;
        bottomLimit -= currentCamSize - val;
        leftLimit -= (currentCamSize - val)*cam.aspect;
        rightLimit += (currentCamSize - val)*cam.aspect;

        if(bottomLimit2 != 0) bottomLimit2 -= currentCamSize - val;
        if(leftLimit2 != 0) leftLimit2 -= (currentCamSize - val)*cam.aspect;
        SetLimited(mapGame);
    }

    public void PlayCloundAnimation(string txt)
    {
        beTouched = false;
        buttonControl.OffBtn();
        stopCountDown = true;
        // adsManager.StopCountDownInter();
        adsManager.StopCountDownForHintSale();
        audioManager.PlaySFX("cloud_close");
        for(int i = 0; i < 4; i++) 
        {
            foreach(Transform child in listCloud.transform.GetChild(i))
            {
                SkeletonAnimation cloud = child.GetComponent<SkeletonAnimation>();
                cloud.AnimationState.SetAnimation(0, "Idle", true);
                cloud.timeScale = 0.1f;
            }
        }

        // if(info != null) Invoke("ComeBackHome", 2f);
        // else Invoke("GameComplete", 2f);
        if(txt == "win") 
        {
            Invoke("WinChallenge", 2f);
            cloudAnimation.PlayChangeScreenAnimation("WinChallenge");    
        }
        else if(txt == "fail") Invoke("FailedChallenge", 2f);
        else if(txt == "return home") {
            cloudAnimation.PlayChangeScreenAnimation("ComeBackHome");
            Invoke("ComeBackHome", 2f);
        }
    }

    public void ComeBackHome()
    {
        ResetData();
        challengeList.SetActive(true);
        buttonControl.ReturnToHome("ChallengeMode");
    }

    void WinChallenge()
    {
        gameObject.SetActive(false);
        GameScreen.SetActive(false);
        CompleteChallenge.SetActive(true);

        CompleteChallenge.GetComponent<CompleteChallenge>().SetImage(ChallengeData);

        ResetData();
    }

    void FailedChallenge()
    {
        adsManager.LogEvent($"FailedChallenge{ChallengeData.challengeID}_{ChallengeData.level}");
        ChallengeDialog.SetActive(true);
        ChallengeDialog.GetComponent<ChallengeDialog>().SetInfo();
        // ResetData();
    }
}
