using System.Collections.Generic;
using UnityEngine;

// ═══════════════════════════════════════════════════════════════════════════════
// CONTROLADOR SONIDOS — Central de audio del juego
// ─────────────────────────────────────────────────────────────────────────────
// Responsabilidades:
//   · Es el único lugar donde viven los AudioSources del juego.
//   · Expone métodos públicos para que GameManager y ControlPuerta
//     pidan sonidos sin conocer los detalles de implementación.
//
// JERARQUÍA EN UNITY:
//   GameObject "Sonidos"                ← este script va aquí
//   ├── Sonido 1  "Ocio"               ← AudioSource, Loop ✅, clip idle del recolectable
//   ├── Sonido 2  "Recoleccion"        ← AudioSource, clip al recoger un objeto
//   ├── Sonido 3  "Puerta"             ← AudioSource, clip al abrir Y cerrar la puerta
//   └── Sonido 4  "Bloqueada"          ← AudioSource, clip al intentar abrir bloqueada
//
// SETUP EN UNITY:
//   1. Crea un GameObject vacío llamado "Sonidos".
//   2. Agrega este script al GameObject "Sonidos".
//   3. Crea 4 hijos con los nombres de arriba.
//   4. Agrega un AudioSource a cada hijo y asigna su AudioClip.
//   5. Arrastra cada hijo a su campo correspondiente en el Inspector.
// ═══════════════════════════════════════════════════════════════════════════════

