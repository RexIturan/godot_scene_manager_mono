#if TOOLS
namespace SceneManagerMono.Editor.Data;

public struct SceneDataTag {
    public int index = 0;
    public string name = "";
    public bool group = false;
    public bool scene = false;

    public static bool operator ==(SceneDataTag @this, SceneDataTag other) {
        return @this.index == other.index
               && @this.name == other.name
               && @this.group == other.group
               && @this.scene == other.scene;
    }
    
    public static bool operator !=(SceneDataTag @this, SceneDataTag other) {
        return !(@this == other);
    }
    
    public SceneDataTag() { }
}
#endif
