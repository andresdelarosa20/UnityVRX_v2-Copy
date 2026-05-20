using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// ═══════════════════════════════════════════════════════════════════════════════
// CONTROL PUERTA — Comportamiento individual de cada puerta
// ─────────────────────────────────────────────────────────────────────────────
// Responsabilidades:
//   · Reaccionar a la interacción XR (ray + trigger del control).
//   · Ejecutar la animación de apertura y cierre via Animator.
//   · Notificar al GameManager los eventos de sonido.
//     → El GameManager ordena al ControladorSonidos qué reproducir.
//
// Lo que NO hace este script:
//   · NO llama al ControladorSonidos directamente.
//   · NO conoce al ControladorSonidos.
//   · Solo conoce al GameManager.
//
// Patrón:  Observador — es notificado por GameManager (Desbloquear) y
//          a su vez notifica al GameManager los eventos de interacción.
//
// SETUP EN UNITY:
//   1. Agrega este script al GameObject de la puerta.
//   2. Agrega "XR Simple Interactable" al mismo GameObject.
//   3. El GameObject necesita un Collider (Is Trigger = ❌).
//   4. Arrastra el Animator de la puerta al campo "Animator Puerta".
//   5. Arrastra este script al campo "Puerta" en el GameManager.
// ═══════════════════════════════════════════════════════════════════════════════

[RequireComponent(typeof(XRSimpleInteractable))]
public class ControlPuerta : MonoBehaviour
{
    // ── Animación ─────────────────────────────────────────────────────────────

    [Header("─── Animación ──────────────────────────────────")]
    [Tooltip("Animator de la puerta. Arrastra aquí el componente Animator.")]
    public Animator animadorPuerta;

    [Tooltip("Nombre exacto del parámetro bool en el Animator.\n" +
             "Debe coincidir letra a letra con el Animator Controller.")]
    public string parametroAnimator = "Abierta";

    // ── Feedback visual (Hover del Ray) ───────────────────────────────────────

    [Header("─── Feedback Visual ─────────────────────────────")]
    [Tooltip("Material que se aplica cuando el ray XR apunta a la puerta.\n" +
             "Deja vacío si no quieres feedback visual.")]
    public Material materialHighlight;

    [Tooltip("Material original. Se restaura al salir del hover.\n" +
             "Si queda vacío, se captura automáticamente en Start().")]
    public Material materialOriginal;

    [Tooltip("Renderer de la puerta. Si queda vacío, se busca automáticamente.")]
    public Renderer rendererPuerta;

    // ── Estado interno ────────────────────────────────────────────────────────
    private bool estaAbierta  = false;
    private bool desbloqueada = false;
    private XRSimpleInteractable interactable;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnRayClick);
        interactable.hoverEntered.AddListener(OnRayEnter);
        interactable.hoverExited.AddListener(OnRayExit);
    }

    void Start()
    {
        if (rendererPuerta == null)
            rendererPuerta = GetComponentInChildren<Renderer>();

        if (materialOriginal == null && rendererPuerta != null)
            materialOriginal = rendererPuerta.material;

        if (animadorPuerta == null)
            Debug.LogWarning($"[ControlPuerta] '{name}': No tiene Animator asignado.");
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnRayClick);
            interactable.hoverEntered.RemoveListener(OnRayEnter);
            interactable.hoverExited.RemoveListener(OnRayExit);
        }
    }

    // ── Eventos XR ────────────────────────────────────────────────────────────

    /// <summary>
    /// El jugador apunta con el ray y presiona el trigger del control.
    /// Si está bloqueada: notifica al GameManager → él ordena el sonido.
    /// Si está desbloqueada: alterna la puerta → notifica al GameManager → él ordena el sonido.
    /// </summary>
    private void OnRayClick(SelectEnterEventArgs args)
    {
        if (!desbloqueada)
        {
            GameManager.Instancia?.NotificarIntentoPuertaBloqueada();
            Debug.Log($"[ControlPuerta] '{name}': Bloqueada.");
            return;
        }

        AlternarPuerta();
    }

    /// <summary>Aplica el material highlight cuando el ray entra en hover.</summary>
    private void OnRayEnter(HoverEnterEventArgs args)
    {
        if (materialHighlight != null && rendererPuerta != null)
            rendererPuerta.material = materialHighlight;
    }

    /// <summary>Restaura el material original cuando el ray sale del hover.</summary>
    private void OnRayExit(HoverExitEventArgs args)
    {
        if (materialOriginal != null && rendererPuerta != null)
            rendererPuerta.material = materialOriginal;
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>
    /// Llamado por el GameManager cuando el jugador completó los objetos necesarios.
    /// Marca la puerta como desbloqueada y la abre automáticamente.
    /// </summary>
    public void Desbloquear()
    {
        desbloqueada = true;
        Debug.Log($"[ControlPuerta] '{name}': ¡Desbloqueada!");
        AbrirPuerta();
    }

    /// <summary>Alterna entre abrir y cerrar. Solo funciona si está desbloqueada.</summary>
    public void AlternarPuerta()
    {
        if (estaAbierta) CerrarPuerta();
        else             AbrirPuerta();
    }

    /// <summary>
    /// Abre la puerta: notifica al GameManager y ejecuta la animación via Animator.
    /// </summary>
    public void AbrirPuerta()
    {
        if (estaAbierta) return;
        estaAbierta = true;

        Debug.Log($"[ControlPuerta] '{name}': Abriendo...");
        GameManager.Instancia?.NotificarPuertaAbierta();
        animadorPuerta?.SetBool(parametroAnimator, true);
    }

    /// <summary>
    /// Cierra la puerta: notifica al GameManager y ejecuta la animación via Animator.
    /// </summary>
    public void CerrarPuerta()
    {
        if (!estaAbierta || !desbloqueada) return;
        estaAbierta = false;

        Debug.Log($"[ControlPuerta] '{name}': Cerrando...");
        GameManager.Instancia?.NotificarPuertaCerrada();
        animadorPuerta?.SetBool(parametroAnimator, false);
    }

    // ── Accesores ─────────────────────────────────────────────────────────────
    public bool EstaAbierta  => estaAbierta;
    public bool Desbloqueada => desbloqueada;
}
