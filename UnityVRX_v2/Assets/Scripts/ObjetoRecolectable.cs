using UnityEngine;

/// <summary>
/// Adjunta este script a cada objeto recolectable de la escena.
/// Requiere: AudioSource (x2), Collider con Is Trigger = true, y un tag "Player" en el Character Controller.
/// El objeto siempre mirará de frente al jugador (billboard) y emitirá un brillo pulsante.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ObjetoRecolectable : MonoBehaviour
{
    // ── Audio ─────────────────────────────────────────────────────────────────
    [Header("Referencias de Audio")]
    [Tooltip("AudioSource para el sonido de ocio (idle). Configúralo en el Inspector.")]
    public AudioSource audioSourceOcio;

    [Tooltip("AudioSource para el sonido de recolección. Configúralo en el Inspector.")]
    public AudioSource audioSourceRecoleccion;

    [Header("Sonido de Ocio - Proximidad")]
    [Tooltip("Distancia máxima desde la cual el jugador comienza a escuchar el sonido de ocio.")]
    public float distanciaMaxima = 15f;

    [Tooltip("Distancia mínima a la que el volumen del sonido de ocio llega al máximo.")]
    public float distanciaMinima = 2f;

    [Tooltip("Volumen máximo del sonido de ocio cuando el jugador está muy cerca.")]
    [Range(0f, 1f)]
    public float volumenMaxOcio = 1f;

    // ── Billboard ─────────────────────────────────────────────────────────────
    [Header("Billboard - Mirar al jugador")]
    [Tooltip("Si está activo, el objeto siempre rotará para mirar de frente al jugador.")]
    public bool activarBillboard = true;

    [Tooltip("Solo rota en el eje Y (horizontal), ideal para objetos 3D que no deben inclinarse.")]
    public bool soloEjeY = true;

    // ── Brillo ────────────────────────────────────────────────────────────────
    [Header("Brillo Pulsante (Emission)")]
    [Tooltip("Activa el efecto de brillo pulsante en el material del objeto.")]
    public bool activarBrillo = true;

    [Tooltip("Color del brillo. Asegúrate de que el material use shader Standard o URP/Lit con Emission activado.")]
    public Color colorBrillo = new Color(1f, 0.85f, 0.2f); // Dorado por defecto

    [Tooltip("Intensidad mínima del brillo (en HDR, valores > 1 son válidos).")]
    public float brilloMinimo = 0.3f;

    [Tooltip("Intensidad máxima del brillo.")]
    public float brilloMaximo = 2.5f;

    [Tooltip("Velocidad del pulso del brillo.")]
    public float velocidadPulso = 2f;

    [Tooltip("Renderer del mesh del objeto. Si está vacío, se busca automáticamente en hijos.")]
    public Renderer rendererObjeto;

    // ── Estado interno ────────────────────────────────────────────────────────
    private Transform jugadorTransform;
    private bool recolectado = false;
    private Material materialInstancia;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        // Buscar al jugador por tag
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
            jugadorTransform = jugador.transform;
        else
            Debug.LogWarning("[ObjetoRecolectable] No se encontró un GameObject con tag 'Player'.");

        // Iniciar sonido de ocio en loop
        if (audioSourceOcio != null)
        {
            audioSourceOcio.loop = true;
            audioSourceOcio.volume = 0f;
            audioSourceOcio.spatialBlend = 0f;
            audioSourceOcio.Play();
        }
        else
        {
            Debug.LogWarning($"[ObjetoRecolectable] '{gameObject.name}': No tiene asignado audioSourceOcio.");
        }

        // Preparar material para el brillo
        ConfigurarBrillo();
    }

    // ─────────────────────────────────────────────────────────────────────────
    void Update()
    {
        if (recolectado) return;

        // 1. Billboard: el objeto mira de frente al jugador
        if (activarBillboard && jugadorTransform != null)
            AplicarBillboard();

        // 2. Volumen del sonido de ocio según distancia
        if (jugadorTransform != null && audioSourceOcio != null)
        {
            float distancia = Vector3.Distance(transform.position, jugadorTransform.position);
            audioSourceOcio.volume = CalcularVolumenPorDistancia(distancia);
        }

        // 3. Brillo pulsante
        if (activarBrillo && materialInstancia != null)
            AplicarBrilloPulsante();
    }

    // ── Billboard ─────────────────────────────────────────────────────────────
    /// <summary>
    /// Rota el objeto para que su frente apunte siempre hacia el jugador.
    /// </summary>
    private void AplicarBillboard()
    {
        Vector3 direccion = jugadorTransform.position - transform.position;

        if (soloEjeY)
        {
            // Ignora diferencia de altura — solo gira horizontalmente
            direccion.y = 0f;
        }

        if (direccion.sqrMagnitude < 0.001f) return; // evitar NaN si están solapados

        transform.rotation = Quaternion.LookRotation(direccion);
    }

    // ── Brillo ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Crea una instancia del material para modificar la emisión de forma independiente
    /// sin afectar a los demás objetos que compartan el mismo material.
    /// </summary>
    private void ConfigurarBrillo()
    {
        if (!activarBrillo) return;

        // Buscar Renderer automáticamente si no fue asignado
        if (rendererObjeto == null)
            rendererObjeto = GetComponentInChildren<Renderer>();

        if (rendererObjeto == null)
        {
            Debug.LogWarning($"[ObjetoRecolectable] '{gameObject.name}': No se encontró Renderer para el brillo.");
            activarBrillo = false;
            return;
        }

        // .material crea automáticamente una instancia propia del material
        materialInstancia = rendererObjeto.material;

        // Activar la keyword de emisión (necesario para Standard y URP/Lit)
        materialInstancia.EnableKeyword("_EMISSION");

        // Valor inicial del brillo
        materialInstancia.SetColor(EmissionColor, colorBrillo * brilloMinimo);
    }

    /// <summary>
    /// Actualiza el color de emisión del material con un pulso sinusoidal suave.
    /// </summary>
    private void AplicarBrilloPulsante()
    {
        // Sin(t) oscila entre -1 y 1 → lo normalizamos a 0..1
        float t = (Mathf.Sin(Time.time * velocidadPulso) + 1f) / 2f;
        float intensidad = Mathf.Lerp(brilloMinimo, brilloMaximo, t);

        materialInstancia.SetColor(EmissionColor, colorBrillo * intensidad);
    }

    // ── Audio por distancia ───────────────────────────────────────────────────
    private float CalcularVolumenPorDistancia(float distancia)
    {
        if (distancia >= distanciaMaxima) return 0f;
        if (distancia <= distanciaMinima) return volumenMaxOcio;
        float t = 1f - ((distancia - distanciaMinima) / (distanciaMaxima - distanciaMinima));
        return Mathf.Lerp(0f, volumenMaxOcio, t);
    }

    // ── Recolección ───────────────────────────────────────────────────────────
    private void OnTriggerEnter(Collider otro)
    {
        if (recolectado) return;
        if (otro.CompareTag("Player"))
            Recolectar();
    }

    private void Recolectar()
    {
        recolectado = true;

        if (audioSourceOcio != null)
            audioSourceOcio.Stop();

        if (audioSourceRecoleccion != null && audioSourceRecoleccion.clip != null)
            AudioSource.PlayClipAtPoint(audioSourceRecoleccion.clip, transform.position, 1f);

        if (GestorRecolectables.Instancia != null)
            GestorRecolectables.Instancia.RegistrarRecoleccion();
        else
            Debug.LogError("[ObjetoRecolectable] No se encontró GestorRecolectables en la escena.");

        // Apagar brillo antes de ocultar el objeto
        if (materialInstancia != null)
            materialInstancia.SetColor(EmissionColor, Color.black);

        if (rendererObjeto != null)
            rendererObjeto.enabled = false;

        Destroy(gameObject, 0f);
    }
}
