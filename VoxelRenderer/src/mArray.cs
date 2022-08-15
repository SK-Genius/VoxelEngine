using static mV2;

public static class
mArray {
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	GetSize<t>(
		this t[] a
	) => a.Length;
}
