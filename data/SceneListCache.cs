using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
#if TOOLS
using SceneManagerMono.Editor.Data;
#endif

namespace SceneManagerMono.Data;

[Tool]
[GlobalClass]
public partial class SceneListCache : Resource {
    [Export] public Godot.Collections.Dictionary<string, string> SceneDict { get; private set; } = new ();
    //todo maybe add bidirectional dictonary
    
    [Export] public Array<SceneGroupData> sceneGroups = new();
    [Export] public Array<SceneData> sceneDatas = new Array<SceneData>();
    
    [Export] public Array<string> groupTags = new();
    [Export] public Array<string> sceneTags = new();
    [Export] public Array<string> unusedTags = new();
    [Export] public Array<string> excludeDirs = new();
    [Export] public Array<string> includeDirs = new();
    
    public SceneListCache() { }

    ///// SceneDict Functions /////
    
    public bool ContainsName(string name) {
        return SceneDict.ContainsKey(name);
    }
    
    public bool ContainsPath(string path) {
        var key = "";
        
        foreach (var sceneData  in SceneDict) {
            if (sceneData.Value == path) {
                key = sceneData.Key;
                return true;
            }
        }

        return false;
    }

    public bool TryGetSceneKey(string path, out string key) {
        key = "";
        
        foreach (var sceneData  in SceneDict) {
            if (sceneData.Value == path) {
                key = sceneData.Key;
                return true;
            }
        }

        return false;
    }

    public string GetScenePath(string key) {
        return SceneDict[key];
    }
    
    public bool TryGetPath(string key, out string path) {
        return SceneDict.TryGetValue(key, out path);
    }

    public bool TryAdd(string key, string path) {
        if (ContainsName(key)) return false;
        Add(key, path);
        return true;
    }

    public void Add(string key, string path) {
        SceneDict.Add(key, path);
    }

    public void Clear() {
        SceneDict.Clear();
    }

    ///// Init Functions /////
    
    /// convert complex data to scene key path dict
    public void InitSceneDict() {
        Clear();
        foreach (var sceneData in sceneDatas) {
            Add(sceneData.Key, sceneData.Path);
        }
    }
#if TOOLS
    public void InitFromList(
        List<SceneDataItem> sceneDataList,
        List<SceneDataGroupItem> sceneDataGroups,
        List<SceneDataTag> tags,
        List<string> excludeDirs,
        List<string> includeDirs
    ) {
        // create scene data
        UpdateSceneData(sceneDataList);
        
        //create groups
        UpdateSceneGroupData(sceneDataGroups);
        
        // create tags
        sceneTags.Clear();
        groupTags.Clear();
        unusedTags.Clear();
        foreach (var tag in tags) {
            if (tag.group) {
                groupTags.Add(tag.name);
            }
            
            if (tag.scene) {
                sceneTags.Add(tag.name);
            }

            if (!tag.group || !tag.scene) {
                unusedTags.Add(tag.name);
            }
        }

        this.excludeDirs = new Array<string>(excludeDirs);
        
        this.includeDirs = new Array<string>(includeDirs);
    }

    private void UpdateSceneData(List<SceneDataItem> newSceneDataItems) {
        List<(SceneData, SceneDataItem)> matchedSceneData = new();
        List<SceneData> removeList = new();
        List<SceneDataItem> addList = new();
        
        foreach (var sceneDataItem in newSceneDataItems) {
            var foundMatch = false;
            
            foreach (var sceneData in sceneDatas) {
                if (sceneDataItem.ResourcePath != "" && sceneDataItem.ResourcePath == sceneData.ResourcePath) {
                    matchedSceneData.Add((sceneData, sceneDataItem));
                    foundMatch = true;
                }
            }

            if (!foundMatch) {
                addList.Add(sceneDataItem);
            }
        }

        foreach (var sceneData in sceneDatas) {
            if (matchedSceneData.All(tuple => {
                    var (scene, _) = tuple;
                    return sceneData.ResourcePath != scene.ResourcePath;
                })) {
                
                removeList.Add(sceneData);
            }
        }

        //remove
        foreach (var sceneData in removeList) {
            sceneDatas.Remove(sceneData);
        }

        //add new resources
        foreach (var sceneItem in addList) {
            sceneDatas.Add(sceneItem.GetResource());
        }

        foreach (var (sceneData, dataItem) in matchedSceneData) {
            sceneData.UpdateData(dataItem);
        }
    }

    private void UpdateSceneGroupData(List<SceneDataGroupItem> sceneDataGroups) {
        List<(SceneGroupData, SceneDataGroupItem)> matchedGroups = new();
        List<SceneGroupData> removeList = new();
        List<SceneDataGroupItem> addList = new();
        
        foreach (var groupItem in sceneDataGroups) {
            var foundMatch = false;
            
            foreach (var group in sceneGroups) {
                if (groupItem.ResourcePath != "" && groupItem.ResourcePath == group.ResourcePath) {
                    matchedGroups.Add((group, groupItem));
                    foundMatch = true;
                }
            }

            if (!foundMatch) {
                addList.Add(groupItem);
            }
        }

        foreach (var group in sceneGroups) {
            if (matchedGroups.All(tuple => {
                    var (matchedGroup, _) = tuple;
                    return group.ResourcePath != matchedGroup.ResourcePath;
                })) {
                
                removeList.Add(group);
            }
        }

        //remove
        foreach (var group in removeList) {
            sceneGroups.Remove(group);
        }

        //add new resources
        foreach (var groupItem in addList) {
            sceneGroups.Add(groupItem.GetResource());
        }

        foreach (var (group, dataItem) in matchedGroups) {
            group.UpdateData(dataItem);
        }
    }
#endif    
}
