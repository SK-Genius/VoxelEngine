using static mMath;
using static mMath2D;
using static mMath3D;

public static class
mRenderEngine {
	
	public const tInt32
	cQuaterParts = 6;
	
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
		
		public tM3x3 M;
		public tM3x3 InvM;
		public tInt32 Det;
		
		public tAxis[,] NormalPattern;
	}
	
	public struct
	tSprite {
		public tV2 Size;
		public tV2 Offset;
		public short[,] Color;
		public short[,] Deep;
	}
	
	public static tSprite
	CreateSprite(
		tV2 aSize,
		tV2 aOffset
	) {
		return new tSprite {
			Size = aSize,
			Offset = aOffset,
			Color = new short[aSize.X, aSize.Y],
			Deep = new short[aSize.X, aSize.Y],
		};
	}
	
	static readonly tM3x3[,]
	_Matrix = new tM3x3[cQuaterParts, cAngleParts] {
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
				V3( 1, -2, 4),
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
				V3( 2, -1, 4),
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
	Matrix(
		tInt32 aDir,
		tInt32 aAngle
	) {
		var M = _Matrix[aDir % cQuaterParts, aAngle];
		while (aDir >= cQuaterParts) {
			M = M * cRotRight;
			aDir -= cQuaterParts;
		}
		return M;
	}
	
	static tAxis[,][,] NormalPatterns = new tAxis[cQuaterParts, cAngleParts][,] {
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
	NormalPattern(
		tInt32 aDir,
		tInt32 aAngle
	) {
		var P = NormalPatterns[aDir % cQuaterParts, aAngle];
		if (
			aDir.InRange(0*cQuaterParts, 1*cQuaterParts-1) ||
			aDir.InRange(2*cQuaterParts, 3*cQuaterParts-1)
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
	
	public static void
	Clear(
		this tSprite aGrid
	) {
		var MaxX = aGrid.Size.X;
		var MaxY = aGrid.Size.Y;
		
		for (var X = 0; X < MaxX; X += 1) {
			for (var Y = 0; Y < MaxY; Y += 1) {
				aGrid.Color[X, Y] = 0;
				aGrid.Deep[X, Y] = short.MaxValue;
			}
		}
	}
	
	public static void
	Update(
		this tRenderEnv a
	) {
		while (a.Dir < 0) { a.Dir += 4 * cQuaterParts; }
		while (a.Dir >= 4 * cQuaterParts) { a.Dir -= 4 * cQuaterParts; }
		mMath.Clamp(ref a.Angle, 0, 4);
		a.M = Matrix(a.Dir, a.Angle);
		(a.InvM, a.Det) = mMath3D.Invers(a.M);
		a.NormalPattern = NormalPattern(a.Dir, a.Angle);
	}
	
	public static mMath3D.tV3
	To3D(
		this tSprite aGrid,
		mMath2D.tV2 aPoint,
		tRenderEnv aRenderEnv,
		tInt32 aCubeLength
	) {
		var X = aPoint.X;
		var Y = aPoint.Y;
		var MaxX = aGrid.Size.X;
		var MaxY = aGrid.Size.Y;
		if (X.InRange(0, MaxX - 1) && Y.InRange(0, MaxY - 1)) {
			var P2D = mMath3D.V3(X, Y, aGrid.Deep[X, Y] - 1) - mMath3D.V3(36 + 1, 36 + 1, 0);
			var CubeLengthHalf = (aCubeLength * aRenderEnv.Det) >> 1;
			return ((P2D * aRenderEnv.InvM) + mMath3D.V3(CubeLengthHalf, CubeLengthHalf, CubeLengthHalf)) / aRenderEnv.Det;
		} else {
			return mMath3D.V3(0, 0, 0);
		}
	}
	
	public static void
	DrawCube(
		this tSprite aGrid,
		sbyte[,,] aCube,
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
					if (Color != 0) {
						var Des_ = V3(
							X - CubeLengthHalf,
							Y - CubeLengthHalf,
							Z - CubeLengthHalf
						) * aMatrix;
						
						for (var V = 0; V < MaxV; V += 1) {
							for (var U = 0; U < MaxU; U += 1) {
								var Axis = aNormal[U, V];
								var Des = Des_ + V3(U + 4 * CubeLength, V + 4 * CubeLength, 0);
								if ((Color & 128) != 0 && ((Des.X ^ Des.Y) & 1) == 0) {
									continue;
								}
								if (Axis != tAxis._ && Des.X >= 0 && Des.Y >= 0) {
									ref var Color_ = ref aGrid.Color[Des.X, Des.Y];
									ref var Deep_ = ref aGrid.Deep[Des.X, Des.Y];
									if (Des.Z <= Deep_) {
										Color_ = (sbyte)(
											(Color & 0b111) | Axis switch {
												tAxis.X => 16,
												tAxis.Y => 8,
												tAxis.Z => 0,
											}
										);
										Deep_ = (short)Des.Z;
									}
								}
							}
						}
					}
				}
			}
		}
	}
	
	public static void
	DrawSprite(
		this tSprite aDes,
		tSprite aSrc,
		tV3 aPos,
		tM3x3 aMatrix
	) {
		var XOffset = 0;
		var YOffset = 0;
		
		var Min = V2(0, 0);
		var Max = V2(
			System.Math.Min(aSrc.Size.X, aDes.Size.X),
			System.Math.Min(aSrc.Size.Y, aDes.Size.Y)
		);
		
		for (var Y = Min.Y; Y < Max.Y; Y += 1) {
			for (var X = Min.X; X < Max.X; X += 1) {
				ref var SrcDeep = ref aSrc.Deep[X, Y];
				ref var DesDeep = ref aDes.Deep[X + XOffset, Y + YOffset];
				if (SrcDeep <= DesDeep) {
					DesDeep = SrcDeep;
					aDes.Color[X + XOffset, Y + YOffset] =  aSrc.Color[X, Y];
				}
			}
		}
	}
}
