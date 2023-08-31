using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odl;

public static class FontCache
{
    private static List<Font> Fonts = new List<Font>();

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

	public static Font Get(string name, int size)
	{
		Font f = TryGet(name, size);
		if (f is null) throw new FontCacheException($"No font '{name}' with size {size} was found in the cache.");
		return f;
	}

	public static Font TryGet(string name, int size)
    {
        Font f = Fonts.Find(f => f.Name == name && f.Size == size);
        return f;
    }

    public static bool Contains(Font font)
    {
        return Fonts.Contains(font);
    }

    public static void Add(Font font)
    {
        if (Contains(font)) throw new FontCacheException($"A font '{font.Name}' with size {font.Size} already exists in the cache.");
        Fonts.Add(font);
    }

    public static void Remove(Font font)
    {
        if (!Contains(font)) throw new FontCacheException($"No font '{font.Name}' with size {font.Size} exists in the cache.");
        Fonts.Remove(font);
    }

    public static void Clear()
    {
        Fonts.ForEach(f => f.Dispose());
        Fonts.Clear();
    }
}

public class FontCacheException : FontException
{
    public FontCacheException(string message) : base(message) { }
}