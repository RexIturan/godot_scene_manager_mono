using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using SceneManagerMono.Data;
using Array = Godot.Collections.Array;

namespace SceneManagerMono.Runtime;

public partial class SceneManager : Node {
    public struct Options {
        public bool addToHistory;
    }
    
#region Singelton

    public static SceneManager Instance;

#endregion
    
    private string currentSceneKey = "";

    // scene history
    private Stack<string> sceneStack = new Stack<string>();
    private int stackLimit = -1;

    // Scene List
    [Export]
    private SceneListCache sceneListCache;

    // async scene laodingh fields
    private string loadScenePath = "";
    private Array loadProgress = new Array();

    //todo solve laoding screens a other way
    //todo or move to "loading screen cache" singelton
    //for delayed scene laoding with intermideate scenes
    private string _nextSceneToLoad = "";
    public string NextSceneToLoad {
        get => _nextSceneToLoad;
        set { _nextSceneToLoad = ValidateSceneKey(value) ? value : ""; }
    }

    //signals
    [Signal] public delegate void LoadPercentChangedEventHandler(int percent);
    [Signal] public delegate void LoadingFinishedEventHandler();
    [Signal] public delegate void SceneChangedEventHandler();

    // helper to await signals
    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
    
    ///// Godot Functions /////

    #region Godot Functions

    public override void _EnterTree() {
        if (Instance is null) {
            Instance = this;
            sceneListCache = ResourceLoader.Load<SceneListCache>(
                SceneManagerSettings.GetSceneListPath(),
                null,
                ResourceLoader.CacheMode.Replace
            );
            sceneListCache.InitSceneDict();
        }
        else {
            Free();
        }
    }

    public override void _Ready() {
        SetProcess(false);
        SetInitialCurrentScene();
    }

