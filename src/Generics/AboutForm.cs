﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает интерфейс отображения сведений о программе
	/// </summary>
	public partial class AboutForm: Form
		{
		// Переменные
		private string projectLink, updatesLink, userManualLink;
		private string updatesMessage = "", updatesMessageForText = "", description = "",
			policyLoaderCaption = "", /*registryFail = "",*/
			dpModuleAbsence = "", startDownload = "", packageFail = "", fileWriteFail = "",
			versionDescription = "", adpRevision = "";
		private bool accepted = false;              // Флаг принятия Политики
		private const string toolName = "DPArray";

		private static string[][] locale = new string[][] { new string [] {

			"",
			"",
			"Поиск обновлений...",
			"Открыть в браузере",		// 03
			"",
			"",
			"",
			"",	// 07
			"От&клонить",

			"Не удалось получить текст Политики. Нажмите кнопку ▶ для её открытия в веб-браузере",
			"[Проверка обновлений...]\r\n\r\n",
			"Подготовка к запуску...",	// 11

			/*" не может сохранить настройки в реестре Windows. Приложение не будет работать корректно.\n\n" +
			"Попробуйте выполнить следующие изменения в свойствах исполняемого файла:\n" +
			"• разблокируйте приложение в общих свойствах (кнопка «Разблокировать»);\n" +
			"• включите запуск от имени администратора для всех пользователей в настройках совместимости.\n\n" +
			"После этого перезапустите программу и повторите попытку",*/
			"",

			"Инструмент развёртки пакетов " + toolName + " не найден на этом ПК. Перейти к его загрузке?" +
			"\n\nВы можете обновить этот продукт прямо из " + toolName + " или вернуться сюда после его установки. " +
			"Также Вы можете ознакомиться с презентацией " + toolName + " на YouTube, нажав кнопку «Видео»",

			"Не удалось загрузить пакет развёртки. Проверьте Ваше подключение к Интернету",
			"Не удалось сохранить пакет развёртки. Проверьте Ваши права доступа",

			"Начать загрузку пакета?\n\nПосле завершения пакет развёртки будет запущен автоматически",	// 16

			"Политика разработки и соглашение пользователя",
			"",
			"Версия актуальна",
			"[Версия актуальна, см. описание в конце]",		// 20
			"&Доступна {0:S}",
			"[Доступна {0:S}, см. описание в конце]",
			"Сервер" + "\xA0" + "недоступен",
			"[Страница обновлений недоступна]",		// 24

			"",
			"",

			"Предупреждение: необходимые расширения файлов будут зарегистрированы с использованием " +
			"текущего местоположения приложения.\n\nУбедитесь, что вы не будете менять расположение " +
			"этого приложения перед использованием этой функции.\n\nПродолжить?",

			"Предупреждение: необходимые протоколы будут зарегистрированы с использованием " +
			"текущего местоположения приложения.\n\nУбедитесь, что вы не будете менять расположение " +
			"этого приложения перед использованием этой функции.\n\nПродолжить?",		// 28

			"&Видео",	// 29

			"Предупреждение! Прочтите перед использованием этой опции!\n\n" +
			"Вы можете помочь нам сделать наше сообщество более популярным. Если эта функция включена, " +
			"она будет вызывать случайную страницу Лаборатории при запуске приложения. Она будет использовать " +
			"скрытый режим, поэтому Вам не придётся смотреть на какие-либо страницы или окна. Этот алгоритм " +
			"просто загружает вызываемую страницу и удаляет её, чтобы имитировать активность. Небольшой чит, " +
			"чтобы помочь нашему развитию.\n\n" +
			"Согласно ADP:\n" +
			"1. Эта опция НИКОГДА НЕ БУДЕТ активирована без Вашего согласия.\n" +
			"2. Эта опция делает В ТОЧНОСТИ то, что описано здесь. Никакого сбора данных, никаких опасных " +
			"веб-ресурсов.\n" +
			"3. Вы можете отключить её здесь в любое время.\n\n" +
			"И да: если у Вас дорогой интернет, НЕ АКТИВИРУЙТЕ ЭТУ ОПЦИЮ. Она может занять немного трафика.\n\n" +
			"Заранее благодарим Вас за участие!"

			}, new string [] {

			"",
			"",
			"Checking updates...",
			"Open in browser",			// 03
			"",
			"",
			"",
			"",		// 07
			"De&cline",

			"Failed to get the Policy text. Press ▶ button to open it in web browser",
			"[Checking for updates...]\r\n\r\n",
			"Preparing for launch...",	// 11
		
			/*" cannot save settings in the Windows registry. It will not work properly.\n\n" +
			"Try the following changes to properties of the executable file:\n" +
			"• unblock the app in general properties (“Unblock” button);\n" +
			"• enable running as administrator for all users in compatibility settings.\n\n" +
			"Then restart the program and try again",*/
			"",

			toolName + ", the packages deployment tool isn’t installed on this PC. " +
			"Download it?\n\nYou can update this product directly from " + toolName + " or come back here " +
			"after installing it. Also you can view the " + toolName + " presentation on YouTube by pressing " +
			"“Video” button",

			"Failed to download deployment package. Check your internet connection",
			"Failed to save deployment package. Check your user access rights",

			"Download the package?\n\nThe deployment package will be started automatically after completion",	// 16

			"Development policy and user agreement",
			"",
			"App is up-to-date",
			"[Version is up to date, see description below]",	// 20
			"{0:S} a&vailable",
			"[{0:S} is available, see description below]",
			"Not" + "\xA0" + "available",
			"[Updates page is unavailable]",	// 24

			"",
			"",

			"Warning: required file extensions will be registered using current app location.\n\n" +
			"Make sure you will not change location of this application before using this feature.\n\n" +
			"Continue?",

			"Warning: required protocols will be registered using current app location.\n\n" +
			"Make sure you will not change location of this application before using this feature.\n\n" +
			"Continue?",			// 28

			"&Video",	// 29

			"Warning! Read this first before using this option!\n\n" +
			"You can help us to make our community more popular. When enabled, this function will call " +
			"random Lab’s page at the app start. It will use hidden mode, so, you don’t have to look at any " +
			"page or window. This algorithm just downloads called page and removes it to imitate the activity. " +
			"A little cheat to help our development.\n\n" +
			"According to ADP:\n" +
			"1. This option WILL NEVER be activated without your agreement.\n" +
			"2. This option do EXACTLY what described here. No data collection, no dangerous web resources.\n" +
			"3. You can disable it here anytime.\n\n" +
			"And yes: if you have expensive internet, DO NOT ACTIVATE THIS OPTION. " +
			"It can take away some amount of traffic.\n" +
			"Thank you in advance for your participation!"
			} };

		/// <summary>
		/// Ключ реестра, хранящий версию, на которой отображалась справка
		/// </summary>
		public const string LastShownVersionKey = "HelpShownAt";

		// Ключ реестра, хранящий последнюю принятую версию ADP
		private const string ADPRevisionKey = "ADPRevision";
		/*private const string ADPRevisionPath = RDGenerics.AssemblySettingsStorage + "DPModule";*/

		// Элементы поддержки HypeHelp
		private const string HypeHelpKey = "HypeHelp";
		private bool hypeHelp;
		private const string LastHypeHelpKey = "LastHypeHelp";
		private DateTime lastHypeHelp;

		private string[] hypeHelpLinks = new string[] {
			"https://vk.com/rd_aaow_fdl",
			"https://vk.com/grammarmustjoy",
			"https://t.me/rd_aaow_fdl",
			"https://t.me/grammarmustjoy",

			"https://youtube.com/c/rdaaowfdl",
			"https://youtu.be/seFfQkfL6Sk",
			"https://youtu.be/fdH4SKFdTCI",
			"https://youtu.be/I_sXoDxPQQ0",
			"https://youtu.be/hTnDR89VR8w",

			"https://moddb.com/mods/esrm",
			"https://moddb.com/mods/eshq",
			"https://moddb.com/mods/ccm",
			};
		private Random rnd = new Random ();

		/// <summary>
		/// Левый маркер лога изменений
		/// </summary>
		public const string ChangeLogMarkerLeft = "markdown-body my-3\">";

		/// <summary>
		/// Правый маркер лога изменений
		/// </summary>
		public const string ChangeLogMarkerRight = "</div>";

		// Список подстановок при восстановлении спецсимволов из HTML-кода
		private static string[][] htmlReplacements = new string[][] {
			new string[] { "<p", "\r\n<" },
			new string[] { "<li>", "\r\n• " },
			new string[] { "</p>", "\r\n" },
			new string[] { "<br", "\r\n<" },

			new string[] { "<h1", "\r\n<" },
			new string[] { "</h1>", "\r\n" },
			new string[] { "<h3", "\r\n<" },

			new string[] { "&gt;", "›" },
			new string[] { "&lt;", "‹" },
			new string[] { "&#39;", "’" },
			new string[] { "&bull;", "•" }
			};

		// Доступные варианты перехода к ресурсам Лаборатории
		private enum LinkTypes
			{
			UserManual,
			ProjectPage,
			ADP,
			AskDeveloper,
			ToLabMain,
			ToLabVK,
			ToLabTG,
			}
		private List<LinkTypes> linkTypes = new List<LinkTypes> ();

		/// <summary>
		/// Конструктор. Инициализирует форму
		/// </summary>
		/// <param name="UserManualLink">Ссылка на страницу руководства пользователя;
		/// кнопка отключается, если это значение не задано</param>
		/// <param name="AppLogo">Лого приложения</param>
		public AboutForm (string UserManualLink, Bitmap AppLogo)
			{
			// Инициализация
			InitializeComponent ();
			this.AcceptButton = ExitButton;
			this.CancelButton = MisacceptButton;

			// Получение параметров
			userManualLink = (UserManualLink == null) ? "" : UserManualLink;

			projectLink = RDGenerics.DefaultGitLink + ProgramDescription.AssemblyMainName;
			updatesLink = RDGenerics.DefaultGitLink + ProgramDescription.AssemblyMainName +
				RDGenerics.GitUpdatesSublink;

			// Загрузка окружения
			AboutLabel.Text = RDGenerics.AppAboutLabelText;
			if (AppLogo != null)
				IconBox.BackgroundImage = AppLogo;

			AboutForm_Resize (null, null);
			}

		/// <summary>
		/// Метод отображает справочное окно приложения
		/// </summary>
		/// <param name="Description">Описание программы и/или справочная информация</param>
		/// <param name="StartupMode">Флаг, указывающий, что справка не должна отображаться, если
		/// она уже была показана для данной версии приложения</param>
		/// <returns>Возвращает:
		/// 1, если справка уже отображалась для данной версии (при StartupMode == true);
		/// другое значение, если окно справки было отображено</returns>
		public int ShowAbout (string Description, bool StartupMode)
			{
			description = Description;

			return LaunchForm (StartupMode, false);
			}

		/// <summary>
		/// Метод запускает окно в режиме принятия Политики
		/// </summary>
		/// <returns>Возвращает 0, если Политика принята;
		/// 1, если Политика уже принималась ранее;
		/// -1, если Политика отклонена</returns>
		public int AcceptEULA ()
			{
			return LaunchForm (false, true);
			}

		// Основной метод запуска окна
		private int LaunchForm (bool StartupMode, bool AcceptMode)
			{
			// HypeHelp
			/*hypeHelp = RDGenerics.GetAppSettingsValue (HypeHelpKey, ADPRevisionPath) == "1";*/
			hypeHelp = RDGenerics.GetDPArraySettingsValue (HypeHelpKey) == "1";
			try
				{
				/*lastHypeHelp = DateTime.Parse (RDGenerics.GetAppSettingsValue (LastHypeHelpKey,
					ADPRevisionPath));*/
				lastHypeHelp = DateTime.Parse (RDGenerics.GetDPArraySettingsValue (LastHypeHelpKey));
				}
			catch
				{
				lastHypeHelp = DateTime.Now;
				}

			HardWorkExecutor hwe, hweh;
			if (hypeHelp && (StartupMode || AcceptMode) && (lastHypeHelp <= DateTime.Now))
				{
#if DPMODULE
				hweh = new HardWorkExecutor (HypeHelper, null, null, true, false, false);
#else
				hweh = new HardWorkExecutor (HypeHelper, null, null, true, false);
#endif

				lastHypeHelp = DateTime.Now.AddMinutes (rnd.Next (65, 95));
				/*RDGenerics.SetAppSettingsValue (LastHypeHelpKey, lastHypeHelp.ToString (), ADPRevisionPath);*/
				RDGenerics.SetDPArraySettingsValue (LastHypeHelpKey, lastHypeHelp.ToString ());
				}

			// Запрос настроек
			string helpShownAt = "";
			if (StartupMode || AcceptMode)
				{
				/*adpRevision = RDGenerics.GetAppSettingsValue (ADPRevisionKey, ADPRevisionPath);*/
				adpRevision = RDGenerics.GetDPArraySettingsValue (ADPRevisionKey);
				helpShownAt = RDGenerics.GetAppSettingsValue (LastShownVersionKey);

				// Если поле пустое, устанавливается минимальное значение
				if (adpRevision == "")
					{
					adpRevision = "rev. 10!";
					/*RDGenerics.SetAppSettingsValue (ADPRevisionKey, adpRevision, ADPRevisionPath);*/
					RDGenerics.SetDPArraySettingsValue (ADPRevisionKey, adpRevision);
					}
				}

			// Контроль
			if (StartupMode && (helpShownAt == ProgramDescription.AssemblyVersion) ||   // Справка уже отображалась
				AcceptMode && (!adpRevision.EndsWith ("!")))                            // Политика уже принята
				return 1;

			// Настройка контролов
			int al = (int)Localization.CurrentLanguage;

			UpdatesPageButton.Text = locale[al][2];

			ExitButton.Text = Localization.GetDefaultText (AcceptMode ? LzDefaultTextValues.Button_Accept :
				LzDefaultTextValues.Button_OK);

			MisacceptButton.Text = locale[al][8];

			if (!desciptionHasBeenUpdated)
				DescriptionBox.Text = locale[al][AcceptMode ? 9 : 10] + description;

			policyLoaderCaption = locale[al][11];
			/*registryFail = ProgramDescription.AssemblyMainName + locale[al][12];*/
			dpModuleAbsence = locale[al][13];
			packageFail = locale[al][14];
			fileWriteFail = locale[al][15];
			startDownload = locale[al][16];

			// Загрузка списка доступных переходов к ресурсам Лаборатории
			if (ToLaboratoryCombo.Items.Count < 1)
				{
				if (!AcceptMode)
					{
					if (!string.IsNullOrWhiteSpace (userManualLink))
						{
						linkTypes.Add (LinkTypes.UserManual);
						ToLaboratoryCombo.Items.Add (Localization.GetDefaultText (LzDefaultTextValues.Control_UserManual));
						}

					linkTypes.Add (LinkTypes.ProjectPage);
					ToLaboratoryCombo.Items.Add (Localization.GetDefaultText (LzDefaultTextValues.Control_ProjectWebpage));

					linkTypes.Add (LinkTypes.AskDeveloper);
					ToLaboratoryCombo.Items.Add (Localization.GetDefaultText (LzDefaultTextValues.Control_AskDeveloper));
					}

				linkTypes.Add (LinkTypes.ADP);
				ToLaboratoryCombo.Items.Add (AcceptMode ? locale[al][3] :
					Localization.GetDefaultText (LzDefaultTextValues.Control_PolicyEULA));

				linkTypes.Add (LinkTypes.ToLabMain);
				linkTypes.Add (LinkTypes.ToLabTG);
				linkTypes.Add (LinkTypes.ToLabVK);
				ToLaboratoryCombo.Items.AddRange (RDGenerics.CommunitiesNames);
				}
			ToLaboratoryCombo.SelectedIndex = 0;

			this.Text = AcceptMode ? locale[al][17] :
				Localization.GetDefaultText (LzDefaultTextValues.Control_AppAbout);

			// Запуск проверки обновлений
			if (!AcceptMode)
				{
				UpdatesPageButton.Enabled = false;
#if DPMODULE
				hwe = new HardWorkExecutor (UpdatesChecker, null, null, false, false, false);
#else
				hwe = new HardWorkExecutor (UpdatesChecker, null, null, false, false);
#endif
				UpdatesTimer.Enabled = true;
				}

			// Получение Политики
			else
				{
#if DPMODULE
				hwe = new HardWorkExecutor (PolicyLoader, null, policyLoaderCaption, true, false, true);
#else
				hwe = new HardWorkExecutor (PolicyLoader, null, policyLoaderCaption, true, false);
#endif

				string html = hwe.Result.ToString ();
				if (!string.IsNullOrWhiteSpace (html))
					{
					DescriptionBox.Text = html;

					int left = html.IndexOf ("rev");
					int right = html.IndexOf ("\n", left);
					if ((left >= 0) && (right >= 0))
						adpRevision = html.Substring (left, right - left);
					}
				}

			// Настройка контролов
			HypeHelpFlag.Visible = !AcceptMode;

#if DPMODULE
			UpdatesPageButton.Visible = false;
#else
			UpdatesPageButton.Visible = !AcceptMode;
#endif

			MisacceptButton.Visible = AcceptMode;

			// Запуск с управлением настройками окна
			HypeHelpFlag.Checked = hypeHelp;
			/*RDGenerics.LoadWindowDimensions (this, ADPRevisionPath);*/
			RDGenerics.LoadAppAboutWindowDimensions (this);

			this.ShowDialog ();

			/*RDGenerics.SaveWindowDimensions (this, ADPRevisionPath);*/
			RDGenerics.SaveAppAboutWindowDimensions (this);
			/*RDGenerics.SetAppSettingsValue (HypeHelpKey, HypeHelpFlag.Checked ? "1" : "0", ADPRevisionPath);*/
			RDGenerics.SetDPArraySettingsValue (HypeHelpKey, HypeHelpFlag.Checked ? "1" : "0");

			// Запись версий по завершению
			/*try
				{*/
			if (StartupMode)
				{
				RDGenerics.SetAppSettingsValue (LastShownVersionKey, ProgramDescription.AssemblyVersion);

				/* Контроль доступа к реестру
				WindowsIdentity identity = WindowsIdentity.GetCurrent ();
				WindowsPrincipal principal = new WindowsPrincipal (identity);

				if (!principal.IsInRole (WindowsBuiltInRole.Administrator))
					RDGenerics.MessageBox (RDMessageTypes.Warning_Center, registryFail);*/
				}

			// В случае невозможности загрузки Политики признак необходимости принятия до этого момента
			// не удаляется из строки версии. Поэтому требуется страховка
			if (AcceptMode && accepted)
				/*RDGenerics.SetAppSettingsValue (ADPRevisionKey, adpRevision.Replace ("!", ""), ADPRevisionPath);*/
				RDGenerics.SetDPArraySettingsValue (ADPRevisionKey, adpRevision.Replace ("!", ""));
			/*}
		catch
			{
			RDGenerics.MessageBox (RDMessageTypes.Warning_Left, registryFail);
			}*/

			// Завершение
			return accepted ? 0 : -1;
			}

		// Метод получает Политику разработки
		private void PolicyLoader (object sender, DoWorkEventArgs e)
			{
			string html = RDGenerics.GetHTML (RDGenerics.ADPLink);
			int textLeft, textRight;

			if (((textLeft = html.IndexOf ("code\">")) >= 0) &&
				((textRight = html.IndexOf ("<footer", textLeft)) >= 0))
				{
				// Обрезка
				textLeft += 6;
				html = html.Substring (textLeft, textRight - textLeft);

				// Формирование текста
				html = ApplyReplacements (html);

				while (html.Contains ("\x0A\x0A"))
					html = html.Replace ("\x0A\x0A", "\x0A");   // Лишние Unix-абзацы
				html = html.Replace ("\xA0\x0D", "\x0D");       // Скрытые неразрывные пробелы
				html = html.Replace ("\x0A\x20", "\x0A     ");  // Отступы в строках Положений

				html = html.Replace ("\x0D\x0A", "\x01");       // Устранение оставшихся задвоений Unix-абзацев
				html = html.Replace ("\x0A", "\x20");
				html = html.Replace ("\x01", "\x0D\x0A");
				html = html.Replace (" \x0D\x0A", "\x0D\x0A");  // Устранение оставшихся лишних пробелов

				//File.WriteAllText (RDGenerics.AppStartupPath + "Policy.dmp", html);
				html = html.Substring (1, html.Length - 48);    // Финальная обрезка
				}
			else
				{
				e.Result = "";
				return;
				}

			e.Result = html;
			return;
			}

		/// <summary>
		/// Конструктор. Открывает указанную ссылку без запуска формы
		/// </summary>
		/// <param name="Link">Ссылка для отображения;
		/// если указан null, запускается ссылка на релизы продукта</param>
		public AboutForm (string Link)
			{
			if (string.IsNullOrWhiteSpace (Link))
				RDGenerics.RunURL (RDGenerics.DefaultGitLink + ProgramDescription.AssemblyMainName +
						RDGenerics.GitUpdatesSublink + "/latest");
			else
				RDGenerics.RunURL (Link);
			}

		// Закрытие окна
		private void ExitButton_Click (object sender, EventArgs e)
			{
			accepted = true;
			UpdatesTimer.Enabled = false;
			this.Close ();
			}

		// Изменение размера окна
		private void AboutForm_Resize (object sender, EventArgs e)
			{
			DescriptionBox.Width = this.ClientSize.Width - 28;

			DescriptionBox.Height = this.ClientSize.Height - 195;
			ExitButton.Top = MisacceptButton.Top = HypeHelpFlag.Top = this.ClientSize.Height - 33;

			UpdatesPageButton.Left = DescriptionBox.Left + DescriptionBox.Width - UpdatesPageButton.Width;
			AboutLabel.Left = DescriptionBox.Left + DescriptionBox.Width - AboutLabel.Width;
			ExitButton.Left = DescriptionBox.Left + DescriptionBox.Width - ExitButton.Width;
			}

		// Запуск ссылок
		private void ToLaboratory_Click (object sender, EventArgs e)
			{
			string link;
			switch (linkTypes[ToLaboratoryCombo.SelectedIndex])
				{
				case LinkTypes.ADP:
					link = RDGenerics.ADPLink;
					break;

				case LinkTypes.AskDeveloper:
					link = RDGenerics.LabMailLink + ("?subject=" +
						RDGenerics.LabMailCaption).Replace (" ", "%20");
					break;

				case LinkTypes.ProjectPage:
					link = projectLink;
					break;

				case LinkTypes.ToLabTG:
					link = RDGenerics.LabTGLink;
					break;

				case LinkTypes.ToLabVK:
					link = RDGenerics.LabVKLink;
					break;

				case LinkTypes.UserManual:
					link = userManualLink;
					break;

				default:
				case LinkTypes.ToLabMain:
					link = RDGenerics.DPArrayLink;
					break;
				}

			RDGenerics.RunURL (link);
			}

		// Загрузка пакета обновления изнутри приложения
		private void UpdatesPageButton_Click (object sender, EventArgs e)
			{
#if !DPMODULE

			// Контроль наличия DPArray
			/*string dpmv = RDGenerics.GetAppSettingsValue (LastShownVersionKey, ADPRevisionPath);*/
			string dpmv = RDGenerics.GetDPArraySettingsValue (LastShownVersionKey);
			string downloadLink, packagePath;

			int l;
			if (string.IsNullOrWhiteSpace (dpmv))
				{
				// Выбор варианта обработки
				switch (RDGenerics.MessageBox (RDMessageTypes.Question_Left, dpModuleAbsence,
						Localization.GetDefaultText (LzDefaultTextValues.Button_Yes),
						locale[(int)Localization.CurrentLanguage][29],
						Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel)))
					{
					case RDMessageButtons.ButtonThree:
						return;

					case RDMessageButtons.ButtonTwo:
						RDGenerics.RunURL (RDGenerics.DPArrayUserManualLink);
						return;
					}

				downloadLink = RDGenerics.DPArrayDirectLink;
				packagePath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + "\\";
				}
			else
				{
				if (RDGenerics.MessageBox (RDMessageTypes.Question_Center, startDownload,
					Localization.GetDefaultText (LzDefaultTextValues.Button_Yes),
					Localization.GetDefaultText (LzDefaultTextValues.Button_No)) !=
					RDMessageButtons.ButtonOne)
					return;

#if MONOGAME
				downloadLink = packagePath = "";
				RDGenerics.RunURL (RDGenerics.DPArrayProtocolPrefix + ProgramDescription.AssemblyMainName);
				return;
#else
				downloadLink = RDGenerics.DPArrayPackageLink;

				/*packagePath = RDGenerics.GetAppSettingsValue (toolName, ADPRevisionPath);*/
				packagePath = RDGenerics.GetDPArraySettingsValue (toolName);
				if (string.IsNullOrWhiteSpace (packagePath))    // Такое может быть, если DPArray ни разу не обновлялся
					{
					packagePath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + "\\";
					}
				else
					{
					if ((l = packagePath.IndexOf ('\t')) >= 0)
						packagePath = packagePath.Substring (0, l);
					packagePath += "\\Downloaded\\";
					}
#endif
				}

			l = downloadLink.LastIndexOf ('/');
			packagePath += downloadLink.Substring (l + 1);

			// Запуск загрузки
			HardWorkExecutor hwe = new HardWorkExecutor (RDGenerics.PackageLoader, downloadLink,
				packagePath, "0", false);

			// Разбор ответа
			string msg = "";
			switch (hwe.ExecutionResult)
				{
				case 0:
					break;

				case -1:
				case -2:
					msg = packageFail;
					break;

				case -3:
					msg = fileWriteFail;
					break;

				default: // Отмена
					try
						{
						File.Delete (packagePath);
						}
					catch { }
					return;
				}

			if (!string.IsNullOrWhiteSpace (msg))
				{
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center, msg);
				return;
				}

			// Запуск пакета
			RDGenerics.RunURL (packagePath);

