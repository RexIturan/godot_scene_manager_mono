#if TOOLS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
using SceneManagerMono.Data;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Editor.Util;

namespace SceneManagerMono.Editor;

[Tool]
public partial class SceneManagerEditor : Node {
    #region Singelton

    public static SceneManagerEditor Instance;

    public SceneManagerEditor() {
        if (Instance is null) {
            Instance = this;
        } else {
            QueueFree();
        }
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        if (disposing) {
            if (Instance == this) {
                Instance = null;
            }
        }
    }

    #endregion

    private SceneListCache sceneListCache;

    private List<SceneDataItem> sceneDataList = new();
    public List<SceneDataItem> SceneDataItems => sceneDataList;
    private List<SceneDataItem> duplicateSceneDataKeys = new();

    private List<SceneDataGroupItem> sceneDataGroups = new();
    public List<SceneDataGroupItem> SceneDataGroupList => sceneDataGroups;
    private List<SceneDataTag> tags = new();
    public IReadOnlyList<SceneDataTag> TagList => tags.AsReadOnly();

    private bool hasChanges = false;
    
    ///// Properties /////
    
    private List<string> _excludeDirs = new List<string>();
    public List<string> ExcludeDirs {
        get => _excludeDirs;
        set {
            _excludeDirs = value;
            HandleOnChange();
        }
    }

    private List<string> _includeDirs = new List<string>();
    public List<string> IncludeDirs {
        get => _includeDirs;
        set {
            _includeDirs = value;
            HandleOnChange();
        }
    }

    // public Godot.Collections.Dictionary<string, string> SceneKeyPathDict => sceneListCache.SceneDict;
    //todo check if valid data
    public bool HasValidData => sceneListCache is not null;

    private bool _showSceneDelete;
    public bool ShowSceneDelete {
        get => _showSceneDelete;
        set {
            if (_showSceneDelete != value) {
                _showSceneDelete = value;
                EmitSignal(SignalName.ShowSceneDeleteChanged, value);
            }
        }
    }

    public List<SceneDataItem> UnGroupedItems {
        get {
            var ungrouped = new List<SceneDataItem>();
            var grouped = new HashSet<int>();

            foreach (var group in SceneDataGroupList) {
                foreach (var sceneIndex in group.scenes) {
                    grouped.Add(sceneIndex);
                }
            }

            for (int i = 0; i < SceneDataItems.Count; i++) {
                if (grouped.Contains(i)) {
                    continue;
                }

                ungrouped.Add(SceneDataItems[i]);
            }

            return ungrouped;
        }
    }

    // ##### Signals #####
    
    [Signal] public delegate void ChangedEventHandler();
    [Signal] public delegate void IndicesChangedEventHandler();
    [Signal] public delegate void SavedEventHandler();
    [Signal] public delegate void SceneDeletedEventHandler();
    [Signal] public delegate void GroupDeletedEventHandler();
    [Signal] public delegate void ShowSceneDeleteChangedEventHandler(bool newValue);
    
    // ##### Godot Functions #####

    public override void _Ready() {
        base._Ready();

        if (Load()) {
            InitData();
        }
    }

    // ##### Public Functions #####

    public void CreateNewResource(string path) {
        sceneListCache = new SceneListCache();
        SceneManagerSettings.SetSceneListPath(path);
        GD.Print("[SceneManagerEditor] CreateNewResource Save");
        ResourceSaver.Save(sceneListCache, SceneManagerSettings.GetSceneListPath());

        Clear();
        HandleOnChange();
        HandleOnSave();
    }

    public void ChangeResource(string path) {
        SceneManagerSettings.SetSceneListPath(path);

        if (Load()) {
            InitData();
        }
    }

    private void Clear() {
        sceneDataList.Clear();
        sceneDataGroups.Clear();
        tags.Clear();
        ExcludeDirs.Clear();
        IncludeDirs.Clear();
    }

    public void Save() {
        if (!hasChanges) return;
        
        Debug.Assert(sceneListCache is not null, "[SceneManagerEditor] Save with null resource");

        sceneListCache ??= new SceneListCache();
        sceneListCache.InitFromList(sceneDataList, sceneDataGroups, tags, ExcludeDirs, IncludeDirs);

        GD.Print("[SceneManagerEditor] Save ResourceSaver.Save");
        ResourceSaver.Save(sceneListCache, SceneManagerSettings.GetSceneListPath());
        HandleOnSave();
    }

