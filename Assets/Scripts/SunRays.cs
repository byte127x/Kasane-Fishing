using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRays : MonoBehaviour
{
    // Update is called once per frame
    void Update() {
        transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 0.003f);
    }
}
