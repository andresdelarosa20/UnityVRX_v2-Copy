using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaControlador : MonoBehaviour
{
    public Animator doorAnimator;
    private bool abierta = false;

    //abrir puerta
    [ContextMenu("Abrir Puerta")]
    public void abrirPuerta()
    {
        print("puerta abierta");
        doorAnimator.SetBool("Abierta", true);
    }

    //cerrar puerta
    [ContextMenu("Cerrar Puerta")]
    public void cerrarPuerta()
    {
        print("puerta cerrada");
        doorAnimator.SetBool("Abierta", false);

    }

    //puerta bloqueada
    //[ContextMenu("Bloquear Puerta")]
    //public void puertaBloqueada()
    //{
    //    print("puerta bloqueada");
    //}

    public void TogglePuerta()
    {
        abierta = !abierta;

        if (abierta)
        {
            cerrarPuerta();
        }
        else
        {
            abrirPuerta();
        }
    }
}
