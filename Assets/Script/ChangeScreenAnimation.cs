using Spine.Unity;
using UnityEngine;

public class ChangeScreenAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    // public GameObject cloudChangeScreen;
    public Game game;
    public Home home;
    public ChallengeList challengeList;
    public GameObject Complete;
    public CompleteChallenge completeChallenge;
    private string gameStatus;
    public AudioManager audioManager;
    public void PlayChangeScreenAnimation(string status = "")
    {
        audioManager.PlaySFX("cloud_close");
        gameObject.SetActive(true);
        gameStatus = status;

        gameObject.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
        Invoke("CheckGameStatus", 2.1f); // when cloud delay
        Invoke("NextChangeScreenAnimation", 2.5f); // when cloud open
        Invoke("CloseChangeScreenAnimation", 2.833f);

    }

    void CloseChangeScreenAnimation()
    {
        gameObject.SetActive(false);
    }

    void NextChangeScreenAnimation()
    {
        audioManager.PlaySFX("cloud_open");
        // Invoke("CloseChangeScreenAnimation", 0.833f);
    }

    void CheckGameStatus()
    {
        if(Complete.activeSelf && game.gameObject.activeSelf) Complete.SetActive(false);

        if(gameStatus == "Start") game.AfterChangeScreenAnimation();
        else if(gameStatus == "Challenge")
        {
            challengeList.ShowChallenge();
        }
        else if(gameStatus == "ChallengeComplete")
        {
            completeChallenge.ShowChallenge();
        }else if(gameStatus == "Special")
        {
            game.GetComponent<Game>().enabled = false;
            game.GetComponent<SpecialGame>().enabled = true;
            game.GetComponent<SpecialGame>().AfterChangeScreenAnimation();
        }
    }
}
