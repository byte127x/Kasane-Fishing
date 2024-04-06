using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainManager : MonoBehaviour {
    public static MainManager Instance;
    public int CurrentBoat = 0;
    public int CurrentLure = 0;
    public int Wallet = 1000;
    public string LoadingMessage;
    public AudioClip[] Songs;
    public AudioSource source;
    public AudioClip[] Splashes;
    public AudioSource WaveSplashSound;

    [System.Serializable]
    public struct Boat {
        public string BoatName;
        public int Price;
        public bool IsOwned;
        public Sprite BoatImage;

        public Boat(string name, int price, bool owned, Sprite img) {
            this.BoatName = name;
            this.Price = price;
            this.IsOwned = owned;
            this.BoatImage = img;
        }
    }

    [System.Serializable]
    public struct Lure {
        // Probability Indexes: 0 = Common, 1 = Uncommon, 2 = Rare, 
        public string LureName;
        public int Price;
        public bool IsOwned;
        public int[] FishProbability;
        public int LureBonus;
        public Sprite LureImage;

        public Lure(string name, int price, bool owned, int[] prob, int bonus, Sprite lureimg) {
            this.LureName = name;
            this.Price = price;
            this.IsOwned = owned;
            this.FishProbability = prob;
            this.LureBonus = bonus;
            this.LureImage = lureimg;
        }

        public static bool operator != (Lure f1, Lure f2) {
            return !f1.Equals(f2);
        }

        public static bool operator == (Lure f1, Lure f2) {
            return f1.Equals(f2);
        }
    }

    [System.Serializable]
    public struct Fish {
        public string Name;
        public Sprite FishSprite;
        public int Value;
        public bool BeenCaught;

        public Fish(string name, Sprite sprite, int val, bool caught) {
            this.Name = name;
            this.FishSprite = sprite;
            this.Value = val;
            this.BeenCaught = caught;
        }
    }

    [System.Serializable]
    public struct Achievement {
        public string Name;
        public string Desc;
        public Sprite Image;
        public bool BeenAchieved;

        public Achievement(string name, string desc, Sprite img, bool ach) {
            this.Name = name;
            this.Desc = desc;
            this.Image = img;
            this.BeenAchieved = ach;
        }
    }

    public string[] ProbabilityNames = new string[4] {
        "Common",
        "Uncommon",
        "Rare",
        "VOCALOID (Insane)"
    };

    [Header("Fish Types")]
    public Fish[] CommonTypes = new Fish[6];
    public Fish[] UncommonTypes = new Fish[6];
    public Fish[] RareTypes = new Fish[6];
    public Fish[] VocaloidTypes = new Fish[6];
    public Fish TetoFish;
    
    [Header("Boat Types")]
    public Boat[] Boats = new Boat[3];

    [Header("Lure Types (Probability out of 100)")]
    public Lure[] Lures = new Lure[3];

    [Header("Achievements")]
    public Achievement[] Achievements = new Achievement[3];

    [Header("AchievementGUI")]
    public Image AchieveImg;
    public TextMeshProUGUI AchieveTitle;
    public TextMeshProUGUI AchieveDesc;
    public Animator AchieveAnim;

    public int NumberOfAchievementsAchieved = 0;
    private IEnumerator CurrentCo;
    private List<int> AchievementQueue = new List<int>();

    public List<AudioClip> Playlist;
    public int PlaylistIdx;

    private AudioClip RandomSong() {
        if (PlaylistIdx == Playlist.Count) {PlaylistIdx = 0;}
        AudioClip Music = Playlist[PlaylistIdx];
        PlaylistIdx++;
        return Music;
    }

    void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        while (Playlist.Count < Songs.Length) {
            AudioClip Music = Songs[Random.Range(0, Songs.Length)];
            while (Playlist.IndexOf(Music) != -1) {
                Music = Songs[Random.Range(0, Songs.Length)];
            }
            Playlist.Add(Music);
        }
    }

    void Update() {
        // Playlist Script
        if (!source.isPlaying) {
            source.clip = RandomSong();
            source.Play();
        }
        else if (source.clip == Songs[4]) {
            ShowAchievement(16);
        }

        // achievement stuff
        if (AchievementQueue.Count > 0) {
            if (CurrentCo == null) {
                CurrentCo = WaitToAchieve();
                StartCoroutine(CurrentCo);
            }
        }
    }

    public void SplashSFX() {
        WaveSplashSound.clip = Splashes[Random.Range(0, Splashes.Length)];
        WaveSplashSound.Play();
    }

    public void ShowAchievement(int i) {
        AchievementQueue.Add(i);
    }

    public IEnumerator ShowAchievementAsync() {
        AchieveAnim.SetTrigger("AchievementIn");
        yield return new WaitForSeconds(3);
        AchieveAnim.SetTrigger("AchievementOut");
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator PrepareAchievement(int i) {
        if (Achievements[i].BeenAchieved == true) {Debug.Log("ALREADY GOTIT!!!!!!");yield return null;}
        if (Achievements[i].BeenAchieved == false) {
            Achievements[i].BeenAchieved = true;
            AchieveImg.sprite = Achievements[i].Image;
            AchieveTitle.text = Achievements[i].Name;
            AchieveDesc.text = Achievements[i].Desc;
            NumberOfAchievementsAchieved++;
            yield return StartCoroutine(ShowAchievementAsync());
        }
    }

    public IEnumerator WaitToAchieve() {
        while (AchievementQueue.Count > 0) {
            int CurrentAch = AchievementQueue[0];
            yield return StartCoroutine(PrepareAchievement(CurrentAch));
            AchievementQueue.Remove(CurrentAch);
            Debug.Log($"NUMBER OF ACHI: {NumberOfAchievementsAchieved}");
            if (NumberOfAchievementsAchieved == Achievements.Length-1) {
                AchievementQueue.Add(Achievements.Length-1);
            }
        }
        CurrentCo = null;
        yield return null;
    }
}
