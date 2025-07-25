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
	private static (int Zoom, int Upscale)[] cZoomScaleLevels = {
		(1,  1), //  1
		(2,  1), //  2
		(1,  3), //  3
		(4,  1), //  4
		(2,  3), //  6
		(1,  9), //  9
		(4,  3), // 12
		(2,  9), // 18
		(1, 27), // 27
		(4,  9), // 36
	};
	
	public static ref tEditorState
	Update(
		ref tEditorState aEditorState,
		tInt64 aElapsedMilliSeconds,
		ImmutableStack<iEvent> aEvents
	) {
		using var Log = mPerfLog.LogPerf();
		
		foreach (var Event in aEvents) {
			aEditorState.KeysUpdated = default;
			
			switch (Event) {
				case tKeyDown(var Key): {
					aEditorState.Keys |= Key;
					aEditorState.KeysUpdated |= Key;
					break;
				}
				case tKeyUp(var Key): {
					aEditorState.Keys &= ~Key;
					aEditorState.KeysUpdated |= Key;
					break;
				}
				case tMouseMove(var Old, var New): {
					aEditorState.MousePos = New;
					aEditorState.Pos3D = aEditorState.RenderEnv.To3D(aEditorState.Canvas, New);
					break;
				}
				case tMouseScroll(var Count): {
					//aEditorState.Zoom = (aEditorState.Zoom + Count).Clamp(1, 6);
					
					var Zoom = aEditorState.Zoom;
					var Upscale = aEditorState.RenderEnv.PatternScale;
					var Index = cZoomScaleLevels.Select(
						(_, I) => (I, _.Zoom, _.Upscale)
					).First(
						_ => _.Zoom == Zoom && _.Upscale == Upscale
					).I;
					
					Index = (Index + Count).Clamp(0, cZoomScaleLevels.Length - 1);
					aEditorState.Zoom = cZoomScaleLevels[Index].Zoom;
					
					aEditorState.RenderEnv.PatternFileWatcher.SetFile(
						new System.IO.FileInfo("./Patterns4_6x9.txt")
					);
					aEditorState.RenderEnv._SetScale(cZoomScaleLevels[Index].Upscale);
					break;
				}
				default: {
					break;
				}
			}
			
			if (aEditorState.Keys.HasFlag(tKeys.MouseLeft) && aEditorState.KeysUpdated.HasFlag(tKeys.MouseLeft)) {
				var Size = aEditorState.TargetBlock.GetSize();
				var Index3D = aEditorState.Pos3D + (Size >> 1);
				var P2D = aEditorState.MousePos;
				var N = aEditorState.Canvas.Normal[P2D.X, P2D.Y];
				switch (N) {
					case (0, 0): {
						if (aEditorState.RenderEnv.Angle > 0) {
							Index3D -= V3(0, 0, 1);
						} else {
							Index3D += V3(0, 0, 1);
						}
						break;
					}
					case (var X, 0): {
						Index3D += V3(X.Sign(), 0, 0);
						break;
					}
					case (0, var Y): {
						Index3D -= V3(0, Y.Sign(), 0);
						break;
					}
				}
				
				if (!Index3D.IsInRange(V3(), Size - V3(1))) {
					break;
				}
				var BlockSize = V3(9);
				var BlockCenterIndex = BlockIndexCenter(Index3D, BlockSize);
				
				Assert(BlockCenterIndex.IsInRange(V3(), aEditorState.RenderEnv.PatternScale * Size - V3(1)));
				
				var BlockCenter3D = BlockCenterIndex - (Size >> 1);
				
				aEditorState.TargetBlock[Index3D.X, Index3D.Y, Index3D.Z] = aEditorState.SelectedColor;
				aEditorState.Map = aEditorState.Map.Where(_ => _.Pos != BlockCenter3D).ToImmutableList();
				aEditorState.Map = aEditorState.Map.Add(
					(
						BlockCenter3D,
						CreateBlock(
							V3(),
							aEditorState.TargetBlock
								.SliceByMinSize(mBlockModifier.BlockIndexBegin(Index3D, BlockSize), BlockSize)
						)
					)
				);
			}
			
			// Rechteck-Lösch-Logik mit aufziehbarem Rechteck (Control nur beim Loslassen relevant)
			if (aEditorState.Keys.HasFlag(tKeys.MouseRight) && aEditorState.KeysUpdated.HasFlag(tKeys.MouseRight)) {
				// Startpunkt und Achse merken (unabhängig von Control)
				aEditorState.StartPos3D = aEditorState.Pos3D;
				var N = aEditorState.Canvas.Normal[aEditorState.MousePos.X, aEditorState.MousePos.Y];
				switch (N) {
					case (0, 0): aEditorState.SelectedAxis = mVoxelEditor.tAxis.Z; break;
					case (var X, 0): aEditorState.SelectedAxis = mVoxelEditor.tAxis.X; break;
					case (0, var Y): aEditorState.SelectedAxis = mVoxelEditor.tAxis.Y; break;
					default: aEditorState.SelectedAxis = mVoxelEditor.tAxis.None; break;
				}
			}
			
			if (aEditorState.KeysUpdated.HasFlag(tKeys.MouseRight) && !aEditorState.Keys.HasFlag(tKeys.MouseRight)) {
				if (aEditorState.Keys.HasFlag(tKeys.Control) && aEditorState.SelectedAxis != mVoxelEditor.tAxis.None) {
					var Size = aEditorState.TargetBlock.GetSize();
					var SizeHalf = Size >> 1;
					var Start3D = aEditorState.StartPos3D;
					var Mouse2D = aEditorState.MousePos;
					var End3D = Start3D;
					
					if (aEditorState.SelectedAxis == mVoxelEditor.tAxis.Z) {
						if (aEditorState.RenderEnv.TryGet3DOnZ(aEditorState.Canvas, Mouse2D, Start3D.Z, out var hit)) {
							End3D = hit;
						}
					} else if (aEditorState.SelectedAxis == mVoxelEditor.tAxis.X) {
						if (aEditorState.RenderEnv.TryGet3DOnX(aEditorState.Canvas, Mouse2D, Start3D.X, out var hit)) {
							End3D = hit;
						}
					} else if (aEditorState.SelectedAxis == mVoxelEditor.tAxis.Y) {
						if (aEditorState.RenderEnv.TryGet3DOnY(aEditorState.Canvas, Mouse2D, Start3D.Y, out var hit)) {
							End3D = hit;
						}
					}
					
					var MinX = mMath.Min(Start3D.X, End3D.X) + SizeHalf.X;
					var MaxX = mMath.Max(Start3D.X, End3D.X) + SizeHalf.X;
					var MinY = mMath.Min(Start3D.Y, End3D.Y) + SizeHalf.Y;
					var MaxY = mMath.Max(Start3D.Y, End3D.Y) + SizeHalf.Y;
					var MinZ = mMath.Min(Start3D.Z, End3D.Z) + SizeHalf.Z;
					var MaxZ = mMath.Max(Start3D.Z, End3D.Z) + SizeHalf.Z;
					// Rechteck auf der gewählten Ebene löschen
					if (aEditorState.SelectedAxis is mVoxelEditor.tAxis.X) {
						for (var Y = MinY; Y <= MaxY; Y += 1) {
							for (var Z = MinZ; Z <= MaxZ; Z += 1) {
								var pos = V3(Start3D.X + SizeHalf.X, Y, Z);
								if (pos.IsInRange(V3(), Size - V3(1))) {
									aEditorState.TargetBlock[pos.X, pos.Y, pos.Z] = default;
								}
							}
						}
					} else if (aEditorState.SelectedAxis is mVoxelEditor.tAxis.Y) {
						for (var X = MinX; X <= MaxX; X += 1) {
							for (var Z = MinZ; Z <= MaxZ; Z += 1) {
								var pos = V3(X, Start3D.Y + SizeHalf.Y, Z);
								if (pos.IsInRange(V3(), Size - V3(1))) {
									aEditorState.TargetBlock[pos.X, pos.Y, pos.Z] = default;
								}
							}
						}
					} else if (aEditorState.SelectedAxis is mVoxelEditor.tAxis.Z) {
						for (var X = MinX; X <= MaxX; X += 1) {
							for (var Y = MinY; Y <= MaxY; Y += 1) {
								var pos = V3(X, Y, Start3D.Z + SizeHalf.Z);
								if (pos.IsInRange(V3(), Size - V3(1))) {
									aEditorState.TargetBlock[pos.X, pos.Y, pos.Z] = default;
								}
							}
						}
					}
				} else {
					var Size = aEditorState.TargetBlock.GetSize();
					var Index3D = aEditorState.Pos3D + (Size >> 1);
					
					System.Console.WriteLine();
					System.Console.WriteLine(aEditorState.Pos3D);
					System.Console.WriteLine(Index3D);
					System.Console.WriteLine(Size);
					
					if (!Index3D.IsInRange(V3(), Size - V3(1))) {
						break;
					}
					
					var BlockSize = V3(9);
					var BlockCenterIndex = BlockIndexCenter(Index3D, BlockSize);
					Assert(BlockCenterIndex.IsInRange(V3(), aEditorState.RenderEnv.PatternScale * Size - V3(1)));
					
					var BlockCenter3D = BlockCenterIndex - (Size >> 1);
					System.Console.WriteLine(BlockCenter3D);
					
					aEditorState.TargetBlock[Index3D.X, Index3D.Y, Index3D.Z] = default;
					aEditorState.Map = aEditorState.Map.Where(_ => _.Pos != BlockCenter3D).ToImmutableList();
					aEditorState.Map = aEditorState.Map.Add(
						(
							BlockCenter3D,
							CreateBlock(
								V3(),
								aEditorState.TargetBlock
									.SliceByMinSize(mBlockModifier.BlockIndexBegin(Index3D, BlockSize), BlockSize)
							)
						)
					);
				}
				// Reset
				aEditorState.SelectedAxis = mVoxelEditor.tAxis.None;
			}
		}
		
		return ref aEditorState;
	}
	
	public static ref tEditorState
	Render(
		ref tEditorState aEditorState,
		tInt64 aElapsedMilliSeconds
	) {
		using var Log = mPerfLog.LogPerf();
		
		var RenderEnv = aEditorState.RenderEnv;
		var Canvas = aEditorState.Canvas;
		
		if (RenderEnv.HotReload.HasNewDLL) {
			RenderEnv.HotReload._LoadDLL();
		}
		
		var Shadow = CreateShadow(
			GetShadowSize(
				3 * aEditorState.TargetBlock.GetSize(),
				RenderEnv.LightDirection
			),
			V2()
		)._Clear(
		);
		
		#if !true
		
		{ // blocks
			Canvas._Clear();
			foreach (var (Pos, Block) in aEditorState.Map) {
				RenderEnv._DrawTo(Canvas, Shadow, Block, Pos);
			}
		}
		
		#else
		
		{ // targetBlock
			Canvas._Clear();
			var Block = CreateBlock(V3(), aEditorState.TargetBlock);
			RenderEnv._DrawTo(Canvas, Shadow, Block, V3());
		}
		
		#endif
		
		var P2D = aEditorState.MousePos;
		var P3D = V3();
		
		if (aEditorState.Keys.HasFlag(tKeys.MouseRight) && aEditorState.Keys.HasFlag(tKeys.Control)) {
			// Remove Area Marker
			
			var Size = aEditorState.TargetBlock.GetSize();
			var SizeHalf = Size >> 1;
			var Start3D = aEditorState.StartPos3D;
			var Mouse2D = aEditorState.MousePos;
			var End3D = Start3D;
			
			if (aEditorState.SelectedAxis == mVoxelEditor.tAxis.Z) {
				if (aEditorState.RenderEnv.TryGet3DOnZ(aEditorState.Canvas, Mouse2D, Start3D.Z, out var hit)) {
					End3D = hit;
				}
			} else if (aEditorState.SelectedAxis == mVoxelEditor.tAxis.X) {
				if (aEditorState.RenderEnv.TryGet3DOnX(aEditorState.Canvas, Mouse2D, Start3D.X, out var hit)) {
					End3D = hit;
				}
			} else if (aEditorState.SelectedAxis == mVoxelEditor.tAxis.Y) {
				if (aEditorState.RenderEnv.TryGet3DOnY(aEditorState.Canvas, Mouse2D, Start3D.Y, out var hit)) {
					End3D = hit;
				}
			}
			
			var MinX = mMath.Min(Start3D.X, End3D.X) + SizeHalf.X;
			var MaxX = mMath.Max(Start3D.X, End3D.X) + SizeHalf.X;
			var MinY = mMath.Min(Start3D.Y, End3D.Y) + SizeHalf.Y;
			var MaxY = mMath.Max(Start3D.Y, End3D.Y) + SizeHalf.Y;
			var MinZ = mMath.Min(Start3D.Z, End3D.Z) + SizeHalf.Z;
			var MaxZ = mMath.Max(Start3D.Z, End3D.Z) + SizeHalf.Z;
			
			var Map_ = System.Collections.Immutable.ImmutableList.Create<(tV3, tBlock)>();
			
			if (aEditorState.SelectedAxis is mVoxelEditor.tAxis.X) {
				for (var Y = MinY; Y <= MaxY; Y += 1) {
					for (var Z = MinZ; Z <= MaxZ; Z += 1) {
						var Pos = V3(Start3D.X + SizeHalf.X, Y, Z);
						if (Pos.IsInRange(V3(), Size - V3(1))) {
							Map_ = Map_.Add((Pos - SizeHalf, OneTransparentBlock));
						}
					}
				}
			} else if (aEditorState.SelectedAxis is mVoxelEditor.tAxis.Y) {
				for (var X = MinX; X <= MaxX; X += 1) {
					for (var Z = MinZ; Z <= MaxZ; Z += 1) {
						var Pos = V3(X, Start3D.Y + SizeHalf.Y, Z);
						if (Pos.IsInRange(V3(), Size - V3(1))) {
							Map_ = Map_.Add((Pos - SizeHalf, OneTransparentBlock));
						}
					}
				}
			} else if (aEditorState.SelectedAxis is mVoxelEditor.tAxis.Z) {
				for (var X = MinX; X <= MaxX; X += 1) {
					for (var Y = MinY; Y <= MaxY; Y += 1) {
						var Pos = V3(X, Y, Start3D.Z + SizeHalf.Z);
						if (Pos.IsInRange(V3(), Size - V3(1))) {
							Map_ = Map_.Add((Pos - SizeHalf, OneTransparentBlock));
						}
					}
				}
			}
			
			foreach (var (Pos, Block) in Map_) {
				RenderEnv._DrawTo(Canvas, Shadow, Block, Pos);
			}
		} else { // Axis Marker
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
				}
			}
			
			var Map_ = System.Collections.Immutable.ImmutableList.Create<(tV3, tBlock)>();
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
		
		//{ // new cube
		//	var N = aEditorState.Canvas.Normal[P2D.X, P2D.Y];
		//	var Normal = GetNormal3D(N);
		//	
		//	//var NewCube = System.Collections.Immutable.ImmutableList.Create<(tV3,tBlock)>();
		//	RenderEnv._DrawTo(Canvas, Shadow, OneBlock, P3D + Normal.Sign());
		//}
		
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
