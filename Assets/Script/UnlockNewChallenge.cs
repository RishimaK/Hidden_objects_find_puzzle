using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnlockNewChallenge : MonoBehaviour
{
    public AudioManager audioManager;
    // public List<Sprite> ListImage;
    public GameObject ListChallenge;
    public Image image;
    public Complete complete;
    public Home home;
    public Button challengeButton;
    public ControlImageFromResources controlImage;
    public Transform BtnPlay;
    public void SetImage(int map)
    {
        if (map > ListChallenge.transform.childCount) return;
        gameObject.SetActive(true);
        home.UnlockChallenge(map);
        image.sprite = controlImage.TakeImage(controlImage.ListChallengeTypeImage, $"Challenge{map}");
        BtnPlay.localScale = Vector3.one;
        BtnPlay.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void Play()
    {
        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
        complete.HomeBtn();
        home.ChangeMainPage(challengeButton);
        BtnPlay.DOKill();
    }

    public void Later()
    {
        BtnPlay.DOKill();

        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
    }
}
