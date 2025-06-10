using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BloqueActivador : MonoBehaviour {

    public Button botonAccion;
    public Button botonTiempo;
    public Button botonAciertos;
    public Button botonEfectividad;

    public TMP_Dropdown dropdownAccion;
    public TMP_Dropdown dropdownObjeto;
    public List<string> listaAcciones;
    public List<string> listaObjetos;

    public TMP_InputField inputAciertos;
    public TMP_InputField inputRate;

    public TMP_InputField inputTimeAccion;
    public TMP_InputField inputTimeAciertos;
    public TMP_InputField inputTime;
    public TMP_InputField inputTimeEfectividad;

    public TMP_Text textoTipo;
    public TMP_Text textoDetalle;

    public GameObject contenedorAccion;
    public GameObject contenedorAciertos;
    public GameObject contenedorTiempo;
    public GameObject contenedorEfectividad;

    public Color colorSeleccionado;
    public Color colorNormal;

    public Color colorError;
    public Color colorNormalTexto;

    private ActivadorData datosGuardados = null;
    private string tipoSeleccionado = null;
    private string tipoAnterior = null;


    void Start() {
        botonAccion.onClick.AddListener(() => SeleccionarTipo("accion"));
        botonTiempo.onClick.AddListener(() => SeleccionarTipo("tiempo"));
        botonAciertos.onClick.AddListener(() => SeleccionarTipo("aciertos"));
        botonEfectividad.onClick.AddListener(() => SeleccionarTipo("efectividad"));
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
        ColoreaBoton(botonAccion, tipoSeleccionado == "accion");
        ColoreaBoton(botonTiempo, tipoSeleccionado == "tiempo");
        ColoreaBoton(botonAciertos, tipoSeleccionado == "aciertos");
        ColoreaBoton(botonEfectividad, tipoSeleccionado == "efectividad");
    }

    private void ColoreaBoton(Button boton, bool seleccionado) {
        var img = boton.GetComponent<Image>();
        if (img != null)
            img.color = seleccionado ? colorSeleccionado : colorNormal;
    }

    // --- PULSAR "Check" PARA GUARDAR ESTADO ---
    public void GuardarDatos(){

        ActivadorData datos = new ActivadorData();
        datos.tipo = tipoSeleccionado;

        switch (tipoSeleccionado) {
            case "accion":
                string accion = dropdownAccion.options[dropdownAccion.value].text;
                string objeto = dropdownObjeto.options[dropdownObjeto.value].text;
                datos.accionElegida = (dropdownAccion.value == 0 || accion == "Seleccione uno") ? "" : accion;
                datos.objetoElegido = (dropdownObjeto.value == 0 || objeto == "Seleccione uno") ? "" : objeto;
                int.TryParse(inputTimeAccion.text, out datos.time);
                break;
            case "aciertos":
                int.TryParse(inputAciertos.text, out datos.aciertos);
                int.TryParse(inputTimeAciertos.text, out datos.time);
                break;
            case "tiempo":
                int.TryParse(inputTime.text, out datos.time);
                break;
            case "efectividad":
                int.TryParse(inputRate.text, out datos.rate);
                int.TryParse(inputTimeEfectividad.text, out datos.time);
                break;
        }

        datosGuardados = datos;
        Debug.Log("Activador guardado:\n" + JsonUtility.ToJson(datosGuardados, true));

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
            case "aciertos":
                inputAciertos.text = datosGuardados.aciertos.ToString();
                inputTimeAciertos.text = datosGuardados.time.ToString();
                break;
            case "tiempo":
                inputTime.text = datosGuardados.time.ToString();
                break;
            case "efectividad":
                inputRate.text = datosGuardados.rate.ToString();
                inputTimeEfectividad.text = datosGuardados.time.ToString();
                break;
        }
    }

    public ActivadorData GetDatosGuardados() {
        return datosGuardados;
    }

    public void SetDatosGuardados(ActivadorData datos) {
        datosGuardados = datos;
    }

    public void ResetUI() {
        tipoSeleccionado = null;
        ActualizarColoresBotones();

        dropdownAccion.value = 0;
        dropdownObjeto.value = 0;
        inputAciertos.text = "";
        inputRate.text = "";
        inputTimeAccion.text = "";
        inputTimeAciertos.text = "";
        inputTime.text = "";
        inputTimeEfectividad.text = "";
    }

    private void LimpiarCamposParaTipo(string tipo) {
        if (datosGuardados == null || datosGuardados.tipo != tipo) {
            switch (tipo) {
                case "accion":
                    dropdownAccion.value = 0;
                    dropdownObjeto.value = 0;
                    inputTimeAccion.text = "";
                    break;
                case "aciertos":
                    inputAciertos.text = "";
                    inputTimeAciertos.text = "";
                    break;
                case "tiempo":
                    inputTime.text = "";
                    break;
                case "efectividad":
                    inputRate.text = "";
                    inputTimeEfectividad.text = "";
                    break;
            }
        }
    }

    public void OcultarCamposDeTipos() {
        if (contenedorAccion) contenedorAccion.SetActive(false);
        if (contenedorAciertos) contenedorAciertos.SetActive(false);
        if (contenedorTiempo) contenedorTiempo.SetActive(false);
        if (contenedorEfectividad) contenedorEfectividad.SetActive(false);
    }

    public void MostrarVisualizacion() {
        
        // Si no hay datos del activador
        if (datosGuardados == null) {
            textoTipo.text = "Seleccione uno";
            textoDetalle.text = "Seleccione uno";
            ColoreaCampo(textoTipo);
            ColoreaCampo(textoDetalle);
            return;
        }

        // Si hay datos del activador
        switch (datosGuardados.tipo) {
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

            case "aciertos":
                textoTipo.text = "Aciertos";
                textoDetalle.text = datosGuardados.aciertos > 0
                    ? $"{datosGuardados.aciertos} acierto{(datosGuardados.aciertos > 1 ? "s" : "")}" +
                    (datosGuardados.time > 0
                        ? $", tras {FormatearTiempo(datosGuardados.time)}"
                        : ", inmediatamente")
                    : "Seleccione uno";
                break;

            case "tiempo":
                textoTipo.text = "Tiempo";
                textoDetalle.text = datosGuardados.time > 0
                    ? $"{FormatearTiempo(datosGuardados.time)} en la escena"
                    : "Seleccione uno";
                break;

            case "efectividad":
                textoTipo.text = "Efectividad";
                textoDetalle.text = datosGuardados.rate > 0
                    ? $"Ratio de {datosGuardados.rate} acierto{(datosGuardados.rate > 1 ? "s" : "")} por minuto" +
                    (datosGuardados.time > 0
                        ? $", tras {FormatearTiempo(datosGuardados.time)}"
                        : ", inmediatamente")
                    : "Seleccione uno";
                break;

            case "ninguno":
                textoTipo.text = "Ninguna";
                textoDetalle.text = "Manualmente";
                break;

            default:
                textoTipo.text = "Seleccione uno";
                textoDetalle.text = "Seleccione uno";
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
            case "accion":
                return !string.IsNullOrEmpty(datosGuardados.accionElegida)
                    && !string.IsNullOrEmpty(datosGuardados.objetoElegido)
                    && datosGuardados.time >= 0;
            case "tiempo":
                return datosGuardados.time > 0;
            case "aciertos":
                return datosGuardados.aciertos > 0 && datosGuardados.time >= 0;
            case "efectividad":
                return datosGuardados.rate > 0 && datosGuardados.time >= 0;
            default:
                return false;
        }
    }

}