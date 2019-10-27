using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DrawMolecule)), RequireComponent(typeof(ScalingMovement))]
public class ModalInputSwitcher : HandTrackedInputReciever
{
    public enum ActiveTool { Draw, Movement }
    public ActiveTool activeTool;

    DrawMolecule draw;
    ScalingMovement movement;

    public void Start ()
    {
        draw = GetComponent<DrawMolecule>();
        movement = GetComponent<ScalingMovement>();
    }

    public HandTrackedInputReciever target {
        get {
            switch (activeTool)
            {
                case ActiveTool.Draw: return draw;
                case ActiveTool.Movement: return movement;
            }
            return null;
        }
    }

    IFocusable focusedObj;

    public void SetFocus (IFocusable focus)
    {
        if (focusedObj != null) focusedObj.OnSetFocused(false);
        if (focus != null) focus.OnSetFocused(true);
        focusedObj = focus;
    }
    void tryHandleMouseover (HandTrackedInfo info)
    {
        /*if (info.raycastHit == false) return;
        var target = info.raycastInfo.collider.GetComponent<Focusable>();
        if (focusedObj) focusedObj.SetFocused(false);
        if (target) target.SetFocused(true);
        focusedObj = target;*/
    }

    public override void OnTriggerPressed(HandTrackedInfo info)
    {
        tryHandleMouseover(info);
        target?.OnTriggerPressed(info);
    }
    public override void OnTriggerReleased(HandTrackedInfo info)
    {
        tryHandleMouseover(info);
        target?.OnTriggerReleased(info);
    }
    public override void OnCancelPressed(HandTrackedInfo info)
    {
        tryHandleMouseover(info);
        target?.OnCancelPressed(info);
    }
    public override void OnGripPressed(HandTrackedInfo info)
    {
        Debug.Log("p");
        tryHandleMouseover(info);
        target?.OnGripPressed(info);
        movement?.OnGripPressed(info);
    }
    public override void OnGripReleased(HandTrackedInfo info)
    {
        tryHandleMouseover(info);
        target?.OnGripReleased(info);
        movement?.OnGripPressed(info);
    }
}
