﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DrawMolecule))]
public class ModalInputSwitcher : HandTrackedInputReciever
{
    public enum ActiveTool { Draw }
    public ActiveTool activeTool;

    DrawMolecule draw;

    public void Start ()
    {
        draw = GetComponent<DrawMolecule>();
    }

    public HandTrackedInputReciever target {
        get {
            switch (activeTool)
            {
                case ActiveTool.Draw: return draw;
            }
            return null;
        }
    }

    Focusable focusedObj;

    void tryHandleMouseover (HandTrackedInfo info)
    {
        if (info.raycastHit == false) return;
        var target = info.raycastInfo.collider.GetComponent<Focusable>();
        if (focusedObj) focusedObj.SetFocused(false);
        if (target) target.SetFocused(true);
        focusedObj = target;
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
        tryHandleMouseover(info);
        target?.OnGripPressed(info);
    }
    public override void OnGripReleased(HandTrackedInfo info)
    {
        tryHandleMouseover(info);
        target?.OnGripReleased(info);
    }
}
