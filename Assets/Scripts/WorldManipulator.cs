using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class WorldManipulator : MonoBehaviour
{
    // public SteamVR_TrackedObject leftController;
    // public SteamVR_TrackedObject rightController;
    public Transform leftController;
    public Transform rightController;

    public SteamVR_Action_Boolean cameraManipButton;

    public float   worldScale = 1.0f;
    public Vector3 worldOrigin = Vector3.zero;
    public Quaternion worldRotation = Quaternion.identity;

    public float maxScale = 100.0f;
    public float minScale = 0.1f;

    public Vector3 worldBounds = Vector3.one * 1000f;

    public enum CameraRotationConstraints
    {
        None, RotateY, FullRotation
    }
    public CameraRotationConstraints cameraRotationMode = CameraRotationConstraints.RotateY;

    public enum CameraManipState
    {
        None, TranslateFromLeft, TranslateFromRight, TransformWithBothControllers
    }
    public CameraManipState cameraManipState = CameraManipState.None;

    private CameraManipState getCameraManipState ()
    {
        bool leftPressed = cameraManipButton.GetState(SteamVR_Input_Sources.LeftHand);
        bool rightPressed = cameraManipButton.GetState(SteamVR_Input_Sources.RightHand);

        return leftPressed && rightPressed ? CameraManipState.TransformWithBothControllers
            : leftPressed ? CameraManipState.TranslateFromLeft
            : rightPressed ? CameraManipState.TranslateFromRight
            : CameraManipState.None
        ;
    }
    public Vector3 initialLeftPos = Vector3.zero;
    public Vector3 initialRightPos = Vector3.zero;
    public Vector3 initialMidpoint = Vector3.zero;
    public float initialDist = 0f;

    public Vector3 initialOrigin = Vector3.zero;
    public float initialScale = 1.0f;
    public Quaternion initialRotation = Quaternion.identity;

    public Vector3 debugManipMovement = Vector3.zero;
    public float debugRelDelta = 1.0f;
    public float debugScaleFactorChange = 0.0f;
    public Vector3 debugLeft = Vector3.zero, debugRight = Vector3.zero;

    public Transform scaleTransform;
    public Transform translateTransform;
    public Transform rotateTransform;
    public Transform inverseTranslateTransform;

    private void updateCameraManip () {
        var prevState = cameraManipState;
        cameraManipState = getCameraManipState();

        bool leftPressed = cameraManipButton.GetState(SteamVR_Input_Sources.LeftHand);
        bool rightPressed = cameraManipButton.GetState(SteamVR_Input_Sources.RightHand);

        var leftPos = leftController.transform.position;
        var rightPos = rightController.transform.position;
        var midpoint = leftPressed && rightPressed ? (leftPos + rightPos) / 2f
            : leftPressed ? leftPos
            : rightPressed ? rightPos
            : Vector3.zero;

        var dist = leftPressed && rightPressed ? Vector3.Distance(leftPos, rightPos)
            : 1f;

        if (cameraManipState != prevState) {
            Debug.Log("switching from " + prevState + " to " + cameraManipState);
            initialLeftPos = leftPos;
            initialRightPos = rightPos;
            initialScale = worldScale;
            initialOrigin = transform.position;
            initialRotation = transform.rotation;

            initialMidpoint = midpoint;
            initialDist = dist;

        } else if (cameraManipState != CameraManipState.None) {
            Debug.Log("updating with " + cameraManipState);

            var offset = midpoint - initialMidpoint;
            var scale = initialScale * dist / initialDist;
            //  scale = Mathf.Clamp(scale, minScale, maxScale);

            var newOrigin = initialOrigin + offset;
            inverseTranslateTransform.position = initialOrigin + offset;
            translateTransform.position = initialOrigin - offset;
            scaleTransform.localScale = Vector3.one * Mathf.Pow(scale / initialScale, 2f);





          //  var newOrigin = (initialOrigin + offset) * (scale / initialScale + 1f);
            //var newOrigin = (initialOrigin / initialScale + offset) *scale;               
            //var newOrigin = initialOrigin * (1f - scale / initialScale) + offset; 
           // initialOrigin - initialOrigin / scale * initialScale;     
           //     (initialOrigin / initialScale + offset) * scale;
            /*if (scale != worldScale)
            {
                worldScale = scale;
                scaleTransform.localScale = Vector3.one * worldScale;
            }
            if (newOrigin != worldOrigin)
            {
                Debug.Log(newOrigin);
                worldOrigin = newOrigin;
                translateTransform.position = worldOrigin;
            }
            */
            // apply rotation iff two controllers active
           /* if (cameraManipState == CameraManipState.TransformWithBothControllers)
            {
                var delta = leftPos - rightPos;
                var initialDelta = initialLeftPos - initialRightPos;
                var newRotation = getRotation(delta, initialDelta);
                transform.rotation = newRotation;
            }*/
        }
    }

    public Quaternion getRotation (Vector3 delta, Vector3 initialDelta)
    {
        switch (cameraRotationMode)
        {
            case CameraRotationConstraints.None: return initialRotation;
            case CameraRotationConstraints.RotateY: return initialRotation * Quaternion.FromToRotation(
                new Vector3(initialDelta.x, 0.0f, initialDelta.z).normalized,
                new Vector3(delta.x, 0.0f, delta.z).normalized);
            case CameraRotationConstraints.FullRotation: return initialRotation * Quaternion.FromToRotation(
                initialDelta, delta);
        }
        throw new System.Exception("unhandled rotation mode " + cameraRotationMode);
    }


    public float getMovementSpeed () { return 1.0f; }

    public Vector3 getManipMovement (Vector3 leftPos, Vector3 rightPos) {
        switch (cameraManipState)
        {
            case CameraManipState.None: return Vector3.zero;
            case CameraManipState.TranslateFromLeft: return leftPos * getMovementSpeed();
            case CameraManipState.TranslateFromRight: return rightPos * getMovementSpeed();
            case CameraManipState.TransformWithBothControllers: return (leftPos + rightPos) / 2.0f * getMovementSpeed();
        }
        throw new System.Exception("unhandled state " + cameraManipState);
    }

    public float getScaleFactorChange (Vector3 leftDelta, Vector3 rightDelta, float relDelta)
    {
        switch (cameraManipState)
        {
            case CameraManipState.TransformWithBothControllers: return relDelta;
            default: return 1.0f;
        }
    }

    public void Update()
    {
        updateCameraManip();
    }
}
