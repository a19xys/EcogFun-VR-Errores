using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using Newtonsoft.Json;

public class SceneInitializer : MonoBehaviour {

    public FileManager file_manager;
    public LoadData load_data;
    public PiechartController piechart_controller1, piechart_controller2;
    public NotesController notes_controller;
    public VideoController video_controller;

    public VideoPlayer videoPlayer1;
    public VideoPlayer videoPlayer2;

    private string basePath;
    private string video1Path;
    private string video2Path;
    private string erroresJsonPath;
    private string jsonContent;

    void Start() {
    
        video_controller.SetInactive();
    
        // Determinar la ruta de la carpeta donde están los archivos (y crearla si no existe).
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EcogFun-VR");
        
        if (!Directory.Exists(basePath)) {
            Directory.CreateDirectory(basePath);
            Debug.Log($"Directorio creado en: {basePath}");
        }

    }

    public void LoadJson(FileManager.FileNameConfig fileNames) {

        // Determinar la localización de los archivos deseados.
        video1Path = Path.Combine(basePath, fileNames.video_file_names[0]);
        video2Path = Path.Combine(basePath, fileNames.video_file_names[1]);
        erroresJsonPath = Path.Combine(basePath, fileNames.errores_file_name);

        if (string.IsNullOrEmpty(video1Path) || string.IsNullOrEmpty(video2Path) || string.IsNullOrEmpty(erroresJsonPath)) {
            Debug.LogError("Una o más rutas de archivos son inválidas. Verifica el JSON de configuración.");
            return;
        }

        // Verificar y cargar los archivos.
        LoadResources();

    }

    private void LoadResources() {

        try {

            // Cargar el primer vídeo.
            if (File.Exists(video1Path)) {
                videoPlayer1.url = video1Path;
                Debug.Log($"Vídeo 1 cargado correctamente desde: {video1Path}");
            } else { Debug.LogWarning($"El archivo de vídeo 1 no existe en la ruta: {video1Path}"); }

            // Cargar el segundo vídeo.
            if (File.Exists(video2Path)) {
                videoPlayer2.url = video2Path;
                Debug.Log($"Vídeo 2 cargado correctamente desde: {video2Path}");
            } else { Debug.LogWarning($"El archivo de vídeo 2 no existe en la ruta: {video2Path}"); }

            // Comprobación de carga exitosa del archivo JSON de errores.
            if (File.Exists(erroresJsonPath)) { Debug.Log("Archivo JSON de errores cargado correctamente."); }
            else { Debug.LogWarning($"El archivo JSON de errores no existe en la ruta: {erroresJsonPath}"); }

        } catch (Exception e) { Debug.LogError($"Error al cargar los recursos: {e.Message}"); }

        StartProgram();

    }

    // Lanzamiento secuencial de comportamientos del visualizador de errores.
    private void StartProgram() {

        load_data.AwakeLD();
        piechart_controller1.StartPC();
        piechart_controller2.StartPC();
        notes_controller.StartNC(erroresJsonPath);
        video_controller.StartVC();

    }

    // Getter para acceder al contenido del JSON de errores.
    public string GetErrors() { return erroresJsonPath; }

}
