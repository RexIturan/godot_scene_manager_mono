#if TOOLS
using Godot;
using SceneManagerMono.Editor.Data;

namespace SceneManagerMono.Editor.Tags;

[Tool]
public partial class TagItem : Container {
    [Export] private LineEdit lineEdit;
    [Export] private Button groupToggle;
    [Export] private Button sceneToggle;
    [Export] private Button deleteButton;

    public SceneDataTag TagData { get; set; }

    public override void _EnterTree() {
        base._EnterTree();
        
        lineEdit ??= GetNode<LineEdit>("%LineEdit"); 
        groupToggle ??= GetNode<Button>("%GroupToggle");
        sceneToggle ??= GetNode<Button>("%SceneToggle");
        deleteButton ??= GetNode<Button>("%Delete");
    }

    public override void _Ready() {
        base._Ready();

        lineEdit.TextChanged += OnTextChanged;
        groupToggle.Toggled += OnGroupToggled;
        sceneToggle.Toggled += OnSceneToggled;
        deleteButton.Pressed += OnTagDelete;

        UpdateTagData();
    }

    private void UpdateTagData() {
        lineEdit.Text = TagData.name;
        groupToggle.ButtonPressed = TagData.group;
        sceneToggle.ButtonPressed = TagData.scene;
    }
    
    private void OnTextChanged(string newtext) {
        var newTag = TagData;
        newTag.name = newtext;
        SceneManagerEditor.Instance.UpdateTag(newTag.index, newTag);
    }
    
    private void OnSceneToggled(bool toggled) {
        var newTag = TagData;
        newTag.scene = toggled;
        SceneManagerEditor.Instance.UpdateTag(newTag.index, newTag);
    }

    private void OnGroupToggled(bool toggled) {
        var newTag = TagData;
        newTag.group = toggled;
        SceneManagerEditor.Instance.UpdateTag(newTag.index, newTag);
    }
    
    private void OnTagDelete() {
        SceneManagerEditor.Instance.RequestDeleteTag(TagData);
    }
}
#endif
