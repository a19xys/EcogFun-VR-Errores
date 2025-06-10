using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DistractorScrollView : MonoBehaviour {

    public GameObject distractorItemPrefab;
    public Transform contentPanel;
    public GameObject textoNoHayDistractores;

    // Llamar con la lista de distractores cargados
    public void MostrarDistractores(List<DistractorData> distractores) {

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

        foreach (var distractor in distractores) {
            string tipo = distractor.distractorType;
            if (!tipoCounter.ContainsKey(tipo))
                tipoCounter[tipo] = 1;
            else
                tipoCounter[tipo] += 1;
            int numero = tipoCounter[tipo];

            GameObject item = Instantiate(distractorItemPrefab, contentPanel);
            TextMeshProUGUI texto1 = item.transform.Find("text_1").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI texto2 = item.transform.Find("text_2").GetComponent<TextMeshProUGUI>();

            string nombreBonito = PrimeraLetraMayus(distractor.distractorType.Replace("_", " "));
            texto1.text = $"{nombreBonito} {numero:00}";

            string escenaBonita = PrimeraLetraMayus(distractor.scene.Replace("_", " "));
            string tipoActivacion = FormateaTipoTrigger(distractor.activation);
            string tipoDesactivacion = FormateaTipoTrigger(distractor.deactivation);

            texto2.text = $"{escenaBonita} · {tipoActivacion} · {tipoDesactivacion}";
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