#if TOOLS
using System.Linq;
using Godot;
using SceneManagerC.Addons.SceneManagerMono.Editor;

[Tool]
public partial class OpenSceneDataEditWindow : Button {
    //todo replace with direct resource loader
    [Export] private PackedScene SceneDataEditWiondowPrefab;
    
    public int Index { get; set; }
    
    public override void _Ready() {
        base._Ready();

        Pressed += OpenWindow;
    }

    private void OpenWindow() {
        //todo check if a edit window for this scene key already exists
        var sceneDataEditWindows = 
            EditorInterface.Singleton.GetBaseControl().GetNodes<SceneDataEditWindow>();

        var found = sceneDataEditWindows.FirstOrDefault(window => window.Data.index == Index);
        if (found is not null) {
            found.GrabFocus();
        }
        else {
            var window = SceneDataEditWiondowPrefab.Instantiate<SceneDataEditWindow>();
            window.Data = SceneManagerEditor.Instance.SceneDataItems[Index];
        
            EditorInterface.Singleton.GetBaseControl().AddChild(window);
        }
    }
}
#endif
