using Godot;

#if TOOLS
public static class GodotHelper {
    public static T LoadAndInstaniate<T>(string path, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse) where T : Node {
        var packedScene =
            ResourceLoader.Load<PackedScene>(path, null, cacheMode);
        return packedScene.Instantiate<T>();
    }    
}
#endif 
