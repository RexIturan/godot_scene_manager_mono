#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class AutomaticSceneImport : MarginContainer {
    private const string ROOT_ADDRESS = "res://";
    
    private DirectoryFilter includeFilter;
    private DirectoryFilter excludeFilter;
    private Button resetButton;
    private Button generateButton;

    private List<string> autoGenerated = new();
    
    public override void _EnterTree() {
        base._EnterTree();

        includeFilter ??= GetNode<DirectoryFilter>("%Include Directory Filter");
        excludeFilter ??= GetNode<DirectoryFilter>("%Exclude Directory Filter");
        resetButton ??= GetNode<Button>("%Reset");
        generateButton ??= GetNode<Button>("%Generate");
    }

    public override void _Ready() {
        base._Ready();

        resetButton.Pressed += OnReset;
        generateButton.Pressed += OnGenerate;
        
        //load exclude include from resource
        //init both filter

        excludeFilter.Changed += UpdateExcludeDirs;
        includeFilter.Changed += UpdateIncludeDirs;
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Saved += ResetGenerated;
            
            excludeFilter.Values = SceneManagerEditor.Instance.ExcludeDirs;
            excludeFilter.UpdateValues();
            
            includeFilter.Values = SceneManagerEditor.Instance.IncludeDirs;
            includeFilter.UpdateValues();
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        excludeFilter.Changed -= UpdateExcludeDirs;
        includeFilter.Changed -= UpdateIncludeDirs;
    }

    ///// Private Functions /////
    
    private void UpdateExcludeDirs() {
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.ExcludeDirs = excludeFilter.Values;
        }
    }
    
    private void UpdateIncludeDirs() {
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.IncludeDirs = includeFilter.Values;
        }
    }
    
    private void ResetGenerated() {
        autoGenerated.Clear();
    }
    
    private void OnReset() {
        if (SceneManagerEditor.Exists() && autoGenerated.Count > 0) {
            var toRemove = new List<SceneDataItem>();
            foreach (var path in autoGenerated) {
                var found = SceneManagerEditor.Instance.SceneDataItems.FirstOrDefault(item => item.path == path);
                if (found.path == path ) {
                    toRemove.Add(found);
                }
            }
            
            SceneManagerEditor.Instance.ForceDeleteAllScenes(toRemove);
        }
    }

    private void OnGenerate() {
        var dict = new Dictionary<string, string>();

        var includes = includeFilter.Values;

        if (includes.Count == 0) {
            GetAllScenes(ROOT_ADDRESS, excludeFilter.Values.ToHashSet(), ref dict);
        } else {
            foreach (var includeDir in includes) {
                GetAllScenes(includeDir, excludeFilter.Values.ToHashSet(), ref dict);    
            }    
        }
        
        //todo debug pring remove if stable?
        // foreach (var value in dict) {
        //     GD.Print($"auto {value.Value} {value.Key}");
        // }

        if (SceneManagerEditor.Exists()) {
            foreach (var scene in SceneManagerEditor.Instance.SceneDataItems) {
                if (dict.ContainsKey(scene.path)) {
                    dict.Remove(scene.path);    
                }
            }
        }

        var toAdd = new List<Tuple<string, string>>();
        foreach (var value in dict) {
            toAdd.Add(new Tuple<string, string>(value.Value, value.Key));
            
            // create scene data for these
            autoGenerated.Add(value.Key);
        }

        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.AddAllSceneData(toAdd);    
        }
    }

    private void GetAllScenes(string path, HashSet<string> ignores, ref Dictionary<string, string> result) {
        if(ignores.Contains(path)) return;
        
        var dir = DirAccess.Open(path);

        var correctPath = path[^1] != '/' ? path + '/' : path;
        
        if (dir is {} && !dir.FileExists(path + ".gdignore")) {
            var files = dir.GetFiles();
            foreach (var file in files) {
                var filePath = correctPath + file;
                
                if (ignores.Contains(filePath)) {
                    continue;
                }
                
                //todo add scn && res?
                if (file.GetExtension() == "tscn") {
                    var name = file.Replace($".{file.GetExtension()}", "");
                    //todo add name with path to list / dict
                    
                    result.Add(filePath, name);
                }
            }

            var dirs = dir.GetDirectories();
            foreach (var subDir in dirs) {
                GetAllScenes(correctPath + subDir, ignores, ref result);
            }
        }
    }
}
#endif
