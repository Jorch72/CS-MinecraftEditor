using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using MinecraftEditor.Graphics;
using MinecraftEditor.Minecraft;

namespace MinecraftEditor
{
	public class Window : GameWindow
	{
		public bool Fog { get; set; }
		public bool Grid { get; set; }
		public Font Font { get; set; }
		
		public Camera Camera { get; private set; }
		public World World { get; private set; }
		public Target Target { get; private set; }
		
		public Window() : base(1152, 864, new GraphicsMode(32, 24, 0, 0), "Minecraft Editor")
		{
			WindowBorder = WindowBorder.Fixed;
			
			Fog = true;
			Grid = true;
			
			World = new World();
			Camera = new Camera(){
				Location = new Vector3d(0.0, 48.0 - World.RenderRange, 0.0),
				Rotation = new Vector3(0, 90, 0) };
			
			Keyboard.KeyDown += KeyDown;
			Mouse.ButtonDown += MouseButtonDown;
		}
		
		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(Color4.Black);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			
			GL.Enable(EnableCap.ColorMaterial);
			GL.Enable(EnableCap.Light0);
			GL.Light(LightName.Light0, LightParameter.Ambient,
			         new Color4(0.3f, 0.3f, 0.3f, 1.0f));
			GL.Light(LightName.Light0, LightParameter.Diffuse,
			         new Color4(0.8f, 0.8f, 0.8f, 1.0f));
			GL.ColorMaterial(MaterialFace.Front,
			                 ColorMaterialParameter.AmbientAndDiffuse);
			
			GL.AlphaFunc(AlphaFunction.Greater, 0.1f);
			
			GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
			GL.Fog(FogParameter.FogColor, new float[]{ 0.0f, 0.0f, 0.0f, 1.0f });
			GL.Fog(FogParameter.FogStart, (World.RenderRange - 48.0f) * 0.75f);
			GL.Fog(FogParameter.FogEnd, World.RenderRange - 48.0f);
			
