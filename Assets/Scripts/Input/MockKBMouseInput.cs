using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


[RequireComponent(typeof(ModalInputSwitcher))]
    public class MockKBMouseInput : MonoBehaviour
{
    ModalInputSwitcher inputTarget;
    public bool enabled = false;
    public float rayDepth = 10f;
    public Rigidbody mockMousePosTarget;
    public SteamVR_Behaviour_Pose defaultActionPose;
    public Camera fallbackCamera;

    // Start is called before the first frame update
    void Start()
    {
        inputTarget = GetComponent<ModalInputSwitcher>();
        if (Camera.current == null || Camera.current == fallbackCamera)
        {
            fallbackCamera.gameObject.SetActive(true);
        } else
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabled) return;
        Debug.Log("" + (Camera.current == null));
        var mouseRay = Camera.current.ScreenPointToRay(Input.mousePosition);
        var mousePos3d = mouseRay.GetPoint(rayDepth);
        mockMousePosTarget.transform.position = mousePos3d;
        var eventInfo = new HandTrackedInfo()
        {
            pose = defaultActionPose,
            gameobject = mockMousePosTarget.gameObject,
            transform = mockMousePosTarget.transform,
            rigidbody = mockMousePosTarget,
            direction = HandTrackedInfo.Direction.Left,
            pressed = false,
            down = false
        };
        if (Input.GetButtonDown("Fire1"))
        {
            eventInfo.pressed = true; eventInfo.down = true;
            inputTarget.OnTriggerPressed(eventInfo);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            eventInfo.pressed = false; eventInfo.down = true;
            inputTarget.OnTriggerReleased(eventInfo);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            eventInfo.pressed = true; eventInfo.down = true;
            inputTarget.OnCancelPressed(eventInfo);
        }
    }
}
