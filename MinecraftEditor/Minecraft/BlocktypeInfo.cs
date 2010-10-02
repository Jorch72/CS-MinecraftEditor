using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using MinecraftEditor.Graphics;

namespace MinecraftEditor.Minecraft
{
	public class BlocktypeInfo
	{
		static BlocktypeInfo[] _blocktypes = new BlocktypeInfo[255];
		
		#region Initialization
		static BlocktypeInfo()
		{
			new BlocktypeInfo(Blocktype.Air, DrawMode.Invisible){ Opaque = false, Size = Vector3.Zero };
			new BlocktypeInfo(Blocktype.Rock){ Sides = "all = 1:0" };
			new BlocktypeInfo(Blocktype.Grass){ Sides = "top = 0:0, sides = 3:0, bottom = 2:0" };
			new BlocktypeInfo(Blocktype.Dirt){ Sides = "all = 2:0" };
			new BlocktypeInfo(Blocktype.Cobblestone){ Sides = "all = 0:1" };
			new BlocktypeInfo(Blocktype.Wood){ Sides = "all = 4:0" };
			new BlocktypeInfo(Blocktype.Sapling){ Sides = "all = 15:0", Opaque = false, DrawMode = DrawMode.Plant };
			new BlocktypeInfo(Blocktype.Adminium){ Sides = "all = 1:1" };
			new BlocktypeInfo(Blocktype.Water){ Sides = "all = 15:13", Opaque = false, DrawMode = DrawMode.Glass };
			new BlocktypeInfo(Blocktype.StillWater){ Sides = "all = 15:13", Opaque = false, DrawMode = DrawMode.Glass };
			new BlocktypeInfo(Blocktype.Lava){ Sides = "all = 15:15", Opaque = false, DrawMode = DrawMode.Glass };
			new BlocktypeInfo(Blocktype.StillLava){ Sides = "all = 15:15", Opaque = false, DrawMode = DrawMode.Glass };
			new BlocktypeInfo(Blocktype.Sand){ Sides = "all = 2:1" };
			new BlocktypeInfo(Blocktype.Gravel){ Sides = "all = 3:1" };
			new BlocktypeInfo(Blocktype.GoldOre){ Sides = "all = 0:2" };
			new BlocktypeInfo(Blocktype.IronOre){ Sides = "all = 1:2" };
			new BlocktypeInfo(Blocktype.CoalOre){ Sides = "all = 2:2" };
			new BlocktypeInfo(Blocktype.Tree){ Sides = "y = 5:1, sides = 4:1" };
			new BlocktypeInfo(Blocktype.Leaves){ Sides = "all = 4:3", Opaque = false };
			new BlocktypeInfo(Blocktype.Sponge){ Sides = "all = 0:3" };
			new BlocktypeInfo(Blocktype.Glass){ Sides = "all = 1:3", Opaque = false, DrawMode = DrawMode.Glass };
			new BlocktypeInfo(Blocktype.Cloth){ Sides = "all = 0:4" };
			new BlocktypeInfo(Blocktype.Flower){ Sides = "all = 13:0", Opaque = false, DrawMode = DrawMode.Plant };
			new BlocktypeInfo(Blocktype.Rose){ Sides = "all = 12:0", Opaque = false, DrawMode = DrawMode.Plant };
			new BlocktypeInfo(Blocktype.BrownMushroom){ Sides = "all = 13:1", Opaque = false, DrawMode = DrawMode.Plant };
			new BlocktypeInfo(Blocktype.RedMushroom){ Sides = "all = 12:1", Opaque = false, DrawMode = DrawMode.Plant };
			new BlocktypeInfo(Blocktype.GoldBlock){ Sides = "top = 7:1, sides = 7:2, bottom = 7:3" };
			new BlocktypeInfo(Blocktype.IronBlock){ Sides = "top = 6:1, sides = 6:2, bottom = 6:3" };
			new BlocktypeInfo(Blocktype.DoubleStoneSlab){ Sides = "y = 6:0, sides = 5:0" };
			new BlocktypeInfo(Blocktype.StoneSlab){ Sides = "y = 6:0, sides = 5:0", Opaque = false, DrawMode = DrawMode.StoneSlab };
			new BlocktypeInfo(Blocktype.Brick){ Sides = "all = 7:0" };
			new BlocktypeInfo(Blocktype.TNT){ Sides = "top = 9:0, sides = 8:0, bottom = 10:0" };
			new BlocktypeInfo(Blocktype.Bookshelf){ Sides = "y = 4:0, sides = 3:2" };
			new BlocktypeInfo(Blocktype.MossyCobblestone){ Sides = "all = 4:2" };
			new BlocktypeInfo(Blocktype.Obsidian){ Sides = "all = 5:2" };
			new BlocktypeInfo(Blocktype.Torch){ Sides = "all = 0:5", Opaque = false, DrawMode = DrawMode.Torch };
			new BlocktypeInfo(Blocktype.Fire){ Sides = "all = 15:1", Opaque = false, DrawMode = DrawMode.Fire };
			new BlocktypeInfo(Blocktype.MobSpawner){ Sides = "all = 1:4", Opaque = false };
			new BlocktypeInfo(Blocktype.WoodStairs){ Sides = "all = 4:0", Opaque = false, DrawMode = DrawMode.Stairs };
			new BlocktypeInfo(Blocktype.Chest){ Sides = "top = 9:1, sides = 10:1, front = 11:1, bottom = 9:1", DrawMode = DrawMode.Chest };
			new BlocktypeInfo(Blocktype.RedstoneDust){ Sides = "all = 4:5", Opaque = false, DrawMode = DrawMode.RedstoneDust };
			new BlocktypeInfo(Blocktype.DiamondOre){ Sides = "all = 2:3" };
			new BlocktypeInfo(Blocktype.DiamondBlock){ Sides = "top = 8:1, sides = 8:2, bottom = 8:3" };
			new BlocktypeInfo(Blocktype.Workbench){ Sides = "top = 11:2, x = 11:3, z = 12:3, bottom = 4:0" };
			new BlocktypeInfo(Blocktype.Crop){ Sides = "all = 8:5", Opaque = false, DrawMode = DrawMode.Crop };
			new BlocktypeInfo(Blocktype.Soil){ Sides = "all = 2:0, top = 7:5", Opaque = false, DrawMode = DrawMode.Soil };
			new BlocktypeInfo(Blocktype.Furnace){ Sides = "y = 1:0, sides = 13:2, front = 12:2", DrawMode = DrawMode.Furnace };
			new BlocktypeInfo(Blocktype.LitFurnace){ Sides = "y = 1:0, sides = 13:2, front = 13:3", DrawMode = DrawMode.Furnace };
			new BlocktypeInfo(Blocktype.Sign){ Opaque = false, DrawMode = DrawMode.Invisible };
			new BlocktypeInfo(Blocktype.WoodDoorBlock){ Sides = "top = 1:7, bottom = 1:8", Opaque = false, DrawMode = DrawMode.Door };
			new BlocktypeInfo(Blocktype.Ladder){ Sides = "all = 3:7", Opaque = false, DrawMode = DrawMode.Ladder };
			new BlocktypeInfo(Blocktype.Rails){ Sides = "top = 0:9, bottom = 0:10", Opaque = false, DrawMode = DrawMode.Rails };
			new BlocktypeInfo(Blocktype.CobblestoneStairs){ Sides = "all = 0:1", Opaque = false, DrawMode = DrawMode.Stairs };
			new BlocktypeInfo(Blocktype.Sign){ Sides = "all = 4:0", Opaque = false, DrawMode = DrawMode.WallSign };
			new BlocktypeInfo(Blocktype.Lever){ Sides = "all = 0:7, bottom = 0:1", Opaque = false, DrawMode = DrawMode.Lever };
			new BlocktypeInfo(Blocktype.RockPressurePlate){ Sides = "all = 1:0", Opaque = false, DrawMode = DrawMode.PressurePlate };
			new BlocktypeInfo(Blocktype.IronDoorBlock){ Sides = "top = 2:7, bottom = 2:8", Opaque = false, DrawMode = DrawMode.Door };
			new BlocktypeInfo(Blocktype.WoodPressurePlate){ Sides = "all = 4:0", Opaque = false, DrawMode = DrawMode.PressurePlate };
			new BlocktypeInfo(Blocktype.RedstoneOre){ Sides = "all = 3:3" };
			new BlocktypeInfo(Blocktype.LitRedstoneOre){ Sides = "all = 3:3" };
			new BlocktypeInfo(Blocktype.RedstoneTorch){ Sides = "all = 3:7", Opaque = false, DrawMode = DrawMode.Torch };
			new BlocktypeInfo(Blocktype.LitRedstoneTorch){ Sides = "all = 3:6", Opaque = false, DrawMode = DrawMode.Torch };
			new BlocktypeInfo(Blocktype.Button){ Sides = "all = 1:0", Opaque = false, DrawMode = DrawMode.Button };
			new BlocktypeInfo(Blocktype.Snow){ Sides = "all = 2:4", Opaque = false, DrawMode = DrawMode.Snow };
			new BlocktypeInfo(Blocktype.Ice){ Sides = "all = 3:4", Opaque = false };
			new BlocktypeInfo(Blocktype.SnowBlock){ Sides = "all = 2:4" };
			new BlocktypeInfo(Blocktype.Cactus){ Sides = "top = 5:4, sides = 6:4, bottom = 7:4", Opaque = false, DrawMode = DrawMode.Cactus };
			new BlocktypeInfo(Blocktype.ClayBlock){ Sides = "all = 8:4" };
			new BlocktypeInfo(Blocktype.ReedBlock){ Sides = "all = 9:4", Opaque = false, DrawMode = DrawMode.Plant };
			new BlocktypeInfo(Blocktype.Jukebox){ Sides = "all = 9:4, top = 10:4" };
		}
		#endregion
		
