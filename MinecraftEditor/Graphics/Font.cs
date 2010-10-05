using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace MinecraftEditor.Graphics
{
	public class Font
	{
		public Texture Texture { get; private set; }
		public int Colums { get; set; }
		public int Rows { get; set; }
		public int Spacing { get; set; }
		public int RowSpacing { get; set; }
		
		public Font(Texture texture)
		{
			Texture = texture;
			Colums = 16;
			Rows = 8;
			Spacing = 8;
			RowSpacing = 14;
		}
		
		public int GetWidth(string value)
		{
			int width = 0;
			foreach (string line in value.Split('\n'))
				width = Math.Max(width, GetLineWidth(value));
			return width;
		}
		int GetLineWidth(string value)
		{
			return value.Length * Spacing;
		}
		public int GetHeight(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if (value == string.Empty) return 0;
			return (value.Split('\n').Length * RowSpacing);
		}
		
		public void Write(int x, int y, string value)
		{
			Write(x, y, value, new TextAlign(HorAlign.Left, VerAlign.Top));
		}
		public void Write(int x, int y, string value, TextAlign align)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if (value == string.Empty) return;
			int w = Texture.Width / Colums;
			int h = Texture.Height / Rows;
			Display.Texture = Texture;
			GL.Begin(BeginMode.Quads);
			int yy = y - GetHeight(value) * ((int)align.VerAlign + 1) / 2;
			foreach (string line in value.Split('\n')) {
				int xx = x - GetWidth(line) * ((int)align.HorAlign + 1) / 2;
				foreach (char c in line) {
					int i = (int)c;
					if (i > Colums * Rows) i = 0;
					RectangleF tex = new RectangleF(
						(i % Colums) / (float)Colums,
						(i / Colums) / (float)Rows,
						1.0f / Colums, 1.0f / Rows);
					GL.TexCoord2(tex.Left,  tex.Top);    GL.Vertex2(xx,     yy);
					GL.TexCoord2(tex.Left,  tex.Bottom); GL.Vertex2(xx,     yy + h);
					GL.TexCoord2(tex.Right, tex.Bottom); GL.Vertex2(xx + w, yy + h);
					GL.TexCoord2(tex.Right, tex.Top);    GL.Vertex2(xx + w, yy);
					xx += Spacing;
				}
				yy += RowSpacing;
			}
			GL.End();
		}
	}
}
