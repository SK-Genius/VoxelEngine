using System.Diagnostics;

public static class
mMath2D {
	[DebuggerDisplay("({X}, {Y})")]
	public struct
	tV2 {
		public tInt32 X;
		public tInt32 Y;
		
		public static tV2
		operator+(
			tV2 a1,
			tV2 a2
		) => V2(
			a1.X + a2.X,
			a1.Y + a2.Y
		);
		
		public static tV2
		operator-(
			tV2 a1,
			tV2 a2
		) => V2(
			a1.X - a2.X,
			a1.Y - a2.Y
		);
		
		public static tV2
		operator-(
			tV2 a
		) => V2(
			-a.X,
			a.Y
		);
		
		public static tV2
		operator*(
			tV2 a1,
			tInt32 a2
		) => V2(
			a1.X * a2,
			a1.Y * a2
		);
		
		public static tV2
		operator*(
			tInt32 a1,
			tV2 a2
		) => a2 * a1;
		
		public static tV2
		operator>>(
			tV2 a1,
			tInt32 a2
		) => V2(
			a1.X >> a2,
			a1.Y >> a2
		);
		
		public override tText
		ToString() => $"({this.X}, {this.Y})";
	}
	
	public static tV2
	V2(
		tInt32 aX,
		tInt32 aY
	) => new tV2 {
		X = aX,
		Y = aY,
	};
	
	public static tBool
	IsInRange(
		this tV2 aPoint,
		tV2 aMin,
		tV2 aMax
	) => (
		aPoint.X.IsInRange(aMin.X, aMax.X) &&
		aPoint.Y.IsInRange(aMin.Y, aMax.Y)
	);
}
