using System;
// using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class PlayerData
{
    // [JsonProperty]
    public bool PlayTutorial;
    public bool RemoveAds;
    public bool Rate;
    public bool Music;
    public bool Sound;
    public bool Vibration;
    public bool OpenAllMaps;

    public int OpenedMap;
    public int Gift;
    public int Hint;
    public int Compass;
    public int Gold;

    public string Language;
    public string SaleTime;
    public bool LegendarySUB;
    public string TodayLegendarySUB;
    public string VIP3Day;
    public int[]  ListOpenedMap = new int[] {};

    [Header("weekly")]
    public bool WeeklyVIP;
    public string EndWeeklyReward;
    public string[] SoulFireFound = new string[] {};
    // public int RewardOpened;
    // public int RewardWeeklyTook;
    // public int RewardWeeklyVipTook;
    public int[] RewardWeeklyTook = new int[] {};
    public int[] RewardWeeklyVipTook = new int[] {};

    [Header("Collection")]

    public int[] CollecionReward = new int[] {};
    public string[] Collecion = new string[] {};


    [Header("Challenge")]
    public int Challenge1;
    public int Challenge2;
    public int Challenge3;
    public int[]  ListChallenge1 = new int[] {};
    public int[]  ListChallenge2 = new int[] {};
    public int[]  ListChallenge3 = new int[] {};

    [Header("Main game")]

    public bool[] AddMoreItem = {};
    public bool[] ShowAllItem = {};

    public string[] ItemMap1 = new string[] {};
    public string[] ItemMap2 = new string[] {};
    public string[] ItemMap3 = new string[] {};
    public string[] ItemMap4 = new string[] {};
    public string[] ItemMap5 = new string[] {};
    public string[] ItemMap6 = new string[] {};
    public string[] ItemMap7 = new string[] {};
    public string[] ItemMap8 = new string[] {};
    public string[] ItemMap9 = new string[] {};

    public string[] ShowHiddenItems1 = new string[] {};
    public string[] ShowHiddenItems2 = new string[] {};
    public string[] ShowHiddenItems3 = new string[] {};
    public string[] ShowHiddenItems4 = new string[] {};
    public string[] ShowHiddenItems5 = new string[] {};
    public string[] ShowHiddenItems6 = new string[] {};
    public string[] ShowHiddenItems7 = new string[] {};
    public string[] ShowHiddenItems8 = new string[] {};
    public string[] ShowHiddenItems9 = new string[] {};

    [Header("Special Game")]
    public bool[] AddMoreItemSpecial = {};
    public bool[] ShowAllItemSpecial = {};
    public int[]  ListOpenedSpecialMap = {};
    public string[] ItemSpecialMap1 = new string[] {};
    public string[] ShowHiddenItemsSpecial1 = new string[] {};

    public PlayerData () => SetPlayerData();

    public void SetPlayerData
    (
        bool playTutorial = true, bool removeAds = false, bool rate = false, bool music = true, bool sound = true, bool vibration = true, bool openAllMaps = false, int openedMap = 1, int gift = 0,
        int hint = 0, int compass = 0, int gold = 0, int challenge1 = 1, int challenge2 = 1, int challenge3 = 1, string language = null,  string saleTime = null, bool legendarySub = false, string todayLegendarySub = null, 
        string vip3Day = null, bool weeklyVIP = false, string endWeeklyReward = null, string[] soulFireFound = null, int[] rewardWeeklyTook = null, int[] rewardWeeklyVipTook = null, int[] collecionReward = null, string[] collection = null,
        int[] listOpenedMap = null, int[] listChallenge1 = null, int[] listChallenge2 = null, int[] listChallenge3 = null, bool[] addMoreItem = null, bool[] showAllItem = null,
        string[] itemMap1 = null, string[] itemMap2 = null, string[] itemMap3 = null, string[] itemMap4 = null, string[] itemMap5 = null, string[] itemMap6 = null, string[] itemMap7 = null, string[] itemMap8 = null, string[] itemMap9 = null,
        string[] showHiddenItems1 = null, string[] showHiddenItems2 = null, string[] showHiddenItems3 = null, string[] showHiddenItems4 = null, string[] showHiddenItems5 = null,
        string[] showHiddenItems6 = null, string[] showHiddenItems7 = null, string[] showHiddenItems8 = null, string[] showHiddenItems9 = null,
        bool[] addMoreItemSpecial = null, bool[] showAllItemSpecial = null, int[] listOpenedSpecialMap = null, string[] itemSpecialMap1 = null, string[] showHiddenItemsSpecial1 = null
    )
    {
        this.PlayTutorial = playTutorial;
        this.RemoveAds = removeAds;
        this.Rate = rate;
        this.Music = music;
        this.Sound = sound;
        this.Vibration = vibration;
        this.OpenAllMaps = openAllMaps;
        this.AddMoreItem = addMoreItem;
        this.ShowAllItem = showAllItem;

        this.OpenedMap = openedMap;
        this.Gift = gift;
        this.Hint = hint;
        this.Compass = compass;
        this.Gold = gold;
        this.Challenge1 = challenge1;
        this.Challenge2 = challenge2;
        this.Challenge3 = challenge3;

        this.Language = language;
        this.SaleTime = saleTime;
        this.LegendarySUB = legendarySub;
        this.TodayLegendarySUB = todayLegendarySub;
        this.VIP3Day = vip3Day;

        this.WeeklyVIP = weeklyVIP;
        this.EndWeeklyReward = endWeeklyReward;
        this.SoulFireFound = soulFireFound;
        this.RewardWeeklyTook = rewardWeeklyTook;
        this.RewardWeeklyVipTook = rewardWeeklyVipTook;

        this.CollecionReward = collecionReward;
        this.Collecion = collection;

        this.ListOpenedMap = listOpenedMap;
        this.ListChallenge1 = listChallenge1;
        this.ListChallenge2 = listChallenge2;
        this.ListChallenge3 = listChallenge3;

        this.ItemMap1 = itemMap1;
        this.ItemMap2 = itemMap2;
        this.ItemMap3 = itemMap3;
        this.ItemMap4 = itemMap4;
        this.ItemMap5 = itemMap5;
        this.ItemMap6 = itemMap6;
        this.ItemMap7 = itemMap7;
        this.ItemMap8 = itemMap8;
        this.ItemMap9 = itemMap9;

        this.ShowHiddenItems1 = showHiddenItems1;
        this.ShowHiddenItems2 = showHiddenItems2;
        this.ShowHiddenItems3 = showHiddenItems3;
        this.ShowHiddenItems4 = showHiddenItems4;
        this.ShowHiddenItems5 = showHiddenItems5;
        this.ShowHiddenItems6 = showHiddenItems6;
        this.ShowHiddenItems7 = showHiddenItems7;
        this.ShowHiddenItems8 = showHiddenItems8;
        this.ShowHiddenItems9 = showHiddenItems9;

        this.AddMoreItemSpecial = addMoreItemSpecial;
        this.ShowAllItemSpecial = showAllItemSpecial;
        this.ListOpenedSpecialMap = listOpenedSpecialMap;
        this.ItemSpecialMap1 = itemSpecialMap1;
        this.ShowHiddenItemsSpecial1 = showHiddenItemsSpecial1;
    }

    public void SetPlayerData(string name, object val, int mapNum)
    {
        switch (name)
        {
            case "PlayTutorial": this.PlayTutorial = (bool)val; break;
            case "RemoveAds": this.RemoveAds = (bool)val; break;
            case "Rate": this.Rate = (bool)val; break;
            case "Music": this.Music = (bool)val; break;
            case "Sound": this.Sound = (bool)val; break;
            case "Vibration": this.Vibration = (bool)val; break;
            case "OpenAllMaps": this.OpenAllMaps = (bool)val; break;
            case "AddMoreItem": AddItemToList(AddMoreItem, val, name, mapNum); break;
            case "ShowAllItem": AddItemToList(ShowAllItem, val, name, mapNum); break;
            case "OpenedMap": this.OpenedMap = (int)val; break;
            case "Gift": this.Gift = (int)val; break;
            case "Hint": this.Hint = (int)val; break;
            case "Compass": this.Compass = (int)val; break;
            case "Gold": this.Gold = (int)val; break;
            case "Challenge1": this.Challenge1 = (int)val; break;
            case "Challenge2": this.Challenge2 = (int)val; break;
            case "Challenge3": this.Challenge3 = (int)val; break;
            case "Language": this.Language = (string)val; break;
            case "SaleTime": this.SaleTime = (string)val; break;
            case "LegendarySUB": this.LegendarySUB = (bool)val; break;
            case "TodayLegendarySUB": this.TodayLegendarySUB = (string)val; break;
            case "VIP3Day": this.VIP3Day = (string)val; break;
            case "WeeklyVIP": this.WeeklyVIP = (bool)val; break;
            case "EndWeeklyReward": this.EndWeeklyReward = (string)val; break;
            case "SoulFireFound": AddItemToList(SoulFireFound, val, name); break;
            case "RewardWeeklyTook": AddItemToList(RewardWeeklyTook, val, name); break;
            case "RewardWeeklyVipTook": AddItemToList(RewardWeeklyVipTook, val, name); break;
            case "CollecionReward": AddItemToList(CollecionReward, val, name); break;
            case "Collection": AddItemToList(Collecion, val, name); break;
            case "ListOpenedMap": AddItemToList(ListOpenedMap, val, name); break;
            case "ListChallenge1": AddItemToList(ListChallenge1, val, name); break;
            case "ListChallenge2": AddItemToList(ListChallenge2, val, name); break;
            case "ListChallenge3": AddItemToList(ListChallenge3, val, name); break;
            case "ItemMap1": AddItemToList(ItemMap1, val, name); break;
            case "ItemMap2": AddItemToList(ItemMap2, val, name); break;
            case "ItemMap3": AddItemToList(ItemMap3, val, name); break;
            case "ItemMap4": AddItemToList(ItemMap4, val, name); break;
            case "ItemMap5": AddItemToList(ItemMap5, val, name); break;
            case "ItemMap6": AddItemToList(ItemMap6, val, name); break;
            case "ItemMap7": AddItemToList(ItemMap7, val, name); break;
            case "ItemMap8": AddItemToList(ItemMap8, val, name); break;
            case "ItemMap9": AddItemToList(ItemMap9, val, name); break;
            case "ShowHiddenItems1": AddItemToList(ShowHiddenItems1, val, name); break;
            case "ShowHiddenItems2": AddItemToList(ShowHiddenItems2, val, name); break;
            case "ShowHiddenItems3": AddItemToList(ShowHiddenItems3, val, name); break;
            case "ShowHiddenItems4": AddItemToList(ShowHiddenItems4, val, name); break;
            case "ShowHiddenItems5": AddItemToList(ShowHiddenItems5, val, name); break;
            case "ShowHiddenItems6": AddItemToList(ShowHiddenItems6, val, name); break;
            case "ShowHiddenItems7": AddItemToList(ShowHiddenItems7, val, name); break;
            case "ShowHiddenItems8": AddItemToList(ShowHiddenItems8, val, name); break;
            case "ShowHiddenItems9": AddItemToList(ShowHiddenItems9, val, name); break;
            case "AddMoreItemSpecial": AddItemToList(AddMoreItemSpecial, val, name, mapNum); break;
            case "ShowAllItemSpecial": AddItemToList(ShowAllItemSpecial, val, name, mapNum); break;
            case "ListOpenedSpecialMap": AddItemToList(ListOpenedSpecialMap, val, name); break;
            case "ItemSpecialMap1": AddItemToList(ItemSpecialMap1, val, name); break;
            case "ShowHiddenItemsSpecial1": AddItemToList(ShowHiddenItemsSpecial1, val, name); break;
            default: break;
        }
    }

    public void AddItemToList(int[]itemMap, object val, string name)
    {
        if(itemMap == null) itemMap = new int[] {};
        if(val != null){
            bool contains = itemMap.Contains((int)val);
            if(!contains)
            {
                int length = itemMap.Length;
                Array.Resize(ref itemMap, length + 1);
                itemMap[length] = (int)val;
            }
        } else itemMap = new int[] {};

        if(name == "CollecionReward") this.CollecionReward = itemMap;
        else if(name == "ListOpenedMap") this.ListOpenedMap = itemMap;
        else if(name == "RewardWeeklyTook") this.RewardWeeklyTook = itemMap;
        else if(name == "RewardWeeklyVipTook") this.RewardWeeklyVipTook = itemMap;
        else if(name == "ListOpenedSpecialMap") this.ListOpenedSpecialMap = itemMap;
        else if(name == "ListChallenge1") this.ListChallenge1 = itemMap;
        else if(name == "ListChallenge2") this.ListChallenge2 = itemMap;
        else if(name == "ListChallenge3") this.ListChallenge3 = itemMap;

    }

    public void AddItemToList(string[]itemMap, object val, string name)
    {
        if(itemMap == null) itemMap = new string[] {};
        if(val != null){
            if(!itemMap.Contains((string)val))
            {
                int length = itemMap.Length;
                Array.Resize(ref itemMap, length + 1);
                itemMap[length] = (string)val;
            }
        } else itemMap = new string[] {};

        if(name == "Collection") this.Collecion = itemMap;
        else if(name == "SoulFireFound") this.SoulFireFound = itemMap;
        else if(name == "ItemMap1") this.ItemMap1 = itemMap;
        else if(name == "ItemMap2") this.ItemMap2 = itemMap;
        else if(name == "ItemMap3") this.ItemMap3 = itemMap;
        else if(name == "ItemMap4") this.ItemMap4 = itemMap;
        else if(name == "ItemMap5") this.ItemMap5 = itemMap;
        else if(name == "ItemMap6") this.ItemMap6 = itemMap;
        else if(name == "ItemMap7") this.ItemMap7 = itemMap;
        else if(name == "ItemMap8") this.ItemMap8 = itemMap;
        else if(name == "ItemMap9") this.ItemMap9 = itemMap;
        else if(name == "ShowHiddenItems1") this.ShowHiddenItems1 = itemMap;
        else if(name == "ShowHiddenItems2") this.ShowHiddenItems2 = itemMap;
        else if(name == "ShowHiddenItems3") this.ShowHiddenItems3 = itemMap;
        else if(name == "ShowHiddenItems4") this.ShowHiddenItems4 = itemMap;
        else if(name == "ShowHiddenItems5") this.ShowHiddenItems5 = itemMap;
        else if(name == "ShowHiddenItems6") this.ShowHiddenItems6 = itemMap;
        else if(name == "ShowHiddenItems7") this.ShowHiddenItems7 = itemMap;
        else if(name == "ShowHiddenItems8") this.ShowHiddenItems8 = itemMap;
        else if(name == "ShowHiddenItems9") this.ShowHiddenItems9 = itemMap;
        else if(name == "ItemSpecialMap1") this.ItemSpecialMap1 = itemMap;
        else if(name == "ShowHiddenItemsSpecial1") this.ShowHiddenItemsSpecial1 = itemMap;
    }

    public void AddItemToList(bool[]itemMap, object val, string name, int mapNum)
    {
        if(itemMap == null) itemMap = new bool[] {};
        if(val != null){
            int length = itemMap.Length;
            if(length < mapNum){
                Array.Resize(ref itemMap, mapNum);
                itemMap[mapNum - 1] = (bool)val;
            } else itemMap[mapNum - 1] = (bool)val;
        } else itemMap = new bool[] {};

        if(name == "AddMoreItem") this.AddMoreItem = itemMap;
        else if(name == "ShowAllItem") this.ShowAllItem = itemMap;
        else if(name == "AddMoreItemSpecial") this.AddMoreItemSpecial = itemMap;
        else if(name == "ShowAllItemSpecial") this.ShowAllItemSpecial = itemMap;
    }

    private static PlayerData _instance = null;
    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerData();
            }
            return _instance;
        }
    }
}
