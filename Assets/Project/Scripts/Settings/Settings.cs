using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Audio;

public class Settings : MonoBehaviour {
  [System.NonSerialized]
  public SettingsUpdatedEvent settingsUpdatedEvent = new SettingsUpdatedEvent();
  public float musicVolume;
  public float soundVolume;
  public float mouseSensitivity;
  public bool fullScreen;
  public bool invertX;
  public bool invertY;

  private string saveFilePath;

  void Awake() {
    saveFilePath = Application.persistentDataPath + "/Settings.json";
    ResetToDefaults();
    Load();
    Save();
  }

  void Start() {
    gameObject.SetActive(false);
    InvokeEvent();
  }

  public void InvokeEvent() {
    settingsUpdatedEvent.Invoke();
  }

  void Load() {
    try {
      JsonUtility.FromJsonOverwrite(
        System.IO.File.ReadAllText(saveFilePath),
        this
      );
    } catch (FileNotFoundException e) {}
    UpdateFullScreen();
  }

  public void Save() {
    System.IO.File.WriteAllText(
      saveFilePath,
      JsonUtility.ToJson(this, true)
    );
    settingsUpdatedEvent.Invoke();
  }

  public void Cancel() {
    Load();
    settingsUpdatedEvent.Invoke();
  }

  public void ResetToDefaults() {
    musicVolume = 0.5f;
    soundVolume = 0.5f;
    mouseSensitivity = 0.5f;
    fullScreen = Screen.fullScreen;
    invertX = false;
    invertY = false;
  }

  public void SetFullScreen(bool _fullScreen) {
    fullScreen = _fullScreen;
    UpdateFullScreen();
  }

  public void UpdateFullScreen() {
    Screen.fullScreen = fullScreen;
  }
}
