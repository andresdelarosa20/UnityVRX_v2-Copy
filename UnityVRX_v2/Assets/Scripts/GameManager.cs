using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ═══════════════════════════════════════════════════════════════════════════════
// GAMEMANAGER — Sujeto Observable Central
// ─────────────────────────────────────────────────────────────────────────────
// Responsabilidades:
//   · Es el único punto de verdad del estado del juego.
//   · Lleva el conteo de objetos recolectados.
//   · Es el ÚNICO que habla con el ControladorSonidos.
//     Ningún otro script llama sonidos directamente.
//   · Notifica a todos los observadores cuando algo cambia:
//       - Se recogió un objeto        → ordena sonido + notifica UI
//       - Una puerta se desbloqueó    → ordena sonido + notifica puerta
//       - Se completó la colección    → notifica fin de juego
//       - Jugador intenta abrir puer. → ordena sonido bloqueada
//
// Patrón:  Singleton + Observer
// Lugar:   GameObject vacío llamado "GameManager" en la raíz de la Hierarchy.
// ═══════════════════════════════════════════════════════════════════════════════

public class GameManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static GameManager Instancia { get; private set; }

    // ── Configuración de puertas ──────────────────────────────────────────────

    /// <summary>
    /// Datos de cada puerta: nombre, referencia al script y objetos necesarios.
    /// Se configura desde el Inspector.
    /// </summary>
    [System.Serializable]
    public class ConfigPuerta
    {
        [Tooltip("Nombre descriptivo para identificar esta puerta en el Inspector.")]
        public string nombre = "Puerta";

        [Tooltip("Referencia al script ControlPuerta del GameObject de la puerta.")]
        public ControlPuerta puerta;

        [Tooltip("Cantidad de objetos necesarios para desbloquear esta puerta.")]
        public int objetosNecesarios = 3;

        /// <summary>Marca interna: true cuando esta puerta ya fue desbloqueada.</summary>
        [HideInInspector] public bool desbloqueada = false;
    }

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("─── Puertas ───────────────────────────────────")]
    [Tooltip("Lista de todas las puertas de la escena con sus requisitos de objetos.")]
    public List<ConfigPuerta> puertas = new List<ConfigPuerta>();

    [Header("─── UI ─────────────────────────────────────────")]
    [Tooltip("(Opcional) TextMeshPro para mostrar el progreso de objetos recolectados.")]
    public TMPro.TextMeshProUGUI textoProgreso;

    // ── Eventos (Observer visual desde el Inspector) ──────────────────────────

    [Header("─── Eventos ────────────────────────────────────")]

    /// <summary>
    /// Se dispara cada vez que el jugador recoge un objeto.
    /// Parámetros: (int recolectados, int totalMaximo).
    /// Úsalo para actualizar UI adicional, partículas, etc.
    /// </summary>
    [Tooltip("Se invoca al recoger un objeto. Recibe (recolectados, totalMaximo).")]
    public UnityEvent<int, int> EventoObjetoRecogido;

    /// <summary>
    /// Se dispara cuando una puerta específica es desbloqueada.
    /// Parámetro: (string nombrePuerta).
    /// </summary>
    [Tooltip("Se invoca cuando una puerta se desbloquea. Recibe el nombre de la puerta.")]
    public UnityEvent<string> EventoPuertaDesbloqueada;

    /// <summary>
    /// Se dispara cuando se recogen TODOS los objetos (colección global completa).
    /// </summary>
    [Tooltip("Se invoca cuando se completa la colección global.")]
    public UnityEvent EventoColeccionCompleta;

    // ── Estado interno ────────────────────────────────────────────────────────
    private int objetosRecolectados = 0;

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
        ValidarConfiguracion();
        ActualizarUI();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // API PÚBLICA — Los observadores llaman estos métodos para notificar eventos
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Llamado por ControladorRecolectables cuando el jugador recoge un objeto.
    /// El GameManager se encarga de todo lo que sigue: sonido, UI, puertas.
    /// </summary>
    public void NotificarRecoleccion(Vector3 posicionObjeto)
    {
        objetosRecolectados++;
        Debug.Log($"[GameManager] Objeto recogido. Total: {objetosRecolectados}/{TotalMaximo}");

        // 1. Ordenar sonido de recolección al ControladorSonidos
        ControladorSonidos.Instancia?.ReproducirRecoleccion(posicionObjeto);

        // 2. Actualizar UI
        ActualizarUI();

        // 3. Disparar evento para observadores externos (UI extra, efectos, etc.)
        EventoObjetoRecogido?.Invoke(objetosRecolectados, TotalMaximo);

        // 4. Revisar qué puertas se desbloquean con el conteo actual
        RevisarPuertas();

        // 5. Verificar si se completó la colección global
        if (objetosRecolectados >= TotalMaximo)
        {
            Debug.Log("[GameManager] ¡Colección completa!");
            EventoColeccionCompleta?.Invoke();
        }
    }

    /// <summary>
    /// Llamado por ControlPuerta cuando el jugador intenta abrir
    /// una puerta que aún está bloqueada.
    /// El GameManager ordena al ControladorSonidos el sonido correspondiente.
    /// </summary>
    public void NotificarIntentoPuertaBloqueada()
    {
        Debug.Log("[GameManager] Jugador intentó abrir una puerta bloqueada.");
        ControladorSonidos.Instancia?.ReproducirBloqueada();
    }

    /// <summary>
    /// Llamado por ControlPuerta cuando la puerta es abierta por el jugador
    /// (después de estar desbloqueada).
    /// </summary>
    public void NotificarPuertaAbierta()
    {
        ControladorSonidos.Instancia?.ReproducirPuerta();
    }

    /// <summary>
    /// Llamado por ControlPuerta cuando la puerta es cerrada por el jugador.
    /// </summary>
    public void NotificarPuertaCerrada()
    {
        ControladorSonidos.Instancia?.ReproducirPuerta();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LÓGICA INTERNA
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Recorre todas las puertas. Si el conteo actual cumple el requisito
    /// de alguna y aún no fue desbloqueada, la desbloquea y notifica.
    /// </summary>
    private void RevisarPuertas()
    {
        foreach (var config in puertas)
        {
            if (config.puerta == null || config.desbloqueada) continue;

            if (objetosRecolectados >= config.objetosNecesarios)
            {
                config.desbloqueada = true;
                Debug.Log($"[GameManager] Puerta '{config.nombre}' desbloqueada.");

                // Notificar a la puerta que puede abrirse
                config.puerta.Desbloquear();

                // Disparar evento para observadores externos
                EventoPuertaDesbloqueada?.Invoke(config.nombre);
            }
        }
    }

    private void ActualizarUI()
    {
        if (textoProgreso != null)
            textoProgreso.text = $"Objetos: {objetosRecolectados}/{TotalMaximo}";
    }

    private void ValidarConfiguracion()
    {
        if (puertas.Count == 0)
            Debug.LogWarning("[GameManager] No hay puertas configuradas en la lista.");

        foreach (var config in puertas)
            if (config.puerta == null)
                Debug.LogWarning($"[GameManager] La puerta '{config.nombre}' no tiene ControlPuerta asignado.");

        if (ControladorSonidos.Instancia == null)
            Debug.LogWarning("[GameManager] No se encontró ControladorSonidos en la escena.");
    }

    // ── Accesores públicos ────────────────────────────────────────────────────

    /// <summary>Cantidad de objetos recolectados hasta ahora.</summary>
    public int ObjetosRecolectados => objetosRecolectados;

    /// <summary>El mayor número de objetos necesario entre todas las puertas.</summary>
    public int TotalMaximo
    {
        get
        {
            int max = 0;
            foreach (var c in puertas)
                if (c.objetosNecesarios > max) max = c.objetosNecesarios;
            return max;
        }
    }

    /// <summary>True si todas las puertas fueron desbloqueadas.</summary>
    public bool TodasDesbloqueadas => puertas.TrueForAll(p => p.desbloqueada);
}
