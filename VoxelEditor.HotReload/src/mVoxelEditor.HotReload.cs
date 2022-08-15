using System.Collections.Immutable;
using System.Linq;

using static m2DArray;
using static m3DArray;
using static mStd;
using static mMath;
using static mV2;
using static mV3;
using static mVoxelEditor;
using static mVoxelRenderer;
using static mEvents;
using static mBlockModifier;

public static class
mVoxelEditor_HotReload {
	
	public static ref tEditorState
	Update(
		ref tEditorState aEditorState,
		tInt64 aElapsedMilliSeconds,
		ImmutableStack<iEvent> aEvents
	) {
		foreach (var Event in aEvents) {
			switch (Event) {
				case tMouseKeyDown(var Key): {
					aEditorState.MouseKeys |= Key;
					break;
				}
				case tMouseKeyUp(var Key): {
					aEditorState.MouseKeys &= ~Key;
					break;
				}
				case tMouseMove(var Old, var New): {
					aEditorState.MousePos = New;
					aEditorState.Pos3D = aEditorState.RenderEnv.To3D(aEditorState.Canvas, New);
					
					var Scale = 3;
					var Size = aEditorState.TargetBlock.GetSize();
					var Index3D = (aEditorState.Pos3D + ((Scale * Size) >> 1)) / Scale;
					
					if (!Index3D.IsInRange(V3(), Size - V3(1))) {
						break;
					}
					
					var BlockSize = V3(Scale);
					var BlockCenterIndex = BlockIndexCenter(Index3D, BlockSize);
					var BlockCenter3D = (BlockCenterIndex - (Size >> 1)) * Scale;
					Assert(BlockCenterIndex.IsInRange(V3(), Scale * Size - V3(1)));
					
					#if !true
						var Old_ = aEditorState.Map.First(_ => _.Pos == BlockPos).Block;
						var New_ = CreateBlock(
							V3(),
							aEditorState.TargetBlock
								.SliceByMinSize(BlockPos / Scale + (Size >> 1) - V3(1), BlockSize)
								.Scale3()
						);
						
						if (Old_.Id != New_.Id) {
							1.ToString();
						}
					#endif
					
					aEditorState.TargetBlock[Index3D.X, Index3D.Y, Index3D.Z] = default;
					aEditorState.Map = aEditorState.Map.Where(_ => _.Pos != BlockCenter3D).ToImmutableList();
					aEditorState.Map = aEditorState.Map.Add(
						(
							BlockCenter3D,
							CreateBlock(
								V3(),
								aEditorState.TargetBlock
									.SliceByMinSize(mBlockModifier.BlockIndexBegin(BlockCenterIndex, BlockSize), BlockSize)
									.Scale3()
							)
						)
					);
					break;
				}
				default: {
					break;
				}
			}
		}
		
		return ref aEditorState;
	}
	
	public static ref tEditorState
	Render(
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
				3 * aEditorState.TargetBlock.GetSize(),
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
		
		{ // targetBlock
			//var Block = CreateBlock(V3(), aEditorState.TargetBlock.Scale3());
			//RenderEnv._DrawTo(Canvas, Shadow, Block, V3());
		}
		
		var P2D = aEditorState.MousePos;
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
			if (P3D.IsInRange(V3(-3 * (27 / 2)), V3(3 * (27 / 2)))) {
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
			//RenderEnv._DrawTo(Canvas, Shadow, OneBlock, P3D + Normal.Sign());
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
		Render = Render,
		Update = Update,
	};
	
}
