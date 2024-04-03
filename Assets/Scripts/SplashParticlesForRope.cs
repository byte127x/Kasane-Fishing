using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KaimiraGames;

public class RopeEnd : MonoBehaviour
{
    public bool Launching = false;
    public bool ShowingE = false;
    public bool WaitingForFish = false;
    public MainManager.Lure PreviousLure;
    public WeightedList<MainManager.Fish[]> RandomPicker;
    public Animator FishingCanvasAnimator;
    public Button CloseFishButton;

    [Header("Fishing Canvas UI")]
    public Image FishImage;
    public TextMeshProUGUI RarityAndPrice;
    public TextMeshProUGUI FishTitle;
    public GameObject FishIndicator;

    private IEnumerator FishCoroutine;
    private MainManager.Fish MyFish;

    private List<string> AlreadyCaught = new List<string>();
    public int TimesFished = 0;
    public AudioSource NotifSFX;
    public AudioSource ButtonSFX;

    //void OnTriggerEnter2D(Collider2D collision) {}
    public void Start() {
        ButtonSFX = GameObject.Find("BtnSFX").GetComponent<AudioSource>();
        NotifSFX = GameObject.Find("NotifSFX").GetComponent<AudioSource>();
        CloseFishButton.onClick.AddListener(MoveFishScreenOut);
    }

    public void GenerateFish() {
        Launching = true;
        //GetComponent<ParticleSystem>().Play();
        FishCoroutine = Fish(Random.Range(4, 10));
        StartCoroutine(FishCoroutine);
    }

    IEnumerator Fish(int waittime) {
        WaitingForFish = true;
        var CurrentLure = MainManager.Instance.Lures[MainManager.Instance.CurrentLure];
        if (PreviousLure != CurrentLure) {
            RandomPicker = new();
            RandomPicker.Add(MainManager.Instance.CommonTypes, CurrentLure.FishProbability[0]);
            RandomPicker.Add(MainManager.Instance.UncommonTypes, CurrentLure.FishProbability[1]);
            RandomPicker.Add(MainManager.Instance.RareTypes, CurrentLure.FishProbability[2]);
            RandomPicker.Add(MainManager.Instance.VocaloidTypes, CurrentLure.FishProbability[3]);
        }
        PreviousLure = CurrentLure;

        yield return new WaitForSeconds(waittime-1f);
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1f);
        WaitingForFish = false;

        Debug.Log("YOUR FISH IS A ...");

        MainManager.Fish[] ChosenCategory = RandomPicker.Next();

        int idx = Random.Range(0, ChosenCategory.Length);
        if (idx == ChosenCategory.Length) {idx = 0;}
        MyFish = ChosenCategory[idx];

        Debug.Log(MyFish.Name);
        FishImage.sprite = MyFish.FishSprite;
        FishTitle.text = MyFish.Name.ToUpper();

        // Determine Category Name
        string TypeName = "";
        if (ChosenCategory == MainManager.Instance.CommonTypes) {TypeName = "COMMON";}
        if (ChosenCategory == MainManager.Instance.UncommonTypes) {TypeName = "UNCOMMON";}
        if (ChosenCategory == MainManager.Instance.RareTypes) {TypeName = "<material=\"NormalRare\">RARE</font>";}
        if (ChosenCategory == MainManager.Instance.VocaloidTypes) {TypeName = "<material=\"VocaRare\">VOCALOID</font>";}


        ShowingE = true;
        RarityAndPrice.text = $"{TypeName} - ${MyFish.Value}";
        if (CurrentLure.LureBonus != 0) {
            RarityAndPrice.text = $"{RarityAndPrice.text} + ${CurrentLure.LureBonus} BONUS";
        }
        //FishingCanvasAnimator.SetTrigger("MoveInFish");
        //FishingCanvasAnimator.SetTrigger("BlurAgainAfterReverse");
        //Launching = false;
        FishIndicator.GetComponent<Animator>().SetTrigger("FadeIn");
        FishIndicator.transform.position = new Vector3((transform.position.x-0.5f), 0, 0);
        NotifSFX.Play();
        yield return new WaitForSeconds(2.5f);

        FishIndicator.GetComponent<Animator>().SetTrigger("FadeOut");
        ShowingE = false;
    }

    void MoveFishScreenIn() {
        FishingCanvasAnimator.SetTrigger("MoveInFish");
        FishingCanvasAnimator.SetTrigger("BlurAgainAfterReverse");
        Launching = false;
    }

    void MoveFishScreenOut() {
        //FishIndicator.transform.position = new Vector3(0, 90, 0);
        FishingCanvasAnimator.SetTrigger("MoveOutFish");
        FishingCanvasAnimator.SetTrigger("ReverseBlurComponent");
        ButtonSFX.Play();
    }

    void Update() {
        if (!Launching && WaitingForFish) {
            if (FishCoroutine != null) {
                StopCoroutine(FishCoroutine);
                Debug.Log("CANNCELED!!!!! not on twitter!");
                WaitingForFish = false;
            }
        }
        if (!Launching && ShowingE) {
            if (FishCoroutine != null) {
                StopCoroutine(FishCoroutine);
                MoveFishScreenIn();
                
                AlreadyCaught.Add(MyFish.Name);
                
                MainManager.Instance.Wallet = MainManager.Instance.Wallet + MyFish.Value + MainManager.Instance.Lures[MainManager.Instance.CurrentLure].LureBonus;
                ShowingE = false;
                FishIndicator.GetComponent<Animator>().SetTrigger("FadeOut");
                TimesFished++;

                bool ShouldGetVocaAchievement = true;
                for (int i = 0; i < MainManager.Instance.VocaloidTypes.Length; i++) {
                    int AnIndex = AlreadyCaught.IndexOf(MainManager.Instance.VocaloidTypes[i].Name);
                    if (AnIndex < 0) {
                        ShouldGetVocaAchievement = false;
                        break;
                    }
                }

                // Achievements
                if (!MainManager.Instance.Achievements[1].BeenAchieved) {
                    MainManager.Instance.ShowAchievement(1);
                }
                if (MyFish.Name == "Haku Fish") {
                    MainManager.Instance.ShowAchievement(0);
                }
                else if (MyFish.Name == "Tako Luka") {
                    MainManager.Instance.ShowAchievement(3);
                }
                else if (MyFish.Name == "Trash Bag") {
                    MainManager.Instance.ShowAchievement(4);
                }
                else if (MyFish.Name == "Beta Fish") {
                    MainManager.Instance.ShowAchievement(5);
                }
                else if (MyFish.Name == "Dead Fish") {
                    MainManager.Instance.ShowAchievement(6);
                }
                else if (MyFish.Name == "Miku Fish") {
                    MainManager.Instance.ShowAchievement(7);
                }
                else if (MyFish.Name == "Cat Fish") {
                    MainManager.Instance.ShowAchievement(9);
                }
                else if (MyFish.Name == "Polar Bear" || MyFish.Name == "Crab") {
                    MainManager.Instance.ShowAchievement(10);
                }
                // ALL VOCALOIDS ACHIEVEMENT
                if (ShouldGetVocaAchievement) {
                    MainManager.Instance.ShowAchievement(8);
                }
                // FISHING ADDICT ACHIEVEMENT
                if (TimesFished == 20) {
                    MainManager.Instance.ShowAchievement(15);
                }
            }
        }
    }
}
