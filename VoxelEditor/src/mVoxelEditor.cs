using System.Collections.Immutable;
using System.IO;
using System.Linq;

using static m2DArray;
using static m3DArray;
using static mHotReload;
using static mStd;
using static mMath;
using static mV2;
using static mV3;
using static mEvents;
using static mVoxelRenderer;

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
	
	public static tBlock EmptyBlock = CreateBlock(V3(), V3().CreateArray<tColor>());
	
	public static tBlock OneRedBlock = CreateBlock(
		V3(),
		new tColor[1, 1, 1] {
			{
				{
					R
				}
			}
		}
	);
	
	public static tBlock OneTransparentBlock = CreateBlock(
		V3(),
		new tColor[1, 1, 1] {
			{
				{
					T
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
	
	public enum tAxis {
		None,
		X,
		Y,
		Z
	}
	
	public struct tEditorState {
		public tInt32 Zoom = 1;
		public tDebugRenderMode DebugRenderMode;
		public tRenderEnv RenderEnv;
		public tSprite Canvas;
		public ImmutableList<(tV3 Pos, tBlock Block)> Map;
		public tColor[,,] TargetBlock;
		public tShadow Shadow;
		public tV3 Pos3D;
		public tV2 MousePos;
		public tKeys Keys;
		public tKeys KeysUpdated;
		public tColor SelectedColor = R;
		public tAxis SelectedAxis;
		public tV3 StartPos3D;
		
		public tHotReload<tEditorDLL> HotReload = new (
			new FileInfo("./VoxelEditor.HotReload.dll")
		);
		
		public tEditorState() {
		}
	}
	
	public static tEditorState Create (
		tV2 aCanvasSize,
		tInt32 aZoom
	){
		#if !true
		var Dice = CreateBlock(
			V3(),
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
		
		var BlockSize = 9;
		var Upscale = 3;
		
		var RenderEnv = CreateEnv();
		RenderEnv
		._SetScale(Upscale)
		._SetLightDirection(V3(0, 0, BlockSize))
		._Update()
		;
		
		var TargetSize = V3(BlockSize);
		var TargetBlock = TargetSize.CreateArray<tColor>();
		for (var Z = (tNat8)0; Z < TargetSize.Z; Z += 1) {
			for (var Y = (tNat8)0; Y < TargetSize.Y; Y += 1) {
				for (var X = (tNat8)0; X < TargetSize.X; X += 1) {
					TargetBlock[X, Y, Z] = RGB(
						(tNat8)(X % 5),
						(tNat8)(Y % 5),
						(tNat8)(Z % 5)
					);
				}
			}
		}
		
		var Map = TargetBlock.Split(V3(), V3(BlockSize / Upscale))
		.Select(_ => ((_.Pos - V3(BlockSize >> 1)) * Upscale, CreateBlock(V3(), _.Block.Scale3())))
		.ToImmutableList()
		;
		
		return new tEditorState {
			RenderEnv = RenderEnv,
			Zoom = aZoom,
			Canvas = CreateSprite(
				V2(aCanvasSize.X, aCanvasSize.Y) / aZoom
			),
			TargetBlock = TargetBlock,
			Map = Map,
			DebugRenderMode = default,
		};
	}
	
	public static ref tEditorState
	Update(
		this ref tEditorState aEditorState,
		tInt64 aElapsedMilliSeconds,
		iEvent aEvent
	) {
		try {
			if (aEditorState.HotReload.HasNewDLL) {
				aEditorState.HotReload._LoadDLL();
			}
			return ref aEditorState.HotReload.DLL.Update(
				ref aEditorState,
				aElapsedMilliSeconds,
				ImmutableStack.Create(aEvent)
			);
		} catch (System.Exception e) {
			System.Console.WriteLine(e);
			return ref aEditorState;
		}
	}
	
	public static ref tEditorState
	Render(
		ref tEditorState aEditorState,
		tInt64 aElapsedMilliSeconds
	) {
		try {
			if (aEditorState.HotReload.HasNewDLL) {
				aEditorState.HotReload._LoadDLL();
			}
			return ref aEditorState.HotReload.DLL.Render(
				ref aEditorState,
				aElapsedMilliSeconds
			);
		} catch (System.Exception e) {
			System.Console.WriteLine(e);
			return ref aEditorState;
		}
	}
	
	public class tEditorDLL {
		public tMeth<tEditorState, tInt64, ImmutableStack<iEvent>> Update;
		public tMeth<tEditorState, tInt64> Render;
	}
}
