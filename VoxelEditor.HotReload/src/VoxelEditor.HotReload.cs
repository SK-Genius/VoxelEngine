using static mStd;
using static mMath;
using static mMath2D;
using static mMath3D;
using static mVoxelEditor;
using static mVoxelRenderer;

public static class
mVoxelEditor_HotReload {
	
	public static ref tEditorState
	Loop(
		ref tEditorState aEditorState,
		tInt64 aElapsedMilliSeconds
	) {
		var RenderEnv = aEditorState.RenderEnv;
		var Canvas = aEditorState.Canvas;
		
		if (RenderEnv.HotReloat.HasNewDLL) {
			RenderEnv.HotReloat._LoadDLL();	
		}
		
		var Shadow = CreateShadow(
			GetShadowSize(
				V3(5 * 9),
				RenderEnv.LightDirection
			),
			V2()
		)._Clear(
		);
		
		{ // blocks
			Canvas._Clear();
			foreach (var (Pos, Block) in aEditorState.Map) {
				RenderEnv._DrawTo(Canvas, Shadow, Block, Pos);
			}
		}
		
		var P2D = aEditorState.MousePosNew;
		var P3D = V3();
		
		{ // 3d Marker
			var X_ = XAxis;
			var Y_ = YAxis;
			var Z_ = ZAxis;
			
			if (P2D.IsInRange(V2(), aEditorState.Canvas.Size - V2(1))) {
				P3D = RenderEnv.To3D(aEditorState.Canvas, P2D);
				
				var N = aEditorState.Canvas.Normal[P2D.X, P2D.Y];
				switch (N) {
					case (0, 0): {
						Z_ = EmptyBlock;
						break;
					}
					case (_, 0): {
						X_ = EmptyBlock;
						break;
					}
					case (0, _): {
						Y_ = EmptyBlock;
						break;
					}
				};
			}
			
			var Map_ = System.Collections.Immutable.ImmutableList.Create<(tV3,tBlock)>();
			if (P3D.IsInRange(V3(-2 * 9 - 4), V3(2 * 9 + 4))) {
				for (var I = -2; I <= 2; I += 1) {
					Map_ = Map_.Add((V3(9 * I, P3D.Y, P3D.Z), X_));
					Map_ = Map_.Add((V3(P3D.X, 9 * I, P3D.Z), Y_));
					Map_ = Map_.Add((V3(P3D.X, P3D.Y, 9 * I), Z_));
				}
			}
			
			foreach (var (Pos, Block) in Map_) {
				RenderEnv._DrawTo(Canvas, Shadow, Block, Pos);
			}
		}
		
		{ // new cube
			var N = aEditorState.Canvas.Normal[P2D.X, P2D.Y];
			var Normal = GetNormal3D(N);
			
			var NewCube = System.Collections.Immutable.ImmutableList.Create<(tV3,tBlock)>();
			RenderEnv._DrawTo(Canvas, Shadow, OneBlock, P3D + Normal.Sign());
		}
		
		{ // Mouse
			var Color = RGB(c100p, c000p, c100p);
			if (P2D.IsInRange(V2(5), Canvas.Size - V2(5 + 1))) {
				for (var I = -5; I <= 5; I += 1) {
					Canvas.Color[P2D.X + I, P2D.Y] = Color;
					Canvas.Color[P2D.X, P2D.Y + I] = Color;
				}
			}
		}
		
		aEditorState.Shadow = Shadow;
		
		return ref aEditorState;
	}
	
	public static tEditorDLL
	Create(
	) => new tEditorDLL {
		Loop = Loop,
	};
	
}
