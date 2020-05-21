using tBool = System.Boolean;

using tNat8 = System.Byte;
using tNat16 = System.UInt16;
using tNat32 = System.UInt32;
using tNat64 = System.UInt64;

using tInt8 = System.SByte;
using tInt16 = System.Int16;
using tInt32 = System.Int32;
using tInt64 = System.Int64;

using tChar = System.Char;
using tText = System.String;

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
			a1.X / a2,
			a1.Y / a2,
			a1.Z / a2
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
		var A = a.X.X;
		var B = a.X.Y;
		var C = a.X.Z;
		var D = a.Y.X;
		var E = a.Y.Y;
		var F = a.Y.Z;
		var G = a.Z.X;
		var H = a.Z.Y;
		var I = a.Z.Z;
		return (
			M: M3x3(
				V3(E*I - F*H, C*H - B*I, B*F - C*E),
				V3(F*G - D*I, A*I - C*G, C*D - A*F),
				V3(D*H - E*G, B*G - A*H, A*E - B*D)
			),
			Det: -C*E*G +B*F*G +C*D*H -A*F*H -B*D*I +A*E*I
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
