using System;

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
	
}
