using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MinecraftEditor.Graphics;

namespace MinecraftEditor
{
	public static class Ressources
	{
		public static Texture TerrainTexture { get; private set; }
		public static Texture ItemsTexture { get; private set; }
		
		public static bool Load()
		{
			List<string> failed = new List<string>();
			LoadTextures(failed);
			if (failed.Count == 0) return true;
			string message = "Couldn't load "+string.Join(", ", failed.ToArray())+".\nContinue anyway?";
			DialogResult result = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo,
			                                      MessageBoxIcon.Warning);
			return (result == DialogResult.Yes);
		}
		
		static void LoadTextures(List<string> failed)
		{
			try { TerrainTexture = new Texture("terrain.png"); }
			catch { TerrainTexture = new Texture(); failed.Add("terrain.png"); }
			try { ItemsTexture = new Texture("items.png"); }
			catch { ItemsTexture = new Texture(); failed.Add("items.png"); }
			Display.Texture = null;
		}
	}
}
