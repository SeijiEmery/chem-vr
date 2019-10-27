using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class AtomicTemplate : Focusable
{
    public float mass;
    public float radius;
    public string name;
    public string symbol;
    public Color color;
    public void OnFocus(bool focused)
    {
        var renderer = GetComponent<Renderer>();
        if (focused) {
            renderer.material.color = Color.green;
        } else {
            renderer.material.color = color;
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
