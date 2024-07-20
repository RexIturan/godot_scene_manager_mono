#if TOOLS
using System.Linq;
using Godot;
using Godot.Collections;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Editor.Scene_List;
using SceneManagerMono.Editor.Tags;
using SceneManagerMono.Editor.Util;

namespace SceneManagerMono.Editor.Scene_Group;

[Tool]
public partial class SceneGroupEditWindow : Window {
    private string title;
    
    private LineEdit name;
    private FileInput imagePath;
    private TextureRect imageTexture;
    private SceneListDisplay sceneListDisplay;
    private TagSelect tagSelect;
    private Button deleteButton;

    private SceneDataGroupItem _groupData;
    public SceneDataGroupItem GroupData {
        get => _groupData;
        private set {
            _groupData = value;
            if (tagSelect is {}) {
                tagSelect.SelectedTags = new Array<string>(GroupData.tags);    
            }
        }
    }
    
    ///// Godot Functions /////

    public override void _EnterTree() {
        base._EnterTree();
        
        title = Title;
        
        name ??= GetNode<LineEdit>("%DisplayName LineEdit"); 
        imagePath ??= GetNode<FileInput>("%Image File Input");
        imageTexture ??= GetNode<TextureRect>("%Image TextureRect");
        sceneListDisplay ??= GetNode<SceneListDisplay>("%SceneList");
        tagSelect ??= GetNode<TagSelect>("%TagSelect");
        deleteButton = GetNode<Button>("%Delete");
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed += UpdateInputs;
            SceneManagerEditor.Instance.IndicesChanged += UpdateData;
            SceneManagerEditor.Instance.GroupDeleted += OnGroupDeleted;
            SceneManagerEditor.Instance.Changed += OnChange;
            SceneManagerEditor.Instance.Saved += OnSaved;
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateInputs;
            SceneManagerEditor.Instance.IndicesChanged -= UpdateData;
            SceneManagerEditor.Instance.GroupDeleted -= OnGroupDeleted;
            SceneManagerEditor.Instance.Changed -= OnChange;
            SceneManagerEditor.Instance.Saved -= OnSaved;
        }
    }
    
    public override void _Ready() {
        base._Ready();

        CloseRequested += Close;

        name.TextChanged += OnNameChanged;
        imagePath.Changed += OnImagePathChanged;
        deleteButton.Pressed += OnDelete;
        tagSelect.Changed += OnTagSelected;
        
        if (SceneManagerEditor.Exists()) {
            UpdateInputs();
        }
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

    ///// Callback Functions /////
    
    private void OnImagePathChanged() {
        SceneManagerEditor.Instance?.SetGroupImage(GroupData.index, imagePath.Text);
    }

    private void OnNameChanged(string newName) {
        SceneManagerEditor.Instance?.SetGroupName(GroupData.index, newName);
    }
    
    private void OnDelete() {
        SceneManagerEditor.Instance?.RequestDeleteGroup(GroupData);
    }
    
    private void OnTagSelected(Array<string> tags) {
        SceneManagerEditor.Instance?.SetGroupTags(GroupData.index, tags);
    }
    
    private void UpdateData() {
        var found = SceneManagerEditor.Instance.SceneDataGroupList.Find(item => item.name == GroupData.name);
        if (found is {}) {
            GroupData = found;
        }
    }
    
    private void UpdateInputs() {
        GroupData = SceneManagerEditor.Instance.SceneDataGroupList[GroupData.index];
        
        if (name.Text != GroupData.name) {
            name.Text = GroupData.name;
        }

        if (imagePath.Text != GroupData.icon) {
            imagePath.Text = GroupData.icon;
        }
        
        imageTexture.Texture = GroupData.IsImagePathValid()
            ? ResourceLoader.Load<Texture2D>(GroupData.icon)
            : null;
    }
    
    private void OnSaved() {
        Title = title;
    }

    private void OnChange() {
        Title = $"(*) {title}";
    }
    
    private void OnGroupDeleted() {
        var items = SceneManagerEditor.Instance.SceneDataGroupList;
        var shouldClose = items.All(item => item.index != GroupData.index);
        if (shouldClose) {
            Close();
        }
    }
    
    ///// Private Functions /////

    private void Close() {
        GetParent().RemoveChild(this);
        Hide();
        this.QueueFree();
    }
    
    ///// Static Functions /////

    public static SceneGroupEditWindow CreateInstance(SceneDataGroupItem groupData) {
        var window = GodotHelper.LoadAndInstaniate<SceneGroupEditWindow>(
            "res://addons/scene_manager_mono/editor/scene_group/scene_group_edit_window.tscn");
        window.GroupData = groupData;
        return window;
    }
}
#endif
