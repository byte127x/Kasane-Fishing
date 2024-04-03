using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    [Header("Store Panel Transitions")]
    public Animator volumeTransition;
    public Animator storePanelTransition;

    [Header("Scene Transition Stuff")]
    public GameObject Text;
    public float TransitionTime = 1f;
    public string[] VariousMessages = {
        "Loading ...",
        "Loading ...",
        "Loading ...",
        "Loading ...",
        "Loading ...",
        "Loading ...",
        "Loading ...",
        "Sailing the Seven Seas ...",
        "Stopping for a Baguette Break ...",
        "Wishing pollo a Good-Night Rest ...",
        "You're Playing ... Kasane Fishing!",
        "*fades to black cutely*"
    };
    GameObject MainManager;

    [Header("Camera Effects Volume")]
    public GameObject camerafx = null;

    [Header("To Store Menu")]
    public Button storebutton = null;
    [Header("To Gameplay Menu")]
    public Button gamebutton = null;
    [Header("Open Defoko's Shop Button")]
    public Button defokoshopbutton = null;
    [Header("Close Defoko's Shop Button")]
    public Button defokoshopclosebutton = null;
    [Header("Play From MainMenu")]
    public Button playbutton = null;
    [Header("Back To Main Menu")]
    public Button mainmenubutton = null;
    [Header("Leave the Game")]
    public Button quitbutton = null;

    public AudioSource ButtonSFX;

    void Start() {
        ButtonSFX = GameObject.Find("BtnSFX").GetComponent<AudioSource>();
        MainManager = GameObject.Find("MainManager");
        if (MainManager != null) {
            TextMeshProUGUI mText = Text.GetComponent<TextMeshProUGUI>();
            mText.text = MainManager.GetComponent<MainManager>().LoadingMessage;
        }

        if (storebutton != null) {
            storebutton.onClick.AddListener(() => {LoadTitle("Store");ButtonSFX.Play();});
        }

        if (gamebutton != null) {
            gamebutton.onClick.AddListener(() => {LoadTitle("Gameplay");ButtonSFX.Play();});
        }

        if (defokoshopbutton != null) {
            defokoshopbutton.onClick.AddListener(() => {StartCoroutine(OpenDefokoShop());StartCoroutine(AlsoDefokoShop());ButtonSFX.Play();});
        }

        if (defokoshopclosebutton != null) {
            defokoshopclosebutton.onClick.AddListener(() => {StartCoroutine(CloseDefokoShop());StartCoroutine(AlsoCloseDefokoShop());ButtonSFX.Play();});
        }

        if (playbutton != null) {
            playbutton.onClick.AddListener(() => {LoadTitle("Gameplay");ButtonSFX.Play();});
        }

        if (mainmenubutton != null) {
            mainmenubutton.onClick.AddListener(() => {LoadTitle("MainMenu");ButtonSFX.Play();});
        }

        if (quitbutton != null) {
            quitbutton.onClick.AddListener(Application.Quit);
        }
    }

    public void LoadTitle(string LevelID) {
        StartCoroutine(LoadLevel(LevelID));
        StartCoroutine(TransitionAnim());
    }

    public IEnumerator OpenDefokoShop() {
        Debug.Log("Opening Store ...");
        volumeTransition.SetTrigger("BlurAgainAfterReverse");
        yield return new WaitForSeconds(1);
        //camerafx.animation["Unblur"].speed = 1;
    }

    public IEnumerator AlsoDefokoShop() {
        storePanelTransition.SetTrigger("MoveStoreIn");
        yield return new WaitForSeconds(1);
    }

    public IEnumerator CloseDefokoShop() {
        Debug.Log("Opening Store ...");
        volumeTransition.SetTrigger("ReverseBlurComponent");
        yield return new WaitForSeconds(1);
        //camerafx.animation["Unblur"].speed = 1;
    }

    public IEnumerator AlsoCloseDefokoShop() {
        storePanelTransition.SetTrigger("MoveStoreOut");
        yield return new WaitForSeconds(1);
    }

    IEnumerator LoadLevel(string LevelID) {
        int random = Random.Range(0, VariousMessages.Length);
        MainManager.GetComponent<MainManager>().LoadingMessage = VariousMessages[random];

        TextMeshProUGUI mText = Text.GetComponent<TextMeshProUGUI>();
        mText.text = MainManager.GetComponent<MainManager>().LoadingMessage;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionTime);
        SceneManager.LoadScene(LevelID);

        // Load Scene
    }

    IEnumerator TransitionAnim() {
        Debug.Log("insert transition here ...");
        GameObject Ship = GameObject.Find("Ship");
        GameObject TetoCharacter = GameObject.Find("TetoMC");
        TetoCharacter.GetComponent<TetoScript>().SceneTransition = true;
        Rigidbody2D r = Ship.GetComponent<Rigidbody2D>();
        r.simulated = false;
        float Force = 0.1f;

        for (int i = 0; i < 120; i++) {
            Ship.transform.position = Ship.transform.position + new Vector3(Force, 0, 0);
            Force += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
