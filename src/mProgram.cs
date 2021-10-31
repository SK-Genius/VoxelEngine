using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

static class
mProgram {
	
	[STAThread]
	static void
	Main(
	) {
		const tInt32 MaxX = 400;
		const tInt32 MaxY = 300;
		const tInt32 Zoom = 4;
		
		var Sprite = mRenderEngine.CreateSprite(
			mMath2D.V2(MaxX, MaxY),
			mMath2D.V2(0, 0)
		);
		
		var Cube = new []{
			new sbyte[1, 1, 1] { { { 1 }, }, },
			new sbyte[3, 3, 3] {
				{
					{ 7, 1, 7 },
					{ 1, 3, 1 },
					{ 7, 1, 7 },
				},
				{
					{ 1, 4, 1 },
					{ 5, 2, 5 },
					{ 1, 4, 1 },
				},
				{
					{ 7, 1, 7 },
					{ 1, 3, 1 },
					{ 7, 1, 7 },
				}
			},
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
		
		var MousePos = mMath2D.V2(0, 0);
		
		mRenderEngine.Clear(Sprite);
		
		var Image = new Bitmap(1*MaxX, 1*MaxY, PixelFormat.Format32bppArgb);
		
		var Window = new Form{
			Width = 1000,
			Height = 800,
			WindowState = FormWindowState.Maximized,
		};
		
		typeof(Control).GetProperty(
			"DoubleBuffered",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
		)?.SetValue(Window, true, null);
		
		var RenderEnv = new mRenderEngine.tRenderEnv();
		RenderEnv.Update();
		
		Window.Paint += (object _, PaintEventArgs a) => {
			var P3D = mMath3D.V3(0, 0, 0);
			Sprite.Clear();
			Sprite.DrawCube(Cube[2], RenderEnv.NormalPattern, RenderEnv.M);
			
			var P2D = mMath2D.V2(MousePos.X, MousePos.Y);
			if (P2D.X.InRange(0, MaxX - 1) && P2D.Y.InRange(0, MaxY - 1)) {
				var CubeLength = Cube[2].GetLength(0);
				P3D = Sprite.To3D(P2D, RenderEnv, CubeLength);
				
				Window.Text = $"{P2D} -> {P3D}";
			}
			
			var Size = Cube[2].GetLength(0);
			var C = new tInt8[Size, Size, Size];
			for (var I = 0; I < Size; I += 1) {
				if (P3D.X.InRange(0, Size-1) &&
					P3D.Y.InRange(0, Size-1) &&
					P3D.Z.InRange(0, Size-1)
				) {
					C[I, P3D.Y, P3D.Z] = (tInt8)(7 - 128);
					C[P3D.X, I, P3D.Z] = (tInt8)(7 - 128);
					C[P3D.X, P3D.Y, I] = (tInt8)(7 - 128);
				}
			}
			Sprite.DrawCube(C, RenderEnv.NormalPattern, RenderEnv.M);
			Image.DrawSprite(Sprite);
			a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			a.Graphics.DrawImage(Image, 0, 0, Zoom*MaxX, Zoom*MaxY);
			
			//var Font_ = new Font("Arial", 8);
			//var M = RenderEnv.M;
			//a.Graphics.DrawString($"{M.X}", Font_, Brushes.Red, new PointF(0, 10));
			//a.Graphics.DrawString($"{M.Y}", Font_, Brushes.Red, new PointF(0, 20));
			//a.Graphics.DrawString($"{M.Z}", Font_, Brushes.Red, new PointF(0, 30));
			//a.Graphics.DrawString($"Dir:{RenderEnv.Dir} Angle:{RenderEnv.Angle}", Font_, Brushes.Red, new PointF(0, 40));
			//
			//M = RenderEnv.InvM;
			//a.Graphics.DrawString($"{M.X}", Font_, Brushes.Red, new PointF(0, 50));
			//a.Graphics.DrawString($"{M.Y}", Font_, Brushes.Red, new PointF(0, 60));
			//a.Graphics.DrawString($"{M.Z}", Font_, Brushes.Red, new PointF(0, 70));
			//a.Graphics.DrawString($"{RenderEnv.Det}", Font_, Brushes.Red, new PointF(0, 80));
		};
		
		void
		MouseEvent(
			object _,
			MouseEventArgs a
		) {
			var X = a.X / Zoom;
			var Y = a.Y / Zoom;
			MousePos = mMath2D.V2(X, Y);
			Window.Invalidate();
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
		
		Sprite.DrawCube(Cube[2], RenderEnv.NormalPattern, RenderEnv.M);
		
		Application.Run(Window);
		Environment.Exit(0);
	}
	
	static unsafe void
	DrawSprite(
		this Bitmap aImage,
		mRenderEngine.tSprite aSprite
	) {
		var MaxX = Math.Min(aSprite.Color.GetLength(0), aImage.Width);
		var MaxY = Math.Min(aSprite.Color.GetLength(1), aImage.Height);
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
					var ColorIndex = aSprite.Color[X, Y];
					var Color = ColorMap[ColorIndex];
					*Ptr = Color;
					Ptr += 1;
				}
				Ptr += DeltaX;
			}
		} finally {
			aImage.UnlockBits(Lock);
		}
	}
}
