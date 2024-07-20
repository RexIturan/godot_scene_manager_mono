#if TOOLS
using System.Linq;
using Godot;

namespace SceneManagerMono.Editor.Util;

[Tool]
public partial class DragFilePathInput : LineEdit {
    //todo mage string array or string of multiple
    [Export] public string FileExtensionFilter = "";
    [Export] public bool Active { get; set; }
    private string path;

    [Signal]
    public delegate void ChangedEventHandler(string text);
    
    ///// Godot Functions /////
    
    public override bool _CanDropData(Vector2 atPosition, Variant data) {
        return TryGetValidDropData(data, out var _);
    }

    public override void _DropData(Vector2 atPosition, Variant data) {
        if (TryGetValidDropData(data, out path)) {
            Text = path;
            EmitSignal(SignalName.Changed);
        }
    }

    ///// Private Functions /////
    
    private bool TryGetValidDropData(Variant data, out string path) {
        path = "";
        if (!Active) {
            return false;
        }
        
        var dict = data.AsGodotDictionary<string, Variant>();
        if (dict.ContainsKey("type") && dict["type"].AsString() == "files") {
            //has key && value is correct
            if (dict.ContainsKey("files")) {
                var filePath = dict["files"].AsStringArray()[0];

                if (MatchFileExtension(filePath, FileExtensionFilter)) {
                    path = filePath;
                    return true;
                }
            }
        }
        
        return false;
    }

    private bool MatchFileExtension(string filepath, string extensionFilter) {
        var fileExtension = filepath.Split('.').Last();
        
        var extensions = extensionFilter.Split(',');
        return extensions.Any(extension => 
            fileExtension == extension.TrimStart('*').TrimStart('.').Trim()
        );
    }
}
#endif
