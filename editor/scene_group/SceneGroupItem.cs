#if TOOLS
using System.Linq;
using Godot;
using SceneManagerC.Addons.SceneManagerMono.Editor;

[Tool]
public partial class SceneGroupItem : Container {
    private LineEdit lineEdit;
    private Button editButton;
    private Button deleteButton;
    
    public SceneDataGroupItem Data { get; set; }
    public string Text {
        get => lineEdit?.Text ?? "";
        set {
            if (lineEdit is { }) {
                lineEdit.Text = value;    
            }
        }
    }

    public override void _EnterTree() {
        base._EnterTree();

        lineEdit ??= GetNode<LineEdit>("%LineEdit");
        editButton ??= GetNode<Button>("%Edit");
        deleteButton ??= GetNode<Button>("%Delete");
    }

    public override void _Ready() {
        base._Ready();

        deleteButton.Pressed += OnDelete;
        editButton.Pressed += OnEdit;
        
        //todo on change handler
        lineEdit.Text = Data.name;
    }

    private void OnEdit() {
        var sceneDataEditWindows = 
            EditorInterface.Singleton.GetBaseControl().GetNodes<SceneGroupEditWindow>();

        var found = sceneDataEditWindows.FirstOrDefault(window => window.GroupData.index == Data.index);
        if (found is not null) {
            found.GrabFocus();
        }
        else {
            var window = SceneGroupEditWindow.CreateInstance(Data);
            EditorInterface.Singleton.GetBaseControl().AddChild(window);
        }
    }

    private void OnDelete() {
        SceneManagerEditor.Instance?.RequestDeleteGroup(Data);
    }
}
#endif
