using static mStd;

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
		public tHotReload(
			DirectoryInfo aFolder,
			string aFileName
		) {
			this.DLL_File = aFolder.GetFiles(
				aFileName,
				new EnumerationOptions {
					RecurseSubdirectories = true,
					MatchCasing = MatchCasing.CaseInsensitive,
				}
			)[0];
			
			this.DLL_Watcher = new FileSystemWatcher(
				this.DLL_File.Directory.FullName
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
				Filter = this.DLL_File.Name,
				EnableRaisingEvents = true,
			};
			this.DLL_Watcher.Changed += (_, _) => {
				this.HasNewDLL = true;
			};
			
			this._LoadDLL();
		}
		
		public FileInfo DLL_File;
		public tBool HasNewDLL = false;
		public FileSystemWatcher DLL_Watcher;
		public t DLL;
		public System.Runtime.Loader.AssemblyLoadContext DLL_Context;
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
					a.DLL_File.FullName
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
		
		a.HasNewDLL = false;
		return a;
	}
}
