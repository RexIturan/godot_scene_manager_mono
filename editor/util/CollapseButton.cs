#if TOOLS
using Godot;

[Tool]
public partial class CollapseButton : Button {
    [Export] private Control target;

    [Export] private Texture2D activeIcon;
    [Export] private Texture2D inactiveIcon;
    
    public override void _Ready() {
        base._Ready();

        Pressed += ToggleTargetVisibility;

        if (target is { }) {
            ToggleIcon(target.Visible);    
        }
        else {
            GD.Print($"CollapseButton target is missing from: {Name}");
        }
    }

    private void ToggleTargetVisibility() {
        if (!target.Visible) {
            target.Show();
        }
        else {
            target.Hide();
        }

        ToggleIcon(target.Visible);
    }

    private void ToggleIcon(bool visible) {
        Icon = visible ? activeIcon : inactiveIcon;
    }
}
#endif
