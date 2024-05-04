using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishTypeMenu : MonoBehaviour
{
    public GameObject View;
    public GameObject StoreItem;
    public Sprite AvailableSprite;
    public Sprite GottenSprite;

    public Animator MenuAnimator;
    // Start is called before the first frame update
    void Start()
    {
        GenerateMenu();
    }

    public void MenuIn() {
        GenerateMenu();
        GameObject.Find("TetoMC").GetComponent<TetoScript>().ScreenShowing = true;
        MenuAnimator.Rebind();
        MenuAnimator.SetTrigger("TriggerFish");
    }

    public void MenuOut() {
        GameObject.Find("TetoMC").GetComponent<TetoScript>().ScreenShowing = false;
        MenuAnimator.SetTrigger("TriggerReverse");
    }

    void GenerateMenu() {
        // Clear Screen
        foreach(Transform child in View.transform) {
            Destroy(child.gameObject);
        }

        MainManager.Fish[] CurrentInfo = MainManager.Instance.CommonTypes.Concat(MainManager.Instance.UncommonTypes).ToArray();
        CurrentInfo = CurrentInfo.Concat(MainManager.Instance.RareTypes).ToArray();
        CurrentInfo = CurrentInfo.Concat(MainManager.Instance.VocaloidTypes).ToArray();

        GameObject HorizRow = new GameObject();
        int FourRemainder = 0;

        for (int i = 0; i < CurrentInfo.Length; i++) {
            // Generate Shop Item
            GameObject Item = CreateShopItem(i, CurrentInfo);

            // Add Item to Horizontal Row
            Item.transform.SetParent(HorizRow.transform);

            // Stop at 3 Columns
            FourRemainder = i%4;
            if (FourRemainder == 3) {
                HorizRow = StickRowToScreen(HorizRow);
            }
        }
        // Add Horizontal Layout + Add Row to Store Menu
        StickRowToScreen(HorizRow);
    }

    GameObject StickRowToScreen(GameObject HorizRow) {
        HorizRow.AddComponent<HorizontalLayoutGroup>();
        HorizRow.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, 250f);
        HorizontalLayoutGroup Group = HorizRow.GetComponent<HorizontalLayoutGroup>();
        
        Group.childAlignment = TextAnchor.MiddleCenter;
        Group.spacing = -30f;

        HorizRow.transform.SetParent(View.transform);
        HorizRow.transform.localScale = new Vector3(1, 1, 1);
        return new GameObject();
    }

    GameObject CreateShopItem(int i, MainManager.Fish[] CurrentInfo) {
        MainManager.Fish CurrentFishType = CurrentInfo[i];
        GameObject ShopItem = Instantiate(StoreItem);

        var TitleImage = ShopItem.transform.GetChild(0).GetChild(0).gameObject;
        var RightSide = ShopItem.transform.GetChild(0).GetChild(1).gameObject;

        var TitleText = RightSide.transform.GetChild(0).gameObject;
        var InfoPanel = RightSide.transform.GetChild(1).gameObject;

        var GottenImage = InfoPanel.transform.GetChild(0).gameObject;
        var PriceText = InfoPanel.transform.GetChild(1).gameObject;

        TitleImage.GetComponent<Image>().sprite = CurrentFishType.FishSprite;
        TitleText.GetComponent<TextMeshProUGUI>().text = CurrentFishType.Name;
        if (CurrentFishType.BeenCaught) {GottenImage.GetComponent<Image>().sprite = GottenSprite;}
        else {GottenImage.GetComponent<Image>().sprite = AvailableSprite;}
        PriceText.GetComponent<TextMeshProUGUI>().text = $"{CurrentFishType.Value}";

        return ShopItem;
    }
}
