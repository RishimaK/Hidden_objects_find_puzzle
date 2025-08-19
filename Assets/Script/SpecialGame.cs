using TMPro;
// using Spine;
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

public class SpecialGame : MonoBehaviour
{
    public AdsManager adsManager;
    public SaveDataJson saveDataJson;
    private SaveDataJson.SpecialData GameSpecialData;
    public AudioManager audioManager;

    // private float leftLimit;
    // private float rightLimit;
    // private float bottomLimit;
    // private float upperLimit;
    private float leftLimit = -4.184f;
    private float rightLimit = 4.184f;
    private float bottomLimit = -6.34f;
    private float upperLimit = 10.34f;

    private float leftLimit2 = 0;
    private float bottomLimit2 = 0;

    private float defaultRightLimit;
    private float defaultBottomLimit;

    private bool moveAllowed;
    private bool beTouched = true;
    private bool _draggMove = false;
    private bool onSpecialTop = false;
    private bool _isDragActive = false;
    private bool _isScreenChange = false;
    private bool addMoreItem = false;
    private bool showAllItem = false;
    private bool isChallenge = false;
    private bool isGameComplete = false;
    private bool mapAction = true;

    private Vector3 offset;
    private Vector3 offset2;
    private int fistFingerId = 0;
    private Vector3 touchPos;
    private Vector3 _worldPosition;

    private Vector2 _checkPos;
    private Vector2 _screenPosition;


    private dragable _lastDragged; 

    public Canvas canvas;

    public Camera cam;

    public ControlImageFromResources controlImage;

    private float zoomMin = 3;
    private float zoomMax = 7;

    private int totalItem = 0;
    private int totalCurent = 0;
    public int MapNum;
    private int gift;
    private int currentTotalItem = 0;

    public Shop shop;

    public GameObject Compass;
    public GameObject hand;
    public GameObject home;
    public GameObject info;
    public GameObject mapGame;
    public GameObject listMap;
    public GameObject NewArea;
    public GameObject btnDown;
    public GameObject areaName;
    public GameObject InterAds;
    public GameObject infoItem;
    public GameObject listCloud;
    public GameObject listSpecialCloud;
    public GameObject gameScreen;
    public GameObject ItemPrefab;
    public GameObject totalInTrophy;
    public GameObject ListItemToFind;
    public GameObject completeDialog;
    public GameObject totalInStatistical;
    public GameObject wrongItem;
    public GameObject Limited;
    public GameObject OpenedGift;
    public GameObject PoolItem;
    public GameObject MaskUI;
    public GameObject LightEff;
    public GameObject ListParticle;
    public GameObject MainParticle;
    // public GameObject cloudChangeScreen;
    private GameObject ChallengeItem;

    public ChangeScreenAnimation cloudAnimation;

    // private GameObject ListCard;
    private GameObject ListToFind;
    private GameObject ListItemFinded;
    private GameObject HiddenItemList;
    public Slider ProgressInTrophy;
    public Slider ProgressInStatistical;

    public ButtonControl buttonControl;

    public TextMeshProUGUI hint;
    public TextMeshProUGUI GiftIconTxt;
    public TextMeshProUGUI compassTxt;

    private Vector3 testPos;

    void Awake()
    {
        Game[] controllers = FindObjectsOfType<Game>();
        if(controllers.Length > 1)
        {
            Destroy(gameObject);
        }

        SetDefaultLimit();
    }

    void SetListItemToFind()
    {
        int listItemToFindLength = ListItemToFind.transform.parent.childCount;
        if(listItemToFindLength <= 1) return;
        for (int i = 1; i < listItemToFindLength;)
        {
            listItemToFindLength--;
            Transform child = ListItemToFind.transform.parent.GetChild(i);
            child.SetParent(ListItemToFind.transform);
        }

        for (int i = 0; i< ListItemToFind.transform.childCount; i++)
        {
            RectTransform child = ListItemToFind.transform.GetChild(i).GetComponent<RectTransform>();
            child.anchoredPosition = new Vector2(45 + 120 * i, child.anchoredPosition.y);
        }
    }

    private void SetValue()
    {
        string[] itemList = GameSpecialData.ItemList;
        int[] QuantityList = GameSpecialData.QuantityList;
        int numItem = itemList.Length;
        int findedItem = 0;
        ListItemToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(120f * numItem, 130);

        for(int i = 0; i < numItem; i++)
        {
            GameObject newItem = ObjectPoolManager.SpawnObject(ItemPrefab, new Vector3(0,0,-9.5f),Quaternion.identity);
            RectTransform itemTransform = newItem.GetComponent<RectTransform>();
            itemTransform.SetParent(ListItemToFind.transform);
            itemTransform.sizeDelta = new Vector2(90,90);
            itemTransform.localScale = new Vector3(1,1,1);
            newItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(45 + 120 * i, 11);

            Image img = newItem.transform.GetChild(1).GetComponent<Image>();
            img.sprite = controlImage.TakeImage(controlImage.ListItem, itemList[i]);
            // Debug.Log(img.sprite);
            img.SetNativeSize();
            img.rectTransform.sizeDelta *= 100;

            newItem.name = itemList[i];
 
            int count = 0;
            if(ListItemFinded.transform.childCount > 0){
                foreach (Transform obj in ListItemFinded.transform)
                {
                    if(newItem.name == obj.gameObject.name.Split(" (")[0]) count++;
                }
            }
            findedItem += count;
            newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{count}/{QuantityList[i]}";
            if(count == QuantityList[i]) {
                newItem.transform.GetChild(2).gameObject.SetActive(true);
                itemTransform.SetParent(ListItemToFind.transform.parent);
            }
        }
        totalItem = 0;
        foreach(int num in QuantityList) totalItem += num;

        for (int i = 0; i < GameSpecialData.Milestones.Length; i++)
        {
            int child = GameSpecialData.Milestones[i];
            if (findedItem < child) {
                totalCurent = child;
                break;
            }
            else if(i == GameSpecialData.Milestones.Length - 1)
            {
                totalCurent = totalItem;
                break;
            }
        }

        currentTotalItem = findedItem;

        totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"{findedItem}/{totalCurent}";
        totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"{findedItem}/{totalCurent}";
        SetProgressBar(findedItem, totalCurent, "set");

        SetListItemToFind();
        isGameComplete = false;
        if(findedItem == totalCurent) isGameComplete = true;
    }

