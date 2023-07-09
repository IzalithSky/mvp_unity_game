using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsMenuUI;
    public InputListener inputListener;
    public bool isMenuActive = false;
    public TMP_InputField sensXtext;
    public TMP_InputField sensYtext;
    public AudioMixer masterMixer;
    public Slider volumeSlider;
    public Slider musicSlider;

    void Start() {
        sensXtext.text = inputListener.sensHorizontal.ToString();
        sensYtext.text = inputListener.sensVertical.ToString();

        volumeSlider.onValueChanged.AddListener(UpdateVolumeMixer);
        musicSlider.onValueChanged.AddListener(UpdateMusicMixer);
    }

    public void UpdateVolumeMixer(float value) {
        masterMixer.SetFloat("masterVolume", value);
    }

    public void UpdateMusicMixer(float value) {
        masterMixer.SetFloat("musicVolume", value);
    }

    void Update()
    {
        if (inputListener.GetIsMenu())
        {
            if (isMenuActive)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        float sensX;
        if (float.TryParse(sensXtext.text, out sensX)) {
            inputListener.sensHorizontal = sensX;
        }

        float sensY;
        if (float.TryParse(sensYtext.text, out sensY)) {
            inputListener.sensVertical = sensY;
        }

        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isMenuActive = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        settingsMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isMenuActive = true;
        Cursor.lockState = CursorLockMode.None;
    }
}