using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BloqueDistractor : MonoBehaviour {

    public TMP_Dropdown dropdownEscena;
    public TMP_Dropdown dropdownDistractor;
    public List<string> listaEscenarios;
    public List<string> distractoresCocina;
    public List<string> distractoresDormitorio;
    public List<string> distractoresSalon;

    public TMP_Text textoTipo;
    public TMP_Text textoDetalle;

    public Color colorError;
    public Color colorNormalTexto;


    private EscenaDistractorData datosGuardados = null;

    void Start() {
        PoblarDropdown(dropdownEscena, listaEscenarios, "Seleccione uno");
        PoblarDropdown(dropdownDistractor, new List<string>(), "Seleccione uno");
        dropdownEscena.onValueChanged.AddListener(delegate { ActualizarDistractores(); });
    }

    private void PoblarDropdown(TMP_Dropdown dropdown, List<string> lista, string opcionDefault) {
        dropdown.ClearOptions();
        var opciones = new List<string> { opcionDefault };
        opciones.AddRange(lista);
        dropdown.AddOptions(opciones);
        dropdown.value = 0;
    }

    // --- PULSAR "Check" PARA GUARDAR ESTADO ---
    public void GuardarDatos() {
        string escena = dropdownEscena.options[dropdownEscena.value].text;
        string distractor = dropdownDistractor.options[dropdownDistractor.value].text;

        datosGuardados = new EscenaDistractorData {
            escena = (dropdownEscena.value == 0 || escena == "Seleccione uno") ? "" : escena,
            distractor = (dropdownDistractor.value == 0 || distractor == "Seleccione uno") ? "" : distractor
        };

        Debug.Log("Distractor guardado:\n" + JsonUtility.ToJson(datosGuardados, true));
    }

    // --- PULSAR "Edit" PARA RECUPERAR ESTADO ---
    public void RecuperarDatos() {
        if (datosGuardados == null) {
            ResetUI();
            return;
        }

        int idxEscena = dropdownEscena.options.FindIndex(o => o.text == datosGuardados.escena);
        int idxDistractor = dropdownDistractor.options.FindIndex(o => o.text == datosGuardados.distractor);

        dropdownEscena.value = Mathf.Max(idxEscena, 0);
        dropdownDistractor.value = Mathf.Max(idxDistractor, 0);
    }

    public EscenaDistractorData GetDatosGuardados() {
        return datosGuardados;
    }

    public void SetDatosGuardados(EscenaDistractorData datos) {
        datosGuardados = datos;
    }

    public void ResetUI() {
        dropdownEscena.value = 0;
        dropdownDistractor.value = 0;
    }

    public void MostrarVisualizacion() {

        // Si no hay datos del distractor
        if (datosGuardados == null) {
            textoTipo.text = "Seleccione uno";
            textoDetalle.text = "Seleccione uno";
            ColoreaCampo(textoTipo);
            ColoreaCampo(textoDetalle);
            return;
        }

        // Si hay datos del distractor
        textoTipo.text = string.IsNullOrEmpty(datosGuardados.escena) ? "Seleccione uno" : datosGuardados.escena;
        textoDetalle.text = string.IsNullOrEmpty(datosGuardados.distractor) ? "Seleccione uno" : datosGuardados.distractor;
        ColoreaCampo(textoTipo);
        ColoreaCampo(textoDetalle);
        
    }
    
    private void ColoreaCampo(TMP_Text campo) {
        if (campo.text == "Seleccione uno")
            campo.color = colorError;
        else
            campo.color = colorNormalTexto;
    }

    public bool TieneEleccionReal() {
        return datosGuardados != null && (!string.IsNullOrEmpty(datosGuardados.escena) || !string.IsNullOrEmpty(datosGuardados.distractor));
    }

    private void ActualizarDistractores() {
        string escena = dropdownEscena.options[dropdownEscena.value].text;
        List<string> listaDistractores = new List<string>();
        if (escena == "Cocina") listaDistractores = distractoresCocina;
        else if (escena == "Dormitorio") listaDistractores = distractoresDormitorio;
        else if (escena == "Salón") listaDistractores = distractoresSalon;

        PoblarDropdown(dropdownDistractor, listaDistractores, "Seleccione uno");
    }
    
    public bool EsValido() {
        return datosGuardados != null
            && !string.IsNullOrEmpty(datosGuardados.escena)
            && !string.IsNullOrEmpty(datosGuardados.distractor);
    }

}