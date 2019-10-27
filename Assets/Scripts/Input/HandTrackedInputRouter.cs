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

    Collider focused;

    void Update()
    {
        // Highlight the focused object
        var t = controllerHandle.transform;
        var ray = new Ray(t.position, t.rotation * Vector3.forward);
        var hitInfo = new RaycastHit();
        var hit = Physics.Raycast(ray, out hitInfo, 1000f, LayerMask.NameToLayer("interactive"));
        if (focused != null) focused.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        if (hit) {
            hitInfo.collider.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            focused = hitInfo.collider;
        }
        hit = false;
        
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
