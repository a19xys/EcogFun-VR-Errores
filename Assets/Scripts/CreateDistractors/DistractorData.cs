using System;
using System.Collections.Generic;

[Serializable]
public class SessionData {
    public string id;
    public string sessionName;
    public string createdAt;
    public List<DistractorData> distractors;
}

[Serializable]
public class DistractorData {
    public string id;
    public string scene;
    public string distractorType;
    public ActivationBlock activation;
    public ActivationBlock deactivation;
}

[Serializable]
public class ActivationBlock {
    public List<Step> steps;
}

[Serializable]
public class Step {
    public string type;
    public Dictionary<string, object> @params;
}

[System.Serializable]
public class EscenaDistractorData
{
    public string escena;
    public string distractor;
}

[System.Serializable]
public class ActivadorData
{
    public string tipo; // "accion", "tiempo", "aciertos", "efectividad"
    public string accionElegida;
    public string objetoElegido;
    public int aciertos;
    public int rate;
    public int time;
}

[System.Serializable]
public class DesactivadorData
{
    public string tipo; // "ninguno", "accion", "tiempo"
    public string accionElegida;
    public string objetoElegido;
    public int time;
}