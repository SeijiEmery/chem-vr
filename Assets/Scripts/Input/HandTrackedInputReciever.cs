using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(InputActionMapping))]
public abstract class HandTrackedInputReciever : MonoBehaviour {

    [HideInInspector]
    public InputActionMapping actions;
    void Start ()
    {
        actions = GetComponent<InputActionMapping>();
    }
    public virtual void OnTriggerPressed(HandTrackedInfo info) { }
    public virtual void OnTriggerReleased(HandTrackedInfo info) { }
    public virtual void OnCancelPressed(HandTrackedInfo info) { }
    public virtual void OnGripPressed(HandTrackedInfo info) { }
    public virtual void OnGripReleased(HandTrackedInfo info) { }
}
