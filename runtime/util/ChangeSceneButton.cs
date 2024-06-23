using Godot;

namespace SceneManagerMono.Runtime.Util;

public partial class ChangeSceneButton : Button {
    [Export] private bool loadAsync = false;
    [Export] private string sceneKey = "";
    [Export] private bool useHistory = true;
    
    // todo tab or conditional in inspector?
    [Export] private PackedScene packedSceneToLoad;
    [Export] private Node nodeToLoad;

    public override void _Ready() {
        SceneManager.Instance.ValidateSceneKey(sceneKey);
        Pressed += LoadScene;
    }

    private void LoadScene() {
        if (loadAsync) {
            var options = new SceneManager.Options() { addToHistory = false };
            SceneManager.Instance.NextSceneToLoad = sceneKey;
            SceneManager.Instance.ChangeScene("loadingScreen", options);
        }
        else {
            var options = new SceneManager.Options() { addToHistory = useHistory };
            SceneManager.Instance.ChangeScene(sceneKey, options);    
        }
    }
}
