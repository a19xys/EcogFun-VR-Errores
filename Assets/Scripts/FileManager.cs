using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using SimpleFileBrowser;

public class FileManager : MonoBehaviour {

    [Serializable]
    public class FileNameConfig {
        public string[] video_file_names;
        public string execution_file_name;
        public string errores_file_name;
    }

    public SceneInitializer scene_initializer;
    public bool browser_abierto = false;

    // SIMPLE FILE BROWSER: https://github.com/yasirkula/UnitySimpleFileBrowser
    public IEnumerator CargarJsonCorrutina(VisualizadorErrores visualizador) {

        // Configurar filtro para archivos JSON
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Archivos JSON", ".json"));

        // Configurar para no permitir selección múltiple
        FileBrowser.SetDefaultFilter(".json");

        // Mostrar el diálogo de selección de archivo
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Cargar archivo de sesión (.JSON)", "Cargar");
        browser_abierto = true;

        if (FileBrowser.Success) {

            // Parsear el JSON para obtener los nombres de los archivos
            FileNameConfig fileNames;
            fileNames = JsonConvert.DeserializeObject<FileNameConfig>(File.ReadAllText(FileBrowser.Result[0]));
            scene_initializer.LoadJson(fileNames);

        }

        yield return null;

    }

}
