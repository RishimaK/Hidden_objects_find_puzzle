using System;
using UnityEngine;

public class ControlImageFromResources : MonoBehaviour
{
    public Sprite[] ListCard;
    public Sprite[] ListItem;
    public Sprite[] ListMapImage;
    public Sprite[] ListNewAreaImage;
    public Sprite[] ListCompletedMapImage;
    public Sprite[] ListChallengeMapImage;
    public Sprite[] ListChallengeTypeImage;

    public Sprite[] ListMapSpecialImage;
    public Sprite[] ListNewAreaSpecialImage;
    public Sprite[] ListCompletedMapSpecialImage;

    public Sprite[] ListMapSpecialCanBuyImage;
    void Awake()
    {
        ListCard = Resources.LoadAll<Sprite>("Card");
        ListItem = Resources.LoadAll<Sprite>("ListItem");
        ListMapImage = Resources.LoadAll<Sprite>("ListMap");
        ListNewAreaImage = Resources.LoadAll<Sprite>("ListNewArea");
        ListChallengeMapImage = Resources.LoadAll<Sprite>("ListChallengeMap");
        ListCompletedMapImage = Resources.LoadAll<Sprite>("ListCompletedMap");
        ListChallengeTypeImage = Resources.LoadAll<Sprite>("ListChallengeType");

        ListMapSpecialImage = Resources.LoadAll<Sprite>("ListMapSpecial");
        ListNewAreaSpecialImage = Resources.LoadAll<Sprite>("ListNewAreaSpecial");
        ListCompletedMapSpecialImage = Resources.LoadAll<Sprite>("ListCompletedMapSpecial");
        ListMapSpecialCanBuyImage = Resources.LoadAll<Sprite>("ListMapSpecialCanBuyImage");
        // GC.Collect();
    }

    void TakeImagesFromResources(Sprite[] list, string path) => list = Resources.LoadAll<Sprite>(path); 

    public Sprite TakeImage(Sprite[] list, string name)
    {
        Sprite itemNeed = null;
        foreach (Sprite item in list){
            if(item.name.ToLower() == name.ToLower()) 
            {
                itemNeed = item;
                break;
            }
        }
        return itemNeed;
    }
}
