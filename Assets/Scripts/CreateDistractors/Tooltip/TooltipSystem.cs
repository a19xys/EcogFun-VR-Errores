using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
    public TooltipManager tooltip;
    public float appearDelay = 0.5f;
    public bool followMouse = true;
    public Vector2 offset = new Vector2(20, -20);

    private static TooltipSystem current;

    private float timer;
    private bool pendingShow;
    private string pendingHeader;
    private string pendingContent;

    void Awake()
    {
        current = this;
        HideImmediate();
    }

    void Update()
    {
        // Gestionar retraso de aparición
        if (pendingShow)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= appearDelay)
            {
                ShowNow(pendingHeader, pendingContent);
                pendingShow = false;
            }
        }

        // Seguir el ratón
        if (tooltip != null && tooltip.gameObject.activeSelf && followMouse)
        {
            tooltip.UpdatePosition(offset);
        }
    }

    // Llamar desde TooltipElement para solicitar un tooltip
    public static void RequestShow(string header, string content)
    {
        if (current == null || current.tooltip == null)
            return;

        // Si ya hay un tooltip visible, se oculta y reinicia delay
        current.HideImmediate();

        // Guardar el texto pendiente y empezar el temporizador
        current.pendingHeader = header;
        current.pendingContent = content;
        current.pendingShow = true;
        current.timer = 0;
    }

    public static void Hide()
    {
        if (current == null) return;
        current.HideImmediate();
    }

    private void ShowNow(string header, string content)
    {
        if (tooltip == null) return;
        tooltip.SetTextAndShow(header, content);
        tooltip.UpdatePosition(offset);
    }

    private void HideImmediate()
    {
        if (tooltip != null)
        {
            if (tooltip.gameObject.activeSelf && tooltip.gameObject.activeInHierarchy)
                tooltip.FadeOut();
            else
                tooltip.gameObject.SetActive(false);
        }
        timer = 0;
        pendingShow = false;
    }

}