using System;

namespace MinecraftEditor
{
	public static class FPS
	{
		static FPS()
		{
			Interval = new TimeSpan(0, 0, 2);
		}
		
		static DateTime _time = DateTime.Now;
		static int _frames = 0;
		
		public static double Count { get; private set; }
		public static TimeSpan Interval { get; private set; }
		
		public static void RenderFrame()
		{
			_frames++;
			if (DateTime.Now.Subtract(_time) >= Interval) {
				Count = _frames;
				_frames = 0;
				_time = _time + Interval;
			}
		}
	}
}
