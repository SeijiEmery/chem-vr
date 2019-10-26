using System.Collections;
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

    public override void OnTriggerPressed(HandTrackedInfo info)
    {
        target?.OnTriggerPressed(info);
    }
    public override void OnTriggerReleased(HandTrackedInfo info)
    {
        target?.OnTriggerReleased(info);
    }
    public override void OnCancelPressed(HandTrackedInfo info)
    {
        target?.OnCancelPressed(info);
    }
    public override void OnGripPressed(HandTrackedInfo info)
    {
        target?.OnGripPressed(info);
    }
    public override void OnGripReleased(HandTrackedInfo info)
    {
        target?.OnGripReleased(info);
    }
}
