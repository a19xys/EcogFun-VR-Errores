using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PercentageController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public LoadData loadData; // Referencia a LoadData.
    public TooltipController tooltip; // Referencia al Tooltip.
    public string header;

    private string message;
    private int thisCount;

    public void OnPointerEnter(PointerEventData eventData) {

        GameObject textGroup = this.gameObject.transform.parent.gameObject;

        // Obtenemos la cantidad de eventos de cada tipo.
        if (textGroup.name == "piechart_1_texts") {

            if (this.gameObject.name == "text_1_1_comision") { thisCount = loadData.actionCounts[0]; }
            else if (this.gameObject.name == "text_1_2_orden") { thisCount = loadData.actionCounts[1]; }
            else if (this.gameObject.name == "text_1_3_normas") { thisCount = loadData.actionCounts[2]; }
            else if (this.gameObject.name == "text_1_4_omision") { thisCount = loadData.actionCounts[3]; }
            else { thisCount = loadData.actionCounts[4]; }

            message = $"{((float)thisCount / loadData.actionCounts.Sum() * 100f):0}% ({thisCount}/{loadData.actionCounts.Sum()})"; // Formato del porcentaje

        } else {

            if (this.gameObject.name == "text_2_1_aciertos") { thisCount = loadData.correctCounts[0]; }
            if (this.gameObject.name == "text_2_2_fallos") { thisCount = loadData.correctCounts[1]; }

            message = $"{((float)thisCount / loadData.correctCounts.Sum() * 100f):0}% ({thisCount}/{loadData.correctCounts.Sum()})"; // Formato del porcentaje

        }

        if (loadData.correctCounts.Sum() != 0) {
            tooltip.ShowTooltip(header, message); // Mostrar el Tooltip
        }
        
    }

    public void OnPointerExit(PointerEventData eventData) {
        tooltip.HideTooltip();
    }

}