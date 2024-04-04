using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateShopMenu : MonoBehaviour
{
    public GameObject StoreItem;
    public GameObject MoneyText;

    public GameObject Ship;
    public AudioSource ButtonSFX;
    public AudioSource BuyBtnSFX;
    
    void Start() {
        GenerateMenu();
        ButtonSFX = GameObject.Find("BtnSFX").GetComponent<AudioSource>();
        BuyBtnSFX = GameObject.Find("BuyBtnSFX").GetComponent<AudioSource>();
    }

    void GenerateMenu() {
        // Clear Screen
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }


        var CurrentBoatInfo = MainManager.Instance.Boats;
        var CurrentLureInfo = MainManager.Instance.Lures;
        GameObject HorizRow = new GameObject();
        MoneyText.GetComponent<TextMeshProUGUI>().text = $"Money ${MainManager.Instance.Wallet:n0}";
        int ThreeRemainder = 0;

        for (int i = 0; i < CurrentBoatInfo.Length; i++) {
            // Generate Shop Item
            GameObject Item = CreateShopItem(i, CurrentBoatInfo);

            // Add Item to Horizontal Row
            Item.transform.SetParent(HorizRow.transform);

            // Stop at 3 Columns
            ThreeRemainder = i%3;
            if (ThreeRemainder == 2) {
                HorizRow = StickRowToScreen(HorizRow);
            }
        }
        // Add Horizontal Layout + Add Row to Store Menu
        //StickRowToScreen(HorizRow);

        for (int i = 0; i < CurrentLureInfo.Length; i++) {
            // Generate Shop Item
            GameObject Item = CreateLureShopItem(i, CurrentLureInfo);
            ThreeRemainder++;
            if (ThreeRemainder > 2) {ThreeRemainder = 0;}

            // Add Item to Horizontal Row
            Item.transform.SetParent(HorizRow.transform);

            // Stop at 3 Columns
            if (ThreeRemainder == 2) {
                HorizRow = StickRowToScreen(HorizRow);
            }
        }
        // Add Horizontal Layout + Add Row to Store Menu
        StickRowToScreen(HorizRow);
    }

    GameObject CreateShopItem(int i, MainManager.Boat[] CurrentBoatInfo) {
        MainManager.Boat CurrentBoatType = CurrentBoatInfo[i];
        GameObject ShopItem = Instantiate(StoreItem);

        var TitleImage = ShopItem.transform.GetChild(0).gameObject;
        var TitleText = ShopItem.transform.GetChild(1).gameObject;
        var PurchaseBtn = ShopItem.transform.GetChild(2).gameObject;

        TitleImage.GetComponent<Image>().sprite = CurrentBoatType.BoatImage;
        TitleText.GetComponent<TextMeshProUGUI>().text = $"{CurrentBoatType.BoatName} (${CurrentBoatType.Price})";
        if (CurrentBoatType.IsOwned) {

            // Checks if current boat type is being used
            if (MainManager.Instance.CurrentBoat == i) {
                PurchaseBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Currently Equipped";
                PurchaseBtn.GetComponent<Image>().color = new Color(0.1f, 0.1f, 1, 0.9f);
                PurchaseBtn.GetComponent<Button>().onClick.AddListener(AlreadyEquipped);
            } else {
                PurchaseBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip Boat";
                PurchaseBtn.GetComponent<Image>().color = new Color(0.35f, 0.55f, 0.9f, 0.9f);
                PurchaseBtn.GetComponent<Button>().onClick.AddListener(() => {EquipBoat(i);});
            }
        } else {
            PurchaseBtn.GetComponent<Button>().onClick.AddListener(() => {BuyBoat(i, CurrentBoatType.Price);});
            if (MainManager.Instance.Wallet < CurrentBoatType.Price) {
                PurchaseBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Too Expensive";
                PurchaseBtn.GetComponent<Image>().color = new Color(0.88f, 0.25f, 0.27f, 0.9f);
            }
        }
        return ShopItem;
    }

    GameObject CreateLureShopItem(int i, MainManager.Lure[] CurrentLureInfo) {
        MainManager.Lure CurrentLureType = CurrentLureInfo[i];
        GameObject ShopItem = Instantiate(StoreItem);

        var TitleImage = ShopItem.transform.GetChild(0).gameObject;
        var TitleText = ShopItem.transform.GetChild(1).gameObject;
        var PurchaseBtn = ShopItem.transform.GetChild(2).gameObject;

        TitleImage.GetComponent<Image>().sprite = CurrentLureType.LureImage;
        TitleText.GetComponent<TextMeshProUGUI>().text = $"{CurrentLureType.LureName} (${CurrentLureType.Price})";
        if (CurrentLureType.IsOwned) {

            // Checks if current boat type is being used
            if (MainManager.Instance.CurrentLure == i) {
                PurchaseBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Currently Equipped";
                PurchaseBtn.GetComponent<Image>().color = new Color(0.1f, 0.1f, 1, 0.9f);
                PurchaseBtn.GetComponent<Button>().onClick.AddListener(AlreadyEquipped);
            } else {
                PurchaseBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip Lure";
                PurchaseBtn.GetComponent<Image>().color = new Color(0.35f, 0.55f, 0.9f, 0.9f);
                PurchaseBtn.GetComponent<Button>().onClick.AddListener(() => {EquipLure(i);});
            }
        } else {
            PurchaseBtn.GetComponent<Button>().onClick.AddListener(() => {BuyLure(i, CurrentLureType.Price);});
            if (MainManager.Instance.Wallet < CurrentLureType.Price) {
                PurchaseBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Too Expensive";
                PurchaseBtn.GetComponent<Image>().color = new Color(0.88f, 0.25f, 0.27f, 0.9f);
            }
        }
        return ShopItem;
    }

    GameObject StickRowToScreen(GameObject HorizRow) {
        HorizRow.AddComponent<HorizontalLayoutGroup>();
        HorizRow.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, 600f);

        HorizRow.transform.SetParent(gameObject.transform);
        HorizRow.transform.localScale = new Vector3(1, 1, 1);
        return new GameObject();
    }


    void AlreadyEquipped() {
        Debug.Log("Its Already Equipped!!!!!!");
        ButtonSFX.Play();
    }

    void EquipBoat(int i) {
        MainManager.Instance.CurrentBoat = i;
        
        ChangeBoat BoatChanger = Ship.GetComponent<ChangeBoat>();
        BoatChanger.UpdateBoatSprite();
        GenerateMenu();
        ButtonSFX.Play();
    }

    void BuyBoat(int i, int price) {
        if (MainManager.Instance.Wallet >= price) {
            MainManager.Instance.Wallet -= price;
            MainManager.Instance.Boats[i].IsOwned = true;
            GenerateMenu();
            if (i == 2) {
                MainManager.Instance.ShowAchievement(2);
            }
            else if (i == 4) {
                MainManager.Instance.ShowAchievement(11);
            }
            else if (i == 5) {
                MainManager.Instance.ShowAchievement(12);
            }
            else if (i == 6) {
                MainManager.Instance.ShowAchievement(14);
            }
            BuyBtnSFX.Play();
        }
    }

    void BuyLure(int i, int price) {
        if (MainManager.Instance.Wallet >= price) {
            MainManager.Instance.Wallet -= price;
            MainManager.Instance.Lures[i].IsOwned = true;
            GenerateMenu();
            if (i == 4) {
                MainManager.Instance.ShowAchievement(13);
            }
            BuyBtnSFX.Play();
        }
    }

    void EquipLure(int i) {
        MainManager.Instance.CurrentLure = i;
        GenerateMenu();
        ButtonSFX.Play();
    }
}