    public bool Load() {
        GD.Print("[SceneManagerEditor] Try Load");

        var path = SceneManagerSettings.GetSceneListPath();
        if (FileAccess.FileExists(path)) {
            GD.Print("[SceneManagerEditor] Load");
            sceneListCache =
                ResourceLoader.Load<SceneListCache>(path, null, ResourceLoader.CacheMode.Replace);
            GD.Print("[SceneManagerEditor] Load successful!");
            return true;
        }

        return false;
    }

    public void InitData() {
        InitSceneData();

        InitSceneGroup();

        InitSceneTags();

        ExcludeDirs = sceneListCache.excludeDirs.ToList();
        IncludeDirs = sceneListCache.includeDirs.ToList();
        //todo validate & clean up group.scenes && scene.tags && group.tags

        HandleOnChange();
        HandleOnSave();
    }

    public List<SceneDataItem> GetSceneDataWithDuplicateKeys() {
        return duplicateSceneDataKeys;
    }

    public void ValidateSceneDataKeys() {
        duplicateSceneDataKeys.Clear();

        //todo extract into helper maybe cache?
        var keyLevelDict = new System.Collections.Generic.Dictionary<string, List<int>>();
        foreach (var item in sceneDataList) {
            if (keyLevelDict.ContainsKey(item.key)) {
                keyLevelDict[item.key].Add(item.index);
            } else {
                keyLevelDict.Add(item.key, new List<int>() {item.index});
            }
        }

        foreach (var item in keyLevelDict) {
            if (item.Value.Count > 1) {
                foreach (var index in item.Value) {
                    duplicateSceneDataKeys.Add(sceneDataList[index]);
                    SetDuplicate(index, true);
                }
            } else {
                SetDuplicate(item.Value[0], false);
            }
        }
    }

    public void SetSceneKey(int index, string newKey) {
        var item = sceneDataList[index];
        item.key = newKey;
        sceneDataList[index] = item;

        ValidateSceneDataKeys();

        HandleOnChange();
    }

    public void SetScenePath(int index, string path) {
        var item = sceneDataList[index];
        item.path = path;
        sceneDataList[index] = item;

        HandleOnChange();
    }

    public void SetSceneName(int index, string name) {
        var item = sceneDataList[index];
        item.name = name;
        sceneDataList[index] = item;

        HandleOnChange();
    }

    public void SetSceneDescription(int index, string description) {
        var item = sceneDataList[index];
        item.description = description;
        sceneDataList[index] = item;

        HandleOnChange();
    }

    public void SetSceneImage(int index, string imagePath) {
        var item = sceneDataList[index];
        item.imagePath = imagePath;
        sceneDataList[index] = item;

        HandleOnChange();
    }

    public void SetSceneTags(int index, Array<string> newTags) {
        var item = sceneDataList[index];
        item.tags = newTags.ToList();
        sceneDataList[index] = item;

        HandleOnChange();
    }

    public void AddSceneData(string key, string path) {
        sceneDataList.Add(new SceneDataItem() {
            index = sceneDataList.Count,
            key = key,
            path = path
        });

        ValidateSceneDataKeys();
        HandleOnChange();
    }

    public void AddAllSceneData(List<Tuple<string, string>> toAdd) {
        foreach (var tuple in toAdd) {
            sceneDataList.Add(new SceneDataItem() {
                index = sceneDataList.Count,
                key = tuple.Item1,
                path = tuple.Item2
            });
        }

        ValidateSceneDataKeys();
        HandleOnChange();
    }

    public void RequestDeleteScene(SceneDataItem sceneData, bool focusMainSceneManager = true) {
        var confimDialog = CreateConfirmDialog();
        confimDialog.Confirmed += () => {
            DeleteScene(sceneData);
            if (focusMainSceneManager) {
                FocusMainSceneManagerWindow();
            }
        };
        confimDialog.DialogText = $"Do you realy want to delete Scene: {sceneData.key}";
        EditorInterface.Singleton.PopupDialogCentered(confimDialog);
    }


    public void ForceDeleteScene(SceneDataItem found) {
        DeleteScene(found);
        FocusMainSceneManagerWindow();
    }

    public void ForceDeleteAllScenes(List<SceneDataItem> toRemove) {
        foreach (var removeItem in toRemove) {
            sceneDataList.Remove(removeItem);

            foreach (var group in sceneDataGroups) {
                group.scenes.Remove(removeItem.index);
            }
        }

        EmitSignal(SignalName.SceneDeleted);
        UpdateSceneDataIndices();
        HandleOnChange();
    }

