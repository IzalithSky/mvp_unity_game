using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    PlayerControls playerControls;

    void Start() {
        sensXtext.text = inputListener.sensHorizontal.ToString();
        sensYtext.text = inputListener.sensVertical.ToString();

        volumeSlider.onValueChanged.AddListener(UpdateVolumeMixer);
        musicSlider.onValueChanged.AddListener(UpdateMusicMixer);
    }

    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Menus.Menu.performed += ctx => ToggleMenu();

        playerControls.Enable();
    }

    void OnDestroy()
    {
        playerControls.Dispose();
    }

    void UpdateVolumeMixer(float value) {
        masterMixer.SetFloat("masterVolume", value);
    }

    void UpdateMusicMixer(float value) {
        masterMixer.SetFloat("musicVolume", value);
    }

    void ToggleMenu() {
        if (isMenuActive) {
            Resume();
        } else {
            Pause();
        }
    }

    void Resume()
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

    void Pause()
    {
        settingsMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isMenuActive = true;
        Cursor.lockState = CursorLockMode.None;
    }
}