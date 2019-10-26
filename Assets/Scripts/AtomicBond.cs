using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicBond : MonoBehaviour
{

    float initialLength = 1f;
    public void Start () { initialLength = transform.localScale.y; }
    public void SetBond(Atom first, Atom second)
    {
        var p1 = first.transform.position;
        var p2 = second.transform.position;
        transform.position = (p1 + p2) * 0.5f;
        var dist = Vector3.Distance(p1, p2);
        //transform.rotation = Quaternion.LookRotation((p1 - p2).normalized, Vector3.Cross(p1, p2).normalized);
        transform.LookAt(p1);
        //transform.rotation *= (Quaternion.AngleAxis(90f, Vector3.forward));
        transform.localScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y,
            initialLength * dist);
            //transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
