using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public float sensHorizontal = 800f;
    public float sensVertical = 800f;
    public int toolListLength = 6;

    float inputHorizontal = 0f;
    float inputVertical = 0f;
    float cameraHorizontal = 0f;
    float cameraVertical = 0f;
    bool isJumping = false;
    bool isWalking = false;
    bool isFiring = false;
    List<bool> isTool; // Array to store the tool states

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

    public bool GetIsFiring()
    {
        return isFiring;
    }

    public bool GetIsTool(int toolIndex)
    {
        return Input.GetAxis($"Tool {toolIndex}") != 0f;
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        isWalking = Input.GetAxisRaw("Walk") != 0f;
        isJumping = Input.GetAxisRaw("Jump") != 0f;
        isFiring = Input.GetAxis("Fire1") != 0f;
        cameraHorizontal = Input.GetAxis("Mouse X") * sensHorizontal * Time.deltaTime;
        cameraVertical = Input.GetAxis("Mouse Y") * sensHorizontal * Time.deltaTime;
    }
}