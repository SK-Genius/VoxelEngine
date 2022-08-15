using static mV3;
using static mStd;

public static class
m3DArray {
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	GetSize<t>(
		this t[,,] a
	) => V3(
		a.GetLength(0),
		a.GetLength(1),
		a.GetLength(2)
	);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static t[,,]
	CreateArray<t>(
		this tV3 aSize
	) => new t[aSize.X, aSize.Y, aSize.Z];
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static t[,,]
	Map<t>(
		this t[,,] a,
		tFunc<t, t> aF
	) {
		var Size = a.GetSize();
		var Res = Size.CreateArray<t>();
		for (var Z = 0; Z < Size.Z; Z += 1) {
			for (var Y = 0; Y < Size.Y; Y += 1) {
				for (var X = 0; X < Size.X; X += 1) {
					Res[X, Y, Z] = aF(a[X, Y, Z]);
				}
			}
		}
		return Res;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static t[,,]
	_Map<t>(
		this t[,,] a,
		tFunc<t, t> aF
	) {
		var Size = a.GetSize();
		for (var Z = 0; Z < Size.Z; Z += 1) {
			for (var Y = 0; Y < Size.Y; Y += 1) {
				for (var X = 0; X < Size.X; X += 1) {
					a[X, Y, Z] = aF(a[X, Y, Z]);
				}
			}
		}
		return a;
	}
}
