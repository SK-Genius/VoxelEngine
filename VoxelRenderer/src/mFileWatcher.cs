using System.IO;

public static class
mFileWatcher {
	
	public class
	tFileWatcher {
		public FileInfo File { get; private set; }
		public tBool HasUpdated;
		private FileSystemWatcher DLL_Watcher;
		
		public tFileWatcher(
			FileInfo aFile
		) {
			this.SetFile(aFile);
		}
		
		public void
		SetFile(
			FileInfo aFile
		) {
			this.File = aFile;
			this.HasUpdated = true;
			this.DLL_Watcher?.Dispose();
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
