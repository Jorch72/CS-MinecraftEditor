using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace MinecraftEditor.Graphics
{
	public class Camera
	{
		bool _lastMouseEnabled = false;
		
		public Vector3d Location { get; set; }
		public Vector3 Rotation { get; set; }
		
		#region Location
		public double X {
			get { return Location.X; }
			set { Location = new Vector3d(value, Location.Y, Location.Z); }
		}
		public double Y {
			get { return Location.Y; }
			set { Location = new Vector3d(Location.X, value, Location.Z); }
		}
		public double Z {
			get { return Location.Z; }
			set { Location = new Vector3d(Location.X, Location.Y, value); }
		}
		#endregion
		#region Rotation
		public float Yaw {
			get { return Rotation.X; }
			set { Rotation = new Vector3(value, Rotation.Y, Rotation.Z); }
		}
		public float Pitch {
			get { return Rotation.Y; }
			set { Rotation = new Vector3(Rotation.X, value, Rotation.Z); }
		}
		public float Roll {
			get { return Rotation.Z; }
			set { Rotation = new Vector3(Rotation.X, Rotation.Y, value); }
		}
		#endregion
		
		public float YawSpeed { get; set; }
		public float PitchSpeed { get; set; }
		
		public float MoveSpeed { get; set; }
		public float MouseSpeed { get; set; }
		public bool MoveEnabled { get; set; }
		public bool MouseEnabled { get; set; }
		public Point MousePosition { get; set; }
		
		public Camera()
		{
			MoveSpeed = 20.0f;
			MouseSpeed = 6.0f;
		}
		
		public void Update(Window window, double time)
		{
			KeyboardDevice keyboard = window.Keyboard;
			MouseDevice mouse = window.Mouse;
			Point center = new Point(window.Width / 2, window.Height / 2);
			
			bool camEnabled = window.Focused;
			if (camEnabled && !MoveEnabled)
				MousePosition = new Point(mouse.X, mouse.Y);
			MoveEnabled = camEnabled;
			MouseEnabled = camEnabled;
			
			if (MouseEnabled && _lastMouseEnabled) {
				YawSpeed += (mouse.X - center.X) * MouseSpeed * (float)time;
				PitchSpeed += (mouse.Y - center.Y) * MouseSpeed * (float)time;
				Cursor.Position = window.PointToScreen(center);
			} else if (MouseEnabled && !_lastMouseEnabled) {
				Cursor.Position = window.PointToScreen(center);
				Cursor.Hide();
				_lastMouseEnabled = true;
			} else if (_lastMouseEnabled) {
				Cursor.Position = window.PointToScreen(MousePosition);
				Cursor.Show();
				_lastMouseEnabled = false;
			}
			YawSpeed *= 0.6f;
			PitchSpeed *= 0.6f;
			Yaw = (((Yaw + YawSpeed) % 360) + 360) % 360;
			Pitch = Math.Max(-90, Math.Min(90, Pitch + PitchSpeed));
			
			if (MoveEnabled && window.Focused) {
				float yaw = (float)Math.PI * Yaw / 180;
				float pitch = (float)Math.PI * Pitch / 180;
				double speed = MoveSpeed * time;
				if ((keyboard[Key.A] ^ keyboard[Key.D]) &&
				    (keyboard[Key.W] ^ keyboard[Key.S]))
					speed /= Math.Sqrt(2);
				if (keyboard[Key.ShiftLeft] && !keyboard[Key.ControlLeft]) speed *= 5;
				else if (keyboard[Key.ControlLeft] && !keyboard[Key.ShiftLeft]) speed /= 3;
				if (keyboard[Key.W] && !keyboard[Key.S]) {
					X -= Math.Sin(yaw) * Math.Cos(pitch) * speed;
					Y += Math.Sin(pitch) * speed;
					Z += Math.Cos(yaw) * Math.Cos(pitch) * speed;
				} else if (keyboard[Key.S] && !keyboard[Key.W]) {
					X += Math.Sin(yaw) * Math.Cos(pitch) * speed;
					Y -= Math.Sin(pitch) * speed;
					Z -= Math.Cos(yaw) * Math.Cos(pitch) * speed;
				}
				if (keyboard[Key.A] && !keyboard[Key.D]) {
					X += Math.Cos(yaw) * speed;
					Z += Math.Sin(yaw) * speed;
				} else if (keyboard[Key.D] && !keyboard[Key.A]) {
					X -= Math.Cos(yaw) * speed;
					Z -= Math.Sin(yaw) * speed;
				}
			}
		}
		
		public void Render()
		{
			GL.Rotate(Pitch, 1.0, 0.0, 0.0);
			GL.Rotate(Yaw, 0.0, 1.0, 0.0);
			GL.Rotate(Roll, 0.0, 0.0, 1.0);
			GL.Translate(Location);
		}
	}
}
