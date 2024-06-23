#if TOOLS
using System.Linq;
using Godot;

[Tool]
public partial class AddSceneGroup : HBoxContainer {
    private LineEdit lineEdit;
    private Button button;

    public override void _EnterTree() {
        base._EnterTree();

        lineEdit ??= GetNode<LineEdit>("LineEdit");
        button ??= GetNode<Button>("Create");
    }

    public override void _Ready() {
        base._Ready();

        button.Pressed += OnCreateGroup;
    }

    // ##### Private Functions #####
    
    private void OnCreateGroup() {
        if (SceneManagerEditor.Instance is not null) {

            if (lineEdit.Text != "" && IsNameValid(lineEdit.Text)) {
                SceneManagerEditor.Instance.CreateGroup(lineEdit.Text);
                lineEdit.Text = "";
            }
        }
    }

    private bool IsNameValid(string name) {
        return SceneManagerEditor.Instance.SceneDataGroupList.All(group => group.name != name);
    }
}
#endif
