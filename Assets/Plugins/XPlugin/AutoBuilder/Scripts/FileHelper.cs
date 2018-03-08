using System;
using System.IO;

public static class FileHelper {

	public static void DirCopy(string from, string to) {
		DirCopy(new DirectoryInfo(from), new DirectoryInfo(to));
	}

	public static void DirCopy(DirectoryInfo source, DirectoryInfo target) {
		if (source.FullName.ToLower() == target.FullName.ToLower()) {
			return;
		}

		// Check if the target directory exists, if not, create it.
		if (Directory.Exists(target.FullName) == false) {
			Directory.CreateDirectory(target.FullName);
		}

		// Copy each file into it's new directory.
		foreach (FileInfo fi in source.GetFiles()) {
			fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
		}

		// Copy each subdirectory using recursion.
		foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
			DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
			DirCopy(diSourceSubDir, nextTargetSubDir);
		}
	}

	public static void DirMove(string from, string to) {
		DirMove(new DirectoryInfo(from), new DirectoryInfo(to));
	}

	public static void DirMove(DirectoryInfo source, DirectoryInfo target) {
		if (source.FullName.ToLower() == target.FullName.ToLower()) {
			return;
		}

		// Check if the target directory exists, if not, create it.
		if (Directory.Exists(target.FullName) == false) {
			Directory.CreateDirectory(target.FullName);
		}

		// Copy each file into it's new directory.
		foreach (FileInfo fi in source.GetFiles()) {
			fi.MoveTo(Path.Combine(target.ToString(), fi.Name));
		}

		// Copy each subdirectory using recursion.
		foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
			DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
			DirMove(diSourceSubDir, nextTargetSubDir);
		}

		source.Delete();
	}
}
