using System.Linq;

using static m2DArray;
using static m3DArray;
using static mStd;
using static mM3x3;
using static mMath;
using static mSIMD;
using static mV2;
using static mV3;
using static mVoxelRenderer;
using System;
using System.Collections.Generic;

public static class
mVoxelRenderer_HotReload {
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tSprite
	GetOrCreateSprite(
		this Dictionary<(tM3x3, tBlock), tSprite> aSpriteBuffer,
		tCamera aCamera,
		tBlock aBlock
	) {
		using var Log = mPerfLog.LogPerf();
		
		var BlocSize = aBlock.Colors.GetSize();
		
		if (!aSpriteBuffer.TryGetValue((aCamera.M, aBlock), out var Sprite)) {
			var SpriteSize = GetSpriteSize(BlocSize, aCamera.M);
			Sprite = CreateSprite(SpriteSize)
			._Clear()
			._DrawBlock(aBlock, V3(), aCamera);
			
			aSpriteBuffer[(aCamera.M, aBlock)] = Sprite;
		}
		return Sprite;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tShadow
	GetOrCreateShadow(
		this Dictionary<(tV3 LightDirection, tInt32 LayerOffset, tBlock Block), tShadow> aShadowBuffer,
		tV3 aLightDirection,
		tBlock aBlock,
		tV3 aWorldOffset
	) {
		using var Log = mPerfLog.LogPerf();
		
		var LayerOffset = GetMainAxis(aLightDirection).Axis switch {
			tAxis.X => aLightDirection.X == 0 ? 0 : aWorldOffset.X % aLightDirection.X,
			tAxis.Y => aLightDirection.Y == 0 ? 0 : aWorldOffset.Y % aLightDirection.Y,
			tAxis.Z => aLightDirection.Z == 0 ? 0 : aWorldOffset.Z % aLightDirection.Z,
			_ => 0,
		};
		
		var Offset3D = aWorldOffset % aLightDirection.Abs().Max();
		var Offset2D = GetShadowUV(Offset3D, aLightDirection).XY();
		
		if (!aShadowBuffer.TryGetValue((aLightDirection, LayerOffset, aBlock), out var Shadow)) {
			var ShadowSize = GetShadowSize(aBlock.GetSize(), aLightDirection);
			Shadow = CreateShadow(
				ShadowSize,
				Offset2D
			)
			._Clear()
			._DrawBlock(aBlock, V3(), aWorldOffset, aLightDirection);
			
			aShadowBuffer[(aLightDirection, LayerOffset, aBlock)] = Shadow;
		}
		return Shadow;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tAxis[,]
	GetNormalPattern(
		this tAxis[,][,] aNormalPatterns,
		tInt32 aDir,
		tInt32 aAngle
	) {
		using var Log = mPerfLog.LogPerf();
		
		var (AngleParts, QuarterParts) = aNormalPatterns.GetSize();
		var P = aNormalPatterns[aDir % AngleParts, aAngle];
		if (
			aDir.IsInRange(0*QuarterParts, 1*QuarterParts-1) ||
			aDir.IsInRange(2*QuarterParts, 3*QuarterParts-1)
		) {
			return P;
		}
		var Max = P.GetSize();
		var P_ = Max.CreateArray<tAxis>();
		for (var Y = 0; Y < Max.Y; Y += 1) {
			for (var X = 0; X < Max.X; X += 1) {
				var Axis = P[X, Y];
				P_[X, Y] = Axis switch {
					tAxis.X => tAxis.Y,
					tAxis.Y => tAxis.X,
					tAxis.X | tAxis.Z => tAxis.Y | tAxis.Z,
					tAxis.Y | tAxis.Z => tAxis.X | tAxis.Z,
					_ => Axis
				};
			}
		}
		return P_;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tV3
	To3D(
		this tCamera aCamera,
		tSprite aSprite,
		tV2 aV2
	) {
		if (!aV2.IsInRange(V2(), aSprite.Size))
		{
			return V3();
		}
		var Pos = V3(
			aV2 - (aSprite.Size >> 1),
			aSprite.Deep[aV2.X, aV2.Y] - 4
		) * aCamera.InvM / aCamera.Det;
		
		var PosBits = aSprite.PosBits[aV2.X, aV2.Y];
		return V3(
			CorrectAxis(Pos.X, (PosBits >> 4) & 0b11),
			CorrectAxis(Pos.Y, (PosBits >> 2) & 0b11),
			CorrectAxis(Pos.Z, (PosBits >> 0) & 0b11)
		);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tInt32
	CorrectAxis(
		tInt32 aAxis,
		tInt32 aBits
	) {
		if ((aAxis & 0b11) == aBits) {
			return aAxis;
		}
		var Axis1 = aAxis + 1;
		if ((Axis1 & 0b11) == aBits) {
			return Axis1;
		} else {
			return aAxis - 1;
		}
	}
	
	private static tShadow
	_DrawBlock(
		this tShadow aShadow,
		tBlock aBlock,
		tV3 aOffset,
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		using var Log = mPerfLog.LogPerf();
		
		var (MainAxis, AxisSign) = GetMainAxis(aLightDirection);
		
		var UVOffset = GetShadowUV(aWorldOffset, aLightDirection) - V3(aShadow.Size >> 1, 0);
		
		var BlockSize = aBlock.Colors.GetSize();
		var BlockSizeHalf = BlockSize >> 1;
		
		var Base = aWorldOffset + aOffset - BlockSizeHalf;
		
		#if true
		for (var Z = 0; Z < BlockSize.Z; Z += 1) {
			var Z_ = Base.Z + Z;
			for (var Y = 0; Y < BlockSize.Y; Y += 1) {
				var Y_ = Base.Y + Y;
				for (var X = 0; X < BlockSize.X; X += 1) {
					var X_ = Base.X + X;
					
					var Color = aBlock.Colors[X, Y, Z];
					if (Color == default || Color.IsTransparent()) {
						continue;
					}
					
					var UV = GetShadowUV(
						V3(X_, Y_, Z_),
						aLightDirection
					) - UVOffset;
					
					ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
					pDeep = Min(pDeep, (tInt16)UV.Z);
				}
			}
		}
		#else
		switch (GetMainAxis(aLightDirection)) {
			case (tAxis.X, 1): {
				for (var Z = 0; Z < BlockSize.Z; Z += 1) {
					var Z_ = Base.Z + Z;
					for (var Y = 0; Y < BlockSize.Y; Y += 1) {
						var Y_ = Base.Y + Y;
						for (var X = 0; X < BlockSize.X; X += 1) {
							var X_ = Base.X + X;
							
							var Color = aBlock.Colors[X, Y, Z];
							if (Color == default || Color.IsTransparent()) {
								continue;
							}
							
							var UV = GetShadowUV_PosX(
								V3(X_, Y_, Z_),
								aLightDirection
							) - UVOffset;
							
							ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
							pDeep = Min(pDeep, (tInt16)UV.Z);
						}
					}
				}
				break;
			}
			case (tAxis.X, -1): {
				for (var Z = 0; Z < BlockSize.Z; Z += 1) {
					var Z_ = Base.Z + Z;
					for (var Y = 0; Y < BlockSize.Y; Y += 1) {
						var Y_ = Base.Y + Y;
						for (var X = 0; X < BlockSize.X; X += 1) {
							var X_ = Base.X + X;
							
							var Color = aBlock.Colors[X, Y, Z];
							if (Color == default || Color.IsTransparent()) {
								continue;
							}
							
							var UV = GetShadowUV_NegX(
								V3(X_, Y_, Z_),
								aLightDirection
							) - UVOffset;
							
							ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
							pDeep = Min(pDeep, (tInt16)UV.Z);
						}
					}
				}
				break;
			}
			case (tAxis.Y, 1): {
				for (var Z = 0; Z < BlockSize.Z; Z += 1) {
					var Z_ = Base.Z + Z;
					for (var Y = 0; Y < BlockSize.Y; Y += 1) {
						var Y_ = Base.Y + Y;
						for (var X = 0; X < BlockSize.X; X += 1) {
							var X_ = Base.X + X;
							
							var Color = aBlock.Colors[X, Y, Z];
							if (Color == default || Color.IsTransparent()) {
								continue;
							}
							
							var UV = GetShadowUV_PosY(
								V3(X_, Y_, Z_),
								aLightDirection
							) - UVOffset;
							
							ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
							pDeep = Min(pDeep, (tInt16)UV.Z);
						}
					}
				}
				break;
			}
			case (tAxis.Y, -1): {
				for (var Z = 0; Z < BlockSize.Z; Z += 1) {
					var Z_ = Base.Z + Z;
					for (var Y = 0; Y < BlockSize.Y; Y += 1) {
						var Y_ = Base.Y + Y;
						for (var X = 0; X < BlockSize.X; X += 1) {
							var X_ = Base.X + X;
							
							var Color = aBlock.Colors[X, Y, Z];
							if (Color == default || Color.IsTransparent()) {
								continue;
							}
							
							var UV = GetShadowUV_NegY(
								V3(X_, Y_, Z_),
								aLightDirection
							) - UVOffset;
							
							ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
							pDeep = Min(pDeep, (tInt16)UV.Z);
						}
					}
				}
				break;
			}
			case (tAxis.Z, 1): {
				for (var Z = 0; Z < BlockSize.Z; Z += 1) {
					var Z_ = Base.Z + Z;
					for (var Y = 0; Y < BlockSize.Y; Y += 1) {
						var Y_ = Base.Y + Y;
						for (var X = 0; X < BlockSize.X; X += 1) {
							var X_ = Base.X + X;
							
							var Color = aBlock.Colors[X, Y, Z];
							if (Color == default || Color.IsTransparent()) {
								continue;
							}
							
							var UV = GetShadowUV_PosZ(
								V3(X_, Y_, Z_),
								aLightDirection
							) - UVOffset;
							
							ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
							pDeep = Min(pDeep, (tInt16)UV.Z);
						}
					}
				}
				break;
			}
			case (tAxis.Z, -1): {
				for (var Z = 0; Z < BlockSize.Z; Z += 1) {
					var Z_ = Base.Z + Z;
					for (var Y = 0; Y < BlockSize.Y; Y += 1) {
						var Y_ = Base.Y + Y;
						for (var X = 0; X < BlockSize.X; X += 1) {
							var X_ = Base.X + X;
							
							var Color = aBlock.Colors[X, Y, Z];
							if (Color == default || Color.IsTransparent()) {
								continue;
							}
							
							var UV = GetShadowUV_NegZ(
								V3(X_, Y_, Z_),
								aLightDirection
							) - UVOffset;
							
							ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
							pDeep = Min(pDeep, (tInt16)UV.Z);
						}
					}
				}
				break;
			}
			default: {
				throw new Exception();
			}
		}
		#endif
		return aShadow;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tBool
	IsTransparent(
		this tColor a
	) => (a.Value & 0b_1000_0000) != 0;
	
	private static readonly tV3[]
	cMapAxisToNormal = new tV3[] {
		V3(0, 0, 0),
		V3(127, 0, 0),
		V3(0, 127, 0),
		V3(90, 90, 0),
		V3(0, 0, 127),
		V3(90, 0, 90),
		V3(0, 90, 90),
		V3(73, 73, 73),
	};
	
	private static tSprite
	_DrawBlock(
		this tSprite aGrid,
		tBlock aBlock,
		tV3 aOffset,
		tCamera aCamera
	) {
		using var Log = mPerfLog.LogPerf();
		
		var BlockSize = aBlock.Colors.GetSize();
		var BlockSizeHalf = BlockSize >> 1;
		
		var NormalPattern = aCamera.NormalPattern;
		var DeepPattern = aCamera.DeepPattern;
		
		var Max = NormalPattern.GetSize();
		
		var Rot = M3x3(tV3.cY, -tV3.cX, tV3.cZ);
		var MapAxisToNormal = (aCamera.M.X.X, aCamera.M.X.Y) switch {
			(<= 0, > 0) => cMapAxisToNormal.Select(_ => _ * Rot).ToArray(),
			(< 0, <= 0) => cMapAxisToNormal.Select(_ => _ * Rot * Rot).ToArray(),
			(>= 0, < 0) => cMapAxisToNormal.Select(_ => _ * Rot * Rot * Rot).ToArray(),
			(> 0, >= 0) => cMapAxisToNormal,
		};
		
		for (var Z = 0; Z < BlockSize.Z; Z += 1) {
			for (var Y = 0; Y < BlockSize.Y; Y += 1) {
				for (var X = 0; X < BlockSize.X; X += 1) {
					var Color = aBlock.Colors[X, Y, Z];
					if (Color == default) {
						continue;
					}
					
					var P = V3(X, Y, Z) - BlockSizeHalf;
					var DesUVBase = (
						P + aOffset
					) * aCamera.M + V3(
						(aGrid.Color.GetSize() - Max) >> 1,
						0
					);
					
					for (var V = 0; V < Max.Y; V += 1) {
						for (var U = 0; U < Max.X; U += 1) {
							var Des = DesUVBase + V3(U, V, 0);
							if (Color.IsTransparent() && ((Des.X ^ Des.Y) & 1) == 0) {
								continue;
							}
							
							var Axis = NormalPattern[U, V];
							if (Axis != tAxis._ && Des.X >= 0 && Des.Y >= 0) {
								var Deep = Des.Z + DeepPattern[U, V];
								ref var pDeep = ref aGrid.Deep[Des.X, Des.Y];
								if (Deep <= pDeep) {
									pDeep = (short)Deep;
									
									aGrid.Color[Des.X, Des.Y] = Color & 0b_0111_1111;
									
									var Normal = MapAxisToNormal[(tInt32)Axis];
									aGrid.Normal[Des.X, Des.Y] = ((tInt8)Normal.X, (tInt8)Normal.Y);
									
									aGrid.PosBits[Des.X, Des.Y] = (tNat8)(
										((P.X & 0b11) << 4) |
										((P.Y & 0b11) << 2) |
										((P.Z & 0b11) << 0)
									);
								}
							}
						}
					}
				}
			}
		}
		
		#if !true
		var MarkerColor = RGB(4, 0, 0);
		aGrid.Color[0, 0] = MarkerColor;
		aGrid.Color[aGrid.Size.X - 1, 0] = MarkerColor;
		aGrid.Color[0, aGrid.Size.Y - 1] = MarkerColor;
		aGrid.Color[aGrid.Size.X - 1, aGrid.Size.Y - 1] = MarkerColor;
		
		aGrid.Deep[0, 0] = 0;
		aGrid.Deep[aGrid.Size.X - 1, 0] = 0;
		aGrid.Deep[0, aGrid.Size.Y - 1] = 0;
		aGrid.Deep[aGrid.Size.X - 1, aGrid.Size.Y - 1] = 0;
		#endif
		
		return aGrid;
	}
	
	private static tSprite
	_DrawSprite(
		this tSprite aDes,
		tSprite aSrc,
		tV3 aScreenOffset,
		tNat8[] aBitsMap
	) {
		using var Log = mPerfLog.LogPerf();
		
		var SrcMin = V2();
		var SrcSize = aSrc.Color.GetSize();
		var SrcMax = SrcMin + SrcSize;
		
		var DesSize = aDes.Color.GetSize();
		var DesMin = ((DesSize - SrcSize) >> 1) + aScreenOffset.XY();
		var DesMax = DesMin + SrcSize;
		var DesMaxY = DesSize.Y;
		
		if (
			DesMin.X > DesSize.X ||
			DesMin.Y > DesSize.Y ||
			DesMax.X < 0 ||
			DesMax.Y < 0
		) {
			return aDes;
		}
		
		var Size = SrcSize;
		
		if (DesMin.X < 0) {
			var R = -DesMin.X;
			DesMin.X = 0;
			SrcMin.X += R;
			Size.X -= R;
		}
		if (DesMin.Y < 0) {
			var R = -DesMin.Y;
			DesMin.Y = 0;
			SrcMin.Y += R;
			Size.Y -= R;
		}
		if (DesMax.X > DesSize.X) {
			var R = DesMax.X - DesSize.X;
			Size.X -= R;
		}
		if (DesMax.Y > DesSize.Y) {
			var R = DesMax.Y - DesSize.Y;
			Size.Y -= R;
		}
		
		for (var Y = 0; Y < Size.Y; Y += 1) {
			for (var X = 0; X < Size.X; X += 1) {
				var SrcX = SrcMin.X + X;
				var SrcY = SrcMin.Y + Y;
				var DesX = DesMin.X + X;
				var DesY = DesMin.Y + Y;
				var SrcDeep = aSrc.Deep[SrcX, SrcY] + aScreenOffset.Z;
				ref var pDesDeep = ref aDes.Deep[DesX, DesY];
				if (SrcDeep <= pDesDeep) {
					pDesDeep = (tInt16)SrcDeep;
					aDes.Color[DesX, DesY] = aSrc.Color[SrcX, SrcY];
					aDes.Normal[DesX, DesY] = aSrc.Normal[SrcX, SrcY];
					aDes.PosBits[DesX, DesY] = aBitsMap[aSrc.PosBits[SrcX, SrcY]];
				}
			}
		}
		
		return aDes;
	}
	
	private static tShadow
	_DrawShadow(
		this tShadow aDes,
		tShadow aSrc,
		tV3 aOffset2D
	) {
		using var Log = mPerfLog.LogPerf();
		
		var SrcMin = V2();
		var SrcMax = SrcMin + aSrc.Size;
		
		var DesMin = aOffset2D.XY() + (aDes.Size >> 1) - (aSrc.Size >> 1);
		var DesMax = DesMin + aSrc.Size;
		
		if (
			DesMin.X > aDes.Size.X ||
			DesMin.Y > aDes.Size.Y ||
			DesMax.X < 0 ||
			DesMax.Y < 0
		) {
			return aDes;
		}
		
		var SrcSize = aSrc.Size;
		
		if (DesMin.X < 0) {
			var Delta = -DesMin.X;
			DesMin.X = 0;
			SrcMin.X += Delta;
			SrcSize.X -= Delta;
		}
		if (DesMin.Y < 0) {
			var Delta = -DesMin.Y;
			DesMin.Y = 0;
			SrcMin.Y += Delta;
			SrcSize.Y -= Delta;
		}
		if (DesMax.X > aDes.Size.X) {
			var Delta = DesMax.X - aDes.Size.X;
			SrcSize.X -= Delta;
			DesMax.X = aDes.Size.X;
		}
		if (DesMax.Y > aDes.Size.Y) {
			var Delta = DesMax.Y - aDes.Size.Y;
			SrcSize.Y -= Delta;
			DesMax.Y = aDes.Size.Y;
		}
		
		for (var YOffset = 0; YOffset < SrcSize.Y; YOffset += 1) {
			for (var XOffset = 0; XOffset < SrcSize.X; XOffset += 1) {
				var SrcDeep = aSrc.Deep[SrcMin.X + XOffset, SrcMin.Y + YOffset];
				if (SrcDeep != tInt16.MaxValue) {
					ref var pDesDeep = ref aDes.Deep[DesMin.X + XOffset, DesMin.Y + YOffset];
					pDesDeep = Min(
						pDesDeep,
						(tInt16)(SrcDeep + aOffset2D.Z)
					);
				}
			}
		}
		
		return aDes;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tInt16
	GetShadowDistance(
		this tShadow aShadow,
		tV3 aLightDirection,
		tV3 aWorldPos
	) {
		var UV = GetShadowUV(aWorldPos, aLightDirection) + V3(aShadow.Size >> 1, 0);
		
		if (UV.XY().IsInRange(V2(), aShadow.Size - V2(1))) {
			var D = aShadow.Deep[UV.X, UV.Y];
			return D == tInt16.MaxValue
				? tInt16.MaxValue
				: (tInt16)(D - UV.Z);
		} else {
			return tInt16.MaxValue;
		}
	}
	
	public struct
	tShaderInput {
		public tInt32 U;
		public tInt32 V;
		public tV3 WorldPos;
		public tV3 LightDirection;
		public tInt32 H;
		public tSprite Sprite;
		public tInt16 ShadowDistance;
	}
	
	private static (tDebugRenderMode, tFunc<tShaderInput, tNat32>)[]
	cStdShader = new (tDebugRenderMode, tFunc<tShaderInput, tNat32>)[] {
		(
			tDebugRenderMode.None,
			(tShaderInput a) => {
				var D = a.Sprite.Deep[a.U, a.V];
				var F = 0;
				try {
					var D_N = (a.Sprite.Deep[a.U, a.V - 1] - D) >> 0;
					var D_S = (a.Sprite.Deep[a.U, a.V + 1] - D) >> 0;
					var D_E = (a.Sprite.Deep[a.U + 1, a.V] - D) >> 0;
					var D_W = (a.Sprite.Deep[a.U - 1, a.V] - D) >> 0;
					
					if ((D_N - D_S).Abs() is 1) {
						D_S = 0;
						D_N = 0;
					}
					
					if ((D_E - D_W).Abs() is 1) {
						D_W = 0;
						D_E = 0;
					}
					
					F = (D_N + D_S + D_E + D_W) >> 0;
					F <<= 6;
					F = F.Clamp(-32, 32);
					//F = 0;
				} catch {
				}
				var ColorIndex = a.Sprite.Color[a.U, a.V];
				var C = ToRGB(ColorIndex) * a.H >> 8;
				return (tNat32)(
					(0xFF << 24) |
					(((C.X + F).Clamp(0, 255) & 0xFF) << 16) |
					(((C.Y + F).Clamp(0, 255) & 0xFF) << 8) |
					(((C.Z + F).Clamp(0, 255) & 0xFF) << 0)
				);
			}
		), (
			tDebugRenderMode.Pattern,
			(tShaderInput a) => {
				var d = ((a.Sprite.Deep[a.U, a.V] & 0b0000_0111) << 5) | 0b0001_1111;
				var c = a.Sprite.Normal[a.U, a.V] switch {
					(0, 0) => d << 16,
					(_, 0) => d << 8,
					_ => d	
				};
				return (tNat32)(
					0x_FF_00_00_00 | 
					c
				);
			}
		), (
			tDebugRenderMode.Deep,
			(tShaderInput a) => {
				return (tNat32)(
					0x_FF_00_00_00 |
					(a.Sprite.Deep[a.U, a.V] << 4)
				);
			}
		), (
			tDebugRenderMode.Normal,
			(tShaderInput a) => {
				var N = a.Sprite.Normal[a.U, a.V];
				var Normal = GetNormal3D(N);
				
				return (tNat32)(
					0x_FF_00_00_00 |
					(((128 + Normal.X) & 0xFF) << 16) |
					(((128 + Normal.Y) & 0xFF) << 8) |
					(((128 + Normal.Z) & 0xFF) << 0)
				);
			}
		), (
			tDebugRenderMode.PosBits,
			(tShaderInput a) => {
				var PosBits = a.Sprite.PosBits[a.U, a.V];
				return (tNat32)(
					0x_FF_00_00_00 |
					((PosBits & 0b110000) << 17) |
					((PosBits & 0b001100) << 11) |
					((PosBits & 0b000011) << 5)
				);
			}
		), (
			tDebugRenderMode.Pos,
			(tShaderInput a) => {
				var Pos = a.WorldPos;
				return (tNat32)(
					0x_FF_00_00_00 |
					((Pos.X & 0b0111) << 20) |
					((Pos.Y & 0b0111) << 12) |
					((Pos.Z & 0b0111) << 4)
				);
			}
		),
	};
	
	private static unsafe ref tCamera
	_RenderToBuffer(
		this ref tCamera aCamera,
		tV3 aLightDirection,
		tSprite aSprite,
		tShadow aShadow,
		System.IntPtr aBuffer,
		tV2 aBufferSize,
		tDebugRenderMode aDebugRenderMode
	) {
		using var Log = mPerfLog.LogPerf();
		
		var Max = Min(aSprite.Color.GetSize(), aBufferSize);
		var DeltaX = aBufferSize.X - Max.X;
		
		{
			using var PerfLog1 = mPerfLog.LogPerf("_RenderToBuffer/Shader");
			
			var Shader = cStdShader.First(_ => _.Item1 == aDebugRenderMode).Item2;
			var ShaderInput = new tShaderInput {
				Sprite = aSprite,
				LightDirection = aLightDirection,
			};
			
			var YPtr = (tNat32*)aBuffer;
			for (var V = 0; V < Max.Y; V += 1) {
				ShaderInput.V = V;
				var XPtr = YPtr;
				for (var U = 0; U < Max.X; U += 1) {
					ShaderInput.U = U;
					var ColorIndex = aSprite.Color[U, V];
					if (ColorIndex == default) {
						XPtr += 1;
						continue;
					}
					
					var Normal = GetNormal3D(aSprite.Normal[U, V]);
					
					var WorldPos = To3D(aCamera, aSprite, V2(U, V));
					ShaderInput.WorldPos = WorldPos;
					
					var ShadowDistance = aShadow.GetShadowDistance(
						aLightDirection,
						WorldPos + Normal.Sign()
					);
					ShaderInput.ShadowDistance = ShadowDistance;
					
					var H = 0;
					if (ShadowDistance < 0) {
						H = 50;
					} else {
						var S = (Normal * aLightDirection * V3(1, 1, -1)).Sum();
						if (S <= 0) {
							H = 50;
						} else {
							H = 50 + (tInt32)(S / 9);
						}
					}
					ShaderInput.H = H;
					
					*XPtr = Shader(ShaderInput);
					
					XPtr += 1;
				}
				YPtr += aBufferSize.X;
			}
		}
		
		{
			using var PerfLog = mPerfLog.LogPerf("_RenderToBuffer/Copy");
			
			var YPtr = (tNat32*)aBuffer;
			for (var Y = 0; Y < Min(aShadow.Size.Y, aBufferSize.Y); Y += 1) {
				var XPtr = YPtr;
				for (var X = 0; X < Min(aShadow.Size.X, aBufferSize.X); X += 1) {
					if (aShadow.Deep[X, Y] == tInt16.MaxValue) {
						*XPtr = 0xFF_FF_00_FF;
					} else {
						var Deep = (aShadow.Deep[X, Y] << 4) & 0xFF;
						*XPtr = (tNat32)(
							0xFF_00_00_00 |
							(Deep << 16) |
							(Deep << 8) |
							(Deep << 0)
						);
					}
					XPtr += 1;
				}
				YPtr += aBufferSize.X;
			}
		}
		
		return ref aCamera;
	}
	
	private static unsafe ref tRenderEnv
	_DrawTo(
		this ref tRenderEnv aRenderEnv,
		tSprite aCanvas,
		tShadow aShadow,
		tBlock aBlock,
		tV3 aWorldOffset
	) {
		using var Log = mPerfLog.LogPerf();
		
		var BitsMap = new tNat8[64];
		for (var Z = 0; Z < 4; Z += 1) {
			for (var Y = 0; Y < 4; Y += 1) {
				for (var X = 0; X < 4; X += 1) {
					BitsMap[
						((X & 0b11) << 4) |
						((Y & 0b11) << 2) |
						((Z & 0b11) << 0)
					] = (tNat8)(
						(((X + aWorldOffset.X) & 0b11) << 4) |
						(((Y + aWorldOffset.Y) & 0b11) << 2) |
						(((Z + aWorldOffset.Z) & 0b11) << 0)
					);
				}
			}
		}
		
		var BlockSprite = GetOrCreateSprite(aRenderEnv.SpriteBuffer, aRenderEnv.Camera, aBlock);
		aCanvas._DrawSprite(BlockSprite, aWorldOffset * aRenderEnv.Camera.M, BitsMap);
		
		if (aShadow.Size != V2()) {
			var BlockShadow = aRenderEnv.ShadowBuffer.GetOrCreateShadow(
				aRenderEnv.LightDirection,
				aBlock,
				aWorldOffset
			);
			
			aShadow._DrawShadow(BlockShadow, GetShadowUV(aWorldOffset, aRenderEnv.LightDirection));
		}
		
		return ref aRenderEnv;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetShadowUV(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var (MainAxis, AxisSign) = GetMainAxis(aLightDirection);
		
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var Deep = default(tInt32);
		var UV = default(tV2);
		
		switch (MainAxis) {
			case tAxis.X: {
				UV = V2(Z_, -Y_) + aLightDirection.ZY() * X_ / aLightDirection.X;
				UV *= V2(-AxisSign, 1);
				Deep = -X_;
				break;
			}
			case tAxis.Y: {
				UV = V2(-X_, Z_) + aLightDirection.XZ() * Y_ / aLightDirection.Y;
				UV *= V2(-1, AxisSign);
				Deep = -Y_;
				break;
			}
			case tAxis.Z: {
				UV = V2(X_, Y_) + aLightDirection.XY() * Z_ / aLightDirection.Z; 
				UV *= V2(-1, AxisSign);
				Deep = Z_;
				break;
			}
		}
		
		return V3(UV, AxisSign * Deep);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetShadowUV_PosX(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var UV = V2(Z_, -Y_) + aLightDirection.ZY() * X_ / aLightDirection.X;
		UV *= V2(-1, 1);
		
		return V3(UV, -X_);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetShadowUV_NegX(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var UV = V2(Z_, -Y_) + aLightDirection.ZY() * X_ / aLightDirection.X;
		
		return V3(UV, -X_);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetShadowUV_PosY(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var UV = V2(-X_, Z_) + aLightDirection.XZ() * Y_ / aLightDirection.Y;
		UV *= V2(-1, 1);
		
		return V3(UV, -Y_);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetShadowUV_NegY(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var UV = V2(-X_, Z_) + aLightDirection.XZ() * Y_ / aLightDirection.Y;
		
		return V3(-UV, -Y_);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetShadowUV_PosZ(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var UV = V2(X_, Y_) + aLightDirection.XY() * Z_ / aLightDirection.Z; 
		UV *= V2(-1, 1);
		
		return V3(UV, Z_);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetShadowUV_NegZ(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var UV = V2(X_, Y_) + aLightDirection.XY() * Z_ / aLightDirection.Z; 
		
		return V3(-UV, -Z_);
	}
	
	public static tRendererDLL
	Create(
	) => new tRendererDLL {
		To3D = To3D,
		_RenderToBuffer = _RenderToBuffer,
		_DrawTo = _DrawTo,
	};
}
