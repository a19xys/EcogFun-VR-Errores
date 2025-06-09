using System.Collections;
using UnityEngine;
using TMPro;

public class CreadorDistractores : MonoBehaviour
{
    public FileManagerDistractors fileManager;
    public DistractorScrollView distractorScrollView;
    public TextMeshProUGUI cargarSesionButtonText;
    public GameObject[] blocks_init;

    // Pulsando "Cargar sesion"
    public void CargarSesion()
    {
        StartCoroutine(fileManager.CargarSesionCoroutine(this));
    }

    // Cuando la sesion y los distractores estan listos
    public void OnSessionLoaded(SessionData session)
    {
        // Actualiza la UI con la sesion cargada
        distractorScrollView.MostrarDistractores(session.distractors);

        // Cambia el texto del boton al ID de la sesion
        if (cargarSesionButtonText != null && session != null)
        {
            cargarSesionButtonText.text = session.id;
        }

        // Desactiva los GameObjects asignados
        if (blocks_init != null)
        {
            foreach (GameObject go in blocks_init)
            {
                if (go != null) go.SetActive(false);
            }
        }
    }
}