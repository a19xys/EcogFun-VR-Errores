using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipController : MonoBehaviour {

    public Camera sceneCamera; // Asigna la cámara de la escena.
    public RectTransform canvasRectTransform; // Asigna el RectTransform del Canvas.
    public TextMeshProUGUI tooltipHeader; // Referencia a la cabecera del Tooltip.
    public TextMeshProUGUI tooltipText; // Referencia al texto del Tooltip.

    private RectTransform rectTransform; // RectTransform del prefab.

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        // Obtener la posición del ratón en coordenadas de pantalla.
        Vector2 mousePosition = Input.mousePosition;

        // Convertir la posición de pantalla a coordenadas locales del RectTransform del Canvas.
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, sceneCamera, out localPoint)) {
            rectTransform.localPosition = localPoint; // Asignar la posición al RectTransform del prefab.
        }
    }

    public void ShowTooltip(string header, string message) {
        if (rectTransform == null) { rectTransform = GetComponent<RectTransform>(); }

        tooltipHeader.text = $"<u>{header}</u>";
        tooltipText.text = message;

        // Actualizar la posición inmediatamente antes de mostrarlo.
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, sceneCamera, out localPoint)) {
            rectTransform.localPosition = localPoint;
        }

        gameObject.SetActive(true);
    }

    public void HideTooltip() {
        gameObject.SetActive(false);
    }
}
