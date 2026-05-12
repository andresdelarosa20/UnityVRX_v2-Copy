using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PruebaInterfacesControlador : MonoBehaviour
{
    void Start()
    {

        // HEAD UP DISPLAY (HUD)

        UISObserver.LC_RC_Distance?.Invoke(1.2f);
        UISObserver.RC_TO_HMD_Distance?.Invoke(2.3f);
        UISObserver.LC_TO_HMD_Distance?.Invoke(3.4f);
        UISObserver.HMD_Position?.Invoke(new Vector3(1f,2f,3f));
        UISObserver.HMD_Rotation?.Invoke(new Vector3(4f, 5f, 6f));
        UISObserver.HMD_Velocity?.Invoke(2.58f);
        UISObserver.HMD_VMagnitud?.Invoke(5.85f);

        // LEFT CONTROLLER

        UISObserver.LC_TriggerButton?.Invoke(true);
        UISObserver.LC_GripButton?.Invoke(true);
        UISObserver.LC_AButton?.Invoke(true);
        UISObserver.LC_BButton?.Invoke(true);
        UISObserver.LC_MenuButton?.Invoke(true);
        UISObserver.LC_StickButton?.Invoke(true);
        UISObserver.LC_StickNorthPos?.Invoke(true);
        UISObserver.LC_StickSouthPos?.Invoke(true);
        UISObserver.LC_StickWestPos?.Invoke(true);
        UISObserver.LC_StickEastPos?.Invoke(true);

        UISObserver.LC_Position?.Invoke(new Vector3(0.5f, 0.5f, 0.5f));
        UISObserver.LC_Rotation?.Invoke(new Vector3(0.5f, 0.5f, 0.5f));
        UISObserver.LC_Velocity?.Invoke(new Vector3(0.5f, 0.5f, 0.5f));
        UISObserver.LC_VelocityMagnitud?.Invoke(115f);

        UISObserver.LC_TriggerDepth?.Invoke(0.5f);
        UISObserver.LC_GripDepth?.Invoke(0.5f);
        UISObserver.LC_StickNorthDepth?.Invoke(0.5f);
        UISObserver.LC_StickSouthDepth?.Invoke(0.25f);
        UISObserver.LC_StickWestDepth?.Invoke(0.75f);
        UISObserver.LC_StickEastDepth?.Invoke(0.15f);

        UISObserver.LC_StickXPos?.Invoke(0.5f);
        UISObserver.LC_StickYPos?.Invoke(0.5f);

        // RIGHT CONTROLLER

        UISObserver.RC_TriggerButton?.Invoke(true);
        UISObserver.RC_GripButton?.Invoke(true);
        UISObserver.RC_AButton?.Invoke(true);
        UISObserver.RC_BButton?.Invoke(true);
        UISObserver.RC_MenuButton?.Invoke(true);
        UISObserver.RC_StickButton?.Invoke(true);
        UISObserver.RC_StickNorthPos?.Invoke(true);
        UISObserver.RC_StickSouthPos?.Invoke(true);
        UISObserver.RC_StickWestPos?.Invoke(true);
        UISObserver.RC_StickEastPos?.Invoke(true);

        UISObserver.RC_Position?.Invoke(new Vector3(0.5f, 0.5f, 0.5f));
        UISObserver.RC_Rotation?.Invoke(new Vector3(0.5f, 0.5f, 0.5f));
        UISObserver.RC_Velocity?.Invoke(new Vector3(0.5f, 0.5f, 0.5f));
        UISObserver.RC_VelocityMagnitud?.Invoke(115f);

        UISObserver.RC_TriggerDepth?.Invoke(0.5f);
        UISObserver.RC_GripDepth?.Invoke(0.5f);
        UISObserver.RC_StickNorthDepth?.Invoke(0.5f);
        UISObserver.RC_StickSouthDepth?.Invoke(0.25f);
        UISObserver.RC_StickWestDepth?.Invoke(0.75f);
        UISObserver.RC_StickEastDepth?.Invoke(0.15f);

        UISObserver.RC_StickXPos?.Invoke(0.5f);
        UISObserver.RC_StickYPos?.Invoke(0.5f);

    }
}
