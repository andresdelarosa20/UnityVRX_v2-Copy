using UnityEngine;

// ═══════════════════════════════════════════════════════════════════════════════
// CONTROLADOR RECOLECTABLES — Comportamiento de cada objeto recolectable
// ─────────────────────────────────────────────────────────────────────────────
// Responsabilidades:
//   · Detectar cuando el jugador toca el objeto (OnTriggerEnter).
//   · Manejar efectos visuales: billboard y brillo pulsante.
//   · Notificar al GameManager cuando es recogido.
//     → El GameManager se encarga del sonido y del conteo.
//
// Lo que NO hace este script:
//   · NO llama al ControladorSonidos directamente.
//   · NO conoce al ControladorSonidos.
//   · Solo conoce al GameManager.
//
// Patrón:  Observador — detecta y notifica. El GameManager decide qué hacer.
// Lugar:   Cada GameObject recolectable en la escena.
//
// SETUP EN UNITY:
//   1. Agrega este script al GameObject del objeto recolectable.
//   2. El GameObject necesita un Collider con "Is Trigger" = ✅.
//   3. El jugador debe tener el tag "Player".
// ═══════════════════════════════════════════════════════════════════════════════

public class ControladorRecolectables : MonoBehaviour
{
    // ── Billboard ─────────────────────────────────────────────────────────────

    [Header("─── Billboard ──────────────────────────────────")]
    [Tooltip("El objeto rotará para siempre mirar de frente al jugador.")]
    public bool activarBillboard = true;

    [Tooltip("Solo rota en el eje Y (horizontal). Ideal para objetos 3D.\n" +
             "Desactívalo para sprites o íconos que deben mirar en 3D completo.")]
    public bool soloEjeY = true;

    // ── Brillo pulsante ───────────────────────────────────────────────────────

    [Header("─── Brillo Pulsante ─────────────────────────────")]
    [Tooltip("Activa el efecto de emisión pulsante en el material del objeto.\n" +
             "El material debe tener Emission habilitado (Standard o URP/Lit).")]
    public bool activarBrillo = true;

    [Tooltip("Color del brillo en HDR.")]
    public Color colorBrillo = new Color(1f, 0.85f, 0.2f); // Dorado

    [Tooltip("Intensidad mínima del brillo (HDR: valores > 1 son válidos).")]
    public float brilloMinimo = 0.3f;

    [Tooltip("Intensidad máxima del brillo.")]
    public float brilloMaximo = 2.5f;

    [Tooltip("Velocidad del ciclo de pulso.")]
    public float velocidadPulso = 2f;

    [Tooltip("(Opcional) Renderer del objeto. Si queda vacío se busca automáticamente en hijos.")]
    public Renderer rendererObjeto;

    // ── Sonido de proximidad ──────────────────────────────────────────────────

    [Header("─── Sonido de Proximidad ───────────────────────")]
    [Tooltip("Distancia máxima desde la que el jugador puede oír el sonido de ocio.")]
    public float distanciaMaxSonido = 15f;

    [Tooltip("Distancia mínima a la que el sonido de ocio llega a volumen máximo.")]
    public float distanciaMinSonido = 2f;

    // ── Estado interno ────────────────────────────────────────────────────────
    private Transform jugadorTransform;
    private bool recolectado = false;
    private Material materialInstancia;
    private static readonly int PropEmission = Shader.PropertyToID("_EmissionColor");

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        // Buscar al jugador por tag
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
            jugadorTransform = jugador.transform;
        else
            Debug.LogWarning($"[ControladorRecolectables] '{name}': No se encontró tag 'Player'.");

        ConfigurarBrillo();

