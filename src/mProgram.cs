using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using tBool = System.Boolean;

using tNat8 = System.Byte;
using tNat16 = System.UInt16;
using tNat32 = System.UInt32;
using tNat64 = System.UInt64;

using tInt8 = System.SByte;
using tInt16 = System.Int16;
using tInt32 = System.Int32;
using tInt64 = System.Int64;

using tChar = System.Char;
using tText = System.String;

static class
mProgram {
	
	[STAThread]
	static void
	Main(
	) {
		const tInt32 MaxX = 400;
		const tInt32 MaxY = 300;
		const tInt32 Zoom = 2;
		
		var Grid = mRenderEngine.CreateSprite(
			mMath2D.V2(MaxX, MaxY),
			mMath2D.V2(0, 0)
		);
		
		var Cube = new []{
			new sbyte[1, 1, 1],
			new sbyte[3, 3, 3],
			new sbyte[9, 9, 9] {
				{
					{ 7, 0, 0, 0, 0, 0, 0, 0, 7 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 3, 3, 3, 0, 0, 0 },
					{ 0, 0, 0, 3, 3, 3, 0, 0, 0 },
					{ 0, 0, 0, 3, 3, 3, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 7, 0, 0, 0, 0, 0, 0, 0, 7 },
				},
				{
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 1, 1, 1, 0, 0, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 0, 0, 1, 1, 1, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
				},
				{
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
				},
				{
					{ 0, 0, 0, 4, 4, 4, 0, 0, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 0, 0, 4, 4, 4, 0, 0, 0 },
				},
				{
					{ 0, 0, 0, 4, 4, 4, 0, 0, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 0, 0, 4, 4, 4, 0, 0, 0 },
				},
				{
					{ 0, 0, 0, 4, 4, 4, 0, 0, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 5, 2, 2, 2, 2, 2, 2, 2, 5 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 0, 0, 4, 4, 4, 0, 0, 0 },
				},
				{
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 1, 2, 2, 2, 2, 2, 1, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
				},
				{
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 1, 1, 1, 0, 0, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 1, 1, 2, 2, 2, 1, 1, 0 },
					{ 0, 0, 1, 1, 1, 1, 1, 0, 0 },
					{ 0, 0, 0, 1, 1, 1, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
				},
				{
					{ 7, 0, 0, 0, 0, 0, 0, 0, 7 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 3, 3, 3, 0, 0, 0 },
					{ 0, 0, 0, 3, 3, 3, 0, 0, 0 },
					{ 0, 0, 0, 3, 3, 3, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
					{ 7, 0, 0, 0, 0, 0, 0, 0, 7 },
				},
			},
		};
		
		mRenderEngine.Clear(Grid);
		
		var Image = new Bitmap(1*MaxX, 1*MaxY, PixelFormat.Format32bppArgb);
		
		var Window = new Form{
			Width = 1000,
			Height = 800,
			WindowState = FormWindowState.Maximized,
		};
		
		typeof(Control).GetProperty(
			"DoubleBuffered",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
		).SetValue(Window, true, null);
		
		var RenderEnv = new mRenderEngine.tRenderEnv();
		RenderEnv.Update();
		
		Window.Paint += (object _, PaintEventArgs a) => {
			Grid.Clear();
			Grid.DrawCube(Cube[2], RenderEnv.NormalPattern, RenderEnv.M);
			Image.DrawGrid(Grid);
			a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			a.Graphics.DrawImage(Image, 0, 0, Zoom*MaxX, Zoom*MaxY);
		};
		
		void
		MouseEvent(
			object _,
			MouseEventArgs a
		) {
			var X = (a.X + ((Zoom + 1)>>1)) / Zoom;
			var Y = (a.Y + ((Zoom + 1)>>1)) / Zoom;
			if (X.InRange(0, MaxX - 1) && Y.InRange(0, MaxY - 1)) {
				switch (a.Button) {
					case MouseButtons.Left: {
						var X_ = a.X / Zoom;
						var Y_ = a.Y / Zoom;
						var Z_ = Grid.Deep[X, Y];
						var P2D = mMath3D.V3(X_, Y_, Z_);
						var P3D = (P2D * RenderEnv.InvM) / RenderEnv.Det;
						
						Window.Text = $"{P2D} -> {P3D}";
						if (Grid.Color[X, Y] != 1) {
							Grid.Color[X, Y] = 1;
							Window.Invalidate();
						}
						break;
					}
					case MouseButtons.Right: {
						if (Grid.Color[X, Y] != 2) {
							Grid.Color[X, Y] = 2;
							Window.Invalidate();
						}
						break;
					}
					case MouseButtons.Left | MouseButtons.Right: {
						if (Grid.Color[X, Y] != 3) {
							Grid.Color[X, Y] = 3;
							Window.Invalidate();
						}
						break;
					}
					case MouseButtons.Middle: {
						if (Grid.Color[X, Y] != 0) {
							Grid.Color[X, Y] = 0;
							Window.Invalidate();
						}
						break;
					}
				}
			}
			Window.Refresh();
		}
		
		Window.MouseMove += MouseEvent;
		Window.MouseDown += MouseEvent;
		Window.KeyPress += (_, a) => {
			switch (a.KeyChar) {
				case 'D':
				case 'd': {
					RenderEnv.Dir += 1;
					RenderEnv.Update();
					Window.Invalidate();
					break;
				}
				case 'A':
				case 'a': {
					RenderEnv.Dir -= 1;
					RenderEnv.Update();
					Window.Invalidate();
					break;
				}
				case 'W':
				case 'w': {
					RenderEnv.Angle += 1;
					RenderEnv.Update();
					Window.Invalidate();
					break;
				}
				case 'S':
				case 's': {
					RenderEnv.Angle -= 1;
					RenderEnv.Update();
					Window.Invalidate();
					break;
				}
			}
		};
		
		Grid.DrawCube(Cube[2], RenderEnv.NormalPattern, RenderEnv.M);
		
		Application.Run(Window);
		Environment.Exit(0);
	}
	
	static unsafe void
	DrawGrid(
		this Bitmap aImage,
		mRenderEngine.tSprite aGrid
	) {
		var MaxX = Math.Min(aGrid.Color.GetLength(0), aImage.Width);
		var MaxY = Math.Min(aGrid.Color.GetLength(1), aImage.Height);
		var DeltaX = MaxX - aImage.Width;
		
		var Lock = aImage.LockBits(
			new Rectangle(new Point(0, 0), aImage.Size),
			ImageLockMode.WriteOnly,
			aImage.PixelFormat
		);
		var ColorMap = new uint[] {
			0xFF__00_00_00,
			0xFF__00_00_FF,
			0xFF__00_FF_00,
			0xFF__00_FF_FF,
			0xFF__FF_00_00,
			0xFF__FF_00_FF,
			0xFF__FF_FF_00,
			0xFF__FF_FF_FF,
			0xFF__00_00_00,
			0xFF__00_00_88,
			0xFF__00_88_00,
			0xFF__00_88_88,
			0xFF__88_00_00,
			0xFF__88_00_88,
			0xFF__88_88_00,
			0xFF__88_88_88,
			0xFF__00_00_00,
			0xFF__00_00_44,
			0xFF__00_44_00,
			0xFF__00_44_44,
			0xFF__44_00_00,
			0xFF__44_00_44,
			0xFF__44_44_00,
			0xFF__44_44_44,
		};
		
		try {
			var Ptr = (uint*)Lock.Scan0;
			for (var Y = 0; Y < MaxY; Y += 1) {
				for (var X = 0; X < MaxX; X += 1) {
					*Ptr = ColorMap[aGrid.Color[X, Y]];
					Ptr += 1;
				}
				Ptr += DeltaX;
			}
		} finally {
			aImage.UnlockBits(Lock);
		}
	}
	
}
