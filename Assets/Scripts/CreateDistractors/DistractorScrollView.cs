using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DistractorScrollView : MonoBehaviour {

    public Transform contentPanel;
    public GameObject distractorItemPrefab;
    public GameObject textoNoHayDistractores;

    public Color colorNormal;
    public Color colorSeleccionado;

    private List<DistractorData> distractoresActuales;
    private List<GameObject> distractorItems = new List<GameObject>();
    private int indiceSeleccionado = -1;

    private CreadorDistractores managerReferencia;
    private bool esModoEditor;

    // Llamar con la lista de distractores cargados
    public void MostrarDistractores(List<DistractorData> distractores, bool modoEditor, CreadorDistractores manager) {
        // Guardamos referencias
        esModoEditor = modoEditor;
        managerReferencia = manager;
        distractorItems.Clear();
        indiceSeleccionado = -1;
        distractoresActuales = distractores;

        // Limpiar scrollview
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        // Mostrar u ocultar el texto "No hay distractores"
        bool hayDistractores = distractores != null && distractores.Count > 0;
        if (textoNoHayDistractores != null)
            textoNoHayDistractores.SetActive(!hayDistractores);

        if (!hayDistractores)
            return;

        // Calculo de la numeracion con diccionario
        Dictionary<string, int> tipoCounter = new Dictionary<string, int>();

        for (int i = 0; i < distractores.Count; i++)
        {
            var distractor = distractores[i];
            string tipo = distractor.distractorType;
            if (!tipoCounter.ContainsKey(tipo))
                tipoCounter[tipo] = 1;
            else
                tipoCounter[tipo] += 1;
            int numero = tipoCounter[tipo];

            GameObject item = Instantiate(distractorItemPrefab, contentPanel);
            distractorItems.Add(item);

            // --- TEXTOS ---
            TextMeshProUGUI texto1 = item.transform.Find("text_1").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI texto2 = item.transform.Find("text_2").GetComponent<TextMeshProUGUI>();

            string nombreBonito = PrimeraLetraMayus(distractor.distractorType.Replace("_", " "));
            texto1.text = $"{nombreBonito} {numero:00}";
            string escenaBonita = PrimeraLetraMayus(distractor.scene.Replace("_", " "));
            string tipoActivacion = FormateaTipoTrigger(distractor.activation);
            string tipoDesactivacion = FormateaTipoTrigger(distractor.deactivation);
            texto2.text = $"{escenaBonita} · {tipoActivacion} · {tipoDesactivacion}";

            // --- INTERACTIVIDAD EN MODO EDITOR ---
            Button btn = item.GetComponent<Button>();
            Image img = item.GetComponent<Image>();
            if (img != null) img.color = colorNormal; // Reset color

            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.interactable = esModoEditor;
                if (esModoEditor)
                {
                    int idx = i;
                    btn.onClick.AddListener(() => OnSelectDistractor(idx));
                }
            }

            DistractorItemUI ui = item.GetComponent<DistractorItemUI>();
            if (ui != null) {
                ui.Inicializar(distractor, manager);
                ui.SetInteractable(esModoEditor);
            }
        }
    }

    public void OnSelectDistractor(int idx) {
        for (int i = 0; i < distractorItems.Count; i++) {
            Image img = distractorItems[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == idx) ? colorSeleccionado : colorNormal;
        }
        indiceSeleccionado = idx;

        // Notificar al manager para que muestre la info en los bloques
        if (managerReferencia != null && distractoresActuales != null && idx >= 0 && idx < distractoresActuales.Count)
            managerReferencia.OnDistractorSeleccionado(distractoresActuales[idx]);
    }

    public void DeseleccionarDistractor() {
        indiceSeleccionado = -1;
        foreach (var item in distractorItems) {
            Image img = item.GetComponent<Image>();
            if (img != null)
                img.color = colorNormal;
        }
    }

    // Devuelve el nombre formateado del primer trigger
    private string FormateaTipoTrigger(ActivationBlock block) {
        if (block == null || block.steps == null || block.steps.Count == 0)
            return "Ninguno";
        string tipo = block.steps[0].type;
        switch (tipo) {
            case "action": return "Acción";
            case "time": return "Tiempo";
            case "success_count": return "Aciertos";
            case "success_rate": return "Efectividad";
            default: return PrimeraLetraMayus(tipo.Replace("_", " "));
        }
    }

    private string PrimeraLetraMayus(string input) {
        if (string.IsNullOrEmpty(input)) return "";
        input = input.ToLower();
        return char.ToUpper(input[0]) + input.Substring(1);
    }

}