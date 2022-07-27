using System.Linq;
using System.Runtime.CompilerServices;

using static mStd;
using static mMath;
using static mMath2D;
using static mMath3D;
using static mVoxelRenderer;

public static class
mVoxelRenderer_HotReload {
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tSprite
	GetOrCreateSprite(
		this tRenderEnv aRenderEnv,
		tBlock aBlock
	) {
		var BlocSize = aBlock.Colors.GetSize();
		
		if (!aRenderEnv.SpriteBuffer.TryGetValue((aRenderEnv.M, aBlock), out var Sprite)) {
			var SpriteSize = GetSpriteSize(BlocSize, aRenderEnv.M);
			Sprite = CreateSprite(
				SpriteSize,
				mMath2D.V2() 
			)
			._Clear()
			._DrawBlock(aBlock, V3(), aRenderEnv);
			
			aRenderEnv.SpriteBuffer[(aRenderEnv.M, aBlock)] = Sprite;
		}
		return Sprite;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tShadow
	GetOrCreateShadow(
		this tRenderEnv aRenderEnv,
		tBlock aBlock,
		tV3 aWorldOffset
	) {
		var LightDirection = aRenderEnv.LightDirection;
		var LayerOffset = GetMainAxis(aRenderEnv.LightDirection).Axis switch {
			tAxis.X => LightDirection.X == 0 ? 0 : aWorldOffset.X % LightDirection.X,
			tAxis.Y => LightDirection.Y == 0 ? 0 : aWorldOffset.Y % LightDirection.Y,
			tAxis.Z => LightDirection.Z == 0 ? 0 : aWorldOffset.Z % LightDirection.Z,
			_ => 0,
		};
		
		if (!aRenderEnv.ShadowBuffer.TryGetValue((LightDirection, LayerOffset, aBlock), out var Shadow)) {
			var ShadowSize = GetShadowSize(aBlock.GetSize(), LightDirection);
			Shadow = CreateShadow(
				ShadowSize,
				V2() // TODO
			)
			._Clear()
			._DrawBlock(aBlock, V3(), aWorldOffset, LightDirection);
			
			aRenderEnv.ShadowBuffer[(LightDirection, LayerOffset, aBlock)] = Shadow;
		}
		return Shadow;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tM3x3
	GetMatrix(
		this tRenderEnv aRenderEnv,
		tInt32 aDir,
		tInt32 aAngle
	) {
		var (QuarterParts, AngleParts) = aRenderEnv.NormalPatterns.GetSize();
		var M = aRenderEnv.Matrixes[aDir % QuarterParts, aAngle];
		while (aDir >= QuarterParts) {
			M *= cRotRight;
			aDir -= QuarterParts;
		}
		return M;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tAxis[,]
	GetNormalPattern(
		this tRenderEnv aRenderEnv
	) {
		var (AngleParts, QuarterParts) = aRenderEnv.NormalPatterns.GetSize();
		var P = aRenderEnv.NormalPatterns[aRenderEnv.Dir % AngleParts, aRenderEnv.Angle];
		if (
			aRenderEnv.Dir.IsInRange(0*QuarterParts, 1*QuarterParts-1) ||
			aRenderEnv.Dir.IsInRange(2*QuarterParts, 3*QuarterParts-1)
		) {
			return P;
		}
		var Max = P.GetSize();
		var P_ = new tAxis[Max.X, Max.Y];
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
		this tRenderEnv aRenderEnv,
		tSprite aSprite,
		tV2 aV2
	) {
		var Pos = V3(
			aV2 - (aSprite.Size >> 1),
			aSprite.Deep[aV2.X, aV2.Y] - 4
		) * aRenderEnv.InvM / aRenderEnv.Det;
		
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
		if (((aAxis ^ aBits) & 0b11) == 0) {
			return aAxis;
		}
		var Axis = aAxis + 1;
		if (((Axis ^ aBits) & 0b11) == 0) {
			return Axis;
		} else {
			return aAxis -1;
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
		var UVOffset = GetShadowUV(aWorldOffset, aLightDirection) - V3(aShadow.Size >> 1, 0);
		
		var BlockSize = aBlock.Colors.GetSize();
		var BlockSizeHalf = BlockSize >> 1;
		
		var Base = aWorldOffset + aOffset - BlockSizeHalf;
		
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
		
		return aShadow;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tBool
	IsTransparent(
		this tColor a
	) => (a.Value & 0b_1000_0000) != 0;
	
	private static tSprite
	_DrawBlock(
		this tSprite aGrid,
		tBlock aBlock,
		tV3 aOffset,
		tRenderEnv aRenderEnv
	) {
		var BlockSize = aBlock.Colors.GetSize();
		var BlockSizeHalf = BlockSize >> 1;
		
		var NormalPattern = aRenderEnv.NormalPattern;
		var DeepPattern = aRenderEnv.DeepPattern;
		
		var Max = NormalPattern.GetSize();
		
		var MabAxisToNormal = new [] {
			V3(0, 0, 0),
			V3(127, 0, 0),
			V3(0, 127, 0),
			V3(90, 90, 0),
			V3(0, 0, 127),
			V3(90, 0, 90),
			V3(0, 90, 90),
			V3(73, 73, 73),
		};
		
		var Rot = M3x3(tV3.cY, -tV3.cX, tV3.cZ);
		var QDir = aRenderEnv.NormalPatterns.GetLength(0);
		MabAxisToNormal = ((aRenderEnv.Dir / QDir) % 4) switch {
			1 => MabAxisToNormal.Select(_ => _ * Rot).ToArray(),
			2 => MabAxisToNormal.Select(_ => _ * Rot * Rot).ToArray(),
			3 => MabAxisToNormal.Select(_ => _ * Rot * Rot * Rot).ToArray(),
			_ => MabAxisToNormal,
		};
		
		for (var Z = 0; Z < BlockSize.Z; Z += 1) {
			for (var Y = 0; Y < BlockSize.Y; Y += 1) {
				for (var X = 0; X < BlockSize.X; X += 1) {
					var Color = aBlock.Colors[X, Y, Z];
					if (Color == default) {
						continue;
					}
					
					var P = V3(X, Y, Z);
					var DesUVBase = (
						(P - BlockSizeHalf) + aOffset
					) * aRenderEnv.M + V3(
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
									
									var Normal = MabAxisToNormal[(tInt32)Axis];
									aGrid.Normal[Des.X, Des.Y] = ((tInt8)Normal.X, (tInt8)Normal.Y);
									
									aGrid.PosBits[Des.X, Des.Y] = (tNat8)(
										((X & 0b11) << 4) |
										((Y & 0b11) << 2) |
										((Z & 0b11) << 0)
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
		this tRenderEnv aRenderEnv,
		tShadow aShadow,
		tV3 aWorldPos
	) {
		var UV = GetShadowUV(aWorldPos, aRenderEnv.LightDirection) + V3(aShadow.Size >> 1, 0);
		
		if (UV.XY().IsInRange(V2(), aShadow.Size - V2(1))) {
			var D = aShadow.Deep[UV.X, UV.Y];
			return D == tInt16.MaxValue
				? tInt16.MaxValue
				: (tInt16)(D - UV.Z);
		} else {
			return tInt16.MaxValue;
		}
	}
	
	private static unsafe ref tRenderEnv
	_RenderToBuffer(
		this ref tRenderEnv aRenderEnv,
		tSprite aSprite,
		tShadow aShadow,
		System.IntPtr aBuffer,
		tV2 aBufferSize,
		tDebugRenderMode aDebugRenderMode
	) {
		var Max = Min(aSprite.Color.GetSize(), aBufferSize);
		var DeltaX = aBufferSize.X - Max.X;
		
		var YPtr = (tNat32*)aBuffer;
		for (var Y = 0; Y < Max.Y; Y += 1) {
			var XPtr = YPtr;
			for (var X = 0; X < Max.X; X += 1) {
				var ColorIndex = aSprite.Color[X, Y];
				if (ColorIndex == default) {
					XPtr += 1;
					continue;
				}
				var N = aSprite.Normal[X, Y];
				var Normal = GetNormal3D(N);
				
				var WorldPos = To3D(aRenderEnv, aSprite, V2(X, Y));
				var ShadowDistance = aRenderEnv.GetShadowDistance(
					aShadow,
					WorldPos + Normal.Sign()
				);
				
				var H = 0;
				if (ShadowDistance < 0) {
					H = 50;
				} else
				if ((Normal * aRenderEnv.LightDirection * V3(-1, -1, 1)).Sum() >= 0) {
					H = 50;
				} else {
					H = 50 -(tInt32)((Normal * aRenderEnv.LightDirection * V3(-1, -1, 1)).Sum() / 9);
				}
				
				if (ColorIndex == RGB(4, 0, 0)) {
					"".ToString();
				}
				
				switch (aDebugRenderMode) {
					case tDebugRenderMode.None: {
						var C = ToRGB(ColorIndex) * H / 256;
						*XPtr = (tNat32)(
							(0xFF << 24) |
							((C.X & 0xFF) << 16) |
							((C.Y & 0xFF) << 8) |
							((C.Z & 0xFF) << 0)
						);
						break;
					}
					case tDebugRenderMode.Pattern: {
						var d = ((aSprite.Deep[X, Y] & 0b0000_0111) << 5) | 0b0001_1111;
						var c = aSprite.Normal[X, Y] switch {
							(0, 0) => d << 16,
							(_, 0) => d << 8,
							_ => d	
						};
						*XPtr = (tNat32)(
							0x_FF_00_00_00 |
							c
						);
						break;
					}
					case tDebugRenderMode.Deep: {
						*XPtr = (tNat32)(
							0x_FF_00_00_00 |
							(aSprite.Deep[X, Y] << 4)
						);
						break;
					}
					case tDebugRenderMode.Normal: {
						*XPtr = (tNat32)(
							0x_FF_00_00_00 |
							(((128 + Normal.X) & 0xFF) << 16) |
							(((128 + Normal.Y) & 0xFF) << 8) |
							(((128 + Normal.Z) & 0xFF) << 0)
						);
						break;
					}
					case tDebugRenderMode.PosBits: {
						var PosBits = aSprite.PosBits[X, Y];
						*XPtr = (tNat32)(
							0x_FF_00_00_00 |
							((PosBits & 0b110000) << 17) |
							((PosBits & 0b001100) << 11) |
							((PosBits & 0b000011) << 5)
						);
						break;
					}
					case tDebugRenderMode.Pos: {
						var Pos = To3D(aRenderEnv, aSprite, V2(X, Y));
						*XPtr = (tNat32)(
							0x_FF_00_00_00 |
							((Pos.X & 0b0111) << 20) |
							((Pos.Y & 0b0111) << 12) |
							((Pos.Z & 0b0111) << 4)
						);
						break;
					}
				}
				
				XPtr += 1;
			}
			YPtr += aBufferSize.X;
		}
		
		YPtr = (tNat32*)aBuffer;
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
		
		return ref aRenderEnv;
	}
	
	private static unsafe ref tRenderEnv
	_DrawTo(
		this ref tRenderEnv aRenderEnv,
		tSprite aCanvas,
		tShadow aShadow,
		tBlock aBlock,
		tV3 aWorldOffset
	) {
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
		
		var BlockSprite = GetOrCreateSprite(aRenderEnv, aBlock);
		aCanvas._DrawSprite(BlockSprite, aWorldOffset * aRenderEnv.M, BitsMap);
		
		if (aShadow.Size != V2()) {
			var BlockShadow = GetOrCreateShadow(aRenderEnv, aBlock, aWorldOffset);
			aShadow._DrawShadow(BlockShadow, GetShadowUV(aWorldOffset, aRenderEnv.LightDirection));
		}
		
		return ref aRenderEnv;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
				UV *= V2(AxisSign, 1);
				Deep = -X_;
				break;
			}
			case tAxis.Y: {
				UV = V2(-X_, Z_) + aLightDirection.XZ() * Y_ / aLightDirection.Y;
				UV *= V2(1, AxisSign);
				Deep = -Y_;
				break;
			}
			case tAxis.Z: {
				UV = V2(X_, Y_) + aLightDirection.XY() * Z_ / aLightDirection.Z; 
				UV *= V2(1, AxisSign);
				Deep = Z_;
				break;
			}
		}
		
		return V3(
			-UV.X,
			UV.Y,
			AxisSign * Deep);
	}
	
	public static tRendererDLL
	Create(
	) => new tRendererDLL {
		GetMatrix = GetMatrix,
		To3D = To3D,
		_RenderToBuffer = _RenderToBuffer,
		_DrawTo = _DrawTo,
	};
}
