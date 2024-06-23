#if TOOLS
using Godot;
using Godot.Collections;

[Tool]
public partial class TagSelect : HFlowContainer {
    public Array<string> _selectedTags = new();

    public Array<string> SelectedTags {
        get => _selectedTags;
        set {
            _selectedTags = value;
            CreateTagCheckboxes();
        }
    }
    [Export] private bool useGroup;
    [Export] private bool useScene;

    [Signal]
    public delegate void ChangedEventHandler(Array<string> newTags);
    
    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed += CreateTagCheckboxes;
            CreateTagCheckboxes();
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= CreateTagCheckboxes;
        }
    }

    private void CreateTagCheckboxes() {
        if (!SceneManagerEditor.Exists()) return;
        
        var tags = SceneManagerEditor.Instance.TagList;

        Clear();
        
        foreach (var tag in tags) {
            if (useGroup && tag.group || useScene && tag.scene) {
                var checkbox = new CheckBox();
                AddChild(checkbox);
                checkbox.Text = tag.name;
                checkbox.ButtonPressed = SelectedTags.Contains(tag.name);
                checkbox.Pressed += () => ToggleTag(tag.name);
            }
        }
    }

    private void ToggleTag(string tagName) {
        if (SelectedTags.Contains(tagName)) {
            SelectedTags.Remove(tagName);
        }
        else {
            SelectedTags.Add(tagName);
        }

        EmitSignal(SignalName.Changed, SelectedTags);
    }

    private void Clear() {
        foreach (var child in GetChildren()) {
            RemoveChild(child);
            child.QueueFree();
        }
    }
}
#endif
