#if TOOLS
using Godot;
using Godot.Collections;

[Tool]
public partial class SceneDataEditor : Container {
    private LineEdit keyLineEdit;
    private FileInput pathInput;
    private LineEdit displayNameLineEdit;
    private TextEdit descriptionTextEdit;
    private FileInput imageInput;
    private TextureRect imageTextureRect;
    private TagSelect tagSelect;

    private Button deleteButton;
    //todo tagSelect
    // private TagSelect tagSelect;
    private SceneDataItem _sceneData;
    public SceneDataItem SceneData {
        get => _sceneData;
        set {
            _sceneData = value;
            if (tagSelect is not null) {
                tagSelect.SelectedTags = new Array<string>(_sceneData.tags);    
            }
        }
    }

    ///// Godot Functions /////

    public override void _EnterTree() {
        base._EnterTree();
        
        keyLineEdit = GetNode<LineEdit>("%Key LineEdit");
        pathInput = GetNode<FileInput>("%Path File Input");
        displayNameLineEdit = GetNode<LineEdit>("%DisplayName LineEdit");
        descriptionTextEdit = GetNode<TextEdit>("%Description TextEdit");
        imageInput = GetNode<FileInput>("%Image File Input");
        imageTextureRect = GetNode<TextureRect>("%Image TextureRect");
        tagSelect ??= GetNode<TagSelect>("%TagSelect");
        
        deleteButton = GetNode<Button>("%Delete");
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed += UpdateInputs;
            SceneManagerEditor.Instance.IndicesChanged += UpdateData;
        }
    }

    public override void _Ready() {
        base._Ready();

        keyLineEdit.TextChanged += OnKeyChanged;
        pathInput.Changed += OnPathChanged;
        displayNameLineEdit.TextChanged += OnNameChanged;
        descriptionTextEdit.TextChanged += OnDescriptionChanged;
        imageInput.Changed += OnImageChanged;
        tagSelect.Changed += OnTagsChanged;
        
        deleteButton.Pressed += OnDelete;
        
        if (SceneManagerEditor.Exists()) {
            UpdateInputs();
        }
    }


    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateInputs;
            SceneManagerEditor.Instance.IndicesChanged -= UpdateData;
        }
    }

    ///// Private Callback Functions /////

    private void OnKeyChanged(string newKey) {
        SceneManagerEditor.Instance.SetSceneKey(SceneData.index, newKey);
    }
    
    private void OnPathChanged() {
        SceneManagerEditor.Instance.SetScenePath(SceneData.index, pathInput.Text);
    }
    
    private void OnNameChanged(string name) {
        SceneManagerEditor.Instance.SetSceneName(SceneData.index, name);
    }
    
    private void OnDescriptionChanged() {
        SceneManagerEditor.Instance.SetSceneDescription(SceneData.index, descriptionTextEdit.Text);
    }
    
    private void OnImageChanged() {
        SceneManagerEditor.Instance.SetSceneImage(SceneData.index, imageInput.Text);
    }
    
    private void OnTagsChanged(Array<string> newTags) {
        SceneManagerEditor.Instance.SetSceneTags(SceneData.index, newTags);
    }
    
    private void OnDelete() {
        SceneManagerEditor.Instance?.RequestDeleteScene(SceneData);
    }
    
    private void UpdateData() {
        var found = SceneManagerEditor.Instance.SceneDataItems.Find(item => item.key == SceneData.key);
        if (found is {}) {
            SceneData = found;
        }
    }
    
    private void UpdateInputs() {
        if (SceneManagerEditor.Exists()) {
            SceneData = SceneManagerEditor.Instance.SceneDataItems[SceneData.index];    
        }
        
        if (keyLineEdit.Text != SceneData.key) {
            keyLineEdit.Text = SceneData.key;    
        }
        
        if (pathInput.Text != SceneData.path) {
            pathInput.Text = SceneData.path;
            pathInput.SetValid(SceneData.IsPathValid());
        }
        
        if (displayNameLineEdit.Text != SceneData.name) {
            displayNameLineEdit.Text = SceneData.name;
        }

        if (descriptionTextEdit.Text != SceneData.description) {
            descriptionTextEdit.Text = SceneData.description;
        }

        if (imageInput.Text != SceneData.imagePath) {
            imageInput.Text = SceneData.imagePath;
        }
        
        imageTextureRect.Texture = SceneData.IsImagePathValid()
            ? ResourceLoader.Load<Texture2D>(SceneData.imagePath)
            : null;
    }
}
#endif
