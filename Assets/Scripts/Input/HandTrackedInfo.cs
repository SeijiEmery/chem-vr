using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public struct HandTrackedInfo {
    public SteamVR_Behaviour_Pose pose;
    public Rigidbody rigidbody;
    public GameObject gameobject;
    public Transform transform;
    public bool pressed;
    public bool down;
    public enum Direction { Left, Right };
    public Direction direction;
}
