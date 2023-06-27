using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public int toolListLength = 6;

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

    void LateUpdate()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        isWalking = !(Input.GetAxisRaw("Walk") != 0f);
        isCrouching = Input.GetAxisRaw("Crouch") != 0f;
        isJumping = (Input.GetAxisRaw("Jump") != 0f || Input.GetAxis("Mouse ScrollWheel") < 0f);
        isFiring = Input.GetAxis("Fire1") != 0f;
        cameraHorizontal = Input.GetAxis("Mouse X") * sensHorizontal * Time.deltaTime;
        cameraVertical = Input.GetAxis("Mouse Y") * sensVertical * Time.deltaTime;
        scrollInput = Input.GetAxis("Zoom");
        isNext = Input.GetAxisRaw("ToolNext") != 0f;
        isPrev = Input.GetAxisRaw("ToolPrev") != 0f;
    }
}