using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

static class
mProgram {
	
	[STAThread]
	static void
	Main(
	) {
		var Zoom = 2;
		var LoD = 2; // 0..2 // TODO: move into RenderEnv ?
		
		var LoD_Size = 1; // TODO: move into RenderEnv ?
		for (var I = 0; I < LoD; I += 1) {
			LoD_Size *= 3;
		}
		
		var R = mRenderEngine.RGB(4, 0, 0);
		var G = mRenderEngine.RGB(0, 4, 0);
		var B = mRenderEngine.RGB(0, 0, 4);
		var Y = mRenderEngine.RGB(4, 4, 0);
		var C = mRenderEngine.RGB(0, 4, 4);
		var V = mRenderEngine.RGB(4, 0, 4);
		var W = mRenderEngine.RGB(4, 4, 4);
		var _ = default(mRenderEngine.tColor);
		
		var Cube = new []{
			new mRenderEngine.tColor[1, 1, 1] {
				{
					{ C },
				},
			},
			new mRenderEngine.tColor[3, 3, 3] {
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
			new mRenderEngine.tColor[9, 9, 9] {
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
				
		var Map = new Dictionary<mMath3D.tV3, mRenderEngine.tColor[,,]>();
		Map[LoD_Size * mMath3D.V3(-2, 2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(-1, 2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(0, 2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(1, 2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(2, 2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(2, -1, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(2, 0, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(2, 1, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(-2, -1, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(-2, 0, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(-2, 1, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(-2, -2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(-1, -2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(0, -2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(1, -2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(2, -2, 0)] = Cube[LoD];
		Map[LoD_Size * mMath3D.V3(0, 0, 0)] = Cube[LoD];
		
		Window.Paint += (_, a) => {
			RenderToCanvas(
				RenderEnv,
				MousePos,
				Map,
				LoD_Size,
				ref Canvas
			);
			
			using var Image = new Bitmap(Canvas.Size.X, Canvas.Size.Y, PixelFormat.Format32bppArgb);
			Image
			._DrawSprite(Canvas, mMath3D.V3(-70, 170, 170)); // (left, top, front)
			
			//a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
			a.Graphics.Clear(System.Drawing.Color.White);
			a.Graphics.DrawImage(Image, 40, 40, Zoom*Image.Width / 2, Zoom*Image.Height / 2);
		};
		
		Window.MouseMove += (_, a) => {
			var NewMousePos = mMath2D.V2(
				2 * (a.X - 40) / Zoom,
				2 * (a.Y - 40) / Zoom
			);
			
			if (a.Button.HasFlag(MouseButtons.Right)) {
				var D = NewMousePos - MousePos;
				RenderEnv.Dir += D.X / 2;
				RenderEnv.Angle += D.Y / 2;
				
				System.Diagnostics.Debug.WriteLine($"DELTA: ({a.X}, {a.Y}) => D: {RenderEnv.Dir}; A: {RenderEnv.Angle}");
				
				RenderEnv.Update();
				Window.Invalidate();
			}
			
			MousePos = NewMousePos;
		};
		
		Window.KeyDown += (_, a) => {
			switch (a.KeyCode) {
				case Keys.Right:
				case Keys.D: {
					RenderEnv.Dir += 1;
					RenderEnv.Update();
					Window.Invalidate();
					break;
				}
				case Keys.Left:
				case Keys.A: {
					RenderEnv.Dir -= 1;
					RenderEnv.Update();
					Window.Invalidate();
					break;
				}
				case Keys.Up:
				case Keys.W: {
					RenderEnv.Angle += 1;
					RenderEnv.Update();
					Window.Invalidate();
					break;
				}
				case Keys.Down:
				case Keys.S: {
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
	RenderToCanvas(
		mRenderEngine.tRenderEnv aRenderEnv,
		mMath2D.tV2 aMousePos,
		Dictionary<mMath3D.tV3, mRenderEngine.tColor[,,]> aMap, // 
		tInt32 aStdCubeSize,
		ref mRenderEngine.tSprite aCanvas
	) {
		var Size = aStdCubeSize;
		var HSize = Size / 2;
		
		{ // cubes
			aCanvas
			._Clear();
			
			foreach (var (Pos, Cube) in aMap) {
				aCanvas
				._DrawSprite(
					aRenderEnv.GetOrCreateSprite(Cube),
					Pos * aRenderEnv.M
				);
			}
		}
		
		{ // axes lines
			var P2D = aMousePos;
			var P3D = mMath3D.V3(0, 0, 0);
			if (P2D.X.IsInRange(0, aCanvas.Size.X - 1) && P2D.Y.IsInRange(0, aCanvas.Size.Y - 1)) {
				P3D = aRenderEnv.To3D(aCanvas, P2D);
				if (P3D.Z != tInt16.MaxValue) {
					P3D += mMath3D.V3(HSize, HSize, HSize);
				}
			} else {
				P3D = mMath3D.V3(0, 0, tInt16.MaxValue);
			}
			
			if (P3D.Z != tInt16.MaxValue) {
				var BlockUnderMouse = new mRenderEngine.tColor[3, 3, 3];
				for (var Z = -1; Z <= 1; Z += 1) {
					for (var Y = -1; Y <= 1; Y += 1) {
						for (var X = -1; X <= 1; X += 1) {
							if (
								aMap.Any(
									(_) => (
										_.Key.X.IsInRange(P3D.X + X - _.Value.GetLength(0), P3D.X + X + _.Value.GetLength(0)) &&
										_.Key.Y.IsInRange(P3D.Y + Y - _.Value.GetLength(1), P3D.Y + Y + _.Value.GetLength(1)) &&
										_.Key.Z.IsInRange(P3D.Z + Z - _.Value.GetLength(2), P3D.Z + Z + _.Value.GetLength(2))
									)
								)
							) {
								//if (Cube_[X + 1, Y + 0, Z + 1] != 0) {
								BlockUnderMouse[X + 1, Y + 1, Z + 1] = mRenderEngine.RGBA(
									(tNat8)(X + 1),
									(tNat8)(Y + 1),
									(tNat8)(Z + 1),
									false
								);
							}
						}
					}
				}
				var TempSprite = mRenderEngine.CreateSprite(
					mRenderEngine.GetSpriteSize(BlockUnderMouse.GetLength(0), aRenderEnv.M),
					mMath2D.V2(0, 0)
				)
				._Clear()
				._DrawCube(BlockUnderMouse, aRenderEnv.NormalPattern, aRenderEnv.M);
				
				var Offset = mMath3D.V2(P3D * aRenderEnv.M);
				var V2 = P2D - aCanvas.Size / 2 + TempSprite.Size / 2;
				var CInt = V2.IsInRange(mMath2D.V2(0, 0), TempSprite.Size)
					? TempSprite.Color[V2.X, V2.Y]
					: default;
				
				if (CInt != default) {
					var C = mRenderEngine.ToRGB(TempSprite.Color[V2.X, V2.Y]) / 63;
					P3D += mMath3D.V3(C.X - 1, C.Y - 1, C.Z - 1);
				}
			}
			
			var BlackTransparent = mRenderEngine.RGBA(0, 0, 0, true);
			
			var Axis = new mRenderEngine.tColor[Size, Size, Size];
			for (var I = 0; I < Size; I += 1) {
				if (P3D.X.IsInRange(0, Size-1) &&
					P3D.Y.IsInRange(0, Size-1) &&
					P3D.Z.IsInRange(0, Size-1)
				) {
					Axis[I, P3D.Y, P3D.Z] = BlackTransparent;
					Axis[P3D.X, I, P3D.Z] = BlackTransparent;
					Axis[P3D.X, P3D.Y, I] = BlackTransparent;
				}
			}
			
			var Sprite = mRenderEngine.CreateSprite(
				mRenderEngine.GetSpriteSize(Axis.GetLength(0), aRenderEnv.M),
				mMath2D.V2(0, 0)
			)
			._Clear()
			._DrawCube(Axis, aRenderEnv.NormalPattern, aRenderEnv.M);
			
			aCanvas
			._DrawSprite(Sprite, mMath3D.V3(0, 0, 0));
		}
	}
	
	static Bitmap
	_DrawSprite(
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
			aSprite._RenderToBuffer(Lock.Scan0, Size, aLightVector);
		} finally {
			aImage.UnlockBits(Lock);
		}
		return aImage;
	}
}
