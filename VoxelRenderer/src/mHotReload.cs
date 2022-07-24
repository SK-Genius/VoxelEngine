using static mStd;
using static mFileWatcher;

using System.IO;
using System.Linq;
using System.Reflection;

public static class
mHotReload {
	
	class tDLL_Context : System.Runtime.Loader.AssemblyLoadContext {
		public tDLL_Context(
		) : base(isCollectible: true) {
		}
		
		protected override Assembly Load(
			AssemblyName aName
		) => null;
	}
	
	public class
	tHotReload<t> {
		public tFileWatcher DLL_File;
		public t DLL;
		public System.Runtime.Loader.AssemblyLoadContext DLL_Context;
		
		public tBool HasNewDLL => this.DLL_File.HasUpdated;
		
		public tHotReload(
			DirectoryInfo aFolder,
			string aFileName
		) {
			this.DLL_File = new tFileWatcher(aFolder, aFileName);
			this._LoadDLL();
		}
	}
	
	public static tHotReload<t>
	_LoadDLL<t>(
		this tHotReload<t> a
	) {
		a.DLL_Context?.Unload();
		a.DLL_Context = new tDLL_Context();
		
		a.DLL = a.DLL_Context.LoadFromStream(
			new System.IO.MemoryStream(
				System.IO.File.ReadAllBytes(
					a.DLL_File.File.FullName
				)
			)
		).GetTypes(
		).First(
			_ => _.Name.EndsWith("_HotReload")
		).GetMethod(
			"Create"
		).CreateDelegate<
			tFunc<t>
		>()();
		
		a.DLL_File.HasUpdated = false;
		return a;
	}
}
