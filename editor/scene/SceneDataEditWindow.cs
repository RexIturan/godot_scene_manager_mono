#if TOOLS
using System;
using System.Linq;
using System.Runtime.Loader;
using Godot;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Util;

namespace SceneManagerMono.Editor.Scene;

[Tool]
public partial class SceneDataEditWindow : Window {
    [Export] private SceneDataEditor sceneDataEditor;
    
    private string title;
    private SceneDataItem _sceneDataItem;

    public SceneDataItem Data {
        get => _sceneDataItem;
        set {
            _sceneDataItem = value;
            if (sceneDataEditor is { }) {
                sceneDataEditor.SceneData = Data;
            }
        }
    }

    ///// Editor Plugin Helper /////

    private Action<AssemblyLoadContext> unloadHandle;

    ///// Godot Functions /////
    
    public override void _EnterTree() {
        base._EnterTree();

        sceneDataEditor ??= GetNode<SceneDataEditor>("MarginContainer/MarginContainer/SceneData Editor");
        
        title = Title;
        
        sceneDataEditor.SceneData = Data;
        
        this.TryConnect(Window.SignalName.CloseRequested, Close);
        
        if (SceneManagerEditor.Exists() && IsInstanceValid(SceneManagerEditor.Instance)) {
            SceneManagerEditor.Instance.TryConnect(SceneManagerEditor.SignalName.SceneDeleted, OnSceneDeleted);
            SceneManagerEditor.Instance.TryConnect(SceneManagerEditor.SignalName.Changed, OnChange);
            SceneManagerEditor.Instance.TryConnect(SceneManagerEditor.SignalName.Saved, OnSaved);
        }
        
        unloadHandle = UnloadHelper.RegisterUnload(Cleanup);
    }

    public override void _ExitTree() {
        Cleanup();
    }

    public override void _UnhandledKeyInput(InputEvent @event) {
        base._UnhandledKeyInput(@event);

        if (@event is InputEventWithModifiers modifiers) {
            if (@event is InputEventKey eventKey && eventKey.Keycode == Key.S) {
                if (modifiers.CtrlPressed) {
                    SceneManagerEditor.Instance?.Save();
                }
            }
        }
    }

    ///// Helper Functions /////
    
    private void Cleanup() {
        UnloadHelper.UnregisterUnload(unloadHandle);
        
        if (!IsInstanceValid(this)) return;
        
        this.TryDisconnect(Window.SignalName.CloseRequested, Close);
        
        if (SceneManagerEditor.Exists() && IsInstanceValid(SceneManagerEditor.Instance)) {
            SceneManagerEditor.Instance.TryDisconnect(SceneManagerEditor.SignalName.SceneDeleted, OnSceneDeleted);
            SceneManagerEditor.Instance.TryDisconnect(SceneManagerEditor.SignalName.Changed, OnChange);
            SceneManagerEditor.Instance.TryDisconnect(SceneManagerEditor.SignalName.Saved, OnSaved);
        }
    }
    
    // ##### Public Functions #####
    
    // ##### Private Functions #####
    
    private void OnSaved() {
        Title = title;
    }

    private void OnChange() {
        Title = $"(*) {title}";
    }
    
    private void OnSceneDeleted() {
        var items = SceneManagerEditor.Instance.SceneDataItems;
        var shouldClose = items.All(item => item.index != Data.index);
        if (shouldClose) {
            Close();
        }
    }
    
    private void Close() {
        Hide();
        GetParent().RemoveChild(this);
        QueueFree();
    }
}
#endif
