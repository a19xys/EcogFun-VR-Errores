using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string header;
    [TextArea] public string content;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.RequestShow(header, content);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }

    void OnDisable()
    {
        TooltipSystem.Hide();
    }
}