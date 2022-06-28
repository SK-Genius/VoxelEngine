using System.Collections.Generic;
using System.Diagnostics;

using static mStd;
using static mMath;
using static mMath2D;
using static mMath3D;
using System.Reflection;


public static class
mVoxelRenderer {
	
	public class
	tDLL {
		public tM3x3[,] Matrixes;
		public tAxis[,][,] NormalPatterns;
		public tFunc<tRenderEnv, tColor[,,], tSprite> GetOrCreateSprite;
		public tFunc<tRenderEnv, tColor[,,], tShadow> GetOrCreateShadow;
		public tFunc<tRenderEnv, tSprite, tV2,  tV3> To3D;
		public tFunc<tRenderEnv, tSprite, tShadow, System.IntPtr, tV2, tDebugRenderMode, tRenderEnv> _RenderToBuffer;
		public tFunc<tRenderEnv, tSprite, tShadow, tColor[,,], tV3, tRenderEnv> _DrawTo;
		public tFunc<tRenderEnv, tInt32, tInt32, tM3x3> GetMatrix;
	}
	
	public static tSprite
	GetOrCreateSprite(
		this tRenderEnv aRenderEnv,
		tColor[,,] aCube
	) => aRenderEnv.DLL.GetOrCreateSprite(aRenderEnv, aCube);
	
	public static tShadow
	GetOrCreateShadow(
		this tRenderEnv aRenderEnv,
		tColor[,,] aCube
	)  =>  aRenderEnv.DLL.GetOrCreateShadow(aRenderEnv, aCube);
	
	private static tM3x3
	GetMatrix(
		this tRenderEnv aRenderEnv,
		tInt32 aDir,
		tInt32 aAngle
	) => aRenderEnv.DLL.GetMatrix(aRenderEnv, aDir, aAngle);

	public static tV3
	To3D(
		this tRenderEnv aRenderEnv,
		tSprite aSprite,
		tV2 aV2
	) => aRenderEnv.DLL.To3D(aRenderEnv, aSprite, aV2);
	
	public static unsafe tRenderEnv
	_RenderToBuffer(
		this tRenderEnv aRenderEnv,
		tSprite aSprite,
		tShadow aShadow,
		System.IntPtr aBuffer,
		tV2 aBufferSize,
		tDebugRenderMode aDebugRenderMode
	) => aRenderEnv.DLL._RenderToBuffer(aRenderEnv, aSprite, aShadow, aBuffer, aBufferSize, aDebugRenderMode);
	
	public static unsafe tRenderEnv
	_DrawTo(
		this tRenderEnv aRenderEnv,
		tSprite aCanvas,
		tShadow aShadow,
		tColor[,,] aCube,
		tV3 aOffset
	) => aRenderEnv.DLL._DrawTo(
		aRenderEnv,
		aCanvas,
		aShadow,
		aCube,
		aOffset
	);

	[System.Flags]
	public enum
	tAxis {
		_ = 0,
		X = 1,
		Y = 2,
		Z = 4,
	};
	
	public sealed class
	tRenderEnv {
		public tInt32 Dir;
		public tInt32 Angle;
		
		public tDLL DLL;
		public System.Runtime.Loader.AssemblyLoadContext DLL_Context;
		
		public tV3 LightDirection;
		
		public tM3x3 M;
		public tM3x3 InvM;
		public tInt32 Det;
		
		public tAxis[,] NormalPattern => this.DLL.NormalPatterns[this.Dir % this.DLL.NormalPatterns.GetLength(0), this.Angle];
		
		public Dictionary<(mMath3D.tM3x3, tColor[,,]), mVoxelRenderer.tSprite> SpriteBuffer  = new();
		public Dictionary<(tV3, tColor[,,]), mVoxelRenderer.tShadow> ShadowBuffer  = new();
	}
	
	[DebuggerDisplay("{Value}")]
	public struct
	tColor {
		public tNat8 Value;
		
		public static tBool
		operator==(
			tColor a1,
			tColor a2
		) => a1.Value == a2.Value;
		
		public static tBool
		operator!=(
			tColor a1,
			tColor a2
		) => a1.Value != a2.Value;
		
		public static tColor
		operator&(
			tColor aColor,
			tNat8 aMask
		) => new tColor{ Value = (tNat8)(aColor.Value & aMask) };
	}
	
	public struct
	tShadow {
		public tV2 Size;
		public tV2 Offset;
		public tInt16[,] Deep;
	}
	
	public struct
	tSprite {
		public tV2 Size;
		public tV2 Offset;
		public tColor[,] Color;
		public (tInt8 U, tInt8 V)[,] Normal;
		public tInt16[,] Deep;
		public tNat8[,] PosBits;
	}
	
	public static tRenderEnv
	CreateEnv(
	) => new tRenderEnv {
	}._LoadDLL(
	)._Update(
	);
	
	class tDLL_Context : System.Runtime.Loader.AssemblyLoadContext {
		public tDLL_Context(
		) : base(isCollectible: true) {
		}
		
		protected override Assembly Load(
			AssemblyName aName
		) => null;
	}
	
	public static tRenderEnv
	_LoadDLL(
		this tRenderEnv aRenderEnv
	) {
		aRenderEnv.DLL = null;
		aRenderEnv.DLL_Context?.Unload();
		
		aRenderEnv.DLL_Context = new tDLL_Context();
		aRenderEnv.DLL = aRenderEnv.DLL_Context.LoadFromStream(
			new System.IO.MemoryStream(
				System.IO.File.ReadAllBytes(
					"../VoxelRenderer.HotReload/bin/Debug/net7.0/VoxelRenderer.HotReload.dll"
				)
			)
		).GetType(
			"mVoxelRenderer_HotReload"
		).GetMethod(
			"Create"
		).CreateDelegate<
			tFunc<tDLL>
		>()();
		
		return aRenderEnv;
	}
	
	public static tV3
	GetSize(
		this tColor[,,] aCube
	) => V3(
		aCube.GetLength(0),
		aCube.GetLength(1),
		aCube.GetLength(2)
	);
	
	public static tRenderEnv
	_SetLightDirection(
		this tRenderEnv a,
		mMath3D.tV3 aLightDirection
	) {
		a.LightDirection = aLightDirection;
		return a;
	}
	
	public static tRenderEnv
	_Update(
		this tRenderEnv a
	) {
		var QuarterParts = a.DLL.Matrixes.GetLength(0);
		while (a.Dir < 0) { a.Dir += 4 * QuarterParts; }
		while (a.Dir >= 4 * QuarterParts) { a.Dir -= 4 * QuarterParts; }
		mMath.Clamp(ref a.Angle, 0, 4);
		a.M = a.GetMatrix(a.Dir, a.Angle);
		(a.InvM, a.Det) = mMath3D.Inverse(a.M);
		return a;
	}
	
	public enum
	tDebugRenderMode {
		None,
		Normal,
		Deep,
		Pos,
		PosBits,
	};
	
	public static tColor
	RGB(
		tNat8 aR,
		tNat8 aG,
		tNat8 aB
	) => RGBA(aR, aG, aB, false);
	
	public static tColor
	RGBA(
		tNat8 aR,
		tNat8 aG,
		tNat8 aB,
		tBool aIsTransparent
	) => new tColor {
		Value =  (tNat8)(
			(
				aIsTransparent
				? 0b_1000_0000
				: 0
			) | (
				(aR * 5 + aG) * 5 + aB + 1
			)
		)
	};
	
	public static tShadow
	CreateShadow(
		tV2 aSize,
		tV2 aOffset
	) => new tShadow {
		Size = aSize,
		Offset = aOffset,
		Deep = new tInt16[aSize.X, aSize.Y],
	};
	
	public static tSprite
	CreateSprite(
		tV2 aSize,
		tV2 aOffset
	) => new tSprite {
		Size = aSize,
		Offset = aOffset,
		Color = new tColor[aSize.X, aSize.Y],
		Deep = new tInt16[aSize.X, aSize.Y],
		Normal = new (tInt8 U, tInt8 V)[aSize.X, aSize.Y],
		PosBits = new tNat8[aSize.X, aSize.Y],
	};
	
	public static tV2
	GetSpriteSize(
		tInt32 aCubeSize,
		mMath3D.tM3x3 aM
	) {
		var D = aCubeSize;
		var V3s = new [] {
			V3(+D, +D, +D),
			V3(+D, +D, -D),
			V3(+D, -D, +D),
			V3(+D, -D, -D),
			V3(-D, +D, +D),
			V3(-D, +D, -D),
			V3(-D, -D, +D),
			V3(-D, -D, -D),
		};
		var MaxX = 0;
		var MaxY = 0;
		foreach (var V3 in V3s) {
			var V2 = V3 * aM;
			MaxX = mMath.Max(MaxX, V2.X);
			MaxY = mMath.Max(MaxY, V2.Y);
		}
		return V2(MaxX + 2, MaxY + 2);
	} 
	
	public static tV2
	GetShadowSize(
		tV3 aCubeSize,
		mMath3D.tV3 aLightDirection
	) {
		var (MainAxis, _) = GetMainAxis(aLightDirection);
		
		return MainAxis switch {
			tAxis.X => mMath2D.V2(
				aCubeSize.Z + mMath.Abs(aLightDirection.Z * aCubeSize.X / aLightDirection.X),
				aCubeSize.Y + mMath.Abs(aLightDirection.Y * aCubeSize.X / aLightDirection.X)
			),
			tAxis.Y => mMath2D.V2(
				aCubeSize.X + mMath.Abs(aLightDirection.X * aCubeSize.Y / aLightDirection.Y),
				aCubeSize.Z + mMath.Abs(aLightDirection.Z * aCubeSize.Y / aLightDirection.Y)
			),
			tAxis.Z => mMath2D.V2(
				aCubeSize.X + mMath.Abs(aLightDirection.X * aCubeSize.Z / aLightDirection.Z),
				aCubeSize.Y + mMath.Abs(aLightDirection.Y * aCubeSize.Z / aLightDirection.Z)
			)
		};
	}
	
	public static tV2
	GetDeepSize(
		tV3 LightDirection,
		tInt32 aCubeSize
	) {
		var V = LightDirection * aCubeSize;
		return V2(aCubeSize, aCubeSize) + GetMainAxis(LightDirection) switch {
			(tAxis.Z, _) => V2(mMath.Abs(V.X / LightDirection.Z), mMath.Abs(V.Y / LightDirection.Z)),
			(tAxis.Y, _) => V2(mMath.Abs(V.X / LightDirection.Y), mMath.Abs(V.Z / LightDirection.Y)),
			(tAxis.X, _) => V2(mMath.Abs(V.Z / LightDirection.X), mMath.Abs(V.Y / LightDirection.X)),
		};
	}
	
	public static tShadow
	_Clear(
		this tShadow aGrid
	) {
		var MaxX = aGrid.Size.X;
		var MaxY = aGrid.Size.Y;
		
		for (var X = 0; X < MaxX; X += 1) {
			for (var Y = 0; Y < MaxY; Y += 1) {
				aGrid.Deep[X, Y] = short.MaxValue;
			}
		}
		
		return aGrid;
	}
	
	public static tSprite
	_Clear(
		this tSprite aGrid
	) {
		var MaxX = aGrid.Size.X;
		var MaxY = aGrid.Size.Y;
		
		for (var X = 0; X < MaxX; X += 1) {
			for (var Y = 0; Y < MaxY; Y += 1) {
				aGrid.Color[X, Y] = default;
				aGrid.Deep[X, Y] = short.MaxValue;
			}
		}
		
		return aGrid;
	}
	
	public static (tAxis, tInt32)
	GetMainAxis(
		tV3 aDirection
	) {
		if (
			mMath.Abs(aDirection.Z) >= mMath.Abs(aDirection.X) &&
			mMath.Abs(aDirection.Z) >= mMath.Abs(aDirection.Y)
		) {
			return (tAxis.Z, mMath.Sign(aDirection.Z));
		} else if (
			mMath.Abs(aDirection.Y) >= mMath.Abs(aDirection.X)
		) {
			return (tAxis.Y, mMath.Sign(aDirection.Y));
		} else {
			return (tAxis.X, mMath.Sign(aDirection.X));
		}
	}
	
	public static tV3
	ToRGB(
		tColor aColor
	) {
		if (aColor == default) {
			return default;
		}
		aColor.Value -= 1;
		return mMath3D.V3(
			(aColor.Value / 5 / 5) % 5,
			(aColor.Value / 5) % 5,
			aColor.Value % 5
		) * 63;
	}
	
	public static tNat32
	ToRGB32(
		tNat8 aColor
	) {
		if (aColor == 0) {
			return 0;
		}
		aColor -= 1;
		var B = 63 * (aColor % 5);
		var G = 63 * ((aColor / 5) % 5);
		var R = 63 * ((aColor / 5 / 5) % 5);
		var A = tNat8.MaxValue;
		return (tNat32)((A << 24) | (R << 16) | (G << 8) | B);
	}
	
}
