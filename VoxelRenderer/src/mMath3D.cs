using System.Diagnostics;

public static class
mMath3D {
	
	[DebuggerDisplay("({X}, {Y}, {Z})")]
	public struct
	tV3 {
		public readonly tInt32 X;
		public readonly tInt32 Y;
		public readonly tInt32 Z;
		
		internal
		tV3 (
			tInt32 aX,
			tInt32 aY,
			tInt32 aZ
		) {
			this.X = aX;
			this.Y = aY;
			this.Z = aZ;
		}
		
		public static tV3
		operator+(
			tV3 a1,
			tV3 a2
		) => V3(
			a1.X + a2.X,
			a1.Y + a2.Y,
			a1.Z + a2.Z
		);
		
		public static tV3
		operator-(
			tV3 a
		) => V3(-a.X, -a.Y, -a.Z);
		
		public static tV3
		operator-(
			tV3 a1,
			tV3 a2
		) => a1 + -a2;
		
		public static tV3
		operator*(
			tV3 a1,
			tV3 a2
		) => V3(
			a1.X * a2.X,
			a1.Y * a2.Y,
			a1.Z * a2.Z
		);
		
		public static tV3
		operator*(
			tInt32 a1,
			tV3 a2
		) => V3(
			a1 * a2.X,
			a1 * a2.Y,
			a1 * a2.Z
		);
		
		public static tV3
		operator*(
			tV3 a1,
			tInt32 a2
		) => a2 * a1;
		
		public static tV3
		operator/(
			tV3 a1,
			tInt32 a2
		) => V3(
			Div(a1.X + (a2 >> 1), a2),
			Div(a1.Y + (a2 >> 1), a2),
			Div(a1.Z + (a2 >> 1), a2)
		);      
		
		public override tText
		ToString() => $"({this.X}, {this.Y}, {this.Z})";
	}
	
	private static tInt32
	Div(
		tInt32 a1,
		tInt32 a2
	) {
		if (((a1 < 0) ^ (a2 < 0)) && a1 % a2 != 0)
		{
			return a1 / a2 - 1;
		}
		else
		{
			return a1 / a2;
		}
	}
	
	public static tV3
	V3(
		tInt32 aX,
		tInt32 aY,
		tInt32 aZ
	) => new tV3(aX, aY, aZ);
	
	public static tV3
	V3(
		mMath2D.tV2 aV2,
		tInt32 aZ
	) => new tV3(aV2.X, aV2.Y, aZ);
	
	public static mMath2D.tV2
	V2(
		tV3 aV3
	) => mMath2D.V2(aV3.X, aV3.Y);
	
	public static tInt32
	Sum(
		this tV3 a
	) => a.X + a.Y + a.Z;
	
	public struct
	tM3x3 {
		public readonly tV3 X;
		public readonly tV3 Y;
		public readonly tV3 Z;
		
		internal
		tM3x3 (
			tV3 aX,
			tV3 aY,
			tV3 aZ
		) {
			this.X = aX;
			this.Y = aY;
			this.Z = aZ;
		}
		
		public static tV3
		operator *(
			tV3 aV,
			tM3x3 aM
		) => V3(
			aM.X.X * aV.X + aM.X.Y * aV.Y + aM.X.Z * aV.Z,
			aM.Y.X * aV.X + aM.Y.Y * aV.Y + aM.Y.Z * aV.Z,
			aM.Z.X * aV.X + aM.Z.Y * aV.Y + aM.Z.Z * aV.Z
		);
		
		public static tM3x3
		operator *(
			tInt32 aS,
			tM3x3 aM
		) => M3x3(
			V3(aM.X.X * aS, aM.X.Y * aS, aM.X.Z * aS),
			V3(aM.Y.X * aS, aM.Y.Y * aS, aM.Y.Z * aS),
			V3(aM.Z.X * aS, aM.Z.Y * aS, aM.Z.Z * aS)
		);
		
		public static tM3x3
		operator *(
			tM3x3 aM1,
			tM3x3 aM2
		) {
			aM2 = aM2.T();
			return M3x3(
				V3(Sum(aM1.X * aM2.X), Sum(aM1.X * aM2.Y), Sum(aM1.X * aM2.Z)),
				V3(Sum(aM1.Y * aM2.X), Sum(aM1.Y * aM2.Y), Sum(aM1.Y * aM2.Z)),
				V3(Sum(aM1.Z * aM2.X), Sum(aM1.Z * aM2.Y), Sum(aM1.Z * aM2.Z))
			); 
		}
	}
	
	public static tM3x3
	M3x3(
		tV3 aX,
		tV3 aY,
		tV3 aZ
	) => new tM3x3(aX, aY, aZ);
	
	public static tM3x3
	T(
		this tM3x3 a
	) => M3x3(
		V3(a.X.X, a.Y.X, a.Z.X),
		V3(a.X.Y, a.Y.Y, a.Z.Y),
		V3(a.X.Z, a.Y.Z, a.Z.Z)
	);
	
	public static (tM3x3 M, tInt32 Det)
	Inverse(
		this tM3x3 a
	) {
		var Res = (
			M: M3x3(
				V3(Det(a.Y.Y, a.Y.Z, a.Z.Y, a.Z.Z), Det(a.X.Z, a.X.Y, a.Z.Z, a.Z.Y), Det(a.X.Y, a.X.Z, a.Y.Y, a.Y.Z)),
				V3(Det(a.Y.Z, a.Y.X, a.Z.Z, a.Z.X), Det(a.X.X, a.X.Z, a.Z.X, a.Z.Z), Det(a.X.Z, a.X.X, a.Y.Z, a.Y.X)),
				V3(Det(a.Y.X, a.Y.Y, a.Z.X, a.Z.Y), Det(a.X.Y, a.X.X, a.Z.Y, a.Z.X), Det(a.X.X, a.X.Y, a.Y.X, a.Y.Y))
			),
			Det: Det(
				a.X.X, a.X.Y, a.X.Z,
				a.Y.X, a.Y.Y, a.Y.Z,
				a.Z.X, a.Z.Y, a.Z.Z
			)
		);
		//System.Console.WriteLine($@"M:");
		//System.Console.WriteLine($@"/ {a.X.X,2} {a.X.Y,2} {a.X.Z,2} \");
		//System.Console.WriteLine($@"| {a.Y.X,2} {a.Y.Y,2} {a.Y.Z,2} |");
		//System.Console.WriteLine($@"\ {a.Z.X,2} {a.Z.Y,2} {a.Z.Z,2} /");
		//System.Console.WriteLine($@"M^-1:");
		//System.Console.WriteLine($@"/ {Res.M.X.X,2} {Res.M.X.Y,2} {Res.M.X.Z,2} \");
		//System.Console.WriteLine($@"| {Res.M.Y.X,2} {Res.M.Y.Y,2} {Res.M.Y.Z,2} |");
		//System.Console.WriteLine($@"\ {Res.M.Z.X,2} {Res.M.Z.Y,2} {Res.M.Z.Z,2} /");
		//System.Console.WriteLine($@"Det: {Res.Det}");
		//System.Console.WriteLine();
		return Res;
	}
	
	public static int
	Det(
		tInt32 a11,
		tInt32 a12,
		tInt32 a13,
		tInt32 a21,
		tInt32 a22,
		tInt32 a23,
		tInt32 a31,
		tInt32 a32,
		tInt32 a33
	) => (
		+a11*a22*a33
		+a12*a23*a31
		+a13*a21*a32
		-a13*a22*a31
		-a11*a23*a32
		-a12*a21*a33
	);
	
	public static int
	Det(
		tInt32 a11,
		tInt32 a12,
		tInt32 a21,
		tInt32 a22
	) => (
		+a11*a22
		-a12*a21
	);
	
	public static readonly tM3x3
	cRotLeft = M3x3(
		V3( 0, -1,  0),
		V3( 1,  0,  0),
		V3( 0,  0,  1)
	);
	
	public static readonly tM3x3
	cRotRight = M3x3(
		V3( 0,  1,  0),
		V3(-1,  0,  0),
		V3( 0,  0,  1)
	);
	
}