    public void CreateGroup(string groupName) {
        sceneDataGroups.Add(new SceneDataGroupItem() {
            name = groupName,
            index = sceneDataGroups.Count
        });

        HandleOnChange();
    }

    public void RequestDeleteGroup(SceneDataGroupItem groupData,
        bool focusMainSceneManager = true) {
        var confimDialog = CreateConfirmDialog();
        confimDialog.Confirmed += () => {
            DeleteGroup(groupData);
            if (focusMainSceneManager) {
                FocusMainSceneManagerWindow();
            }
        };
        confimDialog.DialogText = $"Do you realy want to delete Group: {groupData.name}";
        EditorInterface.Singleton.PopupDialogCentered(confimDialog);
    }

    public void AddSceneToGroup(SceneDataItem sceneData, int groupIndex) {
        var index = sceneData.index;

        //remove from other group
        foreach (var group in sceneDataGroups) {
            if (group.scenes.Contains(index)) {
                group.scenes.Remove(index);
            }
        }

        sceneDataGroups[groupIndex].scenes.Add(index);
        HandleOnChange();
    }

    public void RemoveSceneFromGroup(SceneDataItem sceneData) {
        var index = sceneData.index;

        //remove from other group
        foreach (var group in sceneDataGroups) {
            if (group.scenes.Contains(index)) {
                group.scenes.Remove(index);
            }
        }

        HandleOnChange();
    }

    public bool ValidTagName(string name) {
        return tags.All(tag => tag.name != name);
    }

    public void AddTag(string name) {
        tags.Add(new SceneDataTag() {
            index = tags.Count,
            name = name,
        });

        HandleOnChange();
    }

    public void UpdateTag(int index, SceneDataTag newTag) {
        var oldTag = tags[index];
        if (oldTag != newTag) {
            tags[index] = newTag;

            if (oldTag.scene) {
                if (newTag.scene) {
                    ReplaceSceneTag(oldTag.name, newTag.name);
                } else {
                    RemoveSceneTag(oldTag.name);
                }
            }

            if (oldTag.group) {
                if (newTag.group) {
                    ReplaceGroupTag(oldTag.name, newTag.name);
                } else {
                    RemoveGroupTag(oldTag.name);
                }
            }

            HandleOnChange();
        }
    }

    public void RequestDeleteTag(SceneDataTag tagData, bool focusMainSceneManager = true) {
        var confimDialog = CreateConfirmDialog();
        confimDialog.Confirmed += () => {
            DeleteTag(tagData);
            if (focusMainSceneManager) {
                FocusMainSceneManagerWindow();
            }
        };
        confimDialog.DialogText = $"Do you realy want to delete Group: {tagData.name}";
        EditorInterface.Singleton.PopupDialogCentered(confimDialog);
    }

    public void SetGroupImage(int index, string imagePath) {
        var group = SceneDataGroupList[index];
        group.icon = imagePath;
        SceneDataGroupList[index] = group;

        HandleOnChange();
    }

    public void SetGroupName(int index, string newName) {
        var group = SceneDataGroupList[index];
        group.name = newName;
        SceneDataGroupList[index] = group;

        HandleOnChange();
    }

    public void SetGroupTags(int index, Array<string> newTags) {
        var group = SceneDataGroupList[index];
        group.tags = newTags.ToList();
        SceneDataGroupList[index] = group;

        HandleOnChange();
    }

    // ##### Private Functions #####

    private void HandleOnChange() {
        GD.Print("[SceneManagerEditor] OnChange");
        hasChanges = true;
        EmitSignal(SignalName.Changed);
    }
    
    private void HandleOnSave() {
        GD.Print("[SceneManagerEditor] OnSave");
        hasChanges = false;
        EmitSignal(SignalName.Saved);
    }
    
    private void FocusMainSceneManagerWindow() {
        var sceneDataEditWindows =
            EditorInterface.Singleton.GetBaseControl().GetNodes<SceneManagerWindow>();

        var first = sceneDataEditWindows.FirstOrDefault();
        if (first is not null) {
            first.GrabFocus();
        }
    }

    private void InitSceneData() {
        var index = 0;
        sceneDataList.Clear();
        foreach (var item in sceneListCache.sceneDatas) {
            sceneDataList.Add(new SceneDataItem(item) {
                index = index++,
            });
        }
    }

