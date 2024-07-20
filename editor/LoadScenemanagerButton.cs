#if TOOLS
using Godot;
using SceneManagerMono.Data;

namespace SceneManagerMono.Editor;

[Tool]
public partial class LoadScenemanagerButton : Button {
    private FileDialog fileDialog => GetNode<FileDialog>("FileDialog");
    
    public override void _Ready() {
        base._Ready();
        
        if (SceneManagerEditor.Exists()) {
            Pressed += OnLoad;
            fileDialog.FileSelected += OnFileSelected;
        }
    }

    private void OnFileSelected(string path) {
        //validate new path
        if (SceneManagerEditor.Exists()) {
            var testResource = ResourceLoader.Load(path);
            if (testResource is SceneListCache) {
                //set to new
                SceneManagerEditor.Instance.ChangeResource(path);
            }
            else {
                //error?
            }
        }
    }

    private void OnLoad() {
        fileDialog.Show();
    }
}
#endif
