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
    List<AtomicBond> newBonds = new List<AtomicBond>();

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
            Debug.Log("" + i + ": " + dist + ": " + (dist <= bondFormationRadius));
            if (dist <= bondFormationRadius)
            {
                //if (i < atoms.Length)
                //{
                //    bond = newBonds[i];
                //} else
                //{
                    bond = GameObject.Instantiate(bondPrefab, transform.position, transform.rotation, transform);
                    newBonds.Add(bond);
                //}
                bond.SetBond(atom, drawnAtom.GetComponent<Atom>());
            }
            ++i;
        }
       while (i < newBonds.Count)
       {
            var obj = newBonds[i];
            GameObject.Destroy(obj);
            newBonds.RemoveAt(i);
       }
    }
}
