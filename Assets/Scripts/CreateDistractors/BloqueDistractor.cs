using UnityEngine;
using TMPro;

public class BloqueDistractor : MonoBehaviour {

    public TMP_Dropdown dropdownEscena;
    public TMP_Dropdown dropdownDistractor;

    private EscenaDistractorData datosGuardados = null;

    // --- PULSAR "Check" PARA GUARDAR ESTADO ---
    public void GuardarDatos() {
        datosGuardados = new EscenaDistractorData {
            escena = dropdownEscena.options[dropdownEscena.value].text,
            distractor = dropdownDistractor.options[dropdownDistractor.value].text
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

}
