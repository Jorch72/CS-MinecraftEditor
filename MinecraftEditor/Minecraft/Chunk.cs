using System;
using OpenTK.Graphics.OpenGL;
using NBT;
using MinecraftEditor.Graphics;
using MinecraftEditor.Minecraft;

namespace MinecraftEditor.Minecraft
{
	public class Chunk
	{
		public const int Width = 16;
		public const int Depth = 128;
		public const int Height = 16;
		
		DisplayList _displayList1, _displayList2;
		bool _cached1, _cached2;
		
		public short X { get; private set; }
		public short Z { get; private set; }
		public byte[] Blocks { get; set; }
		public byte[] Data { get; set; }
		public byte[] BlockLight { get; set; }
		public byte[] SkyLight { get; set; }
		public byte[] HeightMap { get; set; }
		
		public bool Cached {
			get { return _cached1 && _cached2; }
			set {
				_cached1 = value;
				_cached2 = value;
			}
		}
		public bool ListCached {
			get {
				return (_displayList1.Cached &&
				        _displayList2.Cached);
			}
		}
		
		public World World { get; internal set; }
		public Chunk Right { get; internal set; }
		public Chunk Left { get; internal set; }
		public Chunk Front { get; internal set; }
		public Chunk Back { get; internal set; }
		
		public Chunk(short x, short z)
		{
			X = x; Z = z;
			_displayList1 = new DisplayList();
			_displayList2 = new DisplayList();
		}
		
		public static Chunk Load(string path)
		{
			Tag tag = Tag.Load(path);
			return Chunk.Load(tag["Level"]);
		}
		public static Chunk Load(Tag tag)
		{
			short x = (short)(int)tag["xPos"];
			short z = (short)(int)tag["zPos"];
			Chunk chunk = new Chunk(x, z);
			chunk.Blocks = (byte[])tag["Blocks"];
			chunk.Data = (byte[])tag["Data"];
			chunk.BlockLight = (byte[])tag["BlockLight"];
			chunk.SkyLight = (byte[])tag["SkyLight"];
			chunk.HeightMap = (byte[])tag["HeightMap"];
			return chunk;
		}
		
		#region Manipulation
		static int GetIndex(int x, int y, int z)
		{
			x = (x % Width + Width) % Width;
			y = (y % Depth + Depth) % Depth;
			z = (z % Height + Height) % Height;
			return x * Depth * Height + y + z * Depth;
		}
		static int GetIndex(int x, int z)
		{
			x = (x % Width + Width) % Width;
			z = (z % Height + Height) % Height;
			return x + z * Width;
		}
		public Blocktype GetBlocktype(int x, int y, int z)
		{
			return (Blocktype)Blocks[GetIndex(x, y, z)];
		}
		public void SetBlocktype(int x, int y, int z, Blocktype type)
		{
			Blocks[GetIndex(x, y, z)] = (byte)type;
		}
		public byte GetData(int x, int y, int z)
		{
			int index = GetIndex(x, y, z);
			if (index % 2 == 0) return (byte)(Data[index/2] & 0xF);
			else return (byte)(Data[index/2] >> 4);
		}
		public void SetData(int x, int y, int z, byte data)
		{
			int index = GetIndex(x, y, z);
			if (index % 2 == 0) Data[index/2] = (byte)((Data[index/2] & 0xF) | (data & 0x0F));
			else Data[index/2] = (byte)((Data[index/2] & 0x0F) | (data << 4));
		}
		public byte GetBlockLight(int x, int y, int z)
		{
			int index = GetIndex(x, y, z);
			if (index % 2 == 0) return (byte)(BlockLight[index/2] & 0xF);
			else return (byte)(BlockLight[index/2] >> 4);
		}
		public void SetBlockLight(int x, int y, int z, byte blockLight)
		{
			int index = GetIndex(x, y, z);
			if (index % 2 == 0) BlockLight[index/2] = (byte)((BlockLight[index/2] & 0xF) | (blockLight & 0x0F));
			else BlockLight[index/2] = (byte)((BlockLight[index/2] & 0x0F) | (blockLight << 4));
		}
		public byte GetSkyLight(int x, int y, int z)
		{
			int index = GetIndex(x, y, z);
			if (index % 2 == 0) return (byte)(SkyLight[index/2] & 0xF);
			else return (byte)(SkyLight[index/2] >> 4);
		}
		public void SetSkyLight(int x, int y, int z, byte skyLight)
		{
			int index = GetIndex(x, y, z);
			if (index % 2 == 0) SkyLight[index/2] = (byte)((SkyLight[index/2] & 0xF) | (skyLight & 0x0F));
			else SkyLight[index/2] = (byte)((SkyLight[index/2] & 0x0F) | (skyLight << 4));
		}
		public byte GetHeightMap(int x, int z)
		{
			return HeightMap[GetIndex(x, z)];
		}
		public void SetHeightMap(int x, int z, byte heightMap)
		{
			HeightMap[GetIndex(x, z)] = heightMap;
		}
		#endregion
		
		#region Render
		public void Render(bool cache)
		{
			if (_cached1 || (_displayList1.Cached && !cache))
				_displayList1.Call();
			else if (cache) {
				_displayList1.Begin();
				GL.Begin(BeginMode.Quads);
				for (int x = 0; x < Width; ++x)
					for (int y = 0; y < Depth; ++y)
						for (int z = 0; z < Height; ++z) {
					Blocktype type = GetBlocktype(x, y, z);
					if (type == Blocktype.Air ||
					    type == Blocktype.Water ||
					    type == Blocktype.StillWater ||
					    type == Blocktype.Ice) continue;
					BlocktypeInfo info = BlocktypeInfo.Find(type);
					BlockRenderer.Render(this, info, x, y, z);
				}
				GL.End();
				_displayList1.End();
				_cached1 = true;
			}
		}
		
		public void RenderTransparent(bool cache)
		{
			if (_cached2 || (_displayList2.Cached && !cache))
				_displayList2.Call();
			else if (cache) {
				_displayList2.Begin();
				GL.Begin(BeginMode.Quads);
				for (int x = 0; x < Width; ++x)
					for (int y = 0; y < Depth; ++y)
						for (int z = 0; z < Height; ++z) {
					Blocktype type = GetBlocktype(x, y, z);
					if (type != Blocktype.Water &&
					    type != Blocktype.StillWater &&
					    type != Blocktype.Ice) continue;
					BlocktypeInfo info = BlocktypeInfo.Find(type);
					BlockRenderer.Render(this, info, x, y, z);
				}
				GL.End();
				_displayList2.End();
				_cached2 = true;
			}
		}
		#endregion
	}
}
