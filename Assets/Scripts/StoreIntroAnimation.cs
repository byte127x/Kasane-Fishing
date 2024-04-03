using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreIntroAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(100f, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    IEnumerator AnimationFrame() {
        while (frames < 480) {
            transform.position = transform.position + new Vector3(Mathf.Lerp(0.01f, 0f, frames/240), 0, 0);
            frames++;
            yield return new WaitForSeconds(framedelay);
        }
    }*/
}
