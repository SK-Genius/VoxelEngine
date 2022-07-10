using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class
mMath2D {
	[DebuggerDisplay("({X}, {Y})")]
	public struct
	tV2 {
		public tInt32 X;
		public tInt32 Y;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV2
		operator+(
			tV2 a1,
			tV2 a2
		) => V2(
			a1.X + a2.X,
			a1.Y + a2.Y
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV2
		operator-(
			tV2 a1,
			tV2 a2
		) => V2(
			a1.X - a2.X,
			a1.Y - a2.Y
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV2
		operator-(
			tV2 a
		) => V2(
			-a.X,
			a.Y
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV2
		operator*(
			tV2 a1,
			tInt32 a2
		) => V2(
			a1.X * a2,
			a1.Y * a2
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV2
		operator*(
			tInt32 a1,
			tV2 a2
		) => a2 * a1;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV2
		operator/(
			tV2 a1,
			tInt32 a2
		) => V2(
			mMath.Div(a1.X, a2),
			mMath.Div(a1.Y, a2)
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV2
		operator>>(
			tV2 a1,
			tInt32 a2
		) => V2(
			a1.X >> a2,
			a1.Y >> a2
		);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override tText
		ToString() => $"({this.X}, {this.Y})";
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	V2(
		tInt32 aX,
		tInt32 aY
	) => new tV2 {
		X = aX,
		Y = aY,
	};
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	V2(
		tInt32 a
	) => new tV2 {
		X = a,
		Y = a,
	};
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	V2(
	) => V2(0);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tBool
	IsInRange(
		this tV2 aPoint,
		tV2 aMin,
		tV2 aMax
	) => (
		aPoint.X.IsInRange(aMin.X, aMax.X) &&
		aPoint.Y.IsInRange(aMin.Y, aMax.Y)
	);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	Min(
		tV2 a1,
		tV2 a2
	) => V2(
		mMath.Min(a1.X, a2.X),
		mMath.Min(a1.Y, a2.Y)
	);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	Max(
		tV2 a1,
		tV2 a2
	) => V2(
		mMath.Max(a1.X, a2.X),
		mMath.Max(a1.Y, a2.Y)
	);
}
