public static class
mMath2D {
	public struct
	tV2 {
		public tInt32 X;
		public tInt32 Y;
	}
	
	public static tV2
	V2(
		tInt32 aX,
		tInt32 aY
	) => new tV2 {
		X = aX,
		Y = aY,
	};
}
