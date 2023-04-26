using System.Diagnostics;
using System.Runtime.Intrinsics;

using static mMath;
using static mV2;

public static class
mV3 {
	
	[DebuggerDisplay("({X}, {Y}, {Z})")]
	public struct
	tV3 {
		public readonly tInt32 X;
		public readonly tInt32 Y;
		public readonly tInt32 Z;
		
		public static tV3 cX = V3(1, 0, 0);
		public static tV3 cY = V3(0, 1, 0);
		public static tV3 cZ = V3(0, 0, 1);
		
		public static tV3 cZero = V3(0, 0, 0);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Deconstruct(out tInt32 aX, out tInt32 aY, out tInt32 aZ)
		{
			aX = this.X;
			aY = this.Y;
			aZ = this.Z;
		}
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator+(
			tV3 a1,
			tV3 a2
		) => V3(
			a1.X + a2.X,
			a1.Y + a2.Y,
			a1.Z + a2.Z
		);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator-(
			tV3 a
		) => V3(
			-a.X,
			-a.Y,
			-a.Z
		);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator-(
			tV3 a1,
			tV3 a2
		) => a1 + -a2;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator*(
			tV3 a1,
			tV3 a2
		) => V3(
			a1.X * a2.X,
			a1.Y * a2.Y,
			a1.Z * a2.Z
		);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator*(
			tInt32 a1,
			tV3 a2
		) => V3(
			a1 * a2.X,
			a1 * a2.Y,
			a1 * a2.Z
		);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator*(
			tV3 a1,
			tInt32 a2
		) => a2 * a1;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator/(
			tV3 a1,
			tInt32 a2
		) => V3(
			Div(a1.X, a2),
			Div(a1.Y, a2),
			Div(a1.Z, a2)
		);      
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator/(
			tV3 a1,
			tV3 a2
		) => V3(
			Div(a1.X, a2.X),
			Div(a1.Y, a2.Y),
			Div(a1.Z, a2.Z)
		);      
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator%(
			tV3 a1,
			tInt32 a2
		) => V3(
			Mod(a1.X, a2),
			Mod(a1.Y, a2),
			Mod(a1.Z, a2)
		);      
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator%(
			tV3 a1,
			tV3 a2
		) => V3(
			Mod(a1.X, a2.X),
			Mod(a1.Y, a2.Y),
			Mod(a1.Z, a2.Z)
		);      
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV3
		operator>>(
			tV3 a1,
			tInt32 a2
		) => V3(
			a1.X >> a2,
			a1.Y >> a2,
			a1.Z >> a2
		);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator==(
			tV3 a1,
			tV3 a2
		) => (
			a1.X == a2.X &
			a1.Y == a2.Y &
			a1.Z == a2.Z
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator!=(
			tV3 a1,
			tV3 a2
		) => !(a1 == a2);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator<(
			tV3 a1,
			tV3 a2
		) => (
			a1.X < a2.X &
			a1.Y < a2.Y &
			a1.Z < a2.Z
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator>(
			tV3 a1,
			tV3 a2
		) => (
			a1.X > a2.X &
			a1.Y > a2.Y &
			a1.Z > a2.Z
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator>=(
			tV3 a1,
			tV3 a2
		) => (
			a1.X >= a2.X &
			a1.Y >= a2.Y &
			a1.Z >= a2.Z
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tBool
		operator<=(
			tV3 a1,
			tV3 a2
		) => (
			a1.X <= a2.X &
			a1.Y <= a2.Y &
			a1.Z <= a2.Z
		);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override tText
		ToString() => $"({this.X}, {this.Y}, {this.Z})";
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	V3(
	) => tV3.cZero;
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	V3(
		tInt32 a
	) => new tV3(a, a, a);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	V3(
		tInt32 aX,
		tInt32 aY,
		tInt32 aZ
	) => new tV3(aX, aY, aZ);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	V3(
		tV2 aV2,
		tInt32 aZ
	) => new tV3(aV2.X, aV2.Y, aZ);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	XY(
		this tV3 aV3
	) => V2(aV3.X, aV3.Y);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	XZ(
		this tV3 aV3
	) => V2(aV3.X, aV3.Z);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	YZ(
		this tV3 aV3
	) => V2(aV3.Y, aV3.Z);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	ZY(
		this tV3 aV3
	) => V2(aV3.Z, aV3.Y);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Min(
		this tV3 a
	) => mMath.Min(mMath.Min(a.X, a.Y), a.Z);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Max(
		this tV3 a
	) => mMath.Max(mMath.Max(a.X, a.Y), a.Z);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	Abs(
		this tV3 a
	) => V3(mMath.IAbs(a.X), mMath.IAbs(a.Y), mMath.IAbs(a.Z));
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Sum(
		this tV3 a
	) => a.X + a.Y + a.Z;
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	Sign(
		this tV3 a
	) => V3(
		a.X.Sign(),
		a.Y.Sign(),
		a.Z.Sign()
	);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Abs2(
		this tV3 a
	) => a.X*a.X + a.Y*a.Y + a.Z*a.Z;
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	Norm(
		this tV3 a,
		tInt32 aScale
	) {
		var Length = System.MathF.Sqrt(a.Abs2()) / aScale;
		return V3(
			Floor(a.X / Length),
			Floor(a.Y / Length),
			Floor(a.Z / Length)
		);
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	Min(
		tV3 a1,
		tV3 a2
	) => V3(
		mMath.Min(a1.X, a2.X),
		mMath.Min(a1.Y, a2.Y),
		mMath.Min(a1.Z, a2.Z)
	);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	Max(
		tV3 a1,
		tV3 a2
	) => V3(
		mMath.Max(a1.X, a2.X),
		mMath.Max(a1.Y, a2.Y),
		mMath.Max(a1.Z, a2.Z)
	);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tBool
	IsInRange(
		this tV3 a,
		tV3 aMin,
		tV3 aMax
	) => (aMin <= a) & (a <= aMax);
}
