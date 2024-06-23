#if TOOLS
using Godot;

[Tool]
public partial class CreateNewSceneListResourceButton : Button {
    private CreateResourceDialog Dialog => GetNode<CreateResourceDialog>("%ConfirmationDialog");
    
    public override void _Ready() {
        base._Ready();

        Pressed += OnPressed;

        var dialog = Dialog;
        dialog.CreationRequested += OnDialogConfirm;
    }

    ///// Private Functions /////
    
    private void OnPressed() {
        Dialog.Show();
    }

    private void OnDialogConfirm(string path) {
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.CreateNewResource(path);
            GD.Print($"create new at {path}");    
        }
        else {
            GD.PrintErr("SceneManagerEditor doesn't exist, can't create new resource.");
        }
    }
}
#endif
