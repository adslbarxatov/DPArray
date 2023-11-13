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
			new string[] { "<p", Localization.RN + "<" },
			new string[] { "<li>", Localization.RN + "• " },
			new string[] { "</p>", Localization.RN },
			new string[] { "<br", Localization.RN + "<" },

			new string[] { "<h1", Localization.RN + "<" },
			new string[] { "</h1>", Localization.RN },
			new string[] { "<h3", Localization.RN + "<" },

			new string[] { "&gt;", "›" },
			new string[] { "&lt;", "‹" },
			new string[] { "&#39;", "’" },
			new string[] { "&bull;", "•" }
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
			if (string.IsNullOrWhiteSpace (ProgramDescription.AssemblyReferenceMaterials[0]))
				userManualLink = "";
			else if (ProgramDescription.AssemblyReferenceMaterials[0] == DefaultRefMaterialAlias)
				userManualLink = RDGenerics.AssemblyGitPageLink;
			else
				userManualLink = ProgramDescription.AssemblyReferenceMaterials[0];

			if (string.IsNullOrWhiteSpace (ProgramDescription.AssemblyReferenceMaterials[1]))
				userVideomanualLink = "";
			else
				userVideomanualLink = RDGenerics.StaticYTLink + ProgramDescription.AssemblyReferenceMaterials[1];

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
				description = File.ReadAllText (Localization.GetHelpFilePath (Localization.CurrentLanguage),
					RDGenerics.GetEncoding (SupportedEncodings.UTF8));
				}
			catch
				{
				description = Localization.GetDefaultText (LzDefaultTextValues.Message_NoOfflineHelp);
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
			try
				{
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
				RDGenerics.SetDPArraySettingsValue (LastHypeHelpKey, lastHypeHelp.ToString ());
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
			int al = (int)Localization.CurrentLanguage;

			UpdatesPageButton.Text =
				Localization.GetDefaultText (LzDefaultTextValues.Message_CheckingUpdates);

			ExitButton.Text = Localization.GetDefaultText (AcceptMode ? LzDefaultTextValues.Button_Accept :
				LzDefaultTextValues.Button_OK);

			MisacceptButton.Text =
				Localization.GetDefaultText (LzDefaultTextValues.Button_Decline);

			if (!desciptionHasBeenUpdated)
				{
				if (AcceptMode)
					DescriptionBox.Text =
						Localization.GetDefaultText (LzDefaultTextValues.Message_PolicyFailure);
				else
					DescriptionBox.Text =
						Localization.GetDefaultText (LzDefaultTextValues.Message_CheckingUpdatesPrefix) +
						Localization.RN + description;
				}

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
					if (!string.IsNullOrWhiteSpace (userVideomanualLink))
						{
						linkTypes.Add (LinkTypes.UserVideomanual);
						ToLaboratoryCombo.Items.Add (Localization.GetDefaultText (LzDefaultTextValues.Control_UserVideomanual));
						}

					linkTypes.Add (LinkTypes.ProjectPage);
					ToLaboratoryCombo.Items.Add (Localization.GetDefaultText (LzDefaultTextValues.Control_ProjectWebpage));

					linkTypes.Add (LinkTypes.AskDeveloper);
					ToLaboratoryCombo.Items.Add (Localization.GetDefaultText (LzDefaultTextValues.Control_AskDeveloper));
					}

				linkTypes.Add (LinkTypes.ADP);
				ToLaboratoryCombo.Items.Add (AcceptMode ?
					Localization.GetDefaultText (LzDefaultTextValues.Message_OpenInBrowser) :
					Localization.GetDefaultText (LzDefaultTextValues.Control_PolicyEULA));

				linkTypes.Add (LinkTypes.ToLabMain);
				linkTypes.Add (LinkTypes.ToLabTG);
				linkTypes.Add (LinkTypes.ToLabVK);
				ToLaboratoryCombo.Items.AddRange (RDGenerics.CommunitiesNames);
				}
			ToLaboratoryCombo.SelectedIndex = 0;

			this.Text = AcceptMode ?
				Localization.GetDefaultText (LzDefaultTextValues.Control_PolicyEULA) :
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
				hwe = new HardWorkExecutor (PolicyLoader, null,
					Localization.GetDefaultText (LzDefaultTextValues.Message_PreparingForLaunch),
					true, false, true);
#else
				hwe = new HardWorkExecutor (PolicyLoader, null,
					Localization.GetDefaultText (LzDefaultTextValues.Message_PreparingForLaunch),
					true, false);
#endif

				string html = hwe.Result.ToString ();
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
			UpdatesPageButton.Visible = !AcceptMode;
