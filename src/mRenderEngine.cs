using System.Collections.Generic;
using System.Diagnostics;

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
	
	[DebuggerDisplay("{Value}")]
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
	
	public static tRenderEnv
	_SetLightDirection(
		this tRenderEnv a,
		mMath3D.tV3 aLightDirection
	) {
		a.LightDirection = aLightDirection;
		return a;
	}
	
	public static tSprite
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
	
	public static mRenderEngine.tShadow
	GetOrCreateShadow(
		this tRenderEnv aRenderEnv,
		tColor[,,] aCube
	) {
		if (aRenderEnv.ShadowBuffer.TryGetValue((aRenderEnv.LightDirection, aCube), out var Shadow)) {
			return Shadow;
		}
		
		var ShadowSize = mRenderEngine.GetShadowSize(aCube.GetLength(0), aRenderEnv.LightDirection);
		Shadow = mRenderEngine.CreateShadow(
			ShadowSize,
			mMath2D.V2(0, 0)
		)
		._Clear()
		._DrawCube(aCube, aRenderEnv.LightDirection);
		
		aRenderEnv.ShadowBuffer[(aRenderEnv.LightDirection, aCube)] = Shadow;
		return Shadow;
	}
	
	public static tShadow
	CreateShadow(
		tV2 aSize,
		tV2 aOffset
	) => new tShadow {
		Size = aSize,
		Offset = aOffset,
		Deep = new tInt16[aSize.X, aSize.Y],
	};
	
	public static tSprite
	CreateSprite(
		tV2 aSize,
		tV2 aOffset
	) => new tSprite {
		Size = aSize,
		Offset = aOffset,
		Color = new tColor[aSize.X, aSize.Y],
		Deep = new tInt16[aSize.X, aSize.Y],
		Normal = new (tInt8 U, tInt8 V)[aSize.X, aSize.Y],
		PosBits = new tNat8[aSize.X, aSize.Y],
	};
	
	static readonly tM3x3[,]
	_Matrix__ = new tM3x3[cQuarterParts, cAngleParts] {
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
	
	public static tRenderEnv
	_Update(
		this tRenderEnv a
	) {
		while (a.Dir < 0) { a.Dir += 4 * cQuarterParts; }
		while (a.Dir >= 4 * cQuarterParts) { a.Dir -= 4 * cQuarterParts; }
		mMath.Clamp(ref a.Angle, 0, 4);
		a.M = a.GetMatrix(a.Dir, a.Angle);
		(a.InvM, a.Det) = mMath3D.Inverse(a.M);
		a.NormalPattern = a.GetNormalPattern(a.Dir, a.Angle);
		return a;
	}
	
	public static tV3
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
		#if true
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
	
	public static (tAxis, tInt32)
	GetMainAxis(
		tV3 aDirection
	) {
		if (mMath.Abs(aDirection.Z) >= mMath.Abs(aDirection.X) &&
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
	
	public static tV2
	GetDeepSize(
		tV3 LightDirection,
		tInt32 aCubeSize
	) {
		var V = LightDirection * aCubeSize;
		return V2(aCubeSize, aCubeSize) + GetMainAxis(LightDirection) switch {
			(tAxis.Z, _) => V2(mMath.Abs(V.X / LightDirection.Z), mMath.Abs(V.Y / LightDirection.Z)),
			(tAxis.Y, _) => V2(mMath.Abs(V.X / LightDirection.Y), mMath.Abs(V.Z / LightDirection.Y)),
			(tAxis.X, _) => V2(mMath.Abs(V.Z / LightDirection.X), mMath.Abs(V.Y / LightDirection.X)),
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
	
	public static tV2
	GetShadowSize(
		tInt32 aCubeSize,
		mMath3D.tV3 aLightDirection
	) {
		var (MainAxis, _) = GetMainAxis(aLightDirection);
		
		return MainAxis switch {
			tAxis.X => mMath2D.V2(
				aCubeSize + mMath.Abs(aLightDirection.Z * aCubeSize / aLightDirection.X),
				aCubeSize + mMath.Abs(aLightDirection.Y * aCubeSize / aLightDirection.X)
			),
			tAxis.Y => mMath2D.V2(
				aCubeSize + mMath.Abs(aLightDirection.X * aCubeSize / aLightDirection.Y),
				aCubeSize + mMath.Abs(aLightDirection.Z * aCubeSize / aLightDirection.Y)
			),
			tAxis.Z => mMath2D.V2(
				aCubeSize + mMath.Abs(aLightDirection.X * aCubeSize / aLightDirection.Z),
				aCubeSize + mMath.Abs(aLightDirection.Y * aCubeSize / aLightDirection.Z)
			)
		};
	}
	
	public static tShadow
	_DrawCube(
		this tShadow aShadow,
		tColor[,,] aCube,
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
			var Z_ = Z - CubeLengthHalf;
			for (var Y = (tInt16)0; Y < CubeLength; Y += 1) {
				var Y_ = Y - CubeLengthHalf;
				for (var X = (tInt16)0; X < CubeLength; X += 1) {
					var X_ = X - CubeLengthHalf;
					
					var Color = aCube[X, Y, Z];
					if (Color == default) {
						continue;
					}
					
					var Deep = default(tInt16);
					var U = default(tInt32);
					var V = default(tInt32);
					
					switch (MainAxis) {
						case tAxis.X: {
							U = UCenter + Z_ + aLightDirection.Z * X_ / aLightDirection.X;
							V = VCenter + Y_ + aLightDirection.Y * X_ / aLightDirection.X;
							Deep = (tInt16)(AxisSign * X_);
							break;
						}
						case tAxis.Y: {
							U = UCenter + X_ + aLightDirection.X * Y_ / aLightDirection.Y;
							V = VCenter + Z_ + aLightDirection.Z * Y_ / aLightDirection.Y;
							Deep = (tInt16)(AxisSign * Y_);
							break;
						}
						case tAxis.Z: {
							U = UCenter + X_ + aLightDirection.X * Z_ / aLightDirection.Z;
							V = VCenter + Y_ + aLightDirection.Y * Z_ / aLightDirection.Z;
							Deep = (tInt16)(AxisSign * Z_);
							break;
						}
					}
					ref var pDeep = ref aShadow.Deep[U, V];
					pDeep = Min(pDeep, Deep);
				}
			}
		}
		
		return aShadow;
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
		var DesMin = ((DesSize - SrcSize) >> 1) + V2(aOffset2D);
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
					aDes.PosBits[DesX, DesY] = aSrc.PosBits[SrcX, SrcY];
				}
			}
		}
		
		return aDes;
	}
	
	public static tShadow
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
	
	public static tInt16
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
		
		var UCenter = aShadow.Size.X >> 1;
		var VCenter = aShadow.Size.Y >> 1;
		
		var Deep = default(tInt16);
		var U = default(tInt32);
		var V = default(tInt32);
		
		switch (MainAxis) {
			case tAxis.X: {
				U = UCenter + Z_ + aLightDirection.Z * X_ / aLightDirection.X;
				V = VCenter + Y_ + aLightDirection.Y * X_ / aLightDirection.X;
				Deep = (tInt16)(AxisSign * X_);
				break;
			}
			case tAxis.Y: {
				U = UCenter + X_ + aLightDirection.X * Y_ / aLightDirection.Y;
				V = VCenter + Z_ + aLightDirection.Z * Y_ / aLightDirection.Y;
				Deep = (tInt16)(AxisSign * Y_);
				break;
			}
			case tAxis.Z: {
				U = UCenter + X_ + aLightDirection.X * Z_ / aLightDirection.Z;
				V = VCenter + Y_ + aLightDirection.Y * Z_ / aLightDirection.Z;
				Deep = (tInt16)(AxisSign * Z_);
				break;
			}
		}
		
		if (
			0 <= U && U < aShadow.Size.X &&
			0 <= V && V < aShadow.Size.Y
		) {
			return (tInt16)(Deep - aShadow.Deep[U, V]);
		} else {
			return tInt16.MinValue; 
		}
	}
	
	public enum
	tDebugRenderMode {
		None,
		Normal,
		Deep,
		Pos,
		PosBits,
	};
	
	public static unsafe tRenderEnv
	_RenderToBuffer(
		this tRenderEnv aRenderEnv,
		tSprite aSprite,
		tShadow aShadow,
		System.IntPtr aBuffer,
		tV2 aBufferSize,
		tDebugRenderMode aDebugRenderMode
	) {
		System.Console.WriteLine(aRenderEnv.Det);
		
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
					aRenderEnv.To3D(aSprite, V2(X, Y))
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
						var Pos = aRenderEnv.To3D(aSprite, V2(X, Y));
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
	
	public static unsafe mRenderEngine.tShadow
	_RenderToBuffer(
		this mRenderEngine.tShadow aShadow,
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
	
}
