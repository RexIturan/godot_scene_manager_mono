using System;
using System.Runtime.Loader;
using Godot;
using SceneManagerMono.Util;

namespace SceneManagerMono.Editor;

[Tool]
public partial class SceneManagerWindow : Window {
    private string title;
    
    ///// Editor Plugin Helper /////

    private Action<AssemblyLoadContext> unloadHandle;
    
    ///// Godot Functions /////
    
    public override void _Ready() {
        base._Ready();

        title = Title;

        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.TryConnect(SceneManagerEditor.SignalName.Changed, OnChange);
            SceneManagerEditor.Instance.TryConnect(SceneManagerEditor.SignalName.Saved, OnSaved);
        }
        
        this.TryConnect(Window.SignalName.CloseRequested, OnClose);
        
        unloadHandle = UnloadHelper.RegisterUnload(Cleanup);
    }

    public override void _ExitTree() {
        base._ExitTree();
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

    private void Cleanup() {
        UnloadHelper.UnregisterUnload(unloadHandle);
        
        if(!IsInstanceValid(this)) return;
        
        this.TryDisconnect(Window.SignalName.CloseRequested, OnClose);
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.TryDisconnect(SceneManagerEditor.SignalName.Changed, OnChange);
            SceneManagerEditor.Instance.TryDisconnect(SceneManagerEditor.SignalName.Saved, OnSaved);    
        }
    }
    
    ///// Callback Functions /////
        
    private void OnSaved() {
        Title = title;
    }

    private void OnChange() {
        Title = $"(*) {title}";
    }

    private void OnClose() {
        Close();
    }
    
    ///// Private Functions /////
    
    private void Close() {
        Hide();
    }
}
