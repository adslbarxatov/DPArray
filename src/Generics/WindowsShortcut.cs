using IWshRuntimeLibrary;
using System;
using System.IO;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает функционал для создания ярлыков к приложениям Windows
	/// </summary>
	public static class WindowsShortcut
		{
		/// <summary>
		/// Метод создаёт ярлык к файлу в текущем меню Пуск
		/// </summary>
		/// <param name="ShortcutArguments">Аргументы командной строки, передаваемые с файлом</param>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <param name="TargetFile">Файл, для которого создаётся ярлык</param>
		/// <param name="SubdirectoryName">Субдиректория в меню Пуск для данного пакета</param>
		/// <returns>Возвращает 0 в случае успеха</returns>
		public static int CreateMenuShortcut (string TargetFile, string SubdirectoryName,
			string ShortcutFileName, string ShortcutArguments)
			{
			string dir = Environment.GetFolderPath (Environment.SpecialFolder.CommonStartMenu) +
				"\\" + SubdirectoryName;

			try
				{
				Directory.CreateDirectory (dir);
				}
			catch
				{
				return -4;
				}

			return CreateShortcut (TargetFile, ShortcutFileName, dir, ShortcutArguments);
			}

		/// <summary>
		/// Метод удаляет ярлык к файлу из текущего меню Пуск
		/// </summary>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <param name="SubdirectoryName">Субдиректория в меню Пуск для данного пакета</param>
		/// <returns>Возвращает 0 в случае успеха</returns>
		public static int DeleteMenuShortcut (string SubdirectoryName, string ShortcutFileName)
			{
			string dir = Environment.GetFolderPath (Environment.SpecialFolder.CommonStartMenu) +
				"\\" + SubdirectoryName;
			int res = DeleteShortcut (ShortcutFileName, dir);

			try
				{
				Directory.Delete (dir);
				}
			catch { }

			return res;
			}

		/// <summary>
		/// Метод создаёт ярлык к файлу на текущем рабочем столе
		/// </summary>
		/// <param name="ShortcutArguments">Аргументы командной строки, передаваемые с файлом</param>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <param name="TargetFile">Файл, для которого создаётся ярлык</param>
		/// <returns>Возвращает 0 в случае успеха</returns>
		public static int CreateDesktopShortcut (string TargetFile, string ShortcutFileName,
			string ShortcutArguments)
			{
			return CreateShortcut (TargetFile, ShortcutFileName,
				Environment.GetFolderPath (Environment.SpecialFolder.Desktop),
				ShortcutArguments);
			}

		/// <summary>
		/// Метод удаляет ярлык к файлу с текущего рабочего стола
		/// </summary>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <returns>Возвращает 0 в случае успеха</returns>
		public static int DeleteDesktopShortcut (string ShortcutFileName)
			{
			return DeleteShortcut (ShortcutFileName,
				Environment.GetFolderPath (Environment.SpecialFolder.Desktop));
			}

		/// <summary>
		/// Метод создаёт ярлык к файлу в меню автозапуска
		/// </summary>
		/// <param name="ShortcutArguments">Аргументы командной строки, передаваемые с файлом</param>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <param name="TargetFile">Файл, для которого создаётся ярлык</param>
		/// <returns>Возвращает 0 в случае успеха</returns>
		public static int CreateStartupShortcut (string TargetFile, string ShortcutFileName,
			string ShortcutArguments)
			{
			return CreateShortcut (TargetFile, ShortcutFileName,
				Environment.GetFolderPath (Environment.SpecialFolder.CommonStartup),
				ShortcutArguments);
			}

		/// <summary>
		/// Метод удаляет ярлык к файлу из меню автозапуска
		/// </summary>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <returns>Возвращает 0 в случае успеха</returns>
		public static int DeleteStartupShortcut (string ShortcutFileName)
			{
			return DeleteShortcut (ShortcutFileName,
				Environment.GetFolderPath (Environment.SpecialFolder.CommonStartup));
			}

		/// <summary>
		/// Метод создаёт ярлык к файлу
		/// </summary>
		/// <param name="ShortcutArguments">Аргументы командной строки, передаваемые с файлом</param>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <param name="TargetFile">Файл, для которого создаётся ярлык</param>
		/// <param name="ShortcutFilePath">Расположение файла ярлыка</param>
		/// <returns>Возвращает 0 в случае успеха;
		/// -1, если переданы пустые строки в качестве путей;
		/// -2, если файл, для которого создаётся ярлык, недоступен;
		/// -3, если ярлык с заданными параметрами не может быть создан (целевой путь некорректен 
		/// или недоступен)</returns>
		public static int CreateShortcut (string TargetFile, string ShortcutFileName, string ShortcutFilePath,
			string ShortcutArguments)
			{
			// Контроль
			if (string.IsNullOrEmpty (TargetFile) || string.IsNullOrEmpty (ShortcutFileName) ||
				string.IsNullOrEmpty (ShortcutFilePath))
				return -1;

			if (!System.IO.File.Exists (TargetFile))
				return -2;

			string filePath = ShortcutFilePath.EndsWith ("\\") ? ShortcutFilePath : (ShortcutFilePath + "\\");
			filePath += (ShortcutFileName + ".lnk");

			// Удаление старого ярлыка
			if (System.IO.File.Exists (filePath))
				DeleteShortcut (ShortcutFileName, ShortcutFilePath);

			try
				{
				// Создание объектов
				WshShell shell = new WshShell ();
				IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut (filePath);

				// Настройка
				//shortcut.Description = "New shortcut for a Notepad";
				//shortcut.Hotkey = "Ctrl+Shift+N";
				if (!string.IsNullOrEmpty (ShortcutArguments))
					shortcut.Arguments = ShortcutArguments;

				shortcut.TargetPath = TargetFile;
				shortcut.WorkingDirectory = Path.GetDirectoryName (TargetFile);

				// Сохранение
				shortcut.Save ();
				}
			catch
				{
				return -3;
				}

			// Успешно
			return 0;
			}

		/// <summary>
		/// Метод удаляет ярлык файла
		/// </summary>
		/// <param name="ShortcutFileName">Имя файла ярлыка</param>
		/// <param name="ShortcutFilePath">Расположение файла ярлыка</param>
		/// <returns>Возвращает 0 в случае успеха;
		/// -1, если переданы пустые строки в качестве путей;</returns>
		/// -2, если удалить файл не удалось
		public static int DeleteShortcut (string ShortcutFileName, string ShortcutFilePath)
			{
			// Контроль
			if (string.IsNullOrEmpty (ShortcutFileName) || string.IsNullOrEmpty (ShortcutFilePath))
				return -1;

			string filePath = ShortcutFilePath.EndsWith ("\\") ? ShortcutFilePath : (ShortcutFilePath + "\\");
			filePath += (ShortcutFileName + ".lnk");

			try
				{
				System.IO.File.Delete (filePath);
				}
			catch
				{
				return -2;
				}

			// Успешно
			return 0;
			}
		}
	}
