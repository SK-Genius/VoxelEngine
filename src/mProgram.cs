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
		var Zoom = 1;
		var LoD = 2; // 0..2 // TODO: move into RenderEnv ?
		var LightDirection = (mMath3D.tV3?)mMath3D.V3(0, 0, 120);
		
		var DebugRenderMode = default(mRenderEngine.tDebugRenderMode);
		
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
		var H = mRenderEngine.RGB(2, 2, 2);
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
		
		var Plane = new []{
			new mRenderEngine.tColor[1, 1, 1] {
				{
					{ H },
				},
			},
			new mRenderEngine.tColor[3, 3, 3] {
				{
					{ H, H, H },
					{ H, H, H },
					{ H, H, H },
				},
				{
					{ _, _, _ },
					{ _, _, _ },
					{ _, _, _ },
				},
				{
					{ _, _, _ },
					{ _, _, _ },
					{ _, _, _ },
				}
			},
			new mRenderEngine.tColor[9, 9, 9] {
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
				{
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
					{ _, _, _, _, _, _, _, _, H },
				},
			},
		};
		
		var MousePos = mMath2D.V2(0, 0);
		
		var RenderEnv = mRenderEngine.CreateEnv()
		._SetLightDirection(LightDirection.Value)
		._Update();
		
		var Window = new Form{
			Width = 1000,
			Height = 800,
			WindowState = FormWindowState.Maximized,
		};
		
		var Canvas = mRenderEngine.CreateSprite(
			mMath2D.V2(Window.Width / Zoom, Window.Height / Zoom),
			mMath2D.V2(0, 0)
		);
		
		var Shadow = mRenderEngine.CreateShadow(
			mRenderEngine.GetShadowSize(5 * LoD_Size, RenderEnv.LightDirection),
			mMath2D.V2(0, 0)
		)
		._Clear();
		
		typeof(Control).GetProperty(
			"DoubleBuffered",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
		)?.SetValue(Window, true, null);
		
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
		Map[LoD_Size * mMath3D.V3(0, 0, 1)] = Cube[LoD];
		{
			for (var Y_ = -2; Y_ <= 2; Y_ += 1) {
				for (var X_ = -2; X_ <= 2; X_ += 1) {
					Map[LoD_Size * mMath3D.V3(X_, Y_, 2)] = Plane[LoD];
				}
			}
		}
		
		Window.Paint += (_, a) => {
			RenderToCanvas(
				RenderEnv,
				MousePos,
				Map,
				LoD_Size,
				ref Canvas,
				ref Shadow
			);
			
			var Shadow_ = mRenderEngine.CreateShadow(
				mRenderEngine.GetShadowSize(LoD_Size, RenderEnv.LightDirection),
				mMath2D.V2(0, 0)
			)
			._Clear()
			._DrawCube(Cube[LoD], RenderEnv.LightDirection);
			
			using var Image = new Bitmap(Canvas.Size.X, Canvas.Size.Y, PixelFormat.Format32bppArgb);
			RenderEnv._RenderToImage(Canvas, Shadow, Image, DebugRenderMode); // (left, top, front)
			
			Image
			._DrawShadowDebug(Shadow, mMath2D.V2(0, 50))
			._DrawShadowDebug(Shadow_, mMath2D.V2(0, 0))
			;
			
			a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			//a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
			a.Graphics.Clear(System.Drawing.Color.White);
			a.Graphics.DrawImage(Image, 0, 0, Zoom*Image.Width, Zoom*Image.Height);
		};
		
		Window.MouseMove += (_, a) => {
			var NewMousePos = mMath2D.V2(
				1 + a.X / Zoom,
				1 + a.Y / Zoom
			);
			
			if (a.Button.HasFlag(MouseButtons.Right)) {
				var D = NewMousePos - MousePos;
				RenderEnv.Dir += D.X / 2;
				RenderEnv.Angle += D.Y / 2;
				
				System.Diagnostics.Debug.WriteLine($"DELTA: ({a.X}, {a.Y}) => D: {RenderEnv.Dir}; A: {RenderEnv.Angle}");
				
				RenderEnv._Update();
				Window.Invalidate();
			}
			
			if (a.Button.HasFlag(MouseButtons.Left)) {
				RenderEnv._Update();
				Window.Invalidate();
			}
			
			MousePos = NewMousePos;
		};
		
		Window.KeyDown += (_, a) => {
			switch (a.KeyCode) {
				case Keys.F1: {
					DebugRenderMode = mRenderEngine.tDebugRenderMode.None;
					Window.Invalidate();
					break;
				}
				case Keys.F2: {
					DebugRenderMode = mRenderEngine.tDebugRenderMode.Deep;
					Window.Invalidate();
					break;
				}
				case Keys.F3: {
					DebugRenderMode = mRenderEngine.tDebugRenderMode.Normal;
					Window.Invalidate();
					break;
				}
				case Keys.F4: {
					DebugRenderMode = mRenderEngine.tDebugRenderMode.Pos;
					Window.Invalidate();
					break;
				}
				case Keys.F5: {
					DebugRenderMode = mRenderEngine.tDebugRenderMode.PosBits;
					Window.Invalidate();
					break;
				}
				case Keys.L:
				case Keys.Right: {
					LightDirection = LightDirection + mMath3D.V3(10, 0, 0);
					Shadow = mRenderEngine.CreateShadow(
						mRenderEngine.GetShadowSize(5 * LoD_Size, LightDirection.Value),
						mMath2D.V2(0, 0)
					);
					RenderEnv._SetLightDirection(LightDirection.Value);
					RenderEnv._Update();
					Window.Invalidate();
					break;
				}
				case Keys.J:
				case Keys.Left: {
					LightDirection = LightDirection + mMath3D.V3(-10, 0, 0);
					Shadow = mRenderEngine.CreateShadow(
						mRenderEngine.GetShadowSize(5 * LoD_Size, LightDirection.Value),
						mMath2D.V2(0, 0)
					);
					RenderEnv._SetLightDirection(LightDirection.Value);
					RenderEnv._Update();
					Window.Invalidate();
					break;
				}
				case Keys.I:
				case Keys.Up: {
					LightDirection = LightDirection + mMath3D.V3(0, 10, 0);
					Shadow = mRenderEngine.CreateShadow(
						mRenderEngine.GetShadowSize(5 * LoD_Size, LightDirection.Value),
						mMath2D.V2(0, 0)
					);
					RenderEnv._SetLightDirection(LightDirection.Value);
					RenderEnv._Update();
					Window.Invalidate();
					break;
				}
				case Keys.K:
				case Keys.Down: {
					LightDirection = LightDirection + mMath3D.V3(0, -10, 0);
					Shadow = mRenderEngine.CreateShadow(
						mRenderEngine.GetShadowSize(5 * LoD_Size, LightDirection.Value),
						mMath2D.V2(0, 0)
					);
					RenderEnv._SetLightDirection(LightDirection.Value);
					RenderEnv._Update();
					Window.Invalidate();
					break;
				}
				case Keys.D: {
					RenderEnv.Dir += 1;
					RenderEnv._Update();
					Window.Invalidate();
					break;
				}
				case Keys.A: {
					RenderEnv.Dir -= 1;
					RenderEnv._Update();
					Window.Invalidate();
					break;
				}
				case Keys.W: {
					RenderEnv.Angle += 1;
					RenderEnv._Update();
					Window.Invalidate();
					break;
				}
				case Keys.S: {
					RenderEnv.Angle -= 1;
					RenderEnv._Update();
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
		ref mRenderEngine.tSprite aCanvas,
		ref mRenderEngine.tShadow aShadow
	) {
		var Size = aStdCubeSize;
		var HSize = Size / 2;
		
		{ // cubes
			aCanvas
			._Clear();
			
			aShadow
			._Clear();
			
			foreach (var (Pos, Cube) in aMap) {
				aCanvas
				._DrawSprite(
					aRenderEnv.GetOrCreateSprite(Cube),
					Pos * aRenderEnv.M
				);
				
				var ShadowOffset = default(mMath3D.tV3);
				var (MainAxis, AxisSign) = mRenderEngine.GetMainAxis(aRenderEnv.LightDirection);
				switch (MainAxis) {
					case mRenderEngine.tAxis.X: {
						ShadowOffset = Pos + aRenderEnv.LightDirection * Pos.X / aRenderEnv.LightDirection.X;
						ShadowOffset = AxisSign * mMath3D.V3(
							ShadowOffset.Z,
							ShadowOffset.Y,
							ShadowOffset.X
						);
						break;
					}
					case mRenderEngine.tAxis.Y: {
						ShadowOffset = Pos + aRenderEnv.LightDirection * Pos.Y / aRenderEnv.LightDirection.Y;
						ShadowOffset = AxisSign * mMath3D.V3(
							ShadowOffset.X,
							ShadowOffset.Z,
							ShadowOffset.Y
						);
						break;
					}
					case mRenderEngine.tAxis.Z: {
						ShadowOffset = Pos + aRenderEnv.LightDirection * Pos.Z / aRenderEnv.LightDirection.Z;
						ShadowOffset *= AxisSign;
						break;
					}
				}
				
				aShadow
				._DrawShadow(
					aRenderEnv.GetOrCreateShadow(Cube),
					ShadowOffset
				);
			}
		}
		
		{ // axes lines
			var P2D = aMousePos;
			var P3D = mMath3D.V3(0, 0, 0);
			if (
				P2D.X.IsInRange(0, aCanvas.Size.X - 1) &&
				P2D.Y.IsInRange(0, aCanvas.Size.Y - 1)
			) {
				P3D = aRenderEnv.To3D(aCanvas, P2D) + mMath3D.V3(4, 4, 4);
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
		
		{ // Mouse
			var Color = mRenderEngine.RGB(4, 0, 4);
			for (var I = -5; I < 5; I += 1)
			{
				aCanvas.Color[aMousePos.X + I, aMousePos.Y] =  Color;
				aCanvas.Color[aMousePos.X, aMousePos.Y + I] = Color;
			}
		}
	}
	
	static mRenderEngine.tRenderEnv
	_RenderToImage(
		this mRenderEngine.tRenderEnv aRenderEnv,
		mRenderEngine.tSprite aSprite,
		mRenderEngine.tShadow aShadow,
		Bitmap aImage,
		mRenderEngine.tDebugRenderMode aDebugRenderMode
	) {
		var BufferSize = mMath2D.V2(aImage.Width, aImage.Height);
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
	
	static Bitmap
	_DrawShadowDebug(
		this Bitmap aImage,
		mRenderEngine.tShadow aShadow,
		mMath2D.tV2 aOffset
	) {
		var Size = mMath2D.V2(aImage.Width, aImage.Height);
		var Lock = aImage.LockBits(
			new Rectangle(new Point(0, 0), aImage.Size),
			ImageLockMode.WriteOnly,
			aImage.PixelFormat
		);
		try {
			aShadow._RenderToBuffer(Lock.Scan0, Size, aOffset);
		} finally {
			aImage.UnlockBits(Lock);
		}
		return aImage;
	}
	
}
