using System.Collections.Generic;
using static mMath3D;
using static mVoxelRenderer;

public static class
mBlockModifier {
	
	public static tColor[,,]
	Scale2(
		this tColor[,,] a
	) {
		var Size = a.GetSize();
		var Size_ = Size * 2;
		var Result = new tColor[Size_.X, Size_.Y, Size_.Z];
		
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
	
	public static tColor[,,]
	Scale3(
		this tColor[,,] a
	) {
		var Size = a.GetSize();
		var Size_ = Size * 3;
		var Result = new tColor[Size_.X, Size_.Y, Size_.Z];
		
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
	
	public static tColor[,,]
	SliceByMinMax(
		this tColor[,,] a,
		tV3 aMin,
		tV3 aMax
	) => a.SliceByMinSize(aMin, aMax - aMin);
	
	public static tColor[,,]
	SliceByMinSize(
		this tColor[,,] a,
		tV3 aMin,
		tV3 aSize
	) {
		var Result = new tColor[aSize.X, aSize.Y, aSize.Z];
		for (var Z = 0; Z < aSize.Z; Z += 1) {
			for (var Y = 0; Y < aSize.Y; Y += 1) {
				for (var X = 0; X < aSize.X; X += 1) {
					Result[X, Y, Z] = a[aMin.X + X, aMin.Y + Y, aMin.Z + Z];
				}
			}
		}
		return Result;
	}
	
	public static tV3
	BlockCenter(
		tV3 aPos,
		tV3 aBlockSize
	) => ((aPos + (aBlockSize / 2) * aPos.Sign()) / aBlockSize) * aBlockSize;
	
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
