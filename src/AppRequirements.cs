namespace RD_AAOW
	{
	/// <summary>
	/// Варианты стандартных зависимостей для пакетов
	/// </summary>
	public enum AppDefaultRequirements
		{
		/// <summary>
		/// Microsoft Visual C++ Runtime Libraries
		/// </summary>
		VC_RTL,

		/// <summary>
		/// Microsoft .NET Framework
		/// </summary>
		DotNETFramework,

		/// <summary>
		/// Microsoft XNA Framework
		/// </summary>
		XNAFramework,

		/// <summary>
		/// Microsoft SQL Compact edition
		/// </summary>
		SQLCE,

		/// <summary>
		/// Не является стандартной зависимостью
		/// </summary>
		None
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
					fileName = "DotNETfx48.exe";	// Автозагрузка
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
					fileName = "VCRedist143.exe";
					fileSize = "13730768";

					s = RDGenerics.GetAppSettingsValue ("Version",
						"HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\Installer\\Dependencies\\" +
						"Microsoft.VS.VC_RuntimeAdditionalVSU_x86,v14");

					alreadyInstalled = s.StartsWith ("14.3");
					break;

				case AppDefaultRequirements.XNAFramework:
					downloadLink = "https://microsoft.com/en-us/download/details.aspx?id=20914";
					description = "Microsoft XNA Framework Redistributable 4.0";
					// Автозагрузка не предусмотрена
					/*fileName = "XNAfx40.msi";*/

					s = RDGenerics.GetAppSettingsValue ("ProductVersion",
						"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\XNA\\Framework\\v4.0");

					alreadyInstalled = s.StartsWith ("4.0");
					break;

				case AppDefaultRequirements.SQLCE:
					downloadLink = "https://microsoft.com/en-us/download/details.aspx?id=30709";
					description = "Microsoft SQL Server Compact 4.0 SP1";
					/*fileName = "SSCE40.exe";*/

					s = RDGenerics.GetAppSettingsValue ("Version",
						"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Microsoft SQL Server Compact Edition\\v4.0");

					alreadyInstalled = s.StartsWith ("4.0");
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
		}
	}
