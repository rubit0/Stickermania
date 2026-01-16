using UnityEngine;
using System.Collections;
using Lean.Touch;
using System.Collections.Generic;

public class GyroCamera : MonoBehaviour
{
    [Header("Gyro Control")]
    [SerializeField]
    private float smoothing = 0.9f;

    [Header("Mouse Control")]
    [SerializeField]
    public float sensitivityX = 5F;
    [SerializeField]
    public float sensitivityY = 5F;
    [SerializeField]
    public float minimumY = -60F;
    [SerializeField]
    public float maximumY = 60F;

    private float initialYAngle = 0f;
    private float appliedGyroYAngle = 0f;
    private float calibrationYAngle = 0f;
    private Transform rawGyroRotation;
    private float tempSmoothing;
    private bool hasGyroSupport;
    private float rotationY = 0F;

    private void Start()
    {
        hasGyroSupport = SystemInfo.supportsGyroscope;
        initialYAngle = transform.eulerAngles.y;

        rawGyroRotation = new GameObject("GyroRaw").transform;
        rawGyroRotation.position = transform.position;
        rawGyroRotation.rotation = transform.rotation;

        if (hasGyroSupport)
        {
            Input.gyro.enabled = true;
            StartCoroutine(CalibrateYAngle());
        }
        else
        {
            LeanTouch.OnGesture += HandleOnFingerGesture;
        }
    }

    private void HandleOnFingerGesture(List<LeanFinger> finger)
    {
        var delta = finger[0].ScaledDelta;

        var rotationX = transform.localEulerAngles.y + delta.x * sensitivityX;
        rotationY += delta.y * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    private void LateUpdate()
    {
        if (hasGyroSupport)
        {
            ApplyGyroRotation();
            ApplyCalibration();
            transform.rotation = Quaternion.Slerp(transform.rotation, rawGyroRotation.rotation, smoothing);
        }
    }

    private IEnumerator CalibrateYAngle()
    {
        yield return new WaitForSeconds(1);

        tempSmoothing = smoothing;
        smoothing = 1;
        calibrationYAngle = appliedGyroYAngle - initialYAngle; // Offsets the y angle in case it wasn't 0 at edit time.
        yield return null;
        smoothing = tempSmoothing;
    }

    private void ApplyGyroRotation()
    {
        rawGyroRotation.rotation = Input.gyro.attitude;
        rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self); // Swap "handedness" of quaternion from gyro.
        rawGyroRotation.Rotate(90f, 180f, 0f, Space.World); // Rotate to make sense as a camera pointing out the back of your device.
        appliedGyroYAngle = rawGyroRotation.eulerAngles.y; // Save the angle around y axis for use in calibration.
    }

    private void ApplyCalibration()
    {
        rawGyroRotation.Rotate(0f, -calibrationYAngle, 0f, Space.World); // Rotates y angle back however much it deviated when calibrationYAngle was saved.
    }

    public void SetEnabled(bool value)
    {
        enabled = true;
        StartCoroutine(CalibrateYAngle());
    }
}