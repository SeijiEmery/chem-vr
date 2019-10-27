using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicBond : MonoBehaviour, IFocusable
{

    float initialLength = 1f;
    Atom first = null;
    Atom second = null;
    public bool isElectronGroup { get { return electronCount <= 0; } }

    SpringJoint joint = null;
    public int electronCount = 0;

    public void Start () { initialLength = transform.localScale.y; }
    public bool SetBond(Atom first, Atom second)
    {
        if (!first.TryCreateBond(second, this)) return false;
        if (!second.TryCreateBond(first, this))
        {
            first.UncreateBond(second, this);
            return false;
        }
        this.first = first; this.second = second;
        if (joint != null)
        {
            GameObject.DestroyImmediate(joint);
            joint = null;
        }
        if (first != null && second != null)
        {
           
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (first != null && second != null)
        {
            var p1 = first.transform.position;
            var p2 = second.transform.position;
            transform.position = (p1 + p2) * 0.5f;
            var dist = Vector3.Distance(p1, p2);
            //transform.rotation = Quaternion.LookRotation((p1 - p2).normalized, Vector3.Cross(p1, p2).normalized);
            transform.LookAt(p1);
            transform.rotation = transform.rotation * Quaternion.AngleAxis(90f, Vector3.right);
            //transform.rotation *= (Quaternion.AngleAxis(90f, Vector3.forward));
            //transform.localScale = new Vector3(
            //    transform.localScale.x,
            //     transform.localScale.y,
            //     dist);
            transform.localScale = new Vector3(transform.localScale.x, dist * .5F, transform.localScale.z);
        }
        if (first == null || second == null)
        {
            GameObject.DestroyImmediate(this.gameObject);
        }
    }

    public void DestroyBond ()
    {

    }

    internal static AtomicBond MakeElectronGroup()
    {
        return new AtomicBond() { first = null, second = null, electronCount = 1 };
    }

    internal void SetElectronCount(int v)
    {
        electronCount = v ;
    }

    internal static AtomicBond Create(Atom atom, Atom other)
    {
        var bond = new AtomicBond();
        bond.SetBond(atom, other);
        return bond;
    }

    public void OnSetFocused(bool focused)
    {
        if (focused)
        {
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
        else
        {
            GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
    public float bondLength // all bond lengths in non-adj nanometers
    {
        get
        {
            switch (Mathf.Min(first.atomicNumber, second.atomicNumber))
            {
                case 1: switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 1: return 0.074f; // H-H
                        case 6: return 0.109f; // H-C
                        case 7: return 0.096f; // H-N
                        case 9: return 0.092f; // H-F
                        default: return 0.1f;
                    } break;
                case 6:
                    switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 6: return 0.154f; // C-C
                        case 7: return 0.147f; // C-N
                        case 8: return 0.143f; // C-O
                        case 16: return 0.182f; // C-S
                        case 9: return 0.147f; // C-F
                        case 17: return 0.177f; // C-Cl
                        case 35: return 0.214f; // C-I
                        default: return 0.15f;
                    } break;
                case 7:
                    switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 7: return 0.145f; // N-N
                        default: return 0.15f;
                    } break;
                case 8:
                    switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 8: return 0.148f; // O-O
                        default: return 0.15f;
                    } break;
                case 9:
                    switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 9: return 0.142f; // F-F
                        default: return 0.15f;
                    } break;
                case 17:
                    switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 17: return 0.199f; // Cl-Cl
                        default: return 0.2f;
                    }
                    break;
                case 35:
                    switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 35: return 0.228f; // Br-Br
                        default: return 0.2f;
                    }
                    break;
                case 53:
                    switch (Mathf.Max(first.atomicNumber, second.atomicNumber))
                    {
                        case 53: return 0.267f; // I-I
                        default: return 0.22f;
                    }
                    break;
                default: return 0.15f;
            }
        }
    }


    public void CreateBondConstraints ()
    {
        joint = first.gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = second.gameObject.GetComponent<Rigidbody>();
        joint.connectedAnchor = (first.transform.position - second.transform.position).normalized * bondLength * 0.1f;
        joint.spring = 1.0f;
    }
}
