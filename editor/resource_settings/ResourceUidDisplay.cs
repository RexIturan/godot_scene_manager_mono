#if TOOLS
using System;
using System.Runtime.Loader;
using Godot;
using SceneManagerMono.Util;

namespace SceneManagerMono.Editor.Resource_Settings;

[Tool]
public partial class ResourceUidDisplay : Container {
    private LineEdit FileInputLong => GetNode<LineEdit>("%UID long");
    private LineEdit FileInputString => GetNode<LineEdit>("%UID string");

    ///// Editor Plugin Helper /////

    private Action<AssemblyLoadContext> unloadHandle;
        
    ///// Godot Functions /////
    
    public override void _Ready() {
        base._Ready();

        SetUids();
        
        ProjectSettings.Singleton.TryConnect(ProjectSettings.SignalName.SettingsChanged, OnProjectSettingsChanged);
        unloadHandle = UnloadHelper.RegisterUnload(Cleanup);
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
    
    ///// Callback Functions /////
    
    private void OnProjectSettingsChanged() {
        SetUids();
    }

    ///// Private Functions /////
    
    private void SetUids() {
        var path = SceneManagerSettings.GetSceneListPath();
        
        if (ResourceLoader.Exists(path)) {
            var uid = ResourceLoader.GetResourceUid(SceneManagerSettings.GetSceneListPath());
            FileInputLong.Text = uid.ToString();
            FileInputString.Text = ResourceUid.IdToText(uid);
        }
        else {
            FileInputLong.Text = "";
            FileInputString.Text = "";
        }
    }
}
#endif