#endif
			}

		// Флаг hype help
		private void HypeHelpFlag_CheckedChanged (object sender, EventArgs e)
			{
			if (HypeHelpFlag.Checked)
				RDGenerics.MessageBox (RDMessageTypes.Success_Left, locale[(int)Localization.CurrentLanguage][30]);
			}

		/// <summary>
		/// Метод выполняет пост-обработку текста лога или политики после загрузки
		/// </summary>
		/// <param name="Source">Исходный текст</param>
		/// <returns>Текст с применёнными заменами символов форматирования</returns>
		public static string ApplyReplacements (string Source)
			{
			string res = Source;

			// Замена элементов разметки
			for (int i = 0; i < htmlReplacements.Length; i++)
				res = res.Replace (htmlReplacements[i][0], htmlReplacements[i][1]);

			// Удаление вложенных тегов
			int textLeft, textRight;
			while (((textLeft = res.IndexOf ("<")) >= 0) && ((textRight = res.IndexOf (">", textLeft)) >= 0))
				res = res.Replace (res.Substring (textLeft, textRight - textLeft + 1), "");

			return res;
			}

		// Метод выполняет фоновую проверку обновлений
		private void UpdatesChecker (object sender, DoWorkEventArgs e)
			{
			// Запрос обновлений пакета
			int al = (int)Localization.CurrentLanguage;
			string html = RDGenerics.GetHTML (projectLink);
			bool htmlError = true;  // Сбрасывается при успешной загрузке

			// Разбор ответа (извлечение версии)
			string[] htmlMarkers = {
				"</a>" + ProgramDescription.AssemblyMainName,
				"</h1>",
				ChangeLogMarkerLeft,
				ChangeLogMarkerRight
				};

			int i = html.IndexOf (htmlMarkers[0]);
			if (i < 0)
				goto policy;

			i += htmlMarkers[0].Length;

			int j = html.IndexOf (htmlMarkers[1], i);
			if ((j < 0) || (j <= i))
				goto policy;

			string version = html.Substring (i, j - i).Trim ();

			// Запрос описания пакета
			html = RDGenerics.GetHTML (updatesLink);

			// Разбор ответа (извлечение версии)
			i = html.IndexOf (htmlMarkers[2]);
			if (i < 0)
				goto policy;

			i += htmlMarkers[2].Length;

			j = html.IndexOf (htmlMarkers[3], i);
			if ((j < 0) || (j <= i))
				goto policy;

			versionDescription = html.Substring (i, j - i);
			versionDescription = "\r\n" + ApplyReplacements (versionDescription);

			// Отображение результата
			if (ProgramDescription.AssemblyTitle.EndsWith (version))
				{
				updatesMessage = locale[al][19];
				updatesMessageForText = locale[al][20];
				}
			else
				{
				updatesMessage = string.Format (locale[al][21], version);
				updatesMessageForText = string.Format (locale[al][22], version);
				}
			htmlError = false;

// Получение обновлений Политики (ошибки игнорируются)
policy:
			html = RDGenerics.GetHTML (RDGenerics.ADPLink);
			if (((i = html.IndexOf ("<title")) >= 0) && ((j = html.IndexOf ("</title", i)) >= 0))
				{
				// Обрезка
				html = html.Substring (i, j - i);

				if ((i = html.IndexOf ("rev")) >= 0)
					{
					html = html.Substring (i);

					// Сброс версии для вызова Политики при следующем старте
					if (!html.StartsWith (adpRevision))
						/*RDGenerics.SetAppSettingsValue (ADPRevisionKey, html + "!", ADPRevisionPath);*/
						RDGenerics.SetDPArraySettingsValue (ADPRevisionKey, html + "!");
					}
				}

			// Не было проблем с загрузкой страницы
			if (!htmlError)
				{
				e.Result = 0;
				return;
				}

			// Есть проблема при загрузке страницы. Отмена
			updatesMessage = locale[al][23];
			updatesMessageForText = locale[al][24];

			e.Result = -2;
			return;
			}

		// Метод выполняет фоновую проверку обновлений
		private void HypeHelper (object sender, DoWorkEventArgs e)
			{
			RDGenerics.GetHTML (hypeHelpLinks[rnd.Next (hypeHelpLinks.Length)]);
			e.Result = 0;
			}

		// Контроль сообщения об обновлении
		private bool desciptionHasBeenUpdated = false;
		private void UpdatesTimer_Tick (object sender, EventArgs e)
			{
			if (string.IsNullOrWhiteSpace (updatesMessage))
				return;

			// Получение описания версии
			if (!string.IsNullOrWhiteSpace (versionDescription))
				{
				description += versionDescription;
				versionDescription = "";
				}

			// Обновление состояния
			if (!desciptionHasBeenUpdated)
				{
				DescriptionBox.Text = updatesMessageForText + "\r\n\r\n" + description;
				desciptionHasBeenUpdated = true;
				}

			// Включение текста кнопки
			if (string.IsNullOrWhiteSpace (UpdatesPageButton.Text))
				{
				UpdatesPageButton.Text = updatesMessage;

				// Включение кнопки и установка интервала
				if (!UpdatesPageButton.Enabled)
					{
					// Исключение задвоения
					if (updatesMessage.Contains (" "))    // Интернет доступен
						{
						UpdatesTimer.Interval = 1000;
						UpdatesPageButton.Enabled = true;

						if (updatesMessage.Contains ("."))
							UpdatesPageButton.Font = new Font (UpdatesPageButton.Font, FontStyle.Bold);
						else
							UpdatesTimer.Enabled = false;
						}

					// Отключение таймера, если обновлений нет
					else
						{
						UpdatesTimer.Enabled = false;
						}
					}
				}

			// Выключение
			else
				{
				UpdatesPageButton.Text = "";
				}
			}

		// Непринятие Политики
		private void MisacceptButton_Click (object sender, EventArgs e)
			{
			accepted = false;
			UpdatesTimer.Enabled = false;
			this.Close ();
			}

		/// <summary>
		/// Метод выполняет регистрацию указанного расширения файла и привязывает его к текущему приложению
		/// </summary>
		/// <param name="FileExtension">Расширение файла без точки</param>
		/// <param name="FileTypeName">Название типа файла</param>
		/// <param name="Openable">Флаг указывает, будет ли файл доступен для открытия в приложении</param>
		/// <param name="ShowWarning">Флаг указывает, что необходимо отобразить предупреждение перед регистрацией</param>
		/// <param name="FileIcon">Ресурс, хранящий значок формата файла</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public static bool RegisterFileExtension (string FileExtension, string FileTypeName, Icon FileIcon,
			bool Openable, bool ShowWarning)
			{
			// Подготовка
			string fileExt = FileExtension.ToLower ().Replace (".", "");

			// Контроль
			if (ShowWarning && (RDGenerics.MessageBox (RDMessageTypes.Warning_Left,
				locale[(int)Localization.CurrentLanguage][27],
				Localization.GetDefaultText (LzDefaultTextValues.Button_Yes),
				Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel)) ==
				RDMessageButtons.ButtonTwo))
				return false;

			// Выполнение
			try
				{
				// Запись значка
				FileStream FS = new FileStream (RDGenerics.AppStartupPath + fileExt + ".ico", FileMode.Create);
				FileIcon.Save (FS);
				FS.Close ();
				}
			catch
				{
				return false;
				}

			// Запись значений реестра
			bool res = true;
			/*res &= RDGenerics.SetAppSettingsValue ("", fileExt + "file", "HKEY_CLASSES_ROOT\\." + fileExt);
			res &= RDGenerics.SetAppSettingsValue ("", FileTypeName, "HKEY_CLASSES_ROOT\\" + fileExt + "file");
			res &= RDGenerics.SetAppSettingsValue ("", RDGenerics.AppStartupPath + fileExt + ".ico",
				"HKEY_CLASSES_ROOT\\" + fileExt + "file\\DefaultIcon");*/
			res &= RDGenerics.SetFileExtensionValue ("." + fileExt, "", fileExt + "file");
			res &= RDGenerics.SetFileExtensionValue (fileExt + "file", "", FileTypeName);
			res &= RDGenerics.SetFileExtensionValue (fileExt + "file\\DefaultIcon", "",
				RDGenerics.AppStartupPath + fileExt + ".ico");

			if (Openable)
				{
				/*res &= RDGenerics.SetAppSettingsValue ("", "open", "HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell");
				res &= RDGenerics.SetAppSettingsValue ("Icon", RDGenerics.AppStartupPath + fileExt + ".ico",
					"HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell\\open");
				res &= RDGenerics.SetAppSettingsValue ("", "\"" + Application.ExecutablePath + "\" \"%1\"",
					"HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell\\open\\command");*/
				res &= RDGenerics.SetFileExtensionValue (fileExt + "file\\shell", "", "open");
				res &= RDGenerics.SetFileExtensionValue (fileExt + "file\\shell\\open", "Icon",
					RDGenerics.AppStartupPath + fileExt + ".ico");
				res &= RDGenerics.SetFileExtensionValue (fileExt + "file\\shell\\open\\command", "",
					"\"" + Application.ExecutablePath + "\" \"%1\"");
				}
			else
				{
				res &= RDGenerics.SetFileExtensionValue (fileExt + "file", "NoOpen", "");
				}

			return res;
			}

		/// <summary>
		/// Метод выполняет регистрацию указанного протокола и привязывает его к текущему приложению
		/// </summary>
		/// <param name="ProtocolCode">Имя протокола</param>
		/// <param name="ProtocolName">Название протокола</param>
		/// <param name="ShowWarning">Флаг указывает, что необходимо отобразить предупреждение 
		/// перед регистрацией</param>
		/// <param name="FileIcon">Ресурс, хранящий значок формата файла</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public static bool RegisterProtocol (string ProtocolCode, string ProtocolName, Icon FileIcon,
			bool ShowWarning)
			{
			// Подготовка
			string protocol = ProtocolCode.ToLower ();

			// Контроль
			if (ShowWarning && (RDGenerics.MessageBox (RDMessageTypes.Warning_Left,
				locale[(int)Localization.CurrentLanguage][28],
				Localization.GetDefaultText (LzDefaultTextValues.Button_Yes),
				Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel)) ==
				RDMessageButtons.ButtonTwo))
				return false;

			// Выполнение
			try
				{
				// Запись значка
				FileStream FS = new FileStream (protocol + ".ico", FileMode.Create);
				FileIcon.Save (FS);
				FS.Close ();
				}
			catch
				{
				return false;
				}

			// Запись значений реестра
			bool res = true;
			/*res &= RDGenerics.SetAppSettingsValue ("", ProtocolName, "HKEY_CLASSES_ROOT\\" + protocol);
			res &= RDGenerics.SetAppSettingsValue ("URL Protocol", "", "HKEY_CLASSES_ROOT\\" + protocol);*/
			res &= RDGenerics.SetFileExtensionValue (protocol, "", ProtocolName);
			res &= RDGenerics.SetFileExtensionValue (protocol, "URL Protocol", "");

			/*res &= RDGenerics.SetAppSettingsValue ("", RDGenerics.AppStartupPath + protocol + ".ico",
				"HKEY_CLASSES_ROOT\\" + protocol + "\\DefaultIcon");*/
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\DefaultIcon", "",
				RDGenerics.AppStartupPath + protocol + ".ico");

			/*res &= RDGenerics.SetAppSettingsValue ("", "open", "HKEY_CLASSES_ROOT\\" + protocol + "\\shell");
			res &= RDGenerics.SetAppSettingsValue ("Icon", RDGenerics.AppStartupPath + protocol + ".ico",
				"HKEY_CLASSES_ROOT\\" + protocol + "\\shell\\open");
			res &= RDGenerics.SetAppSettingsValue ("", "\"" + Application.ExecutablePath + "\" \"%1\"",
				"HKEY_CLASSES_ROOT\\" + protocol + "\\shell\\open\\command");*/
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\shell", "", "open");
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\shell\\open", "Icon",
				RDGenerics.AppStartupPath + protocol + ".ico");
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\shell\\open\\command", "",
				"\"" + Application.ExecutablePath + "\" \"%1\"");

			return res;
			}
		}
	}
