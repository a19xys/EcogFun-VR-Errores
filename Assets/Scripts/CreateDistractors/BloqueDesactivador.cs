using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BloqueDesactivador : MonoBehaviour {

    public Button botonNinguno;
    public Button botonAccion;
    public Button botonTiempo;

    public TMP_Dropdown dropdownAccion;
    public TMP_Dropdown dropdownObjeto;

    public TMP_InputField inputTimeAccion;
    public TMP_InputField inputTime; // Usado para tipo "tiempo"

    public Color colorSeleccionado;
    public Color colorNormal;

    private DesactivadorData datosGuardados = null;
    private string tipoSeleccionado = null;

    void Start() {
        botonNinguno.onClick.AddListener(() => SeleccionarTipo("ninguno"));
        botonAccion.onClick.AddListener(() => SeleccionarTipo("accion"));
        botonTiempo.onClick.AddListener(() => SeleccionarTipo("tiempo"));
        ResetUI();
    }

    public void SeleccionarTipo(string tipo) {
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
                datos.accionElegida = dropdownAccion.options[dropdownAccion.value].text;
                datos.objetoElegido = dropdownObjeto.options[dropdownObjeto.value].text;
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
}