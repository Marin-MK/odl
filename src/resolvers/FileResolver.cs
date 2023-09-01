using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odl;

/// <summary>
/// Resolves relative filenames into absolute filenames if they exist.
/// </summary>
public class FileResolver
{
    /// <summary>
    /// A list of registered folders to search through when resolving files.
    /// </summary>
    private List<string> FolderPaths = new List<string>();
    /// <summary>
    /// A list of registered extensions to try when resolving files.
    /// </summary>
    private List<string> Extensions = new List<string>();

    /// <summary>
    /// Registers a folder to use for resolving files.
    /// </summary>
    /// <param name="path">The folder to look in.</param>
    /// <exception cref="FileResolverException">If the path was already registered.</exception>
    public void AddPath(string path)
    {
        path = path.Replace('\\', '/');
        if (ContainsPath(path)) throw new FileResolverException($"The path '{path}' is already part of the file resolver's folder paths.");
        FolderPaths.Add(path);
    }

    /// <summary>
    /// Whether a folder was already registered.
    /// </summary>
    /// <param name="path">The folder to test.</param>
    /// <returns>Whether the specified folder was already registered.</returns>
    public bool ContainsPath(string path)
    {
        return FolderPaths.Contains(path.Replace('\\', '/'));
    }

    /// <summary>
    /// Deregisters the folder for resolving files.
    /// </summary>
    /// <param name="path">The folder to deregister.</param>
    /// <exception cref="FileResolverException">If the path was not registered to begin with.</exception>
    public void RemovePath(string path)
    {
        path = path.Replace('\\', '/');
        if (!ContainsPath(path)) throw new FileResolverException($"The path '{path}' is not part of the file resolver's folder paths.");
        FolderPaths.Remove(path);
    }

    /// <summary>
    /// Registers an extension to look for when resolving files.
    /// </summary>
    /// <param name="ext">The extension to register. May or may not start with '.'</param>
    /// <exception cref="FileResolverException">If the extension is invalid or already registered.</exception>
    public void AddExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext) || ext == ".") throw new FileResolverException("Cannot add null or empty extensions.");
        if (!ext.StartsWith(".")) ext = "." + ext;
        ext = ext.ToLower();
        if (ContainsExtension(ext)) throw new FileResolverException($"The extension '{ext}' is already part of the file resolver's extensions.");
        Extensions.Add(ext);
    }

    /// <summary>
    /// Whether the extension was already registered.
    /// </summary>
    /// <param name="ext">The extension to test.</param>
    /// <returns>Whether the specified extension was already registered.</returns>
    public bool ContainsExtension(string ext)
    {
        if (!ext.StartsWith(".")) ext = "." + ext;
        ext = ext.ToLower();
        return Extensions.Contains(ext);
    }

    /// <summary>
    /// Deregisters the extension for resolving files.
    /// </summary>
    /// <param name="ext">The extension to deregister.</param>
    /// <exception cref="FileResolverException">If the extension is invalid or not registered.</exception>
    public void RemoveExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext) || ext == ".") throw new FileResolverException($"Cannot remove null or empty extensions.");
        if (!ext.StartsWith(".")) ext = "." + ext;
        ext = ext.ToLower();
        if (!ContainsExtension(ext)) throw new FileResolverException($"The extension '{ext}' is not part of the file resolver's extensions.");
        Extensions.Remove(ext);
    }

    /// <summary>
    /// Resolves a full filename based on a partial filename by looking through all registered folders and extensions with the default resolver strategy.
    /// </summary>
    /// <param name="filename"><The (partial) filename to resolve./param>
    /// <returns>The resolved filename, or null if it could not be found.</returns>
    public string? ResolveFilename(string filename)
    {
        return ResolveFilename(filename, FileResolverStrategy.Default);
    }

    /// <summary>
    /// Resolves a full filename based on a partial filename by looking through all registered folders and extensions with the specified resolver strategy.
    /// </summary>
    /// <param name="filename"><The (partial) filename to resolve./param>
    /// <param name="strategy">The strategy to use for resolving the filename.</param>
    /// <returns>The resolved filename, or null if it could not be found.</returns>
    public string? ResolveFilename(string filename, FileResolverStrategy strategy)
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

    /// <summary>
    /// Resolves a full filename based on a partial filename by looking through the specified folder for all extensions with the specified resolver strategy.
    /// </summary>
    /// <param name="folder">The folder to look through.</param>
    /// <param name="filename"><The (partial) filename to resolve./param>
    /// <param name="strategy">The strategy to use for resolving the filename.</param>
    /// <returns>The resolved filename, or null if it could not be found.</returns>
    public string? ResolveFilename(string folder, string filename, FileResolverStrategy strategy)
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

/// <summary>
/// Describes the strategy to use when attempting to resolve files.
/// </summary>
public enum FileResolverStrategy
{
    /// <summary>
    /// The most strict resolver strategy that only resolves an exact match.
    /// </summary>
    Strict = 0,
    /// <summary>
    /// Checks the specified filename and tries to append all registered extensions if it was not found.
    /// </summary>
    TryWithExtension = 1,
    /// <summary>
    /// Checks the specified filename and retries case-insensitively if it was not found.
    /// </summary>
    CaseInsensitive = 2,
    /// <summary>
    /// Checks the specified filename and counts partial matches if it was not found.
    /// </summary>
    PartialMatches = 4,
    /// <summary>
    /// Extends the search into subdirectories if it was not found.
    /// </summary>
    IncludeSubdirectories = 8,
    /// <summary>
    /// The default resolver strategy, which combines <see cref="TryWithExtension"/>, <see cref="CaseInsensitive"/>, <see cref="PartialMatches"/> and <see cref="IncludeSubdirectories"/>.
    /// </summary>
    Default = TryWithExtension | CaseInsensitive | PartialMatches | IncludeSubdirectories
}

/// <summary>
/// Describes exceptions that occur during the process of resolving a file.
/// </summary>
public class FileResolverException : Exception
{
    public FileResolverException(string message) : base(message) { }
}