using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class DrawMolecule : HandTrackedInputReciever
{
    public Atom atomPrefab;
    public AtomicBond bondPrefab;
    AtomicTemplate template;
    GameObject drawnAtom = null;
    FixedJoint connectAtomToHandControllerJoint;
    Atom[] atoms;
    public float bondFormationRadius;
    public float minBondFormationRadius = 0.01f;

    struct Bond { public int from; public int to; }
    List<AtomicBond> newBonds = new List<AtomicBond>();

    IFocusable focusTarget = null;
    GameObject focusObject = null;
    enum FocusType { None, AtomicTemplate, Atom, Bond, Unknown };
    FocusType focusType = FocusType.None;
    public enum EditMode { Draw, AddBonds, Delete };
    EditMode editMode = EditMode.Draw;


    public TextMeshPro editModeText;
   void toggleNextEditMode ()
    {
        editMode += 1;
        if (editMode > EditMode.Delete)
        {
            editMode = EditMode.Draw;
        }
        switch (editMode)
        {
            case EditMode.Draw: editModeText.text = "Draw"; break;
            case EditMode.AddBonds: editModeText.text = "Add bonds"; break;
            case EditMode.Delete: editModeText.text = "Delete"; break;
        }
    }


    public TextMeshPro tooltipTextMesh;
    public Transform playerTarget;

    GameObject targetBond;
    AtomicBond targetBondObj;

    public override void OnFocusChanged(IFocusable focused, GameObject go, HandTrackedInfo.Direction direction, GameObject origin)
    {
        if (focused != focusTarget)
        {
            focusObject = go;
            if (focusTarget != null) focusTarget.OnSetFocused(false);
            focusTarget = focused;
            if (focusTarget != null) focusTarget.OnSetFocused(true);

            if (focusTarget == null)
            {
                focusType = FocusType.None;
            } else
            {
                var template = go.GetComponent<AtomicTemplate>();
                if (template != null)
                {
                    //tooltipTextMesh.gameObject.SetActive(false);
                    focusType = FocusType.AtomicTemplate;
                    tooltipTextMesh.text = template.name;
                    tooltipTextMesh.gameObject.transform.position = template.gameObject.transform.position
                        + Vector3.back * 0.22f;
                    //tooltipTextMesh.gameObject.transform.LookAt(playerTarget, Vector3.up);
                } else
                {
                    //tooltipTextMesh.gameObject.SetActive(false);
                }


                if (go.GetComponent<AtomicTemplate>() != null) focusType = FocusType.AtomicTemplate;
                else if (go.GetComponent<Atom>() != null) focusType = FocusType.Atom;
                else if (go.GetComponent<AtomicBond>() != null) focusType = FocusType.Bond;
                else focusType = FocusType.Unknown;
            }
        }
        if (editMode == EditMode.AddBonds)
        {
            if (targetBond != null && targetBondObj != focusObject)
            {
               //  targetBond.DestroyBond();
            }
            // startBoundsFrom.GetComponent<Atom>().
        }
    }
    GameObject SpawnAtom (HandTrackedInfo info, AtomicTemplate template)
    {
        var atom = GameObject.Instantiate(atomPrefab.gameObject, info.transform.position, info.transform.rotation, transform);
        atom.transform.position = info.transform.position;
        atom.GetComponent<Atom>().SetAtomType(template);
        atom.transform.localScale = Vector3.one * template.radius * 0.1f;
        atom.GetComponent<Renderer>().material.color = template.color;
        atom.GetComponent<Rigidbody>().mass = template.mass;
        return atom;
    }
    void SpawnAndEditAtom(HandTrackedInfo info)
    {
        if (drawnAtom == null && template != null)
        {
            atoms = gameObject.GetComponentsInChildren<Atom>();
            drawnAtom = SpawnAtom(info, template);
            connectAtomToHandControllerJoint = drawnAtom.AddComponent<FixedJoint>();
            connectAtomToHandControllerJoint.connectedBody = info.rigidbody;
            newBonds.Clear();
        }
    }

    public override void OnTriggerPressed(HandTrackedInfo info)
    {
        switch (editMode)
        {
            case EditMode.Draw:
                {
                    switch (focusType)
                    {
                        case FocusType.None:
                            {
                                SpawnAndEditAtom(info);
                                break;
                            }
                        case FocusType.AtomicTemplate:
                            {
                                template = focusObject.GetComponent<AtomicTemplate>();
                                SpawnAndEditAtom(info);
                                break;
                            }
                        case FocusType.Atom:
                            {
                                drawnAtom = focusObject;
                                connectAtomToHandControllerJoint = drawnAtom.AddComponent<FixedJoint>();
                                connectAtomToHandControllerJoint.connectedBody = info.rigidbody;
                                break;
                            }
                        case FocusType.Bond:
                            {
                                break;
                            }
                    }
                }
                break;
            case EditMode.AddBonds:
                switch (focusType)
                {
                    case FocusType.None:
                        {
                            break;
                        }
                    case FocusType.AtomicTemplate:
                        {
                            break;
                        }
                    case FocusType.Atom:
                        {
                            startBoundsFrom = focusObject;
                            break;
                        }
                    case FocusType.Bond:
                        {
                            break;
                        }
                }
                break;
            case EditMode.Delete:
                switch (focusType)
                {
                    case FocusType.None:
                        {
                            break;
                        }
                    case FocusType.AtomicTemplate:
                        {
                            template = focusObject.GetComponent<AtomicTemplate>();
                            SpawnAndEditAtom(info);
                            break;
                        }
                    case FocusType.Atom:
                        {
                            GameObject.DestroyImmediate(focusObject);
                            break;
                        }
                    case FocusType.Bond:
                        {
                            GameObject.DestroyImmediate(focusObject);
                            break;
                        }
                }
                break;
        }
    }

    private GameObject startBoundsFrom;

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
        }else 
        toggleNextEditMode();
    }

    List<AtomicBond> tempBonds = new List<AtomicBond>();

    public void FixedUpdate()
    {
        if (atoms == null || drawnAtom == null) return;
        return;
        foreach (var item in tempBonds)
        {
            item.DestroyBond();
        }

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
