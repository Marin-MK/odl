using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odl;

public class FileResolver
{
    private List<string> FolderPaths = new List<string>();
    private List<string> Extensions = new List<string>();

    public void AddPath(string path)
    {
        path = path.Replace('\\', '/');
        if (ContainsPath(path)) throw new FileResolverException($"The path '{path}' is already part of the file resolver's folder paths.");
        FolderPaths.Add(path);
    }

    public bool ContainsPath(string path)
    {
        return FolderPaths.Contains(path.Replace('\\', '/'));
    }

    public void RemovePath(string path)
    {
        path = path.Replace('\\', '/');
        if (!ContainsPath(path)) throw new FileResolverException($"The path '{path}' is not part of the file resolver's folder paths.");
        FolderPaths.Remove(path);
    }

    public void AddExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext) || ext == ".") throw new FileResolverException("Cannot add null or empty extensions.");
        if (!ext.StartsWith(".")) ext = "." + ext;
        ext = ext.ToLower();
        if (ContainsExtension(ext)) throw new FileResolverException($"The extension '{ext}' is already part of the file resolver's extensions.");
        Extensions.Add(ext);
    }

    public bool ContainsExtension(string ext)
    {
        if (!ext.StartsWith(".")) ext = "." + ext;
        ext = ext.ToLower();
        return Extensions.Contains(ext);
    }

    public void RemoveExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext) || ext == ".") throw new FileResolverException($"Cannot remove null or empty extensions.");
        if (!ext.StartsWith(".")) ext = "." + ext;
        ext = ext.ToLower();
        if (!ContainsExtension(ext)) throw new FileResolverException($"The extension '{ext}' is not part of the file resolver's extensions.");
        Extensions.Remove(ext);
    }

    public string ResolveFilename(string filename)
    {
        return ResolveFilename(filename, FileResolverStrategy.Default);
    }

    public string ResolveFilename(string filename, FileResolverStrategy strategy)
    {
        string? parentFolder = Path.GetDirectoryName(filename);
        string? result = null;
        if (!string.IsNullOrEmpty(parentFolder))
        {
            string filenameEnding = Path.GetFileName(filename);
            result = ResolveFilename(parentFolder, filenameEnding, strategy);
            if (result is not null) return result;
        }
        result = ResolveFilename(Directory.GetCurrentDirectory(), filename, strategy);
        if (result is not null) return result;
        foreach (string filePath in FolderPaths)
        {
            result = ResolveFilename(filePath, filename, strategy);
            if (result is not null) return result;
        }
        return null;
    }

    public string ResolveFilename(string folder, string filename, FileResolverStrategy strategy)
    {
        bool tryWithExtension = (strategy & FileResolverStrategy.TryWithExtension) != 0;
        bool caseInsensitive = (strategy & FileResolverStrategy.CaseInsensitive) != 0;
        bool partialMatches = (strategy & FileResolverStrategy.PartialMatches) != 0;
        bool includeSubdirectories = (strategy & FileResolverStrategy.IncludeSubdirectories) != 0;
        string attemptedPath = Path.Combine(folder, filename);
        // Attempt strict strategy before anything else
        if (File.Exists(attemptedPath)) return attemptedPath.Replace('\\', '/');
        else if (tryWithExtension)
        {
            foreach (string ext in Extensions)
            {
                if (File.Exists(attemptedPath + ext)) return (attemptedPath + ext).Replace('\\', '/');
            }
        }
        // No perfect match was found; now consider other files in this directory if the Lenient strategy is applied
        foreach (string file in Directory.EnumerateFiles(folder))
        {
            // i.e. (Arial.ttf) -> arial.ttf == arial.ttf
            if (caseInsensitive && file.ToLower() == filename.ToLower()) return Path.Combine(folder, file).Replace('\\', '/');
            // i.e. (Arial) -> arial.ttf == arial(.ttf)
            if (caseInsensitive && tryWithExtension)
            {
                foreach (string ext in Extensions)
                {
                    if (file.ToLower() == filename.ToLower() + ext) return Path.Combine(folder, file).Replace('\\', '/');
                }
            }
            // i.e. (Arial) -> (Arial)-R.ttf
            if (partialMatches && file.Contains(filename)) return Path.Combine(folder, file).Replace('\\', '/');
            // i.e. (Arial) -> (arial)-r.ttf
            if (partialMatches && caseInsensitive && file.ToLower().Contains(filename.ToLower())) return Path.Combine(folder, file).Replace('\\', '/');
        }
        // Test subdirectories
        if (includeSubdirectories)
        {
            foreach (string directory in Directory.EnumerateDirectories(folder))
            {
                string result = ResolveFilename(Path.Combine(folder, directory), filename, strategy);
                if (result is not null) return result;
            }
        }
        return null;
    }
}

public enum FileResolverStrategy
{
    Strict = 0,
    TryWithExtension = 1,
    CaseInsensitive = 2,
    PartialMatches = 4,
    IncludeSubdirectories = 8,
    Default = 15
}

public class FileResolverException : Exception
{
    public FileResolverException(string message) : base(message) { }
}