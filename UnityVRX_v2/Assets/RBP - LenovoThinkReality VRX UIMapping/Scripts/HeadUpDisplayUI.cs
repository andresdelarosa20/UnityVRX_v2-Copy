using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HeadUpDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI LC_TO_HMD_Distance;
    public TextMeshProUGUI LC_RC_Distance;
    public TextMeshProUGUI RC_TO_HMD_Distance;
    public TextMeshProUGUI HMD_Position;
    public TextMeshProUGUI HMD_Rotation;

    public float LC_TO_HMD_Distance_Value;
    public float LC_RC_Distance_Value;
    public float RC_TO_HMD_Distance_Value;

    public Vector3 HMD_Position_Value;
    public Vector3 HMD_Rotation_Value;

    public float HMD_VMagnitud_Value;
    public float HMD_Velocity_Value;

    private void LateUpdate() 
    {
        UpdateHMDValues();
    }

    public void UpdateHMDValues()
    {
        LC_TO_HMD_Distance_Fnd(LC_TO_HMD_Distance_Value);
        LC_RC_Distance_Fnd(LC_RC_Distance_Value);
        RC_TO_HMD_Distance_Fnd(RC_TO_HMD_Distance_Value);
        HMD_Position_Fnd(HMD_Position_Value);
        HMD_Rotation_Fnd(HMD_Rotation_Value);
        HMD_VMagnitud_Fnd(HMD_VMagnitud_Value);
        HMD_Velocity_Fnd(HMD_Velocity_Value);
    }

    public void LC_TO_HMD_Distance_Fnd(float value)
    {
        LC_TO_HMD_Distance_Value = value;
        LC_TO_HMD_Distance.text = LC_TO_HMD_Distance_Value.ToString("00.00");
    }
    public void LC_RC_Distance_Fnd(float value)
    {
        LC_RC_Distance_Value = value;
        LC_RC_Distance.text = LC_RC_Distance_Value.ToString("00.00");
    }
    public void RC_TO_HMD_Distance_Fnd(float value)
    {
        RC_TO_HMD_Distance_Value = value;
        RC_TO_HMD_Distance.text = RC_TO_HMD_Distance_Value.ToString("00.00");
    }
    public void HMD_Position_Fnd(Vector3 value)
    {
        HMD_Position_Value = value;
        HMD_Position.text = HMD_Position_Value.x.ToString("00.00") + "  " +
                                HMD_Position_Value.y.ToString("00.00") + "  " +
                                HMD_Position_Value.z.ToString("00.00") + "  ";
    }
    public void HMD_Rotation_Fnd(Vector3 value)
    {
        HMD_Rotation_Value = value;
        HMD_Rotation.text = HMD_Rotation_Value.x.ToString("00.00") + "  " +
                                HMD_Rotation_Value.y.ToString("00.00") + "  " +
                                HMD_Rotation_Value.z.ToString("00.00") + "  ";
    }

    private void HMD_VMagnitud_Fnd(float value)
    {
        // not inmplemented
    }

    private void HMD_Velocity_Fnd(float value)
    {
        // not inmplemented
    }

    private void OnEnable()
    {

        UISObserver.LC_RC_Distance += LC_RC_Distance_Fnd; 
        UISObserver.RC_TO_HMD_Distance += RC_TO_HMD_Distance_Fnd; 
        UISObserver.LC_TO_HMD_Distance += LC_TO_HMD_Distance_Fnd; 
        UISObserver.HMD_Position += HMD_Position_Fnd; 
        UISObserver.HMD_Rotation += HMD_Rotation_Fnd; 
        UISObserver.HMD_Velocity += HMD_Velocity_Fnd;
        UISObserver.HMD_VMagnitud += HMD_VMagnitud_Fnd;
    }

    private void OnDisable()
    {
        UISObserver.LC_RC_Distance -= LC_RC_Distance_Fnd;
        UISObserver.RC_TO_HMD_Distance -= RC_TO_HMD_Distance_Fnd;
        UISObserver.LC_TO_HMD_Distance -= LC_TO_HMD_Distance_Fnd;
        UISObserver.HMD_Position -= HMD_Position_Fnd;
        UISObserver.HMD_Rotation -= HMD_Rotation_Fnd;
        UISObserver.HMD_Velocity -= HMD_Velocity_Fnd;
        UISObserver.HMD_VMagnitud -= HMD_VMagnitud_Fnd;
    }
}
