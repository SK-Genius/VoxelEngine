using System;

static class
mMath {
	
	public static tInt8
	LimitTo(
		this tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) => (tInt8)Math.Min(Math.Max(aMin, a), aMax);
	
	public static tInt8
	Sign(
		this tInt32 a
	) => (sbyte)Math.Sign(a);
	
	public static tInt8
	Sign(
		this sbyte a
	) => (sbyte)Math.Sign(a);
	
	public static tInt8
	Sign(
		this tInt16 a
	) => (sbyte)Math.Sign(a);
	
	public static tInt8
	Abs(
		this tInt8 a
	) => (sbyte)Math.Abs(a);
	
	public static tInt32
	Abs(
		this tInt32 a
	) => Math.Abs(a);
	
	public static tBool
	InRange(
		this tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) => aMin <= a && a <= aMax;

	public static tInt32
	Clamp(
		this tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) => a < aMin ? aMin : a > aMax ? aMax : a;

	public static void
	Clamp(
		ref tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) {
		a = Clamp(a, aMin, aMax);
	}
}
