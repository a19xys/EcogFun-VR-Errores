using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Linq;
using SimpleFileBrowser;

public class VisualizadorErrores : MonoBehaviour {

    public FileManager file_manager;

    public void cargaJson() { StartCoroutine(file_manager.CargarJsonCorrutina(this)); }

}
