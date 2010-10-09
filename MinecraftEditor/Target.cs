using System;

namespace MinecraftEditor
{
	public class Target
	{
		public int X { get; private set; }
		public int Y { get; private set; }
		public int Z { get; private set; }
		public Side Side { get; private set; }
		
		public Target(int x, int y, int z, Side side)
		{
			X = x; Y = y; Z = z;
			Side = side;
		}
		
		public void GetSide(out int x, out int y, out int z)
		{
			x = X; y = Y; z = Z;
			switch (Side) {
				case Side.Right:  x++; break;
				case Side.Left:   x--; break;
				case Side.Top:    y++; break;
				case Side.Bottom: y--; break;
				case Side.Front:  z++; break;
				case Side.Back:   z--; break;
			}
		}
	}
}
