using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BloqueActivador : MonoBehaviour {

    public Button botonAccion;
    public Button botonTiempo;
    public Button botonAciertos;
    public Button botonEfectividad;

    public TMP_Dropdown dropdownAccion;
    public TMP_Dropdown dropdownObjeto;

    public TMP_InputField inputAciertos;
    public TMP_InputField inputRate;

    public TMP_InputField inputTimeAccion;
    public TMP_InputField inputTimeAciertos;
    public TMP_InputField inputTime; // Usado para tipo "tiempo"
    public TMP_InputField inputTimeEfectividad;

    public Color colorSeleccionado;
    public Color colorNormal;

    private ActivadorData datosGuardados = null;
    private string tipoSeleccionado = null;

    void Start()
    {
        botonAccion.onClick.AddListener(() => SeleccionarTipo("accion"));
        botonTiempo.onClick.AddListener(() => SeleccionarTipo("tiempo"));
        botonAciertos.onClick.AddListener(() => SeleccionarTipo("aciertos"));
        botonEfectividad.onClick.AddListener(() => SeleccionarTipo("efectividad"));
        ResetUI();
    }

    public void SeleccionarTipo(string tipo) {
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
                datos.accionElegida = dropdownAccion.options[dropdownAccion.value].text;
                datos.objetoElegido = dropdownObjeto.options[dropdownObjeto.value].text;
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

}