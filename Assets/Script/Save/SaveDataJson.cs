using System;
using System.IO;
using UnityEngine;

public class SaveDataJson : MonoBehaviour
{
    private PlayerData playerData;
    private string filePath;
    private PlayerData data;


    public TextAsset jsonData;
    public MapList MapData = new MapList();
    [Serializable]public class Map
    {
        public string AreaName;
        public string[] ItemList;
        public int[] QuantityList;
        public int[] Milestones;
    }
    [Serializable]public class MapList
    {
            public Map[] map;
    }


    //// challenge data
    public TextAsset jsonChallengeData;
    public ChallengeList ChallengeData = new ChallengeList();
    [Serializable]public class Challenge
    {
        public string challengeID;
        public int level;
        public int map;
        public int[] area;
        public string item;
        public int quantity;
        public int time;
    }
    [Serializable]public class ChallengeList
    {
            public Challenge[] ChallengeMap;
    }


    public TextAsset jsonGameSpecialData;
    public SpecialList GameSpecialData = new SpecialList();
    [Serializable]public class SpecialData
    {
        public string AreaName;
        public string[] ItemList;
        public int[] QuantityList;
        public int[] Milestones;
    }
    [Serializable]public class SpecialList
    {
            public SpecialData[] SpecialMap;
    }
    
    void Awake()
    {
        MapData = JsonUtility.FromJson<MapList>(jsonData.text);
        ChallengeData = JsonUtility.FromJson<ChallengeList>(jsonChallengeData.text);
        GameSpecialData = JsonUtility.FromJson<SpecialList>(jsonGameSpecialData.text);

        playerData = PlayerData.Instance;
        filePath = Application.dataPath + Path.AltDirectorySeparatorChar + "PlayerData.json";
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        }

