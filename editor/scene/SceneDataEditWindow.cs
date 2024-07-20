#if TOOLS
using System.Linq;
using Godot;
using SceneManagerMono.Editor.Data;

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

    public override void _EnterTree() {
        base._EnterTree();

        sceneDataEditor ??= GetNode<SceneDataEditor>("MarginContainer/MarginContainer/SceneData Editor");
        
        title = Title;
        
        sceneDataEditor.SceneData = Data;
    }

    public override void _Ready() {
        base._Ready();

        CloseRequested += Close;
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.SceneDeleted += OnSceneDeleted;
            SceneManagerEditor.Instance.Changed += OnChange;
            SceneManagerEditor.Instance.Saved += OnSaved;
        }
    }

    public override void _ExitTree() {
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.SceneDeleted -= OnSceneDeleted;
            SceneManagerEditor.Instance.Changed -= OnChange;
            SceneManagerEditor.Instance.Saved -= OnSaved;
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