    public void SetProgressBar(float curent, float total, string txt = "") 
    {
        float val = curent / total;

        if(txt == "set")
        {
            ProgressInTrophy.value = val;
            ProgressInStatistical.value = val;
        }
        else
        {
            ProgressInTrophy.DOPause();
            ProgressInStatistical.DOPause();
            ProgressInTrophy.DOValue(val, 0.2f, false);
            ProgressInStatistical.DOValue(val, 0.2f, false);
        }
    }

    void CheckItemInMap()
    {
        string[] itemsFinded = (string[]) saveDataJson.GetData($"ItemSpecialMap{MapNum}");
        string[] itemsNeedToShow = (string[]) saveDataJson.GetData($"ShowHiddenItemsSpecial{MapNum}");

        if(itemsFinded == null || itemsFinded.Length == 0) return;
        foreach (string name in itemsFinded)
        {
            Transform child = ListToFind.transform.Find(name);
            if(!child) child = HiddenItemList.transform.Find(name);
            if(!child) continue;
            child.gameObject.SetActive(false);
            child.SetParent(ListItemFinded.transform);
        }

        if(itemsNeedToShow.Length > 0){
            foreach (string name in itemsNeedToShow)
            {
                Transform child = HiddenItemList.transform.Find(name);
                if(!child) continue;
                child.gameObject.SetActive(true);
                child.SetParent(ListToFind.transform);
            }
        }
    }

    void ShowFooter()
    {
        Transform footer = btnDown.transform.parent.parent;
        footer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 140);

        btnDown.SetActive(true);
        footer.Find("Status/Up").gameObject.SetActive(false);
        footer.Find("Statistical").gameObject.SetActive(false);

        GameObject btnSetting = gameScreen.transform.Find("Bt_Setting").gameObject;
        GameObject btnHome = gameScreen.transform.Find("Bt_home").gameObject;
        GameObject giftIcon = gameScreen.transform.Find("GiftIcon").gameObject;
        btnSetting.SetActive(true);
        btnHome.SetActive(true);

