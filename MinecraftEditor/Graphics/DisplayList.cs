using System;
using OpenTK.Graphics.OpenGL;

namespace MinecraftEditor.Graphics
{
	public class DisplayList : IDisposable
	{
		public int ID { get; private set; }
		
		public DisplayList()
		{
			ID = GL.GenLists(1);
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
