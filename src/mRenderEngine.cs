using System.Collections.Generic;
using static mMath;
using static mMath2D;
using static mMath3D;

public static class
mRenderEngine {
	
	public const tInt32
	cQuarterParts = 6;
	
	public const tInt32
	cAngleParts = 5;
	
	[System.Flags]
	public enum
	tAxis {
		_ = 0,
		X = 1,
		Y = 2,
		Z = 4,
	};
	
	public sealed class
	tRenderEnv {
		public tInt32 Dir;
		public tInt32 Angle;
		
		public tV3 LightDirection;
		
		public tM3x3 M;
		public tM3x3 InvM;
		public tInt32 Det;
		
		public tAxis[,] NormalPattern;

		public tM3x3[,] Ms;
		public tAxis[,][,] NormalPatterns;
		
		public Dictionary<(mMath3D.tM3x3, tColor[,,]), mRenderEngine.tSprite> SpriteBuffer  = new();
		public Dictionary<(tV3, tColor[,,]), mRenderEngine.tShadow> ShadowBuffer  = new();
	}
	
	public struct
	tColor {
		public tNat8 Value;
		
		public static tBool
		operator==(
			tColor a1,
			tColor a2
		) => a1.Value == a2.Value;
		
		public static tBool
		operator!=(
			tColor a1,
			tColor a2
		) => a1.Value != a2.Value;
		
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
		public short[,] Deep;
	}
	
	public struct
	tSprite {
		public tV2 Size;
		public tV2 Offset;
		public tColor[,] Color;
		public (tInt8 U, tInt8 V)[,] Normal;
		public short[,] Deep;
	}
	
	public static mRenderEngine.tSprite
	GetOrCreateSprite(
		this tRenderEnv aRenderEnv,
		tColor[,,] aCube
	) {
		if (!aRenderEnv.SpriteBuffer.TryGetValue((aRenderEnv.M, aCube), out var Sprite)) {
			var SpriteSize = mRenderEngine.GetSpriteSize(aCube.GetLength(0), aRenderEnv.M);
			Sprite = mRenderEngine.CreateSprite(
				SpriteSize,
				mMath2D.V2(0, 0)
			)
			._Clear()
			._DrawCube(aCube, aRenderEnv.NormalPattern, aRenderEnv.M);
			
			aRenderEnv.SpriteBuffer[(aRenderEnv.M, aCube)] = Sprite;
		}
		return Sprite;
	}
	
	public static tShadow
	CreateShadow(
		tV2 aSize,
		tV2 aOffset
	) => new tShadow {
		Size = aSize,
		Offset = aOffset,
		Deep = new short[(int)aSize.X, (int)aSize.Y],
	};
	
	public static tSprite
	CreateSprite(
		tV2 aSize,
		tV2 aOffset
	) => new tSprite {
		Size = aSize,
		Offset = aOffset,
		Color = new tColor[(int)aSize.X, (int)aSize.Y],
		Deep = new short[(int)aSize.X, (int)aSize.Y],
		Normal = new (tInt8 U, tInt8 V)[(int)aSize.X, (int)aSize.Y],
	};
	
	static readonly tM3x3[,]
	_Matrix__ = new tM3x3[(int)cQuarterParts, (int)cAngleParts] {
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
				V3(-1, 3, 1)
			),
			M3x3(
				V3( 4, 1, 0),
				V3( 1, -3, 3),
				V3(-1, 2, 2)
			),
			M3x3(
				V3( 4, 1, 0),
				V3( 1, -4, 2),
				V3(-1, 1, 3)
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
				V3(-1, 3, 1)
			),
			M3x3(
				V3( 4, 2, 0),
				V3( 1, -3, 3),
				V3(-1, 2, 2)
			),
			M3x3(
				V3( 4, 2, 0),
				V3( 2, -4, 2),
				V3(-1, 1, 3)
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
				V3(-3, 1, 1)
			),
			M3x3(
				V3( 2, 4, 0),
				V3( 3, -1, 3),
				V3(-2, 1, 2)
			),
			M3x3(
				V3( 2, 4, 0),
				V3( 4, -2, 2),
				V3(-1, 1, 3)
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
				V3(-3, 1, 1)
			),
			M3x3(
				V3( 1, 4, 0),
				V3( 3, -1, 3),
				V3(-2, 1, 2)
			),
			M3x3(
				V3( 1, 4, 0),
				V3( 4, -1, 2),
				V3(-1, 1, 3)
			),
			M3x3(
				V3( 1, 4, 0),
				V3( 4, -1, 0),
				V3( 0, 0, 4)
			)
		}
	};
	
	static tM3x3
	GetMatrix(
		this tRenderEnv aEnv,
		tInt32 aDir,
		tInt32 aAngle
	) {
		var M = aEnv.Ms[aDir % aEnv.Ms.GetLength(0), aAngle];
		while (aDir >= cQuarterParts) {
			M = M * cRotRight;
			aDir -= cQuarterParts;
		}
		return M;
	}
	
	public static tRenderEnv
	CreateEnv(
	) => new tRenderEnv {
		Ms = _Matrix__,
		NormalPatterns = NormalPatterns__,
	};
	
	static tAxis[,][,] NormalPatterns__ = new tAxis[cQuarterParts, cAngleParts][,] {
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
	
	static tAxis[,]
	GetNormalPattern(
		this tRenderEnv aEnv,
		tInt32 aDir,
		tInt32 aAngle
	) {
		var P = aEnv.NormalPatterns[(int)(aDir % aEnv.NormalPatterns.GetLength(0)), (int)aAngle];
		if (
			aDir.IsInRange(0*cQuarterParts, 1*cQuarterParts-1) ||
			aDir.IsInRange(2*cQuarterParts, 3*cQuarterParts-1)
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
	
	public static void
	Update(
		this tRenderEnv a
	) {
		while (a.Dir < 0) { a.Dir += 4 * cQuarterParts; }
		while (a.Dir >= 4 * cQuarterParts) { a.Dir -= 4 * cQuarterParts; }
		mMath.Clamp(ref a.Angle, 0, 4);
		a.M = a.GetMatrix(a.Dir, a.Angle);
		(a.InvM, a.Det) = mMath3D.Inverse(a.M);
		a.NormalPattern = a.GetNormalPattern(a.Dir, a.Angle);
	}
	
	public static tV3
	To3D(
		this tRenderEnv aRenderEnv,
		tSprite aGrid,
		tV2 aV2
	) {
		var Z = aGrid.Deep[aV2.X, aV2.Y];
		if (Z == tInt16.MaxValue) {
			return V3(0, 0, tInt16.MaxValue);
		} else {
			return V3(
				aV2 - aGrid.Size / 2,
				Z
			) * aRenderEnv.InvM / aRenderEnv.Det;
		}
	}
	
	public static tAxis
	GetMainAxis(
		tV3 aDirection
	) {
		if (aDirection.Z >= aDirection.X && aDirection.Z >= aDirection.Y) {
			return tAxis.Z;
		} else if (aDirection.Y >= aDirection.X) {
			return tAxis.Y;
		} else {
			return tAxis.X;
		}
	}
	
	public static tV2
	GetDeepSize(
		tV3 LightDirection,
		tInt32 aCubeSize
	) {
		var V = LightDirection * aCubeSize;
		return V2(aCubeSize, aCubeSize) + GetMainAxis(LightDirection) switch {
			tAxis.Z => V2(V.X / LightDirection.Z, V.Y / LightDirection.Z),
			tAxis.Y => V2(V.X / LightDirection.Y, V.Z / LightDirection.Y),
			tAxis.X => V2(V.Z / LightDirection.X, V.Y / LightDirection.X),
		};
	}	
	
	public static tV2
	GetSpriteSize(
		tInt32 aCubeSize,
		mMath3D.tM3x3 aM
	) {
		var D = aCubeSize;
		var V3s = new [] {
			V3(+D, +D, +D),
			V3(+D, +D, -D),
			V3(+D, -D, +D),
			V3(+D, -D, -D),
			V3(-D, +D, +D),
			V3(-D, +D, -D),
			V3(-D, -D, +D),
			V3(-D, -D, -D),
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
	
	public static tShadow
	_DrawCube(
		this tShadow aGrid,
		tNat8[,,] aCube,
		tV3 aLightDirection
	) {
		var CubeLength = aCube.GetLength(0);
		var CubeLengthHalf = CubeLength >> 1;
		if (aCube.GetLength(1) != CubeLength) { throw null; }
		if (aCube.GetLength(2) != CubeLength) { throw null; }
		
		var MainAxis_ = GetMainAxis(aLightDirection);
		
		for (var Z = (tInt16)0; Z < CubeLength; Z += 1) {
			for (var Y = (tInt16)0; Y < CubeLength; Y += 1) {
				for (var X = (tInt16)0; X < CubeLength; X += 1) {
					var Color = aCube[X, Y, Z];
					if (Color == 0) {
						continue;
					}
					
					switch (MainAxis_) {
						case tAxis.X: {
							ref var pP = ref aGrid.Deep[Z, Y];
							pP = Max(pP, X);
							break;
						}
						case tAxis.Y: {
							ref var pP = ref aGrid.Deep[X, Z];
							pP = Max(pP, Y);
							break;
						}
						case tAxis.Z: {
							ref var pP = ref aGrid.Deep[X, Y];
							pP = Max(pP, Z);
							break;
						}
					}
				}
			}
		}
		
		return aGrid;
	}
	
	public static tSprite
	_DrawCube(
		this tSprite aGrid,
		tColor[,,] aCube,
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
					
					var Des_ = V3(
						X - CubeLengthHalf,
						Y - CubeLengthHalf,
						Z - CubeLengthHalf
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
								if (Des.Z <= pDeep) {
									pColor = Color & 0b_0111_1111;
									pDeep = (short)Des.Z;
									var Normal = Axis switch {
										tAxis.X => V3(127, 0, 0),
										tAxis.Y => V3(0, 127, 0),
										tAxis.Z => V3(0, 0, 127),
										tAxis._ => V3(0, 0, 0),
									} * aMatrix / 8; // TODO: remove 8
									if (Normal.Z < 0) {
										Normal *= -1;
									}
									pNormal = ((tInt8)Normal.X, (tInt8)Normal.Y);
								}
							}
						}
					}
				}
			}
		}
		return aGrid;
	}
	
	public static tSprite
	_DrawSprite(
		this tSprite aDes,
		tSprite aSrc,
		tV3 aOffset2D
	) {
		var SrcMin = mMath2D.V2(0, 0);
		var SrcSize = mMath2D.V2(aSrc.Color.GetLength(0), aSrc.Color.GetLength(1));
		var SrcMax = SrcMin + SrcSize;
		
		var DesSize = mMath2D.V2(aDes.Color.GetLength(0), aDes.Color.GetLength(1));
		var DesMin = (DesSize - SrcSize) / 2 + V2(aOffset2D);
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
				var SrcDeep = aSrc.Deep[SrcX, SrcY] + aOffset2D.Z;
				ref var pDesDeep = ref aDes.Deep[DesX, DesY];
				if (SrcDeep <= pDesDeep) {
					aDes.Color[DesX, DesY] = aSrc.Color[SrcX, SrcY];
					pDesDeep = (tInt16)SrcDeep;
					aDes.Normal[DesX, DesY] = aSrc.Normal[SrcX, SrcY];
				}
			}
		}
		
		return aDes;
	}
	
	public static tColor
	RGB(
		tNat8 aR,
		tNat8 aG,
		tNat8 aB
	) => RGBA(aR, aG, aB, false);
	
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
	
	public static tV3
	ToRGB(
		tColor aColor
	) {
		if (aColor == default) {
			return default;
		}
		aColor.Value -= 1;
		return mMath3D.V3(
			(aColor.Value / 5 / 5) % 5,
			(aColor.Value / 5) % 5,
			aColor.Value % 5
		) * 63;
	}
	
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
	
	public static unsafe tSprite
	_RenderToBuffer(
		this tSprite aSprite,
		System.IntPtr aBuffer,
		tV2 aSize,
		tV3 aLightVector
	) {
		var MaxX = mMath.Min(aSprite.Color.GetLength(0), aSize.X);
		var MaxY = mMath.Min(aSprite.Color.GetLength(1), aSize.Y);
		var DeltaX = aSize.X - MaxX;
		
		var YPtr = (uint*)aBuffer;
		for (var Y = 0; Y < MaxY; Y += 1) {
			var XPtr = YPtr;
			for (var X = 0; X < MaxX; X += 1) {
				var ColorIndex = aSprite.Color[X, Y];
				if (ColorIndex != default) {
					var N = aSprite.Normal[X, Y];
					var Normal = mMath3D.V3(
						N.U,
						N.V,
						mMath.Max(128 - mMath.Abs(N.U) - mMath.Abs(N.V), 0)
					);
					var H = mMath.Max((Normal * aLightVector / 128).Sum(), 0);
					var C = ToRGB(ColorIndex) * H / 256;
					*XPtr = (tNat32)(
						(0xFF << 24) |
						((C.X & 0xFF) << 16) |
						((C.Y & 0xFF) << 8) |
						((C.Z & 0xFF) << 0)
					);
				}
				XPtr += 1;
			}
			YPtr += aSize.X;
		}
		return aSprite;
	}
}
