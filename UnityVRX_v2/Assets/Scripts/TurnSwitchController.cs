using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TurnSwitchController : MonoBehaviour
{
    public ActionBasedSnapTurnProvider SnapturnProvider;
    public ActionBasedContinuousTurnProvider ContinuousTurnProvider;

    public bool SnapTurnActive = true;

    private void Awake()
    {
        SwitchTurn();   
    }

    [ContextMenu("Ejecutar -> SwitchTurn")]
    public void SwitchTurn()
    {
        if(SnapTurnActive == true)
        {
            SnapturnProvider.turnAmount = 0;
            ContinuousTurnProvider.turnSpeed = 60;

            SnapTurnActive = false;

        } else if (SnapTurnActive == false)
        {
            SnapturnProvider.turnAmount = 15;
            ContinuousTurnProvider.turnSpeed = 0;

            SnapTurnActive = true;
        }
    }
}
