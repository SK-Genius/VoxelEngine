using System.Collections.Generic;

using static m2DArray;
using static m3DArray;
using static mV3;
using static mVoxelRenderer;

public static class
mBlockModifier {
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tColor[,,]
	Scale2(
		this tColor[,,] a
	) {
		var Size = a.GetSize();
		var Result = (Size * 2).CreateArray<tColor>();
		
		for (var Z = 0; Z < Size.Z; Z += 1) {
			for (var Y = 0; Y < Size.Y; Y += 1) {
				for (var X = 0; X < Size.X; X += 1) {
					var C = a[X, Y, Z];
					Result[2*X + 0, 2*Y + 0, 2*Z + 0] = C;
					Result[2*X + 0, 2*Y + 0, 2*Z + 1] = C;
					Result[2*X + 0, 2*Y + 1, 2*Z + 0] = C;
					Result[2*X + 0, 2*Y + 1, 2*Z + 1] = C;
					Result[2*X + 1, 2*Y + 0, 2*Z + 0] = C;
					Result[2*X + 1, 2*Y + 0, 2*Z + 1] = C;
					Result[2*X + 1, 2*Y + 1, 2*Z + 0] = C;
					Result[2*X + 1, 2*Y + 1, 2*Z + 1] = C;
				}
			}
		}
		
		return Result;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tColor[,,]
	Scale3(
		this tColor[,,] a
	) {
		var Size = a.GetSize();
		var Result = (Size * 3).CreateArray<tColor>();
		
		for (var Z = 0; Z < Size.Z; Z += 1) {
			for (var Y = 0; Y < Size.Y; Y += 1) {
				for (var X = 0; X < Size.X; X += 1) {
					var C = a[X, Y, Z];
					Result[3*X + 0, 3*Y + 0, 3*Z + 0] = C;
					Result[3*X + 0, 3*Y + 0, 3*Z + 1] = C;
					Result[3*X + 0, 3*Y + 0, 3*Z + 2] = C;
					Result[3*X + 0, 3*Y + 1, 3*Z + 0] = C;
					Result[3*X + 0, 3*Y + 1, 3*Z + 1] = C;
					Result[3*X + 0, 3*Y + 1, 3*Z + 2] = C;
					Result[3*X + 0, 3*Y + 2, 3*Z + 0] = C;
					Result[3*X + 0, 3*Y + 2, 3*Z + 1] = C;
					Result[3*X + 0, 3*Y + 2, 3*Z + 2] = C;
					Result[3*X + 1, 3*Y + 0, 3*Z + 0] = C;
					Result[3*X + 1, 3*Y + 0, 3*Z + 1] = C;
					Result[3*X + 1, 3*Y + 0, 3*Z + 2] = C;
					Result[3*X + 1, 3*Y + 1, 3*Z + 0] = C;
					Result[3*X + 1, 3*Y + 1, 3*Z + 1] = C;
					Result[3*X + 1, 3*Y + 1, 3*Z + 2] = C;
					Result[3*X + 1, 3*Y + 2, 3*Z + 0] = C;
					Result[3*X + 1, 3*Y + 2, 3*Z + 1] = C;
					Result[3*X + 1, 3*Y + 2, 3*Z + 2] = C;
					Result[3*X + 2, 3*Y + 0, 3*Z + 0] = C;
					Result[3*X + 2, 3*Y + 0, 3*Z + 1] = C;
					Result[3*X + 2, 3*Y + 0, 3*Z + 2] = C;
					Result[3*X + 2, 3*Y + 1, 3*Z + 0] = C;
					Result[3*X + 2, 3*Y + 1, 3*Z + 1] = C;
					Result[3*X + 2, 3*Y + 1, 3*Z + 2] = C;
					Result[3*X + 2, 3*Y + 2, 3*Z + 0] = C;
					Result[3*X + 2, 3*Y + 2, 3*Z + 1] = C;
					Result[3*X + 2, 3*Y + 2, 3*Z + 2] = C;
				}
			}
		}
		
		return Result;
	}
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tColor[,,]
	SliceByMinMax(
		this tColor[,,] a,
		tV3 aMin,
		tV3 aMax
	) => a.SliceByMinSize(aMin, aMax - aMin);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tColor[,,]
	SliceByMinSize(
		this tColor[,,] a,
		tV3 aMin,
		tV3 aSize
	) {
		var Result = aSize.CreateArray<tColor>();
		for (var Z = 0; Z < aSize.Z; Z += 1) {
			for (var Y = 0; Y < aSize.Y; Y += 1) {
				for (var X = 0; X < aSize.X; X += 1) {
					Result[X, Y, Z] = a[aMin.X + X, aMin.Y + Y, aMin.Z + Z];
				}
			}
		}
		return Result;
	}
	
	//[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static tV3
	//BlockCenter(
	//	tV3 aPos,
	//	tV3 aBlockSize
	//) => BlockBegin(aPos, aBlockSize) + (aBlockSize >> 1);
	//
	//[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static tV3
	//BlockBegin(
	//	tV3 aPos,
	//	tV3 aBlockSize
	//) => BlockIndexBegin(aPos - (aBlockSize >> 1), aBlockSize) + (aBlockSize >>1);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	BlockIndexCenter(
		tV3 aPos,
		tV3 aBlockSize
	) => BlockIndexBegin(aPos, aBlockSize) + (aBlockSize >> 1);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tV3
	BlockIndexBegin(
		tV3 aPos,
		tV3 aBlockSize
	) => aPos - (aPos % aBlockSize);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<(tV3 Pos, tColor[,,] Block)>
	Split(
		this tColor[,,] a,
		tV3 aOffset,
		tV3 aBlockSize
	) {
		var Size = a.GetSize();
		var Count = a.GetSize() / aBlockSize;
		for (var Z = 0; Z < Size.Z; Z += aBlockSize.Z) {
			for (var Y = 0; Y < Size.Y; Y += aBlockSize.Y) {
				for (var X = 0; X < Size.X; X += aBlockSize.X) {
					var Min = V3(X, Y, Z);
					var Block = a.SliceByMinSize(Min, aBlockSize);
					yield return (Min - aOffset + aBlockSize / 2, Block);
				}
			}
		}
	}
}
