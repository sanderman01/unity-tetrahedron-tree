// Copyright(C) 2017, Alexander Verbeek

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A basic orbital camera.
/// </summary>
public class OrbitCamera : MonoBehaviour {
    // This is the target we'll orbit around
    [SerializeField]
    private FocusPoint target;

    // Our desired distance from the target object.
    [SerializeField]
    private float distance = 5;

    [SerializeField]
    private float damping = 2;

    // These will store our currently desired angles
    private Quaternion pitch;
    private Quaternion yaw;

    // this is where we want to go.
    private Quaternion targetRotation;
    //private Vector3 targetPosition;

    public FocusPoint Target {
        get { return target; }
        set { target = value; }
    }

    public float Yaw {
        get { return yaw.eulerAngles.y; }
        private set { yaw = Quaternion.Euler(0, value, 0); }
    }

    public float Pitch {
        get { return pitch.eulerAngles.x; }
        private set { pitch = Quaternion.Euler(value, 0, 0); }
    }

    public void ZoomOut() {
        distance *= 2;
    }

    public void ZoomIn() {
        distance *= 0.5f;
    }

    public void Move(float yawDelta, float pitchDelta) {
        yaw = yaw * Quaternion.Euler(0, yawDelta, 0);
        pitch = pitch * Quaternion.Euler(pitchDelta, 0, 0);
        ApplyConstraints();
    }

    private void ApplyConstraints() {
        // We'll simply use lerp to move a bit towards the focus target's orientation. Just enough to get back within the constraints.
        // This way we don't need to worry about wether we need to move left or right, up or down.

        if (target.YawLimit < 180) {
            Quaternion targetYaw = Quaternion.Euler(0, target.transform.rotation.eulerAngles.y, 0);
            float yawDifference = Quaternion.Angle(yaw, targetYaw);
            float yawOverflow = yawDifference - target.YawLimit;
            if (yawOverflow > 0) { yaw = Quaternion.Slerp(yaw, targetYaw, yawOverflow / yawDifference); }
        }
        if (target.PitchLimit < 180) {
            Quaternion targetPitch = Quaternion.Euler(target.transform.rotation.eulerAngles.x, 0, 0);
            float pitchDifference = Quaternion.Angle(pitch, targetPitch);
            float pitchOverflow = pitchDifference - target.PitchLimit;
            if (pitchOverflow > 0) { pitch = Quaternion.Slerp(pitch, targetPitch, pitchOverflow / pitchDifference); }
        }
    }

    void Awake() {
        // initialise our pitch and yaw settings to our current orientation.
        pitch = Quaternion.Euler(this.transform.rotation.eulerAngles.x, 0, 0);
        yaw = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0);
    }

    void Update() {
        // calculate target positions
        targetRotation = yaw * pitch;
        //targetPosition = target.transform.position + targetRotation * (-Vector3.forward * distance);

        // apply movement damping
        // (Yeah I know this is not a mathematically correct use of Lerp. We'll never reach destination. Sue me!)
        // (It doesn't matter because we are damping. We Do Not Need to arrive at our exact destination, we just want to move smoothly and get really, really close to it.)
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, Mathf.Clamp01(Time.smoothDeltaTime * damping));

        // offset the camera at distance from the target position.
        Vector3 offset = this.transform.rotation * (-Vector3.forward * distance);
        this.transform.position = target.transform.position + offset;

        // alternatively, if we desire a slightly different behaviour, we could also add damping to the target position. But this can lead to awkward behaviour if the user rotates quickly or the damping is low.
        //this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Mathf.Clamp01(Time.smoothDeltaTime * damping));
    }
}