		#region Find methods
		public static BlocktypeInfo Find(Blocktype type)
		{
			return Find((byte)type);
		}
		public static BlocktypeInfo Find(byte type)
		{
			return _blocktypes[type];
		}
		public static BlocktypeInfo Find(string name)
		{
			foreach (BlocktypeInfo type in _blocktypes)
				if (type != null && type.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return type;
			return null;
		}
		#endregion
		
		public Blocktype Type { get; private set; }
		public DrawMode DrawMode { get; private set; }
		public Vector3 Size { get; set; }
		public bool Opaque { get; set; }
		public byte Radiate { get; set; }
		
		public RectangleF Right { get; private set; }
		public RectangleF Left { get; private set; }
		public RectangleF Top { get; private set; }
		public RectangleF Bottom { get; private set; }
		public RectangleF Front { get; private set; }
		public RectangleF Back { get; private set; }
		
		public string Name {
			get { return Type.ToString(); }
		}
		public string Sides {
			set { SetSides(value); }
		}
		
		public BlocktypeInfo(Blocktype type)
			: this (type, DrawMode.Block) {  }
		public BlocktypeInfo(Blocktype type, DrawMode drawMode)
		{
			Type = type;
			DrawMode = drawMode;
			Opaque = true;
			Size = Vector3.One;
			_blocktypes[(byte)type] = this;
		}
		
		#region Sides
		void SetSides(string str)
		{
			string[] list = str.Split(',');
			foreach (string kvp in list) {
				string[] split = kvp.Split('=');
				string key = split[0].Trim();
				split = split[1].Split(':');
				byte x = byte.Parse(split[0].Trim());
				byte y = byte.Parse(split[1].Trim());
				RectangleF rect = new RectangleF(x / 16.0f + 0.00004f, y / 16.0f + 0.00004f,
				                                 1 / 16.0f - 0.00008f, 1 / 16.0f - 0.00008f);
				switch (key) {
					case "all":
						Right = rect; Left = rect;
						Top = rect; Bottom = rect;
						Front = rect; Back = rect;
						break;
					case "sides":
						Right = rect; Left = rect;
						Front = rect; Back = rect;
						break;
					case "x":
						Right = rect; Left = rect;
						break;
					case "y":
						Top = rect; Bottom = rect;
						break;
					case "z":
						Front = rect; Back = rect;
						break;
					case "right":
						Right = rect;
						break;
					case "left":
						Left = rect;
						break;
					case "top":
						Top = rect;
						break;
					case "bottom":
						Bottom = rect;
						break;
					case "front":
						Front = rect;
						break;
					case "back":
						Back = rect;
						break;
				}
			}
		}
		#endregion
	}
}
