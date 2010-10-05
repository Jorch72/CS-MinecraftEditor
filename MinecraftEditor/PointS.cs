using System;

namespace MinecraftEditor
{
	public struct PointS
	{
		public short X { get; private set; }
		public short Y { get; private set; }
		
		public PointS(short x, short y) : this()
		{
			X = x; Y = y;
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is PointS)) return false;
			PointS p = (PointS)obj;
			return (p.X == X && p.Y == Y);
		}
		public override int GetHashCode()
		{
			return X ^ Y << 16;
		}
		public override string ToString()
		{
			return X + ":" + Y;
		}
	}
}
