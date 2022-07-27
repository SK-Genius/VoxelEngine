using static mHotReload;
using static mStd;
using static mMath;
using static mMath2D;
using static mMath3D;
using static mVoxelRenderer;

using System.Collections.Immutable;
using System.IO;

public static class
mVoxelEditor {
	
	public static readonly tNat8 c000p = 0;
	public static readonly tNat8 c025p = 1;
	public static readonly tNat8 c050p = 2;
	public static readonly tNat8 c075p = 3;
	public static readonly tNat8 c100p = 4;
	
	public static tColor R = RGB(c100p, c000p, c000p);
	public static tColor G = RGB(c000p, c100p, c000p);
	public static tColor B = RGB(c000p, c000p, c100p);
	public static tColor Y = RGB(c050p, c050p, c000p);
	public static tColor C = RGB(c000p, c050p, c050p);
	public static tColor V = RGB(c050p, c000p, c050p);
	public static tColor W = RGB(c100p, c100p, c100p);
	public static tColor T = RGBA(c100p, c100p, c100p, true);
	public static tColor H = RGB(c050p, c050p, c050p);
	public static tColor _ = default;
	
	public static tBlock EmptyBlock = CreateBlock(V3(), new tColor[0, 0, 0]);
	
	public static tBlock OneBlock = CreateBlock(
		V3(),
		new tColor[1, 1, 1] {
			{
				{
					R
				}
			}
		}
	);
	
	public static tBlock XAxis = CreateBlock(
		V3(),
		new tColor[9, 1, 1] {
			{
				{ T },
			},
			{
				{ T },
			},
			{
				{ T },
			},
			{
				{ T },
			},
			{
				{ T },
			},
			{
				{ T },
			},
			{
				{ T },
			},
			{
				{ T },
			},
			{
				{ T },
			},
		}
	);
	
	public static tBlock YAxis = CreateBlock(
		V3(),
		new tColor[1, 9, 1] {
			{
				{ T },
				{ T },
				{ T },
				{ T },
				{ T },
				{ T },
				{ T },
				{ T },
				{ T },
			},
		}
	);
	
	public static tBlock ZAxis = CreateBlock(
		V3(),
		new tColor[1, 1, 9] {
			{
				{ T, T, T, T, T, T, T, T, T },
			},
		}
	);
	
	public class tEditorState {
		public tInt32 Zoom;
		public tInt32 StdBlockSize;
		public tDebugRenderMode DebugRenderMode;
		public tRenderEnv RenderEnv;
		public tSprite Canvas;
		public ImmutableList<(tV3, tBlock)> Map;
		public tShadow Shadow;
		public tV2 MousePosOld;
		public tV2 MousePosNew;
		
		public tHotReload<tEditorDLL> HotReload = new (
			new DirectoryInfo(".."),
			"VoxelEditor.HotReload.dll"
		);
	}
	
	public static tEditorState Create (
		tV2 aCanvasSize,
		tInt32 aZoom
	){
		#if true
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
		#else
		var Dice = CreateBlock(
			V3(),
			new tColor[9, 9, 9] {
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
				{
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
					{ H, H, H, H, H, H, H, H, H },
				},
			}
		) ;
		#endif
		
		var Plane = CreateBlock(
			V3(),
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
		
		var RenderEnv = CreateEnv();
		RenderEnv
		._SetLightDirection(V3(0, 0, 9))
		._Update()
		;
		
		var StdBlockSize = 9;
		
		var Map = ImmutableList
		.Create<(tV3, tBlock)>()
		.Add((StdBlockSize * V3(-2, 2, 0), Dice))
		.Add((StdBlockSize * V3(-1, 2, 0), Dice))
		.Add((StdBlockSize * V3(0, 2, 0), Dice))
		.Add((StdBlockSize * V3(1, 2, 0), Dice))
		.Add((StdBlockSize * V3(2, 2, 0), Dice))
		.Add((StdBlockSize * V3(2, -1, 0), Dice))
		.Add((StdBlockSize * V3(2, 0, 0), Dice))
		.Add((StdBlockSize * V3(2, 1, 0), Dice))
		.Add((StdBlockSize * V3(-2, -1, 0), Dice))
		.Add((StdBlockSize * V3(-2, 0, 0), Dice))
		.Add((StdBlockSize * V3(-2, 1, 0), Dice))
		.Add((StdBlockSize * V3(-2, -2, 0), Dice))
		.Add((StdBlockSize * V3(-1, -2, 0), Dice))
		.Add((StdBlockSize * V3(0, -2, 0), Dice))
		.Add((StdBlockSize * V3(1, -2, 0), Dice))
		.Add((StdBlockSize * V3(2, -2, 0), Dice))
		.Add((StdBlockSize * V3(0, 0, 0), Dice))
		.Add((StdBlockSize * V3(0, 0, 1), Dice))
		;
		
		for (var Y_ = -2; Y_ <= 2; Y_ += 1) {
			for (var X_ = -2; X_ <= 2; X_ += 1) {
				Map = Map.Add((StdBlockSize * V3(X_, Y_, 2), Plane));
			}
		}
		
		return new tEditorState {
			RenderEnv = RenderEnv,
			Zoom = aZoom,
			Canvas = CreateSprite(
				V2(aCanvasSize.X, aCanvasSize.Y) / aZoom,
				V2()
			),
			StdBlockSize = StdBlockSize,
			Map = Map,
			DebugRenderMode = default,
		};
	}
	
	public static ref tEditorState
	Loop(
		ref tEditorState aEditorState,
		tInt64 aElapsedMilliSeconds
	) {
		if (aEditorState.HotReload.HasNewDLL) {
			aEditorState.HotReload._LoadDLL();
		}
		return ref aEditorState.HotReload.DLL.Loop(ref aEditorState, aElapsedMilliSeconds);
	}
	
	public class tEditorDLL {
		public tMeth<tEditorState, tInt64> Loop;
	}
}
