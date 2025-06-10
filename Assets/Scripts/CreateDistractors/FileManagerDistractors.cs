using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using SimpleFileBrowser;

public class FileManagerDistractors : MonoBehaviour {

    // Almacena la sesion actualmente cargada
    public SessionData currentSession;
    // Guarda el path del archivo al cargarlo
    public string currentFilePath;

    private CreadorDistractores creadorDistractores;

    public IEnumerator CargarSesionCoroutine(CreadorDistractores caller) {
        // Configurar filtros
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Archivos JSON", ".json"));
        FileBrowser.SetDefaultFilter(".json");

        // Espera a que el usuario seleccione un archivo
        yield return FileBrowser.WaitForLoadDialog(
            FileBrowser.PickMode.Files, false, null, null,
            "Cargar archivo de sesion (.JSON)", "Cargar"
        );

        if (FileBrowser.Success) {
            string filePath = FileBrowser.Result[0];
            string jsonText = File.ReadAllText(filePath);

            // Deserializa como SessionData
            SessionData session = JsonConvert.DeserializeObject<SessionData>(jsonText);

            // Si no hay distractors, créalo como lista vacía
            if (session.distractors == null)
                session.distractors = new List<DistractorData>();

            // Guarda la sesion y el path
            currentSession = session;
            currentFilePath = filePath;

            // Callback a UI
            caller.OnSessionLoaded(currentSession, Path.GetFileName(filePath));
        }
        yield return null;
    }

    public void GuardarSesionEnArchivo() {
        if (currentSession == null || string.IsNullOrEmpty(currentFilePath)) return;

        string jsonString = JsonConvert.SerializeObject(currentSession, Formatting.Indented);
        File.WriteAllText(currentFilePath, jsonString);
        Debug.Log("Sesión guardada en: " + currentFilePath);
    }

}