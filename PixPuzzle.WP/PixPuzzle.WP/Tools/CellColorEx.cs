using Microsoft.Xna.Framework;

namespace PixPuzzle.Data
{
    public static class CellColorEx
    {
        public static Color ToXnaColor(this CellColor color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }
    }
}
