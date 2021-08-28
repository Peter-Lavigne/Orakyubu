using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour {
  public Settings settings;
  public AudioMixer music;
  public AudioMixer sounds;

  void Awake() {
    settings.settingsUpdatedEvent.AddListener(UpdateSounds);
  }

  float SettingToVolume(float setting) {
    if (setting < 0.001f) return -80f;
    if (setting < 0.2f) return setting * 132f - 30f;
    return setting * 12f - 6f;
  }

  public void UpdateSounds() {
    music.SetFloat ("volume", SettingToVolume(settings.musicVolume));
    sounds.SetFloat ("volume", SettingToVolume(settings.soundVolume));
  }
}
