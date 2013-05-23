using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	public interface IPicrossGridView : IGridView<PicrossCell>
	{
	}
}

