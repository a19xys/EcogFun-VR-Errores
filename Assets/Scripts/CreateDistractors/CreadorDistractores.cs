using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreadorDistractores : MonoBehaviour
{
    public FileManagerDistractors fileManager;
    public DistractorScrollView distractorScrollView;
    public BloqueDistractor bloqueDistractor;
    public BloqueActivador bloqueActivador;
    public BloqueDesactivador bloqueDesactivador;

    public GameObject[] blocks_init;
    public GameObject[] blocks_edit;
    public GameObject[] blocks_visualize;

    public Button botonCrearDistractor;
    public Button botonDeshacer;
    public TextMeshProUGUI cargarSesionButtonText;

    void Start() {
        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = false;
        if (botonDeshacer != null)
            botonDeshacer.interactable = false;
    }

    public void CargarSesion() {
        StartCoroutine(fileManager.CargarSesionCoroutine(this));
    }

    public void OnSessionLoaded(SessionData session) {

        // Deshacer cambios para limpieza visual
        DeshacerCambios();

        distractorScrollView.MostrarDistractores(session.distractors);

        if (cargarSesionButtonText != null && session != null)
            cargarSesionButtonText.text = session.id;

        if (blocks_init != null)
            foreach (GameObject go in blocks_init)
                if (go != null) go.SetActive(false);

        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = false;
        if (botonDeshacer != null)
            botonDeshacer.interactable = false;
    
    }

    public void RevisarBotonCrearDistractor() {
        bool esValido =
            bloqueDistractor != null && bloqueDistractor.EsValido() &&
            bloqueActivador != null && bloqueActivador.EsValido() &&
            bloqueDesactivador != null && bloqueDesactivador.EsValido();

        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = esValido;
    }

    public void RevisarBotonDeshacer() {
        bool hayDatos =
            (bloqueDistractor != null && bloqueDistractor.TieneEleccionReal()) ||
            (bloqueActivador != null && bloqueActivador.TieneEleccionReal()) ||
            (bloqueDesactivador != null && bloqueDesactivador.TieneEleccionReal());

        if (botonDeshacer != null)
            botonDeshacer.interactable = hayDatos;
    }

    public void CrearDistractor() {
        var sessionActual = fileManager.currentSession;
        if (sessionActual == null) return;

        if (!bloqueDistractor.EsValido() || !bloqueActivador.EsValido() || !bloqueDesactivador.EsValido())
            return;

        if (sessionActual.distractors == null)
            sessionActual.distractors = new List<DistractorData>();

        var escena = bloqueDistractor.GetDatosGuardados();
        var activador = bloqueActivador.GetDatosGuardados();
        var desactivador = bloqueDesactivador.GetDatosGuardados();

        var nuevoDistractor = new DistractorData
        {
            id = $"{escena.escena}-{escena.distractor}-{sessionActual.distractors.Count + 1}",
            scene = escena.escena,
            distractorType = escena.distractor,
            activation = ConstruirActivationBlock(activador),
            deactivation = ConstruirDeactivationBlock(desactivador)
        };

        sessionActual.distractors.Add(nuevoDistractor);

        distractorScrollView.MostrarDistractores(sessionActual.distractors);

        // Guardar en JSON
        fileManager.GuardarSesionEnArchivo();

        // Deshacer cambios para limpieza visual
        DeshacerCambios();
    }

    private ActivationBlock ConstruirActivationBlock(ActivadorData datos) {
        var steps = new List<Step>();
        switch (datos.tipo) {
            case "accion":
                steps.Add(new Step {
                    type = "action",
                    @params = new Dictionary<string, string> {
                        { "action", datos.accionElegida },
                        { "item", datos.objetoElegido },
                        { "time", datos.time.ToString() }
                    }
                });
                break;
            case "aciertos":
                steps.Add(new Step {
                    type = "success_count",
                    @params = new Dictionary<string, string> {
                        { "count", datos.aciertos.ToString() },
                        { "time", datos.time.ToString() }
                    }
                });
                break;
            case "tiempo":
                steps.Add(new Step {
                    type = "time",
                    @params = new Dictionary<string, string> {
                        { "time", datos.time.ToString() }
                    }
                });
                break;
            case "efectividad":
                steps.Add(new Step {
                    type = "success_rate",
                    @params = new Dictionary<string, string> {
                        { "rate", datos.rate.ToString() },
                        { "time", datos.time.ToString() }
                    }
                });
                break;
        }
        return new ActivationBlock { steps = steps };
    }

    private ActivationBlock ConstruirDeactivationBlock(DesactivadorData datos) {
        var steps = new List<Step>();
        switch (datos.tipo) {
            case "accion":
                steps.Add(new Step {
                    type = "action",
                    @params = new Dictionary<string, string> {
                        { "action", datos.accionElegida },
                        { "item", datos.objetoElegido },
                        { "time", datos.time.ToString() }
                    }
                });
                break;
            case "tiempo":
                steps.Add(new Step {
                    type = "time",
                    @params = new Dictionary<string, string> {
                        { "time", datos.time.ToString() }
                    }
                });
                break;
            case "ninguno":
            default:
                break;
        }
        return new ActivationBlock { steps = steps };
    }

    public void DeshacerCambios() {
        bloqueDistractor.SetDatosGuardados(null);
        bloqueActivador.SetDatosGuardados(null);
        bloqueDesactivador.SetDatosGuardados(null);

        bloqueDistractor.ResetUI();
        bloqueActivador.ResetUI();
        bloqueDesactivador.ResetUI();

        bloqueActivador.OcultarCamposDeTipos();
        bloqueDesactivador.OcultarCamposDeTipos();

        bloqueDistractor.MostrarVisualizacion();
        bloqueActivador.MostrarVisualizacion();
        bloqueDesactivador.MostrarVisualizacion();

        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = false;
        if (botonDeshacer != null)
            botonDeshacer.interactable = false;
        
        AlternarBloquesVisualizacion(true);
    }

    public void AlternarBloquesVisualizacion(bool visualizar)
    {
        if (blocks_visualize != null)
            foreach (GameObject go in blocks_visualize)
                if (go != null) go.SetActive(visualizar);

        if (blocks_edit != null)
            foreach (GameObject go in blocks_edit)
                if (go != null) go.SetActive(!visualizar);
    }

}