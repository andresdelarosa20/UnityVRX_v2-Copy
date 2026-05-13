using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton que lleva la cuenta de los objetos recolectados.
/// Soporta múltiples puertas, cada una con su propio requisito de objetos.
/// Coloca este script en un GameObject vacío llamado "GestorRecolectables" en la escena.
/// </summary>
public class GestorRecolectables : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static GestorRecolectables Instancia { get; private set; }

    // ── Estructura por puerta ─────────────────────────────────────────────────
    [System.Serializable]
    public class ConfiguracionPuerta
    {
        [Tooltip("Nombre descriptivo para identificar esta puerta en el Inspector.")]
        public string nombre = "Puerta";

        [Tooltip("Referencia al script ControlPuerta de esta puerta.")]
        public ControlPuerta puerta;

        [Tooltip("Cuántos objetos necesita el jugador recolectar para desbloquear ESTA puerta.")]
        public int objetosNecesarios = 3;

        [Tooltip("¿Ya fue desbloqueada? (solo lectura en runtime).")]
        [HideInInspector] public bool desbloqueada = false;
    }

    // ── Inspector ─────────────────────────────────────────────────────────────
    [Header("Puertas")]
    [Tooltip("Agrega aquí todas las puertas que quieres desbloquear con objetos recolectables.")]
    public List<ConfiguracionPuerta> puertas = new List<ConfiguracionPuerta>();

    [Header("UI (Opcional)")]
    [Tooltip("Texto de UI para mostrar el progreso global de objetos recolectados.")]
    public TMPro.TextMeshProUGUI textoProgreso;

    [Header("Evento personalizado (Opcional)")]
    [Tooltip("Se invoca cada vez que se recoge un objeto. Pasa (recolectados, totalNecesarioMayor).")]
    public UnityEvent<int, int> onObjetoRecolectado;

    // ── Estado interno ────────────────────────────────────────────────────────
    private int objetosRecolectados = 0;

    // El total que se muestra en UI es el máximo de objetos necesarios entre todas las puertas
    private int totalObjetosUI => ObtenerMaximoNecesario();

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    void Start()
    {
        ActualizarUI();

        if (puertas.Count == 0)
            Debug.LogWarning("[GestorRecolectables] No hay puertas configuradas en la lista.");

        foreach (var config in puertas)
        {
            if (config.puerta == null)
                Debug.LogWarning($"[GestorRecolectables] La entrada '{config.nombre}' no tiene puerta asignada.");
        }
    }

    // ── Recolección ───────────────────────────────────────────────────────────
    /// <summary>
    /// Llamado por cada ObjetoRecolectable al ser recogido.
    /// Revisa qué puertas se desbloquean con la cantidad actual.
    /// </summary>
    public void RegistrarRecoleccion()
    {
        objetosRecolectados++;

        Debug.Log($"[GestorRecolectables] Objetos recolectados: {objetosRecolectados}");

        ActualizarUI();
        onObjetoRecolectado?.Invoke(objetosRecolectados, totalObjetosUI);

        // Revisar cada puerta para ver si su requisito se cumple
        foreach (var config in puertas)
        {
            if (!config.desbloqueada && objetosRecolectados >= config.objetosNecesarios)
            {
                config.desbloqueada = true;
                Debug.Log($"[GestorRecolectables] ¡'{config.nombre}' desbloqueada! ({objetosRecolectados}/{config.objetosNecesarios})");
                config.puerta?.AbrirPuerta();
            }
        }
    }

    // ── UI ────────────────────────────────────────────────────────────────────
    private void ActualizarUI()
    {
        if (textoProgreso != null)
            textoProgreso.text = $"Objetos: {objetosRecolectados}/{totalObjetosUI}";
    }

    private int ObtenerMaximoNecesario()
    {
        int max = 0;
        foreach (var config in puertas)
            if (config.objetosNecesarios > max)
                max = config.objetosNecesarios;
        return max;
    }

    // ── Accesores públicos ────────────────────────────────────────────────────
    public int ObjetosRecolectados => objetosRecolectados;
    public bool TodasDesbloqueadas => puertas.TrueForAll(p => p.desbloqueada);
}
