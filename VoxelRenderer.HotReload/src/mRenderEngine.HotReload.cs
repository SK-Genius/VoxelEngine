using System.Collections.Generic;
using System.Diagnostics;

using static mMath;
using static mMath2D;
using static mMath3D;
using static mVoxelRenderer;

public static class
mVoxelRenderer_HotReload {
	
	private const tInt32
	cQuarterParts = 6;
	
	private const tInt32
	cAngleParts = 5;
	
	private static readonly tM3x3[,]
	Matrixes = new tM3x3[cQuarterParts, cAngleParts] {
		{
			M3x3(
				V3( 4, 0, 0),
				V3( 0, 0, 4),
				V3( 0, 4, 0)
			),
			M3x3(
				V3( 4, 0, 0),
				V3( 0, -2, 4),
				V3( 0, 3, 1)
			),
			M3x3(
				V3( 4, 0, 0),
				V3( 0, -3, 3),
				V3( 0, 2, 2)
			),
			M3x3(
				V3( 4, 0, 0),
				V3( 0, -4, 2),
				V3( 0, 1, 3)
			),
			M3x3(
				V3( 4, 0, 0),
				V3( 0, -4, 0),
				V3( 0, 0, 4)
			)
		}, {
			M3x3(
				V3( 4, 1, 0),
				V3( 0, 0, 4),
				V3(-1, 4, 0)
			),
			M3x3(
				V3( 4, 1, 0),
				V3( 0, -2, 4),
				V3(0, 3, 1)
			),
			M3x3(
				V3( 4, 1, 0),
				V3( 1, -3, 3),
				V3(-1, 3, 2)
			),
			M3x3(
				V3( 4, 1, 0),
				V3( 1, -4, 2),
				V3(-1, 3, 3)
			),
			M3x3(
				V3( 4, 1, 0),
				V3( 1, -4, 0),
				V3( 0, 0, 4)
			)
		}, {
			M3x3(
				V3( 4, 2, 0),
				V3( 0, 0, 4),
				V3(-1, 4, 0)
			),
			M3x3(
				V3( 4, 2, 0),
				V3( 1, -1, 4),
				V3(-2, 2, 1)
			),
			M3x3(
				V3( 4, 2, 0),
				V3( 1, -3, 3),
				V3(-1, 3, 2)
			),
			M3x3(
				V3( 4, 2, 0),
				V3( 2, -4, 2),
				V3(-1, 2, 3)
			),
			M3x3(
				V3( 4, 2, 0),
				V3( 2, -4, 0),
				V3( 0, 0, 4)
			)
		}, {
			M3x3(
				V3( 3, 3, 0),
				V3( 0, 0, 4),
				V3(-3, 3, 0)
			),
			M3x3(
				V3( 3, 3, 0),
				V3( 1, -1, 4),
				V3(-2, 2, 1)
			),
			M3x3(
				V3( 3, 3, 0),
				V3( 2, -2, 3),
				V3(-2, 2, 2)
			),
			M3x3(
				V3( 3, 3, 0),
				V3( 3, -3, 2),
				V3(-1, 1, 3)
			),
			M3x3(
				V3( 3, 3, 0),
				V3( 3, -3, 0),
				V3( 0, 0, 4)
			)
		}, {
			M3x3(
				V3( 2, 4, 0),
				V3( 0, 0, 4),
				V3(-4, 1, 0)
			),
			M3x3(
				V3( 2, 4, 0),
				V3( 1, -1, 4),
				V3(-2, 2, 1)
			),
			M3x3(
				V3( 2, 4, 0),
				V3( 3, -1, 3),
				V3(-3, 1, 2)
			),
			M3x3(
				V3( 2, 4, 0),
				V3( 4, -2, 2),
				V3(-2, 1, 3)
			),
			M3x3(
				V3( 2, 4, 0),
				V3( 4, -2, 0),
				V3( 0, 0, 4)
			)
		}, {
			M3x3(
				V3( 1, 4, 0),
				V3( 0, 0, 4),
				V3(-4, 1, 0)
			),
			M3x3(
				V3( 1, 4, 0),
				V3( 2, 0, 4),
				V3(-3, 0, 1)
			),
			M3x3(
				V3( 1, 4, 0),
				V3( 3, -1, 3),
				V3(-3, 1, 2)
			),
			M3x3(
				V3( 1, 4, 0),
				V3( 4, -1, 2),
				V3(-3, 1, 3)
			),
			M3x3(
				V3( 1, 4, 0),
				V3( 4, -1, 0),
				V3( 0, 0, 4)
			)
		}
	};
	
	private static tAxis[,][,] NormalPatterns = new tAxis[cQuarterParts, cAngleParts][,] {
		{
			new tAxis[,] {
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
			},
			new tAxis[,] {
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
			},
			new tAxis[,] {
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
			},
			new tAxis[,] {
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
			},
			new tAxis[,] {
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
			},
		},
		{
			new tAxis[,] {
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis._, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis._, tAxis._},
			},
		},
		{
			new tAxis[,] {
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis._, tAxis._, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis._, tAxis.Z, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis._, tAxis._, tAxis._},
			},
		},
		{
			new tAxis[,] {
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
			},
			new tAxis[,] {
				{tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X, tAxis._},
				{tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis._, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis._, tAxis._},
			},
		},
		{
			new tAxis[,] {
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis._, tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis._, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis._, tAxis.Z, tAxis._, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis._, tAxis._, tAxis._, tAxis.Z, tAxis._, tAxis._},
			},
		},
		{
			new tAxis[,] {
				{tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.X, tAxis.X, tAxis.X, tAxis.X},
			},
			new tAxis[,] {
				{tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X},
				{tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis.X},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis.Y, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis.X, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Y, tAxis.Y, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis.Z, tAxis.X, tAxis.X, tAxis._},
			},
			new tAxis[,] {
				{tAxis._, tAxis.Z, tAxis.Z, tAxis._, tAxis._},
				{tAxis._, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z},
				{tAxis.Z, tAxis.Z, tAxis.Z, tAxis.Z, tAxis._},
				{tAxis._, tAxis._, tAxis.Z, tAxis.Z, tAxis._},
			},
		},
	};
	
	private static tSprite
	GetOrCreateSprite(
		this tRenderEnv aRenderEnv,
		tColor[,,] aCube
	) {
		if (!aRenderEnv.SpriteBuffer.TryGetValue((aRenderEnv.M, aCube), out var Sprite)) {
			var SpriteSize = GetSpriteSize(aCube.GetLength(0), aRenderEnv.M);
			Sprite = CreateSprite(
				SpriteSize,
				mMath2D.V2(0, 0)
			)
			._Clear()
			._DrawCube(aCube, V3(0, 0, 0), aRenderEnv.NormalPattern, aRenderEnv.M);
			
			aRenderEnv.SpriteBuffer[(aRenderEnv.M, aCube)] = Sprite;
		}
		return Sprite;
	}
	
	private static tShadow
	GetOrCreateShadow(
		this tRenderEnv aRenderEnv,
		tColor[,,] aCube
	) {
		if (aRenderEnv.ShadowBuffer.TryGetValue((aRenderEnv.LightDirection, aCube), out var Shadow)) {
			return Shadow;
		}
		
		var ShadowSize = GetShadowSize(aCube.GetSize(), aRenderEnv.LightDirection);
		Shadow = CreateShadow(
			ShadowSize,
			mMath2D.V2(0, 0)
		)
		._Clear()
		._DrawCube(aCube, V3(0, 0, 0), aRenderEnv.LightDirection);
		
		aRenderEnv.ShadowBuffer[(aRenderEnv.LightDirection, aCube)] = Shadow;
		return Shadow;
	}
	
	private static tM3x3
	GetMatrix(
		this tRenderEnv aEnv,
		tInt32 aDir,
		tInt32 aAngle
	) {
		var M = Matrixes[aDir % Matrixes.GetLength(0), aAngle];
		while (aDir >= cQuarterParts) {
			M = M * cRotRight;
			aDir -= cQuarterParts;
		}
		return M;
	}
	
	private static tAxis[,]
	GetNormalPattern(
		this tRenderEnv aEnv
	) {
		var P = NormalPatterns[(int)(aEnv.Dir % NormalPatterns.GetLength(0)), aEnv.Angle];
		if (
			aEnv.Dir.IsInRange(0*cQuarterParts, 1*cQuarterParts-1) ||
			aEnv.Dir.IsInRange(2*cQuarterParts, 3*cQuarterParts-1)
		) {
			return P;
		}
		var MaxX = P.GetLength(0);
		var MaxY = P.GetLength(1);
		var P_ = new tAxis[MaxX, MaxY];
		for (var Y = 0; Y < MaxY; Y += 1) {
			for (var X = 0; X < MaxX; X += 1) {
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
	
	private static tV3
	To3D(
		this tRenderEnv aRenderEnv,
		tSprite aSprite,
		tV2 aV2
	) {
		var Pos = V3(
			aV2 - (aSprite.Size >> 1),
			aSprite.Deep[aV2.X, aV2.Y]
		) * aRenderEnv.InvM / aRenderEnv.Det;
		
		var PosBits = aSprite.PosBits[aV2.X, aV2.Y];
		#if !true
			return Pos;
		#else
			return V3(
				CorrectAxis(Pos.X, (PosBits >> 4) & 0b11),
				CorrectAxis(Pos.Y, (PosBits >> 2) & 0b11),
				CorrectAxis(Pos.Z, (PosBits >> 0) & 0b11)
			);
		#endif
	}
	
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
	_DrawCube(
		this tShadow aShadow,
		tColor[,,] aCube,
		tV3 aOffset,
		tV3 aLightDirection
	) {
		var CubeLength = aCube.GetLength(0);
		var CubeLengthHalf = CubeLength >> 1;
		if (aCube.GetLength(1) != CubeLength) { throw null; }
		if (aCube.GetLength(2) != CubeLength) { throw null; }
		
		var (MainAxis, AxisSign) = GetMainAxis(aLightDirection);
		var ShadowSizeHalf = aShadow.Size >> 1;
		var UCenter = ShadowSizeHalf.X;
		var VCenter = ShadowSizeHalf.Y;
		
		for (var Z = (tInt16)0; Z < CubeLength; Z += 1) {
			var Z_ = Z - CubeLengthHalf + aOffset.Z;
			for (var Y = (tInt16)0; Y < CubeLength; Y += 1) {
				var Y_ = Y - CubeLengthHalf + aOffset.Y;
				for (var X = (tInt16)0; X < CubeLength; X += 1) {
					var X_ = X - CubeLengthHalf + aOffset.X;
					
					var Color = aCube[X, Y, Z];
					if (Color == default) {
						continue;
					}
					
					var UV = GetShadowOffset(V3(X_, Y_, Z_), aLightDirection) + V3(UCenter, VCenter, 0);
					
					ref var pDeep = ref aShadow.Deep[UV.X, UV.Y];
					pDeep = Min(pDeep, (tInt16)UV.Z);
				}
			}
		}
		
		return aShadow;
	}
	
	private static tSprite
	_DrawCube(
		this tSprite aGrid,
		tColor[,,] aCube,
		tV3 aOffset,
		tAxis[,] aNormal,
		tM3x3 aMatrix
	) {
		var CubeLength = aCube.GetLength(0);
		var CubeLengthHalf = CubeLength >> 1;
		if (aCube.GetLength(1) != CubeLength) { throw null; }
		if (aCube.GetLength(2) != CubeLength) { throw null; }
		
		var MaxU = aNormal.GetLength(0);
		var MaxV = aNormal.GetLength(1);
		
		for (var Z = (short)0; Z < CubeLength; Z += 1) {
			for (var Y = 0; Y < CubeLength; Y += 1) {
				for (var X = 0; X < CubeLength; X += 1) {
					var Color = aCube[X, Y, Z];
					if (Color == default) {
						continue;
					}
					
					var Des_ = (
						V3(
							X - CubeLengthHalf,
							Y - CubeLengthHalf,
							Z - CubeLengthHalf
						) + aOffset
					) * aMatrix + V3(
						(aGrid.Color.GetLength(0) - MaxU) / 2,
						(aGrid.Color.GetLength(1) - MaxV) / 2,
						0
					);
					
					for (var V = 0; V < MaxV; V += 1) {
						for (var U = 0; U < MaxU; U += 1) {
							var Axis = aNormal[U, V];
							var Des = Des_ + V3(U, V, 0);
							if ((Color & 0b_1000_0000) != default && ((Des.X ^ Des.Y) & 1) == 0) {
								continue;
							}
							if (Axis != tAxis._ && Des.X >= 0 && Des.Y >= 0) {
								ref var pColor = ref aGrid.Color[Des.X, Des.Y];
								ref var pDeep = ref aGrid.Deep[Des.X, Des.Y];
								ref var pNormal =ref aGrid.Normal[Des.X, Des.Y];
								ref var pPosBits =ref aGrid.PosBits[Des.X, Des.Y];
								if (Des.Z <= pDeep) {
									pColor = Color & 0b_0111_1111;
									pDeep = (short)Des.Z;
									var Normal = Axis switch {
										tAxis.X => V3(127, 0, 0),
										tAxis.Y => V3(0, 127, 0),
										tAxis.Z => V3(0, 0, 127),
										tAxis._ => default,
									};
									pNormal = ((tInt8)Normal.X, (tInt8)Normal.Y);
									pPosBits = (tNat8)(((X & 0b11) << 4) | ((Y & 0b11) << 2) | ((Z & 0b11) << 0));
								}
							}
						}
					}
				}
			}
		}
		return aGrid;
	}
	
	private static tSprite
	_DrawSprite(
		this tSprite aDes,
		tSprite aSrc,
		tV3 aScreenOffset,
		tNat8[] aBitsMap
	) {
		var SrcMin = mMath2D.V2(0, 0);
		var SrcSize = mMath2D.V2(aSrc.Color.GetLength(0), aSrc.Color.GetLength(1));
		var SrcMax = SrcMin + SrcSize;
		
		var DesSize = mMath2D.V2(aDes.Color.GetLength(0), aDes.Color.GetLength(1));
		var DesMin = ((DesSize - SrcSize) >> 1) + V2(aScreenOffset);
		var DesMax = DesMin + SrcSize;
		var DesMaxY = aSrc.Color.GetLength(1);
		
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
					aDes.Color[DesX, DesY] = aSrc.Color[SrcX, SrcY];
					pDesDeep = (tInt16)SrcDeep;
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
		var SrcMin = mMath2D.V2(0, 0);
		var SrcMax = SrcMin + aSrc.Size;
		
		var DesMin = ((aDes.Size - aSrc.Size) >> 1) + V2(aOffset2D);
		var DesMax = DesMin + aSrc.Size;
		var DesMaxY = aSrc.Deep.GetLength(1);
		
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
			var R = -DesMin.X;
			DesMin.X = 0;
			SrcMin.X += R;
			SrcSize.X -= R;
		}
		if (DesMin.Y < 0) {
			var R = -DesMin.Y;
			DesMin.Y = 0;
			SrcMin.Y += R;
			SrcSize.Y -= R;
		}
		if (DesMax.X > aDes.Size.X) {
			var R = DesMax.X - aDes.Size.X;
			SrcSize.X -= R;
			DesMax.X = aDes.Size.X;
		}
		if (DesMax.Y > aDes.Size.Y) {
			var R = DesMax.Y - aDes.Size.Y;
			SrcSize.Y -= R;
			DesMax.Y = aDes.Size.Y;
		}
		
		for (var YOffset = 0; YOffset < SrcSize.Y; YOffset += 1) {
			for (var XOffset = 0; XOffset < SrcSize.X; XOffset += 1) {
				var SrcDeep = aSrc.Deep[SrcMin.X + XOffset, SrcMin.Y + YOffset];
				if (SrcDeep != tInt16.MaxValue) {
					ref var pDesDeep = ref aDes.Deep[DesMin.X + XOffset, DesMin.Y + YOffset];
					pDesDeep = mMath.Min(
						(tInt16)(SrcDeep + aOffset2D.Z),
						pDesDeep
					);
				}
			}
		}
		
		return aDes;
	}
	
	private static tV3
	GetShadowOffset(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var (MainAxis, AxisSign) = GetMainAxis(aLightDirection);
		
		var Deep = default(tInt16);
		var U = default(tInt32);
		var V = default(tInt32);
		
		switch (MainAxis) {
			case tAxis.X: {
				U = Z_ + aLightDirection.Z * X_ / aLightDirection.X;
				V = Y_ + aLightDirection.Y * X_ / aLightDirection.X;
				Deep = (tInt16)(AxisSign * X_);
				break;
			}
			case tAxis.Y: {
				U = X_ + aLightDirection.X * Y_ / aLightDirection.Y;
				V = Z_ + aLightDirection.Z * Y_ / aLightDirection.Y;
				Deep = (tInt16)(AxisSign * Y_);
				break;
			}
			case tAxis.Z: {
				U = X_ + aLightDirection.X * Z_ / aLightDirection.Z;
				V = Y_ + aLightDirection.Y * Z_ / aLightDirection.Z;
				Deep = (tInt16)(AxisSign * Z_);
				break;
			}
		}
		
		return V3(U, V, Deep);
	}
	
	private static tInt16
	GetShadowDistance( // TODO: remove code duplication
		this tRenderEnv aRenderEnv,
		tShadow aShadow,
		tV3 aPos
	) {
		var X_ = aPos.X;
		var Y_ = aPos.Y;
		var Z_ = aPos.Z;
		
		var aLightDirection = aRenderEnv.LightDirection;
		var (MainAxis, AxisSign) = GetMainAxis(aLightDirection);
		
		var UV = GetShadowOffset(aPos, aRenderEnv.LightDirection) + V3(aShadow.Size.X >> 1, aShadow.Size.Y >> 1, 0);
		var U = UV.X;
		var V = UV.Y;
		
		if (
			0 <= U && U < aShadow.Size.X &&
			0 <= V && V < aShadow.Size.Y
		) {
			return (tInt16)(UV.Z - aShadow.Deep[U, V]);
		} else {
			return tInt16.MinValue; 
		}
	}
	
	private static unsafe tRenderEnv
	_RenderToBuffer(
		this tRenderEnv aRenderEnv,
		tSprite aSprite,
		tShadow aShadow,
		System.IntPtr aBuffer,
		tV2 aBufferSize,
		tDebugRenderMode aDebugRenderMode
	) {
		var MaxX = mMath.Min(aSprite.Color.GetLength(0), aBufferSize.X);
		var MaxY = mMath.Min(aSprite.Color.GetLength(1), aBufferSize.Y);
		var DeltaX = aBufferSize.X - MaxX;
		
		var YPtr = (uint*)aBuffer;
		for (var Y = 0; Y < MaxY; Y += 1) {
			var XPtr = YPtr;
			for (var X = 0; X < MaxX; X += 1) {
				var ColorIndex = aSprite.Color[X, Y];
				if (ColorIndex == default) {
					XPtr += 1;
					continue;
				}
				
				var ShadowDistance = aRenderEnv.GetShadowDistance(
					aShadow,
					To3D(aRenderEnv, aSprite, V2(X, Y))
				);
				
				var N = aSprite.Normal[X, Y];
				var Normal = mMath3D.V3(
					N.U,
					N.V,
					mMath.Max(128 - mMath.Abs(N.U) - mMath.Abs(N.V), 0)
				);
				var H = mMath.Max((Normal * aRenderEnv.LightDirection / 128).Sum(), 0);
				if (ShadowDistance > 0) {
					H >>= 1;
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
					case tDebugRenderMode.Deep: {
						*XPtr = (tNat32)(0x_FF_00_00_00 | (aSprite.Deep[X, Y] << 4));
						break;
					}
					case tDebugRenderMode.Normal: {
						*XPtr = (tNat32)(0x_FF_00_00_00 | ((128 + Normal.X) << 16) | ((128 + Normal.Y) << 8));
						break;
					}
					case tDebugRenderMode.PosBits: {
						var PosBits = aSprite.PosBits[X, Y];
						*XPtr = (tNat32)(0x_FF_00_00_00 | ((PosBits & 0b110000) << 17) | ((PosBits & 0b001100) << 11) | ((PosBits & 0b000011) << 5));
						break;
					}
					case tDebugRenderMode.Pos: {
						var Pos = To3D(aRenderEnv, aSprite, V2(X, Y));
						*XPtr = (tNat32)(0x_FF_00_00_00 | ((Pos.X & 0b0111) << 20) | ((Pos.Y & 0b0111) << 12) | ((Pos.Z & 0b0111) << 4));
						break;
					}
				}
				
				XPtr += 1;
			}
			YPtr += aBufferSize.X;
		}
		return aRenderEnv;
	}
	
	private static unsafe tShadow
	_RenderToBuffer(
		this tShadow aShadow,
		System.IntPtr aBuffer,
		mMath2D.tV2 aBufferSize,
		mMath2D.tV2 aOffset
	) {
		var MaxX = mMath.Min(aShadow.Size.X, aBufferSize.X);
		var MaxY = mMath.Min(aShadow.Size.Y, aBufferSize.Y);
		
		var YPtr = (tNat32*)aBuffer + aOffset.Y * aBufferSize.X;
		for (var Y = 0; Y < MaxY; Y += 1) {
			var XPtr = YPtr + aOffset.X;
			for (var X = 0; X < MaxX; X += 1) {
				var Deep = aShadow.Deep[X, Y];
				if (Deep != tInt16.MaxValue) {
					*XPtr = (tNat32)(
						(0xFF << 24) |
						((Deep & 0xFF) << 16) |
						((Deep & 0xFF) << 8) |
						((Deep & 0xFF) << 0)
					);
				} else {
					*XPtr = 0x_FF_FF_00_FF;
				}
				XPtr += 1;
			}
			YPtr += aBufferSize.X;
		}
		return aShadow;
	}
	
	private static unsafe tRenderEnv
	_DrawTo(
		this tRenderEnv aRenderEnv,
		tSprite aCanvas,
		tShadow aShadow,
		tColor[,,] aCube,
		tV3 aOffset
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
						(((X + aOffset.X) & 0b11) << 4) |
						(((Y + aOffset.Y) & 0b11) << 2) |
						(((Z + aOffset.Z) & 0b11) << 0)
					);
				}
			}
		}
		
		var CubeSprite = GetOrCreateSprite(aRenderEnv, aCube);
		aCanvas._DrawSprite(CubeSprite, aOffset * aRenderEnv.M, BitsMap);
		
		var CubeShadow = GetOrCreateShadow(aRenderEnv, aCube);
		//aShadow._DrawShadow(CubeShadow, aRenderEnv.LightDirection);	
		
		return aRenderEnv;
	}
		
	public static tDLL
	Create(
	) => new tDLL {
		Matrixes = Matrixes,
		NormalPatterns = NormalPatterns,
		GetOrCreateSprite = GetOrCreateSprite,
		GetOrCreateShadow = GetOrCreateShadow,
		GetMatrix = GetMatrix,
		To3D = To3D,
		_RenderToBuffer = _RenderToBuffer,
		_DrawTo = _DrawTo,
	};
}