    public override void _Process(double delta) {
        int prevProgress = 0;
        if (loadProgress.Count != 0) {
            prevProgress = (int)(loadProgress[0].AsSingle() * 100);
        }

        var status = ResourceLoader.LoadThreadedGetStatus(loadScenePath, loadProgress);

        int currentProgress = (int)(loadProgress[0].AsSingle() * 100);
        if (prevProgress != currentProgress) {
            EmitSignal(SignalName.LoadPercentChanged, currentProgress);
        }

        if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
            SetProcess(false);
            loadProgress.Clear();
            EmitSignal(SignalName.LoadingFinished);
        }
        else if (status == ResourceLoader.ThreadLoadStatus.InProgress) { }
        else {
            //todo throw correct exception
            throw new Exception("for some reason, loading failed");
        }
    }

    #endregion

    ///// Public Functions /////

    public void ResetSceneStack() {
        SetInitialCurrentScene();
        sceneStack.Clear();
    }
    
    public bool ValidateSceneKey(string key) {
        var valid = sceneListCache.ContainsName(key);
        
        if(!valid) {
            // GD.PushError($"SceneManager.ValidateSceneKey param 'key' with value: {key} is Invalid!");
            throw new Exception($"SceneKey: \'{key}\' is Invalid!\nSceneManager.ValidateSceneKey param 'key' with value: {key} is Invalid!");
        }
        Debug.Assert(valid, $"SceneManager.ValidateSceneKey param 'key' with value: {key} is Invalid!");

        return valid;
    }

    public void AddSceneKeyPathPair(string key, string path) {
        sceneListCache.SceneDict.Add(key, path);
    }
    
    ///// async scene loading
    #region Load Scene Async

    /// <summary>
    /// loads scene interactive
    /// connect to `load_percent_changed(value: int)` and `load_finished` signals
    /// to listen to updates on your scene loading status 
    /// </summary>
    public void LoadSceneAsync(String key) {
        if (ValidateSceneKey(key)) {
            SetProcess(true);
            loadScenePath = sceneListCache.GetScenePath(key);
            ResourceLoader.LoadThreadedRequest(loadScenePath, "", true, ResourceLoader.CacheMode.Ignore);
        }
    }

    /// <summary>
    /// returns loaded scene
    ///
    /// If scene is not loaded, blocks and waits until scene is ready. (acts blocking in code
    /// and may freeze your game, make sure scene is ready to get)
    /// </summary>
    /// <returns>loaded scene or null</returns>
    public PackedScene GetLoadedScene() {
        if (loadScenePath != "") {
            return ResourceLoader.LoadThreadedGet(loadScenePath) as PackedScene;
        }

        return null;
    }

    public bool TryGetLoadedScene(out PackedScene scene) {
        scene = GetLoadedScene();
        return scene is not null;
    }

    // public function to change loaded async scene 
    public void ChangeSceneToLoadedScene(Options options) {
        //todo transition options
        
        if (TryGetLoadedScene(out var scene)) {
            loadScenePath = "";
            ChangeScene(scene, options);
        }
    }

    #endregion

    // public change scene functions
    //todo add transitions
    #region ChangeScene

    public void ChangeScene(PackedScene scene, Options options) {
        if (ChangeScene(scene, options.addToHistory)) {
            EmitSignal(SignalName.SceneChanged);
        }
    }

    public void ChangeScene(Node scene, Options options) {
        if (ChangeScene(scene, options.addToHistory)) {
            RegisterSceneChangedToNodeAdded();
        }
    }
    
    public void ChangeScene(SceneAction action, Options options) {
        if (ChangeScene(action)) {
            EmitSignal(SignalName.SceneChanged);
        }
    }
    
    public void ChangeSceneWithPath(string scenePath, Options options) {
        if (ChangeSceneWithPath(scenePath, options.addToHistory)) {
            EmitSignal(SignalName.SceneChanged);
        }
    }
    
    public void ChangeScene(string sceneKey, Options options) {
        if (ChangeSceneWithKey(sceneKey, options.addToHistory)) {
            EmitSignal(SignalName.SceneChanged);
        }
    }

    #endregion
    
    ///// Private Functions /////
    
    //todo remove
    /// <summary>
    /// Quits the application
    /// </summary>
    private void Quit() {
        GetTree().Quit(0);
    }
    
    /// <summary>
    /// Reloads the current Scene
    /// </summary>
    /// <returns>true if sucessful, false if not</returns>
    private bool Restart() {
        if (sceneListCache.ContainsName(currentSceneKey)) {
            return ChangeSceneWithKey(currentSceneKey);    
        }
        return GetTree().ReloadCurrentScene() != Error.Failed;
    }
    
    /// <summary>
    /// Changes scene to the previous scene
    /// </summary>
    /// <returns>true if sucessful, false if not</returns>
    private bool Back() {
        if (TryPopSceneKey(out var popedSceneKey)) {
            return ChangeSceneWithKey(popedSceneKey, false);
        }
        
        return false;
    }
    
    private void SetInitialCurrentScene() {
        // var root_key: String = get_tree().current_scene.scene_file_path
        // for key in Scenes.scenes:
        // if key.begins_with("_"):
        // continue
        // if Scenes.scenes[key]["value"] == root_key:
        // _current_scene = key
        // assert (_current_scene != "", "Scene Manager Error: loaded scene is not defined in scene manager tool.")

        string rootKey = GetTree().CurrentScene.SceneFilePath;

        if (sceneListCache.TryGetSceneKey(rootKey, out var sceneKey)) {
            currentSceneKey = sceneKey;    
        }
    }

    private Node GetCurrentScene() {
        return GetTree().CurrentScene;
    }
    
    private void AppendSceneKeyToStack(string key) {
        //todo implement 
        // if _stack_limit == -1:
        // _stack.append(_current_scene)
        // elif _stack_limit > 0:
        // if _stack_limit <= len(_stack):
        // for i in range(len(_stack) - _stack_limit + 1):
        // _stack.pop_front()
        // _stack.append(_current_scene)
        // else:
        // _stack.append(_current_scene)
        // _current_scene = key
        sceneStack.Push(key);
    }

    private bool TryPopSceneKey(out string sceneKey) {
        // sceneKey = "";
        //
        // if (sceneStack.Count > 1) {
        //     sceneStack.Pop();
        //     sceneKey = sceneStack.Peek();
        //     return true;
        // }
        
        return sceneStack.TryPop(out sceneKey);
    }
    
    ///// "change scene" functions /////
    
    private bool ChangeScene(PackedScene scene, bool addToHistory = true) {
        GetTree().ChangeSceneToPacked(scene);

        if (sceneListCache.TryGetSceneKey(scene.ResourcePath, out var sceneKey)) {
            if (addToHistory) {
                AppendSceneKeyToStack(currentSceneKey);
            }

            currentSceneKey = sceneKey;
        }
        
        return true;
    }
    
    private bool ChangeScene(Node scene, bool addToHistory = true) {
        var root = GetTree().Root;
        root.GetChild(root.GetChildCount() - 1).Free();
        root.AddChild(scene);
        GetTree().CurrentScene = scene;

        if (sceneListCache.TryGetSceneKey(scene.SceneFilePath, out var sceneKey)) {
            if (addToHistory) {
                AppendSceneKeyToStack(currentSceneKey);
            }
            
            currentSceneKey = sceneKey;
        }
        
        return true;
    }
    
    private bool ChangeScene(SceneAction action) {
        switch (action) {
            case SceneAction.BACK:
                return Back();
            
            case SceneAction.REFRESH:
            case SceneAction.RESTART:
            case SceneAction.RELOAD:
                return Restart();
            
            case SceneAction.QUIT:
            case SceneAction.EXIT:
                Quit();
                return false;
            
            case SceneAction.RESET:
                ResetSceneStack();
                return true;
            
            case SceneAction.NULL:
            case SceneAction.IGNORE:
            default:
                return false;
        }
    }

    private bool ChangeSceneWithPath(string path, bool addToHistory = true) {
        if (sceneListCache.TryGetSceneKey(path, out var sceneKey)) {
            return ChangeSceneWithKey(sceneKey, addToHistory);
        }

        return false;
    }
    
    private bool ChangeSceneWithKey(string key, bool addToHistory = true) {
        try {
            if (ValidateSceneKey(key)) {
                GetTree().ChangeSceneToFile(sceneListCache.GetScenePath(key));    
            }
            else {
                return false;
            }

            if (addToHistory) {
                AppendSceneKeyToStack(currentSceneKey);
            }
            
            currentSceneKey = key;

            return true;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
            // return false;
        }
    }
    
    ///// Signal Functions /////

    private void TriggerSceneChanged() {
        EmitSignal(SignalName.SceneChanged);
    }

    private void TriggerSceneChangedOnce(Node node) {
        TriggerSceneChanged();
        UnregisterSceneChangedToNodeAdded();
    }
    
    private void RegisterSceneChangedToNodeAdded() {
        GetTree().NodeAdded += TriggerSceneChangedOnce;
    }

    private void UnregisterSceneChangedToNodeAdded() {
        GetTree().NodeAdded -= TriggerSceneChangedOnce;
    }
}