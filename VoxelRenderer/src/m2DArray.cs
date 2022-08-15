using static mV2;
using static mStd;

public static class
m2DArray {
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV2
	GetSize<t>(
		this t[,] a
	) => V2(
		a.GetLength(0),
		a.GetLength(1)
	);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static t[,]
	CreateArray<t>(
		this tV2 aSize
	) => new t[aSize.X, aSize.Y];
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static t[,]
	Map<t>(
		this t[,] a,
		tFunc<t, t> aF
	) {
		var Size = a.GetSize();
		var Res = Size.CreateArray<t>();
		for (var Y = 0; Y < Size.Y; Y += 1) {
			for (var X = 0; X < Size.X; X += 1) {
				Res[X, Y] = aF(a[X, Y]);
			}
		}
		return Res;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static t[,]
	_Map<t>(
		this t[,] a,
		tFunc<t, t> aF
	) {
		var Size = a.GetSize();
		for (var Y = 0; Y < Size.Y; Y += 1) {
			for (var X = 0; X < Size.X; X += 1) {
				a[X, Y] = aF(a[X, Y]);
			}
		}
		return a;
	}
}
