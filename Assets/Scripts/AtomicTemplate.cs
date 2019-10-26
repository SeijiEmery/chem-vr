using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class AtomicTemplate : MonoBehaviour
{
    public float mass;
    public float radius;
    public string name;
    public string symbol;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Material>().color = color;
        transform.localScale = Vector3.one * radius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
