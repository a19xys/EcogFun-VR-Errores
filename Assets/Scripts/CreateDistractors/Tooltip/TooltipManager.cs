using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class TooltipManager : MonoBehaviour
{

    [Header("Tiempos de fundido")]
    public float fadeInDuration = 0.32f;
    public float fadeOutDuration = 0.18f;

    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int maxWidth = 400;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Coroutine adjustRoutine;
    private Coroutine fadeRoutine;

    [HideInInspector] public RectTransform CanvasRectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            CanvasRectTransform = canvas.GetComponent<RectTransform>();
        else
            CanvasRectTransform = transform.root.GetComponent<RectTransform>();
    }

    public void SetText(string header, string content)
    {
        // (Asegura que el objeto está activo por si acaso)
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        headerField.text = header;
        headerField.gameObject.SetActive(!string.IsNullOrEmpty(header));
        contentField.text = content;

        // Si hay una corrutina anterior de ajuste de tamaño, detenla
        if (adjustRoutine != null)
            StopCoroutine(adjustRoutine);

        // Inicia la corrutina que ajusta el tamaño tras un frame
        adjustRoutine = StartCoroutine(DelayedAdjustSize());
    }

    private System.Collections.IEnumerator DelayedAdjustSize()
    {
        // Espera al menos 1 frame para que TMP y Layout actualicen los valores
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        AdjustSize();
        adjustRoutine = null;
    }

    public void SetPosition(Vector2 anchoredPos)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
    }

    public void UpdatePositionToMouse()
    {
        if (CanvasRectTransform == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                CanvasRectTransform = canvas.GetComponent<RectTransform>();
            else
                CanvasRectTransform = transform.root.GetComponent<RectTransform>();
        }
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        // 1. Obtener posición del mouse en el canvas
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;
        Camera cam = GetCanvasCamera();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRectTransform, mousePosition, cam, out localPoint))
            return;

        // 2. Obtener tamaño del tooltip
        Vector2 tooltipSize = rectTransform.sizeDelta;

        // 3. Obtener los límites del canvas (en local)
        Vector2 canvasMin = CanvasRectTransform.rect.min;
        Vector2 canvasMax = CanvasRectTransform.rect.max;

        // --- Nuevo: decide el mejor pivot ---
        Vector2 pivot = new Vector2(0, 1); // por defecto: esquina superior izquierda

        // ¿Cabría el tooltip hacia la derecha del ratón?
        bool fitsRight = (localPoint.x + tooltipSize.x) < canvasMax.x;
        // ¿Cabría el tooltip hacia la izquierda del ratón?
        bool fitsLeft = (localPoint.x - tooltipSize.x) > canvasMin.x;
        // ¿Cabría el tooltip por arriba del ratón?
        bool fitsUp = (localPoint.y + tooltipSize.y) < canvasMax.y;
        // ¿Cabría el tooltip por debajo del ratón?
        bool fitsDown = (localPoint.y - tooltipSize.y) > canvasMin.y;

        // Si no cabe hacia abajo, pero sí hacia arriba
        if (!fitsDown && fitsUp)
            pivot.y = 0; // inferior
        // Si tampoco cabe arriba, mantenemos el valor por defecto

        // Si no cabe a la derecha, pero sí a la izquierda
        if (!fitsRight && fitsLeft)
            pivot.x = 1; // derecha
        // Si tampoco cabe izquierda, mantenemos el valor por defecto

        // Aplicar el pivot
        rectTransform.pivot = pivot;

        // 4. Asignar la posición final del tooltip
        rectTransform.localPosition = localPoint;

        // Opcional: Si quieres un pequeño desplazamiento para que no esté justo encima del ratón:
        // rectTransform.localPosition += new Vector3(3, -3, 0);
    }

    public void AdjustSize()
    {
        float headerWidth = headerField.preferredWidth;
        float contentWidth = contentField.preferredWidth;
        float tooltipWidth = Mathf.Max(headerWidth, contentWidth);

        layoutElement.enabled = (tooltipWidth > maxWidth);
        layoutElement.preferredWidth = Mathf.Clamp(tooltipWidth, 200, maxWidth);

        float headerHeight = headerField.preferredHeight;
        float contentHeight = contentField.preferredHeight;
        layoutElement.preferredHeight = headerHeight + contentHeight + 24;
    }

    public Camera GetCanvasCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            return canvas.worldCamera;
        return null;
    }

    public void FadeIn()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        gameObject.SetActive(true);
        fadeRoutine = StartCoroutine(FadeTo(1f, fadeInDuration, disableOnEnd: false));
    }

    public void FadeOut()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        gameObject.SetActive(true); // Activo para que pueda hacer fade
        fadeRoutine = StartCoroutine(FadeTo(0f, fadeOutDuration, disableOnEnd: true));
    }

    private System.Collections.IEnumerator FadeTo(float targetAlpha, float duration, bool disableOnEnd)
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        if (disableOnEnd && targetAlpha == 0f)
            gameObject.SetActive(false);
    }

    public void SetTextAndShow(string header, string content)
    {
        headerField.text = header;
        headerField.gameObject.SetActive(!string.IsNullOrEmpty(header));
        contentField.text = content;

        if (adjustRoutine != null)
            StopCoroutine(adjustRoutine);
        // Empezar invisible
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
        adjustRoutine = StartCoroutine(DelayedAdjustAndShow());
    }

    private System.Collections.IEnumerator DelayedAdjustAndShow()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        AdjustSize();
        // Ahora que el tamaño es correcto, mostrar con fade
        if (canvasGroup != null) FadeIn();
        adjustRoutine = null;
    }

}