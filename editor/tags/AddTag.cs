#if TOOLS
using Godot;

namespace SceneManagerMono.Editor.Tags;

[Tool]
public partial class AddTag : Container {
    private LineEdit lineEdit;
    private Button addButton;

    public override void _EnterTree() {
        base._EnterTree();
        
        lineEdit ??= GetNode<LineEdit>("%LineEdit");
        addButton ??= GetNode<Button>("%Add");
    }

    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Exists()) {
            addButton.Pressed += OnAddpressed;    
        }
    }

    private void OnAddpressed() {
        if (lineEdit.Text != "" &&
            SceneManagerEditor.Instance.ValidTagName(lineEdit.Text)) {
            SceneManagerEditor.Instance.AddTag(lineEdit.Text);
            lineEdit.Text = "";    
        }
    }
}
#endif
