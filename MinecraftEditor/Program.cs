using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MinecraftEditor.Minecraft;

namespace MinecraftEditor
{
	static class Program
	{
		static void Main()
		{
			try {
				if (!LoadAssembly("NBT")) return;
				if (!LoadAssembly("OpenTK")) return;
				Console.Clear();
				Console.WriteLine("Platform: "+Platform.Current);
				string path;
				if (Platform.Current == Platform.Windows)
					path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.minecraft";
				else if (Platform.Current == Platform.Mac)
					path = Environment.GetEnvironmentVariable("HOME") + "/Library/Application Support/minecraft";
				else path = Environment.GetEnvironmentVariable("HOME") + "/.minecraft";
				DirectoryInfo saves = new DirectoryInfo(path + "/saves");
				List<DirectoryInfo> worlds = new List<DirectoryInfo>();
				if (saves.Exists) foreach (DirectoryInfo world in saves.GetDirectories())
					if (world.GetFiles("level.dat").Length == 1) worlds.Add(world);
				Console.CursorVisible = false;
				Console.WriteLine("Select a world you want to open:");
				Console.WriteLine();
				int cursor = Console.CursorTop;
				if (worlds.Count == 0) {
					cursor += 2;
					Console.WriteLine(" (Could not find any worlds.)");
				} else foreach (DirectoryInfo world in worlds)
					Console.WriteLine(" [ ] "+world.Name);
				Console.WriteLine();
				Console.WriteLine(" [ ] Exit");
				Console.CursorLeft = 2;
				Console.CursorTop = cursor;
				Console.Write("x");
				int option = 0;
				while (true) {
					int lastOption = option;
					ConsoleKeyInfo key = Console.ReadKey(true);
					switch (key.Key) {
						case ConsoleKey.Escape:
							if (option == worlds.Count) return;
							option = worlds.Count;
							break;
						case ConsoleKey.UpArrow:
							option = (option + worlds.Count) % (worlds.Count + 1);
							break;
						case ConsoleKey.DownArrow:
							option = (option + worlds.Count + 2) % (worlds.Count + 1);
							break;
						case ConsoleKey.Enter:
							if (option != worlds.Count) {
								Console.Clear();
								Run(worlds[option]);
							} return;
					}
					if (lastOption != option) {
						Console.CursorLeft--;
						Console.Write(" ");
						Console.CursorLeft--;
						Console.CursorTop = cursor + option;
						if (option == worlds.Count) Console.CursorTop++;
						Console.Write("x");
					}
				}
			} catch (Exception e) { Console.Write(e.ToString()); Console.ReadKey(); }
		}
		
		static void Run(DirectoryInfo dir)
		{
			Console.Write("Total chunks: ");
			int cursor = Console.CursorLeft;
			List<FileInfo> files = new List<FileInfo>();
			foreach (DirectoryInfo dir1 in dir.GetDirectories())
				foreach (DirectoryInfo dir2 in dir1.GetDirectories())
					foreach (FileInfo file in dir2.GetFiles("*.dat")) {
				files.Add(file);
				Console.CursorLeft = cursor;
				Console.Write(files.Count);
			}
			Console.WriteLine();
			Window window = new Window();
			Console.Write("Loading world: ");
			cursor = Console.CursorLeft;
			for (int i = 0; i < files.Count; i++) {
				FileInfo file = files[i];
				window.World.SetChunk(Chunk.Load(file.FullName));
				Console.CursorLeft = cursor;
				Console.Write((i * 100 / files.Count)+"%");
			}
			Console.Clear();
			try { window.Run(60.0); }
			catch (Exception) { window.Dispose(); throw; }
		}
		
		static bool LoadAssembly(string assembly)
		{
			string error = null;
			try { Assembly.Load(assembly); }
			catch (FileNotFoundException) { error = "Couldn't find "+assembly+".dll"; }
			catch (Exception) { error = "Error loading "+assembly+".dll"; }
			if (error == null) return true;
			Console.WriteLine(error);
			Console.WriteLine("Press any key to exit ...");
			Console.ReadKey();
			return false;
		}
	}
}
