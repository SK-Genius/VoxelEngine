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
		tInt64 aElapsedMilliSeconds,
		tV2 aMousePos
	) {
		var RenderEnv = aEditorState.RenderEnv;
		var Canvas = aEditorState.Canvas;
		
		if (RenderEnv.HotReloat.HasNewDLL) {
			RenderEnv.HotReloat._LoadDLL();	
		}
		
		var Shadow = CreateShadow(
			GetShadowSize(
				V3(5 * aEditorState.StdBlockSize),
				RenderEnv.LightDirection
			),
			V2()
		)._Clear(
		);
		
		var P2D = aMousePos;
		var P3D = V3(0);
		if (P2D.IsInRange(V2(), aEditorState.Canvas.Size - V2(1))) {
			P3D = RenderEnv.To3D(aEditorState.Canvas, P2D) + V3(4);
		}
		
		var Map_ = aEditorState.Map;
		if (P3D.IsInRange(V3(-2 * 9 - 4), V3(2 * 9 + 4))) {
			for (var I = -2; I < 2; I += 1) {
				Map_ = Map_.Add((V3(9 * I, P3D.Y, P3D.Z), XAxis));
				Map_ = Map_.Add((V3(P3D.X, 9 * I, P3D.Z), YAxis));
				Map_ = Map_.Add((V3(P3D.X, P3D.Y, 9 * I), ZAxis));
			}
		}
		
		{ // blocks
			Canvas._Clear();
			foreach (var (Pos, Block) in Map_) {
				RenderEnv._DrawTo(Canvas, Shadow, Block, Pos);
			}
		}
		
		{ // Mouse
			var Color = RGB(c100p, c000p, c100p);
			if (aMousePos.IsInRange(V2(5), Canvas.Size - V2(5 + 1))) {
				for (var I = -5; I <= 5; I += 1) {
					Canvas.Color[aMousePos.X + I, aMousePos.Y] = Color;
					Canvas.Color[aMousePos.X, aMousePos.Y + I] = Color;
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
