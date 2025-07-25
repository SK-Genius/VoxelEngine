using static mV2;

public static class
mPixelShader {
	
	public enum
	tBorder {
		Zero,
		Min,
		Max,
		Last,
		Repeat,
		Mirror,
	}
	
	public struct
	tGrid32 {
		public tInt32[] Values;
		public tV2 Size;
		public tBorder BorderH;
		public tBorder BorderV;
	}
	
	public static tGrid32
	Grid32(
		tV2 aSize,
		tBorder aBorderH,
		tBorder aBorderV
	) => new tGrid32 {
		Size = aSize,
		Values = new tInt32[aSize.X * aSize.Y],
		BorderH = aBorderH,
		BorderV = aBorderV,
	};
	
	public static bool
	TryAdd(
		tGrid32 aIn1,
		tGrid32 aIn2,
		tGrid32 aOut
	) {
		if (
			aIn2.Size != aIn1.Size ||
			aOut.Size != aIn1.Size
		) {
			return false;
		}
		
		var Size = aIn1.Size.X * aIn1.Size.Y;
		
		var Count = mSIMD.VCount<tInt32>();
		
		var I = 0; 
		for (; I + Count < Size; I += Count) {
			var Range = I .. (I + Count);
			var _1 = mSIMD.V<tInt32>(aIn1.Values[Range]);
			var _2 = mSIMD.V<tInt32>(aIn2.Values[Range]);
			var _3 = _1 + _2;
			_3.Into(aOut.Values[Range]);
		}
		for (; I < Size; I += 1) {
			aOut.Values[I] = aIn1.Values[I] + aIn2.Values[I];
		}
		return true;
	}
	
	public static bool
	TryAdd(
		tGrid32 aIn1,
		tInt32 aIn2,
		tGrid32 aOut
	) {
		if (
			aOut.Size != aIn1.Size
		) {
			return false;
		}
		
		var Size = aIn1.Size.X * aIn1.Size.Y;
		
		var Count = mSIMD.VCount<tInt32>();
		
		var I = 0; 
		for (; I + Count < Size; I += Count) {
			var Range = I .. (I + Count);
			var _1 = mSIMD.V<tInt32>(aIn1.Values[Range]);
			var _2 = mSIMD.V(aIn2);
			var _3 = _1 + _2;
			_3.Into(aOut.Values[Range]);
		}
		for (; I < Size; I += 1) {
			aOut.Values[I] = aIn1.Values[I] + aIn2;
		}
		return true;
	}
	
	public static bool
	TryAdd(
		tInt32 aIn1,
		tGrid32 aIn2,
		tGrid32 aOut
	) => TryAdd(aIn2, aIn1, aOut);
	
	public static bool
	TrySub(
		tGrid32 aIn1,
		tGrid32 aIn2,
		tGrid32 aOut
	) {
		if (
			aIn2.Size != aIn1.Size ||
			aOut.Size != aIn1.Size
		) {
			return false;
		}
		
		var Size = aIn1.Size.X * aIn1.Size.Y;
		
		var Count = mSIMD.VCount<tInt32>();
		
		var I = 0; 
		for (; I + Count < Size; I += Count) {
			var Range = I .. (I + Count);
			var _1 = mSIMD.V<tInt32>(aIn1.Values[Range]);
			var _2 = mSIMD.V<tInt32>(aIn2.Values[Range]);
			var _3 = _1 - _2;
			_3.Into(aOut.Values[Range]);
		}
		for (; I < Size; I += 1) {
			aOut.Values[I] = aIn1.Values[I] - aIn2.Values[I];
		}
		return true;
	}
	
	public static bool
	TrySub(
		tGrid32 aIn1,
		tInt32 aIn2,
		tGrid32 aOut
	) {
		if (
			aOut.Size != aIn1.Size
		) {
			return false;
		}
		
		var Size = aIn1.Size.X * aIn1.Size.Y;
		
		var Count = mSIMD.VCount<tInt32>();
		
		var I = 0; 
		for (; I + Count < Size; I += Count) {
			var Range = I .. (I + Count);
			var _1 = mSIMD.V<tInt32>(aIn1.Values[Range]);
			var _2 = mSIMD.V(aIn2);
			var _3 = _1 - _2;
			_3.Into(aOut.Values[Range]);
		}
		for (; I < Size; I += 1) {
			aOut.Values[I] = aIn1.Values[I] - aIn2;
		}
		return true;
	}
	
	public static bool
	TrySub(
		tInt32 aIn1,
		tGrid32 aIn2,
		tGrid32 aOut
	) {
		if (
			aOut.Size != aIn2.Size
		) {
			return false;
		}
		
		var Size = aIn2.Size.X * aIn2.Size.Y;
		
		var Count = mSIMD.VCount<tInt32>();
		
		var I = 0; 
		for (; I + Count < Size; I += Count) {
			var Range = I .. (I + Count);
			var _1 = mSIMD.V(aIn1);
			var _2 = mSIMD.V<tInt32>(aIn2.Values[Range]);
			var _3 = _1 - _2;
			_3.Into(aOut.Values[Range]);
		}
		for (; I < Size; I += 1) {
			aOut.Values[I] = aIn1 - aIn2.Values[I];
		}
		return true;
	}
}
