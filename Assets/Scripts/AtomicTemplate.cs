using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class AtomicTemplate : MonoBehaviour, IFocusable
{
    public int atomicNumber;
    public float mass;
    public float radius;
    public string name;
    public string symbol;
    public Color color;
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


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = color;
        transform.localScale = Vector3.one * radius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
