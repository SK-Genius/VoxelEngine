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
		var Zoom = 1;
		
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
			Zoom
		);
		
		var Image = new Bitmap(
			Window.Width / Zoom,
			Window.Height / Zoom,
			cImageFormat
		);
		
		Window.Paint += (_, a) => {
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
				EditorState.Canvas,
				EditorState.Shadow,
				EditorState.DebugRenderMode
			);
			
			a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			//a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
			a.Graphics.Clear(System.Drawing.Color.White);
			a.Graphics.DrawImage(Image, 0, 0, EditorState.Zoom*Image.Width, EditorState.Zoom*Image.Height);
			a.Graphics.DrawString($"{NewTime - LastTimeRender} ms", DefaultFont, Brushes.Black, 100, 0);
			
			
			var P2D = LastMousePos;
			var P3D = V3();
			if (P2D.IsInRange(V2(), EditorState.Canvas.Size - V2(1))) {
				P3D = pRenderEnv.To3D(EditorState.Canvas, P2D);
			}
			
			a.Graphics.DrawString($"({P3D.X}, {P3D.Y}, {P3D.Z})", DefaultFont, Brushes.Black, 200, 0);
			LastTimeRender = NewTime;
		};
		
		Window.MouseDown += (_, a) => {
			var NewTime = SW.ElapsedMilliseconds;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tMouseKeyDown(
					(a.Button.HasFlag(MouseButtons.Left) ? tMouseKeys.Left : default) |
					(a.Button.HasFlag(MouseButtons.Middle) ? tMouseKeys.Middle : default) |
					(a.Button.HasFlag(MouseButtons.Right) ? tMouseKeys.Right : default)
				)
			);
			LastTimeUpdate = NewTime;
		};
		
		Window.MouseUp += (_, a) => {
			var NewTime = SW.ElapsedMilliseconds;
			EditorState.Update(
				NewTime - LastTimeUpdate,
				new tMouseKeyUp(
					(a.Button.HasFlag(MouseButtons.Left) ? tMouseKeys.Left : default) |
					(a.Button.HasFlag(MouseButtons.Middle) ? tMouseKeys.Middle : default) |
					(a.Button.HasFlag(MouseButtons.Right) ? tMouseKeys.Right : default)
				)
			);
			LastTimeUpdate = NewTime;
			
			EditorState.RenderEnv._Update();
			Window.Invalidate();
		};
		
		Window.MouseMove += (_, a) => {
			var NewTime = SW.ElapsedMilliseconds;
			var NewPos = V2(a.X, a.Y) / Zoom;
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
		
		Window.Resize += (_, a) => {
			var Size = V2(Window.Size.Width, Window.Size.Height) / EditorState.Zoom;
			EditorState.Canvas = CreateSprite(
				Size,
				V2()
			);
			
			Image.Dispose();
			Image = new Bitmap(Size.X, Size.Y, cImageFormat);
			Window.Invalidate();
		};
		
		Window.KeyDown += (_, a) => {
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
					System.Environment.Exit(0);
					break;	
				}
				case Keys.L:
				case Keys.Right: {
					var LightDirection = EditorState.RenderEnv.LightDirection;
					EditorState.RenderEnv._SetLightDirection(
						LightDirection + (V3(LightDirection.Z, 0, -LightDirection.X) >> 3)
					);
					break;
				}
				case Keys.J:
				case Keys.Left: {
					var LightDirection = EditorState.RenderEnv.LightDirection;
					EditorState.RenderEnv._SetLightDirection(
						LightDirection - (V3(LightDirection.Z, 0, -LightDirection.X) >> 3)
					);
					break;
				}
				case Keys.I:
				case Keys.Up: {
					var LightDirection = EditorState.RenderEnv.LightDirection;
					EditorState.RenderEnv._SetLightDirection(
						LightDirection + (V3(0, LightDirection.Z, -LightDirection.Y) >> 3)
					);
					break;
				}
				case Keys.K:
				case Keys.Down: {
					var LightDirection = EditorState.RenderEnv.LightDirection;
					EditorState.RenderEnv._SetLightDirection(
						LightDirection - (V3(0, LightDirection.Z, -LightDirection.Y) >> 3)
					);
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
