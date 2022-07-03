using System;

public static class
mMath {
	
	public static tInt32
	Div(
		tInt32 a1,
		tInt32 a2
	) {
		if (((a1 ^ a2) & 0x80_00__00_00) == 0)
		{
			return a1 / a2;
		}
		else
		{
			return (a1 - a2 + 1) / a2;
		}
	}
	
	public static tInt8
	Sign(
		this tInt32 a
	) => (tInt8)Math.Sign(a);
	
	public static tInt8
	Sign(
		this tInt8 a
	) => (tInt8)Math.Sign(a);
	
	public static tInt8
	Sign(
		this tInt16 a
	) => (tInt8)Math.Sign(a);
	
	public static tInt8
	Abs(
		this tInt8 a
	) => (tInt8)Math.Abs(a);
	
	public static tInt32
	Abs(
		this tInt32 a
	) => Math.Abs(a);
	
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
	Sqrt(
		tInt32 a
	) => (tInt32)Math.Sqrt(a);
	
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
