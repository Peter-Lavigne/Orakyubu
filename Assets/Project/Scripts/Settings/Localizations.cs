using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Audio;

public class Localizations {
  public string language_name_in_language;
  public string new_game;
  public string continue_game;
  public string settings;
  public string exit;
  public string return_to_menu;
  public string credits;
  public string full_screen;
  public string volume;
  public string sound_effects;
  public string look_sensitivity;
  public string resume;
  public string main_menu;
  public string level_select;
  public string invert_left_right;
  public string invert_up_down;
  public string reset_all_progress;
  public string save;
  public string cancel;
  public string restore_defaults;
  public string are_you_sure_reset;
  public string confirm_reset;
  public string tutorial_move_circle_1;
  public string tutorial_move_circle_2;
  public string tutorial_camera_1;
  public string tutorial_camera_2;
  public string tutorial_zoom;
  public string tutorial_reset_undo_1;
  public string tutorial_reset_undo_2;
  public string tutorial_move_cube_1;
  public string tutorial_move_cube_2;
  public string created_by_peter;
  public string playtesters;
  public string music;
  public string space_skybox;
  public string localization;
  public string translations;
  public string thank_you;

  public string GetLocalizationByName(string key) {
    switch (key) {
      case "language_name_in_language":
        return language_name_in_language;
      case "new_game":
        return new_game;
      case "continue_game":
        return continue_game;
      case "settings":
        return settings;
      case "exit":
        return exit;
      case "return_to_menu":
        return return_to_menu;
      case "credits":
        return credits;
      case "full_screen":
        return full_screen;
      case "volume":
        return volume;
      case "sound_effects":
        return sound_effects;
      case "look_sensitivity":
        return look_sensitivity;
      case "resume":
        return resume;
      case "main_menu":
        return main_menu;
      case "level_select":
        return level_select;
      case "invert_left_right":
        return invert_left_right;
      case "invert_up_down":
        return invert_up_down;
      case "reset_all_progress":
        return reset_all_progress;
      case "save":
        return save;
      case "cancel":
        return cancel;
      case "restore_defaults":
        return restore_defaults;
      case "are_you_sure_reset":
        return are_you_sure_reset;
      case "confirm_reset":
        return confirm_reset;
      case "tutorial_move_circle_1":
        return tutorial_move_circle_1;
      case "tutorial_move_circle_2":
        return tutorial_move_circle_2;
      case "tutorial_camera_1":
        return tutorial_camera_1;
      case "tutorial_camera_2":
        return tutorial_camera_2;
      case "tutorial_zoom":
        return tutorial_zoom;
      case "tutorial_reset_undo_1":
        return tutorial_reset_undo_1;
      case "tutorial_reset_undo_2":
        return tutorial_reset_undo_2;
      case "tutorial_move_cube_1":
        return tutorial_move_cube_1;
      case "tutorial_move_cube_2":
        return tutorial_move_cube_2;
      case "created_by_peter":
        return created_by_peter;
      case "playtesters":
        return playtesters;
      case "music":
        return music;
      case "space_skybox":
        return space_skybox;
      case "localization":
        return localization;
      case "translations":
        return translations;
      case "thank_you":
        return thank_you;
      default:
        return "";
    }
  }
}
