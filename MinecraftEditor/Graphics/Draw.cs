using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace MinecraftEditor.Graphics
{
	public static class Draw
	{
		#region FilledRectangle
		public static void FilledRectangle(Rectangle rect)
		{
			FilledRectangle(rect, new RectangleF(0.0f, 0.0f, 1.0f, 1.0f));
		}
		public static void FilledRectangle(Rectangle rect, RectangleF texCoords)
		{
			FilledRectangle(rect.X, rect.Y, rect.Width, rect.Height, texCoords);
		}
		public static void FilledRectangle(Rectangle rect, Texture tex)
		{
			FilledRectangle(rect, tex, new RectangleF(0.0f, 0.0f, 1.0f, 1.0f));
		}
		public static void FilledRectangle(Rectangle rect, Texture tex, RectangleF texCoords)
		{
			FilledRectangle(rect.X, rect.Y, rect.Width, rect.Height, tex, texCoords);
		}
		public static void FilledRectangle(int x, int y, int width, int height)
		{
			FilledRectangle(x, y, width, height, new RectangleF(0.0f, 0.0f, 1.0f, 1.0f));
		}
		public static void FilledRectangle(int x, int y, int width, int height, RectangleF texCoords)
		{
			GL.Begin(BeginMode.Quads);
			GL.TexCoord2(texCoords.Left,  texCoords.Top);    GL.Vertex2(x, y);
			GL.TexCoord2(texCoords.Right, texCoords.Top);    GL.Vertex2(x + width, y);
			GL.TexCoord2(texCoords.Right, texCoords.Bottom); GL.Vertex2(x + width, y + height);
			GL.TexCoord2(texCoords.Left,  texCoords.Bottom); GL.Vertex2(x, y + height);
			GL.End();
		}
		public static void FilledRectangle(int x, int y, int width, int height, Texture tex)
		{
			FilledRectangle(x, y, width, height, tex, new RectangleF(0.0f, 0.0f, 1.0f, 1.0f));
		}
		public static void FilledRectangle(int x, int y, int width, int height, Texture tex, RectangleF texCoords)
		{
			Display.Texture = tex;
			FilledRectangle(x, y, width, height, texCoords);
		}
		#endregion
		#region Rectangle
		public static void Rectangle(Rectangle rect)
		{
			Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}
		public static void Rectangle(int x, int y, int width, int height)
		{
			GL.Begin(BeginMode.LineLoop);
			GL.Vertex2(x, y);
			GL.Vertex2(x + width, y);
			GL.Vertex2(x + width, y + height);
			GL.Vertex2(x, y + height);
			GL.End();
		}
		#endregion
		#region Line
		public static void Line(Point p1, Point p2)
		{
			Line(p1.X, p1.Y, p2.X, p2.Y);
		}
		public static void Line(int x1, int y1, int x2, int y2)
		{
			GL.Begin(BeginMode.Lines);
			GL.Vertex2(x1, y1);
			GL.Vertex2(x2, y2);
			GL.End();
		}
		#endregion
	}
}
