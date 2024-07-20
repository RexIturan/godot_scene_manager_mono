#if TOOLS
using Godot;

namespace SceneManagerMono.Editor.Scene_List;

[Tool]
public partial class SeceneItemActionMenu : OptionButton {
    // private PopupMenu groupMenu;
    
    ///// Godot Functions /////

    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Exists()) {
            GetPopup().IdPressed += OnGroupSelected;
            UpdateGroupSelectionMenu();
            SceneManagerEditor.Instance.Changed += UpdateGroupSelectionMenu;    
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateGroupSelectionMenu;    
        }
    }

    private void OnGroupSelected(long id) {
        //todo idk this is not that safe
        var sceneData = GetParent<SceneItem>().SceneDataItem;
        if (id > 0) {
            SceneManagerEditor.Instance.AddSceneToGroup(sceneData, (int)id -1);    
        }
        else {
            SceneManagerEditor.Instance.RemoveSceneFromGroup(sceneData);
        }
    }

    ///// Private Functions /////
    
    private void OnIdPressed(long id) {
        switch (id) {
            case 0:
                //open set group dialog
                break;
            case 1:
                //open item edit window
                break;
        }
    }

    private void UpdateGroupSelectionMenu() {
        //todo idk this is not that safe
        var sceneData = GetParent<SceneItem>().SceneDataItem;

        var popup = GetPopup();
        popup.Clear();
        popup.AddItem("None", 0);
        popup.SetItemAsRadioCheckable(0, true);
        
        Selected = 0;
        
        foreach (var group in SceneManagerEditor.Instance.SceneDataGroupList) {
            var id = group.index + 1;
            popup.AddItem(group.name, id);
            popup.SetItemAsRadioCheckable(id, true);
            bool isSelected = group.scenes.Contains(sceneData.index);
            popup.SetItemChecked(id, isSelected);
            if (isSelected) {
                Selected = id;
            }
        }
        
        popup.SetItemChecked(0, Selected == 0);
    }
}
#endif
