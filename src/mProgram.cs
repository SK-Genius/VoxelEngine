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
		const tInt32 Zoom = 1;
		
		var R = mRenderEngine.RGB(4, 0, 0);
		var G = mRenderEngine.RGB(0, 4, 0);
		var B = mRenderEngine.RGB(0, 0, 4);
		var Y = mRenderEngine.RGB(4, 4, 0);
		var C = mRenderEngine.RGB(0, 4, 4);
		var V = mRenderEngine.RGB(4, 0, 4);
		var W = mRenderEngine.RGB(4, 4, 4);
		var _ = (tNat8)0;
		
		var Cube = new []{
			new tNat8[1, 1, 1] {
				{
					{ 1 },
				},
			},
			new tNat8[3, 3, 3] {
				{
					{ W, C, W },
					{ C, B, C },
					{ W, C, W },
				},
				{
					{ C, G, C },
					{ R, V, R },
					{ C, G, C },
				},
				{
					{ W, C, W },
					{ C, B, C },
					{ W, C, W },
				}
			},
			new tNat8[9, 9, 9] {
				{
					{ W, _, _, _, _, _, _, _, W },
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, B, B, B, _, _, _ },
					{ _, _, _, B, B, B, _, _, _ },
					{ _, _, _, B, B, B, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
					{ W, _, _, _, _, _, _, _, W },
				},
				{
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, C, C, C, _, _, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, _, _, C, C, C, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
				},
				{
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
				},
				{
					{ _, _, _, G, G, G, _, _, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ R, V, V, V, V, V, V, V, R },
					{ R, V, V, V, V, V, V, V, R },
					{ R, V, V, V, V, V, V, V, R },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, _, _, G, G, G, _, _, _ },
				},
				{
					{ _, _, _, G, G, G, _, _, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ R, V, V, V, V, V, V, V, R },
					{ R, V, V, V, V, V, V, V, R },
					{ R, V, V, V, V, V, V, V, R },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, _, _, G, G, G, _, _, _ },
				},
				{
					{ _, _, _, G, G, G, _, _, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ R, V, V, V, V, V, V, V, R },
					{ R, V, V, V, V, V, V, V, R },
					{ R, V, V, V, V, V, V, V, R },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, _, _, G, G, G, _, _, _ },
				},
				{
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, C, V, V, V, V, V, C, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
				},
				{
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, C, C, C, _, _, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, C, C, V, V, V, C, C, _ },
					{ _, _, C, C, C, C, C, _, _ },
					{ _, _, _, C, C, C, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
				},
				{
					{ W, _, _, _, _, _, _, _, W },
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, B, B, B, _, _, _ },
					{ _, _, _, B, B, B, _, _, _ },
					{ _, _, _, B, B, B, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _, _ },
					{ W, _, _, _, _, _, _, _, W },
				},
			},
		};
		
		var MousePos = mMath2D.V2(0, 0);
		
		var Canvas = mRenderEngine.CreateSprite(
			mMath2D.V2(2000 / Zoom, 1000 / Zoom),
			mMath2D.V2(0, 0)
		);
		
		var Window = new Form{
			Width = 1000,
			Height = 800,
			WindowState = FormWindowState.Maximized,
		};
		
		typeof(Control).GetProperty(
			"DoubleBuffered",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
		)?.SetValue(Window, true, null);
		
		var RenderEnv = mRenderEngine.CreateEnv();
		RenderEnv.Update();
		
		Window.Paint += (object _, PaintEventArgs a) => {
			var P3D = mMath3D.V3(0, 0, 0);
			
			var Cube_ = Cube[2];
			
			var SpriteSize = RenderEnv.SpriteSize(Cube_.GetLength(0));
			var Sprite = mRenderEngine.CreateSprite(
				SpriteSize,
				mMath2D.V2(0, 0)
			);
			
			Sprite.Clear();
			Sprite.DrawCube(Cube_, RenderEnv.NormalPattern, RenderEnv.M);
			
			var P2D = MousePos;
			var Size = Cube[2].GetLength(0);
			var HSize = Size / 2;
			if (P2D.X.InRange(0, Canvas.Size.X - 1) && P2D.Y.InRange(0, Canvas.Size.Y - 1)) {
				P3D = RenderEnv.To3D(Canvas, P2D) + mMath3D.V3(HSize, HSize, HSize);
				Window.Text = $"{P2D} -> {P3D}";
			}
			
			var C = new tNat8[Size, Size, Size];
			var BlackTransparent = mRenderEngine.RGBA(0, 0, 0, true);
			for (var I = 0; I < Size; I += 1) {
				if (P3D.X.InRange(0, Size-1) &&
					P3D.Y.InRange(0, Size-1) &&
					P3D.Z.InRange(0, Size-1)
				) {
					C[I, P3D.Y, P3D.Z] = BlackTransparent;
					C[P3D.X, I, P3D.Z] = BlackTransparent;
					C[P3D.X, P3D.Y, I] = BlackTransparent;
				}
			}
			Canvas.Clear();
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(-2, 2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(-1, 2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(0, 2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(1, 2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(2, 2, 0) * RenderEnv.M);
			
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(2, -1, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(2, 0, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(2, 1, 0) * RenderEnv.M);
			
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(-2, -1, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(-2, 0, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(-2, 1, 0) * RenderEnv.M);
			
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(-2, -2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(-1, -2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(0, -2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(1, -2, 0) * RenderEnv.M);
			Canvas.DrawSprite(Sprite, 9 * mMath3D.V3(2, -2, 0) * RenderEnv.M);
			
			Sprite.DrawCube(C, RenderEnv.NormalPattern, RenderEnv.M);
			Canvas.DrawSprite(Sprite, mMath3D.V3(0, 0, 0));
			
			using var Image = new Bitmap(Canvas.Size.X, Canvas.Size.Y, PixelFormat.Format32bppArgb);
			Image.DrawSprite(Canvas, mMath3D.V3(-70, 170, 170)); // (left, top, front)
			//a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
			a.Graphics.Clear(System.Drawing.Color.White);
			a.Graphics.DrawImage(Image, 40, 40, Zoom*Image.Width / 2, Zoom*Image.Height / 2);
		};
		
		void
		MouseEvent(
			object _,
			MouseEventArgs a
		) {
			MousePos = mMath2D.V2(
				2 * (a.X - 40) / Zoom,
				2 * (a.Y - 40) / Zoom
			);
			Window.Invalidate();
		}
		
		Window.MouseMove += MouseEvent;
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
		
		Application.Run(Window);
		Environment.Exit(0);
	}
	
	static void
	DrawSprite(
		this Bitmap aImage,
		mRenderEngine.tSprite aSprite,
		mMath3D.tV3 aLightVector
	) {
		var Size = mMath2D.V2(aImage.Width, aImage.Height);
		var Lock = aImage.LockBits(
			new Rectangle(new Point(0, 0), aImage.Size),
			ImageLockMode.WriteOnly,
			aImage.PixelFormat
		);
		try {
			aSprite.RenderToBuffer(Lock.Scan0, Size, aLightVector);
		} finally {
			aImage.UnlockBits(Lock);
		}
	}
}
