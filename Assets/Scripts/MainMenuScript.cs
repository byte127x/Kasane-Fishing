using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuScript : MonoBehaviour
{
    public Animator AnimateAbout;
    public Button OpenAnimate;
    public Button CloseAnimate;
    public Button OpenSettings;
    public Button CloseSettings;

    public AudioMixer Mixer;
    public AudioSource ButtonSFX;

    // Start is called before the first frame update
    void Start() {
        CloseAnimate.onClick.AddListener(() => {AnimateAbout.SetTrigger("Close");ButtonSFX.Play();});
        OpenAnimate.onClick.AddListener(() => {AnimateAbout.SetTrigger("Open");ButtonSFX.Play();});
        CloseSettings.onClick.AddListener(() => {AnimateAbout.SetTrigger("CloseSettings");ButtonSFX.Play();});
        OpenSettings.onClick.AddListener(() => {AnimateAbout.SetTrigger("OpenSettings");ButtonSFX.Play();});
    }

    public void Fullscreen(Toggle toggle) {
        bool Check = toggle.isOn;
        ButtonSFX.Play();
        if (Check) {Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;}
        else {Screen.fullScreenMode = FullScreenMode.Windowed;}
    }

    public void SetLevel(float val) {
        Mixer.SetFloat("AudioVolume", Mathf.Log10(val) * 20);
    }
}
