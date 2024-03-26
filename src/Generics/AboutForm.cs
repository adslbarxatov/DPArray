using System;
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
		private string projectLink, updatesLink, userManualLink, userVideomanualLink;
		private string updatesMessage = "", updatesMessageForText = "", description = "",
			versionDescription = "", adpRevision = "";
		private bool policyAccepted = false;
		private bool startupMode = false;
		private bool desciptionHasBeenUpdated = false;
		private const string newPolicyAlias = "!";

		/// <summary>
		/// Ключ реестра, хранящий версию, на которой отображалась справка
		/// </summary>
		public const string LastShownVersionKey = "HelpShownAt";

		// Ключ реестра, хранящий последнюю принятую версию ADP
		private const string ADPRevisionKey = "ADPRevision";

		// Элементы поддержки HypeHelp
		private const string HypeHelpKey = "HypeHelp";
		private bool hypeHelp;
		private const string LastHypeHelpKey = "LastHypeHelp";
		/*private DateTime lastHypeHelp;*/

		private string[] hypeHelpLinks = new string[] {
			"https://vk.com/rd_aaow_fdl",
			"https://vk.com/grammarmustjoy",
			"https://t.me/rd_aaow_fdl",
			"https://t.me/grammarmustjoy",

			"https://youtube.com/c/rdaaowfdl",

			"https://youtu.be/0_78MtyTAdA",
			"https://youtu.be/UlB0zh3YH3A",
			"https://youtu.be/QiOMIN4aE-Q",
			"https://youtu.be/afbtGUPnZ2w",
			"https://youtu.be/gdg-kN8ALyI",
			"https://youtu.be/UPfBmVmQvZA",
			"https://youtu.be/hU62FlQ5JNk",
			"https://youtu.be/gjewz9mQMgI",
			"https://youtu.be/Dqcrs0F6Gq0",
			"https://youtu.be/xy0LDXgoR5U",
			"https://youtu.be/n2DNUsWvfpQ",
			"https://youtu.be/x3ImaRYH7_A",
			"https://youtu.be/b0rn2wIuU0Y",
			"https://youtu.be/-1HR72BJS-E",
			"https://youtu.be/25BYSySdAJk",
			"https://youtu.be/gjs9K1EsFG8",
			"https://youtu.be/nOb4MbL-jlI",

			"https://moddb.com/mods/esrm",
			"https://moddb.com/mods/eshq",
			"https://moddb.com/mods/ccm",
			};
		/*private Random rnd = new Random ();*/

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
			new string[] { "<p", RDLocale.RN + "<" },
			new string[] { "<li>", RDLocale.RN + "• " },
			new string[] { "</p>", RDLocale.RN },
			new string[] { "<br", RDLocale.RN + "<" },

			new string[] { "<h1", RDLocale.RN + "<" },
			new string[] { "</h1>", RDLocale.RN },
			new string[] { "<h3", RDLocale.RN + "<" },

			new string[] { "&gt;", "›" },
			new string[] { "&lt;", "‹" },
			new string[] { "&#39;", "’" },
			new string[] { "&bull;", "•" },
			new string[] { "&ldquo;", "“" },
			new string[] { "&rdquo;", "”" },
			};

		// Доступные варианты перехода к ресурсам Лаборатории
		private enum LinkTypes
			{
			UserManual,
			UserVideomanual,
			ProjectPage,
			ADP,
			AskDeveloper,
			ToLabMain,
			ToLabVK,
			ToLabTG,
			Donate,
			}
		private List<LinkTypes> linkTypes = new List<LinkTypes> ();

		/// <summary>
		/// Возвращает псевдоним для справочного материала по умолчанию
		/// </summary>
		public const string DefaultRefMaterialAlias = "D";

		/// <summary>
		/// Возвращает псевдоним для отсутствующего справочного материала
		/// </summary>
		public const string MissingRefMaterialAlias = "";

		/// <summary>
		/// Конструктор. Инициализирует форму
		/// </summary>
		public AboutForm ()
			{
			// Инициализация
			InitializeComponent ();
			this.AcceptButton = ExitButton;
			this.CancelButton = MisacceptButton;

			// Получение параметров
			bool localized = (ProgramDescription.AssemblyLocalizedReferences.Length >=
				RDLocale.LanguagesNames.Length * 2);
			short locOffset = (short)RDLocale.CurrentLanguage;

			if (veryFirstStart < 0)
				veryFirstStart =
					string.IsNullOrWhiteSpace (RDGenerics.GetAppSettingsValue (LastShownVersionKey)) ? 1 : 0;

			string line = ProgramDescription.AssemblyLocalizedReferences[localized ? 0 + locOffset : 0];
			if (string.IsNullOrWhiteSpace (line))
				userManualLink = "";
			else if (line == DefaultRefMaterialAlias)
				userManualLink = localized ? RDGenerics.AssemblyLocalizedGitPageLink :
					RDGenerics.AssemblyGitPageLink;
			else
				userManualLink = line;

			line = ProgramDescription.AssemblyLocalizedReferences[localized ?
				RDLocale.LanguagesNames.Length + locOffset : 1];
			if (string.IsNullOrWhiteSpace (line))
				userVideomanualLink = "";
			else
				userVideomanualLink = RDGenerics.StaticYTLink + line;

			projectLink = RDGenerics.DefaultGitLink + ProgramDescription.AssemblyMainName;
			updatesLink = RDGenerics.DefaultGitLink + ProgramDescription.AssemblyMainName +
				RDGenerics.GitUpdatesSublink;

			// Загрузка окружения
			AboutLabel.Text = RDGenerics.AppAboutLabelText;
			IconBox.BackgroundImage = (Bitmap)ProgramDescription.AssemblyResources[0].GetObject ("LogoIcon");

			AboutForm_Resize (null, null);
			}

		/// <summary>
		/// Метод отображает справочное окно приложения
		/// </summary>
		/// <param name="StartupMode">Флаг, указывающий, что справка не должна отображаться, если
		/// она уже была показана для данной версии приложения</param>
		/// <returns>Возвращает:
		/// 1, если справка уже отображалась для данной версии (при StartupMode == true);
		/// другое значение, если окно справки было отображено</returns>
		public int ShowAbout (bool StartupMode)
			{
			try
				{
				description = File.ReadAllText (RDLocale.GetHelpFilePath (RDLocale.CurrentLanguage),
					RDGenerics.GetEncoding (RDEncodings.UTF8));
				}
			catch
				{
				description = RDLocale.GetDefaultText (RDLDefaultTexts.Message_NoOfflineHelp);
				}
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
			hypeHelp = RDGenerics.GetDPArraySettingsValue (HypeHelpKey) == "1";
			if (hypeHelp)
				{
				DateTime lastHypeHelp;
				try
					{
					lastHypeHelp = DateTime.Parse (RDGenerics.GetDPArraySettingsValue (LastHypeHelpKey));
					}
				catch
					{
					lastHypeHelp = DateTime.Now;
					}

				if (/*hypeHelp &&*/ (StartupMode || AcceptMode) && (lastHypeHelp <= DateTime.Now))
					{
					RDGenerics.RunWork (HypeHelper, null, null, RDRunWorkFlags.DontSuspendExecution);

					lastHypeHelp = DateTime.Now.AddMinutes (RDGenerics.RND.Next (65, 95));
					RDGenerics.SetDPArraySettingsValue (LastHypeHelpKey, lastHypeHelp.ToString ());
					}
				}

			// Запрос настроек
			adpRevision = RDGenerics.GetDPArraySettingsValue (ADPRevisionKey);
			string helpShownAt = RDGenerics.GetAppSettingsValue (LastShownVersionKey);

			// Если поле пустое, устанавливается минимальное значение
			if (adpRevision == "")
				{
				adpRevision = "rev. 10" + newPolicyAlias;
				RDGenerics.SetDPArraySettingsValue (ADPRevisionKey, adpRevision);
				}

			// Контроль
			startupMode = StartupMode;
			if (StartupMode && (helpShownAt == ProgramDescription.AssemblyVersion) ||   // Справка уже отображалась
				AcceptMode && (!adpRevision.EndsWith (newPolicyAlias)))                 // Политика уже принята
				return 1;

			// Настройка контролов
			int al = (int)RDLocale.CurrentLanguage;

			UpdatesPageButton.Text =
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_CheckingUpdates);

			ExitButton.Text = RDLocale.GetDefaultText (AcceptMode ? RDLDefaultTexts.Button_Accept :
				RDLDefaultTexts.Button_OK);

			MisacceptButton.Text =
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Decline);

			if (!desciptionHasBeenUpdated)
				{
				if (AcceptMode)
					DescriptionBox.Text =
						RDLocale.GetDefaultText (RDLDefaultTexts.Message_PolicyFailure);
				else
					DescriptionBox.Text =
						RDLocale.GetDefaultText (RDLDefaultTexts.Message_CheckingUpdatesPrefix) +
						RDLocale.RN + description;
				}

			// Загрузка списка доступных переходов к ресурсам Лаборатории
			if (ToLaboratoryCombo.Items.Count < 1)
				{
				if (!AcceptMode)
					{
					if (!string.IsNullOrWhiteSpace (userManualLink))
						{
						linkTypes.Add (LinkTypes.UserManual);
						ToLaboratoryCombo.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Control_UserManual));
						}
					if (!string.IsNullOrWhiteSpace (userVideomanualLink))
						{
						linkTypes.Add (LinkTypes.UserVideomanual);
						ToLaboratoryCombo.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Control_UserVideomanual));
						}

					linkTypes.Add (LinkTypes.ProjectPage);
					ToLaboratoryCombo.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Control_ProjectWebpage));

					linkTypes.Add (LinkTypes.AskDeveloper);
					ToLaboratoryCombo.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Control_AskDeveloper));
					}

				linkTypes.Add (LinkTypes.ADP);
				ToLaboratoryCombo.Items.Add (AcceptMode ?
					RDLocale.GetDefaultText (RDLDefaultTexts.Message_OpenInBrowser) :
					RDLocale.GetDefaultText (RDLDefaultTexts.Control_PolicyEULA));

				linkTypes.Add (LinkTypes.ToLabMain);
				linkTypes.Add (LinkTypes.ToLabTG);
				linkTypes.Add (LinkTypes.ToLabVK);
				linkTypes.Add (LinkTypes.Donate);

				ToLaboratoryCombo.Items.AddRange (RDGenerics.CommunitiesNames);
				ToLaboratoryCombo.Items.Add (
					RDLocale.GetDefaultText (RDLDefaultTexts.Control_HelpTheProject));
				}
			ToLaboratoryCombo.SelectedIndex = 0;

			this.Text = AcceptMode ?
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_PolicyEULA) :
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout);

			// Запуск проверки обновлений
			if (!AcceptMode)
				{
				UpdatesPageButton.Enabled = false;
				RDGenerics.RunWork (UpdatesChecker, null, null, RDRunWorkFlags.DontSuspendExecution);
				UpdatesTimer.Enabled = true;
				}

			// Получение Политики
			else
				{
				RDGenerics.RunWork (PolicyLoader, null,
					RDLocale.GetDefaultText (RDLDefaultTexts.Message_PreparingForLaunch),
					RDRunWorkFlags.CaptionInTheMiddle | RDRunWorkFlags.AlwaysOnTop);

				string html = RDGenerics.WorkResultAsString;
				if (!string.IsNullOrWhiteSpace (html))
					{
					DescriptionBox.Text = html;

					string adpRev = ExtractPolicyRevision (html);
					if (!string.IsNullOrWhiteSpace (adpRev))
						adpRevision = adpRev;
					}
				}

			// Настройка контролов
			HypeHelpFlag.Visible = !AcceptMode;

