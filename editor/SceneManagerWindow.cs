using Godot;

[Tool]
public partial class SceneManagerWindow : Window {
    private string title;
    
    ///// Godot Function /////
    
    public override void _Ready() {
        base._Ready();

        title = Title;

        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed += OnChange;
            SceneManagerEditor.Instance.Saved += OnSaved;
        }
        
        CloseRequested += Close;
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= OnChange;    
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

    ///// Private Functions /////
    
    private void OnSaved() {
        Title = title;
    }

    private void OnChange() {
        Title = $"(*) {title}";
    }
    
    private void Close() {
        Hide();
    }
}
