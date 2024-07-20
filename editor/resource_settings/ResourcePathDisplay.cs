#if TOOLS
using System;
using System.Runtime.Loader;
using Godot;
using SceneManagerMono.Editor.Util;
using SceneManagerMono.Util;

namespace SceneManagerMono.Editor.Resource_Settings;

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
        // GD.Print("[ResourcePathDisplay] _ExitTree");
        
        base._ExitTree();
        Cleanup();
    }

    protected override void Dispose(bool disposing) {
        // GD.Print("[ResourcePathDisplay] Dispose");
        Cleanup();
        base.Dispose(disposing);
    }

    private void Cleanup() {
        // GD.Print("[ResourcePathDisplay] Cleanup");
        
        UnloadHelper.UnregisterUnload(unloadHandle);
        
        if (!IsInstanceValid(this)) return;
        if (!IsInstanceValid(ProjectSettings.Singleton)) return;
        
        ProjectSettings.Singleton.TryDisconnect(ProjectSettings.SignalName.SettingsChanged, OnProjectSettingsChanged);
    }

    ///// Callback Functions /////
    
    private void OnProjectSettingsChanged() {
        UpdateResourcePath();
    }

    ///// Private Functions /////
    
    private void UpdateResourcePath() {
        fileInput.Text = SceneManagerSettings.GetSceneListPath();
    }
}
#endif
