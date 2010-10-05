using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MinecraftEditor.Minecraft;

namespace MinecraftEditor.Graphics
{
	public static class BlockRenderer
	{
		public static void Render(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			if (info == null) return;
			switch (info.DrawMode) {
				case DrawMode.Block:
					RenderBlock(chunk, info, x, y, z); break;
				case DrawMode.Plant:
					RenderPlant(chunk, info, x, y, z); break;
				case DrawMode.Glass:
					RenderGlass(chunk, info, x, y, z); break;
				case DrawMode.StoneSlab:
					RenderStoneSlab(chunk, info, x, y, z); break;
				case DrawMode.Torch:
					RenderTorch(chunk, info, x, y, z); break;
				case DrawMode.Stairs:
					RenderStairs(chunk, info, x, y, z); break;
				case DrawMode.Crop:
					RenderCrop(chunk, info, x, y, z); break;
				case DrawMode.Soil:
					RenderSoil(chunk, info, x, y, z); break;
				case DrawMode.Chest:
					RenderChest(chunk, info, x, y, z); break;
				case DrawMode.Furnace:
					RenderFurnace(chunk, info, x, y, z); break;
			}
		}
		public static void RenderPicking(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			if (info == null) return;
			switch (info.DrawMode) {
				case DrawMode.Block:
				case DrawMode.Plant:
				case DrawMode.Glass:
				case DrawMode.StoneSlab:
				case DrawMode.Torch:
				case DrawMode.Stairs:
				case DrawMode.Crop:
				case DrawMode.Soil:
				case DrawMode.Chest:
				case DrawMode.Furnace:
					RenderPickingBlock(chunk, info, x, y, z);
					break;
			}
		}
		
		static void GetAdjacentBlocks(Chunk chunk, int x, int y, int z,
		                              out BlocktypeInfo right, out BlocktypeInfo left,
		                              out BlocktypeInfo top, out BlocktypeInfo bottom,
		                              out BlocktypeInfo front, out BlocktypeInfo back)
		{
			BlocktypeInfo air = BlocktypeInfo.Find(Blocktype.Air);
			right  = GetAdjacentBlock(chunk, x + 1, y, z) ?? air;
			left   = GetAdjacentBlock(chunk, x - 1, y, z) ?? air;
			top    = GetAdjacentBlock(chunk, x, y + 1, z) ?? air;
			bottom = GetAdjacentBlock(chunk, x, y - 1, z) ?? air;
			front  = GetAdjacentBlock(chunk, x, y, z + 1) ?? air;
			back   = GetAdjacentBlock(chunk, x, y, z - 1) ?? air;
		}
		static BlocktypeInfo GetAdjacentBlock(Chunk chunk, int x, int y, int z)
		{
			if (chunk == null || y < 0 || y >= Chunk.Depth) return null;
			if (x == -1) { x = Chunk.Width - 1; chunk = chunk.Left; }
			else if (x == Chunk.Width) { x = 0; chunk = chunk.Right; }
			else if (z == -1) { z = Chunk.Height - 1; chunk = chunk.Back; }
			else if (z == Chunk.Height) { z = 0; chunk = chunk.Front; }
			if (chunk == null) return null;
			return BlocktypeInfo.Find(chunk.GetBlocktype(x, y, z));
		}
		static void Light(Chunk chunk, int x, int y, int z)
		{
			if (chunk == null) return;
			double blockLight = chunk.GetBlockLight(x, y, z) / 16.0;
			double skyLight = (chunk.GetSkyLight(x, y, z) / 16.0) * (chunk.World.Light / 16.0);
			double light = Math.Max(Math.Pow(Math.Max(blockLight, skyLight), 2), chunk.World.MinimumLight / 16.0);
			GL.Color3(light, light, light);
		}
		static void AdjacentLight(Chunk chunk, int x, int y, int z)
		{
			if (chunk == null || y < 0 || y >= Chunk.Depth) return;
			if (x == -1) { x = Chunk.Width - 1; chunk = chunk.Left; }
			else if (x == Chunk.Width) { x = 0; chunk = chunk.Right; }
			else if (z == -1) { z = Chunk.Height - 1; chunk = chunk.Back; }
			else if (z == Chunk.Height) { z = 0; chunk = chunk.Front; }
			if (chunk == null) return;
			Light(chunk, x, y, z);
		}
		