        if(!File.Exists(filePath)) SaveNewData();
        else LoadData();
    }
    
    private void SaveNewData()
    {
        SaveData();
        LoadData();
    }

    private void SaveData()
    {
        data = playerData;
        string json = JsonUtility.ToJson(playerData);

        using(StreamWriter writer = new StreamWriter(filePath)) writer.Write(json);
    }

    private PlayerData LoadData()
    {
        string json = string.Empty;

        using(StreamReader reader = new StreamReader(filePath)) json = reader.ReadToEnd();
        if(json == ""){
            //Nếu file json rỗng
            SaveNewData();
            return null;
        }   

        data = JsonUtility.FromJson<PlayerData>(json);

        playerData.SetPlayerData
        (
            data.PlayTutorial, data.RemoveAds, data.Rate,data.Music, data.Sound, data.Vibration, data.OpenAllMaps, data.OpenedMap, data.Gift, data.Hint, data.Compass, data.Gold, 
            data.Challenge1, data.Challenge2, data.Challenge3, data.Language, data.SaleTime, data.LegendarySUB, data.TodayLegendarySUB, data.VIP3Day,
            data.WeeklyVIP, data.EndWeeklyReward, data.SoulFireFound, data.RewardWeeklyTook, data.RewardWeeklyVipTook,
            data.CollecionReward,data.Collecion, data.ListOpenedMap, data.ListChallenge1, data.ListChallenge2, data.ListChallenge3, data.AddMoreItem, data.ShowAllItem, 
            data.ItemMap1, data.ItemMap2, data.ItemMap3, data.ItemMap4, data.ItemMap5, data.ItemMap6, data.ItemMap7, data.ItemMap8, data.ItemMap9,
            data.ShowHiddenItems1, data.ShowHiddenItems2, data.ShowHiddenItems3, data.ShowHiddenItems4, data.ShowHiddenItems5, data.ShowHiddenItems6, data.ShowHiddenItems7, data.ShowHiddenItems8, data.ShowHiddenItems9,
            data.AddMoreItemSpecial, data.ShowAllItemSpecial, data.ListOpenedSpecialMap, data.ItemSpecialMap1, data.ShowHiddenItemsSpecial1
        );

        return data;
    }
    private object TakePLayerData(string name)
    {
        object player = null;
        switch (name)
        {
            case "PlayTutorial": player = data.PlayTutorial; break;
            case "RemoveAds": player = data.RemoveAds; break;
            case "Rate": player = data.Rate; break;
            case "Music": player = data.Music; break;
            case "Sound": player = data.Sound; break;
            case "Vibration": player = data.Vibration; break;
            case "OpenAllMaps": player = data.OpenAllMaps; break;
            case "OpenedMap": player = data.OpenedMap; break;
            case "Gift": player = data.Gift; break;
            case "Hint": player = data.Hint; break;
            case "Compass": player = data.Compass; break;
            case "Gold": player = data.Gold; break;
            case "Challenge1": player = data.Challenge1; break;
            case "Challenge2": player = data.Challenge2; break;
            case "Challenge3": player = data.Challenge3; break;
            case "Language": player = data.Language; break;
            case "SaleTime": player = data.SaleTime; break;
            case "LegendarySUB": player = data.LegendarySUB; break;
            case "TodayLegendarySUB": player = data.TodayLegendarySUB; break;
            case "VIP3Day": player = data.VIP3Day; break;
            case "WeeklyVIP": player = data.WeeklyVIP; break;
            case "EndWeeklyReward": player = data.EndWeeklyReward; break;
            case "SoulFireFound": player = data.SoulFireFound; break;
            case "RewardWeeklyTook": player = data.RewardWeeklyTook; break;
            case "RewardWeeklyVipTook": player = data.RewardWeeklyVipTook; break;
            case "CollecionReward": player = data.CollecionReward; break;
            case "Collection": player = data.Collecion; break;
            case "ListOpenedMap": player = data.ListOpenedMap; break;
            case "ListChallenge1": player = data.ListChallenge1; break;
            case "ListChallenge2": player = data.ListChallenge2; break;
            case "ListChallenge3": player = data.ListChallenge3; break;
            case "AddMoreItem": player = data.AddMoreItem; break;
            case "ShowAllItem": player = data.ShowAllItem; break;
            case "ItemMap1": player = data.ItemMap1; break;
            case "ItemMap2": player = data.ItemMap2; break;
            case "ItemMap3": player = data.ItemMap3; break;
            case "ItemMap4": player = data.ItemMap4; break;
            case "ItemMap5": player = data.ItemMap5; break;
            case "ItemMap6": player = data.ItemMap6; break;
            case "ItemMap7": player = data.ItemMap7; break;
            case "ItemMap8": player = data.ItemMap8; break;
            case "ItemMap9": player = data.ItemMap9; break;
            case "ShowHiddenItems1": player = data.ShowHiddenItems1; break;
            case "ShowHiddenItems2": player = data.ShowHiddenItems2; break;
            case "ShowHiddenItems3": player = data.ShowHiddenItems3; break;
            case "ShowHiddenItems4": player = data.ShowHiddenItems4; break;
            case "ShowHiddenItems5": player = data.ShowHiddenItems5; break;
            case "ShowHiddenItems6": player = data.ShowHiddenItems6; break;
            case "ShowHiddenItems7": player = data.ShowHiddenItems7; break;
            case "ShowHiddenItems8": player = data.ShowHiddenItems8; break;
            case "ShowHiddenItems9": player = data.ShowHiddenItems9; break;
            case "AddMoreItemSpecial": player = data.AddMoreItemSpecial; break;
            case "ShowAllItemSpecial": player = data.ShowAllItemSpecial; break;
            case "ListOpenedSpecialMap": player = data.ListOpenedSpecialMap; break;
            case "ItemSpecialMap1": player = data.ItemSpecialMap1; break;
            case "ShowHiddenItemsSpecial1": player = data.ShowHiddenItemsSpecial1; break;
            default: break;
        }
        return player;
    }


    public MapList TakeMapData()
    {
        // lấy dữ liệu mặc định không bao giờ thay đổi trong thời gian game chạy từ file GameData
        return MapData;
    }

    public SpecialList TakeGameSpecialData()
    {
        // lấy dữ liệu mặc định không bao giờ thay đổi trong thời gian game chạy từ file GameData
        return GameSpecialData;
    }

    public ChallengeList TakeChallengeData()
    {
        // lấy dữ liệu mặc định không bao giờ thay đổi trong thời gian game chạy từ file challengeData
        return ChallengeData;
    }

    public void SaveData(string name, object val, int mapNum = 0)
    {
        // lưu dữ liệu tại thời gian thực

        playerData.SetPlayerData(name, val, mapNum);
        SaveData();
    }

    public object GetData(string name)
    {
        // lấy 1 dữ liệu cự thể đã lưu
        object  player = TakePLayerData(name);
        return player;
    }

    public PlayerData GetData()
    {
        // lấy toàn bộ dữ liệu đã lưu
        return data;
    }
}
