using System;

namespace MinecraftEditor.Graphics
{
	public struct TextAlign
	{
		public HorAlign HorAlign { get; private set; }
		public VerAlign VerAlign { get; private set; }
		
		public TextAlign(HorAlign horAlign, VerAlign verAlign) : this()
		{
			if ((int)horAlign < -1 || (int)horAlign > 1)
				throw new ArgumentOutOfRangeException("horAlign");
			if ((int)verAlign < -1 || (int)verAlign > 1)
				throw new ArgumentOutOfRangeException("verAlign");
			HorAlign = horAlign;
			VerAlign = verAlign;
		}
	}
	public enum HorAlign : sbyte
	{
		Left = -1,
		Center = 0,
		Right = 1
	}
	public enum VerAlign : sbyte
	{
		Top = -1,
		Middle = 0,
		Bottom = 1
	}
}
