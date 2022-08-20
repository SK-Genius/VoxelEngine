using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

using static m2DArray;
using static m3DArray;
using static mStd;
using static mMath;
using static mM3x3;
using static mV2;
using static mV3;
using static mHotReload;
using static mFileWatcher;

public static class
mVoxelRenderer {
	
	public class
	tRendererDLL {
		public tFunc<tRenderEnv, tSprite, tV2,  tV3> To3D;
		public tMeth<tRenderEnv, tSprite, tShadow, System.IntPtr, tV2, tDebugRenderMode> _RenderToBuffer;
		public tMeth<tRenderEnv, tSprite, tShadow, tBlock, tV3> _DrawTo;
		public tFunc<tRenderEnv, tInt32, tInt32, tM3x3> GetMatrix;
	}
	
	public struct
	tBlock {
		public tInt32 Id;
		public tV3 Offset;
		public tColor[,,] Colors;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int
		GetHashCode(
		) => this.Id;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool
		Equals(
			object? a
		) => this.Id == ((tBlock)a).Id;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tBlock
	CreateBlock(
		tV3 aOffset,
		tColor[,,] aColors
	) => new tBlock {
		Id = HashBlock(aColors),
		Offset = aOffset,
		Colors = aColors,
	};
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	HashBlock(
		tColor[,,] aColors
	) {
		var Size = aColors.GetSize();
		
		var Hash = 0;
		AddToHash(ref Hash, Size.X);
		AddToHash(ref Hash, Size.Y);
		AddToHash(ref Hash, Size.Z);
		for (var Z = 0; Z < Size.Z; Z += 1) {
			for (var Y = 0; Y < Size.Y; Y += 1) {
				for (var X = 0; X < Size.X; X += 1) {
					AddToHash(ref Hash, aColors[X, Y, Z].Value);
				}
			}
		}
		return Hash;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetSize(
		this tBlock a
	) => a.Colors.GetSize();
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tInt32
	AddToHash(
		ref tInt32 aHash,
		tInt32 aValue
	) {
		aHash ^= aHash << 13;
		aHash ^= aHash >> 17;
		aHash ^= aHash << 5;
		aHash ^= 0x12_34_56_78;
		aHash ^= aValue;
		return aHash;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tM3x3
	GetMatrix(
		this ref tRenderEnv aRenderEnv,
		tInt32 aDir,
		tInt32 aAngle
	) => aRenderEnv.HotReloat.DLL.GetMatrix(aRenderEnv, aDir, aAngle);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	To3D(
		this ref tRenderEnv aRenderEnv,
		tSprite aSprite,
		tV2 aV2
	) => aRenderEnv.HotReloat.DLL.To3D(aRenderEnv, aSprite, aV2);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe ref tRenderEnv
	_RenderToBuffer(
		this ref tRenderEnv aRenderEnv,
		tSprite aSprite,
		tShadow aShadow,
		System.IntPtr aBuffer,
		tV2 aBufferSize,
		tDebugRenderMode aDebugRenderMode
	) => ref aRenderEnv.HotReloat.DLL._RenderToBuffer(ref aRenderEnv, aSprite, aShadow, aBuffer, aBufferSize, aDebugRenderMode);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe ref tRenderEnv
	_DrawTo(
		this ref tRenderEnv aRenderEnv,
		tSprite aCanvas,
		tShadow aShadow,
		tBlock aBlock,
		tV3 aOffset
	) => ref aRenderEnv.HotReloat.DLL._DrawTo(
		ref aRenderEnv,
		aCanvas,
		aShadow,
		aBlock,
		aOffset
	);
	
	[System.Flags]
	public enum
	tAxis {
		_ = 0,
		X = 1,
		Y = 2,
		Z = 4,
	};
	
	public struct
	tRenderEnv {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public tRenderEnv() {}
		public tInt32 Dir;
		public tInt32 Angle;
		
		public tM3x3[,] Matrixes;
		public tAxis[,][,] NormalPatterns;
		public tInt16[,][,] DeepPatterns;
		public tInt32 PatternScale = 1;
		
		public tHotReload<tRendererDLL> HotReloat = new (
			new DirectoryInfo("./"),
			"VoxelRenderer.HotReload.dll"
		);
		
		public tFileWatcher PatternFile = new (
			new DirectoryInfo("./"),
			"Patterns4_6x9.txt"
		);
		
		public tV3 LightDirection;
		
		public tM3x3 M;
		public tM3x3 InvM;
		public tInt32 Det;
		
		public tAxis[,] NormalPattern;
		public tInt16[,] DeepPattern;
		
		public Dictionary<(tM3x3, tBlock), tSprite> SpriteBuffer  = new();
		public Dictionary<(tV3 LightDirection, tInt32 LayerOffset, tBlock Block), tShadow> ShadowBuffer  = new();
	}
	
	[DebuggerDisplay("{Value}")]
	public struct
	tColor {
		public tNat8 Value;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator==(
			tColor a1,
			tColor a2
		) => a1.Value == a2.Value;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator!=(
			tColor a1,
			tColor a2
		) => a1.Value != a2.Value;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tColor
		operator&(
			tColor aColor,
			tNat8 aMask
		) => new tColor{ Value = (tNat8)(aColor.Value & aMask) };
	}
	
	public struct
	tShadow {
		public tV2 Size;
		public tV2 Offset;
		public tInt16[,] Deep;
	}
	
	public struct
	tSprite {
		public tV2 Size;
		public tV2 Offset;
		public tColor[,] Color;
		public (tInt8 U, tInt8 V)[,] Normal;
		public tInt16[,] Deep;
		public tNat8[,] PosBits;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static t[,]
	ToArray2D<t>(
		this List<t[]> a
	) {
		return a
		.SelectMany((aRow, aRowNr) => aRow.Select((aVal, aColNr) => (V: aRowNr, U: aColNr, Val: aVal)))
		.Aggregate(
			new t[a[0].Length, a.Count],
			(aRes, a) => {
				aRes[a.U, a.V] = a.Val;
				return aRes;
			}
		);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tRenderEnv
	CreateEnv(
	) {
		var RenderEnv = new tRenderEnv();
		return RenderEnv
		._LoadPatterns()
		._Update()
		;
	}
	
	public static ref tRenderEnv
	_LoadPatterns(
		this ref tRenderEnv aRenderEnv
	) {
		var AngelParts = 0;
		var QuoterParts = 0;
		var Angel = 0;
		var Dir = -1;
		
		var MRow = 0;
		var M = M3x3();
		var NormalPattern = new List<tAxis[]>();
		var DeepPattern = new List<tInt16[]>();
		
		var IsFirstLine = true;
		foreach (var Line in File.ReadAllLines(aRenderEnv.PatternFile.File.FullName)) {
			if (IsFirstLine) {
				IsFirstLine = false;
				
				var Parts = Line.Split('x');
				AngelParts = int.Parse(Parts[0]);
				QuoterParts = int.Parse(Parts[1]);
				aRenderEnv.Matrixes = new tM3x3[AngelParts, QuoterParts];
				aRenderEnv.DeepPatterns = new tInt16[AngelParts, QuoterParts][,];
				aRenderEnv.NormalPatterns = new tAxis[AngelParts, QuoterParts][,];
			} else if (Line.StartsWith('#') || string.IsNullOrWhiteSpace(Line)) {
				if (Dir < 0) {
					Dir += 1;
					continue;
				}
				
				aRenderEnv.Matrixes[Dir, Angel] = M;
				MRow = 0;
				
				aRenderEnv.DeepPatterns[Dir, Angel] = DeepPattern.ToArray2D();
				DeepPattern.Clear();
				
				aRenderEnv.NormalPatterns[Dir, Angel] = NormalPattern.ToArray2D();
				NormalPattern.Clear();
				
				if (Line.StartsWith('#')) {
					Angel = 0;
					Dir += 1;
				} else {
					Angel += 1;
				}
			} else if (Line.StartsWith('|')) {
				var Row = Line
				.Substring(1)
				.Split(' ', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
				.Select(int.Parse)
				.ToArray()
				;
				
				M = MRow switch {
					0 => M3x3(
						V3(Row[0], Row[1], Row[2]),
						M.Y,
						M.Z
					),
					1 => M3x3(
						M.X,
						V3(Row[0], Row[1], Row[2]),
						M.Z
					),
					2 => M3x3(
						M.X,
						M.Y,
						V3(Row[0], Row[1], Row[2])
					),
					_ => M
				};
				MRow += 1;
			} else {
				var Row = Line
				.Split(' ', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
				.Select(
					_ => (
						Axis: _[0] switch {
							'x' => tAxis.X,
							'y' => tAxis.Y,
							'z' => tAxis.Z,
							_ => tAxis._,
						},
						Deep: (tInt16)(_[1] - '0')
					)
				)
				.ToArray();
				
				NormalPattern.Add(Row.Select(_ => _.Axis).ToArray());
				DeepPattern.Add(Row.Select(_ => _.Deep).ToArray());
			}
		}		
		
		aRenderEnv.PatternFile.HasUpdated = false;
		
		aRenderEnv._ScalePatterns(aRenderEnv.PatternScale);
		
		return ref aRenderEnv;
	}
	
	public static ref tRenderEnv
	_SetScale(
		this ref tRenderEnv aRenderEnv,
		tInt32 aPatternScale
	) {
		aRenderEnv.PatternScale = aPatternScale;
		return ref aRenderEnv
		._LoadPatterns()
		;
	}
	
	private static ref tRenderEnv
	_ScalePatterns(
		this ref tRenderEnv aRenderEnv,
		tInt32 aScale
	) {
		var Matrixes = aRenderEnv.Matrixes.Map(_ => aScale * _).Map(_ => _);
		var NormalPatterns = aRenderEnv.NormalPatterns.Map(_ => _);
		var DeepPatterns = aRenderEnv.DeepPatterns.Map(_ => _);
		
		var ScaleHalf = aScale >> 1;
		
		var (MaxDir, MaxAngel) = Matrixes.GetSize();
		for (var Angel = 0; Angel < MaxAngel; Angel += 1) {
			for (var Dir = 0; Dir < MaxDir; Dir += 1) {
				var OldDeepPattern = aRenderEnv.DeepPatterns[Dir, Angel];
				var OldNormalPattern = aRenderEnv.NormalPatterns[Dir, Angel];
				var OldPatternSize = OldNormalPattern.GetSize();
				
				var M = aRenderEnv.Matrixes[Dir, Angel];
				var NewPatternSize = GetSpriteSize(V3(aScale), M);
				
				var NewDeepPattern = NewPatternSize.CreateArray<tInt16>()._Map(_ => tInt16.MaxValue);
				var NewNormalPattern = NewPatternSize.CreateArray<tAxis>();
				
				var OffsetUV = V3((NewPatternSize - OldPatternSize) >> 1, 0);
				
				for (var Z = -ScaleHalf; Z <= ScaleHalf; Z += 1) {
					for (var Y = -ScaleHalf; Y <= ScaleHalf; Y += 1) {
						for (var X = -ScaleHalf; X <= ScaleHalf; X += 1) {
							var Offset = V3(X, Y, Z) * M + OffsetUV;
							for (var V = 0; V < OldPatternSize.Y; V += 1) {
								for (var U = 0; U < OldPatternSize.X; U += 1) {
									var OldDeep = OldDeepPattern[U, V];
									ref var pNewDeep = ref NewDeepPattern[U + Offset.X, V + Offset.Y];
									if (OldDeep + Offset.Z <= pNewDeep) {
										pNewDeep = (tInt16)(OldDeep + Offset.Z);
										NewNormalPattern[U + Offset.X, V + Offset.Y] = OldNormalPattern[U, V];
									}
								}
							}
						}
					}
				}
				NormalPatterns[Dir, Angel] = NewNormalPattern;
				DeepPatterns[Dir, Angel] = NewDeepPattern;
			}
		}
		
		aRenderEnv.Matrixes = Matrixes;
		aRenderEnv.NormalPatterns = NormalPatterns;
		aRenderEnv.DeepPatterns = DeepPatterns;
		
		return ref aRenderEnv;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref tRenderEnv
	_SetLightDirection(
		this ref tRenderEnv a,
		tV3 aLightDirection
	) {
		a.LightDirection = GetMainAxis(aLightDirection) switch {
			(tAxis.X, int Sign) => V3(Sign * 9, aLightDirection.Y, aLightDirection.Z),
			(tAxis.Y, int Sign) => V3(aLightDirection.X, Sign * 9, aLightDirection.Z),
			(tAxis.Z, int Sign) => V3(aLightDirection.X, aLightDirection.Y, Sign * 9),
			_ => aLightDirection,
		};
		
		return ref a;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref tRenderEnv
	_Update(
		this ref tRenderEnv a
	) {
		if (a.HotReloat.HasNewDLL) {
			a.HotReloat._LoadDLL();
		}
		if (a.PatternFile.HasUpdated) {
			a._LoadPatterns();
		}
		var (QuarterParts, AngleParts) = a.Matrixes.GetSize();
		while (a.Dir < 0) { a.Dir += 4 * QuarterParts; }
		while (a.Dir >= 4 * QuarterParts) { a.Dir -= 4 * QuarterParts; }
		a.Angle.Clamp(0, AngleParts - 1);
		
		a.M = a.GetMatrix(a.Dir, a.Angle);
		a.NormalPattern = a.NormalPatterns[a.Dir % QuarterParts, a.Angle];
		a.DeepPattern = a.DeepPatterns[a.Dir % QuarterParts, a.Angle];
		
		(a.InvM, a.Det) = a.M.Inverse();
		return ref a;
	}
	
	public enum
	tDebugRenderMode {
		None,
		Normal,
		Deep,
		Pos,
		PosBits,
		Pattern,
	};
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tColor
	RGB(
		tNat8 aR,
		tNat8 aG,
		tNat8 aB
	) => RGBA(aR, aG, aB, false);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tColor
	RGBA(
		tNat8 aR,
		tNat8 aG,
		tNat8 aB,
		tBool aIsTransparent
	) => new tColor {
		Value =  (tNat8)(
			(
				aIsTransparent
				? 0b_1000_0000
				: 0
			) | (
				(aR * 5 + aG) * 5 + aB + 1
			)
		)
	};
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tShadow
	CreateShadow(
		tV2 aSize,
		tV2 aOffset
	) => new tShadow {
		Size = aSize,
		Offset = aOffset,
		Deep = aSize.CreateArray<tInt16>(),
	};
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tSprite
	CreateSprite(
		tV2 aSize,
		tV2 aOffset
	) => new tSprite {
		Size = aSize,
		Offset = aOffset,
		Color = aSize.CreateArray<tColor>(),
		Deep = aSize.CreateArray<tInt16>(),
		Normal = aSize.CreateArray<(tInt8 U, tInt8 V)>(),
		PosBits = aSize.CreateArray<tNat8>(),
	};
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	GetSpriteSize(
		tV3 aBlockSize,
		tM3x3 aM
	) {
		var D = aBlockSize;
		var V3s = new [] {
			V3(+D.X, +D.Y, +D.Z),
			V3(+D.X, +D.Y, -D.Z),
			V3(+D.X, -D.Y, +D.Z),
			V3(+D.X, -D.Y, -D.Z),
			V3(-D.X, +D.Y, +D.Z),
			V3(-D.X, +D.Y, -D.Z),
			V3(-D.X, -D.Y, +D.Z),
			V3(-D.X, -D.Y, -D.Z),
		};
		var MaxX = 0;
		var MaxY = 0;
		foreach (var V3 in V3s) {
			var V2 = V3 * aM;
			MaxX = mMath.Max(MaxX, V2.X);
			MaxY = mMath.Max(MaxY, V2.Y);
		}
		return V2(MaxX + 2, MaxY + 2);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	GetShadowSize(
		tV3 aBlockSize,
		tV3 aLightDirection
	) {
		var (MainAxis, _) = GetMainAxis(aLightDirection);
		
		var L = V3(
			aLightDirection.X.IAbs(),
			aLightDirection.Y.IAbs(),
			aLightDirection.Z.IAbs()
		);
		
		var UV = default(tV2);
		
		switch (MainAxis) {
			case tAxis.X: {
				UV = aBlockSize.ZY() + L.ZY() * aBlockSize.X / L.X;
				break;
			}
			case tAxis.Y: {
				UV = aBlockSize.XZ() + L.XZ() * aBlockSize.Y / L.Y;
				break;
			}
			case tAxis.Z: {
				UV = aBlockSize.XY() + L.XY() * aBlockSize.Z / L.Z; 
				break;
			}
		}
		
		return UV + V2(2);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tShadow
	_Clear(
		this tShadow aGrid
	) {
		var MaxX = aGrid.Size.X;
		var MaxY = aGrid.Size.Y;
		
		for (var X = 0; X < MaxX; X += 1) {
			for (var Y = 0; Y < MaxY; Y += 1) {
				aGrid.Deep[X, Y] = short.MaxValue;
			}
		}
		
		return aGrid;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tSprite
	_Clear(
		this tSprite aGrid
	) {
		var MaxX = aGrid.Size.X;
		var MaxY = aGrid.Size.Y;
		
		for (var X = 0; X < MaxX; X += 1) {
			for (var Y = 0; Y < MaxY; Y += 1) {
				aGrid.Color[X, Y] = default;
				aGrid.Deep[X, Y] = short.MaxValue;
			}
		}
		
		return aGrid;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (tAxis Axis, tInt32 Sign)
	GetMainAxis(
		tV3 aDirection
	) {
		if (
			mMath.Abs(aDirection.Z) >= mMath.Abs(aDirection.X) &&
			mMath.Abs(aDirection.Z) >= mMath.Abs(aDirection.Y)
		) {
			return (tAxis.Z, mMath.Sign(aDirection.Z));
		} else if (
			mMath.Abs(aDirection.Y) >= mMath.Abs(aDirection.X)
		) {
			return (tAxis.Y, mMath.Sign(aDirection.Y));
		} else {
			return (tAxis.X, mMath.Sign(aDirection.X));
		}
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	ToRGB(
		tColor aColor
	) {
		if (aColor == default) {
			return default;
		}
		aColor.Value -= 1;
		return V3(
			(aColor.Value / 5 / 5) % 5,
			(aColor.Value / 5) % 5,
			aColor.Value % 5
		) * 63;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tNat32
	ToRGB32(
		tNat8 aColor
	) {
		if (aColor == 0) {
			return 0;
		}
		aColor -= 1;
		var B = 63 * (aColor % 5);
		var G = 63 * ((aColor / 5) % 5);
		var R = 63 * ((aColor / 5 / 5) % 5);
		var A = tNat8.MaxValue;
		return (tNat32)((A << 24) | (R << 16) | (G << 8) | B);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetNormal3D(
		(tInt8 U, tInt8 V) a
	) => V3(
		a.U,
		-a.V,
		-FastSqrt(mMath.Abs(127*127 - a.U*a.U - a.V*a.V))
	);
}
