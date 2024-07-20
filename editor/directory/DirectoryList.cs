#if TOOLS
using Godot;
using System.Collections.Generic;

namespace SceneManagerMono.Editor.Directory;

[Tool]
public partial class DirectoryList : Container {
    [Signal]
    public delegate void RemovedEventHandler(string removedPath);
    
    ///// Public Functions /////

    public void UpdateValues(List<string> values) {
        Clear();
        AddAll(values);
    }
    
    ///// Private Functions /////
    
    private void Clear() {
        foreach (var child in GetChildren()) {
            RemoveChild(child);
            child.QueueFree();
        }
    }

    private void Add(string value) {
        var item = DirectoryItem.CreateInstance();
        AddChild(item);
        item.Text = value;
        item.DeleteRequested += DeleteItem;
    }

    private void AddAll(List<string> values) {
        foreach (var value in values) {
            Add(value);
        }
    }
    
    private void DeleteItem(string value) {
        EmitSignal(SignalName.Removed, value);
    }
}
#endif
