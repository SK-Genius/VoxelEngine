﻿using System;
using System.Runtime.CompilerServices;

public static class
mMath {
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt8
	Sign(
		this tInt32 a
	) => (tInt8)Math.Sign(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt8
	Sign(
		this tInt8 a
	) => (tInt8)Math.Sign(a);
	
	public static tInt8
	Sign(
		this tInt16 a
	) => (tInt8)Math.Sign(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tNat8
	Abs(
		this tInt8 a
	) => (tNat8)Math.Abs(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tNat16
	Abs(
		this tInt16 a
	) => (tNat16)Math.Abs(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tNat32
	Abs(
		this tInt32 a
	) => (tNat32)Math.Abs(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt8
	IAbs(
		this tInt8 a
	) => Math.Abs(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt16
	IAbs(
		this tInt16 a
	) => Math.Abs(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	IAbs(
		this tInt32 a
	) => Math.Abs(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Floor(
		tReal32 a
	) {
		return (tInt32)Math.Floor(a);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Round(
		tReal32 a
	) {
		return Floor(a + 0.5f);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Sqrt(
		tInt32 a
	) => (tInt32)Math.Sqrt(a);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tNat16
	FastSqrt(
		tNat32 a
	) {
		var Result = (tNat16)0;
		var NewBit = (tNat16)0x8000;   
		while (NewBit > 0) {
			var NewResult = (tNat16)(Result | NewBit);
			var Test = (tNat32)(NewResult * NewResult);      
			if (Test <= a) {
				Result = NewResult;           
			}
			NewBit >>= 1;
		}
		return Result;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tNat8
	FastSqrt(
		tNat16 a
	) {
		var Result = (tNat8)0;
		var NewBit = (tNat8)0x80;   
		while (NewBit > 0) {
			var NewResult = (tNat8)(Result | NewBit);
			var Test = (tNat16)(NewResult * NewResult);      
			if (Test <= a) {
				Result = NewResult;           
			}
			NewBit >>= 1;
		}
		return Result;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Min(
		tInt32 a1,
		tInt32 a2
	) => Math.Min(a1, a2);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	Max(
		tInt32 a1,
		tInt32 a2
	) => Math.Max(a1, a2);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt16
	Min(
		tInt16 a1,
		tInt16 a2
	) => Math.Min(a1, a2);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt16
	Max(
		tInt16 a1,
		tInt16 a2
	) => Math.Max(a1, a2);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void
	Clamp(
		this ref tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) {
		a = Math.Clamp(a, aMin, aMax);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tBool
	IsInRange(
		this tInt32 a,
		tInt32 aMin,
		tInt32 aMax
	) => aMin <= a & a <= aMax;
}
