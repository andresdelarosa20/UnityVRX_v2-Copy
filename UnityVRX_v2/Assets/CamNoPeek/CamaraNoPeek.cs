using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraNoPeek : MonoBehaviour
{
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float fadeSpeed;
    [SerializeField] float sphereCheckSize = 0.15f;

    [SerializeField] private Material CamaraSuperposicion_Material;
    private bool isCameraFadedOut = false;

    void Update()
    {
        if (Physics.CheckSphere(transform.position, sphereCheckSize, collisionLayer, QueryTriggerInteraction.Ignore))
        {
            CameraFade(1f);
            isCameraFadedOut = true;
        }
        else
        {
            if (!isCameraFadedOut)
                return;

            CameraFade(0f);
        }
    }

    public void CameraFade(float targetAlpha)
    {
        var fadeValue = Mathf.MoveTowards(CamaraSuperposicion_Material.GetFloat("_Float_Alpha_Value"), targetAlpha, Time.deltaTime * fadeSpeed);
        CamaraSuperposicion_Material.SetFloat("_Float_Alpha_Value", fadeValue);
        //print(CamaraSuperposicion_Material.GetFloat("_Float_Alpha_Value"));

        if (fadeValue <= 0.01f)
            isCameraFadedOut = false;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f,1f,0f,075f);
        Gizmos.DrawSphere(transform.position, sphereCheckSize);
    }

}
