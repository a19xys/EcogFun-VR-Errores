using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DistractorItemUI : MonoBehaviour {

    public Button botonSelect;
    public Image backgroundImage;
    private DistractorData datos;
    private CreadorDistractores manager;

    public void Inicializar(DistractorData datos, CreadorDistractores manager) {
        this.datos = datos;
        this.manager = manager;
        if (botonSelect != null)
            botonSelect.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        if (manager != null)
            manager.OnDistractorSeleccionado(datos);
    }

    public void SetInteractable(bool interactable) {
        if (botonSelect != null)
            botonSelect.interactable = interactable;
    }

}