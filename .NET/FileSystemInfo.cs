namespace InteractiveServer;

public record FileSystemInfo
{
    public FileSystemInfo(DriveInfo drive)
    {
        Name      = Path = drive.Name;
        IsFile    = false;
        Extension = string.Empty;
        Display   = drive.VolumeLabel;
    }

    public FileSystemInfo(string path)
    {
        Path      = path;
        Name      = Display = System.IO.Path.GetFileName(path);
        IsFile    = File.Exists(path);
        Extension = System.IO.Path.GetExtension(path);
    }

    public string Name      { get; set; }
    public string Path      { get; set; }
    public bool   IsFile    { get; set; }
    public string Extension { get; set; }
    public string Display   { get; set; }

    public static implicit operator FileSystemInfo(string path) => new(path);
}