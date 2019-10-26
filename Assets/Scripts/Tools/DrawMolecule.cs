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
    public float minBondFormationRadius = 0.01f;

    struct Bond { public int from; public int to; }
    List<AtomicBond> newBonds = new List<AtomicBond>();

    public override void OnTriggerPressed(HandTrackedInfo info)
    {
        Debug.Log("trigger pressed: " + info);
        if (drawnAtom == null)
        {
            atoms = gameObject.GetComponentsInChildren<Atom>();
            drawnAtom = GameObject.Instantiate(atomPrefab.gameObject, info.transform.position, info.transform.rotation, transform);
            drawnAtom.transform.position = info.transform.position;
            connectAtomToHandControllerJoint = drawnAtom.AddComponent<FixedJoint>();
            connectAtomToHandControllerJoint.connectedBody = info.rigidbody;
            newBonds.Clear();
        }
    }
    public override void OnTriggerReleased(HandTrackedInfo info)
    {
        if (drawnAtom != null)
        {
            drawnAtom = null;
            Object.DestroyImmediate(connectAtomToHandControllerJoint);
            connectAtomToHandControllerJoint = null;
            newBonds.Clear();
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
        if (atoms == null || drawnAtom == null) return;
        int i = 0; AtomicBond bond;
        foreach (var atom in atoms)
        {
            var dist = Vector3.Distance(atom.transform.position, drawnAtom.transform.position);
            //Debug.Log("" + i + ": " + dist + ": " + (dist <= bondFormationRadius));
            if (dist <= bondFormationRadius && dist > minBondFormationRadius)
            {
                if (i < newBonds.Count)
                {
                    bond = newBonds[i++];
                } else
                {
                    bond = GameObject.Instantiate(bondPrefab, transform.position, transform.rotation, transform);
                    newBonds.Add(bond);
                }
                bond.SetBond(atom, drawnAtom.GetComponent<Atom>());
            }
        }
       Debug.Log("found " + i + " bonds this cycle, have " + newBonds.Count);
       while (i < newBonds.Count)
       {
            var j = i;
            //var j = newBonds.Count - 1;
            var obj = newBonds[j];
            GameObject.Destroy(obj);
            newBonds.RemoveAt(j);
      }
    }
}
