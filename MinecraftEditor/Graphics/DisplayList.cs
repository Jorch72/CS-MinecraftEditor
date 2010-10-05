using System;
using OpenTK.Graphics.OpenGL;

namespace MinecraftEditor.Graphics
{
	public class DisplayList : IDisposable
	{
		public int ID { get; private set; }
		
		public bool Cached {
			get { return (ID != -1); }
		}
		
		public DisplayList()
		{
			ID = -1;
		}
		
		public void Dispose()
		{
			if (ID == -1) return;
			GL.DeleteLists(ID, 1);
			ID = -1;
		}
		
		public void Begin()
		{
			Begin(ListMode.CompileAndExecute);
		}
		public void Begin(ListMode mode)
		{
			if (ID == -1) ID = GL.GenLists(1);
			GL.NewList(ID, mode);
		}
		public void End()
		{
			GL.EndList();
		}
		
		public void Call()
		{
			GL.CallList(ID);
		}
	}
}
