using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBoat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateBoatSprite();
    }

    public void UpdateBoatSprite()
    {
        if (MainManager.Instance != null) {
            GetComponent<SpriteRenderer>().sprite = MainManager.Instance.Boats[MainManager.Instance.CurrentBoat].BoatImage;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<ParticleSystem>().Play();
        MainManager.Instance.SplashSFX();
    }
}
