/**
 *
 * Copyright (c) 2019 XZIMG Limited , All Rights Reserved
 * No part of this software and related documentation may be used, copied,
 * modified, distributed and transmitted, in any form or by any means,
 * without the prior written permission of XZIMG Limited
 *
 * contact@xzimg.com, www.xzimg.com
 *
 */

using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using System.Threading;

/// Base Class for Augmented Vision Tracker
public class xmgAugmentedVisionBase : MonoBehaviour
{
    public xmgVideoCaptureParametersVision m_videoParameters;
    public xmgVisionParameters m_visionParameters;

    protected WebCamTexture m_webcamTexture = null;
    protected xmgVideoCapturePlaneVision m_capturePlane = null;
    protected Color[] m_imageData;
    protected xmgAugmentedVisionBridge.xmgImage m_image;
    protected xmgAugmentedVisionBridge.xmgVideoCaptureOptions m_xmgVideoParams;

    // -------------------------------------------------------------------------------

    public void CheckParameters()
    {
        m_videoParameters.CheckVideoCaptureParameters();
    }

    public void Awake()
    {
#if UNITY_ANDROID
        // -- Camera permission for Android
        GameObject dialog = null;
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            dialog = new GameObject();
        }
#endif
    }

    // -------------------------------------------------------------------------------

    public virtual void Start()
    {

        CheckParameters();
        if (!m_videoParameters.useNativeCapture)
        {
            // -- Unity webcam capture
            if (m_capturePlane == null)
            {
                m_capturePlane = (xmgVideoCapturePlaneVision)gameObject.AddComponent(typeof(xmgVideoCapturePlaneVision));
                m_webcamTexture = m_capturePlane.OpenVideoCapture(ref m_videoParameters);
                m_capturePlane.CreateVideoCapturePlane(
                    m_videoParameters.VideoPlaneScale,
                    m_videoParameters);
            }
            if (m_webcamTexture == null)
            {
                Debug.Log("Error - No camera detected!");
                return;
            }
        }
        else
        {
            // -- Native camera capture using xzimgCamera
            xmgAugmentedVisionBridge.PrepareNativeVideoCaptureDefault(
                ref m_xmgVideoParams,
                m_videoParameters.videoCaptureMode,
                m_videoParameters.UseFrontal ? 1 : 0);

            m_capturePlane = (xmgVideoCapturePlaneVision)gameObject.AddComponent(typeof(xmgVideoCapturePlaneVision));
            m_capturePlane.CreateVideoCapturePlane(
                m_videoParameters.VideoPlaneScale,
                m_videoParameters);

#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
            xmgAugmentedVisionBridge.xzimgCamera_create(ref m_xmgVideoParams);
#endif
        }
    }

    public virtual void Update()
    {
#if (!UNITY_EDITOR && UNITY_ANDROID)
        // -- double tap to start camera focus event
        if (xmgToolsVision.IsDoubleTap())
            xmgAugmentedVisionBridge.xzimgCamera_focus();
#endif
    }

    /// Get a video frame
    public bool UpdateCamera()
    {
        if (!m_videoParameters.useNativeCapture)
        {
            if (m_capturePlane == null || !m_capturePlane.GetData())
                return false;
        }
        else
        {
#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
            int res = xmgAugmentedVisionBridge.xzimgCamera_getImage(
                m_capturePlane.m_PixelsHandle.AddrOfPinnedObject());
#endif
        }
        return true;
    }

    // -------------------------------------------------------------------------------

    public virtual void OnDisable()
    {
        if (m_capturePlane != null)
        {
            m_capturePlane.ReleaseVideoCapturePlane();
            m_capturePlane = null;
        }
#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
        if (m_videoParameters.useNativeCapture)
            xmgAugmentedVisionBridge.xzimgCamera_delete();
#endif
    }

    // -------------------------------------------------------------------------------

    void OnApplicationPaused(bool pauseStatus)
    {
        // Do something here if you need
    }

    // -------------------------------------------------------------------------------

    void OnApplicationFocus(bool status)
    {
        // track when losing/recovering focus
        bool handle_focus_loss = false;
        if (handle_focus_loss)
        {
#if (UNITY_STANDALONE || UNITY_EDITOR)
            if (m_webcamTexture != null && status == false)
                m_webcamTexture.Stop();     // you can pause as well
            else if (m_webcamTexture != null && status == true)
                m_webcamTexture.Play();
#endif
        }
    }

    public void UpdateDebugDisplay(int iDetected)
    {
        if (iDetected > 0)
        {
            xmgDebug.m_debugMessage = "Marker Detected";
        }
        else if (iDetected == -11)
            xmgDebug.m_debugMessage = "Protection Alert - Wait or restart";
        else
            xmgDebug.m_debugMessage = "Marker not Detected";
    }


    // -------------------------------------------------------------------------------

    public void PrepareCamera()
    {
        float arVideo = m_capturePlane.m_captureWidth / m_capturePlane.m_captureHeight;
        float arScreen = m_videoParameters.GetScreenAspectRatio();
        float fovy_degree = (float)m_videoParameters.CameraVerticalFOV;

        // Compute correct focal length according to video capture crops and different available modes
        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenHorizontally &&
                (xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.LandscapeLeft ||
                xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.LandscapeRight))
        {
            float fovx = (float)xmgToolsVision.ConvertFov(
                m_videoParameters.CameraVerticalFOV,
                m_videoParameters.GetVideoAspectRatio());
            Camera.main.fieldOfView = (float)xmgToolsVision.ConvertFov(
                fovx, 1.0f / m_videoParameters.GetScreenAspectRatio());
        }
        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenVertically &&
                (xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.LandscapeLeft ||
                xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.LandscapeRight))
        {
            //float scaleY = (float)xmgVideoCapturePlane.GetScaleY(m_videoParameters);
            Camera.main.fieldOfView = m_videoParameters.CameraVerticalFOV;
        }

        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenHorizontally &&
                (xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.Portrait ||
                xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.PortraitUpsideDown))
        {
            Camera.main.fieldOfView = (float)xmgToolsVision.ConvertFov(
                m_videoParameters.CameraVerticalFOV,
                m_videoParameters.GetVideoAspectRatio());
        }

        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenVertically &&
                (xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.Portrait ||
                xmgToolsVision.GetRenderOrientation() == xmgOrientationMode.PortraitUpsideDown))
        {
            Camera.main.fieldOfView = (float)xmgToolsVision.ConvertFov(
                fovy_degree,
                arVideo,
                arScreen);
        }

        Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }
}