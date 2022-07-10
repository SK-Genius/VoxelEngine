using System.Linq;
using System.Runtime.CompilerServices;

using static mStd;
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
		tBlock aBlock
	) {
		if (aRenderEnv.ShadowBuffer.TryGetValue((aRenderEnv.LightDirection, aBlock), out var Shadow)) {
			return Shadow;
		}
		
		var ShadowSize = GetShadowSize(aBlock.GetSize(), aRenderEnv.LightDirection);
		Shadow = CreateShadow(
			ShadowSize,
			mMath2D.V2()
		)
		._Clear()
		._DrawBlock(aBlock, V3(), aRenderEnv.LightDirection);
		
		aRenderEnv.ShadowBuffer[(aRenderEnv.LightDirection, aBlock)] = Shadow;
		return Shadow;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tM3x3
	GetMatrix(
		this tRenderEnv aEnv,
		tInt32 aDir,
		tInt32 aAngle
	) {
		var M = Matrixes[aDir % Matrixes.GetSize().X, aAngle];
		while (aDir >= cQuarterParts) {
			M *= cRotRight;
			aDir -= cQuarterParts;
		}
		return M;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tAxis[,]
	GetNormalPattern(
		this tRenderEnv aEnv
	) {
		var P = NormalPatterns[aEnv.Dir % NormalPatterns.GetSize().X, aEnv.Angle];
		if (
			aEnv.Dir.IsInRange(0*cQuarterParts, 1*cQuarterParts-1) ||
			aEnv.Dir.IsInRange(2*cQuarterParts, 3*cQuarterParts-1)
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
		tV3 aLightDirection
	) {
		var BlockSize = aBlock.Colors.GetSize();
		var BlockSizeHalf = BlockSize >> 1;
		
		var Center = aShadow.Size >> 1;
		
		for (var Z = 0; Z < BlockSize.Z; Z += 1) {
			var Z_ = Z - BlockSizeHalf.Z + aOffset.Z;
			for (var Y = 0; Y < BlockSize.Y; Y += 1) {
				var Y_ = Y - BlockSizeHalf.Y + aOffset.Y;
				for (var X = 0; X < BlockSize.X; X += 1) {
					var X_ = X - BlockSizeHalf.X + aOffset.X;
					
					var Color = aBlock.Colors[X, Y, Z];
					if (Color == default || Color.IsTransparent()) {
						continue;
					}
					
					var UV = GetShadowUV(V3(X_, Y_, Z_), aLightDirection) + V3(Center, 0);
					
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
		
		var Rot = M3x3(- tV3.cY, tV3.cX, tV3.cZ);
		var QDir = NormalPatterns.GetLength(0);
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
						(aGrid.Color.GetSize() - Max) / 2,
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
								ref var pDeep = ref aGrid.Deep[Des.X, Des.Y];
								if (Des.Z <= pDeep) {
									pDeep = (short)Des.Z;
									
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
		return aGrid;
	}
	
	private static tSprite
	_DrawSprite(
		this tSprite aDes,
		tSprite aSrc,
		tV3 aScreenOffset,
		tNat8[] aBitsMap
	) {
		var SrcMin = mMath2D.V2();
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
		var SrcMin = mMath2D.V2();
		var SrcMax = SrcMin + aSrc.Size;
		
		var DesMin = (aDes.Size >> 1) - (aSrc.Size >> 1) + aOffset2D.XY();
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
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static tInt16
	GetShadowDistance(
		this tRenderEnv aRenderEnv,
		tShadow aShadow,
		tV3 aPos
	) {
		var UV = GetShadowUV(aPos, aRenderEnv.LightDirection) + V3(aShadow.Size >> 1, 0);
		
		if (UV.XY().IsInRange(V2(), aShadow.Size - V2(1))) {
			return (tInt16)(UV.Z - aShadow.Deep[UV.X, UV.Y]);
		} else {
			return tInt16.MinValue; 
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
				
				var ShadowDistance = aRenderEnv.GetShadowDistance(
					aShadow,
					To3D(aRenderEnv, aSprite, V2(X, Y))
				);
				
				var N = aSprite.Normal[X, Y];
				var Normal = mMath3D.V3(
					N.U,
					-N.V,
					mMath.FastSqrt(mMath.Abs(128*128 - N.U*N.U - N.V*N.V)) // * mMath.Sign(N.U) * mMath.Sign(N.V)
				);
				
				var H = 50;
				if (ShadowDistance < 3) {
					H = mMath.Max((Normal * aRenderEnv.LightDirection / 9).Sum(), H);
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
						*XPtr = (tNat32)(
							0x_FF_00_00_00 |
							(aSprite.Deep[X, Y] << 4)
						);
						break;
					}
					case tDebugRenderMode.Normal: {
						*XPtr = (tNat32)(
							0x_FF_00_00_00 |
							((128 + Normal.X) << 16) |
							((128 + Normal.Y) << 8)
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
				var Deep = (aShadow.Deep[X, Y] << 4) & 0xFF;
				*XPtr = (tNat32)(
					0xFF_00_00_00 |
					(Deep << 16) |
					(Deep << 8) |
					(Deep << 0)
				);
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
		
		var BlockSprite = GetOrCreateSprite(aRenderEnv, aBlock);
		aCanvas._DrawSprite(BlockSprite, aOffset * aRenderEnv.M, BitsMap);
		
		var BlockShadow = GetOrCreateShadow(aRenderEnv, aBlock);
		aShadow._DrawShadow(BlockShadow, GetShadowUV(aOffset, aRenderEnv.LightDirection));
		
		return ref aRenderEnv;
	}
	
	public static tRendererDLL
	Create(
	) => new tRendererDLL {
		Matrixes = Matrixes,
		NormalPatterns = NormalPatterns,
		GetMatrix = GetMatrix,
		To3D = To3D,
		_RenderToBuffer = _RenderToBuffer,
		_DrawTo = _DrawTo,
	};
}
