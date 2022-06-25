using System;

public static class
mMath {
	
	public static tInt8
	Sign(
		this tInt32 a
	) => (sbyte)Math.Sign((int)a);
	
	public static tInt8
	Sign(
		this tInt8 a
	) => (sbyte)Math.Sign((sbyte)a);
	
	public static tInt8
	Sign(
		this tInt16 a
	) => (tInt8)Math.Sign((short)a);
	
	public static tInt8
	Abs(
		this tInt8 a
	) => (tInt8)Math.Abs((sbyte)a);
	
	public static tInt32
	Abs(
		this tInt32 a
	) => Math.Abs((int)a);
	
	public static tInt32
	Floor(
		tReal32 a
	) {
		return (tInt32)Math.Floor(a);
	}
	
	public static tInt32
	Round(
		tReal32 a
	) {
		return Floor(a + 0.5f);
	}
	
	public static tInt32
	Min(
		tInt32 a1,
		tInt32 a2
	) => Math.Min(a1, a2);
	
	public static tInt32
	Max(
		tInt32 a1,
		tInt32 a2
	) => Math.Max(a1, a2);
	
	public static tInt16
	Min(
		tInt16 a1,
		tInt16 a2
	) => Math.Min(a1, a2);
	
	public static tInt16
	Max(
		tInt16 a1,
		tInt16 a2
	) => Math.Max(a1, a2);
	
	public static void
	Clamp(
		this ref tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) {
		a = Math.Clamp(a, aMin, aMax);
	}
	
	public static tBool
	IsInRange(
		this tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) => aMin <= a & a <= aMax;
}
