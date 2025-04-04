using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PiechartController : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public LoadData loadData;

    // PÚBLICO //
    public Image[] imagesPieChart;
    public TextMeshProUGUI[] textCounter;
    // piechart_1 -> 0: Correctas | 1: Comisión | 2: Orden | 3: Normas | 4: Omisión | 5: Repetición
    // piechart_2 -> 0: Aciertos | 1: Fallos

    // PRIVADO //
    private int[] counts = new int[5];
    private float[] listPercentage = new float[5];
    private bool allCompleted = false;

void Update() {

    // Animación para rellenar el gráfico.
    if (!allCompleted) {
        for (int i = 0; i < imagesPieChart.Length; i++) {
            float currentFillAmount = imagesPieChart[i].fillAmount;
            imagesPieChart[i].fillAmount = Mathf.Min(currentFillAmount + 0.011f, listPercentage[i]);
        }
    }

    allCompleted = imagesPieChart.Select((image, index) => image.fillAmount == listPercentage[index]).All(isFilled => isFilled);

}

    public void StartPC() {

        for (int i = 0; i < imagesPieChart.Length; i++) { imagesPieChart[i].fillAmount = 0.0f; }
        allCompleted = false;

        GameObject pieChart = this.gameObject;

        //for(int i=0; i < counts.Length; i++) { counts[i] = Random.Range(0, 50); } // Lista aleatoria.

        // Obtenemos la cantidad de eventos de cada tipo.
        if (pieChart.name == "piechart_1") {

            counts = loadData.actionCounts; // Lectura del JSON para dividir entre seis tipos de acciones.

            string[] singularTexts = { "comisión", "error de orden", "rotura de normas", "omisión", "repetición" };
            string[] pluralTexts = { "comisiones", "errores de orden", "roturas de normas", "omisiones", "repeticiones" };

            // Textos que muestran el conteo de los eventos.
            for (int i = 0; i < counts.Length; i++) {
                textCounter[i].text = counts[i] == 1
                    ? $"1 {singularTexts[i]}"
                    : $"{counts[i]} {pluralTexts[i]}";
            }

        } else {
        
            counts = loadData.correctCounts; // Lectura del JSON para dividir entre aciertos y fallos.

            string[] singularTexts = { "acierto", "fallo"};
            string[] pluralTexts = { "aciertos", "fallos"};

            // Textos que muestran el conteo de los eventos.
            for (int i = 0; i < counts.Length; i++) {
                textCounter[i].text = counts[i] == 1
                    ? $"1 {singularTexts[i]}"
                    : $"{counts[i]} {pluralTexts[i]}";
            }

        }

        // Y calculamos el porcentaje que deberá rellenar cada trozo del gráfico.
        SetValues(counts);
        
    }

    void SetValues(int[] values) {

        float totalAmount = 0; // Cantidad de eventos existentes.
        float totalPercentage = 0; // Porcentaje total entre los eventos acumulados (máximo 1.0f).

        for(int i = 0; i < imagesPieChart.Length; i++) { totalAmount += values[i]; }
        
        for(int i = 0; i < imagesPieChart.Length; i++) {
            totalPercentage += values[i] / totalAmount;
            listPercentage[i] = totalPercentage;
        }

    }

}
