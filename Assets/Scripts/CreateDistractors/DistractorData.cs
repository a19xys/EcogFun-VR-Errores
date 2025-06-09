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