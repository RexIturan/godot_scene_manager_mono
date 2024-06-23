using Godot;

public enum FileType {
    Scene,
    Image,
    Directories,
    Resource,
    Any
}

public static class FileTypeExtension {
    public static FileDialog.FileModeEnum GetFileMode(this FileType fileType) {
        switch (fileType) {
            default:
            case FileType.Scene:
            case FileType.Image:
            case FileType.Resource:
                return FileDialog.FileModeEnum.OpenFile;
            case FileType.Directories:
                return FileDialog.FileModeEnum.OpenDir;
            case FileType.Any:
                return FileDialog.FileModeEnum.OpenAny;
        }
    }
    
    public static string GetExtensions(this FileType fileType) {
        var extension = "";
        switch (fileType) {
            case FileType.Resource:
                extension = "*.tres";
                break;
            case FileType.Scene:
                extension = "*.tscn, *.scn, *.res";
                break;
            case FileType.Image:
                extension = "*.svg, *.png";
                break;
        }

        return extension;
    }
    
    public static string GetFilterName(this FileType fileType) {
        var extension = "";
        switch (fileType) {
            case FileType.Resource:
                extension = "Resources";
                break;
            case FileType.Scene:
                extension = "Scene Files";
                break;
            case FileType.Image:
                extension = "Images";
                break;
            case FileType.Any:
                extension = "Any";
                break;
        }

        return extension;
    }
}