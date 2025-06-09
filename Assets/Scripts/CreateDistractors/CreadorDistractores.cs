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

    public TextMeshProUGUI cargarSesionButtonText;
    public GameObject[] blocks_init;
    public Button botonCrearDistractor;
    public Button botonDeshacer;

    void Start()
    {
        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = false;
        if (botonDeshacer != null)
            botonDeshacer.interactable = false;
    }

    public void CargarSesion()
    {
        StartCoroutine(fileManager.CargarSesionCoroutine(this));
    }

    public void OnSessionLoaded(SessionData session)
    {
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

    public void RevisarBotonCrearDistractor()
    {
        bool esValido =
            bloqueDistractor != null && bloqueDistractor.EsValido() &&
            bloqueActivador != null && bloqueActivador.EsValido() &&
            bloqueDesactivador != null && bloqueDesactivador.EsValido();

        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = esValido;
    }

    public void RevisarBotonDeshacer()
    {
        bool hayDatos =
            (bloqueDistractor != null && bloqueDistractor.TieneEleccionReal()) ||
            (bloqueActivador != null && bloqueActivador.TieneEleccionReal()) ||
            (bloqueDesactivador != null && bloqueDesactivador.TieneEleccionReal());

        if (botonDeshacer != null)
            botonDeshacer.interactable = hayDatos;
    }

    public void CrearDistractor()
    {
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

    private ActivationBlock ConstruirActivationBlock(ActivadorData datos)
    {
        var steps = new List<Step>();
        switch (datos.tipo)
        {
            case "accion":
                steps.Add(new Step
                {
                    type = "action",
                    @params = new Dictionary<string, object> {
                        { "action", datos.accionElegida },
                        { "item", datos.objetoElegido },
                        { "time", datos.time }
                    }
                });
                break;
            case "aciertos":
                steps.Add(new Step
                {
                    type = "success_count",
                    @params = new Dictionary<string, object> {
                        { "count", datos.aciertos },
                        { "time", datos.time }
                    }
                });
                break;
            case "tiempo":
                steps.Add(new Step
                {
                    type = "time",
                    @params = new Dictionary<string, object> {
                        { "time", datos.time }
                    }
                });
                break;
            case "efectividad":
                steps.Add(new Step
                {
                    type = "success_rate",
                    @params = new Dictionary<string, object> {
                        { "rate", datos.rate },
                        { "time", datos.time }
                    }
                });
                break;
        }
        return new ActivationBlock { steps = steps };
    }

    private ActivationBlock ConstruirDeactivationBlock(DesactivadorData datos)
    {
        var steps = new List<Step>();
        switch (datos.tipo)
        {
            case "accion":
                steps.Add(new Step
                {
                    type = "action",
                    @params = new Dictionary<string, object> {
                        { "action", datos.accionElegida },
                        { "item", datos.objetoElegido },
                        { "time", datos.time }
                    }
                });
                break;
            case "tiempo":
                steps.Add(new Step
                {
                    type = "time",
                    @params = new Dictionary<string, object> {
                        { "time", datos.time }
                    }
                });
                break;
            case "ninguno":
                break;
        }
        return new ActivationBlock { steps = steps };
    }

    public void DeshacerCambios()
    {
        bloqueDistractor.SetDatosGuardados(null);
        bloqueActivador.SetDatosGuardados(null);
        bloqueDesactivador.SetDatosGuardados(null);

        bloqueDistractor.ResetUI();
        bloqueActivador.ResetUI();
        bloqueDesactivador.ResetUI();

        bloqueActivador.OcultarCamposDeTipos();
        bloqueDesactivador.OcultarCamposDeTipos();

        bloqueDistractor.textoTipo.text = "Seleccione uno";
        bloqueDistractor.textoDetalle.text = "Seleccione uno";
        bloqueActivador.textoTipo.text = "Seleccione uno";
        bloqueActivador.textoDetalle.text = "Seleccione uno";
        bloqueDesactivador.textoTipo.text = "Seleccione uno";
        bloqueDesactivador.textoDetalle.text = "Seleccione uno";

        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = false;
        if (botonDeshacer != null)
            botonDeshacer.interactable = false;
    }
}