﻿using System;

namespace odl;

public class FontResolver : FileResolver
{
	public FontResolver()
	{
		if (ODL.OnWindows)
		{
			AddPath("C:/Windows/Fonts");
		}
		else if (ODL.OnLinux)
		{
			AddPath("/usr/share/fonts");
			AddPath("/usr/local/share/fonts");
			AddPath("~/.fonts");
		}
		string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
		if (!ContainsPath(fontPath)) AddPath(fontPath);
		AddExtension(".ttf");
	}
}
