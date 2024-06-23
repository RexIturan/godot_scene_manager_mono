using Godot;
using Godot.Collections;

// [Tool]
public static class SceneManagerSettings {
    public const string SETTINGS_PROPERTY_NAME = "scene_manager_mono/scenes/scenes_path";
    public const string DEFAULT_PATH_TO_SCENE_LIST = "res://addons/scene_manager_mono/data/sceneList.tres";
    
    ///// Public Functions /////
    
    public static void InitializeProjectSettings() {
        if (!ProjectSettings.HasSetting(SETTINGS_PROPERTY_NAME)) {
            ProjectSettings.SetSetting(SETTINGS_PROPERTY_NAME, DEFAULT_PATH_TO_SCENE_LIST);
            ProjectSettings.Save();
        }

        UpdateSettingsConfig();
    }

    public static string GetSceneListPath() {
        return (string) ProjectSettings.GetSetting(SETTINGS_PROPERTY_NAME, DEFAULT_PATH_TO_SCENE_LIST);
    }
    
    public static void SetSceneListPath(string path) {
        ProjectSettings.SetSetting(SETTINGS_PROPERTY_NAME, path);
        ProjectSettings.Save();
    }
    
    ///// Private Functions /////

    private static void UpdateSettingsConfig() {
        ProjectSettings.SetAsBasic(SETTINGS_PROPERTY_NAME, true);
        ProjectSettings.SetRestartIfChanged(SETTINGS_PROPERTY_NAME, true);
        ProjectSettings.SetInitialValue(SETTINGS_PROPERTY_NAME, DEFAULT_PATH_TO_SCENE_LIST);
        ProjectSettings.AddPropertyInfo(new Dictionary() {
            {"name", SETTINGS_PROPERTY_NAME},
            {"type", (int)Variant.Type.String},
            {"hint", (int)PropertyHint.File},
            {"hint_string", "*.tres"},
        });
        ProjectSettings.Save();
    }
}
