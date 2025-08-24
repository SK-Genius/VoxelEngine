using System.Numerics;

public static class
mSIMD {
	
	public static tInt32
	VCount<t>(
	) where t : struct => Vector<t>.Count;
	
	public struct tV<t> where t : struct {
		internal Vector<t> Value;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		private tV(
			Vector<t> a
		) {
			this.Value = a;
		}
		
		public static implicit
		operator tV<t>(
			Vector<t> a
		) => new tV<t>(a);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator-(
			tV<t> a
		) => Zero<t>() - a.Value;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator+(
			tV<t> a1,
			tV<t> a2 
		) => a1.Value + a2.Value;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator-(
			tV<t> a1,
			tV<t> a2
		) => a1.Value - a2.Value;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator*(
			tV<t> a1,
			tV<t> a2
		) => a1.Value * a2.Value;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator/(
			tV<t> a1,
			tV<t> a2
		) => a1.Value / a2.Value;
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator>(
			tV<t> a1,
			tV<t> a2
		) => Vector.GreaterThan<t>(a1.Value, a2.Value);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator<(
			tV<t> a1,
			tV<t> a2
		) => Vector.LessThan<t>(a1.Value, a2.Value);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator>=(
			tV<t> a1,
			tV<t> a2
		) => Vector.GreaterThanOrEqual<t>(a1.Value, a2.Value);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator<=(
			tV<t> a1,
			tV<t> a2
		) => Vector.LessThanOrEqual<t>(a1.Value, a2.Value);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator&(
			tV<t> a1,
			tV<t> a2
		) => Vector.BitwiseAnd<t>(a1.Value, a2.Value);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator|(
			tV<t> a1,
			tV<t> a2
		) => Vector.BitwiseOr<t>(a1.Value, a2.Value);
		
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static tV<t>
		operator^(
			tV<t> a1,
			tV<t> a2
		) => Vector.Xor<t>(a1.Value, a2.Value);
		
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV<t>
	Abs<t>(
		tV<t> a
	) where t : struct => Vector.Abs(a.Value);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV<t>
	Sign<t>(
		tV<t> a
	) where t : struct => (a > Zero<t>()) - (a < Zero<t>());
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV<t>
	V<t>(
		t a
	) where t : struct => new Vector<t>(a);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV<t>
	V<t>(
		System.Span<t> a
	) where t : struct => new Vector<t>(a);
	
	public static tV<t>
	Into<t>(
		this tV<t> aFrom,
		System.Span<t> aTo
	)  where t : struct {
		var Count = mSIMD.VCount<t>();
		for (var I = 0; I < Count; I += 1) {
			aTo[I] = aFrom.Value[I];
		}
		return aFrom;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV<t>
	Zero<t>(
	) where t : struct => Vector<t>.Zero;
}
