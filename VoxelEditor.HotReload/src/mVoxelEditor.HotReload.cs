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
		
		var Size = aEditorState.TargetBlock.GetSize();
		var SizeHalf = Size >> 1;
		
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
			
			if (aEditorState.KeysUpdated.HasFlag(tKeys.MouseLeft)) {
				if (aEditorState.Keys.HasFlag(tKeys.MouseLeft)) {
					var Index3D = aEditorState.Pos3D + SizeHalf;
					aEditorState.StartPos3D = aEditorState.Pos3D;
					var P2D = aEditorState.MousePos;
					var N = aEditorState.Canvas.Normal[P2D.X, P2D.Y];
					switch (N) {
						case (0, 0): {
							aEditorState.SelectedAxis = tAxis.Z;
							if (aEditorState.RenderEnv.Angle > 0) {
								aEditorState.StartPos3D -= V3(0, 0, 1);
							} else {
								aEditorState.StartPos3D += V3(0, 0, 1);
							}
							break;
						}
						case (var X, 0): {
							aEditorState.SelectedAxis = tAxis.X;
							aEditorState.StartPos3D += V3(X.Sign(), 0, 0);
							break;
						}
						case (0, var Y): {
							aEditorState.SelectedAxis = tAxis.Y;
							aEditorState.StartPos3D -= V3(0, Y.Sign(), 0);
							break;
						}
					}
				} else {
					var Mouse2D = aEditorState.MousePos - (aEditorState.Canvas.Size >> 1);
					var Start3D = aEditorState.StartPos3D;
					var End3D = aEditorState.RenderEnv.Camera.TryGet3DPos(aEditorState.SelectedAxis, Mouse2D, Start3D, out var Result)
					? Result
					: Start3D;
					
					if (aEditorState.Keys.HasFlag(tKeys.Shift)) {
						var D = End3D - Start3D;
						var D_ = aEditorState.SelectedAxis switch {
							tAxis.X => D.Sign() * V3(D.Abs().YZ().Min()),
							tAxis.Y => D.Sign() * V3(D.Abs().XZ().Min()),
							tAxis.Z => D.Sign() * V3(D.Abs().XY().Min()),
						};
						
						End3D = Start3D + D_;
					}
					if (aEditorState.Keys.HasFlag(tKeys.Control)) {
						Start3D -= End3D - Start3D;
					}
					
					var Min = mV3.Min(Start3D, End3D) + SizeHalf;
					var Max = mV3.Max(Start3D, End3D) + SizeHalf;
					
					var C = (Max + Min) >> 1;
					var R = mV3.Max( (Max - Min) >> 1, V3(1));
					
					for (var X = Min.X; X <= Max.X; X += 1) {
						for (var Y = Min.Y; Y <= Max.Y; Y += 1) {
							for (var Z = Min.Z; Z <= Max.Z; Z += 1) {
								var Pos = V3(X, Y, Z);
								if (Pos.IsInRange(V3(), Size - V3(1))) {
									if (!aEditorState.Keys.HasFlag(tKeys.Alt) || (100 * (Pos - C).Abs() / R).Length2() <= 100 * 100) {
										aEditorState.TargetBlock[Pos.X, Pos.Y, Pos.Z] = aEditorState.SelectedColor;
									}
								}
							}
						}
					}
					
					// Reset
					aEditorState.SelectedAxis = tAxis._;
				}
			}
			
			// Rechteck-Lösch-Logik mit aufziehbarem Rechteck (Control nur beim Loslassen relevant)
			if (aEditorState.KeysUpdated.HasFlag(tKeys.MouseRight)) {
				if (aEditorState.Keys.HasFlag(tKeys.MouseRight)) {
					aEditorState.StartPos3D = aEditorState.Pos3D;
					var N = aEditorState.Canvas.Normal[aEditorState.MousePos.X, aEditorState.MousePos.Y];
					aEditorState.SelectedAxis = N switch {
						(0, 0) => tAxis.Z,
						(var X, 0) => tAxis.X,
						(0, var Y) => tAxis.Y,
						_ => tAxis._,
					};
				} else {
					var Mouse2D = aEditorState.MousePos - (aEditorState.Canvas.Size >> 1);
					var Start3D = aEditorState.StartPos3D;
					var End3D = aEditorState.RenderEnv.Camera.TryGet3DPos(aEditorState.SelectedAxis, Mouse2D, Start3D, out var Result)
					? Result
					: Start3D;
					
					if (aEditorState.Keys.HasFlag(tKeys.Shift)) {
						var D = End3D - Start3D;
						var D_ = aEditorState.SelectedAxis switch {
							tAxis.X => D.Sign() * V3(D.Abs().YZ().Min()),
							tAxis.Y => D.Sign() * V3(D.Abs().XZ().Min()),
							tAxis.Z => D.Sign() * V3(D.Abs().XY().Min()),
						};
						
						End3D = Start3D + D_;
					}
					if (aEditorState.Keys.HasFlag(tKeys.Control)) {
						Start3D -= End3D - Start3D;
					}
					
					var Min = mV3.Min(Start3D, End3D) + SizeHalf;
					var Max = mV3.Max(Start3D, End3D) + SizeHalf;
					
					var C = (Max + Min) >> 1;
					var R = mV3.Max( (Max - Min) >> 1, V3(1));
					
					for (var X = Min.X; X <= Max.X; X += 1) {
						for (var Y = Min.Y; Y <= Max.Y; Y += 1) {
							for (var Z = Min.Z; Z <= Max.Z; Z += 1) {
								var Pos = V3(X, Y, Z);
								if (Pos.IsInRange(V3(), Size - V3(1))) {
									if (!aEditorState.Keys.HasFlag(tKeys.Alt) || (100 * (Pos - C).Abs() / R).Length2() <= 100 * 100) {
										aEditorState.TargetBlock[Pos.X, Pos.Y, Pos.Z] = default;
									}
								}
							}
						}
					}
					
					// Reset
					aEditorState.SelectedAxis = tAxis._;
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
		
		var Size = aEditorState.TargetBlock.GetSize();
		var SizeHalf = Size >> 1;
		
		if (aEditorState.Keys.HasFlag(tKeys.MouseLeft) || aEditorState.Keys.HasFlag(tKeys.MouseRight)) {// && aEditorState.Keys.HasFlag(tKeys.Control)) {
			// Remove Area Marker
			
			var Mouse2D = aEditorState.MousePos - (aEditorState.Canvas.Size >> 1);
			var Start3D = aEditorState.StartPos3D;
			var End3D = aEditorState.RenderEnv.Camera.TryGet3DPos(aEditorState.SelectedAxis, Mouse2D, Start3D, out var Result)
			? Result
			: Start3D;
			
			if (aEditorState.Keys.HasFlag(tKeys.Shift)) {
				var D = End3D - Start3D;
				var D_ = aEditorState.SelectedAxis switch {
					tAxis.X => D.Sign() * V3(D.Abs().YZ().Min()),
					tAxis.Y => D.Sign() * V3(D.Abs().XZ().Min()),
					tAxis.Z => D.Sign() * V3(D.Abs().XY().Min()),
				};
				
				End3D = Start3D + D_;
			}
			if (aEditorState.Keys.HasFlag(tKeys.Control)) {
				Start3D -= End3D - Start3D;
			}
			
			var Min = mV3.Min(Start3D, End3D) + SizeHalf;
			var Max = mV3.Max(Start3D, End3D) + SizeHalf;
			
			var Map_ = System.Collections.Immutable.ImmutableList.Create<(tV3, tBlock)>();
			
			var C = (Max + Min) >> 1;
			var R = mV3.Max( (Max - Min) >> 1, V3(1));
			
			for (var X = Min.X; X <= Max.X; X += 1) {
				for (var Y = Min.Y; Y <= Max.Y; Y += 1) {
					for (var Z = Min.Z; Z <= Max.Z; Z += 1) {
						var Pos = V3(X, Y, Z);
						if (Pos.IsInRange(V3(), Size - V3(1))) {
							if (!aEditorState.Keys.HasFlag(tKeys.Alt) || (100 * (Pos - C).Abs() / R).Length2() <= 100 * 100) {
								Map_ = Map_.Add((Pos - SizeHalf, OneTransparentBlock));
							}
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