#endif

			MisacceptButton.Visible = AcceptMode;

			// Запуск с управлением настройками окна
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
					Localization.GetDefaultText (LzDefaultTextValues.Message_DPArrayIsntInstalled),
					Localization.GetDefaultText (LzDefaultTextValues.Button_Yes),
					Localization.GetDefaultText (LzDefaultTextValues.Button_Guide),
					Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel)))
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
					Localization.GetDefaultText (LzDefaultTextValues.Message_StartPackageDownloading),
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
					msg = Localization.GetDefaultText (LzDefaultTextValues.Message_PackageLoadingFailure);
					break;

				case -3:
					msg = Localization.GetDefaultText (LzDefaultTextValues.Message_PackageSavingFailure);
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
					Localization.GetDefaultText (LzDefaultTextValues.Message_HypeHelp));
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
			versionDescription = Localization.RN + ApplyReplacements (versionDescription);

			// Отображение результата
			if (ProgramDescription.AssemblyTitle.EndsWith (version))
				{
				updatesMessage = Localization.GetDefaultText (LzDefaultTextValues.Message_UpToDate);
				updatesMessageForText = Localization.GetDefaultText (LzDefaultTextValues.Message_UpToDatePrefix);
				}
			else
				{
				updatesMessage =
					string.Format (Localization.GetDefaultText (LzDefaultTextValues.Message_UpdateAvailable),
					version);
				updatesMessageForText =
					string.Format (Localization.GetDefaultText (LzDefaultTextValues.Message_UpdateAvailablePrefix),
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
				Localization.GetDefaultText (LzDefaultTextValues.Message_ServerUnavailable);
			updatesMessageForText =
				Localization.GetDefaultText (LzDefaultTextValues.Message_ServerUnavailablePrefix);

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
		private void UpdatesTimer_Tick (object sender, EventArgs e)
			{
			if (string.IsNullOrWhiteSpace (updatesMessage))
				return;

			// Получение описания версии
			if (!string.IsNullOrWhiteSpace (versionDescription))
				{
				description += (Localization.RN + versionDescription);
				versionDescription = "";
				}

			// Обновление состояния
			if (!desciptionHasBeenUpdated)
				{
				DescriptionBox.Text = updatesMessageForText + Localization.RNRN + Localization.RN +
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
				UpdatesPageButton.Text = Localization.GetDefaultText (LzDefaultTextValues.Message_ManualDownload);
				}
			}

		// Непринятие Политики
		private void MisacceptButton_Click (object sender, EventArgs e)
			{
			policyAccepted = false;
			UpdatesTimer.Enabled = false;
			this.Close ();
			}

		/// <summary>
		/// Метод выполняет регистрацию указанного расширения файла и привязывает его к текущему приложению
		/// </summary>
		/// <param name="FileExtension">Расширение файла</param>
		/// <param name="FileTypeName">Название типа файла</param>
		/// <param name="Openable">Флаг указывает, будет ли файл доступен для открытия в приложении</param>
		/// <param name="ShowWarning">Флаг указывает, что необходимо отобразить предупреждение
		/// перед регистрацией</param>
		/// <param name="FileIcon">Ресурс, хранящий значок формата файла</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public static bool RegisterFileExtension (string FileExtension, string FileTypeName, Icon FileIcon,
			bool Openable, bool ShowWarning)
			{
			// Подготовка
			string fileExt = FileExtension.ToLower ().Replace (".", "");

			// Контроль
			if (ShowWarning && (RDGenerics.MessageBox (RDMessageTypes.Warning_Left,
				Localization.GetDefaultText (LzDefaultTextValues.Message_ExtensionsRegistration),
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
			res &= RDGenerics.SetFileExtensionValue ("." + fileExt, "", fileExt + "file");
			res &= RDGenerics.SetFileExtensionValue (fileExt + "file", "", FileTypeName);
			res &= RDGenerics.SetFileExtensionValue (fileExt + "file\\DefaultIcon", "",
				RDGenerics.AppStartupPath + fileExt + ".ico");

			if (Openable)
				{
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
				Localization.GetDefaultText (LzDefaultTextValues.Message_ProtocolsRegistration),
				Localization.GetDefaultText (LzDefaultTextValues.Button_Yes),
				Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel)) ==
				RDMessageButtons.ButtonTwo))
				return false;

			// Выполнение
			try
				{
				// Запись значка
				FileStream FS = new FileStream (RDGenerics.AppStartupPath + protocol + ".ico", FileMode.Create);
				FileIcon.Save (FS);
				FS.Close ();
				}
			catch
				{
				return false;
				}

			// Запись значений реестра
			bool res = true;
			res &= RDGenerics.SetFileExtensionValue (protocol, "", ProtocolName);
			res &= RDGenerics.SetFileExtensionValue (protocol, "URL Protocol", "");
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\DefaultIcon", "",
				RDGenerics.AppStartupPath + protocol + ".ico");
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\shell", "", "open");
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\shell\\open", "Icon",
				RDGenerics.AppStartupPath + protocol + ".ico");
			res &= RDGenerics.SetFileExtensionValue (protocol + "\\shell\\open\\command", "",
				"\"" + Application.ExecutablePath + "\" \"%1\"");

			return res;
			}
		}
	}
