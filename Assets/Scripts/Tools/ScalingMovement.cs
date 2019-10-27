using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingMovement : HandTrackedInputReciever
{
    public enum State { None, TranslateLeft, TranslateRight, FullTransform };
    public State transformState;

    public float worldScale = 1.0f;
    public Vector3 worldOrigin = Vector3.zero;
    public Quaternion worldRotation = Quaternion.identity;

    public float maxLogScale = 3.0f;
    public float minLogScale = -1.0f;

    public Vector3 worldBounds = Vector3.one * 1e3f;
    public enum CameraRotationConstraints { None, YOnly, FullRotation }
    CameraRotationConstraints cameraRotationConstraints = CameraRotationConstraints.YOnly;

    public bool leftPressed = false;
    public bool rightPressed = false;
    private State state {
        get { return leftPressed && rightPressed
                ? State.FullTransform : leftPressed
                ? State.TranslateLeft : rightPressed
                ? State.TranslateRight
                : State.None;
        }
    }

    public Transform transformLeft = null;
    public Transform transformRight = null;

    public Vector3 initialLeftPos = Vector3.zero;
    public Vector3 initialRightPos = Vector3.zero;
    public Vector3 initialMidpoint = Vector3.zero;
    float initialDist = 0f;

    public Vector3 initialOrigin = Vector3.zero;
    float initialScale = 1f;
    public Quaternion initialRotation = Quaternion.identity;
    public override void OnGripPressed(HandTrackedInfo info) {
        Debug.Log("PRESSED");
        if (info.direction == HandTrackedInfo.Direction.Left && !leftPressed)
        {
            leftPressed = true;
            initialLeftPos = info.transform.position;
            transformLeft = info.transform;
        } else if (info.direction == HandTrackedInfo.Direction.Right && !rightPressed)
        {
            rightPressed = true;
            initialRightPos = info.transform.position;
            transformRight = info.transform;
        }
        if (leftPressed && rightPressed)
        {
            initialMidpoint = (initialLeftPos + initialRightPos) * 0.5f;
            initialDist = Vector3.Distance(initialLeftPos, initialRightPos);
        }
        transformState = state;
    }
    public override void OnGripReleased(HandTrackedInfo info)
    {
        Debug.Log("RELEASED");
        if (info.direction == HandTrackedInfo.Direction.Left && leftPressed)
        {
            leftPressed = false;
        } else if (info.direction == HandTrackedInfo.Direction.Right && rightPressed)
        {
            rightPressed = false;
        }
        transformState = state;
    }
    void Update()
    {
        if (transformState != State.None)
        {
            var left = transformLeft.position;
            var right = transformRight.position;
            var mp = (left + right) * 0.5f;
            var offset = mp - initialMidpoint;
            var dist = Vector3.Distance(left, right);
            var scale = initialScale * dist / initialDist;
            scale = Mathf.Clamp(scale, Mathf.Pow(10f, minLogScale), Mathf.Pow(10f, maxLogScale));
            var newOrigin = (initialOrigin / initialScale + offset) * scale;
            worldScale = scale;

            if (scale != worldScale)
            {
                worldScale = scale;
                transform.localScale = Vector3.one / worldScale;
            }
            if (newOrigin != worldOrigin)
            {
                worldOrigin = newOrigin;
                transform.position = worldOrigin;
            }

            // apply rotation iff two controllers active
            if (transformState == State.FullTransform)
            {
                var delta = left - right;
                var initialDelta = initialLeftPos - initialRightPos;
                var newRotation = getRotation(delta, initialDelta);
                transform.rotation = newRotation;
            }
        } 
    }
    public Quaternion getRotation(Vector3 delta, Vector3 initialDelta)
    {
        switch (cameraRotationConstraints)
        {
            case CameraRotationConstraints.None: return initialRotation;
            case CameraRotationConstraints.YOnly:
                return initialRotation * Quaternion.FromToRotation(
                    new Vector3(initialDelta.x, 0.0f, initialDelta.z).normalized,
                    new Vector3(delta.x, 0.0f, delta.z).normalized);
            case CameraRotationConstraints.FullRotation:
                return initialRotation * Quaternion.FromToRotation(initialDelta, delta);
        }
        throw new System.Exception("unhandled rotation mode " + cameraRotationConstraints);
    }

    public float getMovementSpeed() { return 1.0f; }
    public Vector3 getManipMovement(Vector3 leftPos, Vector3 rightPos)
    {
        switch (transformState)
        {
            case State.None: return Vector3.zero;
            case State.TranslateLeft: return leftPos * getMovementSpeed();
            case State.TranslateRight: return rightPos * getMovementSpeed();
            case State.FullTransform: return (leftPos + rightPos) / 2.0f * getMovementSpeed();
        }
        throw new System.Exception("unhandled state " + transformState);
    }

    public float getScaleFactorChange(Vector3 leftDelta, Vector3 rightDelta, float relDelta)
    {
        switch (transformState)
        {
            case State.FullTransform: return relDelta;
            default: return 1.0f;
        }
    }
    

}
