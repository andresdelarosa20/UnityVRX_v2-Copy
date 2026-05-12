using System;
using UnityEngine;

public static class UISObserver
{

    // HMD - HUD

    public static Action<float> LC_RC_Distance;
    public static Action<float> RC_TO_HMD_Distance;
    public static Action<float> LC_TO_HMD_Distance;
    public static Action<Vector3> HMD_Position;
    public static Action<Vector3> HMD_Rotation;
    public static Action<float> HMD_Velocity;
    public static Action<float> HMD_VMagnitud;

    // LEFT CONTROLLER

    public static Action<bool> LC_TriggerButton;
    public static Action<bool> LC_GripButton;
    public static Action<bool> LC_AButton;
    public static Action<bool> LC_BButton;
    public static Action<bool> LC_MenuButton;
    public static Action<bool> LC_StickButton;
    public static Action<bool> LC_StickNorthPos;
    public static Action<bool> LC_StickSouthPos;
    public static Action<bool> LC_StickWestPos;
    public static Action<bool> LC_StickEastPos;

    public static Action<Vector3> LC_Position;
    public static Action<Vector3> LC_Rotation;
    public static Action<Vector3> LC_Velocity;
    public static Action<float> LC_VelocityMagnitud;

    public static Action<float> LC_TriggerDepth;
    public static Action<float> LC_GripDepth;
    public static Action<float> LC_StickNorthDepth;
    public static Action<float> LC_StickSouthDepth;
    public static Action<float> LC_StickWestDepth;
    public static Action<float> LC_StickEastDepth;

    public static Action<float> LC_StickXPos;
    public static Action<float> LC_StickYPos;

    // RIGHT CONTROLLER

    public static Action<bool> RC_TriggerButton;
    public static Action<bool> RC_GripButton;
    public static Action<bool> RC_AButton;
    public static Action<bool> RC_BButton;
    public static Action<bool> RC_MenuButton;
    public static Action<bool> RC_StickButton;
    public static Action<bool> RC_StickNorthPos;
    public static Action<bool> RC_StickSouthPos;
    public static Action<bool> RC_StickWestPos;
    public static Action<bool> RC_StickEastPos;

    public static Action<Vector3> RC_Position;
    public static Action<Vector3> RC_Rotation;
    public static Action<Vector3> RC_Velocity;
    public static Action<float> RC_VelocityMagnitud;

    public static Action<float> RC_TriggerDepth;
    public static Action<float> RC_GripDepth;
    public static Action<float> RC_StickNorthDepth;
    public static Action<float> RC_StickSouthDepth;
    public static Action<float> RC_StickWestDepth;
    public static Action<float> RC_StickEastDepth;

    public static Action<float> RC_StickXPos;
    public static Action<float> RC_StickYPos;

}
