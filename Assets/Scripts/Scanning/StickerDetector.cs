using System;
using System.Linq;
using System.Runtime.InteropServices;
using Manager;
using Scanning;
using UnityEngine;

namespace XZImg
{
    public class StickerDetector : xmgAugmentedVisionBase
    {
        /// <summary>
        /// The sticker number of the detected marker.
        /// </summary>
        public event EventHandler<int> OnStickerDetected;

        /// <summary>
        /// Scan state has been updated.
        /// </summary>
        public event EventHandler<ScanState> OnScanStateUpdated;

        /// <summary>
        /// Verification process in percent.
        /// </summary>
        public event EventHandler<int> OnVerificationProcessUpdated;

        public ScanState CurrentScaneState
        {
            get { return currentState; }
            private set
            {
                currentState = value;
                OnScanStateUpdated?.Invoke(this, currentState);
            }
        }

        [SerializeField]
        private int timesMarkerNeedsUntilVerificationAndroid = 90;
        [SerializeField]
        private int timesMarkerNeedsUntilVerificationIos = 120;

        private ScanState currentState;
        private int lastFoundMarker = -1;
        private int timesMarkerHasBeenFound;
        private int timesMarkerNeedsUntilVerification;

        public override void Start()
        {
            base.Start();
            InitSettings();
            PrepareCamera();
            LoadImages();

            CurrentScaneState = ScanState.Active;

            timesMarkerNeedsUntilVerification = Application.platform == RuntimePlatform.Android
                ? timesMarkerNeedsUntilVerificationAndroid
                : timesMarkerNeedsUntilVerificationIos;
        }

        public override void OnDisable()
        {
            CurrentScaneState = ScanState.Disabled;
            xmgAugmentedVisionBridge.xzimgNaturalImageRelease();

            try
            {
                base.OnDisable();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public override void Update()
        {
            if (!UpdateCamera())
            {
                return;
            }

            base.Update();
            m_capturePlane.ApplyTexture();

            // Stop scanning if an marker has been confirmed
            if (CurrentScaneState == ScanState.Confirmed)
            {
                return;
            }

            // -- Detection
            xmgAugmentedVisionBridge.xzimgNaturalImageTrack(
                ref m_image, m_visionParameters.FilterStrength);

            var amountDetectedMarkers = xmgAugmentedVisionBridge.xzimgNaturalImageGetNumber();
            // Skip if nothing was detected
            if (amountDetectedMarkers < 1)
            {
                if (CurrentScaneState == ScanState.Detected
                    || CurrentScaneState == ScanState.Verifying)
                {
                    CurrentScaneState = ScanState.Lost;
                    lastFoundMarker = -1;
                    timesMarkerHasBeenFound = 0;
                    OnVerificationProcessUpdated?.Invoke(this, 0);
                }

                return;
            }

            // Get info about the detected marker
            // (currently we only care about the first detected marker to keep it simple)
            var markerInfo = new xmgAugmentedVisionBridge.xmgMarkerInfo();
            xmgAugmentedVisionBridge.xzimgNaturalImageGetInfoForUnity(0, ref markerInfo);

            // Check if is an initial detection
            if (CurrentScaneState == ScanState.Active
                || CurrentScaneState == ScanState.Lost)
            {
                CurrentScaneState = ScanState.Detected;
                lastFoundMarker = markerInfo.markerID;

                return;
            }

            if (CurrentScaneState == ScanState.Detected)
            {
                CurrentScaneState = ScanState.Verifying;
            }

            // Verifiyng process
            if (CurrentScaneState == ScanState.Verifying)
            {
                // Check for correct id
                if (markerInfo.markerID != lastFoundMarker)
                {
                    CurrentScaneState = ScanState.Lost;
                    lastFoundMarker = -1;
                    timesMarkerHasBeenFound = 0;

                    return;
                }

                timesMarkerHasBeenFound++;
                Debug.Log("sticker found..." + timesMarkerHasBeenFound.ToString());
                var processPercentage = (int)(((float)timesMarkerHasBeenFound / timesMarkerNeedsUntilVerification) * 100f);
                OnVerificationProcessUpdated?.Invoke(this, processPercentage);

                // Check marker has been verified engouh times
                if (timesMarkerHasBeenFound == timesMarkerNeedsUntilVerification)
                {
                    lastFoundMarker = -1;
                    timesMarkerHasBeenFound = 0;
                    CurrentScaneState = ScanState.Confirmed;

                    var id = m_visionParameters.ObjectPivotLinks[markerInfo.markerID].MarkerIndex;
                    OnStickerDetected?.Invoke(this, id);

                    CurrentScaneState = ScanState.Active;
                }
            }
        }

        private bool InitSettings()
        {
            float fovy_radian = m_videoParameters.CameraVerticalFOV * 3.1415f / 180.0f;
            int success = xmgAugmentedVisionBridge.xzimgNaturalImageInitialize(
                m_capturePlane.m_captureWidth,
                m_capturePlane.m_captureHeight,
                m_videoParameters.GetProcessingWidth(m_capturePlane.m_captureWidth),
                m_videoParameters.GetProcessingHeight(m_capturePlane.m_captureHeight),
                fovy_radian);
            if (success == 1)
            {
                Debug.Log("xzimgNaturalImageInitialize - success");
            }
            else
            {
                Debug.Log("initialization failed with code: " + success.ToString());
            }

            // -- Prepare image
#if (!UNITY_EDITOR && UNITY_IOS)
        int colorMode = 5; // XMG_BGRA
#else
            int colorMode = 4; // XMG_RGBA
#endif
            xmgAugmentedVisionBridge.PrepareImage(
                ref m_image,
                m_capturePlane.m_captureWidth,
                m_capturePlane.m_captureHeight,
                colorMode,
                m_capturePlane.m_PixelsHandle.AddrOfPinnedObject());

            return success == 1;
        }

        // Load Resources from the classifiers into the xmg-SDK
        private void LoadImages()
        {
            m_visionParameters.ObjectPivotLinks = GameManager.Instance
                .TrackableStickers
                .Select(ts => new xmgObjectPivotLink
                {
                    MarkerIndex = ts.StickerNumber,
                    Classifier = ts.ImageClassifier,
                    ObjectRealWidth = ts.PhysicalWidth
                })
                .ToList();

            for (int i = 0; i < m_visionParameters.ObjectPivotLinks.Count; i++)
            {
                if (m_visionParameters.ObjectPivotLinks[i].Classifier)
                {
                    if (m_visionParameters.ObjectPivotLinks[i].ObjectRealWidth <= 0)
                        m_visionParameters.ObjectPivotLinks[i].ObjectRealWidth = 1;

                    TextAsset asset = m_visionParameters.ObjectPivotLinks[i].Classifier as TextAsset;
                    byte[] arrBytes = new byte[asset.bytes.Length];
                    Buffer.BlockCopy(asset.bytes, 0, arrBytes, 0, asset.bytes.Length);
                    GCHandle bytesHandle = GCHandle.Alloc(arrBytes, GCHandleType.Pinned);
                    int success = xmgAugmentedVisionBridge.xzimgNaturalImageAddTarget(
                        bytesHandle.AddrOfPinnedObject(),
                        arrBytes.Length,
                        m_visionParameters.ObjectPivotLinks[i].ObjectRealWidth);

                    if (success == 1)
                        print("xzimgMarkerlessLoadClassifier - Success");
                    else
                        print("failed to load " + asset.name);

                    bytesHandle.Free();

                }
            }
        }
    }
}