        btnSetting.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, btnSetting.GetComponent<RectTransform>().anchoredPosition.y);
        giftIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, giftIcon.GetComponent<RectTransform>().anchoredPosition.y);
        btnHome.GetComponent<RectTransform>().anchoredPosition = new Vector2(85, btnHome.GetComponent<RectTransform>().anchoredPosition.y);
        // btnCollection.GetComponent<RectTransform>().anchoredPosition = new Vector2(85, btnCollection.GetComponent<RectTransform>().anchoredPosition.y);
    }

    public void LoadGame(int mapNum) 
    {
        listCloud.SetActive(false);
        listSpecialCloud.SetActive(true);
        beTouched = false;
        GameSpecialData = saveDataJson.TakeGameSpecialData().SpecialMap[mapNum];
        MapNum = mapNum;
        // ListCard = GameObject.Find($"Game/MainGame/MapSpecial/Map{mapNum}/ListCard");
        ListToFind = GameObject.Find($"Game/MainGame/MapSpecial/Map{mapNum}/AvailableItemList");
        ListItemFinded = GameObject.Find($"Game/MainGame/MapSpecial/Map{mapNum}/ListItemFinded");
        HiddenItemList = GameObject.Find($"Game/MainGame/MapSpecial/Map{mapNum}/HiddenItemList");
        StartAllSkeletonAnimation();
        GiftIconTxt.transform.parent.parent.gameObject.SetActive(true);
        gift = (int)saveDataJson.GetData("Gift");
        GiftIconTxt.text = $"{gift}/30";
        ListToFind.SetActive(true);
        // ListCard.SetActive(true);
        bool[] a = (bool[])saveDataJson.GetData("AddMoreItemSpecial");
        bool[] b = (bool[])saveDataJson.GetData("ShowAllItemSpecial");
        addMoreItem = a.Length < mapNum ? false :  a[mapNum - 1];
        showAllItem = b.Length < mapNum ? false :  b[mapNum - 1];

        StartCoroutine(ChangeNameArea(GameSpecialData.AreaName));
        listMap.transform.Find($"Map{mapNum}").gameObject.SetActive(true);

        hint.text = $"{(int)saveDataJson.GetData("Hint")}";
        compassTxt.text = $"{(int)saveDataJson.GetData("Compass")}";
        CheckItemInMap();
        SetValue();
        checkLimited();
        cloudAnimation.PlayChangeScreenAnimation("Special");
    }

    public void AfterChangeScreenAnimation()
    {
        home.SetActive(false);
        gameScreen.SetActive(true);
        CheckItemToOpenArea();

        adsManager.StartCountDownInter();
        compassTxt.transform.parent.gameObject.SetActive(true);
        StartCoroutine(adsManager.CountDownForComboDeluxe("first"));
        // StartCoroutine(adsManager.CountDownForHintSale());
    }

    IEnumerator ChangeNameArea (string txt)
    {
        yield return LocalizationSettings.InitializationOperation;

        areaName.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt);
    }

    void SetDefaultLimit()
    {
        float _height = 2f * cam.orthographicSize;
        float _width = _height * cam.aspect;
        defaultRightLimit = Limited.transform.GetChild(1).position.x - _width / 2;
        defaultBottomLimit = -(Limited.transform.GetChild(2).position.y - _height / 2);
    }

    void checkLimited() 
    {
        int[] Milestones = GameSpecialData.Milestones;

        if(totalCurent == Milestones[0])
        {
            leftLimit = 3f;
            rightLimit = defaultRightLimit;
            bottomLimit = 6f;
            upperLimit = 10.34f;
            leftLimit2 = 0f;
            bottomLimit2 = 0f;
        }
        else if(totalCurent == Milestones[1])
        {
            rightLimit = defaultRightLimit;
            leftLimit = - rightLimit;
            bottomLimit = 6f;
            upperLimit = 10.34f;
            leftLimit2 = 0f;
            bottomLimit2 = 0f;
        }
        else if(totalCurent == Milestones[2]){
            rightLimit = defaultRightLimit;
            leftLimit = - rightLimit;
            bottomLimit = defaultBottomLimit;
            upperLimit = 10.34f;
            leftLimit2 = 3f;
            bottomLimit2 = 6f;
        }
        else
        {
            rightLimit = defaultRightLimit;
            leftLimit = - rightLimit;
            bottomLimit = defaultBottomLimit;
            upperLimit = 10.34f;
            leftLimit2 = 0f;
            bottomLimit2 = 0f;
        }

        if(cam.orthographicSize != 5){
            cam.orthographicSize = 5;
        }
        ChangeLimited(7);
    }

    void CheckItemToOpenArea()
    {   
        if(isGameComplete) {
            if(cloudAnimation.gameObject.activeSelf)
            {
                Invoke("CheckItemToOpenArea", Time.deltaTime);
                return;
            }
            PlayCloundAnimation();
            return;    
        }
        if(totalCurent == totalItem) PlayAnimationCloud(3, "Start");
        for (int i = GameSpecialData.Milestones.Length - 1; i >= 0; i--)
        {
            if (GameSpecialData.Milestones[i] <= totalCurent) PlayAnimationCloud(i, "Start");
        }
    }

    void OpenNewArea(int areaNum = 0) 
    {
        if(areaNum == 4) return;
        addMoreItem = true;
        saveDataJson.SaveData("AddMoreItemSpecial", addMoreItem, MapNum);
        NewArea.SetActive(true);
        NewArea.GetComponent<NewArea>().SetImage(areaNum, MapNum, "specialGame");
    }

    public void PlayAnimationCloud(int areaNum = 0, string txt = null)
    {
        if(txt == "StayCamera") CloudAnimation(areaNum);
        else 
        {
            beTouched = false;
            buttonControl.OffBtn();
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
            if(txt == "Start"){
                mapGame.transform.position = pos;
                CloudAnimation(areaNum);
            } else
                mapGame.transform.DOMove(pos, 0.5f).SetDelay(0.5f).OnComplete(() => CloudAnimation(areaNum));
        };
    }

    void CloudAnimation(int areaNum)
    {
        // beTouched = false;
        // buttonControl.OffBtn();
        Transform clouds = listSpecialCloud.transform.GetChild(areaNum);
        audioManager.PlaySFX("cloud_open");
        for(int i = 0; i < 6; i++)
        {
            SkeletonAnimation cloud = clouds.GetChild(i).GetComponent<SkeletonAnimation>();
            cloud.timeScale = 1;
            clouds.GetChild(i).GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Tan", false);

            if(i == 0) Invoke("AnimationCompleteHandler", 1.167f);
        }
    }

    void AnimationCompleteHandler()
    {
        beTouched = true;
        buttonControl.OnBtn();

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
        GameObject AnimationList = GameObject.Find($"Game/MainGame/MapSpecial/Map{MapNum}/AnimationList");

        foreach (Transform child in AnimationList.transform)
        {
            child.GetComponent<SkeletonAnimation>().AnimationState.ClearTracks();
        }
    }

    void StartAllSkeletonAnimation()
    {
        GameObject AnimationList = GameObject.Find($"Game/MainGame/MapSpecial/Map{MapNum}/AnimationList");

        foreach (Transform child in AnimationList.transform)
        {
            child.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "animation", true);
        }
    }

    void Update()
    {
        if(!beTouched) return;
        if(Input.touchCount > 0) infoItem.SetActive(false);
        HandleTouchItems();
        if(mapAction) HandleZoomAndMove();
    }

    void HandleZoomAndMove()
    {
        if(Input.touchCount == 2){
            _isScreenChange = true;
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            TakeOffset(touch1, "2");
    
            if(EventSystem.current != null &&
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
        else if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.fingerId != fistFingerId) {
                offset = offset2;
                fistFingerId = touch.fingerId;
            };

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if(EventSystem.current != null && 
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
                        // Vector3 direction = touchPos - cam.ScreenToWorldPoint(touch.position);
                        // cam.transform.position += direction;

                        Vector3 currentPos = cam.ScreenToWorldPoint(touch.position);
                        currentPos.z = cam.transform.position.z; // Đảm bảo cùng một lớp z
                        mapGame.transform.position = currentPos - offset;
                        SetLimited(mapGame);
                    }
                    break;
                case TouchPhase.Ended:
                    _isScreenChange = false;
                    break;
            }                
        }
    }

    void TakeOffset(Touch touch, string val = null)
    {
        touchPos = cam.ScreenToWorldPoint(touch.position);
        touchPos.z = cam.transform.position.z; // Đảm bảo cùng một lớp z
        // offset = touchPos - mapGame.transform.position;
        if(val == null) offset = touchPos - mapGame.transform.position;
        else offset2 = touchPos - mapGame.transform.position;
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
        _draggMove = false;
        _checkPos = Input.GetTouch(0).position;
    }

    void Drag()
    {   
        if(_checkPos != Input.GetTouch(0).position) _draggMove = true;
        // _lastDragged.transform.position = new Vector2(_worldPosition.x, _worldPosition.y);
        // Camera.main.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue()
    }

    void Drop()
    {
        dragable item = _lastDragged;
        if(!_isScreenChange && !item.finded)
        {
            if(item.transform.parent.name != "Content")
            {
                currentTotalItem += 1;

                audioManager.PlaySFX("click");
                adsManager.VibrationDevice();
                if(hand.name == item.name) StopHandAnimation();

                item.finded = true;
                string itemName = item.name.Split(" (")[0];
                Transform target = ListItemToFind.transform.Find(itemName);
                SkeletonAnimation itemAnimation = item.GetComponent<SkeletonAnimation>();

                ShowInfoScore(item, target);
                CheckHiddenItem(item);
                PlayAnimItemList(target);
                int currentOrder = item.GetComponent<Renderer>().sortingOrder;
                item.GetComponent<Renderer>().sortingOrder = 110;

                if(itemAnimation != null && itemAnimation.Skeleton.Data.FindAnimation("animation") != null)
                {
                    itemAnimation.AnimationState.SetAnimation(0, "animation", false);
                    StartCoroutine(CompleteAnimationItem(item, currentOrder, 1));
                    item.transform.DOScale(new Vector3(1.5f,1.5f,1.5f), 0.5f).OnComplete(() => {
                        CheckValue(item, target, itemName);
                    });
                }
                else{
                    if(itemAnimation != null && itemAnimation.Skeleton.Data.FindAnimation("Idle") != null)
                    {
                        itemAnimation.AnimationState.ClearTrack(0);
                        itemAnimation.Skeleton.UpdateWorldTransform();
                    }

                    Vector3 currentPosition = item.transform.position - mapGame.transform.position;
                    item.transform.SetParent(canvas.transform);
                    AddParticleToItem(item.gameObject);
                    ShowLightEff(item);

                    item.transform.DOScale(new Vector3(189.996f,189.996f,189.996f), 0.5f).OnComplete(() => {
                        Vector3 targetPosition = target.position;
                        if(!btnDown.activeSelf) targetPosition = btnDown.transform.position;
                        audioManager.PlaySFX("collect_item");
                        item.transform.DOMove(targetPosition , 0.5f).OnComplete(() => {
                            if(item.transform.childCount == 2){
                                LightEff.transform.SetParent(canvas.transform);
                                LightEff.SetActive(false);
                            }

                            StartCoroutine(RemoveParticle(item, currentPosition, currentOrder));
                            CheckValue(item, target, itemName);
                        });
                    });
                }
            }
            else if(!_draggMove) showInfoItem(item);
        }
        _isDragActive = false;
        _draggMove = false;
    }

    private float interpolateAmount;
    IEnumerator EvaluateSlerpPoints(GameObject item, Vector3 start, Vector3 path, Vector3 end, string txt = "") {
        if(txt == "start") interpolateAmount = 0;
        yield return new WaitForSeconds(Time.deltaTime);
        interpolateAmount += Time.deltaTime * 2.5f;

        item.transform.position = QuadraticLerp(start, path, end, interpolateAmount);

        // Debug.Log(item.transform.position + " / " + end);
        if(item.transform.position != end) StartCoroutine(EvaluateSlerpPoints(item, start, path, end));
        else 
        {
            StartCoroutine(RemoveParticle(item.transform));
            // CollectionIC.transform.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.2f).SetEase(Ease.OutQuad).OnComplete(() => {
            //     CollectionIC.transform.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.InQuad);
            // });
            item.gameObject.SetActive(false);
        }
    }

    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, interpolateAmount);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);
        return Vector3.Lerp(ab_bc, bc_cd, interpolateAmount);
    }

    void AddParticleToItem(GameObject item)
    {
        Transform particle = null;
        foreach(Transform child in ListParticle.transform)
        {
            if(!child.gameObject.activeSelf) particle = child;
        }
        if(particle == null)
        {
            particle = Instantiate(MainParticle.transform , Vector3.zero, Quaternion.identity);
        }
        particle.SetParent(item.transform);
        float scaleSize = 1 - (5 - cam.orthographicSize) * 0.2f;

        particle.localScale = new Vector3(scaleSize,scaleSize,1);
        particle.localPosition = Vector3.zero;
        particle.gameObject.SetActive(true);
        particle.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator RemoveParticle (Transform item)
    {
        Transform particle = item.transform.GetChild(0);
        item.GetComponent<Image>().enabled = false;
        particle.SetParent(item.transform.parent);
        yield return new WaitForSeconds(0.3f);

        particle.GetComponent<ParticleSystem>().Stop();
        particle.SetParent(ListParticle.transform);
        particle.gameObject.SetActive(false);
        item.GetComponent<Image>().enabled = true;
    }

    IEnumerator RemoveParticle (dragable item, Vector3 currentPosition, int currentOrder)
    {
        Transform particle = item.transform.GetChild(0);
        if(item.GetComponent<SpriteRenderer>() != null) item.GetComponent<SpriteRenderer>().enabled = false;
        else item.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(0.3f);
        particle.GetComponent<ParticleSystem>().Stop();
        particle.SetParent(ListParticle.transform);
        particle.gameObject.SetActive(false);

        if(item.GetComponent<SpriteRenderer>() != null) item.GetComponent<SpriteRenderer>().enabled = true;
        else item.GetComponent<MeshRenderer>().enabled = true;
        item.gameObject.SetActive(false);
        SkeletonAnimation itemAnimation = item.GetComponent<SkeletonAnimation>();
        if(itemAnimation != null && itemAnimation.Skeleton.Data.FindAnimation("Idle") != null) itemAnimation.AnimationState.SetAnimation(0, "Idle", true);
        item.transform.SetParent(ListItemFinded.transform);
        item.transform.localScale = new Vector3(1,1,1);
        item.transform.position = currentPosition + mapGame.transform.position;
        item.GetComponent<Renderer>().sortingOrder = currentOrder;
    }

    void ShowLightEff(dragable item)
    {
        LightEff.SetActive(true);
        LightEff.transform.SetParent(item.transform);
        LightEff.GetComponent<RectTransform>().localPosition = new Vector3(0,0,9);
        LightEff.GetComponent<RectTransform>().localScale = new Vector3(2,2,1);
        LightEff.transform.DOPause();
        LightEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
    }

    IEnumerator CompleteAnimationItem (dragable item, int currentOrder, int num)
    {
        yield return new WaitForSeconds(0.667f);
        if(num <= 2)
        {
            num++;
            item.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "animation", false);
            StartCoroutine(CompleteAnimationItem(item, currentOrder, num));
        }
        else
        {
            // item.GetComponent<SkeletonAnimation>().AnimationState.ClearTrack(0);
            item.GetComponent<Renderer>().sortingOrder = currentOrder;
            item.gameObject.SetActive(false);
            item.transform.SetParent(ListItemFinded.transform);
            item.transform.localScale = new Vector3(1,1,1);
        }
    }

    void CheckHiddenItem(dragable item)
    {
        if(HiddenItemList.transform.childCount == 0) return;
        Vector3 checkPos = item.transform.position - mapGame.transform.position;
        if(totalCurent > GameSpecialData.Milestones[2] && !addMoreItem){
            int count = TakeNumOfItemsInArea(4);
            if(count <= 5) OpenItemOffScreen(checkPos);
            return;
        }
        else if(!addMoreItem) return;
        int openedArea;

        if(totalCurent == GameSpecialData.Milestones[1]) {
            if(checkPos.x < 0) return;
            openedArea = 2;
        }
        else 
        if(totalCurent == GameSpecialData.Milestones[2]) {
            if(checkPos.x >= 0 || checkPos.y <= 0) return;
            openedArea = 3;
        }
        else if(totalCurent > GameSpecialData.Milestones[2]) {
            if(checkPos.x < 0 || checkPos.y < 0) return;
            openedArea = 4;
        }
        else return;
        TakeListItemHidden(openedArea);

        addMoreItem = false;
        saveDataJson.SaveData("AddMoreItemSpecial", addMoreItem, MapNum);
    }

    void TakeListItemHidden(int openedArea)
    {
        GameObject[] listItemAvailable = {};
        int countChild = HiddenItemList.transform.childCount;
        bool addNewItem = false;

        for(int i = 0; i < countChild; i++)
        {
            Transform child = HiddenItemList.transform.GetChild(i);
            Vector3 childPos = child.transform.position - mapGame.transform.position;
            if(
                (openedArea == 2 && childPos.x < 0 && childPos.y < 0) ||
                (openedArea == 3 && childPos.x > 0 && childPos.y < 0) ||
                (openedArea == 4 && childPos.x < 0 && childPos.y >= 0)
            )
            {
                addNewItem = true;
                Array.Resize(ref listItemAvailable, listItemAvailable.Length + 1);
                listItemAvailable[listItemAvailable.Length - 1] = child.gameObject;
            }
            if(addNewItem){
                addNewItem = false;
                if(listItemAvailable.Length % 2 != 0)
                {
                    child.gameObject.SetActive(true);
                    child.transform.SetParent(ListToFind.transform);
                    saveDataJson.SaveData($"ShowHiddenItemsSpecial{MapNum}", child.name);
                    countChild--;
                    i--;
                }
            }
        }
    }

    int TakeNumOfItemsInArea(int area)
    {
        int count = 0;
        foreach(Transform child in ListToFind.transform)
        {
            Vector3 childPos = child.transform.position - mapGame.transform.position;
            if((area == 1 && childPos.x < 0 && childPos.y <= 0) ||
                (area == 2 && childPos.x >= 0 && childPos.y <= 0) ||
                (area == 3 && childPos.x < 0 && childPos.y > 0) ||
                (area == 4 && childPos.x >= 0 && childPos.y > 0)
            ) count++;
        }
        return count;
    }

    void OpenItemOffScreen(Vector3 checkPos)
    {
        showAllItem = true;
        saveDataJson.SaveData("ShowAllItemSpecial", showAllItem, MapNum);

        int countChild = HiddenItemList.transform.childCount;
        for(int i = 0; i < countChild; i++)
        {   
            Transform child = HiddenItemList.transform.GetChild(i);
            Vector3 childPos = child.position - mapGame.transform.position;

            if ((checkPos.x >= 0 && checkPos.y >= 0 && (childPos.x < 0 || childPos.y < 0)) ||
                ((checkPos.x < 0 || checkPos.y < 0) && childPos.x >= 0 && childPos.y >= 0))
            {
                child.gameObject.SetActive(true);
                child.transform.SetParent(ListToFind.transform);
                saveDataJson.SaveData($"ShowHiddenItemsSpecial{MapNum}", child.name);
                countChild--;
                i--;
            }
        }

        if(HiddenItemList.transform.childCount == 0)
        {
            addMoreItem = false;
            showAllItem = false;
            saveDataJson.SaveData("AddMoreItemSpecial", addMoreItem, MapNum);
            saveDataJson.SaveData("ShowAllItemSpecial", showAllItem, MapNum);
        }
    }

    private void CheckValue(dragable item, Transform target, string itemName) 
    {
        StopBounceTool();
        CheckGift();
        TextMeshProUGUI targetTxt = target.GetChild(0).GetComponent<TextMeshProUGUI>();
        int currentQuantity = Convert.ToInt32(targetTxt.text.Split("/")[0]);
            // Convert.ToInt32(targetTxt.text.Substring(0, targetTxt.text.Length - 3));
        targetTxt.text = 
            $"{currentQuantity + 1}/{GameSpecialData.QuantityList[Array.IndexOf(GameSpecialData.ItemList, itemName)]}";
        // int currentTotal = 
        //     Convert.ToInt32(totalInTrophy.GetComponent<TextMeshProUGUI>().text.Split("/")[0]) + 1;

        if(btnDown.activeSelf){
            target.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.2f).SetEase(Ease.OutQuad).OnComplete(() => {
                if(currentQuantity + 1 == Convert.ToInt32(targetTxt.text.Split("/")[1])) target.transform.GetChild(2).gameObject.SetActive(true);  
                target.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.InQuad).OnComplete(() => {
                    if(currentQuantity + 1 == Convert.ToInt32(targetTxt.text.Split("/")[1])) ChangePositionOfDoneItem(target);
                });
            });
        }else if(currentQuantity + 1 == Convert.ToInt32(targetTxt.text.Split("/")[1])) {
            target.transform.GetChild(2).gameObject.SetActive(true);
            ChangePositionOfDoneItem(target);
        }

        if(currentTotalItem >= totalCurent) ChecktotalCurent(totalCurent);
        // if(currentTotal == totalCurent) StartCoroutine(CheckIfGiftOpen(currentTotal));
        else {
            totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
            totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
            SetProgressBar(currentTotalItem, totalCurent);
        }

        saveDataJson.SaveData($"ItemSpecialMap{MapNum}", item.name);

        if(currentTotalItem == totalItem) PlayCloundAnimation();
    }

    void ChangePositionOfDoneItem (Transform target)
    {
        int i = ListItemToFind.transform.childCount - 1;
        int limited = target.GetSiblingIndex();
        // buttonControl.OffBtn();
        Vector2 targetPos =  ListItemToFind.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
        for (; i > limited; i--)
        {
            Transform child = ListItemToFind.transform.GetChild(i);
            child.GetComponent<RectTransform>().DOAnchorPos(ListItemToFind.transform.GetChild(i - 1).GetComponent<RectTransform>().anchoredPosition, 0.2f);
        }
        target.SetParent(ListItemToFind.transform.parent);
        target.SetParent(ListItemToFind.transform);
        target.GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.2f);
    }

    void CheckGift()
    {
        gift++;
        if(gift == 30)
        {
            gift = 0;
            OpenedGift.SetActive(true);
            OpenedGift.GetComponent<OpenedGift>().PlayAnimation();
        } 
        GiftIconTxt.text = $"{gift}/30";
        saveDataJson.SaveData("Gift", gift);
    }

    IEnumerator CheckIfGiftOpen(int area)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if(OpenedGift.activeSelf) StartCoroutine(CheckIfGiftOpen(area));
        else OpenNewArea(area);
    }

    void ChecktotalCurent(int currentTotal = 0)
    {
        int[] Milestones = GameSpecialData.Milestones;
        int i = 0;
        for(; i < 3; i++)
        {
            if(currentTotal == Milestones[i]) 
            {
                if(i < 2) {
                    totalCurent = Milestones[i + 1];
                    break;    
                }
                else
                {
                    totalCurent = totalItem;
                    break;
                }
            }
        }
        totalInTrophy.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
        totalInStatistical.GetComponent<TextMeshProUGUI>().text = $"{currentTotalItem}/{totalCurent}";
        SetProgressBar(currentTotalItem, totalCurent);
        checkLimited();
        StartCoroutine(CheckIfGiftOpen(i + 1));
        // OpenNewArea(i + 1);
    }

    void PlayAnimItemList(Transform target)
    {
        if(Math.Round(target.transform.position.x, 1) == -0.5) return;
        ListItemToFind.transform.DOPause();

        float curentPos = ListItemToFind.GetComponent<RectTransform>().anchoredPosition.x;
        float targetX = -(45 + 120 * target.GetSiblingIndex()) + 285; 
        float limitRight = -ListItemToFind.GetComponent<RectTransform>().sizeDelta.x + ListItemToFind.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        if(targetX < limitRight) targetX = limitRight;
        else if(targetX > 0) targetX = 0;
        StartCoroutine(MoveItemList(curentPos, targetX, 0.19f));
    }

    IEnumerator MoveItemList (float curentPos, float targetX, float timeDuration, float num = 0)
    {
        float distanceX = targetX - curentPos > 0 ? -(targetX - curentPos) : targetX  - curentPos;
        float xx = distanceX / (timeDuration / Time.deltaTime);
        num += Time.deltaTime;
        yield return new WaitForSeconds(Time.deltaTime);
        // ListItemToFind.transform.position += new Vector3(xx,0,0);
        if(num < timeDuration) 
        {
            if(curentPos < targetX) ListItemToFind.GetComponent<RectTransform>().anchoredPosition -= new Vector2(xx, 0);
            else ListItemToFind.GetComponent<RectTransform>().anchoredPosition += new Vector2(xx, 0);
            StartCoroutine(MoveItemList(curentPos, targetX, timeDuration, num));
        }else ListItemToFind.GetComponent<RectTransform>().anchoredPosition = new Vector2(targetX, ListItemToFind.GetComponent<RectTransform>().anchoredPosition.y);;
    }

    void ShowInfoScore(dragable item, Transform target)
    {
        Image obj = info.GetComponent<Image>();
        Image obj1 = info.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI obj2 = info.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI obj3 = info.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        info.transform.position = item.transform.position + new Vector3(0,1f,0);
        info.SetActive(true);

        string txt = item.name.Split(" (")[0];
        info.transform.GetChild(1).GetComponent<LocalizeStringEvent>()
            .StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt);
        obj1.sprite = controlImage.TakeImage(controlImage.ListItem, txt);
        obj1.SetNativeSize();
        obj1.transform.localScale = new Vector3(0.9f,0.9f,1);

        TextMeshProUGUI targetTxt = target.GetChild(0).GetComponent<TextMeshProUGUI>();
        int currentQuantity = Convert.ToInt32(targetTxt.text.Split("/")[0]);
        // Convert.ToInt32(targetTxt.text.Substring(0, targetTxt.text.Length - 3));
        obj3.text = $"{currentQuantity + 1}/{GameSpecialData.QuantityList[Array.IndexOf(GameSpecialData.ItemList, txt)]}";
        obj.DOPause();
        obj1.DOPause();
        obj2.DOPause();
        obj3.DOPause();

        obj.DOFade(1f, 0.2f);
        obj1.DOFade(1f, 0.2f);
        obj2.DOFade(1f, 0.2f);
        obj3.DOFade(1f, 0.2f).OnComplete(() => {
            obj.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj1.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj2.DOFade(0f, 0.5f).SetDelay(0.7f);
            obj3.DOFade(0f, 0.5f).SetDelay(0.7f).OnComplete(() => info.SetActive(false));
        });

    }

    void showInfoItem(dragable item)
    {
        audioManager.PlaySFX("click");
        infoItem.SetActive(true);
        // infoItem.transform.position = new Vector3(item.transform.position.x + 0.05f, item.transform.position.y + 0.6f, item.transform.position.z);
        infoItem.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(item.GetComponent<RectTransform>().anchoredPosition.x + ListItemToFind.GetComponent<RectTransform>().anchoredPosition.x, 100);
        LocalizeStringEvent txt = infoItem.transform.GetChild(0).GetComponent<LocalizeStringEvent>();
        txt.StringReference.TableEntryReference = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.name);

        // Transform itemName = infoItem.transform.GetChild(0).GetComponent<LocalizeStringEvent>();
        
        //////////////////////////////// Chỉnh kích thước theo độ dài của text
        // Debug.Log(txt.StringReference.TableEntryReference);
        // float textLength = txt.preferredWidth;
        float textLength = infoItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().preferredWidth;
        float newWidth = textLength;
        RectTransform rectTransform = txt.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
        ////////////////////////////////        
    }

    private void Zoom(float increment)
    {
        float currentCamSize = cam.orthographicSize;
        // cam.GetComponent<ViewportHandler>().ChangeUnitsSize(increment, zoomMin, zoomMax);
        cam.orthographicSize = Mathf.Clamp(value:cam.orthographicSize - increment, zoomMin, zoomMax);
        
        ChangeLimited(currentCamSize);
    }

    void ChangeLimited(float currentCamSize, float val = 0)
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

    GameObject[] TakeAvailiableItemsToFind()
    {
        GameObject[] listItemAvailable = {};

        int[] Milestones = GameSpecialData.Milestones;
        int areaMap = 1;
        if(totalCurent == Milestones[1]) areaMap = 2;
        else if(totalCurent == Milestones[2]) areaMap = 3;
        else if(totalCurent > Milestones[2]) areaMap = 4;
        for(int i = 0; i < ListToFind.transform.childCount; i++)
        {   
            Transform child = ListToFind.transform.GetChild(i);
            if(child.GetComponent<dragable>().finded) continue;
            Vector3 childPos = child.transform.position - mapGame.transform.position;

            if(
                (areaMap == 1 && childPos.x <= 0 && childPos.y <= 0) ||
                (areaMap == 2 && childPos.y <= 0) || areaMap == 4 ||
                (areaMap == 3 && (childPos.x <= 0 || childPos.y <= 0))
            )
            {
                Array.Resize(ref listItemAvailable, listItemAvailable.Length + 1);
                listItemAvailable[listItemAvailable.Length - 1] = child.gameObject;
            }
        }

        return listItemAvailable;
    }

    GameObject TakeRandomItem()
    {
        GameObject[] listItemAvailable = TakeAvailiableItemsToFind();

        if(listItemAvailable.Length == 0) return null;
        int ran = new System.Random().Next(0, listItemAvailable.Length);
        return listItemAvailable[ran];
    }

    public void FindRandomItem()
    {
        if(hand.activeSelf) return;
        if(ListToFind && ListToFind.transform.childCount == 0 && !isChallenge) return;
        else if(isChallenge && ChallengeItem.transform.childCount == 0) return;
        // StopHandAnimation();
        beTouched = false;

        // GameObject[] listItemAvailable = TakeAvailiableItemsToFind();

        // if(listItemAvailable.Length == 0) return;
        // int ran = new System.Random().Next(0, listItemAvailable.Length);
        GameObject itemRandom = TakeRandomItem();
        if(itemRandom == null) return;

        Vector3 mapGamePosition = mapGame.transform.position;
        float xx = mapGamePosition.x - itemRandom.transform.position.x;
        float yy = mapGamePosition.y - itemRandom.transform.position.y;
        float currentCam = cam.orthographicSize;
        float timeDuration = 0.4f;

        Vector3 newPosition = new Vector3(xx, yy, mapGamePosition.z);

        if(!isChallenge) {
            ChangeLimited(currentCam, 3);
            newPosition = new Vector3
            (
                x:Mathf.Clamp(value:newPosition.x, min:leftLimit, max:rightLimit),
                y:Mathf.Clamp(value:newPosition.y, min:bottomLimit, max:upperLimit),
                newPosition.z
            );
        }
        else {
            gameObject.GetComponent<ChallengeMode>().ChangeLimited(currentCam, 3);
            newPosition = gameObject.GetComponent<ChallengeMode>().GetNewPositionLimit(newPosition);
        }

        mapGame.transform.DOMove(newPosition, timeDuration).OnComplete(() => 
        {
            beTouched = true;
            PlayHandAnimation(itemRandom);
        });
        AnimationCamera(currentCam, 3, timeDuration);

        int hintNum = (int)saveDataJson.GetData("Hint") - 1;
        hint.text = $"{hintNum}";
        shop.ChangeHintText(hintNum);
        saveDataJson.SaveData("Hint", hintNum);
    }

    void PlayHandAnimation(GameObject itemRandom) 
    {
        hand.transform.position = itemRandom.transform.position - new Vector3(-0.5f,0.5f,0);
        hand.name = itemRandom.name;
        hand.SetActive(true);
        // Vector3 curentPos = hand.transform.position;
        // hand.transform.DOMove(new Vector3(curentPos.x + 0.2f, curentPos.y - 0.2f, curentPos.z), 0.4f).SetLoops(-1, LoopType.Yoyo);
    }

    void StopHandAnimation()
    {
        hand.SetActive(false);
        // hand.transform.DOPause();
    }

    void AnimationCamera(float currentCam, int size, float timeDuration)
    {
        float val = cam.orthographicSize - (currentCam - size) / (timeDuration/Time.deltaTime);
        cam.orthographicSize = val <= 3 ? 3 : val;
        if(val > 3) StartCoroutine(callBack(currentCam, size, timeDuration));
    }
    IEnumerator callBack (float currentCam, int size, float timeDuration)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        AnimationCamera(currentCam, size, timeDuration);
    }

    void RadaFindIteminCamera(string place)
    {
        /// tìm items đang hiện trên camera
        GameObject[] listItemAvailable = {};

        for(int i = 0; i < ListToFind.transform.childCount; i++)
        {   
            Transform child = ListToFind.transform.GetChild(i);
            Vector3 childPos = child.transform.position;

            Vector3 viewportPosition = cam.WorldToViewportPoint(childPos);
            // if (viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1 && viewportPosition.z >= 0)
            if (viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1)
            {
                Array.Resize(ref listItemAvailable, listItemAvailable.Length + 1);
                // child.localScale = new Vector3(2,2,1);
                listItemAvailable[listItemAvailable.Length - 1] = child.gameObject;               
            }
        }
    }

    public void UseCompass()
    {
        CancelInvoke("TimeOutOfUseCompass");
        Invoke("TimeOutOfUseCompass", 20);
        GameObject itemRandom = TakeRandomItem();

        if(itemRandom == null) return;
        Compass.SetActive(true);

        shop.AddMoreStuff(0,0,-1);
        compassTxt.text = $"{(int)saveDataJson.GetData("Compass")}";

        StartCoroutine(RotationCompass(itemRandom));
    }

    IEnumerator RotationCompass (GameObject item)
    {
        if(item != null) UpdateCompassRotation(item);

        yield return new WaitForSeconds(Time.deltaTime);
        if(item == null || !item.activeSelf || item.GetComponent<dragable>().finded)
        {
            item = TakeRandomItem();
        }

        if(Compass.activeSelf) StartCoroutine(RotationCompass(item));
    }

    private void UpdateCompassRotation(GameObject targetItem)
    {
        Vector3 vectorToTarget = targetItem.transform.position - Compass.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        Compass.transform.rotation = Quaternion.Slerp(Compass.transform.rotation, q, Time.deltaTime * 20);
    }

    void TimeOutOfUseCompass()
    {
        Compass.SetActive(false);
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

    void ResetData()
    {
        StopAllSkeletonAnimation();
        listCloud.SetActive(true);
        listSpecialCloud.SetActive(false);
        totalItem = 0;
        totalCurent = 0;
        fistFingerId = 0;
        cam.orthographicSize = 7;
        currentTotalItem = 0;
        hand.SetActive(false);
        info.SetActive(false);
        infoItem.SetActive(false);
        wrongItem.SetActive(false);
        ListToFind.SetActive(false);
        GameSpecialData = null;

        leftLimit = -4.184f;
        rightLimit = 4.184f;
        bottomLimit = -6.34f;
        upperLimit = 10.34f;

        isCountTimeTakeItem = false;
        stopCountTimeTakeItem = true;
        resetCountTimeTakeIte = false;
        pauseCountTimeTakeItem = false;
        // beTouched = true;
        // _draggMove = false;
        // onSpecialTop = false;
        // _isDragActive = false;
        // _isScreenChange = false;
        // addMoreItem = false;
        // showAllItem = false;
        GiftIconTxt.transform.parent.parent.gameObject.SetActive(false);
        mapGame.transform.position = new Vector3(0,0,mapGame.transform.position.z);
        int listItemToFindLength = ListItemToFind.transform.childCount;

        for (int i = 0; i < listItemToFindLength;)
        {
            listItemToFindLength--;
            Transform child = ListItemToFind.transform.GetChild(i);
            child.transform.GetChild(2).gameObject.SetActive(false);
            child.gameObject.name = "ItemPrefab";
            ObjectPoolManager.ReturnObjectToPool(child.gameObject);
            child.transform.SetParent(PoolItem.transform);
        }
        // foreach(Transform child in ListCard.transform) 
        // {
        //     child.gameObject.SetActive(true);
        // }

        ListItemToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 130);
        listMap.transform.Find($"Map{MapNum}").gameObject.SetActive(false);
        gameScreen.SetActive(false);
        ShowFooter();
        beTouched = true;
        buttonControl.OnBtn();

        gameObject.GetComponent<Game>().enabled = true;
        gameObject.GetComponent<SpecialGame>().enabled = false;
    }

    public void PlayCloundAnimation(string info = null) {
        beTouched = false;
        buttonControl.OffBtn();
        ListItemToFind.transform.position = new Vector3(0,ListItemToFind.transform.position.y,ListItemToFind.transform.position.z);
        audioManager.PlaySFX("cloud_close");
        for(int i = 0; i < 4; i++) 
        {
            foreach(Transform child in listSpecialCloud.transform.GetChild(i))
            {
                SkeletonAnimation cloud = child.GetComponent<SkeletonAnimation>();
                cloud.AnimationState.SetAnimation(0, "Idle", true);
                cloud.timeScale = 0.1f;
            }
        }

        if(info != null) {
            cloudAnimation.PlayChangeScreenAnimation("ComeBackHome");
            Invoke("ComeBackHome", 2f);
        }
        else 
        {
            adsManager.StopCountDownInter();
            adsManager.StopCountDownForHintSale();
            Invoke("GameComplete", 2f);
            if(!OpenedGift.activeSelf) cloudAnimation.PlayChangeScreenAnimation("GameComplete");
        }
    }

    void ComeBackHome()
    {
        ResetData();
        adsManager.StopCountDownInter();
        adsManager.StopCountDownForHintSale();
        buttonControl.ReturnToHome();
    }

    void GameComplete()
    {
        if(OpenedGift.activeSelf) {
            Invoke("GameComplete", 0.5f);
            return;
        }

        if(!cloudAnimation.gameObject.activeSelf) {
            cloudAnimation.PlayChangeScreenAnimation("GameComplete");

            Invoke("ShowCompleteDilog",2f);
        }else ShowCompleteDilog();

    }

    void ShowCompleteDilog()
    {
        int ListItemFindedLength = ListItemFinded.transform.childCount;
        string[] showHiddenItems = (string[])saveDataJson.GetData($"ShowHiddenItemsSpecial{MapNum}");

        for (int i = 0; i < ListItemFindedLength;)
        {
            ListItemFindedLength--;
            Transform child = ListItemFinded.transform.GetChild(i);
            child.gameObject.SetActive(true);
            if(showHiddenItems.Contains(child.name)) child.SetParent(HiddenItemList.transform);
            else child.SetParent(ListToFind.transform);
            child.GetComponent<dragable>().finded = false;
        }
        saveDataJson.SaveData($"ShowHiddenItemsSpecial{MapNum}", null);
        saveDataJson.SaveData($"ItemSpecialMap{MapNum}", null);
        ResetData();
        // adsManager.StopCountDownInter();

        gameObject.SetActive(false);
        completeDialog.SetActive(true);
        completeDialog.GetComponent<Complete>().SetImage(MapNum, "specialGame");
    }
}
