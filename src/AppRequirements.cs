using System;
using System.IO;

namespace RD_AAOW
	{
	/// <summary>
	/// Варианты стандартных зависимостей для пакетов
	/// </summary>
	public enum AppDefaultRequirements
		{
		/// <summary>
		/// Microsoft .NET Framework 4.8
		/// </summary>
		DotNETFramework = 0,

		/// <summary>
		/// Microsoft Visual C++ Runtime Libraries
		/// </summary>
		VC_RTL = 1,

		/// <summary>
		/// Microsoft SQL Compact edition
		/// </summary>
		SQLCE = 2,

		/// <summary>
		/// Microsoft .NET Framework 6.0
		/// </summary>
		DotNet6 = 3,

		/// <summary>
		/// Microsoft DirectX update
		/// </summary>
		DirectX = 4,

		/// <summary>
		/// Не является стандартной зависимостью
		/// </summary>
		None = -1
		}

	/// <summary>
	/// Класс предоставляет функионал по обработке зависимостей пакета
	/// </summary>
	public class AppRequirements
		{
		// Первичные

		/// <summary>
		/// Возвращает true, если данная зависимость уже удовлетворена
		/// </summary>
		public bool AlreadyInstalled
			{
			get
				{
				return alreadyInstalled;
				}
			}
		private bool alreadyInstalled = false;

		/// <summary>
		/// Возвращает ссылку для загрузки
		/// </summary>
		public string DownloadLink
			{
			get
				{
				return downloadLink;
				}
			}
		private string downloadLink = "";

		/// <summary>
		/// Возвращает описание зависимости
		/// </summary>
		public string Description
			{
			get
				{
				return description;
				}
			}
		private string description = "";

		/// <summary>
		/// Возвращает предлагаемое имя файла
		/// </summary>
		public string FileName
			{
			get
				{
				return fileName;
				}
			}
		private string fileName = "";

		/// <summary>
		/// Возвращает ожидаемый размер файла
		/// </summary>
		public string FileSize
			{
			get
				{
				return fileSize;
				}
			}
		private string fileSize = "";

		/// <summary>
		/// Возвращает тип стандартной зависимости, если она является таковой
		/// </summary>
		public AppDefaultRequirements DefaultType
			{
			get
				{
				return defaultType;
				}
			}
		private AppDefaultRequirements defaultType = AppDefaultRequirements.None;

		// Вторичные

		/// <summary>
		/// Возвращает true, если зависимость является стандартной
		/// </summary>
		public bool IsDefault
			{
			get
				{
				return defaultType != AppDefaultRequirements.None;
				}
			}

		/// <summary>
		/// Возвращает true, если данный экземпляр поддерживает автоматическую загрузку
		/// </summary>
		public bool AutodownloadAvailable
			{
			get
				{
				return !string.IsNullOrWhiteSpace (fileSize);
				}
			}

		/// <summary>
		/// Конструктор. Инициализирует экземпляр стандартной зависимости указанного типа
		/// с поддержкой автозагрузки
		/// </summary>
		/// <param name="ReqType">Тип зависимости</param>
		public AppRequirements (AppDefaultRequirements ReqType)
			{
			uint v;
			string s;
			defaultType = ReqType;

			switch (ReqType)
				{
				case AppDefaultRequirements.DotNETFramework:
				default:
					downloadLink = "https://go.microsoft.com/fwlink/?linkid=2088631";
					description = "Microsoft .NET Framework 4.8";
					fileName = "DotNETFramework48.exe";    // Автозагрузка
					fileSize = "121307088";

					defaultType = AppDefaultRequirements.DotNETFramework;

					s = RDGenerics.GetAppSettingsValue ("Release",
						"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
					try
						{
						v = uint.Parse (s);
						}
					catch
						{
						break;
						}

					alreadyInstalled = (v >= 528040);
					break;

				case AppDefaultRequirements.VC_RTL:
					downloadLink = "https://aka.ms/vs/17/release/vc_redist.x86.exe";
					description = "Microsoft Visual C++ 2015 – 2022 redistributable";
					fileName = "VCRedistributables143.exe";
					fileSize = "13730768";

					s = RDGenerics.GetAppSettingsValue ("Version",
						"HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\Installer\\Dependencies\\" +
						"Microsoft.VS.VC_RuntimeAdditionalVSU_x86,v14");

					alreadyInstalled = s.StartsWith ("14.3");
					break;

				case AppDefaultRequirements.SQLCE:
					downloadLink = "https://microsoft.com/en-us/download/details.aspx?id=30709";
					description = "Microsoft SQL Server Compact 4.0 SP1";

					s = RDGenerics.GetAppSettingsValue ("Version",
						"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Microsoft SQL Server Compact Edition\\v4.0");

					alreadyInstalled = s.StartsWith ("4.0");
					break;

				case AppDefaultRequirements.DotNet6:
					downloadLink = "https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/" +
						"runtime-desktop-6.0.9-windows-x86-installer";
					description = "Microsoft .NET Framework 6.0 (or newer)";

					s = RDGenerics.GetAppSettingsValue ("Version",
						"HKEY_LOCAL_MACHINE\\SOFTWARE\\dotnet\\Setup\\InstalledVersions\\x86\\hostfxr");
					v = 0;
					try
						{
						v = uint.Parse (s.Substring (0, 1));
						}
					catch { }

					alreadyInstalled = (v >= 6);
					break;

				case AppDefaultRequirements.DirectX:
					downloadLink = "https://microsoft.com/en-us/download/confirmation.aspx?id=35";
					description = "Microsoft DirectX 9 updates";

					try
						{
						alreadyInstalled = File.Exists (Environment.GetFolderPath (Environment.SpecialFolder.SystemX86) +
							"\\XAudio2_7.dll");
						}
					catch { }
					break;
				}
			}

		/// <summary>
		/// Конструктор. Инициализирует экземпляр стандартной зависимости без поддержки автозагрузки
		/// </summary>
		/// <param name="Description">Описание зависимости</param>
		/// <param name="URL">Ссылка для веб-страницы</param>
		public AppRequirements (string Description, string URL)
			{
			if (string.IsNullOrWhiteSpace (Description))
				description = "?";
			else
				description = Description;

			if (string.IsNullOrWhiteSpace (URL))
				downloadLink = "?";
			else
				downloadLink = URL;
			}

		/// <summary>
		/// Метод возвращает псевдоним зависимости для скрипта развёртки
		/// </summary>
		/// <param name="ReqType">Тип зависимости</param>
		public static string GetRequirementAlias (AppDefaultRequirements ReqType)
			{
			switch (ReqType)
				{
				// Всякое бывает
				default:
					return "";

				case AppDefaultRequirements.DirectX:
					return "DX+";

				case AppDefaultRequirements.DotNet6:
					return "NF6+";

				case AppDefaultRequirements.DotNETFramework:
					return "CS+";

				case AppDefaultRequirements.SQLCE:
					return "SQL+";

				case AppDefaultRequirements.VC_RTL:
					return "CPP+";
				}
			}

		/// <summary>
		/// Возвращает количество доступных стандартных зависимостей
		/// </summary>
		public const uint DefaultRequirementsCount = 5;
		}
	}
