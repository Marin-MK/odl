using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odl;

/// <summary>
/// Stores and resolves fonts for repeated use.
/// </summary>
public static class FontCache
{
    /// <summary>
    /// The list of fonts in the cache.
    /// </summary>
    private static List<Font> Fonts = new List<Font>();

    /// <summary>
    /// Resolves or creates and caches the font that matches the specified name and size.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="size">The size of the font.</param>
    /// <returns>A <see cref="Font"/> object with the specified name and size.</returns>
    public static Font GetOrCreate(string name, int size)
    {
        Font f = TryGet(name, size);
        if (f is null)
        {
            f = new Font(name, size);
            Add(f);
        }
        return f;
    }

    /// <summary>
    /// Resolves the font that matches the specified name and size.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="size">The size of the font.</param>
    /// <returns>A <see cref="Font"/> object with the specified name and size.</returns>
    /// <exception cref="FontCacheException">If the font does not exist in the cache.</exception>
	public static Font Get(string name, int size)
	{
		Font f = TryGet(name, size);
		if (f is null) throw new FontCacheException($"No font '{name}' with size {size} was found in the cache.");
		return f;
	}

    /// <summary>
    /// Resolves the font that matches the specified name and size, or null if it does not exist in the cache.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="size">The size of the font.</param>
    /// <returns>A <see cref="Font"/> object with the specified name and size, or null if it does not exist in the cache.</returns>
	public static Font? TryGet(string name, int size)
    {
        Font? f = Fonts.Find(f => f.Name == name && f.Size == size);
        return f;
    }

    /// <summary>
    /// Whether the specified font exists in the cache.
    /// </summary>
    /// <param name="font">The font to test.</param>
    /// <returns>Whether the font exists in the cache.</returns>
    public static bool Contains(Font font)
    {
        return Fonts.Contains(font);
    }

    /// <summary>
    /// Whether a font with the specified name and size exists in the cache.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="size">The size of the font.</param>
    /// <returns>Whether a font exists in the cache with the specified name and size.</returns>
    public static bool Contains(string name, int size)
    {
        return Fonts.Any(f => f.Name == name && f.Size == size);
    }

    /// <summary>
    /// Adds a font to the cache.
    /// </summary>
    /// <param name="font">The font to add.</param>
    /// <exception cref="FontCacheException">If the font is already in the cache.</exception>
    public static void Add(Font font)
    {
        if (Contains(font)) throw new FontCacheException($"A font '{font.Name}' with size {font.Size} already exists in the cache.");
        Fonts.Add(font);
    }

    /// <summary>
    /// Removes a font from the cache.
    /// </summary>
    /// <param name="font">The font to remove.</param>
    /// <exception cref="FontCacheException">If the font does not exist in the cache.</exception>
    public static void Remove(Font font)
    {
        if (!Contains(font)) throw new FontCacheException($"No font '{font.Name}' with size {font.Size} exists in the cache.");
        Fonts.Remove(font);
    }

    /// <summary>
    /// Disposes all cached fonts and clears the cache.
    /// </summary>
    public static void Clear()
    {
        Fonts.ForEach(f => f.Dispose());
        Fonts.Clear();
    }
}

/// <summary>
/// Describes exceptions that may occur in the font cache.
/// </summary>
public class FontCacheException : FontException
{
    public FontCacheException(string message) : base(message) { }
}