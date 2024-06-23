#if TOOLS
using System.Collections.Generic;
using Godot;

namespace SceneManagerC.Addons.SceneManagerMono.Editor; 

public static class NodeExtensions {
    public static T GetFirstOrDefaultNode<T>(this Node root) {
        var children = root.GetChildren();

        foreach (var child in children) {
            if (child is T found) {
                return found;
            }
        }

        return default;
    }
    
    public static List<T> GetNodes<T>(this Node root) {
        var list = new List<T>();
        var children = root.GetChildren();

        foreach (var child in children) {
            if (child is T found) {
                list.Add(found);
            }
        }

        return list;
    }
    
    public static T GetFirstParentRecursive<T>(this Node node) {
        var parent = node.GetParent();

        if (parent is T found) {
            return found;
        }
        else if (parent is not null) {
            return parent.GetFirstParentRecursive<T>();
        }

        return default(T);
    }
}
#endif