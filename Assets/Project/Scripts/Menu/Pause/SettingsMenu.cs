using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
  public Settings settings;
  public GameObject musicVolumeSlider;
  public GameObject soundVolumeSlider;
  public GameObject mouseSensitivitySlider;
  public GameObject fullScreenToggle;
  public GameObject invertXToggle;
  public GameObject invertYToggle;

  public List<GameObject> gameObjectsToMoveUpInWebGl;

  void Awake() {
    settings.settingsUpdatedEvent.AddListener(CopySettings);
  }

  void Start() {
    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      fullScreenToggle.SetActive(false);
      foreach (GameObject gameObject in gameObjectsToMoveUpInWebGl) {
        gameObject.transform.localPosition = new Vector3(
          gameObject.transform.localPosition.x,
          gameObject.transform.localPosition.y + 10f,
          gameObject.transform.localPosition.z
        );
      }
    }
    CopySettings();
  }

  void CopySettings() {
    musicVolumeSlider.GetComponent<Slider>().value = settings.musicVolume;
    soundVolumeSlider.GetComponent<Slider>().value = settings.soundVolume;
    mouseSensitivitySlider.GetComponent<Slider>().value = settings.mouseSensitivity;
    fullScreenToggle.GetComponent<Toggle>().isOn = settings.fullScreen;
    invertXToggle.GetComponent<Toggle>().isOn = settings.invertX;
    invertYToggle.GetComponent<Toggle>().isOn = settings.invertY;
  }

  public void ResetDefaults() {
    settings.ResetToDefaults();
    CopySettings();
  }

  public void onMusicVolumeChanged(float volume) {
    settings.musicVolume = volume;
    settings.InvokeEvent();
  }

  public void onSoundVolumeChanged(float volume) {
    settings.soundVolume = volume;
    settings.InvokeEvent();
  }

  public void onMouseSensitivityChanged(float sensitivity) {
    settings.mouseSensitivity = sensitivity;
  }

  public void onFullScreenChanged(bool fullScreen) {
    settings.SetFullScreen(fullScreen);
  }

  public void onInvertXChanged(bool invert) {
    settings.invertX = invert;
  }

  public void onInvertYChanged(bool invert) {
    settings.invertY = invert;
  }
}
