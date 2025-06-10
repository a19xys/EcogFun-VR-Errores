using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BloqueDesactivador : MonoBehaviour {

    public Button botonNinguno;
    public Button botonAccion;
    public Button botonTiempo;

    public TMP_Dropdown dropdownAccion;
    public TMP_Dropdown dropdownObjeto;
    public List<string> listaAcciones;
    public List<string> listaObjetos;

    public TMP_InputField inputTimeAccion;
    public TMP_InputField inputTime;

    public TMP_Text textoTipo;
    public TMP_Text textoDetalle;

    public GameObject contenedorNinguno;
    public GameObject contenedorAccion;
    public GameObject contenedorTiempo;

    public Color colorSeleccionado;
    public Color colorNormal;

    public Color colorError;
    public Color colorNormalTexto;

    private DesactivadorData datosGuardados = null;
    private string tipoSeleccionado = null;
    private string tipoAnterior = null;

    void Start() {
        botonNinguno.onClick.AddListener(() => SeleccionarTipo("ninguno"));
        botonAccion.onClick.AddListener(() => SeleccionarTipo("accion"));
        botonTiempo.onClick.AddListener(() => SeleccionarTipo("tiempo"));
        PoblarDropdown(dropdownAccion, listaAcciones, "Seleccione uno");
        PoblarDropdown(dropdownObjeto, listaObjetos, "Seleccione uno");
        ResetUI();
    }

    private void PoblarDropdown(TMP_Dropdown dropdown, List<string> lista, string opcionDefault) {
        dropdown.ClearOptions();
        var opciones = new List<string> { opcionDefault };
        opciones.AddRange(lista);
        dropdown.AddOptions(opciones);
        dropdown.value = 0; // Por defecto, opcion "Seleccione uno"
    }

    public void SeleccionarTipo(string tipo) {
        if (tipo != tipoSeleccionado) { LimpiarCamposParaTipo(tipo); }
        tipoAnterior = tipoSeleccionado;
        tipoSeleccionado = tipo;
        ActualizarColoresBotones();
    }

    private void ActualizarColoresBotones() {
        ColoreaBoton(botonNinguno, tipoSeleccionado == "ninguno");
        ColoreaBoton(botonAccion, tipoSeleccionado == "accion");
        ColoreaBoton(botonTiempo, tipoSeleccionado == "tiempo");
    }

    private void ColoreaBoton(Button boton, bool seleccionado) {
        var img = boton.GetComponent<Image>();
        if (img != null)
            img.color = seleccionado ? colorSeleccionado : colorNormal;
    }

    // --- PULSAR "Check" PARA GUARDAR ESTADO ---
    public void GuardarDatos() {

        DesactivadorData datos = new DesactivadorData();
        datos.tipo = tipoSeleccionado;

        switch (tipoSeleccionado) {
            case "accion":
                string accionD = dropdownAccion.options[dropdownAccion.value].text;
                string objetoD = dropdownObjeto.options[dropdownObjeto.value].text;
                datos.accionElegida = (dropdownAccion.value == 0 || accionD == "Seleccione uno") ? "" : accionD;
                datos.objetoElegido = (dropdownObjeto.value == 0 || objetoD == "Seleccione uno") ? "" : objetoD;
                int.TryParse(inputTimeAccion.text, out datos.time);
                break;
            case "tiempo":
                int.TryParse(inputTime.text, out datos.time);
                break;
            case "ninguno":
            default:
                break;
        }

        datosGuardados = datos;
        Debug.Log("Desactivador guardado:\n" + JsonUtility.ToJson(datosGuardados, true));

    }

    // --- PULSAR "Edit" PARA RECUPERAR ESTADO ---
    public void RecuperarDatos() {

        if (datosGuardados == null) { ResetUI(); return; }

        tipoSeleccionado = datosGuardados.tipo;
        ActualizarColoresBotones();

        switch (tipoSeleccionado) {
            case "accion":
                dropdownAccion.value = dropdownAccion.options.FindIndex(o => o.text == datosGuardados.accionElegida);
                dropdownObjeto.value = dropdownObjeto.options.FindIndex(o => o.text == datosGuardados.objetoElegido);
                inputTimeAccion.text = datosGuardados.time.ToString();
                break;
            case "tiempo":
                inputTime.text = datosGuardados.time.ToString();
                break;
            case "ninguno":
            default:
                break;
        }
    }

    public DesactivadorData GetDatosGuardados() {
        return datosGuardados;
    }

    public void SetDatosGuardados(DesactivadorData datos) {
        datosGuardados = datos;
    }

    public void ResetUI() {
        tipoSeleccionado = null;
        ActualizarColoresBotones();

        dropdownAccion.value = 0;
        dropdownObjeto.value = 0;
        inputTimeAccion.text = "";
        inputTime.text = "";
    }

    private void LimpiarCamposParaTipo(string tipo) {
        if (datosGuardados == null || datosGuardados.tipo != tipo) {
            switch (tipo) {
                case "accion":
                    dropdownAccion.value = 0;
                    dropdownObjeto.value = 0;
                    inputTimeAccion.text = "";
                    break;
                case "tiempo":
                    inputTime.text = "";
                    break;
                case "ninguno":
                default:
                    break;
            }
        }
    }

    public void OcultarCamposDeTipos() {
        if (contenedorAccion) contenedorAccion.SetActive(false);
        if (contenedorTiempo) contenedorTiempo.SetActive(false);
        if (contenedorNinguno) contenedorNinguno.SetActive(false);
    }

    public void MostrarVisualizacion() {

        // Si no hay datos del desactivador
        if (datosGuardados == null) {
            textoTipo.text = "Seleccione uno";
            textoDetalle.text = "Seleccione uno";
            ColoreaCampo(textoTipo);
            ColoreaCampo(textoDetalle);
            return;
        }

        // Si hay datos del desactivador
        switch (datosGuardados.tipo)
        {
            case "accion":
                textoTipo.text = "Acción";
                textoDetalle.text =
                    (string.IsNullOrEmpty(datosGuardados.accionElegida) || string.IsNullOrEmpty(datosGuardados.objetoElegido))
                    ? "Seleccione uno"
                    : $"{datosGuardados.accionElegida} {MinusculaInicial(datosGuardados.objetoElegido)}" +
                    (datosGuardados.time > 0
                        ? $", tras {FormatearTiempo(datosGuardados.time)}"
                        : ", inmediatamente");
                break;
            case "tiempo":
                textoTipo.text = "Tiempo";
                textoDetalle.text = datosGuardados.time > 0
                    ? $"Tras {FormatearTiempo(datosGuardados.time)} de distracción"
                    : "Seleccione uno";
                break;
            case "ninguno":
                textoTipo.text = "Ninguno";
                textoDetalle.text = "Manualmente";
                break;
        }

        ColoreaCampo(textoTipo);
        ColoreaCampo(textoDetalle);
        
    }
    
    private void ColoreaCampo(TMP_Text campo) {
        if (campo.text == "Seleccione uno")
            campo.color = colorError;
        else
            campo.color = colorNormalTexto;
    }

    private string FormatearTiempo(int segundos) {
        if (segundos <= 0) return "";
        int min = segundos / 60;
        int seg = segundos % 60;
        if (min > 0 && seg > 0)
            return $"{min} minuto{(min > 1 ? "s" : "")} y {seg} segundo{(seg > 1 ? "s" : "")}";
        if (min > 0)
            return $"{min} minuto{(min > 1 ? "s" : "")}";
        return $"{seg} segundo{(seg > 1 ? "s" : "")}";
    }

    private string MinusculaInicial(string s) {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToLower(s[0]) + s.Substring(1);
    }

    public bool TieneEleccionReal() {
        return datosGuardados != null && !string.IsNullOrEmpty(datosGuardados.tipo);
    }

    public bool EsValido() {
        if (datosGuardados == null || string.IsNullOrEmpty(datosGuardados.tipo)) return false;
        switch (datosGuardados.tipo) {
            case "ninguno":
                return true;
            case "accion":
                return !string.IsNullOrEmpty(datosGuardados.accionElegida)
                    && !string.IsNullOrEmpty(datosGuardados.objetoElegido)
                    && datosGuardados.time >= 0;
            case "tiempo":
                return datosGuardados.time > 0;
            default:
                return false;
        }
    }

    public void MostrarContenedorTipoSeleccionado() {
        OcultarCamposDeTipos();
        if (datosGuardados == null || string.IsNullOrEmpty(datosGuardados.tipo)) return;
        switch (datosGuardados.tipo) {
            case "accion":
                if (contenedorAccion) contenedorAccion.SetActive(true);
                break;
            case "tiempo":
                if (contenedorTiempo) contenedorTiempo.SetActive(true);
                break;
            case "ninguno":
                if (contenedorNinguno) contenedorNinguno.SetActive(true);
                break;
        }
    }

}