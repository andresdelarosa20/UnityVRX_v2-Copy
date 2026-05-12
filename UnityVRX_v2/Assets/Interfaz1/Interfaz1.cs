using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Interfaz1 : MonoBehaviour
{
    public TextMeshProUGUI Informacion;
    public TextMeshProUGUI Informacion2;
    public TextMeshProUGUI Informacion3;
    public TextMeshProUGUI Informacion4;

    [ContextMenu("Cargar Informacion")]
    public void CargarInformacion(string __mensaje)
    {
        Informacion.text = __mensaje;
    }
    public void CargarInformacion2(string __mensaje)
    {
        Informacion2.text = __mensaje;
    }
    public void CargarInformacion3(string __mensaje)
    {
        Informacion3.text = __mensaje;
    }
    public void CargarInformacion4(string __mensaje)
    {
        Informacion4.text = __mensaje;
    }

    private void OnEnable()
    {
        Eventos.MensajeInterfaz1 += CargarInformacion;
        Eventos.MensajeInterfaz2 += CargarInformacion2;
        Eventos.MensajeInterfaz3 += CargarInformacion3;
        Eventos.MensajeInterfaz4 += CargarInformacion4;
    }

    private void OnDisable()
    {
        Eventos.MensajeInterfaz1 -= CargarInformacion;
        Eventos.MensajeInterfaz2 -= CargarInformacion2;
        Eventos.MensajeInterfaz3 -= CargarInformacion3;
        Eventos.MensajeInterfaz4 -= CargarInformacion4;
    }
}

static class Eventos
{
    public static Action <string>MensajeInterfaz1;
    public static Action <string>MensajeInterfaz2;
    public static Action <string>MensajeInterfaz3;
    public static Action <string>MensajeInterfaz4;
}