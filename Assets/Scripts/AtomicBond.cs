using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicBond : MonoBehaviour
{

    float initialLength = 1f;
    Atom first = null;
    Atom second = null;
    SpringJoint joint = null;

    public void Start () { initialLength = transform.localScale.y; }
    public void SetBond(Atom first, Atom second)
    {
        if (first == this.first && second == this.second) return;
        this.first = first; this.second = second;
        if (joint != null)
        {
            GameObject.DestroyImmediate(joint);
            joint = null;
        }
        if (first != null && second != null)
        {
            joint = first.gameObject.AddComponent<SpringJoint>();
            joint.connectedBody = second.gameObject.GetComponent<Rigidbody>();
            joint.spring = 1.0f;
        }
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
    }
}
