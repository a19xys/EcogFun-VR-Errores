using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputHandler : MonoBehaviour {

    // PÚBLICO - SCRIPTS //
    public VideoController videoController;

    // PÚBLICO //
    public TMP_InputField inputField, inputfocusField;
    public Button submitButton, submitfocusButton;

    void Start() {
        submitButton.onClick.AddListener(OnSubmit);
        submitfocusButton.onClick.AddListener(OnSubmitFocus);
    }

    void OnSubmit() {

        string inputText = inputField.text; // Toma la entrada del Input Field.
        int inputNumber; // Convierte el texto de la entrada en un número entero.

        if (int.TryParse(inputText, out inputNumber)) {
            videoController.NewLapse(inputNumber);
            inputfocusField.text = inputText;
        } else { Debug.Log("Invalid input (NO FOCUS). Please enter a valid number."); }

    }

    void OnSubmitFocus() {

        string inputfocusText = inputfocusField.text; // Toma la entrada del Input Field.
        int inputfocusNumber; // Convierte el texto de la entrada en un número entero.

        if (int.TryParse(inputfocusText, out inputfocusNumber)) {
            videoController.NewLapse(inputfocusNumber);
            inputField.text = inputfocusText;
        } else { Debug.Log("Invalid input (FOCUS). Please enter a valid number."); }

    }

}
