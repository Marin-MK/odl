using System;

namespace odl;

public class ImageResolver : FileResolver
{
	public ImageResolver()
	{
		AddExtension(".png");
	}
}

