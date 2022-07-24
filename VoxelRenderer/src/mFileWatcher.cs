using System.IO;

public static class
mFileWatcher {
	
	public class
	tFileWatcher {
		public FileInfo File;
		public tBool HasUpdated;
		public FileSystemWatcher DLL_Watcher;
		
		public tFileWatcher(
			DirectoryInfo aFolder,
			string aFileName
		) {
			this.File = aFolder.GetFiles(
				aFileName,
				new EnumerationOptions {
					RecurseSubdirectories = true,
					MatchCasing = MatchCasing.CaseInsensitive,
				}
			)[0];
			
			this.DLL_Watcher = new FileSystemWatcher(
				this.File.Directory.FullName
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
				Filter = this.File.Name,
				EnableRaisingEvents = true,
			};
			this.DLL_Watcher.Changed += (_, _) => {
				this.HasUpdated = true;
			};
		}
	}
}
