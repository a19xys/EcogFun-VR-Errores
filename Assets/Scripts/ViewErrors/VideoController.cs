using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoController : MonoBehaviour {

    // PÚBLICO //
    public VideoPlayer videoPlayer1, videoPlayer2; // Reproductores de vídeo.
    public GameObject background_video; // Fondos que tapan los vídeos ccuando no están disponibles.
    public Button pauseButton, fullscreenButton, pausefocusButton, repeatButton; // Botones de pausa, fullscreen y cerrado.
    public TMP_InputField lapseField; // Campo para modificar notas de los errores.
    public TextMeshProUGUI time_text, timefocus_text, lapsefocus_text, separation_text; // Referencias a los TextMeshPro.
    public RawImage background_focus; // Fondo para evitar distracciones.
    public RectTransform video1Rect, video2Rect; // RectTransform de ambos vídeos.
    public float time_lapse = 10f; // Por defecto, 10 segundos de lapso.

    [Header("Fullscreen Settings")]
    public Vector2 fullscreenSize = new Vector2(400, 225); // Tamaño en modo fullscreen.
    public Vector2 fullscreenVideo1Position = new Vector2(-400, -100); // Posición del primer vídeo.
    public Vector2 fullscreenVideo2Position = new Vector2(0, -100); // Posición del segundo vídeo.

    // PRIVADO //
    private RawImage[] pause_list, pausefocus_list;
    private float begin_time, start_time, end_time;
    private int minutes, seconds;
    private bool isPlaying = false;
    private bool isFullScreen = false;
    public bool canReplay = false;
    public bool firstLoad = true;
    private Vector2 originalSize1, originalPosition1; // Tamaño y posición original del primer vídeo.
    private Vector2 originalSize2, originalPosition2; // Tamaño y posición original del segundo vídeo.
    private float lastClickTime = 0f; // Tiempo del último clic para detectar doble clic.
    private const float doubleClickThreshold = 0.3f; // Tiempo límite para doble clic (en segundos).

    public void StartVC() {
        videoPlayer1.prepareCompleted += OnVideoPrepared;
        videoPlayer2.prepareCompleted += OnVideoPrepared;

        // Preparar vídeos y dejarlos listos.
        videoPlayer1.Prepare();
        videoPlayer2.Prepare();

        if (firstLoad) { firstLoad = false; }
        else { SetInactive(); start_time = 0.0f; end_time = time_lapse; Pause(); }

        time_text.text = $"---:---";

        pause_list = pauseButton.GetComponentsInChildren<RawImage>();
        pausefocus_list = pausefocusButton.GetComponentsInChildren<RawImage>();

        // Guardar tamaño y posición original de los vídeos.
        originalSize1 = video1Rect.sizeDelta;
        originalPosition1 = video1Rect.anchoredPosition;
        originalSize2 = video2Rect.sizeDelta;
        originalPosition2 = video2Rect.anchoredPosition;
    }

    void Update() {
        // Reproducir únicamente entre start_time y end_time.
        if (isPlaying && canReplay && (videoPlayer1.time >= end_time || videoPlayer2.time >= end_time)) {
            Pause();
            pauseButton.interactable = false; pausefocusButton.interactable = false;
        }

        if (isPlaying && !canReplay
            && (videoPlayer1.time < end_time && videoPlayer1.time >= start_time)
            && (videoPlayer2.time < end_time && videoPlayer2.time >= start_time))
            { canReplay = true; }

        // Actualizar icono de pausa.
        if (isPlaying && pause_list != null) { pause_list[0].enabled = false; pause_list[1].enabled = true; pausefocus_list[0].enabled = false; pausefocus_list[1].enabled = true; }
        else if (pause_list != null) { pause_list[0].enabled = true; pause_list[1].enabled = false; pausefocus_list[0].enabled = true; pausefocus_list[1].enabled = false; }

        // Actualizar el tiempo mostrado en pantalla.
        if (isPlaying) {
            float currentTime = (float)videoPlayer1.time;
            time_text.text = CalculateTime(currentTime);
            timefocus_text.text = time_text.text;
            lapsefocus_text.text = $"[ {CalculateTime(start_time)} / {CalculateTime(end_time)} ]";
        }

    }

    public void PlayFromTo(float init_time) {

        pauseButton.interactable = true; pausefocusButton.interactable = true;

        begin_time = init_time;
        start_time = init_time - (time_lapse / 2);
        end_time = init_time + (time_lapse / 2);

        // Comprobar si la marca inicial es negativa.
        if (start_time <= 0.0f) { start_time = 0.0f; }

        // Comprobar si la marca final sobrepasa la duración de uno de los vídeos.
        if (end_time >= videoPlayer1.length || end_time >= videoPlayer2.length) { end_time = (float)Mathf.Min((float)videoPlayer1.length, (float)videoPlayer2.length); }

        // Desactivar el bucle temporalmente si estamos fuera del rango.
        if (videoPlayer1.time >= end_time || videoPlayer2.time >= end_time) { canReplay = false; }
        else { canReplay = true; }

        //TogglePause();
        videoPlayer1.time = start_time;
        videoPlayer2.time = start_time;
        PlayVideos();
        
        pauseButton.gameObject.SetActive(true);
        repeatButton.gameObject.SetActive(true);
        separation_text.gameObject.SetActive(true);
        fullscreenButton.gameObject.SetActive(true);
        lapseField.gameObject.SetActive(true);

    }

    public void Repeat() { PlayFromTo(begin_time); }

    public void NewLapse(float new_time_lapse) {
        time_lapse = new_time_lapse;
        PlayFromTo(begin_time);
    }

    public void PlayVideos() {
        isPlaying = true;
        videoPlayer1.Play();
        videoPlayer2.Play();
    }

    public void Pause() {
        isPlaying = false;
        videoPlayer1.Pause();
        videoPlayer2.Pause();
    }

    public void TogglePause() {
        if (isPlaying) { Pause(); }
        else { PlayVideos(); }
    }

    /* ////////////////////////// */

    public void Omision() {
        Pause();
        
        pauseButton.gameObject.SetActive(false);
        repeatButton.gameObject.SetActive(false);
        separation_text.gameObject.SetActive(false);
        fullscreenButton.gameObject.SetActive(false);
        lapseField.gameObject.SetActive(false);
        StartCoroutine(ToggleBgAfterDelay(true, 0.0f));

        time_text.text = $"---:---";
    }

    public void OnVideoClick() {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;
        if (timeSinceLastClick <= doubleClickThreshold) { ToggleFullScreen(); }
    }

    public void ToggleFullScreen() {
        if (isFullScreen && canReplay) {
            // Volver a la posición original.
            video1Rect.sizeDelta = originalSize1;
            video1Rect.anchoredPosition = originalPosition1;
            video2Rect.sizeDelta = originalSize2;
            video2Rect.anchoredPosition = originalPosition2;

            background_focus.gameObject.SetActive(false);
        } else if (canReplay) {
            // Activar fullscreen.
            video1Rect.sizeDelta = fullscreenSize;
            video1Rect.anchoredPosition = fullscreenVideo1Position;
            video2Rect.sizeDelta = fullscreenSize;
            video2Rect.anchoredPosition = fullscreenVideo2Position;

            background_focus.gameObject.SetActive(true);
        }

        isFullScreen = !isFullScreen;
    }

    // Mantener GameObjects desactivados al inicio.
    public void SetInactive(){
        pauseButton.gameObject.SetActive(false);
        repeatButton.gameObject.SetActive(false);
        separation_text.gameObject.SetActive(false);
        fullscreenButton.gameObject.SetActive(false);
        lapseField.gameObject.SetActive(false);
        background_focus.gameObject.SetActive(false);
    }

    private void OnVideoPrepared(VideoPlayer source) {
        if (!isPlaying) { source.Play(); source.Pause(); }
    }

    private string CalculateTime(float currentTime) {

        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = Mathf.FloorToInt(currentTime % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    // Corrutina para activar o desactivar un GameObject con un retraso.
    public IEnumerator ToggleBgAfterDelay(bool isActive, float time_wait) {

        // Esperar 300 milisegundos.
        yield return new WaitForSeconds(time_wait);

        // Activar o desactivar el GameObject.
        background_video.SetActive(isActive);

    }

}