			if (!Ressources.Load()) Exit();
			Font = new Font(Ressources.FontTexture);
		}
		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(ClientSize);
		}
		
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			Camera.Update(this, e.Time);
		}
		
		void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Target == null) return;
			switch (e.Button)
			{
				case MouseButton.Left:
					World.SetBlocktype(Target.X, Target.Y, Target.Z, Blocktype.Air);
					break;
				case MouseButton.Right:
					int x, y, z;
					Target.GetSide(out x, out y, out z);
					World.SetBlocktype(x, y, z, Blocktype.Rock);
					break;
			}
		}
		void KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					Exit(); break;
				case Key.F:
					Fog = !Fog; break;
				case Key.G:
					Grid = !Grid; break;
				case Key.F5:
					if(World.Light > 0) --World.Light; break;
				case Key.F6:
					if (World.Light < 15) ++World.Light; break;
				case Key.F7:
					if (World.MinimumLight > 0) --World.MinimumLight; break;
				case Key.F8:
					if (World.MinimumLight < 15) ++World.MinimumLight; break;
				case Key.F9:
					if (World.RenderRange > 64) {
						World.RenderRange -= 32;
						GL.Fog(FogParameter.FogStart, (World.RenderRange - 48.0f) * 0.75f);
						GL.Fog(FogParameter.FogEnd, World.RenderRange - 48.0f);
					} break;
				case Key.F10:
					if (World.RenderRange < 640) {
						World.RenderRange += 32;
						GL.Fog(FogParameter.FogStart, (World.RenderRange - 48.0f) * 0.75f);
						GL.Fog(FogParameter.FogEnd, World.RenderRange - 48.0f);
					} break;
			}
		}
		
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			FPS.RenderFrame();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
				(float)Math.PI/4, (float)Width/Height, 0.2f, 1024.0f);
			GL.LoadMatrix(ref projection);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.AlphaTest);
			if (Fog) GL.Enable(EnableCap.Fog);
			
			Camera.Render();
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0.5f, 1.0f, 0.7f, 0.0f));
			
			GL.Color4(Color4.Gray);
			World.RenderOrigin = new Vector2d(Camera.X, Camera.Z);
			World.Render();
			
			GL.Disable(EnableCap.Lighting);
			
			Display.Texture = null;
			Display.BlendMode = BlendMode.Add;
			GL.LineWidth(2.0f);
			if (Grid) {
				GL.Color4(new Color4(1.0f, 1.0f, 1.0f, 0.3f));
				GL.Begin(BeginMode.Lines);
				for (int x = (int)Math.Floor((-Camera.X + Chunk.Width / 2 - World.RenderRange) / 16.0);
				     x < (int)Math.Ceiling((-Camera.X + Chunk.Width / 2  + World.RenderRange) / 16.0); x++) {
					GL.Vertex3(x * 16, Chunk.Depth, Math.Floor((-Camera.Z + Chunk.Height / 2 - World.RenderRange) / 16.0) * 16.0);
					GL.Vertex3(x * 16, Chunk.Depth, Math.Floor((-Camera.Z + Chunk.Height / 2 + World.RenderRange) / 16.0) * 16.0);
				}
				for (int z = (int)Math.Floor((-Camera.Z + Chunk.Height / 2 - World.RenderRange) / 16.0);
				     z < (int)Math.Ceiling((-Camera.Z + Chunk.Height / 2  + World.RenderRange) / 16.0); z++) {
					GL.Vertex3(Math.Floor((-Camera.X + Chunk.Height / 2 - World.RenderRange) / 16.0) * 16.0, Chunk.Depth, z * 16);
					GL.Vertex3(Math.Floor((-Camera.X + Chunk.Height / 2 + World.RenderRange) / 16.0) * 16.0, Chunk.Depth, z * 16);
				}
				GL.End();
			}
			
			Select();
			Display.BlendMode = BlendMode.None;
			
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, Width, Height, 0, -1, 1);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.AlphaTest);
			if (Fog) GL.Disable(EnableCap.Fog);
			
			if (Camera.MouseEnabled) {
				GL.LineWidth(1.0f);
				GL.Color4(Color4.White);
				Draw.FilledRectangle(Width / 2 - 5, Height / 2 - 1, 11, 3);
				Draw.FilledRectangle(Width / 2 - 1, Height / 2 - 5, 3, 11);
				GL.Color4(Color4.Black);
				Draw.Line(Width / 2 - 4, Height / 2 + 1, Width / 2 + 5, Height / 2 + 1);
				Draw.Line(Width / 2, Height / 2 - 4, Width / 2, Height / 2 + 5);
			}
			
			Display.BlendMode = BlendMode.Blend;
			GL.Color4(Color4.White);
			GL.PushMatrix();
			GL.Scale(2, 2, 2);
			Font.Write(4, 4, "Controls:");
			GL.PopMatrix();
			Font.Write(16, 40,
			           "F5/F6 - Sky light: " + World.Light + "\n" +
			           "F7/F8 - Min. light: " + World.MinimumLight + "\n" +
			           "F8/F9 - Render range: " + World.RenderRange + "\n" +
			           "    F - Fog: " + (Fog ? "On" : "Off") + "\n" +
			           "    G - Grid: " + (Grid ? "On" : "Off") + "\n" +
			           "  Esc - Exit");
			Font.Write(16, Height - 16,
			           "FPS: " + FPS.Count + "\n" +
			           "Chunks: " + World.ChunksVisible + " / " + World.ChunksInRange + "\n" +
			           "Total chunks: " + World.ChunksLoaded,
			           new TextAlign(HorAlign.Left, VerAlign.Bottom));
			Display.BlendMode = BlendMode.None;
			
			SwapBuffers();
		}
		
		void Select()
		{
			int[] buffer = new int[128];
			int[] view = new int[4];
			GL.SelectBuffer(buffer.Length, buffer);
			GL.GetInteger(GetPName.Viewport, view);
			GL.RenderMode(RenderingMode.Select);
			GL.InitNames();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.LoadIdentity();
			PickMatrix(Mouse.X, Height - Mouse.Y, 1, 1, view);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
				(float)Math.PI/4, (float)view[2]/view[3], 0.2f, World.RenderRange);
			GL.MultMatrix(ref projection);
			GL.MatrixMode(MatrixMode.Modelview);
			World.RenderPicking(-Camera.Location, 16);
			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			int hits = GL.RenderMode(RenderingMode.Render);
			int pos = 0, result = -1;
			for (int i = 0; i < hits; i++) {
				int number = buffer[pos];
				int minZ = buffer[pos + 1];
				int maxZ = buffer[pos + 2];
				if (result == -1 || minZ < buffer[result + 1]) result = pos;
				pos += number + 3;
			}
			GL.MatrixMode(MatrixMode.Modelview);
			if (result != -1) {
				int x = buffer[result+3];
				int y = buffer[result+4];
				int z = buffer[result+5];
				Side side = (Side)buffer[result+6];
				Target = new Target(x, y, z, side);
				Display.Texture = null;
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				GL.Color4(new Color4(1.0f, 1.0f, 1.0f, 0.4f));
				GL.Begin(BeginMode.Quads);
				GL.Vertex3(x+1.01, y+1.01, z-0.01);
				GL.Vertex3(x+1.01, y+1.01, z+1.01);
				GL.Vertex3(x+1.01, y-0.01, z+1.01);
				GL.Vertex3(x+1.01, y-0.01, z-0.01);
				GL.Vertex3(x-0.01, y+1.01, z+1.01);
				GL.Vertex3(x-0.01, y+1.01, z-0.01);
				GL.Vertex3(x-0.01, y-0.01, z-0.01);
				GL.Vertex3(x-0.01, y-0.01, z+1.01);
				GL.Vertex3(x+1.01, y+1.01, z-0.01);
				GL.Vertex3(x-0.01, y+1.01, z-0.01);
				GL.Vertex3(x-0.01, y+1.01, z+1.01);
				GL.Vertex3(x+1.01, y+1.01, z+1.01);
				GL.Vertex3(x-0.01, y-0.01, z-0.01);
				GL.Vertex3(x+1.01, y-0.01, z-0.01);
				GL.Vertex3(x+1.01, y-0.01, z+1.01);
				GL.Vertex3(x-0.01, y-0.01, z+1.01);
				GL.Vertex3(x+1.01, y+1.01, z+1.01);
				GL.Vertex3(x-0.01, y+1.01, z+1.01);
				GL.Vertex3(x-0.01, y-0.01, z+1.01);
				GL.Vertex3(x+1.01, y-0.01, z+1.01);
				GL.Vertex3(x-0.01, y+1.01, z-0.01);
				GL.Vertex3(x+1.01, y+1.01, z-0.01);
				GL.Vertex3(x+1.01, y-0.01, z-0.01);
				GL.Vertex3(x-0.01, y-0.01, z-0.01);
				GL.End();
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
				GL.Color4(new Color4(1.0f, 1.0f, 1.0f, 0.2f));
				GL.Begin(BeginMode.Quads);
				switch (side) {
					case Side.Right:
						GL.Vertex3(x+1.001, y+1.0-1/16.0, z+1/16.0);
						GL.Vertex3(x+1.001, y+1.0-1/16.0, z+1.0-1/16.0);
						GL.Vertex3(x+1.001, y+1/16.0,     z+1.0-1/16.0);
						GL.Vertex3(x+1.001, y+1/16.0,     z+1/16.0);
						break;
					case Side.Left:
						GL.Vertex3(x-0.001, y+1.0-1/16.0, z+1.0-1/16.0);
						GL.Vertex3(x-0.001, y+1.0-1/16.0, z+1/16.0);
						GL.Vertex3(x-0.001, y+1/16.0,     z+1/16.0);
						GL.Vertex3(x-0.001, y+1/16.0,     z+1.0-1/16.0);
						break;
					case Side.Top:
						GL.Vertex3(x+1.0-1/16.0, y+1.001, z+1/16.0);
						GL.Vertex3(x+1/16.0,     y+1.001, z+1/16.0);
						GL.Vertex3(x+1/16.0,     y+1.001, z+1.0-1/16.0);
						GL.Vertex3(x+1.0-1/16.0, y+1.001, z+1.0-1/16.0);
						break;
					case Side.Bottom:
						GL.Vertex3(x+1/16.0,     y-0.001, z+1/16.0);
						GL.Vertex3(x+1.0-1/16.0, y-0.001, z+1/16.0);
						GL.Vertex3(x+1.0-1/16.0, y-0.001, z+1.0-1/16.0);
						GL.Vertex3(x+1/16.0,     y-0.001, z+1.0-1/16.0);
						break;
					case Side.Front:
						GL.Vertex3(x+1.0-1/16.0, y+1.0-1/16.0, z+1.001);
						GL.Vertex3(x+1/16.0,     y+1.0-1/16.0, z+1.001);
						GL.Vertex3(x+1/16.0,     y+1/16.0,     z+1.001);
						GL.Vertex3(x+1.0-1/16.0, y+1/16.0,     z+1.001);
						break;
					case Side.Back:
						GL.Vertex3(x+1/16.0,     y+1.0-1/16.0, z-0.001);
						GL.Vertex3(x+1.0-1/16.0, y+1.0-1/16.0, z-0.001);
						GL.Vertex3(x+1.0-1/16.0, y+1/16.0,     z-0.001);
						GL.Vertex3(x+1/16.0,     y+1/16.0,     z-0.001);
						break;
				}
				GL.End();
			} else Target = null;
		}
		
		void PickMatrix(double x, double y, double width, double height, int[] viewport)
		{
			GL.MultMatrix(new double[]{
			              	viewport[2] / width, 0, 0, (viewport[2] + 2.0 * (viewport[0] - x)) / width,
			              	0, viewport[3] / height, 0, (viewport[3] + 2.0 * (viewport[1] - y)) / height,
			              	0, 0, 1, 0, 0, 0, 0, 1 });
		}
	}
}