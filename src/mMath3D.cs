public static class
mMath3D {
	
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
		operator/(
			tV3 a1,
			tInt32 a2
		) => V3(
			(tInt32)System.Math.Floor((a1.X + 0.5f) / a2),
			(tInt32)System.Math.Floor((a1.Y + 0.5f) / a2),
			(tInt32)System.Math.Floor((a1.Z + 0.5f) / a2)
		);      
		
		public override tText
		ToString() => $"({this.X}, {this.Y}, {this.Z})";
	}
	
	public static tV3
	V3(
		tInt32 aX,
		tInt32 aY,
		tInt32 aZ
	) => new tV3(aX, aY, aZ);
	
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
	Invers(
		this tM3x3 a
	) {
		var XX = a.X.X;
		var XY = a.X.Y;
		var XZ = a.X.Z;
		var YX = a.Y.X;
		var YY = a.Y.Y;
		var YZ = a.Y.Z;
		var ZX = a.Z.X;
		var ZY = a.Z.Y;
		var ZZ = a.Z.Z;
		return (
			M: M3x3(
				V3(YY*ZZ - YZ*ZY, XZ*ZY - XY*ZZ, XY*YZ - XZ*YY),
				V3(YZ*ZX - YX*ZZ, XX*ZZ - XZ*ZX, XZ*YX - XX*YZ),
				V3(YX*ZY - YY*ZX, XY*ZX - XX*ZY, XX*YY - XY*YX)
			),
			Det: -XZ*YY*ZX +XY*YZ*ZX +XZ*YX*ZY -XX*YZ*ZY -XY*YX*ZZ +XX*YY*ZZ
		);
	}
	
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
