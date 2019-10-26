using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DrawMolecule : HandTrackedInputReciever
{
    public Molecule atomPrefab;
    GameObject drawnAtom = null;
    FixedJoint connectAtomToHandControllerJoint;

    public override void OnTriggerPressed(HandTrackedInfo info)
    {
        Debug.Log("trigger pressed: " + info);
        if (drawnAtom == null)
        {
            drawnAtom = GameObject.Instantiate(atomPrefab.gameObject);
            drawnAtom.transform.position = info.transform.position;
            connectAtomToHandControllerJoint = drawnAtom.AddComponent<FixedJoint>();
            connectAtomToHandControllerJoint.connectedBody = info.rigidbody;
        }
    }
    public override void OnTriggerReleased(HandTrackedInfo info)
    {
        if (drawnAtom != null)
        {
            drawnAtom = null;
            Object.DestroyImmediate(connectAtomToHandControllerJoint);
            connectAtomToHandControllerJoint = null;
        }
    }
    public override void OnCancelPressed(HandTrackedInfo info)
    {
        if (drawnAtom != null)
        {
            GameObject.DestroyImmediate(drawnAtom);
            drawnAtom = null;
            connectAtomToHandControllerJoint = null;
        }
    }
}