#if DPMODULE
			UpdatesPageButton.Visible = false;
#else
			UpdatesPageButton.Visible = !AcceptMode && !RDGenerics.StartedFromMSStore;
#endif

			MisacceptButton.Visible = AcceptMode;

			// Запуск с управлением настройками окна
			if (RDGenerics.StartedFromMSStore)
				HypeHelpFlag.Checked = HypeHelpFlag.Visible = false;
			else
				HypeHelpFlag.Checked = hypeHelp;

			RDGenerics.LoadAppAboutWindowDimensions (this);

			this.ShowDialog ();

			RDGenerics.SaveAppAboutWindowDimensions (this);
			RDGenerics.SetDPArraySettingsValue (HypeHelpKey, HypeHelpFlag.Checked ? "1" : "0");

			// Запись версий по завершению
			if (StartupMode)
				RDGenerics.SetAppSettingsValue (LastShownVersionKey, ProgramDescription.AssemblyVersion);

			// В случае невозможности загрузки Политики признак необходимости принятия до этого момента
			// не удаляется из строки версии. Поэтому требуется страховка
			if (AcceptMode && policyAccepted)
				RDGenerics.SetDPArraySettingsValue (ADPRevisionKey, adpRevision.Replace (newPolicyAlias, ""));

			// Завершение
			return policyAccepted ? 0 : -1;
			}

		// Метод получает Политику разработки
		private void PolicyLoader (object sender, DoWorkEventArgs e)
			{
			e.Result = GetPolicy ();
			}

		// Метод загружает текст Политики
		private string GetPolicy ()
			{
			string html = RDGenerics.GetHTML (RDGenerics.ADPLink);
			int textLeft, textRight;

			if (((textLeft = html.IndexOf ("code\">")) < 0) ||
				((textRight = html.IndexOf ("<footer", textLeft)) < 0))
				{
				return "";
				}

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

			html = html.Substring (1, html.Length - 48);    // Финальная обрезка
			return html;
			}

		// Метод извлекает из загруженного текста Политики её версию
		private string ExtractPolicyRevision (string LoadedPolicy)
			{
			int left, right;

			if (((left = LoadedPolicy.IndexOf ("rev")) < 0) ||
				((right = LoadedPolicy.IndexOf ("\n", left)) < 0))
				return "";

			return LoadedPolicy.Substring (left, right - left);
			}

		/// <summary>
		/// Возвращает true, если приложение запущено впервые
		/// </summary>
		public static bool VeryFirstStart
			{
			get
				{
				return (veryFirstStart > 0);
				}
			}
		private static int veryFirstStart = -1;

		/*
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
		*/

		// Закрытие окна
		private void ExitButton_Click (object sender, EventArgs e)
			{
			policyAccepted = true;
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
					link = "mailto://" + RDGenerics.LabMailLink + ("?subject=" +
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

				case LinkTypes.UserVideomanual:
					link = userVideomanualLink;
					break;

				case LinkTypes.Donate:
					link = RDGenerics.DPArrayContacts;
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
			string dpmv = RDGenerics.GetDPArraySettingsValue (LastShownVersionKey);
			string downloadLink, packagePath;

			int l;
			if (string.IsNullOrWhiteSpace (dpmv))
				{
				// Выбор варианта обработки
				switch (RDGenerics.MessageBox (RDMessageTypes.Question_Left,
					RDLocale.GetDefaultText (RDLDefaultTexts.Message_DPArrayIsntInstalled),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Guide),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel)))
					{
					case RDMessageButtons.ButtonThree:
						return;

					case RDMessageButtons.ButtonTwo:
						RDGenerics.RunURL (RDGenerics.DPArrayLink);
						return;
					}

				downloadLink = RDGenerics.DPArrayDirectLink;
				packagePath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + "\\";
				}
			else
				{
				if (RDGenerics.MessageBox (RDMessageTypes.Question_Center,
					RDLocale.GetDefaultText (RDLDefaultTexts.Message_StartPackageDownloading),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_No)) !=
					RDMessageButtons.ButtonOne)
					return;