		static void RenderBlock(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			if (!right.Opaque) {
				AdjacentLight(chunk, x + 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Left.Right, info.Left.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Left.Left,  info.Left.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(info.Left.Left,  info.Left.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
				GL.TexCoord2(info.Left.Right, info.Left.Bottom); GL.Vertex3(x+1.0, y,     z);
			}
			if (!left.Opaque) {
				AdjacentLight(chunk, x - 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Right.Right, info.Right.Top);    GL.Vertex3(x, y+1.0, z+1.0);
				GL.TexCoord2(info.Right.Left,  info.Right.Top);    GL.Vertex3(x, y+1.0, z);
				GL.TexCoord2(info.Right.Left,  info.Right.Bottom); GL.Vertex3(x, y,     z);
				GL.TexCoord2(info.Right.Right, info.Right.Bottom); GL.Vertex3(x, y,     z+1.0);
			}
			if (!top.Opaque) {
				AdjacentLight(chunk, x, y + 1, z);
				GL.Normal3(0.0, 1.0, 0.0);
				GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x+1.0, y+1.0, z+1.0);
			}
			if (!bottom.Opaque) {
				AdjacentLight(chunk, x, y - 1, z);
				GL.Normal3(0.0, -1.0, 0.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Top);    GL.Vertex3(x,     y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Top);    GL.Vertex3(x+1.0, y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Bottom); GL.Vertex3(x+1.0, y, z+1.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Bottom); GL.Vertex3(x,     y, z+1.0);
			}
			if (!front.Opaque) {
				AdjacentLight(chunk, x, y, z + 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Top);    GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Bottom); GL.Vertex3(x,     y,     z+1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
			}
			if (!back.Opaque) {
				AdjacentLight(chunk, x, y, z - 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Back.Right, info.Back.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Bottom); GL.Vertex3(x+1.0, y,     z);
				GL.TexCoord2(info.Back.Right, info.Back.Bottom); GL.Vertex3(x,     y,     z);
			}
		}
		static void RenderPlant(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			Light(chunk, x, y, z);
			GL.Normal3(0.0, 1.0, 0.0);
			GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x,     y+1.0, z);
			GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
			GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
			GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x,     y,     z);
			GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
			GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x,     y+1.0, z);
			GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x,     y,     z);
			GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
			GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z);
			GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x,     y+1.0, z+1.0);
			GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x,     y,     z+1.0);
			GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x+1.0, y,     z);
			GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x,     y+1.0, z+1.0);
			GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z);
			GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x+1.0, y,     z);
			GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x,     y,     z+1.0);
		}
		static void RenderGlass(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			if (!right.Opaque && right.Type != info.Type) {
				AdjacentLight(chunk, x + 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Left.Right, info.Left.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Left.Left,  info.Left.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(info.Left.Left,  info.Left.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
				GL.TexCoord2(info.Left.Right, info.Left.Bottom); GL.Vertex3(x+1.0, y,     z);
			}
			if (!left.Opaque && left.Type != info.Type) {
				AdjacentLight(chunk, x - 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Right.Right, info.Right.Top);    GL.Vertex3(x, y+1.0, z+1.0);
				GL.TexCoord2(info.Right.Left,  info.Right.Top);    GL.Vertex3(x, y+1.0, z);
				GL.TexCoord2(info.Right.Left,  info.Right.Bottom); GL.Vertex3(x, y,     z);
				GL.TexCoord2(info.Right.Right, info.Right.Bottom); GL.Vertex3(x, y,     z+1.0);
			}
			if (!top.Opaque && top.Type != info.Type) {
				AdjacentLight(chunk, x, y + 1, z);
				GL.Normal3(0.0, 1.0, 0.0);
				GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x+1.0, y+1.0, z+1.0);
			}
			if (!bottom.Opaque && bottom.Type != info.Type) {
				AdjacentLight(chunk, x, y - 1, z);
				GL.Normal3(0.0, -1.0, 0.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Top);    GL.Vertex3(x,     y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Top);    GL.Vertex3(x+1.0, y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Bottom); GL.Vertex3(x+1.0, y, z+1.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Bottom); GL.Vertex3(x,     y, z+1.0);
			}
			if (!front.Opaque && front.Type != info.Type) {
				AdjacentLight(chunk, x, y, z + 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Top);    GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Bottom); GL.Vertex3(x,     y,     z+1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
			}
			if (!back.Opaque && back.Type != info.Type) {
				AdjacentLight(chunk, x, y, z - 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Back.Right, info.Back.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Bottom); GL.Vertex3(x+1.0, y,     z);
				GL.TexCoord2(info.Back.Right, info.Back.Bottom); GL.Vertex3(x,     y,     z);
			}
		}
		static void RenderStoneSlab(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			if (!right.Opaque) {
				AdjacentLight(chunk, x + 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Left.Right, info.Left.Top + 1/32.0); GL.Vertex3(x+1.0, y+0.5, z);
				GL.TexCoord2(info.Left.Left,  info.Left.Top + 1/32.0); GL.Vertex3(x+1.0, y+0.5, z+1.0);
				GL.TexCoord2(info.Left.Left,  info.Left.Bottom);       GL.Vertex3(x+1.0, y,     z+1.0);
				GL.TexCoord2(info.Left.Right, info.Left.Bottom);       GL.Vertex3(x+1.0, y,     z);
			}
			if (!left.Opaque) {
				AdjacentLight(chunk, x - 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Right.Right, info.Right.Top + 1/32.0); GL.Vertex3(x, y+0.5, z+1.0);
				GL.TexCoord2(info.Right.Left,  info.Right.Top + 1/32.0); GL.Vertex3(x, y+0.5, z);
				GL.TexCoord2(info.Right.Left,  info.Right.Bottom);       GL.Vertex3(x, y,     z);
				GL.TexCoord2(info.Right.Right, info.Right.Bottom);       GL.Vertex3(x, y,     z+1.0);
			}
			Light(chunk, x, y, z);
			GL.Normal3(0.0, 1.0, 0.0);
			GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x+1.0, y+0.5, z);
			GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x,     y+0.5, z);
			GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x,     y+0.5, z+1.0);
			GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x+1.0, y+0.5, z+1.0);
			if (!bottom.Opaque) {
				AdjacentLight(chunk, x, y - 1, z);
				GL.Normal3(0.0, -1.0, 0.0);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Top);    GL.Vertex3(x,     y, z+1.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Top);    GL.Vertex3(x,     y, z);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Bottom); GL.Vertex3(x+1.0, y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Bottom); GL.Vertex3(x+1.0, y, z+1.0);
			}
			if (!front.Opaque) {
				AdjacentLight(chunk, x, y, z + 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Top + 1/32.0); GL.Vertex3(x+1.0, y+0.5, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Top + 1/32.0); GL.Vertex3(x,     y+0.5, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Bottom);       GL.Vertex3(x,     y,     z+1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Bottom);       GL.Vertex3(x+1.0, y,     z+1.0);
			}
			if (!back.Opaque) {
				AdjacentLight(chunk, x, y, z - 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Back.Right, info.Back.Top + 1/32.0); GL.Vertex3(x,     y+0.5, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Top + 1/32.0); GL.Vertex3(x+1.0, y+0.5, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Bottom);       GL.Vertex3(x+1.0, y,     z);
				GL.TexCoord2(info.Back.Right, info.Back.Bottom);       GL.Vertex3(x,     y,     z);
			}
		}
		static void RenderTorch(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			double x1 = 0, x2 = 0, z1 = 0, z2 = 0, yy = 0;
			byte data = chunk.GetData(x, y, z);
			switch (data) {
				case 1:
					x1 -= 0.125; x2 -= 0.5;
					yy = 3/16.0; break;
				case 2:
					x1 += 0.125; x2 += 0.5;
					yy = 3/16.0; break;
				case 3:
					z1 -= 0.125; z2 -= 0.5;
					yy = 3/16.0; break;
				case 4:
					z1 += 0.125; z2 += 0.5;
					yy = 3/16.0; break;
			}
			Light(chunk, x, y, z);
			GL.Normal3(1.0, 0.0, 0.0);
			GL.TexCoord2(info.Left.Right, info.Left.Top);    GL.Vertex3(x+x1+9/16.0, y+yy+1.0, z+z1);
			GL.TexCoord2(info.Left.Left,  info.Left.Top);    GL.Vertex3(x+x1+9/16.0, y+yy+1.0, z+z1+1.0);
			GL.TexCoord2(info.Left.Left,  info.Left.Bottom); GL.Vertex3(x+x2+9/16.0, y+yy,     z+z2+1.0);
			GL.TexCoord2(info.Left.Right, info.Left.Bottom); GL.Vertex3(x+x2+9/16.0, y+yy,     z+z2);
			GL.TexCoord2(info.Right.Right, info.Right.Top);      GL.Vertex3(x+x1+7/16.0, y+yy+1.0, z+z1+1.0);
			GL.TexCoord2(info.Right.Left,  info.Right.Top);      GL.Vertex3(x+x1+7/16.0, y+yy+1.0, z+z1);
			GL.TexCoord2(info.Right.Left,  info.Right.Bottom);   GL.Vertex3(x+x2+7/16.0, y+yy,     z+z2);
			GL.TexCoord2(info.Right.Right, info.Right.Bottom);   GL.Vertex3(x+x2+7/16.0, y+yy,     z+z2+1.0);
			GL.Normal3(0.0, 0.0, 1.0);
			GL.TexCoord2(info.Front.Right, info.Front.Top);    GL.Vertex3(x+x1+1.0, y+yy+1.0, z+z1+9/16.0);
			GL.TexCoord2(info.Front.Left,  info.Front.Top);    GL.Vertex3(x+x1,     y+yy+1.0, z+z1+9/16.0);
			GL.TexCoord2(info.Front.Left,  info.Front.Bottom); GL.Vertex3(x+x2,     y+yy,     z+z2+9/16.0);
			GL.TexCoord2(info.Front.Right, info.Front.Bottom); GL.Vertex3(x+x2+1.0, y+yy,     z+z2+9/16.0);
			GL.TexCoord2(info.Back.Right, info.Back.Top);      GL.Vertex3(x+x1,     y+yy+1.0, z+z1+7/16.0);
			GL.TexCoord2(info.Back.Left,  info.Back.Top);      GL.Vertex3(x+x1+1.0, y+yy+1.0, z+z1+7/16.0);
			GL.TexCoord2(info.Back.Left,  info.Back.Bottom);   GL.Vertex3(x+x2+1.0, y+yy,     z+z2+7/16.0);
			GL.TexCoord2(info.Back.Right, info.Back.Bottom);   GL.Vertex3(x+x2,     y+yy,     z+z2+7/16.0);
			x1 *= 2.125; z1 *= 2.125;
			GL.Normal3(0.0, 1.0, 0.0);
			GL.TexCoord2(info.Top.Right-7/256.0, info.Top.Top+6/256.0);    GL.Vertex3(x+x1+9/16.0, y+yy+10/16.0, z+z1+7/16.0);
			GL.TexCoord2(info.Top.Left+7/256.0,  info.Top.Top+6/256.0);    GL.Vertex3(x+x1+7/16.0, y+yy+10/16.0, z+z1+7/16.0);
			GL.TexCoord2(info.Top.Left+7/256.0,  info.Top.Bottom-8/256.0); GL.Vertex3(x+x1+7/16.0, y+yy+10/16.0, z+z1+9/16.0);
			GL.TexCoord2(info.Top.Right-7/256.0, info.Top.Bottom-8/256.0); GL.Vertex3(x+x1+9/16.0, y+yy+10/16.0, z+z1+9/16.0);
		}
		static void RenderCrop(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			Light(chunk, x, y, z);
			GL.Normal3(0.0, 1.0, 0.0);
			byte data = chunk.GetData(x, y, z);
			GL.TexCoord2(info.Right.Right+data/16.0, info.Right.Top);    GL.Vertex3(x+0.25, y+1.0-1/16.0, z+1.0);
			GL.TexCoord2(info.Right.Left+data/16.0,  info.Right.Top);    GL.Vertex3(x+0.25, y+1.0-1/16.0, z);
			GL.TexCoord2(info.Right.Left+data/16.0,  info.Right.Bottom); GL.Vertex3(x+0.25, y-1/16.0,     z);
			GL.TexCoord2(info.Right.Right+data/16.0, info.Right.Bottom); GL.Vertex3(x+0.25, y-1/16.0,     z+1.0);
			GL.TexCoord2(info.Right.Left+data/16.0,  info.Right.Top);    GL.Vertex3(x+0.25, y+1.0-1/16.0, z);
			GL.TexCoord2(info.Right.Right+data/16.0, info.Right.Top);    GL.Vertex3(x+0.25, y+1.0-1/16.0, z+1.0);
			GL.TexCoord2(info.Right.Right+data/16.0, info.Right.Bottom); GL.Vertex3(x+0.25, y-1/16.0,     z+1.0);
			GL.TexCoord2(info.Right.Left+data/16.0,  info.Right.Bottom); GL.Vertex3(x+0.25, y-1/16.0,     z);
			GL.TexCoord2(info.Left.Right+data/16.0, info.Left.Top);    GL.Vertex3(x+0.75, y+1.0-1/16.0, z);
			GL.TexCoord2(info.Left.Left+data/16.0,  info.Left.Top);    GL.Vertex3(x+0.75, y+1.0-1/16.0, z+1.0);
			GL.TexCoord2(info.Left.Left+data/16.0,  info.Left.Bottom); GL.Vertex3(x+0.75, y-1/16.0,     z+1.0);
			GL.TexCoord2(info.Left.Right+data/16.0, info.Left.Bottom); GL.Vertex3(x+0.75, y-1/16.0,     z);
			GL.TexCoord2(info.Left.Left+data/16.0,  info.Left.Top);    GL.Vertex3(x+0.75, y+1.0-1/16.0, z+1.0);
			GL.TexCoord2(info.Left.Right+data/16.0, info.Left.Top);    GL.Vertex3(x+0.75, y+1.0-1/16.0, z);
			GL.TexCoord2(info.Left.Right+data/16.0, info.Left.Bottom); GL.Vertex3(x+0.75, y-1/16.0,     z);
			GL.TexCoord2(info.Left.Left+data/16.0,  info.Left.Bottom); GL.Vertex3(x+0.75, y-1/16.0,     z+1.0);
			GL.TexCoord2(info.Front.Right+data/16.0, info.Front.Top);    GL.Vertex3(x+1.0, y+1.0-1/16.0, z+0.75);
			GL.TexCoord2(info.Front.Left+data/16.0,  info.Front.Top);    GL.Vertex3(x,     y+1.0-1/16.0, z+0.75);
			GL.TexCoord2(info.Front.Left+data/16.0,  info.Front.Bottom); GL.Vertex3(x,     y-1/16.0,     z+0.75);
			GL.TexCoord2(info.Front.Right+data/16.0, info.Front.Bottom); GL.Vertex3(x+1.0, y-1/16.0,     z+0.75);
			GL.TexCoord2(info.Front.Left+data/16.0,  info.Front.Top);    GL.Vertex3(x,     y+1.0-1/16.0, z+0.75);
			GL.TexCoord2(info.Front.Right+data/16.0, info.Front.Top);    GL.Vertex3(x+1.0, y+1.0-1/16.0, z+0.75);
			GL.TexCoord2(info.Front.Right+data/16.0, info.Front.Bottom); GL.Vertex3(x+1.0, y-1/16.0,     z+0.75);
			GL.TexCoord2(info.Front.Left+data/16.0,  info.Front.Bottom); GL.Vertex3(x,     y-1/16.0,     z+0.75);
			GL.TexCoord2(info.Back.Right+data/16.0, info.Back.Top);    GL.Vertex3(x,     y+1.0-1/16.0, z+0.25);
			GL.TexCoord2(info.Back.Left+data/16.0,  info.Back.Top);    GL.Vertex3(x+1.0, y+1.0-1/16.0, z+0.25);
			GL.TexCoord2(info.Back.Left+data/16.0,  info.Back.Bottom); GL.Vertex3(x+1.0, y-1/16.0,     z+0.25);
			GL.TexCoord2(info.Back.Right+data/16.0, info.Back.Bottom); GL.Vertex3(x,     y-1/16.0,     z+0.25);
			GL.TexCoord2(info.Back.Left+data/16.0,  info.Back.Top);    GL.Vertex3(x+1.0, y+1.0-1/16.0, z+0.25);
			GL.TexCoord2(info.Back.Right+data/16.0, info.Back.Top);    GL.Vertex3(x,     y+1.0-1/16.0, z+0.25);
			GL.TexCoord2(info.Back.Right+data/16.0, info.Back.Bottom); GL.Vertex3(x,     y-1/16.0,     z+0.25);
			GL.TexCoord2(info.Back.Left+data/16.0,  info.Back.Bottom); GL.Vertex3(x+1.0, y-1/16.0,     z+0.25);
		}
		static void RenderSoil(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			byte wet = Math.Min(chunk.GetData(x, y, z), (byte)1);
			if (!right.Opaque) {
				AdjacentLight(chunk, x + 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Left.Right, info.Left.Top+1/256.0); GL.Vertex3(x+1.0, y+1.0-1/16.0, z);
				GL.TexCoord2(info.Left.Left,  info.Left.Top+1/256.0); GL.Vertex3(x+1.0, y+1.0-1/16.0, z+1.0);
				GL.TexCoord2(info.Left.Left,  info.Left.Bottom);      GL.Vertex3(x+1.0, y,            z+1.0);
				GL.TexCoord2(info.Left.Right, info.Left.Bottom);      GL.Vertex3(x+1.0, y,            z);
			}
			if (!left.Opaque) {
				AdjacentLight(chunk, x - 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(info.Right.Right, info.Right.Top+1/256.0); GL.Vertex3(x, y+1.0-1/16.0, z+1.0);
				GL.TexCoord2(info.Right.Left,  info.Right.Top+1/256.0); GL.Vertex3(x, y+1.0-1/16.0, z);
				GL.TexCoord2(info.Right.Left,  info.Right.Bottom);      GL.Vertex3(x, y,            z);
				GL.TexCoord2(info.Right.Right, info.Right.Bottom);      GL.Vertex3(x, y,            z+1.0);
			}
			Light(chunk, x, y, z);
			GL.Normal3(0.0, 1.0, 0.0);
			GL.TexCoord2(info.Top.Right-wet/16.0, info.Top.Top);    GL.Vertex3(x+1.0, y+1.0-1/16.0, z);
			GL.TexCoord2(info.Top.Left-wet/16.0,  info.Top.Top);    GL.Vertex3(x,     y+1.0-1/16.0, z);
			GL.TexCoord2(info.Top.Left-wet/16.0,  info.Top.Bottom); GL.Vertex3(x,     y+1.0-1/16.0, z+1.0);
			GL.TexCoord2(info.Top.Right-wet/16.0, info.Top.Bottom); GL.Vertex3(x+1.0, y+1.0-1/16.0, z+1.0);
			if (!bottom.Opaque) {
				AdjacentLight(chunk, x, y - 1, z);
				GL.Normal3(0.0, -1.0, 0.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Top+1/256.0); GL.Vertex3(x,     y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Top+1/256.0); GL.Vertex3(x+1.0, y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Bottom);      GL.Vertex3(x+1.0, y, z+1.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Bottom);      GL.Vertex3(x,     y, z+1.0);
			}
			if (!front.Opaque) {
				AdjacentLight(chunk, x, y, z + 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Top);    GL.Vertex3(x+1.0, y+1.0-1/16.0, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Top);    GL.Vertex3(x,     y+1.0-1/16.0, z+1.0);
				GL.TexCoord2(info.Front.Left,  info.Front.Bottom); GL.Vertex3(x,     y,            z+1.0);
				GL.TexCoord2(info.Front.Right, info.Front.Bottom); GL.Vertex3(x+1.0, y,            z+1.0);
			}
			if (!back.Opaque) {
				AdjacentLight(chunk, x, y, z - 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(info.Back.Right, info.Back.Top+1/256.0); GL.Vertex3(x,     y+1.0-1/16.0, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Top+1/256.0); GL.Vertex3(x+1.0, y+1.0-1/16.0, z);
				GL.TexCoord2(info.Back.Left,  info.Back.Bottom);      GL.Vertex3(x+1.0, y,            z);
				GL.TexCoord2(info.Back.Right, info.Back.Bottom);      GL.Vertex3(x,     y,            z);
			}
		}
		static void RenderStairs(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			double x1 = 0, x2 = 0, z1 = 0, z2 = 0;
			byte data = chunk.GetData(x, y, z);
			switch (data) {
				case 0:
					x1 = 0.5; break;
				case 1:
					x2 = 0.5; break;
				case 2:
					z1 = 0.5; break;
				case 3:
					z2 = 0.5; break;
			}
			if (!right.Opaque && right.DrawMode != DrawMode.Stairs) {
				
				AdjacentLight(chunk, x + 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				if (data == 0) {
					GL.TexCoord2(info.Left.Right, info.Left.Top);    GL.Vertex3(x+1.0, y+1.0, z);
					GL.TexCoord2(info.Left.Left,  info.Left.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
					GL.TexCoord2(info.Left.Left,  info.Left.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
					GL.TexCoord2(info.Left.Right, info.Left.Bottom); GL.Vertex3(x+1.0, y,     z);
				} else {
					GL.TexCoord2(info.Left.Right, info.Left.Top+1/32.0); GL.Vertex3(x+1.0, y+0.5, z);
					GL.TexCoord2(info.Left.Left,  info.Left.Top+1/32.0); GL.Vertex3(x+1.0, y+0.5, z+1.0);
					GL.TexCoord2(info.Left.Left,  info.Left.Bottom);     GL.Vertex3(x+1.0, y,     z+1.0);
					GL.TexCoord2(info.Left.Right, info.Left.Bottom);     GL.Vertex3(x+1.0, y,     z);
					if (data != 1) {
						GL.TexCoord2(info.Left.Right-z1/16, info.Left.Top);           GL.Vertex3(x+1.0, y+1.0, z+z1);
						GL.TexCoord2(info.Left.Left+z2/16,  info.Left.Top);           GL.Vertex3(x+1.0, y+1.0, z+1.0-z2);
						GL.TexCoord2(info.Left.Left+z2/16,  info.Left.Bottom-1/32.0); GL.Vertex3(x+1.0, y+0.5, z+1.0-z2);
						GL.TexCoord2(info.Left.Right-z1/16, info.Left.Bottom-1/32.0); GL.Vertex3(x+1.0, y+0.5, z+z1);
					}
				}
			}
			if (!left.Opaque && left.DrawMode != DrawMode.Stairs) {
				AdjacentLight(chunk, x - 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				if (data == 1) {
					GL.TexCoord2(info.Right.Right, info.Right.Top);    GL.Vertex3(x, y+1.0, z+1.0);
					GL.TexCoord2(info.Right.Left,  info.Right.Top);    GL.Vertex3(x, y+1.0, z);
					GL.TexCoord2(info.Right.Left,  info.Right.Bottom); GL.Vertex3(x, y,     z);
					GL.TexCoord2(info.Right.Right, info.Right.Bottom); GL.Vertex3(x, y,     z+1.0);
				} else {
					GL.TexCoord2(info.Right.Right, info.Right.Top+1/32.0); GL.Vertex3(x, y+0.5, z+1.0);
					GL.TexCoord2(info.Right.Left,  info.Right.Top+1/32.0); GL.Vertex3(x, y+0.5, z);
					GL.TexCoord2(info.Right.Left,  info.Right.Bottom);     GL.Vertex3(x, y,     z);
					GL.TexCoord2(info.Right.Right, info.Right.Bottom);     GL.Vertex3(x, y,     z+1.0);
					if (data != 0) {
						GL.TexCoord2(info.Left.Right-z2/16, info.Left.Top);           GL.Vertex3(x, y+1.0, z+1.0-z2);
						GL.TexCoord2(info.Left.Left+z1/16,  info.Left.Top);           GL.Vertex3(x, y+1.0, z+z1);
						GL.TexCoord2(info.Left.Left+z1/16,  info.Left.Bottom-1/32.0); GL.Vertex3(x, y+0.5, z+z1);
						GL.TexCoord2(info.Left.Right-z2/16, info.Left.Bottom-1/32.0); GL.Vertex3(x, y+0.5, z+1.0-z2);
					}
				}
			}
			GL.Normal3(0.0, 1.0, 0.0);
			if (!top.Opaque) {
				AdjacentLight(chunk, x, y + 1, z);
				GL.TexCoord2(info.Top.Right-x1/16, info.Top.Top+z1/16);    GL.Vertex3(x+1.0-x2, y+1.0, z+z1);
				GL.TexCoord2(info.Top.Left+x2/16,  info.Top.Top+z1/16);    GL.Vertex3(x+x1,     y+1.0, z+z1);
				GL.TexCoord2(info.Top.Left+x2/16,  info.Top.Bottom-z2/16); GL.Vertex3(x+x1,     y+1.0, z+1.0-z2);
				GL.TexCoord2(info.Top.Right-x1/16, info.Top.Bottom-z2/16); GL.Vertex3(x+1.0-x2, y+1.0, z+1.0-z2);
			}
			Light(chunk, x, y, z);
			GL.TexCoord2(info.Top.Right-x2/16, info.Top.Top+z2/16);    GL.Vertex3(x+1.0-x1, y+0.5, z+z2);
			GL.TexCoord2(info.Top.Left+x1/16,  info.Top.Top+z2/16);    GL.Vertex3(x+x2,     y+0.5, z+z2);
			GL.TexCoord2(info.Top.Left+x1/16,  info.Top.Bottom-z1/16); GL.Vertex3(x+x2,     y+0.5, z+1.0-z1);
			GL.TexCoord2(info.Top.Right-x2/16, info.Top.Bottom-z1/16); GL.Vertex3(x+1.0-x1, y+0.5, z+1.0-z1);
			switch (data) {
				case 0:
					GL.Normal3(1.0, 0.0, 0.0);
					GL.TexCoord2(info.Right.Right, info.Right.Top);           GL.Vertex3(x+0.5, y+1.0, z+1.0);
					GL.TexCoord2(info.Right.Left,  info.Right.Top);           GL.Vertex3(x+0.5, y+1.0, z);
					GL.TexCoord2(info.Right.Left,  info.Right.Bottom-1/32.0); GL.Vertex3(x+0.5, y+0.5, z);
					GL.TexCoord2(info.Right.Right, info.Right.Bottom-1/32.0); GL.Vertex3(x+0.5, y+0.5, z+1.0);
					break;
				case 1:
					GL.Normal3(1.0, 0.0, 0.0);
					GL.TexCoord2(info.Left.Right, info.Left.Top);           GL.Vertex3(x+0.5, y+1.0, z);
					GL.TexCoord2(info.Left.Left,  info.Left.Top);           GL.Vertex3(x+0.5, y+1.0, z+1.0);
					GL.TexCoord2(info.Left.Left,  info.Left.Bottom-1/32.0); GL.Vertex3(x+0.5, y+0.5, z+1.0);
					GL.TexCoord2(info.Left.Right, info.Left.Bottom-1/32.0); GL.Vertex3(x+0.5, y+0.5, z);
					break;
				case 2:
					GL.Normal3(0.0, 0.0, 1.0);
					GL.TexCoord2(info.Back.Right, info.Back.Top);           GL.Vertex3(x,     y+1.0, z+0.5);
					GL.TexCoord2(info.Back.Left,  info.Back.Top);           GL.Vertex3(x+1.0, y+1.0, z+0.5);
					GL.TexCoord2(info.Back.Left,  info.Back.Bottom-1/32.0); GL.Vertex3(x+1.0, y+0.5, z+0.5);
					GL.TexCoord2(info.Back.Right, info.Back.Bottom-1/32.0); GL.Vertex3(x,     y+0.5, z+0.5);
					break;
				case 3:
					GL.Normal3(0.0, 0.0, 1.0);
					GL.TexCoord2(info.Front.Right, info.Front.Top);           GL.Vertex3(x+1.0, y+1.0, z+0.5);
					GL.TexCoord2(info.Front.Left,  info.Front.Top);           GL.Vertex3(x,     y+1.0, z+0.5);
					GL.TexCoord2(info.Front.Left,  info.Front.Bottom-1/32.0); GL.Vertex3(x,     y+0.5, z+0.5);
					GL.TexCoord2(info.Front.Right, info.Front.Bottom-1/32.0); GL.Vertex3(x+1.0, y+0.5, z+0.5);
					break;
			}
			if (!bottom.Opaque) {
				AdjacentLight(chunk, x, y - 1, z);
				GL.Normal3(0.0, -1.0, 0.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Top);    GL.Vertex3(x,     y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Top);    GL.Vertex3(x+1.0, y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Bottom); GL.Vertex3(x+1.0, y, z+1.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Bottom); GL.Vertex3(x,     y, z+1.0);
			}
			if (!front.Opaque && front.DrawMode != DrawMode.Stairs) {
				AdjacentLight(chunk, x, y, z + 1);
				GL.Normal3(0.0, 0.0, 1.0);
				if (data == 2) {
					GL.TexCoord2(info.Front.Right, info.Front.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
					GL.TexCoord2(info.Front.Left,  info.Front.Top);    GL.Vertex3(x,     y+1.0, z+1.0);
					GL.TexCoord2(info.Front.Left,  info.Front.Bottom); GL.Vertex3(x,     y,     z+1.0);
					GL.TexCoord2(info.Front.Right, info.Front.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
				} else {
					GL.TexCoord2(info.Front.Right, info.Front.Top+1/32.0); GL.Vertex3(x+1.0, y+0.5, z+1.0);
					GL.TexCoord2(info.Front.Left,  info.Front.Top+1/32.0); GL.Vertex3(x,     y+0.5, z+1.0);
					GL.TexCoord2(info.Front.Left,  info.Front.Bottom);     GL.Vertex3(x,     y,     z+1.0);
					GL.TexCoord2(info.Front.Right, info.Front.Bottom);     GL.Vertex3(x+1.0, y,     z+1.0);
					if (data != 3) {
						GL.TexCoord2(info.Front.Right-x2/16, info.Front.Top);           GL.Vertex3(x+1.0-x2, y+1.0, z+1.0);
						GL.TexCoord2(info.Front.Left+x1/16,  info.Front.Top);           GL.Vertex3(x+x1,     y+1.0, z+1.0);
						GL.TexCoord2(info.Front.Left+x1/16,  info.Front.Bottom-1/32.0); GL.Vertex3(x+x1,     y+0.5, z+1.0);
						GL.TexCoord2(info.Front.Right-x2/16, info.Front.Bottom-1/32.0); GL.Vertex3(x+1.0-x2, y+0.5, z+1.0);
					}
				}
			}
			if (!back.Opaque && back.DrawMode != DrawMode.Stairs) {
				AdjacentLight(chunk, x, y, z - 1);
				GL.Normal3(0.0, 0.0, 1.0);
				if (data == 3) {
					GL.TexCoord2(info.Back.Right, info.Back.Top);    GL.Vertex3(x,     y+1.0, z);
					GL.TexCoord2(info.Back.Left,  info.Back.Top);    GL.Vertex3(x+1.0, y+1.0, z);
					GL.TexCoord2(info.Back.Left,  info.Back.Bottom); GL.Vertex3(x+1.0, y,     z);
					GL.TexCoord2(info.Back.Right, info.Back.Bottom); GL.Vertex3(x,     y,     z);
				} else {
					GL.TexCoord2(info.Back.Right, info.Back.Top+1/32.0); GL.Vertex3(x,     y+0.5, z);
					GL.TexCoord2(info.Back.Left,  info.Back.Top+1/32.0); GL.Vertex3(x+1.0, y+0.5, z);
					GL.TexCoord2(info.Back.Left,  info.Back.Bottom);     GL.Vertex3(x+1.0, y,     z);
					GL.TexCoord2(info.Back.Right, info.Back.Bottom);     GL.Vertex3(x,     y,     z);
					if (data != 2) {
						GL.TexCoord2(info.Back.Right-x1/16, info.Back.Top);           GL.Vertex3(x+x1,     y+1.0, z);
						GL.TexCoord2(info.Back.Left+x2/16,  info.Back.Top);           GL.Vertex3(x+1.0-x2, y+1.0, z);
						GL.TexCoord2(info.Back.Left+x2/16,  info.Back.Bottom-1/32.0); GL.Vertex3(x+1.0-x2, y+0.5, z);
						GL.TexCoord2(info.Back.Right-x1/16, info.Back.Bottom-1/32.0); GL.Vertex3(x+x1,     y+0.5, z);
					}
				}
			}
		}
		static void RenderChest(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			// TODO: Finish double-chest rendering
			//if (right.Type != info.Type && left.Type != info.Type &&
			//    front.Type != info.Type && back.Type != info.Type) {
			RectangleF rect_right = info.Right;
			RectangleF rect_left  = info.Right;
			RectangleF rect_front = info.Right;
			RectangleF rect_back  = info.Right;
			if (right.Opaque) {
				if (left.Opaque) {
					if (front.Opaque) rect_back = info.Front;
					else if (back.Opaque) rect_front = info.Front;
				} else rect_left = info.Front;
			} else rect_right = info.Front;
			if (!right.Opaque) {
				AdjacentLight(chunk, x + 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(rect_right.Right, rect_right.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(rect_right.Left,  rect_right.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(rect_right.Left,  rect_right.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
				GL.TexCoord2(rect_right.Right, rect_right.Bottom); GL.Vertex3(x+1.0, y,     z);
			}
			if (!left.Opaque) {
				AdjacentLight(chunk, x - 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(rect_left.Right, rect_left.Top);    GL.Vertex3(x, y+1.0, z+1.0);
				GL.TexCoord2(rect_left.Left,  rect_left.Top);    GL.Vertex3(x, y+1.0, z);
				GL.TexCoord2(rect_left.Left,  rect_left.Bottom); GL.Vertex3(x, y,     z);
				GL.TexCoord2(rect_left.Right, rect_left.Bottom); GL.Vertex3(x, y,     z+1.0);
			}
			if (!top.Opaque) {
				AdjacentLight(chunk, x, y + 1, z);
				GL.Normal3(0.0, 1.0, 0.0);
				GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x+1.0, y+1.0, z+1.0);
			}
			if (!bottom.Opaque) {
				AdjacentLight(chunk, x, y - 1, z);
				GL.Normal3(0.0, -1.0, 0.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Top);    GL.Vertex3(x,     y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Top);    GL.Vertex3(x+1.0, y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Bottom); GL.Vertex3(x+1.0, y, z+1.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Bottom); GL.Vertex3(x,     y, z+1.0);
			}
			if (!front.Opaque) {
				AdjacentLight(chunk, x, y, z + 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(rect_front.Right, rect_front.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(rect_front.Left,  rect_front.Top);    GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(rect_front.Left,  rect_front.Bottom); GL.Vertex3(x,     y,     z+1.0);
				GL.TexCoord2(rect_front.Right, rect_front.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
			}
			if (!back.Opaque) {
				AdjacentLight(chunk, x, y, z - 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(rect_back.Right, rect_back.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(rect_back.Left,  rect_back.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(rect_back.Left,  rect_back.Bottom); GL.Vertex3(x+1.0, y,     z);
				GL.TexCoord2(rect_back.Right, rect_back.Bottom); GL.Vertex3(x,     y,     z);
			}
			//}
		}
		static void RenderFurnace(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			RectangleF rect_right = info.Right;
			RectangleF rect_left  = info.Right;
			RectangleF rect_front = info.Right;
			RectangleF rect_back  = info.Right;
			byte data = chunk.GetData(x, y, z);
			switch (data) {
				case 2:
					rect_back = info.Front; break;
				case 3:
					rect_front = info.Front; break;
				case 4:
					rect_left = info.Front; break;
				case 5:
					rect_right = info.Front; break;
			}
			if (!right.Opaque) {
				AdjacentLight(chunk, x + 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(rect_right.Right, rect_right.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(rect_right.Left,  rect_right.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(rect_right.Left,  rect_right.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
				GL.TexCoord2(rect_right.Right, rect_right.Bottom); GL.Vertex3(x+1.0, y,     z);
			}
			if (!left.Opaque) {
				AdjacentLight(chunk, x - 1, y, z);
				GL.Normal3(1.0, 0.0, 0.0);
				GL.TexCoord2(rect_left.Right, rect_left.Top);    GL.Vertex3(x, y+1.0, z+1.0);
				GL.TexCoord2(rect_left.Left,  rect_left.Top);    GL.Vertex3(x, y+1.0, z);
				GL.TexCoord2(rect_left.Left,  rect_left.Bottom); GL.Vertex3(x, y,     z);
				GL.TexCoord2(rect_left.Right, rect_left.Bottom); GL.Vertex3(x, y,     z+1.0);
			}
			if (!top.Opaque) {
				AdjacentLight(chunk, x, y + 1, z);
				GL.Normal3(0.0, 1.0, 0.0);
				GL.TexCoord2(info.Top.Right, info.Top.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(info.Top.Left,  info.Top.Bottom); GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(info.Top.Right, info.Top.Bottom); GL.Vertex3(x+1.0, y+1.0, z+1.0);
			}
			if (!bottom.Opaque) {
				AdjacentLight(chunk, x, y - 1, z);
				GL.Normal3(0.0, -1.0, 0.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Top);    GL.Vertex3(x,     y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Top);    GL.Vertex3(x+1.0, y, z);
				GL.TexCoord2(info.Bottom.Right, info.Bottom.Bottom); GL.Vertex3(x+1.0, y, z+1.0);
				GL.TexCoord2(info.Bottom.Left,  info.Bottom.Bottom); GL.Vertex3(x,     y, z+1.0);
			}
			if (!front.Opaque) {
				AdjacentLight(chunk, x, y, z + 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(rect_front.Right, rect_front.Top);    GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.TexCoord2(rect_front.Left,  rect_front.Top);    GL.Vertex3(x,     y+1.0, z+1.0);
				GL.TexCoord2(rect_front.Left,  rect_front.Bottom); GL.Vertex3(x,     y,     z+1.0);
				GL.TexCoord2(rect_front.Right, rect_front.Bottom); GL.Vertex3(x+1.0, y,     z+1.0);
			}
			if (!back.Opaque) {
				AdjacentLight(chunk, x, y, z - 1);
				GL.Normal3(0.0, 0.0, 1.0);
				GL.TexCoord2(rect_back.Right, rect_back.Top);    GL.Vertex3(x,     y+1.0, z);
				GL.TexCoord2(rect_back.Left,  rect_back.Top);    GL.Vertex3(x+1.0, y+1.0, z);
				GL.TexCoord2(rect_back.Left,  rect_back.Bottom); GL.Vertex3(x+1.0, y,     z);
				GL.TexCoord2(rect_back.Right, rect_back.Bottom); GL.Vertex3(x,     y,     z);
			}
		}
		
		static void RenderPickingBlock(Chunk chunk, BlocktypeInfo info, int x, int y, int z)
		{
			BlocktypeInfo right, left, top, bottom, front, back;
			GetAdjacentBlocks(chunk, x, y, z, out right, out left,
			                  out top, out bottom, out front, out back);
			if (right.Type == Blocktype.Air) {
				GL.PushName((int)Side.Right);
				GL.Begin(BeginMode.Quads);
				GL.Vertex3(x+1.0, y+1.0, z);
				GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.Vertex3(x+1.0, y,     z+1.0);
				GL.Vertex3(x+1.0, y,     z);
				GL.End();
				GL.PopName();
			}
			if (left.Type == Blocktype.Air) {
				GL.PushName((int)Side.Left);
				GL.Begin(BeginMode.Quads);
				GL.Vertex3(x, y+1.0, z+1.0);
				GL.Vertex3(x, y+1.0, z);
				GL.Vertex3(x, y,     z);
				GL.Vertex3(x, y,     z+1.0);
				GL.End();
				GL.PopName();
			}
			if (top.Type == Blocktype.Air) {
				GL.PushName((int)Side.Top);
				GL.Begin(BeginMode.Quads);
				GL.Vertex3(x+1.0, y+1.0, z);
				GL.Vertex3(x,     y+1.0, z);
				GL.Vertex3(x,     y+1.0, z+1.0);
				GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.End();
				GL.PopName();
			}
			if (bottom.Type == Blocktype.Air) {
				GL.PushName((int)Side.Bottom);
				GL.Begin(BeginMode.Quads);
				GL.Vertex3(x,     y, z);
				GL.Vertex3(x+1.0, y, z);
				GL.Vertex3(x+1.0, y, z+1.0);
				GL.Vertex3(x,     y, z+1.0);
				GL.End();
				GL.PopName();
			}
			if (front.Type == Blocktype.Air) {
				GL.PushName((int)Side.Front);
				GL.Begin(BeginMode.Quads);
				GL.Vertex3(x+1.0, y+1.0, z+1.0);
				GL.Vertex3(x,     y+1.0, z+1.0);
				GL.Vertex3(x,     y,     z+1.0);
				GL.Vertex3(x+1.0, y,     z+1.0);
				GL.End();
				GL.PopName();
			}
			if (back.Type == Blocktype.Air) {
				GL.PushName((int)Side.Back);
				GL.Begin(BeginMode.Quads);
				GL.Vertex3(x,     y+1.0, z);
				GL.Vertex3(x+1.0, y+1.0, z);
				GL.Vertex3(x+1.0, y,     z);
				GL.Vertex3(x,     y,     z);
				GL.End();
				GL.PopName();
			}
		}
	}
}
