using UnityEngine;

public class EmissionPulse : MonoBehaviour
{
    public Renderer objectRenderer;

    public Color emissionColor = Color.cyan;

    public float pulseSpeed = 2f;
    public float minIntensity = 0f;
    public float maxIntensity = 5f;

    private Material material;

    void Start()
    {
        material = objectRenderer.material;

        // Activa emission
        material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float emission = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f
        );

        Color finalColor = emissionColor * emission;

        material.SetColor("_EmissionColor", finalColor);
    }
}