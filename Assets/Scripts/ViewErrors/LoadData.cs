using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadData : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public VideoController videoController; // Referencia a VideoController.
    public NotesController notesController; // Referencia a NotesController.
    public SceneInitializer sceneInitializer; // Referencia a SceneInitializer.

    // PÚBLICO //
    public Transform contentPanel; // Contenido del ScrollView.
    public GameObject listItemPrefab; // Referencia al prefab del elemento de la lista.
    public GameObject button_type; // Botón que muestra el tipo de error.
    public TextMeshProUGUI text_type; // Tipo del error a mostrar.

    // PÚBLICO - OCULTO EN EL INSPECTOR //
    [HideInInspector]
    public int[] actionCounts = new int[5]; // Array para almacenar el conteo de cada tipo de acción.

    [HideInInspector]
    public int[] correctCounts = new int[2]; // Array para almacenar el conteo de cada tipo de acción (aciertos y fallos).
    
    [HideInInspector]
    public List<Button> buttons = new List<Button>(); // Array para almacenar los botones.

    // PÚBLICO - SERIALIZABLE //
    [System.Serializable] // Clase para deserializar el JSON.
    public class ActionData { public string tipo; public string accion; public string objeto; public float tiempo; public string nota; }

    [System.Serializable] // Clase para deserializar la lista de acciones del JSON.
    public class ActionList { public ActionData[] list; }

    // PRIVADO //
    private List<Outline> outlines = new List<Outline>(); // Array para almacenar los outlines de los botones.
    private Color defaultColor = Color.black; // Color del borde del botón por defecto.
    private Color selectedColor = Color.yellow; // Color del borde cuando el botón está seleccionado.
    private int selectedButtonIndex; // Índice del botón actualmente seleccionado.

    public void AwakeLD() {

        selectedButtonIndex = -1;

        string json = File.ReadAllText(sceneInitializer.GetErrors()); // Cargar el archivo JSON.
        ActionList actionList = JsonUtility.FromJson<ActionList>(json); // Deserializar el JSON en una lista de acciones.
        CountActions(actionList.list); // Contar las acciones de cada tipo.
        ScrollView(actionList.list); // Llenar la lista con los elementos del JSON.

        StartCoroutine(videoController.ToggleBgAfterDelay(true, 0.0f));

    }

    // Método para contar las acciones de cada tipo.
    void CountActions(ActionData[] actions) {

        // Vaciar listas, por si estuviesen con datos
        for (int i = 0; i < actionCounts.Length; i++) { actionCounts[i] = 0; }
        for (int i = 0; i < correctCounts.Length; i++) { correctCounts[i] = 0; }

        foreach (ActionData action in actions) {
            switch (action.tipo) {
                case "Correcto": correctCounts[0]++; break;
                case "Comision": actionCounts[0]++; correctCounts[1]++; break;
                case "Orden": actionCounts[1]++; correctCounts[1]++; break;
                case "Romper las normas": actionCounts[2]++; correctCounts[1]++; break;
                case "Omision": actionCounts[3]++; correctCounts[1]++; break;
                case "Repeticion": actionCounts[4]++; correctCounts[1]++; break;
                default: Debug.LogWarning("Tipo de acción no reconocido: " + action.tipo); break;
            }
        }

        Debug.Log("Conteo de acciones: " + string.Join(", ", actionCounts)); // Debug para mostrar el conteo de acciones.
        Debug.Log("Conteo de correctos: " + string.Join(", ", correctCounts)); // Debug para mostrar el conteo de acciones.

    }

    // Método para llenar la lista con los elementos del JSON.
    void ScrollView(ActionData[] actions) {

        // Limpiar el contenido del scroll view al inicio, en caso de que haya elementos.
        foreach (Transform child in contentPanel) { Destroy(child.gameObject); }
        buttons.Clear();
        outlines.Clear();

        int i = 0;
        int n = 0;

        foreach (ActionData action in actions) {

            if (action.tipo != "Correcto") {

                // Instanciar un nuevo elemento de la lista a partir del prefab.
                GameObject newListItem = Instantiate(listItemPrefab);
                newListItem.transform.SetParent(contentPanel, false);
                newListItem.name = "button_error_" + i;

                // Introducir el botón y su borde en sus respectivas listas.
                Button button = newListItem.GetComponent<Button>();
                Outline outline = newListItem.GetComponent<Outline>();
                buttons.Add(button);
                outlines.Add(outline);

                // Obtener referencias a los textos e imágenes del nuevo elemento.
                TextMeshProUGUI[] textFields = newListItem.GetComponentsInChildren<TextMeshProUGUI>();
                RawImage noteImage = newListItem.GetComponentInChildren<RawImage>();
                Image imageComponent = newListItem.GetComponent<Image>();

                // Mostrar la información de los botones dependiendo de las acciones del JSON.
                if (action.tipo == "Comision") { imageComponent.color = new Color32(255,175,155,255); }
                else if (action.tipo == "Orden") { imageComponent.color = new Color32(240,90,75,255); }
                else if (action.tipo == "Romper las normas") { imageComponent.color = new Color32(180,50,90,255); }
                else if (action.tipo == "Omision") { imageComponent.color = new Color32(150,10,40,255); textFields[1].text = "(acción omitida)"; }
                else if (action.tipo == "Repeticion") { imageComponent.color = new Color32(90,0,24,255); }

                if (action.tipo != "Omision") { textFields[1].text = CalculateTime(action.tiempo); }
                textFields[0].text = action.accion + " " + action.objeto;

                // Desactivar imágenes donde no haya notas.
                if (action.nota == "") { noteImage.enabled = false; }

                // Agregar eventos al seleccionar los botones.
                int index = i;
                int number = n;
                button.onClick.AddListener(() => OnButtonClick(index, number, textFields[0].text, action.tipo, imageComponent));
                if (action.tipo != "Omision") { button.onClick.AddListener(() => videoController.PlayFromTo(action.tiempo));
                } else { button.onClick.AddListener(() => videoController.Omision()); }

                i++;

            }

            n++;

        }

    }

    // Método para ejecutar lo que sucede al pulsar uno de los botones con errores.
    void OnButtonClick(int index, int number, string error_name, string error_type, Image error_color) {

        if (error_type != "Omision") { StartCoroutine(videoController.ToggleBgAfterDelay(false, 0.35f)); }

        if (selectedButtonIndex >= 0) { outlines[selectedButtonIndex].effectColor = defaultColor; } // Revertir el color del botón previamente seleccionado.
        outlines[index].effectColor = selectedColor; // Establecer el color del botón recién seleccionado.
        selectedButtonIndex = index; // Actualizar el índice del botón seleccionado.
        notesController.DisplayCurrentNote(index, number, error_name);

        text_type.text = "Error de tipo «" + error_type + "»";
        Image color_type = button_type.GetComponent<Image>();
        color_type.color = error_color.color;

    }

    // Método para convertir el tiempo en segundos en un string con el formato "0min 0s".
    public string CalculateTime(float timeElapsed) {

        int minutes = Mathf.FloorToInt(timeElapsed / 60); 
        int seconds = Mathf.FloorToInt(timeElapsed % 60);
        return string.Format("{0:0}min {1:0}s", minutes, seconds);

    }

}
