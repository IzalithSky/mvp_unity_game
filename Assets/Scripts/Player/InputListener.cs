using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public bool toggleCrouch = false;
    public bool toggleWalk = false;

    public float sensHorizontal {
        get => PlayerPrefs.GetFloat(SensHorizontalKey, 800f);
        set => PlayerPrefs.SetFloat(SensHorizontalKey, value);
    }

    public float sensVertical {
        get => PlayerPrefs.GetFloat(SensVerticalKey, 800f);
        set => PlayerPrefs.SetFloat(SensVerticalKey, value);
    }

    private const string SensHorizontalKey = "SensHorizontal";
    private const string SensVerticalKey = "SensVertical";

    float inputHorizontal = 0f;
    float inputVertical = 0f;
    float cameraHorizontal = 0f;
    float cameraVertical = 0f;
    bool isJumping = false;
    bool isWalking = false;
    bool isFiring = false;
    bool isCrouching = false;
    List<bool> isTool; // Array to store the tool states
    float scrollInput = 0f;
    bool isNext;
    bool isPrev;
    bool isPlayNext;
    bool isPlayStop;
    bool isMenu;


    private PlayerControls playerControls;

    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Movement.Walk.performed += ctx => {
            inputHorizontal = ctx.ReadValue<Vector2>().x; 
            inputVertical = ctx.ReadValue<Vector2>().y;};
        playerControls.Movement.Walk.canceled += ctx => {inputHorizontal = 0f; inputVertical = 0f;};

        playerControls.Movement.Look.performed += ctx => {
            cameraHorizontal = ctx.ReadValue<Vector2>().x * sensHorizontal * Time.deltaTime; 
            cameraVertical = ctx.ReadValue<Vector2>().y * sensHorizontal * Time.deltaTime;};
        playerControls.Movement.Look.canceled += ctx => {cameraHorizontal = 0f; cameraVertical = 0f;};

        playerControls.Movement.Jump.started += ctx => isJumping = true;
        playerControls.Movement.Jump.canceled += ctx => isJumping = false;

        playerControls.Movement.Crouch.performed += ctx => isCrouching = !isCrouching;

        playerControls.Enable();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void OnDestroy()
    {
        playerControls.Dispose();
    }

    public bool GetIsMenu() {
        return isMenu;
    }

    public float GetInputHorizontal()
    {
        return inputHorizontal;
    }

    public float GetInputVertical()
    {
        return inputVertical;
    }

    public float GetCameraHorizontal()
    {
        return cameraHorizontal;
    }

    public float GetCameraVertical()
    {
        return cameraVertical;
    }

    public bool GetIsJumping()
    {
        return isJumping;
    }

    public bool GetIsWalking()
    {
        return isWalking;
    }

    public bool GetIsCrouching() {
        return isCrouching;
    }

    public bool GetIsFiring()
    {
        return isFiring;
    }

    public bool GetIsTool(int toolIndex)
    {
        return Input.GetAxis($"Tool {toolIndex}") != 0f;
    }

    public float GetScrollInput() {
        return scrollInput;
    }

    public bool GetIsNext() {
        return isNext;
    }

    public bool GetIsPrev() {
        return isPrev;
    }

    public bool GetIsPlayNext() {
        return isPlayNext;
    }

    public bool GetIsPlayStop() {
        return isPlayStop;
    }
}