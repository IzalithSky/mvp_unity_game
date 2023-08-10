using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public bool toggleCrouch = false;
    public bool toggleWalk = false;

    private const string SensHorizontalKey = "SensHorizontal";
    private const string SensVerticalKey = "SensVertical";
    
    public float sensHorizontal {
        get => PlayerPrefs.GetFloat(SensHorizontalKey, 5f);
        set => PlayerPrefs.SetFloat(SensHorizontalKey, value);
    }

    public float sensVertical {
        get => PlayerPrefs.GetFloat(SensVerticalKey, 5f);
        set => PlayerPrefs.SetFloat(SensVerticalKey, value);
    }

    private const string SensHorizontalControllerKey = "SensHorizontalController";
    private const string SensVerticalControllerKey = "SensVerticalController";

    public float sensHorizontalController {
        get => PlayerPrefs.GetFloat(SensHorizontalControllerKey, 200f);
        set => PlayerPrefs.SetFloat(SensHorizontalControllerKey, value);
    }

    public float sensVerticalController {
        get => PlayerPrefs.GetFloat(SensVerticalControllerKey, 200f);
        set => PlayerPrefs.SetFloat(SensVerticalControllerKey, value);
    }

    float inputHorizontal = 0f;
    float inputVertical = 0f;
    float cameraHorizontal = 0f;
    float cameraVertical = 0f;
    bool isJumping = false;
    bool isWalking = false;
    bool isFiring = false;
    bool isSwitch = false;
    bool isMode0 = false;
    bool isMode1 = false;
    bool isMode2 = false;
    bool isCrouching = false;
    private PlayerControls playerControls;


    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Movement.Walk.performed += ctx => {
            inputHorizontal = ctx.ReadValue<Vector2>().x; 
            inputVertical = ctx.ReadValue<Vector2>().y;};
        playerControls.Movement.Walk.canceled += ctx => {inputHorizontal = 0f; inputVertical = 0f;};

        playerControls.Movement.Look.performed += ctx => {
            cameraHorizontal = ctx.ReadValue<Vector2>().x * sensHorizontal; 
            cameraVertical = ctx.ReadValue<Vector2>().y * sensVertical;};
        playerControls.Movement.Look.canceled += ctx => {cameraHorizontal = 0f; cameraVertical = 0f;};

        playerControls.Movement.LookController.performed += ctx => {
            cameraHorizontal = ctx.ReadValue<Vector2>().x * sensHorizontalController * Time.deltaTime; 
            cameraVertical = ctx.ReadValue<Vector2>().y * sensVerticalController * Time.deltaTime;};
        playerControls.Movement.LookController.canceled += ctx => {cameraHorizontal = 0f; cameraVertical = 0f;};

        playerControls.Movement.Jump.performed += ctx => isJumping = true;
        playerControls.Movement.Jump.canceled += ctx => isJumping = false;

        playerControls.Movement.Crouch.performed += ctx => isCrouching = toggleCrouch ? !isCrouching : true;
        playerControls.Movement.Crouch.canceled += ctx => isCrouching = toggleCrouch ? isCrouching : false;

        playerControls.Movement.Sneak.performed += ctx => isWalking = toggleWalk ? !isWalking : true;
        playerControls.Movement.Sneak.canceled += ctx => isWalking = toggleWalk ? isWalking : false;

        playerControls.Tools.Fire.performed += ctx => isFiring = true;
        playerControls.Tools.Fire.canceled += ctx => isFiring = false;

        playerControls.Tools.Switch.performed += ctx => isSwitch = true;
        playerControls.Tools.Switch.canceled += ctx => isSwitch = false;

        playerControls.Tools.Mode0.performed += ctx => isMode0 = true;
        playerControls.Tools.Mode0.canceled += ctx => isMode0 = false;

        playerControls.Tools.Mode1.performed += ctx => isMode1 = true;
        playerControls.Tools.Mode1.canceled += ctx => isMode1 = false;

        playerControls.Tools.Mode2.performed += ctx => isMode2 = true;
        playerControls.Tools.Mode2.canceled += ctx => isMode2 = false;

        playerControls.Menus.CrouchToggle.performed += ctx => toggleCrouch = !toggleCrouch;
        playerControls.Menus.WalkToggle.performed += ctx => toggleWalk = !toggleWalk;

        playerControls.Enable();
    }

    void OnDestroy()
    {
        playerControls.Dispose();
    }

    public void Resume()
    {
        playerControls.Movement.Enable();
        playerControls.Tools.Enable();
    }

    public void Pause()
    {
        playerControls.Movement.Disable();
        playerControls.Tools.Disable();
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

    public bool GetIsSwitching()
    {
        return isSwitch;
    }

    public bool GetIsMode0()
    {
        return isMode0;
    }

    public bool GetIsMode1()
    {
        return isMode1;
    }

    public bool GetIsMode2()
    {
        return isMode2;
    }
}