using System.Diagnostics;

public static class
mStd {
	
	public delegate tRes tFunc<out tRes>();
	public delegate tRes tFunc<in t1, out tRes>(t1 a1);
	public delegate tRes tFunc<in t1, in t2, out tRes>(t1 a1, t2 a2);
	public delegate tRes tFunc<in t1, in t2, in t3, out tRes>(t1 a1, t2 a2, t3 a3);
	public delegate tRes tFunc<in t1, in t2, in t3, in t4, out tRes>(t1 a1, t2 a2, t3 a3, t4 a4);
	public delegate tRes tFunc<in t1, in t2, in t3, in t4, in t5, out tRes>(t1 a1, t2 a2, t3 a3, t4 a4, t5 a5);
	public delegate tRes tFunc<in t1, in t2, in t3, in t4, in t5, in t6, out tRes>(t1 a1, t2 a2, t3 a3, t4 a4, t5 a5, t6 a6);
	
	public delegate ref t tMeth<t>();
	public delegate ref t tMeth<t, in t1>(ref t a, t1 a1);
	public delegate ref t tMeth<t, in t1, in t2>(ref t a, t1 a1, t2 a2);
	public delegate ref t tMeth<t, in t1, in t2, in t3>(ref t a, t1 a1, t2 a2, t3 a3);
	public delegate ref t tMeth<t, in t1, in t2, in t3, in t4>(ref t a, t1 a1, t2 a2, t3 a3, t4 a4);
	public delegate ref t tMeth<t, in t1, in t2, in t3, in t4, in t5>(ref t a, t1 a1, t2 a2, t3 a3, t4 a4, t5 a5);
	public delegate ref t tMeth<t, in t1, in t2, in t3, in t4, in t5, in t6>(ref t a, t1 a1, t2 a2, t3 a3, t4 a4, t5 a5, t6 a6);
	
	[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static tInt32
	GetSize<t>(
		this t[] a
	) => a.Length;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
	public static void
	Assert(
		tBool aCond
	) {
		if (!aCond) {
			throw new System.Exception();
		}
	}
}
