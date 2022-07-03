using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using static mMath;
using static mMath2D;
using static mMath3D;
using static mVoxelRenderer;

static class
mProgram {
	
	[STAThread]
	static void
	Main(
	) {
		var Zoom = 1;
		var StdBlockSize = 9;
		var DebugRenderMode = default(tDebugRenderMode);
		
		var R = RGB(4, 0, 0);
		var G = RGB(0, 4, 0);
		var B = RGB(0, 0, 4);
		var Y = RGB(4, 4, 0);
		var C = RGB(0, 4, 4);
		var V = RGB(4, 0, 4);
		var W = RGB(4, 4, 4);
		var H = RGB(2, 2, 2);
		var _ = default(tColor);
		
		var Dice = CreateBlock(
			V3(0),
			new tColor[9, 9, 9] {
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
			}
		);
		
		var Plane = CreateBlock(
			V3(0),
			new tColor[9, 9, 9] {
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
			}
		);
		
		var MousePos = V2(0, 0);
		
		var RenderEnv = CreateEnv();
		RenderEnv
		._SetLightDirection(V3(0, 0, 9))
		._Update();
		
		var Window = new Form{
			Width = 1000,
			Height = 800,
			WindowState = FormWindowState.Maximized,
		};
		
		var Canvas = CreateSprite(
			V2(Window.Width / Zoom, Window.Height / Zoom),
			V2(0, 0)
		);
		
		typeof(Control).GetProperty(
			"DoubleBuffered",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
		)?.SetValue(Window, true, null);
		
		var Map = new Dictionary<tV3, tBlock>();
		Map[StdBlockSize * V3(-2, 2, 0)] = Dice;
		Map[StdBlockSize * V3(-1, 2, 0)] = Dice;
		Map[StdBlockSize * V3(0, 2, 0)] = Dice;
		Map[StdBlockSize * V3(1, 2, 0)] = Dice;
		Map[StdBlockSize * V3(2, 2, 0)] = Dice;
		Map[StdBlockSize * V3(2, -1, 0)] = Dice;
		Map[StdBlockSize * V3(2, 0, 0)] = Dice;
		Map[StdBlockSize * V3(2, 1, 0)] = Dice;
		Map[StdBlockSize * V3(-2, -1, 0)] = Dice;
		Map[StdBlockSize * V3(-2, 0, 0)] = Dice;
		Map[StdBlockSize * V3(-2, 1, 0)] = Dice;
		Map[StdBlockSize * V3(-2, -2, 0)] = Dice;
		Map[StdBlockSize * V3(-1, -2, 0)] = Dice;
		Map[StdBlockSize * V3(0, -2, 0)] = Dice;
		Map[StdBlockSize * V3(1, -2, 0)] = Dice;
		Map[StdBlockSize * V3(2, -2, 0)] = Dice;
		Map[StdBlockSize * V3(0, 0, 0)] = Dice;
		Map[StdBlockSize * V3(0, 0, 1)] = Dice;
		
		for (var Y_ = -2; Y_ <= 2; Y_ += 1) {
			for (var X_ = -2; X_ <= 2; X_ += 1) {
				Map[StdBlockSize * V3(X_, Y_, 2)] = Plane;
			}
		}
		
		Window.Paint += (_, a) => {
			if (RenderEnv.HasNewDLL) {
				RenderEnv._LoadDLL();	
			}
			
			var Shadow = CreateShadow(
				GetShadowSize(
					V3(
						5 * StdBlockSize,
						5 * StdBlockSize,
						5 * StdBlockSize
					),
					RenderEnv.LightDirection
				),
				V2(0, 0)
			)._Clear(
			);
			
			RenderEnv._RenderTo(
				ref Canvas,
				ref Shadow,
				MousePos,
				Map,
				StdBlockSize
			);
			
			var Image = new Bitmap(
				Canvas.Size.X,
				Canvas.Size.Y,
				PixelFormat.Format32bppArgb
			);
			
			try {
				RenderEnv._RenderTo(
					ref Image,
					Canvas,
					Shadow,
					DebugRenderMode
				);
				
				a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				//a.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
				a.Graphics.Clear(System.Drawing.Color.White);
				a.Graphics.DrawImage(Image, 0, 0, Zoom*Image.Width, Zoom*Image.Height);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			} finally {
				Image.Dispose();
			}
		};
		
		Window.MouseMove += (_, a) => {
			var NewMousePos = V2(
				1 + a.X / Zoom,
				1 + a.Y / Zoom
			);
			
			if (a.Button.HasFlag(MouseButtons.Right)) {
				var D = NewMousePos - MousePos;
				RenderEnv.Dir += D.X / 2;
				RenderEnv.Angle += D.Y / 2;
				
				RenderEnv._Update();
				Window.Invalidate();
			}
			
			//if (a.Button.HasFlag(MouseButtons.Left)) {
				RenderEnv._Update();
				Window.Invalidate();
			//}
			
			MousePos = NewMousePos;
		};
		
		Window.Resize += (_, a) => {
			Canvas = mVoxelRenderer.CreateSprite(
				V2(Window.Size.Width, Window.Size.Height),
				V2(0, 0)
			);
			Window.Invalidate();
		};
		
		Window.KeyDown += (_, a) => {
			switch (a.KeyCode) {
				case Keys.F1: {
					DebugRenderMode = tDebugRenderMode.None;
					Window.Invalidate();
					break;
				}
				case Keys.F2: {
					DebugRenderMode = tDebugRenderMode.Deep;
					Window.Invalidate();
					break;
				}
				case Keys.F3: {
					DebugRenderMode = tDebugRenderMode.Normal;
					Window.Invalidate();
					break;
				}
				case Keys.F4: {
					DebugRenderMode = tDebugRenderMode.Pos;
					Window.Invalidate();
					break;
				}
				case Keys.F5: {
					DebugRenderMode = tDebugRenderMode.PosBits;
					Window.Invalidate();
					break;
				}
				case Keys.Space: {
					RenderEnv._LoadDLL();
					Window.Invalidate();
					break;
				}
				case Keys.Escape: {
					System.Environment.Exit(0);
					break;	
				}
				case Keys.L:
				case Keys.Right: {
					var LightDirection = RenderEnv.LightDirection;
					RenderEnv
					._SetLightDirection(LightDirection + (V3(LightDirection.Z, 0, -LightDirection.X) >> 3))
					._Update()
					;
					Window.Invalidate();
					break;
				}
				case Keys.J:
				case Keys.Left: {
					var LightDirection = RenderEnv.LightDirection;
					RenderEnv
					._SetLightDirection(LightDirection - (V3(LightDirection.Z, 0, -LightDirection.X) >> 3))
					._Update()
					;
					Window.Invalidate();
					break;
				}
				case Keys.I:
				case Keys.Up: {
					var LightDirection = RenderEnv.LightDirection;
					RenderEnv
					._SetLightDirection(LightDirection + (V3(0, LightDirection.Z, -LightDirection.Y) >> 3))
					._Update()
					;
					Window.Invalidate();
					break;
				}
				case Keys.K:
				case Keys.Down: {
					var LightDirection = RenderEnv.LightDirection;
					RenderEnv
					._SetLightDirection(LightDirection - (V3(0, LightDirection.Z, -LightDirection.Y) >> 3))
					._Update()
					;
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
	
	static tRenderEnv
	_RenderTo(
		this tRenderEnv aRenderEnv,
		ref tSprite aCanvas,
		ref tShadow aShadow,
		tV2 aMousePos,
		Dictionary<tV3, tBlock> aMap,
		tInt32 aStdBlockSize
	) {
		var Size = aStdBlockSize;
		var HSize = Size / 2;
		
		{ // cubes
			aCanvas._Clear();
			foreach (var (Pos, Block) in aMap) {
				aRenderEnv._DrawTo(aCanvas, aShadow, Block, Pos);
			}
		}
		
		{ // axes lines
			var P2D = aMousePos;
			var P3D = V3(0, 0, 0);
			if (
				P2D.X.IsInRange(0, aCanvas.Size.X - 1) &&
				P2D.Y.IsInRange(0, aCanvas.Size.Y - 1)
			) {
				P3D = aRenderEnv.To3D(aCanvas, P2D) + V3(4, 4, 4);
			}
			
			var BlackTransparent = RGBA(0, 0, 0, true);
			
			var AxisColors = new tColor[Size, Size, Size];
			for (var I = 0; I < Size; I += 1) {
				if (P3D.X.IsInRange(0, Size-1) &&
					P3D.Y.IsInRange(0, Size-1) &&
					P3D.Z.IsInRange(0, Size-1)
				) {
					AxisColors[I, P3D.Y, P3D.Z] = BlackTransparent;
					AxisColors[P3D.X, I, P3D.Z] = BlackTransparent;
					AxisColors[P3D.X, P3D.Y, I] = BlackTransparent;
				}
			}
			
			var Axis = CreateBlock(V3(), AxisColors);
			
			aRenderEnv._DrawTo(aCanvas, aShadow, Axis, V3(0, 0, 0));
		}
		
		{ // Mouse
			var Color = RGB(4, 0, 4);
			if (
				aMousePos.X.IsInRange(5, aCanvas.Size.X - 5) &&
				aMousePos.Y.IsInRange(5, aCanvas.Size.Y - 5)
			) {
				for (var I = -5; I < 5; I += 1)
				{
					aCanvas.Color[aMousePos.X + I, aMousePos.Y] =  Color;
					aCanvas.Color[aMousePos.X, aMousePos.Y + I] = Color;
				}
			}
		}
		
		return aRenderEnv;
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
