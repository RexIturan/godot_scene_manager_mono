#if TOOLS
using System;
using System.Runtime.Loader;
using Godot;
using SceneManagerMono.Util;

[Tool]
public partial class ResourcePathDisplay : Container {
    [Export] private FileInput fileInput;

    ///// Editor Plugin Helper /////

    private Action<AssemblyLoadContext> unloadHandle;
    
    ///// Godot Functions /////
    
    public override void _EnterTree() {
        base._EnterTree();

        fileInput ??= GetNode<FileInput>("File Input");
        
        ProjectSettings.Singleton.TryConnect(ProjectSettings.SignalName.SettingsChanged, OnProjectSettingsChanged);
        unloadHandle = UnloadHelper.RegisterUnload(Cleanup);
    }

    public override void _Ready() {
        base._Ready();

        UpdateResourcePath();
    }

    public override void _ExitTree() {
        base._ExitTree();

        Cleanup();
    }

    private void Cleanup() {
        UnloadHelper.UnregisterUnload(unloadHandle);
        
        if(!IsInstanceValid(this)) return;
        
        ProjectSettings.Singleton.TryDisconnect(ProjectSettings.SignalName.SettingsChanged, OnProjectSettingsChanged);
    }

    ///// Private Functions /////
    
    private void OnProjectSettingsChanged() {
        UpdateResourcePath();
    }

    private void UpdateResourcePath() {
        fileInput.Text = SceneManagerSettings.GetSceneListPath();
    }
}
#endif
