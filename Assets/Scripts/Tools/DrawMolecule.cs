using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DrawMolecule : HandTrackedInputReciever
{
    public Atom atomPrefab;
    public AtomicBond bondPrefab;
    GameObject drawnAtom = null;
    FixedJoint connectAtomToHandControllerJoint;
    Atom[] atoms;
    public float bondFormationRadius;

    struct Bond { public int from; public int to; }
    List<Bond> currentBonds = new List<Bond>();
    List<Bond> prevBonds = new List<Bond>();

    public override void OnTriggerPressed(HandTrackedInfo info)
    {
        Debug.Log("trigger pressed: " + info);
        if (drawnAtom == null)
        {
            drawnAtom = GameObject.Instantiate(atomPrefab.gameObject, info.transform.position, info.transform.rotation, transform);
            drawnAtom.transform.position = info.transform.position;
            connectAtomToHandControllerJoint = drawnAtom.AddComponent<FixedJoint>();
            connectAtomToHandControllerJoint.connectedBody = info.rigidbody;
            atoms = gameObject.GetComponentsInChildren<Atom>();
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
    public void FixedUpdate()
    {
        var bonds = currentBonds;
        currentBonds = prevBonds;
        prevBonds = bonds;
        currentBonds.Clear();

        if (atoms == null) return;

        for (int i = 0; i < atoms.Length; ++i) {
            for (int j = 1; j < atoms.Length; ++j)
            {
                var dist = Vector3.Distance(atoms[i].transform.position, atoms[j].transform.position);
                //Debug.Log("" + i + ", " + j + ": " + dist);
                if (dist <= bondFormationRadius)
                {
                    currentBonds.Add(new Bond() { from = i, to = j });
                }
            }
        }

        var bondObjects = gameObject.GetComponentsInChildren<AtomicBond>();
        {
            int i = 0;
            for (; i < currentBonds.Count && i < bondObjects.Length; ++i)
            {
                bondObjects[i].SetBond(atoms[currentBonds[i].from], atoms[currentBonds[i].to]);
            }
            for (; i < currentBonds.Count; ++i)
            {
                var bond = GameObject.Instantiate(bondPrefab, transform.position, transform.rotation, transform);
                bond.SetBond(atoms[currentBonds[i].from], atoms[currentBonds[i].to]);
            }
        }
    }
}
