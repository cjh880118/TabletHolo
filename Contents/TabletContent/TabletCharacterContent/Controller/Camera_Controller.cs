using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Camera frontCamera;
    public Camera rightCamera;
    public Camera leftCamera;

    Vector3 frontOri;
    Vector3 rightOri;
    Vector3 leftOri;

    Vector3 frontFacial;
    Vector3 rightFacial;
    Vector3 leftFacial;

    //float oriFov;
    //float facialFov;

    // Start is called before the first frame update
    public void InitCamera()
    {
        frontOri = frontCamera.transform.position;
        rightOri = rightCamera.transform.position;
        leftOri = leftCamera.transform.position;
        //oriFov = frontCamera.fieldOfView;
        //높이 0.5상승
        frontFacial = new Vector3(0, 1.7f, 1.5f);
        rightFacial = new Vector3(17f, 1.5f, 0);
        leftFacial = new Vector3(-17f, 1.5f, 0);
        //facialFov = 30;
        AddMessage();
    }

    private void AddMessage()
    {
        Message.AddListener<CameraZoomMsg>(CameraZoom);
    }

    private void CameraZoom(CameraZoomMsg msg)
    {
        if (msg.isZoom)
        {
            frontCamera.transform.position = frontFacial;
            rightCamera.transform.position = rightFacial;
            leftCamera.transform.position = leftFacial;
            frontCamera.orthographicSize = 0.36f;
            rightCamera.orthographicSize = 0.2f;
            leftCamera.orthographicSize = 0.2f;
        }
        else
        {
            frontCamera.transform.position = frontOri;
            rightCamera.transform.position = rightOri;
            leftCamera.transform.position = leftOri;
            frontCamera.orthographicSize = 1.8f;
            rightCamera.orthographicSize = 1;
            leftCamera.orthographicSize = 1;
        }
    }

    //public void SetCameraPoition(bool isHolostar)
    //{
      
    //}

    private void OnDestroy()
    {
        RemoveMessage();
    }

    private void RemoveMessage()
    {
        Message.RemoveListener<CameraZoomMsg>(CameraZoom);
    }
}
