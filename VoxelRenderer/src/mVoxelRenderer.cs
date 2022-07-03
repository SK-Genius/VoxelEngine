using System.Collections.Generic;
using System.Diagnostics;

using static mStd;
using static mMath;
using static mMath2D;
using static mMath3D;
using System.Reflection;
using System.IO;
using System.Diagnostics.CodeAnalysis;

public static class
mVoxelRenderer {
	
	public class
	tDLL {
		public tM3x3[,] Matrixes;
		public tAxis[,][,] NormalPatterns;
		public tFunc<tRenderEnv, tBlock, tSprite> GetOrCreateSprite;
		public tFunc<tRenderEnv, tBlock, tShadow> GetOrCreateShadow;
		public tFunc<tRenderEnv, tSprite, tV2,  tV3> To3D;
		public tMeth<tRenderEnv, tSprite, tShadow, System.IntPtr, tV2, tDebugRenderMode> _RenderToBuffer;
		public tMeth<tRenderEnv, tSprite, tShadow, tBlock, tV3> _DrawTo;
		public tFunc<tRenderEnv, tInt32, tInt32, tM3x3> GetMatrix;
	}
	
	public struct
	tBlock {
		public tInt32 Id;
		public tV3 Offset;
		public tColor[,,] Colors;
		
		public override int
		GetHashCode(
		) => this.Id;

		public override bool
		Equals(
			object? a
		) => this.Id == ((tBlock)a).Id;
	}
	
	public static tBlock
	CreateBlock(
		tV3 aOffset,
		tColor[,,] aColors
	) => new tBlock {
		Id = HashBlock(aColors),
		Offset = aOffset,
		Colors = aColors,
	};
	
	private static tInt32
	HashBlock(
		tColor[,,] aColors
	) {
		var Hash = 0;
		for (var Z = 0; Z < aColors.GetLength(2); Z += 1) {
			for (var Y = 0; Y < aColors.GetLength(1); Y += 1) {
				for (var X = 0; X < aColors.GetLength(0); X += 1) {
					Hash ^= Hash << 13;
					Hash ^= Hash >> 17;
					Hash ^= Hash << 5;
					Hash ^= 0x12_34_56_78;
					Hash ^= aColors[X, Y, Z].Value;
				}
			}
		}
		return Hash;
	}
	
	public static tSprite
	GetOrCreateSprite(
		this ref tRenderEnv aRenderEnv,
		tBlock aBlock
	) => aRenderEnv.DLL.GetOrCreateSprite(aRenderEnv, aBlock);
	
	public static tShadow
	GetOrCreateShadow(
		this ref tRenderEnv aRenderEnv,
		tBlock aBlock
	)  =>  aRenderEnv.DLL.GetOrCreateShadow(aRenderEnv, aBlock);
	
	private static tM3x3
	GetMatrix(
		this ref tRenderEnv aRenderEnv,
		tInt32 aDir,
		tInt32 aAngle
	) => aRenderEnv.DLL.GetMatrix(aRenderEnv, aDir, aAngle);
	
	public static tV3
	To3D(
		this ref tRenderEnv aRenderEnv,
		tSprite aSprite,
		tV2 aV2
	) => aRenderEnv.DLL.To3D(aRenderEnv, aSprite, aV2);
	
	public static unsafe ref tRenderEnv
	_RenderToBuffer(
		this ref tRenderEnv aRenderEnv,
		tSprite aSprite,
		tShadow aShadow,
		System.IntPtr aBuffer,
		tV2 aBufferSize,
		tDebugRenderMode aDebugRenderMode
	) => ref aRenderEnv.DLL._RenderToBuffer(ref aRenderEnv, aSprite, aShadow, aBuffer, aBufferSize, aDebugRenderMode);
	
	public static unsafe ref tRenderEnv
	_DrawTo(
		this ref tRenderEnv aRenderEnv,
		tSprite aCanvas,
		tShadow aShadow,
		tBlock aBlock,
		tV3 aOffset
	) => ref aRenderEnv.DLL._DrawTo(
		ref aRenderEnv,
		aCanvas,
		aShadow,
		aBlock,
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
	
	public struct
	tRenderEnv {
		public tRenderEnv() {}
		public tInt32 Dir;
		public tInt32 Angle;
		
		public static FileInfo DLL_File = new DirectoryInfo(
			".."
		).GetFiles(
			"VoxelRenderer.HotReload.dll",
			new EnumerationOptions {
				RecurseSubdirectories = true,
				MatchCasing = MatchCasing.CaseInsensitive,
			}
		)[0];
		public tBool HasNewDLL = false;
		public FileSystemWatcher DLL_Watcher;
		public tDLL DLL;
		public System.Runtime.Loader.AssemblyLoadContext DLL_Context;
		
		public tV3 LightDirection;
		
		public tM3x3 M;
		public tM3x3 InvM;
		public tInt32 Det;
		
		public tAxis[,] NormalPattern => this.DLL.NormalPatterns[this.Dir % this.DLL.NormalPatterns.GetLength(0), this.Angle];
		
		public Dictionary<(mMath3D.tM3x3, tBlock), mVoxelRenderer.tSprite> SpriteBuffer  = new();
		public Dictionary<(tV3, tBlock), mVoxelRenderer.tShadow> ShadowBuffer  = new();
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
	) {
		var RenderEnv = new tRenderEnv();
		RenderEnv.DLL_Watcher = new FileSystemWatcher(
			tRenderEnv.DLL_File.Directory.FullName
		) {
            NotifyFilter = NotifyFilters.Attributes
				| NotifyFilters.CreationTime
				| NotifyFilters.DirectoryName
				| NotifyFilters.FileName
				| NotifyFilters.LastAccess
				| NotifyFilters.LastWrite
				| NotifyFilters.Security
				| NotifyFilters.Size,
			IncludeSubdirectories = true,
			Filter = tRenderEnv.DLL_File.Name,
			EnableRaisingEvents = true,
		};
        RenderEnv.DLL_Watcher.Changed += (_, _) => {
			RenderEnv.HasNewDLL = true;
		};
		
		return RenderEnv
		._LoadDLL()
		._Update();
	}
	
	class tDLL_Context : System.Runtime.Loader.AssemblyLoadContext {
		public tDLL_Context(
		) : base(isCollectible: true) {
		}
		
		protected override Assembly Load(
			AssemblyName aName
		) => null;
	}
	
	public static ref tRenderEnv
	_LoadDLL(
		this ref tRenderEnv aRenderEnv
	) {
		aRenderEnv.DLL = null;
		aRenderEnv.DLL_Context?.Unload();
		
		aRenderEnv.DLL_Context = new tDLL_Context();
		aRenderEnv.DLL = aRenderEnv.DLL_Context.LoadFromStream(
			new System.IO.MemoryStream(
				System.IO.File.ReadAllBytes(
					tRenderEnv.DLL_File.FullName
				)
			)
		).GetType(
			"mVoxelRenderer_HotReload"
		).GetMethod(
			"Create"
		).CreateDelegate<
			tFunc<tDLL>
		>()();
		
		aRenderEnv.HasNewDLL = false;
		return ref aRenderEnv;
	}
	
	public static tV3
	GetSize(
		this tBlock aBlock
	) => V3(
		aBlock.Colors.GetLength(0),
		aBlock.Colors.GetLength(1),
		aBlock.Colors.GetLength(2)
	);
	
	public static ref tRenderEnv
	_SetLightDirection(
		this ref tRenderEnv a,
		mMath3D.tV3 aLightDirection
	) {
		a.LightDirection = GetMainAxis(aLightDirection) switch {
			(tAxis.X, int Sign) => V3(Sign * 9, aLightDirection.Y, aLightDirection.Z),
			(tAxis.Y, int Sign) => V3(aLightDirection.X, Sign * 9, aLightDirection.Z),
			(tAxis.Z, int Sign) => V3(aLightDirection.X, aLightDirection.Y, Sign * 9),
			_ => aLightDirection,
		};
		
		return ref a;
	}
	
	public static ref tRenderEnv
	_Update(
		this ref tRenderEnv a
	) {
		if (a.HasNewDLL) {
			a._LoadDLL();
		}
		var QuarterParts = a.DLL.Matrixes.GetLength(0);
		while (a.Dir < 0) { a.Dir += 4 * QuarterParts; }
		while (a.Dir >= 4 * QuarterParts) { a.Dir -= 4 * QuarterParts; }
		mMath.Clamp(ref a.Angle, 0, 4);
		a.M = a.GetMatrix(a.Dir, a.Angle);
		(a.InvM, a.Det) = mMath3D.Inverse(a.M);
		return ref a;
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
	
	public static tV3
	GetShadowUV(
		tV3 aWorldOffset,
		tV3 aLightDirection
	) {
		var (MainAxis, AxisSign) = GetMainAxis(aLightDirection);
		
		var X_ = aWorldOffset.X;
		var Y_ = aWorldOffset.Y;
		var Z_ = aWorldOffset.Z;
		
		var Deep = default(tInt32);
		var UV = default(tV2);
		
		switch (MainAxis) {
			case tAxis.X: {
				UV = V2(-Y_, Z_) + aLightDirection.YZ() * X_ / aLightDirection.X;
				Deep = -X_;
				break;
			}
			case tAxis.Y: {
				UV = V2(-X_, Z_) + aLightDirection.XZ() * Y_ / aLightDirection.Y;
				Deep = -Y_;
				break;
			}
			case tAxis.Z: {
				UV = V2(X_, Y_) + aLightDirection.XY() * Z_ / aLightDirection.Z; 
				Deep = Z_;
				break;
			}
		}
		
		return V3(UV, AxisSign * Deep);
	}
	
	public static tV2
	GetShadowSize(
		tV3 aCubeSize,
		mMath3D.tV3 aLightDirection
	) {
		var (MainAxis, _) = GetMainAxis(aLightDirection);
		
		var L = V3(
			aLightDirection.X.Abs(),
			aLightDirection.Y.Abs(),
			aLightDirection.Z.Abs()
		);
		
		var UV = default(tV2);
		
		switch (MainAxis) {
			case tAxis.X: {
				UV = aCubeSize.YZ() + L.YZ() * aCubeSize.X / L.X;
				break;
			}
			case tAxis.Y: {
				UV = aCubeSize.XZ() + L.XZ() * aCubeSize.Y / L.Y;
				break;
			}
			case tAxis.Z: {
				UV = aCubeSize.XY() + L.XY() * aCubeSize.Z / L.Z; 
				break;
			}
		}
		
		return UV + V2(2, 2);;
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
