using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandTrackedInputRouter : MonoBehaviour
{
    public HandTrackedInputReciever target;
    public SteamVR_Behaviour_Pose trackedObj;
    public Rigidbody controllerHandle;
    public HandTrackedInfo.Direction direction;

    void Start ()
    {
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
    }

    HandTrackedInfo MakeTrackingInfo (bool pressed, bool down)
    {
        var t = controllerHandle.transform;
        var ray = new Ray(t.position, t.rotation * Vector3.forward);
        var hitInfo = new RaycastHit();
        var hit = Physics.Raycast(ray, out hitInfo, 1000f, LayerMask.NameToLayer("interactive"));
        return new HandTrackedInfo()
        {
            direction = direction,
            pose = trackedObj,
            gameobject = controllerHandle.gameObject,
            rigidbody = controllerHandle.GetComponent<Rigidbody>(),
            transform = controllerHandle.transform,
            down = down,
            pressed = pressed,
            raycastHit = hit,
            raycastInfo = hitInfo
        };
    }

    GameObject focusedRaycastObject = null;
    GameObject focusedTriggerObject = null;

    public void OnTriggerEnter(Collider other)
    {
        var focused = other.GetComponent<IFocusable>();
        if (focused != null && other.gameObject != focusedTriggerObject && focusedRaycastObject != focusedRaycastObject)
        {
            focusedTriggerObject = other.gameObject;
            target.OnFocusChanged(focused, direction, gameObject);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (focusedTriggerObject == other.gameObject)
        {
            focusedTriggerObject = null;
            target.OnFocusChanged(null, direction, gameObject);
        }
    }

    void Update()
    {
        // Highlight the focused object
        if (focusedTriggerObject == null)
        {
            var t = controllerHandle.transform;
            Ray ray = new Ray(t.position, t.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != focusedRaycastObject)
                {
                    var focused = hit.collider.GetComponent<IFocusable>();
                    target.OnFocusChanged(focused, direction, gameObject);
                    focusedRaycastObject = hit.collider.gameObject;
                }
            }
        }
        
        if (target.actions.trigger.GetStateDown(trackedObj.inputSource))
        {
            target.OnTriggerPressed(MakeTrackingInfo(true, true));
        }
        else if (target.actions.trigger.GetStateUp(trackedObj.inputSource))
        {
            target.OnTriggerReleased(MakeTrackingInfo(false, true));
        }
        if (target.actions.cancel.GetStateDown(trackedObj.inputSource))
        {
            target.OnCancelPressed(MakeTrackingInfo(true, true));
        }
        if (target.actions.grip.GetStateDown(trackedObj.inputSource))
        {
            target.OnGripPressed(MakeTrackingInfo(true, true));
        }
        else if (target.actions.grip.GetStateUp(trackedObj.inputSource))
        {
            target.OnGripReleased(MakeTrackingInfo(false, true));
        }
    }
}
