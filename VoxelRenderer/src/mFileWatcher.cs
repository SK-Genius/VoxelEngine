using System.IO;

public static class
mFileWatcher {
	
	public class
	tFileWatcher {
		public FileInfo File;
		public tBool HasUpdated;
		public FileSystemWatcher DLL_Watcher;
		
		public tFileWatcher(
			FileInfo aFile
		) {
			this.File = aFile;
			
			this.DLL_Watcher = new FileSystemWatcher(
				this.File.Directory.FullName
			) {
				NotifyFilter =
					NotifyFilters.CreationTime |
					NotifyFilters.LastWrite |
					NotifyFilters.Size,
				IncludeSubdirectories = true,
				Filter = this.File.Name,
				EnableRaisingEvents = true,
			};
			this.DLL_Watcher.Changed += (_, _) => {
				this.HasUpdated = true;
			};
		}
	}
}