    private void InitSceneGroup() {
        var index = 0;
        sceneDataGroups.Clear();
        foreach (var group in sceneListCache.sceneGroups) {
            sceneDataGroups.Add(new SceneDataGroupItem(group) {
                index = index++
            });
        }
    }

    private void InitSceneTags() {
        var tagDict = new System.Collections.Generic.Dictionary<string, SceneDataTag>();

        foreach (var tag in sceneListCache.sceneTags) {
            tagDict.Add(tag, new SceneDataTag() {
                name = tag,
                scene = true
            });
        }

        foreach (var tag in sceneListCache.groupTags) {
            if (tagDict.TryGetValue(tag, out var tagData)) {
                tagData.group = true;
                tagDict[tag] = tagData;
            } else {
                tagDict.Add(tag, new SceneDataTag() {
                    name = tag,
                    group = true
                });
            }
        }

        foreach (var tag in sceneListCache.unusedTags) {
            if (tagDict.TryGetValue(tag, out var tagData)) {
                tagData.group = true;
                tagDict[tag] = tagData;
            } else {
                tagDict.Add(tag, new SceneDataTag() {
                    name = tag,
                });
            }
        }

        tags = tagDict.Values.ToList();
        tagDict.Clear();

        UpdateTagIndices();
    }

    private void DeleteGroup(SceneDataGroupItem groupData) {
        var removeItem = sceneDataGroups[groupData.index];
        sceneDataGroups.Remove(removeItem);

        EmitSignal(SignalName.GroupDeleted);
        UpdateGroupDataIndices();
        HandleOnChange();
    }

    private void DeleteScene(SceneDataItem sceneData) {
        var removeItem = sceneDataList[sceneData.index];
        sceneDataList.Remove(removeItem);

        foreach (var group in sceneDataGroups) {
            group.scenes.Remove(removeItem.index);
        }

        EmitSignal(SignalName.SceneDeleted);
        UpdateSceneDataIndices();
        HandleOnChange();
    }

    private void DeleteTag(SceneDataTag tagData) {
        var removeItem = tags[tagData.index];

        RemoveSceneTag(removeItem.name);
        RemoveGroupTag(removeItem.name);

        tags.Remove(removeItem);

        UpdateTagIndices();
        HandleOnChange();
    }

    private void RemoveSceneTag(string oldTagName) {
        foreach (var scene in sceneDataList) {
            scene.tags.Remove(oldTagName);
        }
    }

    private void RemoveGroupTag(string oldTagName) {
        foreach (var group in sceneDataGroups) {
            group.tags.Remove(oldTagName);
        }
    }

    private void ReplaceSceneTag(string oldTagName, string newTagName) {
        foreach (var scene in sceneDataList) {
            if (scene.tags.Contains(oldTagName)) {
                scene.tags.Remove(oldTagName);
                scene.tags.Add(newTagName);
            }
        }
    }

    private void ReplaceGroupTag(string oldTagName, string newTagName) {
        foreach (var group in sceneDataGroups) {
            if (group.tags.Contains(oldTagName)) {
                group.tags.Remove(oldTagName);
                group.tags.Add(newTagName);
            }
        }
    }

    private void SetDuplicate(int index, bool newValue) {
        var item = sceneDataList[index];
        item.duplicateKey = newValue;
        sceneDataList[index] = item;
    }


    private void UpdateGroupDataIndices() {
        for (int i = 0; i < sceneDataGroups.Count; i++) {
            var item = sceneDataGroups[i];
            item.index = i;
            sceneDataGroups[i] = item;
        }

        EmitSignal(SignalName.IndicesChanged);
    }

    private void UpdateSceneDataIndices() {
        for (int i = 0; i < sceneDataList.Count; i++) {
            var item = sceneDataList[i];
            item.index = i;
            sceneDataList[i] = item;
        }

        EmitSignal(SignalName.IndicesChanged);
    }

    private void UpdateTagIndices() {
        for (int i = 0; i < tags.Count; i++) {
            var item = tags[i];
            item.index = i;
            tags[i] = item;
        }
    }

    ///// Helper Functions /////

    private ConfirmationDialog CreateConfirmDialog() {
        var path = "res://addons/scene_manager_mono/editor/confirm/confirm_delete_dialog.tscn";
        return GodotHelper.LoadAndInstaniate<ConfirmationDialog>(path);
    }

    ///// Static Functions /////

    public static bool Exists() {
        return Instance is not null;
    }
}

#endif