        // Registrarse en el ControladorSonidos para el loop de ocio.
        // Aunque el ControladorSonidos no es el sujeto observable, es
        // correcto que el objeto se registre directamente en el gestor de audio
        // para el loop continuo de proximidad (no es un evento puntual).
        if (ControladorSonidos.Instancia != null)
            ControladorSonidos.Instancia.IniciarSonidoOcio(this);
        else
            Debug.LogWarning("[ControladorRecolectables] No se encontró ControladorSonidos.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    void Update()
    {
        if (recolectado) return;

        // 1. Billboard: rotar hacia el jugador
        if (activarBillboard && jugadorTransform != null)
            AplicarBillboard();

        // 2. Brillo pulsante
        if (activarBrillo && materialInstancia != null)
            AplicarBrilloPulsante();

        // 3. Actualizar volumen de ocio por distancia en el ControladorSonidos
        if (jugadorTransform != null && ControladorSonidos.Instancia != null)
        {
            float distancia = Vector3.Distance(transform.position, jugadorTransform.position);
            float volumen   = CalcularVolumen(distancia);
            ControladorSonidos.Instancia.ActualizarVolumenOcio(this, volumen);
        }
    }

    // ── Billboard ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Rota el objeto para que su frente apunte siempre hacia el jugador.
    /// Si soloEjeY está activo, ignora la diferencia de altura.
    /// </summary>
    private void AplicarBillboard()
    {
        Vector3 dir = jugadorTransform.position - transform.position;
        if (soloEjeY) dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    // ── Brillo ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Crea una instancia del material exclusiva para este objeto.
    /// Evita que el brillo de un objeto afecte a los demás que compartan material.
    /// </summary>
    private void ConfigurarBrillo()
    {
        if (!activarBrillo) return;

        if (rendererObjeto == null)
            rendererObjeto = GetComponentInChildren<Renderer>();

        if (rendererObjeto == null)
        {
            Debug.LogWarning($"[ControladorRecolectables] '{name}': No se encontró Renderer.");
            activarBrillo = false;
            return;
        }

        materialInstancia = rendererObjeto.material; // Unity crea instancia propia con .material
        materialInstancia.EnableKeyword("_EMISSION");
        materialInstancia.SetColor(PropEmission, colorBrillo * brilloMinimo);
    }

    /// <summary>
    /// Pulso sinusoidal: oscila la intensidad de emisión entre brilloMinimo y brilloMaximo.
    /// </summary>
    private void AplicarBrilloPulsante()
    {
        float t = (Mathf.Sin(Time.time * velocidadPulso) + 1f) / 2f;
        float intensidad = Mathf.Lerp(brilloMinimo, brilloMaximo, t);
        materialInstancia.SetColor(PropEmission, colorBrillo * intensidad);
    }

    // ── Sonido de proximidad ──────────────────────────────────────────────────

    /// <summary>
    /// Retorna un valor de 0 a 1 basado en la distancia al jugador.
    /// 0 = fuera de rango / 1 = distancia mínima (máximo volumen).
    /// </summary>
    private float CalcularVolumen(float distancia)
    {
        if (distancia >= distanciaMaxSonido) return 0f;
        if (distancia <= distanciaMinSonido) return 1f;
        float t = 1f - ((distancia - distanciaMinSonido) / (distanciaMaxSonido - distanciaMinSonido));
        return Mathf.Lerp(0f, 1f, t);
    }

    // ── Recolección ───────────────────────────────────────────────────────────

    private void OnTriggerEnter(Collider otro)
    {
        if (recolectado || !otro.CompareTag("Player")) return;
        Recolectar();
    }

    /// <summary>
    /// Secuencia de recolección:
    ///   1. Apaga el brillo y oculta el mesh.
    ///   2. Elimina este objeto del sistema de sonido de ocio.
    ///   3. Notifica al GameManager con la posición (para el sonido de recolección).
    ///      → El GameManager ordena el sonido al ControladorSonidos.
    ///   4. Destruye el GameObject tras 2 segundos.
    /// </summary>
    private void Recolectar()
    {
        recolectado = true;

        // Apagar brillo
        if (materialInstancia != null)
            materialInstancia.SetColor(PropEmission, Color.black);

        // Ocultar mesh inmediatamente
        if (rendererObjeto != null)
            rendererObjeto.enabled = false;

        // Desregistrar del loop de ocio
        ControladorSonidos.Instancia?.DetenerSonidoOcio(this);

        // Notificar al GameManager — él decide el sonido y el conteo
        if (GameManager.Instancia != null)
            GameManager.Instancia.NotificarRecoleccion(transform.position);
        else
            Debug.LogError("[ControladorRecolectables] No se encontró GameManager.");

        Destroy(gameObject, 0f);
    }
}
