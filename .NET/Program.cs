using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseKestrel(x =>
{
    x.Limits.MaxRequestBodySize = long.MaxValue;
    x.ListenAnyIP(int.TryParse(x.ApplicationServices.GetRequiredService<IConfiguration>()["Port"], out var port)
        ? port
        : 5310);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppSerializerContext.Default);
});

builder.Services.AddAntiforgery();

var app = builder.Build();

var views = Path.Combine(AppContext.BaseDirectory, "Views");
var index = Path.Combine(views, "index.html");

app.MapGet("/", () => Results.File(File.ReadAllBytes(index), MediaTypeNames.Text.Html));
app.MapGet("/view/{**rest}", (string rest) =>
{
    return Results.File(File.ReadAllBytes(Path.Combine(views, rest)), Path.GetExtension(rest) switch
    {
        ".js"  => MediaTypeNames.Text.JavaScript,
        ".css" => MediaTypeNames.Text.Css,
        ".svg" => MediaTypeNames.Image.Svg,
        _      => null
    });
});
var upload = app.MapGroup("/upload");

upload.MapPost("/{**rest}", async (HttpRequest request, [FromRoute] string rest) =>
await Save(request.Body, rest));

var tree = app.MapGroup("/tree");

tree.MapGet("/", () =>
    Results.Json(DriveInfo
        .GetDrives()
        .Select(static x => new FileSystemInfo(x))));

tree.MapGet("/{**rest}",
    ([FromRoute] string rest) =>
    {
        try
        {
            return File.Exists(rest)
                ? Results.Stream(File.OpenRead(rest))
                : Directory.Exists(rest)
                    ? Results.Json(Directory.GetFileSystemEntries(rest)
                        .Select(x => (FileSystemInfo)x)
                        .ToArray())
                    : Results.NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
    });
app.UseAntiforgery();
app.Run();
return;

static async Task<IResult> Save(Stream fileStream, string path)
{
    if (!Path.IsPathRooted(path)) path = Path.Combine(AppContext.BaseDirectory, "wwwroot", DateTime.Today.ToString("yyyy-M-d") , path);
    var dir                            = Path.GetDirectoryName(path);
    if (dir is null) return Results.Forbid();
    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    await using var stream = File.Create(path);
    await fileStream.CopyToAsync(stream);
    return Results.Ok();
}

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


[JsonSerializable(typeof(FileSystemInfo[]))]
public partial class AppSerializerContext : JsonSerializerContext;