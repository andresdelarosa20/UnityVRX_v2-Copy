using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Controla la puerta para un juego XR.
/// 
/// SETUP REQUERIDO EN UNITY:
/// 1. Agrega este script al GameObject de la puerta.
/// 2. Agrega el componente "XR Simple Interactable" a la puerta.
/// 3. Asegúrate de tener un Collider (no trigger) en la puerta para que el Ray la detecte.
/// 4. En el XR Ray Interactor de tu control, asegúrate de que Interaction Layer Mask incluya "Default".
/// 5. Conecta el evento: XRSimpleInteractable → OnSelectEntered → ControlPuerta.AlternarPuerta()
///    (o déjalo automático, el script lo conecta en Awake).
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class ControlPuerta : MonoBehaviour
{
    // ── Estado y lógica ───────────────────────────────────────────────────────
    [Header("Estado")]
    [Tooltip("Si está activo, la puerta arrancará abierta.")]
    public bool abiertoAlInicio = false;

    // ── Animación manual ──────────────────────────────────────────────────────
    [Header("Animación de Apertura")]
    [Tooltip("Si tienes Animator, arrástralo aquí. Si no, se usará animación manual.")]
    public Animator animadorPuerta;

    [Tooltip("Nombre del parámetro bool en el Animator.")]
    public string parametroAnimator = "Abierta";

    [Tooltip("Activa la animación manual si NO usas Animator.")]
    public bool usarAnimacionManual = true;

    [Tooltip("Posición local de la puerta cuando está ABIERTA.")]
    public Vector3 posicionAbierta = new Vector3(0f, 3f, 0f);

    [Tooltip("Posición local de la puerta cuando está CERRADA.")]
    public Vector3 posicionCerrada = Vector3.zero;

    [Tooltip("Velocidad de la animación manual.")]
    public float velocidadAnimacion = 2f;

    // ── Audio ─────────────────────────────────────────────────────────────────
    [Header("Audio")]
    public AudioSource audioSourcePuerta;
    public AudioClip sonidoAbrir;
    public AudioClip sonidoCerrar;
    public AudioClip sonidoBloqueada;

    // ── Feedback visual del Ray ───────────────────────────────────────────────
    [Header("Feedback Visual (Hover)")]
    [Tooltip("Material que se aplica cuando el ray apunta a la puerta.")]
    public Material materialHighlight;

    [Tooltip("Material original de la puerta (se restaura al salir del hover).")]
    public Material materialOriginal;

    [Tooltip("Renderer de la puerta para el highlight.")]
    public Renderer rendererPuerta;

    // ── Estado interno ────────────────────────────────────────────────────────
    private bool estaAbierta = false;
    private bool puedoAbrir = false;        // true cuando se recolectan todos los objetos
    private Coroutine corutinaAnimacion;
    private XRSimpleInteractable interactable;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        // Obtener y conectar el XRSimpleInteractable automáticamente
        interactable = GetComponent<XRSimpleInteractable>();

        // SelectEntered = cuando el jugador hace click/trigger apuntando a la puerta
        interactable.selectEntered.AddListener(OnRayClick);

        // HoverEntered / Exited = cuando el ray pasa por encima
        interactable.hoverEntered.AddListener(OnRayEnter);
        interactable.hoverExited.AddListener(OnRayExit);
    }

    void Start()
    {
        estaAbierta = abiertoAlInicio;

        if (rendererPuerta == null)
            rendererPuerta = GetComponentInChildren<Renderer>();

        // Guardar material original si no fue asignado
        if (materialOriginal == null && rendererPuerta != null)
            materialOriginal = rendererPuerta.material;

        // Aplicar estado inicial
        if (estaAbierta)
            AbrirPuerta();
        else
            PuertaBloqueada();
    }

    void OnDestroy()
    {
        // Siempre limpiar listeners para evitar memory leaks
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnRayClick);
            interactable.hoverEntered.RemoveListener(OnRayEnter);
            interactable.hoverExited.RemoveListener(OnRayExit);
        }
    }

    // ── Eventos XR ────────────────────────────────────────────────────────────

    /// <summary>
    /// Se llama cuando el jugador apunta con el ray y presiona el trigger/click.
    /// </summary>
    private void OnRayClick(SelectEnterEventArgs args)
    {
        if (!puedoAbrir)
        {
            // Feedback: puerta bloqueada
            ReproducirSonido(sonidoBloqueada);
            Debug.Log("[ControlPuerta] Puerta bloqueada. Recoge todos los objetos primero.");
            return;
        }

        AlternarPuerta();
    }

    /// <summary>
    /// Feedback visual cuando el ray entra en hover sobre la puerta.
    /// </summary>
    private void OnRayEnter(HoverEnterEventArgs args)
    {
        if (materialHighlight != null && rendererPuerta != null)
            rendererPuerta.material = materialHighlight;
    }

    /// <summary>
    /// Restaura el material original cuando el ray deja de apuntar.
    /// </summary>
    private void OnRayExit(HoverExitEventArgs args)
    {
        if (materialOriginal != null && rendererPuerta != null)
            rendererPuerta.material = materialOriginal;
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>
    /// Alterna entre abrir y cerrar la puerta.
    /// </summary>
    public void AlternarPuerta()
    {
        if (estaAbierta)
            CerrarPuerta();
        else
            AbrirPuerta();
    }

    /// <summary>
    /// La puerta está bloqueada porque el jugador no tiene todos los objetos.
    /// Llamado por GestorRecolectables mientras falten objetos.
    /// </summary>
    public void PuertaBloqueada()
    {
        puedoAbrir = false;
        estaAbierta = false;
        Debug.Log("[ControlPuerta] Estado: BLOQUEADA ");
        //ReproducirSonido(sonidoBloqueada);

        if (animadorPuerta != null)
            animadorPuerta.SetBool(parametroAnimator, false);
        else if (usarAnimacionManual)
        {
            if (corutinaAnimacion != null) StopCoroutine(corutinaAnimacion);
            corutinaAnimacion = StartCoroutine(MoverPuerta(posicionCerrada));
        }
    }

    /// <summary>
    /// Desbloquea y abre la puerta. Llamado por GestorRecolectables al completar colección.
    /// </summary>
    public void AbrirPuerta()
    {
        puedoAbrir = true;

        if (estaAbierta) return;
        estaAbierta = true;

        Debug.Log("[ControlPuerta] ¡Abriendo puerta! 🚪✅");
        ReproducirSonido(sonidoAbrir);

        if (animadorPuerta != null)
        {
            animadorPuerta.SetBool(parametroAnimator, true);
        }
        else if (usarAnimacionManual)
        {
            if (corutinaAnimacion != null) StopCoroutine(corutinaAnimacion);
            corutinaAnimacion = StartCoroutine(MoverPuerta(posicionAbierta));
        }
    }

    /// <summary>
    /// Cierra la puerta (solo si ya estaba desbloqueada).
    /// </summary>
    public void CerrarPuerta()
    {
        if (!puedoAbrir || !estaAbierta) return;
        estaAbierta = false;

        Debug.Log("[ControlPuerta] Cerrando puerta... 🚪");
        ReproducirSonido(sonidoCerrar);

        if (animadorPuerta != null)
        {
            animadorPuerta.SetBool(parametroAnimator, false);
        }
        else if (usarAnimacionManual)
        {
            if (corutinaAnimacion != null) StopCoroutine(corutinaAnimacion);
            corutinaAnimacion = StartCoroutine(MoverPuerta(posicionCerrada));
        }
    }

    // ── Animación manual ──────────────────────────────────────────────────────
    private IEnumerator MoverPuerta(Vector3 destino)
    {
        while (Vector3.Distance(transform.localPosition, destino) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                destino,
                velocidadAnimacion * Time.deltaTime
            );
            yield return null;
        }
        transform.localPosition = destino;
    }

    // ── Audio ─────────────────────────────────────────────────────────────────
    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSourcePuerta != null && clip != null)
            audioSourcePuerta.PlayOneShot(clip);
    }

    // ── Accesores ─────────────────────────────────────────────────────────────
    public bool EstaAbierta => estaAbierta;
    public bool PuedoAbrir => puedoAbrir;
}