#if MONOGAME
				downloadLink = packagePath = "";
				RDGenerics.RunURL (RDGenerics.DPArrayProtocolPrefix + ProgramDescription.AssemblyMainName);
				return;
#else
				downloadLink = RDGenerics.DPArrayPackageLink;
				packagePath = RDGenerics.GetDPArraySettingsValue (RDGenerics.DPArrayName);

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
			RDGenerics.RunWork (RDGenerics.PackageLoader, downloadLink, packagePath, "0", false);

			// Разбор ответа
			string msg = "";
			int res = RDGenerics.WorkResultAsInteger;
			switch (res)
				{
				case 0:
					break;

				case -1:
				case -2:
					msg = RDLocale.GetDefaultText (RDLDefaultTexts.Message_PackageLoadingFailure);
					break;

				case -3:
					msg = RDLocale.GetDefaultText (RDLDefaultTexts.Message_PackageSavingFailure);
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
				RDGenerics.MessageBox (RDMessageTypes.Success_Left,
					RDLocale.GetDefaultText (RDLDefaultTexts.Message_HypeHelp));
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
			string html = RDGenerics.GetHTML (updatesLink);
			bool htmlError = true;  // Сбрасывается при успешной загрузке

			// Разбор ответа (извлечение версии)
			string versionMarker = ProgramDescription.AssemblyMainName + " v ";
			int i = html.IndexOf (versionMarker);
			if (i < 0)
				goto policy;

			i += versionMarker.Length;

			int j = html.IndexOf ("<", i);
			if ((j < 0) || (j <= i))
				goto policy;

			string version = "v " + html.Substring (i, j - i).Trim ();

			// Разбор ответа (извлечение версии)
			i = html.IndexOf (ChangeLogMarkerLeft);
			if (i < 0)
				goto policy;

			i += ChangeLogMarkerLeft.Length;
			j = html.IndexOf (ChangeLogMarkerRight, i);
			if ((j < 0) || (j <= i))
				goto policy;

			versionDescription = html.Substring (i, j - i);
			versionDescription = RDLocale.RN + ApplyReplacements (versionDescription);

			// Отображение результата
			if (ProgramDescription.AssemblyTitle.EndsWith (version))
				{
				updatesMessage = RDLocale.GetDefaultText (RDLDefaultTexts.Message_UpToDate);
				updatesMessageForText = RDLocale.GetDefaultText (RDLDefaultTexts.Message_UpToDatePrefix);
				}
			else
				{
				updatesMessage =
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_UpdateAvailable_Fmt),
					version);
				updatesMessageForText =
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_UpdateAvailablePrefix_Fmt),
					version);
				}
			htmlError = false;

			// Получение обновлений Политики (ошибки игнорируются)
			policy:
			if (startupMode)
				{
				string adpRev = ExtractPolicyRevision (GetPolicy ());
				if (!string.IsNullOrWhiteSpace (adpRev) && (adpRev != adpRevision))
					RDGenerics.SetDPArraySettingsValue (ADPRevisionKey, adpRev + newPolicyAlias);
				}

			// Не было проблем с загрузкой страницы
			if (!htmlError)
				{
				e.Result = 0;
				return;
				}

			// Есть проблема при загрузке страницы. Отмена
			updatesMessage =
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_ServerUnavailable);
			updatesMessageForText =
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_ServerUnavailablePrefix);

			e.Result = -2;
			return;
			}

		// Метод выполняет фоновую проверку обновлений
		private void HypeHelper (object sender, DoWorkEventArgs e)
			{
			RDGenerics.GetHTML (hypeHelpLinks[RDGenerics.RND.Next (hypeHelpLinks.Length)]);
			e.Result = 0;
			}

		// Контроль сообщения об обновлении
		private void UpdatesTimer_Tick (object sender, EventArgs e)
			{
			if (string.IsNullOrWhiteSpace (updatesMessage))
				return;

			// Получение описания версии
			if (!string.IsNullOrWhiteSpace (versionDescription))
				{
				description += (RDLocale.RN + versionDescription);
				versionDescription = "";
				}

			// Обновление состояния
			if (!desciptionHasBeenUpdated)
				{
				DescriptionBox.Text = updatesMessageForText + RDLocale.RNRN + RDLocale.RN +
					description;
				desciptionHasBeenUpdated = true;
				}

			// Включение текста кнопки
			if (UpdatesPageButton.Text.Contains ("..."))
				{
				UpdatesPageButton.Text = updatesMessage;

				// Включение кнопки и установка интервала
				if (!UpdatesPageButton.Enabled)
					{
					// Не запрещать загрузку вручную даже при отсутствии доступа к информации
					UpdatesPageButton.Enabled = true;

					UpdatesTimer.Interval = 2000;
					if (updatesMessage.Contains ("."))
						UpdatesPageButton.Font = new Font (UpdatesPageButton.Font, FontStyle.Bold);
					}
				}

			// Выключение
			else
				{
				UpdatesPageButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Message_ManualDownload);
				}
			}

		// Непринятие Политики
		private void MisacceptButton_Click (object sender, EventArgs e)
			{
			policyAccepted = false;
			UpdatesTimer.Enabled = false;
			this.Close ();
			}
		}
	}
