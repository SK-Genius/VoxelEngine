using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using static mEvents;
using static mMath;
using static mV2;
using static mV3;
using static mVoxelRenderer;
using static mVoxelEditor;

public static class
mVoxelEditorWin {
	
	private static readonly tNat8 c000p = 0;
	private static readonly tNat8 c025p = 1;
	private static readonly tNat8 c050p = 2;
	private static readonly tNat8 c075p = 3;
	private static readonly tNat8 c100p = 4;
	
	private static readonly PixelFormat cImageFormat = PixelFormat.Format32bppArgb;
	
	[STAThread]
	static void
	Main(
	) {
		var LastMousePos = V2();
		
		var DefaultFont = new Font("Arial", 10);
		
		var Window = new Form{
			Width = 1000,
			Height = 800,
			WindowState = FormWindowState.Maximized,
		};
		
		typeof(Control).GetProperty(
			"DoubleBuffered",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
		)?.SetValue(Window, true, null);
		
		var SW = new Stopwatch();
		SW.Start();
		var LastTimeUpdate = SW.ElapsedMilliseconds;
		var LastTimeRender = SW.ElapsedMilliseconds;
		
		var EditorState = mVoxelEditor.Create(
			V2(Window.Width, Window.Height),
			1
		);
		
		var Image = new Bitmap(
			Window.Width / EditorState.Zoom,
			Window.Height / EditorState.Zoom,
			cImageFormat
		);
		
		Window.Paint += (_, a) => {
			using var PerfLog = mPerfLog.LogPerf("Window.Paint");
			
			ref var pRenderEnv = ref EditorState.RenderEnv;
			
			var NewTime = SW.ElapsedMilliseconds;
			
			EditorState.MousePos = LastMousePos;
			
			Render(
				ref EditorState,
				NewTime - LastTimeRender
			);
			
			unsafe {
				var x = Image.LockBits(
					new Rectangle(0, 0, Image.Width, Image.Height),
					ImageLockMode.WriteOnly,
					cImageFormat
				);
				
				var p = (tNat32*)x.Scan0;
				for (var I = Image.Width * Image.Height; I --> 0; ) {
					*p = 0xFF_FF_FF_FF;
					p += 1;
				}
				
				Image.UnlockBits(x);
			} 
			
			pRenderEnv._RenderTo(
				ref Image,
				EditorState.Zoom,
				EditorState.Canvas,
				EditorState.Shadow,
				EditorState.DebugRenderMode
			);
			
			a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			//a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
			a.Graphics.Clear(System.Drawing.Color.White);
			
			var drawWidth = EditorState.Zoom * Image.Width;
			var drawHeight = EditorState.Zoom * Image.Height;
			var offsetX = (Window.ClientSize.Width - drawWidth) / 2;
			var offsetY = (Window.ClientSize.Height - drawHeight) / 2;
			a.Graphics.DrawImage(Image, offsetX, offsetY, drawWidth, drawHeight);
			
			a.Graphics.DrawString($"{NewTime - LastTimeRender} ms", DefaultFont, Brushes.Black, 100, 0);
			
			
			var P2D = LastMousePos;
			var P3D = V3();
			if (P2D.IsInRange(V2(), EditorState.Canvas.Size - V2(1))) {
				P3D = pRenderEnv.To3D(EditorState.Canvas, P2D);
			}
			
			a.Graphics.DrawString($"({P3D.X}, {P3D.Y}, {P3D.Z})", DefaultFont, Brushes.Black, 200, 0);
			var P2D_ = P2D * 1;//EditorState.Zoom;
			a.Graphics.DrawString($"({P2D_.X}, {P2D_.Y})", DefaultFont, Brushes.Black, 300, 0);
			if (P2D.IsInRange(V2(), EditorState.Canvas.Size - V2(1))) {
				try {
					
					var Color = EditorState.Canvas.Color[P2D_.X, P2D_.Y];
					a.Graphics.DrawString($"{Color.Value} ({Color.Value % 5}, {(Color.Value / 5) % 5}, {(Color.Value / (5 * 5)) % 5})", DefaultFont, Brushes.Black, 200, 10);
					
					var Deep = EditorState.Canvas.Deep[P2D_.X, P2D_.Y];
					a.Graphics.DrawString($"({P2D_.X}, {P2D_.Y}, {Deep})", DefaultFont, Brushes.Black, 200, 20);
				} catch {
				}
			}			
			LastTimeRender = NewTime;
		};
		
		Window.MouseDown += (_, a) => {
			var NewTime = SW.ElapsedMilliseconds;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tKeyDown(
					(a.Button.HasFlag(MouseButtons.Left) ? tKeys.MouseLeft : default) |
					(a.Button.HasFlag(MouseButtons.Middle) ? tKeys.MouseMiddle : default) |
					(a.Button.HasFlag(MouseButtons.Right) ? tKeys.MouseRight : default)
				)
			);
			LastTimeUpdate = NewTime;
			
			EditorState.RenderEnv._Update();
			Window.Invalidate();
		};
		
		Window.MouseUp += (_, a) => {
			var NewTime = SW.ElapsedMilliseconds;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tKeyUp(
					(a.Button.HasFlag(MouseButtons.Left) ? tKeys.MouseLeft : default) |
					(a.Button.HasFlag(MouseButtons.Middle) ? tKeys.MouseMiddle : default) |
					(a.Button.HasFlag(MouseButtons.Right) ? tKeys.MouseRight : default)
				)
			);
			LastTimeUpdate = NewTime;
			
			EditorState.RenderEnv._Update();
			Window.Invalidate();
		};
		
		Window.MouseMove += (_, a) => {
			var drawWidth = EditorState.Zoom * Image.Width;
			var drawHeight = EditorState.Zoom * Image.Height;
			var offsetX = (Window.ClientSize.Width - drawWidth) / 2;
			var offsetY = (Window.ClientSize.Height - drawHeight) / 2;
			var NewTime = SW.ElapsedMilliseconds;
			var NewPos = V2(a.X - offsetX, a.Y - offsetY) / EditorState.Zoom;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tMouseMove(
					LastMousePos,
					NewPos
				)
			);
			LastMousePos = NewPos;
			LastTimeUpdate = NewTime;
			
			EditorState.RenderEnv._Update();
			Window.Invalidate();
		};
		
		Window.MouseWheel += (_, a) => {
			var drawWidth = EditorState.Zoom * Image.Width;
			var drawHeight = EditorState.Zoom * Image.Height;
			var offsetX = (Window.ClientSize.Width - drawWidth) / 2;
			var offsetY = (Window.ClientSize.Height - drawHeight) / 2;
			var NewTime = SW.ElapsedMilliseconds;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tMouseScroll(
					a.Delta.Sign()
				)
			);
			
			var NewPos = V2(a.X - offsetX, a.Y - offsetY) / EditorState.Zoom;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tMouseMove(
					LastMousePos,
					NewPos
				)
			);
			LastMousePos = NewPos;
			
			LastTimeUpdate = NewTime;
			
			EditorState.RenderEnv._Update();
			Window.Invalidate();
		};
		
		Window.KeyDown += (_ , a) => {
			var Key = (
				a.KeyCode is Keys.ShiftKey ? tKeys.Shift :
				a.KeyCode is Keys.ControlKey ? tKeys.Control :
				a.KeyCode is Keys.Menu ? tKeys.Alt :
				a.KeyCode is Keys.Up ? tKeys.Up :
				a.KeyCode is Keys.Down ? tKeys.Down :
				a.KeyCode is Keys.Left ? tKeys.Left :
				a.KeyCode is Keys.Right ? tKeys.Right :
				tKeys.None
			);
			
			if (Key is tKeys.None) {
				return;
			}
			
			var NewTime = SW.ElapsedMilliseconds;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tKeyDown(Key)
			);
			LastTimeUpdate = NewTime;
			
			EditorState.RenderEnv._Update();
			Window.Invalidate();
		};
		
		Window.KeyUp += (_ , a) => {
			var Key = (
				a.KeyCode is Keys.ShiftKey ? tKeys.Shift :
				a.KeyCode is Keys.ControlKey ? tKeys.Control :
				a.KeyCode is Keys.Menu ? tKeys.Alt :
				a.KeyCode is Keys.Up ? tKeys.Up :
				a.KeyCode is Keys.Down ? tKeys.Down :
				a.KeyCode is Keys.Left ? tKeys.Left :
				a.KeyCode is Keys.Right ? tKeys.Right :
				tKeys.None
			);
			
			if (Key is tKeys.None) {
				return;
			}
			
			var NewTime = SW.ElapsedMilliseconds;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tKeyUp(Key)
			);
			LastTimeUpdate = NewTime;
			
			EditorState.RenderEnv._Update();
			Window.Invalidate();
		};
		
		Window.Resize += (_, a) => {
			var Size = V2(Window.Size.Width, Window.Size.Height) / EditorState.Zoom;
			EditorState.Canvas = CreateSprite(Size);
			Image.Dispose();
			Image = new Bitmap(Size.X, Size.Y, cImageFormat);
			Window.Invalidate();
		};
		
		var LightHeight = 1f;
		var LightDir = 0f;
		
		Window.KeyDown += (_, a) => {
			// Move to HotReload code
			var NeedsUpdate = true;
			
			switch (a.KeyCode) {
				case Keys.F1: {
					EditorState.DebugRenderMode = tDebugRenderMode.None;
					break;
				}
				case Keys.F2: {
					EditorState.DebugRenderMode = tDebugRenderMode.Deep;
					break;
				}
				case Keys.F3: {
					EditorState.DebugRenderMode = tDebugRenderMode.Normal;
					break;
				}
				case Keys.F4: {
					EditorState.DebugRenderMode = tDebugRenderMode.Pos;
					break;
				}
				case Keys.F5: {
					EditorState.DebugRenderMode = tDebugRenderMode.PosBits;
					break;
				}
				case Keys.F6: {
					EditorState.DebugRenderMode = tDebugRenderMode.Pattern;
					break;
				}
				case Keys.Escape: {
					System.IO.File.WriteAllText(
						@"c:\Temp\VoxelGamePerf.log",
						mPerfLog.Print()
					);
					Environment.Exit(0);
					break;	
				}
				case Keys.L:
				case Keys.Right: {
					LightDir += 1f / 37;
					if (LightDir >= 1) {
						LightDir -= 1;
					}
					break;
				}
				case Keys.J:
				case Keys.Left: {
					LightDir -= 1f / 37;
					if (LightDir < 0) {
						LightDir += 1;
					}
					break;
				}
				case Keys.I:
				case Keys.Up: {
					LightHeight = (LightHeight + 1f / 18).Clamp(-1, 1);
					break;
				}
				case Keys.K:
				case Keys.Down: {
					LightHeight = (LightHeight - 1f / 18).Clamp(-1, 1);
					break;
				}
				case Keys.D: {
					EditorState.RenderEnv.Dir += 1;
					break;
				}
				case Keys.A: {
					EditorState.RenderEnv.Dir -= 1;
					break;
				}
				case Keys.W: {
					EditorState.RenderEnv.Angle += 1;
					break;
				}
				case Keys.S: {
					EditorState.RenderEnv.Angle -= 1;
					break;
				}
				default: {
					NeedsUpdate = false;
					break;
				}
			}
			if (NeedsUpdate) {
				var SinZ = mMath.Sin(LightHeight * mMath.cPi);
				var CosZ = mMath.Cos(LightHeight * mMath.cPi);
				var VDir = mV3.V3(
					(tInt32)(100 * mMath.Sin(2 * mMath.cPi * LightDir) * CosZ),
					(tInt32)(100 * mMath.Cos(2 * mMath.cPi * LightDir) * CosZ),
					(tInt32)(100 * SinZ)
				);
				var Max = VDir.Abs().Max();
				EditorState.RenderEnv._SetLightDirection(
					VDir * 9 / Max
				);
				EditorState.RenderEnv._Update();
				Window.Invalidate();
			}
		};
		
		Application.Run(Window);
		Environment.Exit(0);
	}
	
	static tRenderEnv
	_RenderTo(
		this tRenderEnv aRenderEnv,
		ref Bitmap aImage,
		tInt32 aScale, 
		tSprite aSprite,
		tShadow aShadow,
		tDebugRenderMode aDebugRenderMode
	) {
		var BufferSize = V2(aImage.Width, aImage.Height);
		var Lock = aImage.LockBits(
			new Rectangle(new Point(0, 0), aImage.Size),
			ImageLockMode.WriteOnly,
			aImage.PixelFormat
		);
		try {
			aRenderEnv._RenderToBuffer(aSprite, aShadow, Lock.Scan0, BufferSize, aDebugRenderMode);
		} finally {
			aImage.UnlockBits(Lock);
		}
		return aRenderEnv;
	}
}