public class ControladorSonidos : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static ControladorSonidos Instancia { get; private set; }

    // ── Referencias a los hijos ───────────────────────────────────────────────

    [Header("─── Recolectables ──────────────────────────────")]

    [Tooltip("Hijo 'Ocio' — AudioSource con Loop ✅.\n" +
             "Suena mientras el objeto recolectable existe en la escena.\n" +
             "El volumen se ajusta automáticamente según la distancia al jugador.")]
    public AudioSource sonidoOcio;

    [Tooltip("Hijo 'Recoleccion' — AudioSource sin loop.\n" +
             "Se reproduce una vez cuando el jugador recoge un objeto.")]
    public AudioSource sonidoRecoleccion;

    [Header("─── Puertas ────────────────────────────────────")]

    [Tooltip("Hijo 'Puerta' — AudioSource sin loop.\n" +
             "Se reproduce tanto al abrir como al cerrar cualquier puerta.")]
    public AudioSource sonidoPuerta;

    [Tooltip("Hijo 'Bloqueada' — AudioSource sin loop.\n" +
             "Se reproduce cuando el jugador intenta abrir una puerta bloqueada.")]
    public AudioSource sonidoBloqueada;

    // ── Estado interno ────────────────────────────────────────────────────────

    /// <summary>
    /// Registra el volumen de ocio de cada objeto recolectable activo.
    /// Permite que varios objetos compartan el mismo AudioSource de ocio,
    /// usando siempre el volumen del objeto más cercano al jugador.
    /// </summary>
    private Dictionary<ControladorRecolectables, float> volumenesOcio
        = new Dictionary<ControladorRecolectables, float>();

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        ValidarReferencias();
    }

    void Update()
    {
        // Cada frame aplica el volumen más alto registrado al AudioSource de ocio.
        // Así siempre domina el objeto más cercano al jugador.
        ActualizarVolumenOcioGlobal();
    }

    // ── API Recolectables ─────────────────────────────────────────────────────

    /// <summary>
    /// Registra un objeto recolectable en el sistema de ocio y arranca el loop.
    /// Llamado desde ControladorRecolectables.Start().
    /// </summary>
    public void IniciarSonidoOcio(ControladorRecolectables objeto)
    {
        if (!volumenesOcio.ContainsKey(objeto))
            volumenesOcio.Add(objeto, 0f);

        // Arrancar el loop solo si no está ya corriendo
        if (sonidoOcio != null && !sonidoOcio.isPlaying)
        {
            sonidoOcio.loop   = true;
            sonidoOcio.volume = 0f;
            sonidoOcio.Play();
        }
    }

    /// <summary>
    /// Actualiza el volumen de ocio de un objeto específico según su distancia.
    /// Llamado cada frame desde ControladorRecolectables.Update().
    /// </summary>
    public void ActualizarVolumenOcio(ControladorRecolectables objeto, float volumen)
    {
        if (volumenesOcio.ContainsKey(objeto))
            volumenesOcio[objeto] = volumen;
    }

    /// <summary>
    /// Elimina un objeto del registro de ocio cuando es recolectado o destruido.
    /// Si ya no quedan objetos, detiene el AudioSource de ocio.
    /// Llamado desde ControladorRecolectables al recolectar.
    /// </summary>
    public void DetenerSonidoOcio(ControladorRecolectables objeto)
    {
        if (volumenesOcio.ContainsKey(objeto))
            volumenesOcio.Remove(objeto);

        // Si no quedan objetos activos, detener el loop completamente
        if (volumenesOcio.Count == 0 && sonidoOcio != null)
            sonidoOcio.Stop();
    }

    /// <summary>
    /// Reproduce el sonido de recolección en la posición del objeto recogido.
    /// Usa PlayClipAtPoint para que el sonido persista aunque el GameObject
    /// sea destruido inmediatamente después.
    /// Llamado desde ControladorRecolectables.Recolectar().
    /// </summary>
    public void ReproducirRecoleccion(Vector3 posicion)
    {
        if (sonidoRecoleccion != null && sonidoRecoleccion.clip != null)
            AudioSource.PlayClipAtPoint(sonidoRecoleccion.clip, posicion, 1f);
        else
            Debug.LogWarning("[ControladorSonidos] 'Recoleccion' no tiene clip asignado.");
    }

    // ── API Puertas ───────────────────────────────────────────────────────────

    /// <summary>
    /// Reproduce el sonido de puerta (usado tanto para abrir como para cerrar).
    /// Llamado desde ControlPuerta.AbrirPuerta() y ControlPuerta.CerrarPuerta().
    /// </summary>
    public void ReproducirPuerta()
    {
        Reproducir(sonidoPuerta);
    }

    /// <summary>
    /// Reproduce el sonido de puerta bloqueada.
    /// Llamado desde ControlPuerta cuando el jugador apunta y hace click
    /// sin haber recolectado los objetos necesarios.
    /// </summary>
    public void ReproducirBloqueada()
    {
        Reproducir(sonidoBloqueada);
    }

    // ── Lógica interna ────────────────────────────────────────────────────────

    /// <summary>
    /// Recorre el diccionario de volúmenes y aplica el más alto al AudioSource de ocio.
    /// De esta forma, si hay 3 objetos, siempre se escucha el que está más cerca.
    /// </summary>
    private void ActualizarVolumenOcioGlobal()
    {
        if (sonidoOcio == null || volumenesOcio.Count == 0) return;

        float maxVolumen = 0f;
        foreach (var kvp in volumenesOcio)
            if (kvp.Value > maxVolumen)
                maxVolumen = kvp.Value;

        sonidoOcio.volume = maxVolumen;
    }

    /// <summary>
    /// Método genérico para reproducir un AudioSource con PlayOneShot.
    /// PlayOneShot permite que el mismo AudioSource suene varias veces
    /// sin interrumpir reproducciones anteriores.
    /// </summary>
    private void Reproducir(AudioSource fuente)
    {
        if (fuente != null && fuente.clip != null)
            fuente.PlayOneShot(fuente.clip);
        else
            Debug.LogWarning($"[ControladorSonidos] '{fuente?.name}' no tiene clip asignado.");
    }

    // ── Validación en Start ───────────────────────────────────────────────────
    private void ValidarReferencias()
    {
        if (sonidoOcio        == null) Debug.LogWarning("[ControladorSonidos] Falta asignar: Ocio");
        if (sonidoRecoleccion == null) Debug.LogWarning("[ControladorSonidos] Falta asignar: Recoleccion");
        if (sonidoPuerta      == null) Debug.LogWarning("[ControladorSonidos] Falta asignar: Puerta");
        if (sonidoBloqueada   == null) Debug.LogWarning("[ControladorSonidos] Falta asignar: Bloqueada");
    }
}
