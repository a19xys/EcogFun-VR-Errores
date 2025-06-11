using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

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

    public GameObject botonDeshacerCambios;
    public GameObject botonAnadirDistractor;
    public GameObject botonEliminarDistractor;
    public GameObject botonEditarDistractor;

    public Button botonRefrescar;
    public Button botonDeshacer;
    public Button botonCrearDistractor;
    public Button botonEliminarDistractorBtn;
    public Button botonEditarDistractorBtn;

    public TextMeshProUGUI cargarSesionButtonText;
    public TMP_Text tituloModo;
    public Image fondoUI;

    public Color colorCreador;
    public Color colorEditor;

    private DistractorData distractorSeleccionado = null;
    private DistractorData distractorOriginalCopia = null;
    private bool esModoEditor = false;

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

    public void OnSessionLoaded(SessionData session, string fileName)
    {
        // Deshacer cambios para limpieza visual
        DeshacerCambios();

        distractorScrollView.MostrarDistractores(session.distractors, esModoEditor, this);

        if (cargarSesionButtonText != null && !string.IsNullOrEmpty(fileName))
            cargarSesionButtonText.text = fileName;

        ActivarBlocksInit(false);

        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = false;
        if (botonDeshacer != null)
            botonDeshacer.interactable = false;

        RevisarBotonRefrescar();
        CambiarAModoCreador();
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

    public void RevisarBotonCrearDistractor()
    {
        bool esValido =
            bloqueDistractor != null && bloqueDistractor.EsValido() &&
            bloqueActivador != null && bloqueActivador.EsValido() &&
            bloqueDesactivador != null && bloqueDesactivador.EsValido();

        if (botonCrearDistractor != null)
            botonCrearDistractor.interactable = esValido;
    }

    public void RevisarBotonRefrescar()
    {
        bool sesionCargada = fileManager != null && fileManager.currentSession != null;
        bool hayDistractores = sesionCargada && fileManager.currentSession.distractors != null && fileManager.currentSession.distractors.Count > 0;

        if (botonRefrescar != null)
            botonRefrescar.interactable = hayDistractores;
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

        // Normalizamos los nombres de escena y distractor a minusculas y sin espacios ni acentos
        string escenaId = NormalizarParaId(escena.escena);
        string distractorId = NormalizarParaId(escena.distractor);

        // Fecha y hora en formato yyyyMMdd_HHmmss
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        // "salon_television_20250611_143501"
        string id = $"{escenaId}_{distractorId}_{timestamp}";

        var nuevoDistractor = new DistractorData
        {
            id = id,
            scene = escena.escena,
            distractorType = escena.distractor,
            activation = ConstruirActivationBlock(activador),
            deactivation = ConstruirDeactivationBlock(desactivador)
        };

        sessionActual.distractors.Add(nuevoDistractor);

        distractorScrollView.MostrarDistractores(sessionActual.distractors, esModoEditor, this);

        // Guardar en JSON
        fileManager.GuardarSesionEnArchivo();

        // Deshacer cambios para limpieza visual
        DeshacerCambios();

        RevisarBotonRefrescar();
    }

    public void EliminarDistractorSeleccionado() {
        if (!esModoEditor || distractorSeleccionado == null) return;

        var session = fileManager.currentSession;
        if (session == null || session.distractors == null) return;

        // Elimina el distractor seleccionado de la lista
        session.distractors.Remove(distractorSeleccionado);

        // Guarda el JSON actualizado
        fileManager.GuardarSesionEnArchivo();

        // Refresca el ScrollView en modo editor
        distractorScrollView.MostrarDistractores(session.distractors, esModoEditor, this);

        // Deselecciona cualquier distractor y desactiva el boton
        distractorSeleccionado = null;
        SetEliminarDistractorInteractable(false);

        // Limpia los bloques
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

        // Comprobar si no queda ningún distractor
        if (session.distractors == null || session.distractors.Count == 0)
        {
            RevisarBotonRefrescar(); // Desactiva botón refrescar si corresponde
            CambiarAModoCreador();   // Cambia automáticamente al modo creación
        }
        else { ActivarBlocksInit(true); }
    }

    public void ModificarDistractorSeleccionado() {
        if (!esModoEditor || distractorSeleccionado == null || fileManager.currentSession == null)
            return;

        var distractores = fileManager.currentSession.distractors;
        if (distractores == null || distractores.Count == 0)
            return;

        // Busca el índice del distractor original por su ID
        int idx = distractores.FindIndex(d => d.id == distractorSeleccionado.id);
        if (idx < 0)
            return; // No encontrado

        // Recoge los datos actuales de los bloques
        var escena = bloqueDistractor.GetDatosGuardados();
        var activador = bloqueActivador.GetDatosGuardados();
        var desactivador = bloqueDesactivador.GetDatosGuardados();

        // Genera nueva ID única
        string escenaId = NormalizarParaId(escena.escena);
        string distractorId = NormalizarParaId(escena.distractor);
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string nuevoId = $"{escenaId}_{distractorId}_{timestamp}";

        var distractorModificado = new DistractorData {
            id = nuevoId,
            scene = escena.escena,
            distractorType = escena.distractor,
            activation = ConstruirActivationBlock(activador),
            deactivation = ConstruirDeactivationBlock(desactivador)
        };

        // Reemplaza en la misma posicion
        distractores[idx] = distractorModificado;

        // Guarda el JSON actualizado
        fileManager.GuardarSesionEnArchivo();

        // Refresca el scrollview
        distractorScrollView.MostrarDistractores(distractores, esModoEditor, this);

        // Selecciona de nuevo el distractor editado
        int idxModificado = distractores.FindIndex(d => d.id == distractorModificado.id);
        if (idxModificado >= 0)
            distractorScrollView.OnSelectDistractor(idxModificado);

        // Desactiva el botón de edición hasta el próximo cambio
        SetEditarDistractorInteractable(false);
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
                    @params = new Dictionary<string, object> { { "time", datos.time } }
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
                    @params = new Dictionary<string, object> { { "time", datos.time } }
                });
                break;
            case "ninguno":
            default:
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

    public void ActivarBlocksInit(bool activar)
    {
        if (blocks_init != null)
            foreach (GameObject go in blocks_init)
                if (go != null)
                    go.SetActive(activar);
    }

    public void AlternarModoEditor()
    {
        esModoEditor = !esModoEditor;

        // Cambia el texto del titulo
        if (tituloModo != null)
            tituloModo.text = esModoEditor ? "Editor de distractores" : "Creador de distractores";

        // Cambia el color del fondo
        if (fondoUI != null)
            fondoUI.color = esModoEditor ? colorEditor : colorCreador;

        DeshacerCambios();
        RefrescarScrollDistractores();
        SetEliminarDistractorInteractable(!esModoEditor);
        SetEditarDistractorInteractable(!esModoEditor);
        AlternarBotonesPorModo();

        if (esModoEditor) { ActivarBlocksInit(true); }
        else { ActivarBlocksInit(false); }
    }

    public void CambiarAModoCreador() {
        esModoEditor = false;

        if (tituloModo != null)
            tituloModo.text = "Creador de distractores";
        if (fondoUI != null)
            fondoUI.color = colorCreador;

        AlternarBloquesVisualizacion(true);
        RefrescarScrollDistractores();
        AlternarBotonesPorModo();
        distractorScrollView.DeseleccionarDistractor();
    }

    private void RefrescarScrollDistractores() {
        distractorScrollView.MostrarDistractores(fileManager.currentSession?.distractors ?? new List<DistractorData>(), esModoEditor, this);
    }

    public void AlternarBotonesPorModo() {
        bool esEditor = esModoEditor;

        // Modo CREACION
        if (!esEditor) {
            if (botonDeshacerCambios) botonDeshacerCambios.SetActive(true);
            if (botonAnadirDistractor) botonAnadirDistractor.SetActive(true);

            if (botonEliminarDistractor) botonEliminarDistractor.SetActive(false);
            if (botonEditarDistractor) botonEditarDistractor.SetActive(false);
        }
        // Modo EDICION
        else
        {
            if (botonDeshacerCambios) botonDeshacerCambios.SetActive(false);
            if (botonAnadirDistractor) botonAnadirDistractor.SetActive(false);

            if (botonEliminarDistractor) botonEliminarDistractor.SetActive(true);
            if (botonEditarDistractor) botonEditarDistractor.SetActive(true);
        }
    }

    public void OnDistractorSeleccionado(DistractorData datos)
    {
        if (!esModoEditor) return;
        distractorSeleccionado = datos;

        // Copia profunda del distractor original para comparar cambios
        distractorOriginalCopia = CopiaProfundaDistractor(datos);

        // Rellenar bloques
        bloqueDistractor.SetDatosGuardados(new EscenaDistractorData { escena = datos.scene, distractor = datos.distractorType });
        bloqueDistractor.RecuperarDatos();
        bloqueDistractor.MostrarVisualizacion();

        // Activador
        if (datos.activation != null && datos.activation.steps != null && datos.activation.steps.Count > 0)
        {
            var step = datos.activation.steps[0];
            bloqueActivador.SetDatosGuardados(FromStepToActivadorData(step));
        }
        else { bloqueActivador.SetDatosGuardados(null); }

        bloqueActivador.RecuperarDatos();
        bloqueActivador.MostrarVisualizacion();
        bloqueActivador.MostrarContenedorTipoSeleccionado();

        // Desactivador
        if (datos.deactivation != null && datos.deactivation.steps != null && datos.deactivation.steps.Count > 0)
        {
            var step = datos.deactivation.steps[0];
            bloqueDesactivador.SetDatosGuardados(FromStepToDesactivadorData(step));
        }
        else { bloqueDesactivador.SetDatosGuardados(new DesactivadorData { tipo = "ninguno" }); }

        bloqueDesactivador.RecuperarDatos();
        bloqueDesactivador.MostrarVisualizacion();
        bloqueDesactivador.MostrarContenedorTipoSeleccionado();
        AlternarBloquesVisualizacion(true);
        ActivarBlocksInit(false);
        SetEliminarDistractorInteractable(true);
        SetEditarDistractorInteractable(false);
    }

    public void SetEliminarDistractorInteractable(bool interactable) {
        if (botonEliminarDistractorBtn != null)
            botonEliminarDistractorBtn.interactable = interactable;
    }

    public void SetEditarDistractorInteractable(bool interactable) {
        if (botonEditarDistractorBtn != null)
            botonEditarDistractorBtn.interactable = interactable;
    }

    public static ActivadorData FromStepToActivadorData(Step step)
    {
        var datos = new ActivadorData();
        switch (step.type)
        {
            case "action":
                datos.tipo = "accion";
                if (step.@params != null)
                {
                    object accion, objeto, timeObj;
                    step.@params.TryGetValue("action", out accion);
                    step.@params.TryGetValue("item", out objeto);
                    step.@params.TryGetValue("time", out timeObj);
                    datos.accionElegida = accion != null ? accion.ToString() : "";
                    datos.objetoElegido = objeto != null ? objeto.ToString() : "";
                    datos.time = timeObj != null ? Convert.ToInt32(timeObj) : 0;
                }
                break;
            case "success_count":
                datos.tipo = "aciertos";
                if (step.@params != null)
                {
                    object aciertosObj, timeObj;
                    step.@params.TryGetValue("count", out aciertosObj);
                    step.@params.TryGetValue("time", out timeObj);
                    datos.aciertos = aciertosObj != null ? Convert.ToInt32(aciertosObj) : 0;
                    datos.time = timeObj != null ? Convert.ToInt32(timeObj) : 0;
                }
                break;
            case "time":
                datos.tipo = "tiempo";
                if (step.@params != null)
                {
                    object timeObj;
                    step.@params.TryGetValue("time", out timeObj);
                    datos.time = timeObj != null ? Convert.ToInt32(timeObj) : 0;
                }
                break;
            case "success_rate":
                datos.tipo = "efectividad";
                if (step.@params != null)
                {
                    object rateObj, timeObj;
                    step.@params.TryGetValue("rate", out rateObj);
                    step.@params.TryGetValue("time", out timeObj);
                    datos.rate = rateObj != null ? Convert.ToInt32(rateObj) : 0;
                    datos.time = timeObj != null ? Convert.ToInt32(timeObj) : 0;
                }
                break;
            default:
                datos.tipo = "";
                break;
        }
        return datos;
    }

    public static DesactivadorData FromStepToDesactivadorData(Step step)
    {
        var datos = new DesactivadorData();

        if (step == null)
        {
            datos.tipo = "ninguno";
            return datos;
        }

        switch (step.type)
        {
            case "action":
                datos.tipo = "accion";
                if (step.@params != null)
                {
                    object accion, objeto, timeObj;
                    step.@params.TryGetValue("action", out accion);
                    step.@params.TryGetValue("item", out objeto);
                    step.@params.TryGetValue("time", out timeObj);
                    datos.accionElegida = accion != null ? accion.ToString() : "";
                    datos.objetoElegido = objeto != null ? objeto.ToString() : "";
                    datos.time = timeObj != null ? Convert.ToInt32(timeObj) : 0;
                }
                break;
            case "time":
                datos.tipo = "tiempo";
                if (step.@params != null)
                {
                    object timeObj;
                    step.@params.TryGetValue("time", out timeObj);
                    datos.time = timeObj != null ? Convert.ToInt32(timeObj) : 0;
                }
                break;
            default:
                datos.tipo = "ninguno";
                break;
        }
        return datos;
    }

    private bool SonStepsIguales(Step a, Step b) {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        if (a.type != b.type) return false;
        if ((a.@params == null) != (b.@params == null)) return false;
        if (a.@params == null && b.@params == null) return true;

        if (a.@params.Count != b.@params.Count) return false;
        foreach (var kv in a.@params) {
            if (!b.@params.ContainsKey(kv.Key)) return false;

            var valA = kv.Value;
            var valB = b.@params[kv.Key];

            // Si ambos son numéricos, compara por valor
            if (IsNumericType(valA) && IsNumericType(valB)) {
                var longA = Convert.ToInt64(valA);
                var longB = Convert.ToInt64(valB);
                if (longA != longB) return false;
            } else {
                // Compara como string
                if ((valA == null && valB != null) || (valA != null && valB == null)) return false;
                if (valA != null && valA.ToString() != valB.ToString()) return false;
            }
        }
        return true;
    }

    private bool IsNumericType(object obj) {
        return obj is byte || obj is sbyte ||
            obj is short || obj is ushort ||
            obj is int || obj is uint ||
            obj is long || obj is ulong ||
            obj is float || obj is double || obj is decimal;
    }

    private bool HayCambiosEnBloqueDistractor() {
        var datosEscena = bloqueDistractor.GetDatosGuardados();
        return datosEscena.escena != distractorOriginalCopia.scene ||
            datosEscena.distractor != distractorOriginalCopia.distractorType;
    }

    private bool HayCambiosEnBloqueActivador() {
        var datosActivador = bloqueActivador.GetDatosGuardados();
        var activacionOriginal = (distractorOriginalCopia.activation != null && distractorOriginalCopia.activation.steps != null && distractorOriginalCopia.activation.steps.Count > 0)
            ? distractorOriginalCopia.activation.steps[0]
            : null;
        var datosActivadorBlock = ConstruirActivationBlock(datosActivador);
        var activacionActual = (datosActivadorBlock != null && datosActivadorBlock.steps != null && datosActivadorBlock.steps.Count > 0)
            ? datosActivadorBlock.steps[0]
            : null;
        return !SonStepsIguales(activacionOriginal, activacionActual);
    }

    private bool HayCambiosEnBloqueDesactivador() {
        var datosDesactivador = bloqueDesactivador.GetDatosGuardados();
        var desactivacionOriginal = (distractorOriginalCopia.deactivation != null && distractorOriginalCopia.deactivation.steps != null && distractorOriginalCopia.deactivation.steps.Count > 0)
            ? distractorOriginalCopia.deactivation.steps[0]
            : null;
        var datosDesactivadorBlock = ConstruirDeactivationBlock(datosDesactivador);
        var desactivacionActual = (datosDesactivadorBlock != null && datosDesactivadorBlock.steps != null && datosDesactivadorBlock.steps.Count > 0)
            ? datosDesactivadorBlock.steps[0]
            : null;
        return !SonStepsIguales(desactivacionOriginal, desactivacionActual);
    }

    public void RevisarBotonEditarDistractorMultiBloque() {
        if (!esModoEditor || distractorSeleccionado == null || distractorOriginalCopia == null) {
            SetEditarDistractorInteractable(false);
            return;
        }
        bool cambiado = HayCambiosEnBloqueDistractor() || HayCambiosEnBloqueActivador() || HayCambiosEnBloqueDesactivador();
        bool datosValidos = bloqueDistractor.EsValido() && bloqueActivador.EsValido() && bloqueDesactivador.EsValido();
        SetEditarDistractorInteractable(cambiado && datosValidos);
    }

    private DistractorData CopiaProfundaDistractor(DistractorData original)
    {
        string json = JsonConvert.SerializeObject(original);
        return JsonConvert.DeserializeObject<DistractorData>(json);
    }

    private string NormalizarParaId(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        // Convertimos a minusculas
        string normalized = input.ToLower();

        // Quitamos acentos
        normalized = normalized
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("ñ", "n");

        // Espacios a "_"
        normalized = normalized.Replace(" ", "_");

        // Quitar cualquier caracter no alfanumerico ni "_"
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"[^a-z0-9_]", "");

        return normalized;
    }

}