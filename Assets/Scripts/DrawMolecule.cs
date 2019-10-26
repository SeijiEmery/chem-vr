using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class DrawMolecule : MonoBehaviour
{
    public SteamVR_Action_Boolean drawAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    public Molecule atomPrefab;
    public GameObject drawnAtom = null;

    public Rigidbody attachPoint;
    SteamVR_Behaviour_Pose trackedObj;
    FixedJoint joint;

    // Start is called before the first frame update
    void Start()
    {
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        if (drawAction.GetStateDown(trackedObj.inputSource) && drawnAtom == null)
        {
            drawnAtom = GameObject.Instantiate(atomPrefab.gameObject);
            drawnAtom.transform.position = attachPoint.transform.position;
            joint = drawnAtom.AddComponent<FixedJoint>();
            joint.connectedBody = attachPoint;
        }
        else if (drawAction.GetStateUp(trackedObj.inputSource) && drawnAtom != null)
        {
            drawnAtom = null;
            Object.DestroyImmediate(joint);
            joint = null;
        }
    }
}
