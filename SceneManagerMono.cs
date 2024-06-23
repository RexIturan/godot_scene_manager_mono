#if TOOLS
using Godot;

namespace SceneManagerMono;

[Tool]
public partial class SceneManagerMono : EditorPlugin {
	private SceneManagerEditor sceneManagerEditor;
	private Control sceneQuickAccessControl;

	private const string SCENE_MANAGER_MENU_NAME = "Scene Manager";
	private SceneManagerWindow sceneManagerEditorWindow;
	
	public override void _EnterTree() {
		// Initialization of the plugin goes here.

		SceneManagerSettings.InitializeProjectSettings();
		
		AddAutoloadSingleton("SceneManager", "res://addons/scene_manager_mono/runtime/SceneManager.tscn");

		CreateSceneManagerSingelton();

		///// Scenemanager Quick Access /////
		/// 
		sceneQuickAccessControl = GodotHelper.LoadAndInstaniate<Control>(
			"res://addons/scene_manager_mono/editor/quick_access/scene_quick_access.tscn");
		sceneQuickAccessControl.Name = "Scene Manager Quick Access";
		AddControlToDock(DockSlot.RightBr, sceneQuickAccessControl);
		
		
		///// Scenemanager Editor Window /////
		
		//add window to editor interface
		CreateAndAttachEditorWindow();
		AddToolMenuItem(SCENE_MANAGER_MENU_NAME, Callable.From(ToggleEditorWindow));
	}

	public override void _ExitTree() {
		// Clean-up of the plugin goes here.
		RemoveAutoloadSingleton("SceneManager");
		
		///// Scenemanager Quick Access /////
		
		RemoveControlFromDocks(sceneQuickAccessControl);
		sceneQuickAccessControl.QueueFree();

		
		///// Scenemanager Editor Window /////
		 
		EditorInterface.Singleton.GetBaseControl().RemoveChild(sceneManagerEditorWindow);
		sceneManagerEditorWindow.QueueFree();
		RemoveToolMenuItem(SCENE_MANAGER_MENU_NAME);
		
		RemoveSceneManagerSingelton();
		
		//todo find all scene data editor windows and close them
	}

	public override void _SaveExternalData() {
		// base._SaveExternalData();
		
		sceneManagerEditor.Save();
	}

	// ##### Private Functions #####
	
	private void CreateSceneManagerSingelton() {
		sceneManagerEditor = GodotHelper.LoadAndInstaniate<SceneManagerEditor>(
			"res://addons/scene_manager_mono/editor/scene_manager_editor.tscn");
		EditorInterface.Singleton.GetBaseControl().AddChild(sceneManagerEditor);
	}

	private void RemoveSceneManagerSingelton() {
		EditorInterface.Singleton.GetBaseControl().RemoveChild(sceneManagerEditor);
		sceneManagerEditor.QueueFree();
	}

	private void CreateAndAttachEditorWindow() {
		sceneManagerEditorWindow = GodotHelper.LoadAndInstaniate<SceneManagerWindow>(
			"res://addons/scene_manager_mono/editor/scene_manager_window.tscn");
		sceneManagerEditorWindow.Hide();
		EditorInterface.Singleton.GetBaseControl().AddChild(sceneManagerEditorWindow);
	}
	
	private void ToggleEditorWindow() {
		sceneManagerEditorWindow.Show();
	}
}
#endif
