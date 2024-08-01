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
		private string updatesMessage = "";
		private string versionDescription = "";
		private string adpRevision = "";
		private bool policyAccepted = false;
		private bool startupMode = false, acceptMode = false;
		private const string newPolicyAlias = "!";

		/// <summary>
		/// Ключ реестра, хранящий версию, на которой отображалась справка
		/// </summary>
		public const string LastShownVersionKey = "HelpShownAt";

		// Ключ реестра, хранящий последнюю принятую версию ADP
		private const string ADPRevisionKey = "ADPRevision";

		// Элементы поддержки HypeHelp
		private const string HypeHelpKey = "HypeHelp";
		private const string LastHypeHelpKey = "LastHypeHelp";

		private string[] hypeHelpLinks = new string[] {
			"https://vk.com/rd_aaow_fdl",
			"https://vk.com/grammarmustjoy",
			"https://t.me/rd_aaow_fdl",
			"https://t.me/grammarmustjoy",

			"https://youtube.com/c/rdaaowfdl",

#if HYPE_YT
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
#endif

			"https://moddb.com/mods/esrm",
			"https://moddb.com/mods/eshq",
			"https://moddb.com/mods/ccm",
			};

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
		private List<Button> linkButtons = new List<Button> ();

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

			// Получение параметров
			bool localized = (ProgramDescription.AssemblyLocalizedReferences.Length >=
				RDLocale.LanguagesNames.Length * 2);
			short locOffset = (short)RDLocale.CurrentLanguage;

			if (veryFirstStart < 0)
				veryFirstStart =
					string.IsNullOrWhiteSpace (RDGenerics.GetAppRegistryValue (LastShownVersionKey)) ? 1 : 0;

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
			// Запрос настроек
			adpRevision = RDGenerics.GetDPArrayRegistryValue (ADPRevisionKey);
			string helpShownAt = RDGenerics.GetAppRegistryValue (LastShownVersionKey);

			// Если поле пустое, устанавливается минимальное значение
			if (adpRevision == "")
				{
				adpRevision = "rev. 10" + newPolicyAlias;
				RDGenerics.SetDPArrayRegistryValue (ADPRevisionKey, adpRevision);
				}

			// Контроль
			startupMode = StartupMode;
			acceptMode = AcceptMode;
			if (StartupMode && (helpShownAt == ProgramDescription.AssemblyVersion) ||   // Справка уже отображалась
				AcceptMode && (!adpRevision.EndsWith (newPolicyAlias)))                 // Политика уже принята
				return 1;

			// Настройка контролов
			int al = (int)RDLocale.CurrentLanguage;

			ExitButton.Text = RDLocale.GetDefaultText (AcceptMode ? RDLDefaultTexts.Button_Accept :
				RDLDefaultTexts.Button_Exit);
			MisacceptButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Decline);
			if (AcceptMode)
				{
				this.AcceptButton = ExitButton;
				this.CancelButton = MisacceptButton;
				}
			else
				{
				this.CancelButton = ExitButton;
				}

			if (!AcceptMode)
				{
#if !DPMODULE
				if (!RDGenerics.StartedFromMSStore)
					AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Message_CheckingUpdates),
						UpdatesPageButton_Click, false);
#endif
				AddButton ("ChangeLog", ShowChangeLog_Click, false);
				buttonIndex += (buttonIndex % 2 + 2);

				if (File.Exists (RDLocale.GetHelpFilePath ()))
					AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_OfflineHelp),
						ShowOfflineHelp_Click, true);

				if (!string.IsNullOrWhiteSpace (userManualLink))
					AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_OnlineHelp),
						ShowOnlineHelp_Click, true);

				if (!string.IsNullOrWhiteSpace (userVideomanualLink))
					AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_UserVideomanual),
						ShowVideoguide_Click, true);

				AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_ProjectWebpage),
					ShowProjectPage_Click, true);
				}

			AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_PolicyEULA),
				ShowADP_Click, true);
			if (AcceptMode)
				buttonIndex++;
			AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_SocialPolicy),
				ShowSCP_Click, true);

			if (AcceptMode)
				{
				int amlb = linkButtons.Count - 2;
				linkButtons[amlb].Width = linkButtons[amlb + 1].Width = 404;
				linkButtons[amlb].BackColor = linkButtons[amlb + 1].BackColor =
					RDGenerics.GetInterfaceColor (RDInterfaceColors.WarningMessage);
				}

			buttonIndex += (buttonIndex % 2 + 2);

			AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_AskDeveloper),
				AskDeveloper_Click, true);

			AddButton (RDGenerics.CommunitiesNames[0], GoLabMain_Click, true);
			AddButton (RDGenerics.CommunitiesNames[1], GoLabTG_Click, true);
			AddButton (RDGenerics.CommunitiesNames[2], GoLabYT_Click, true);

			if (RDLocale.IsCurrentLanguageRuRu)
				AddButton (RDGenerics.CommunitiesNames[3], GoLabVK_Click, true);

			AddButton (RDLocale.GetDefaultText (RDLDefaultTexts.Control_HelpTheProject),
				GoDonate_Click, true);

			// Завершение формирования
			this.Text = AcceptMode ?
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_PolicyEULA) :
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout);

			buttonIndex += (buttonIndex % 2 + 2);
			MisacceptButton.Top = ExitButton.Top = HypeHelpFlag.Top =
				linkButtons[linkButtons.Count - 1].Top + linkButtons[linkButtons.Count - 1].Height + 24;

			this.MinimumSize = new Size (this.Width, ExitButton.Top + ExitButton.Height + 12);

			// Получение Политики
			if (AcceptMode)
				{
				RDGenerics.RunWork (PolicyLoader, null,
					RDLocale.GetDefaultText (RDLDefaultTexts.Message_PreparingForLaunch),
					RDRunWorkFlags.CaptionInTheMiddle | RDRunWorkFlags.AlwaysOnTop);

				string html = RDGenerics.WorkResultAsString;
				if (!string.IsNullOrWhiteSpace (html))
					{
					string adpRev = ExtractPolicyRevision (html);
					if (!string.IsNullOrWhiteSpace (adpRev))
						adpRevision = adpRev;
					}

				MisacceptButton.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.ErrorMessage);
				ExitButton.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.SuccessMessage);
				}
			else
				{
				ExitButton.BackColor = HypeHelpFlag.BackColor =
					RDGenerics.GetInterfaceColor (RDInterfaceColors.LightEmerald);
				ExitButton.ForeColor = HypeHelpFlag.ForeColor = this.BackColor;
				}

			// Настройка контролов
			HypeHelpFlag.Visible = !AcceptMode;
			MisacceptButton.Visible = AcceptMode;

			// Запуск с управлением настройками окна
			if (RDGenerics.StartedFromMSStore)
				HypeHelpFlag.Checked = HypeHelpFlag.Visible = false;
			else
				HypeHelpFlag.Checked = (RDGenerics.GetDPArrayRegistryValue (HypeHelpKey) == "1");

			this.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.LightGrey);
			AboutLabel.BackColor = IconBox.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.LightEmerald);
			AboutLabel.ForeColor = this.BackColor;

			RDMessageForm.CreateBackground (this, (uint)(2 * IconBox.Top + IconBox.Height));

			this.ShowDialog ();

			RDGenerics.SetDPArrayRegistryValue (HypeHelpKey, HypeHelpFlag.Checked ? "1" : "0");

			// HypeHelp (только если окно отображено)
			if (HypeHelpFlag.Checked)
				{
				DateTime lastHypeHelp;
				try
					{
					lastHypeHelp = DateTime.Parse (RDGenerics.GetDPArrayRegistryValue (LastHypeHelpKey));
					}
				catch
					{
					lastHypeHelp = DateTime.Now;
					}

				if (!AcceptMode && (lastHypeHelp <= DateTime.Now))
					{
					RDGenerics.RunWork (HypeHelper, null, null, RDRunWorkFlags.DontSuspendExecution);

					lastHypeHelp = DateTime.Now.AddMinutes (RDGenerics.RND.Next (65, 95));
					RDGenerics.SetDPArrayRegistryValue (LastHypeHelpKey, lastHypeHelp.ToString ());
					}
				}

			// Запись версий по завершению
			if (StartupMode)
				RDGenerics.SetAppRegistryValue (LastShownVersionKey, ProgramDescription.AssemblyVersion);

			// В случае невозможности загрузки Политики признак необходимости принятия до этого момента
			// не удаляется из строки версии. Поэтому требуется страховка
			if (AcceptMode && policyAccepted)
				RDGenerics.SetDPArrayRegistryValue (ADPRevisionKey, adpRevision.Replace (newPolicyAlias, ""));

			// Завершение
			return policyAccepted ? 0 : -1;
			}

		// Добавление кнопок в интерфейс
		private void AddButton (string Text, EventHandler Method, bool Enabled)
			{
			linkButtons.Add (new Button ());
			Button b = linkButtons[linkButtons.Count - 1];

			b.Text = Text;
			b.Width = 200;
			b.Height = 26;
			b.Left = 7 + (buttonIndex % 2) * (b.Width + 6);
			b.Top = IconBox.Top + IconBox.Height + 24 + 27 * (int)(buttonIndex / 2);
			b.Click += Method;
			b.FlatStyle = FlatStyle.Flat;
			b.FlatAppearance.BorderSize = 0;
			b.Enabled = Enabled;

			this.Controls.Add (b);
			buttonIndex++;
			}
		private int buttonIndex = 0;

		// Запуск проверки обновлений (только при отображённом окне)
		private void AboutForm_Shown (object sender, EventArgs e)
			{
			if (acceptMode)
				return;

			RDGenerics.RunWork (UpdatesChecker, null, null, RDRunWorkFlags.DontSuspendExecution);
			UpdatesTimer.Enabled = true;
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

		// Закрытие окна
		private void ExitButton_Click (object sender, EventArgs e)
			{
			policyAccepted = true;
			UpdatesTimer.Enabled = false;
			this.Close ();
			}

		// Загрузка пакета обновления изнутри приложения
		private void UpdatesPageButton_Click (object sender, EventArgs e)
			{
#if !DPMODULE

			// Контроль наличия DPArray
			string dpmv = RDGenerics.GetDPArrayRegistryValue (LastShownVersionKey);
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
				packagePath = RDGenerics.GetDPArrayRegistryValue (RDGenerics.DPArrayName);

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

		// Отображение списка изменений в текущей версии
		private void ShowChangeLog_Click (object sender, EventArgs e)
			{
			RDGenerics.MessageBox (RDMessageTypes.Success_Left, versionDescription);
			}

		// Загрузка оффлайн и онлайн справок, видеоинструкции и остальных ссылок
		private void ShowOfflineHelp_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDLocale.GetHelpFilePath ());
			}

		private void ShowOnlineHelp_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (userManualLink);
			}

		private void ShowVideoguide_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (userVideomanualLink);
			}

		private void ShowProjectPage_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (projectLink);
			}

		private void ShowADP_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDGenerics.ADPLink);
			}

		private void ShowSCP_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDGenerics.SCPLink);
			}

		private void GoLabMain_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDGenerics.DPArrayLink);
			}

		private void GoLabVK_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDGenerics.LabVKLink);
			}

		private void GoLabTG_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDGenerics.LabTGLink);
			}

		private void GoDonate_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDGenerics.DPArrayContacts);
			}

		private void AskDeveloper_Click (object sender, EventArgs e)
			{
			AskDeveloper ();
			}

		/// <summary>
		/// Метод запускает почтовый клиент и отображает черновик письма разработчику
		/// </summary>
		public static void AskDeveloper ()
			{
			RDGenerics.RunURL ("mailto://" + RDGenerics.LabMailLink + ("?subject=" +
				RDGenerics.LabMailCaption).Replace (" ", "%20"));
			}

		private void GoLabYT_Click (object sender, EventArgs e)
			{
			RDGenerics.RunURL (RDGenerics.LabYTLink);
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
			versionDescription = ApplyReplacements (versionDescription).Replace ("\r", "").Replace ("\n\n", "\n");

			// Отображение результата
			if (ProgramDescription.AssemblyTitle.EndsWith (version))
				updatesMessage = RDLocale.GetDefaultText (RDLDefaultTexts.Message_UpToDate);
			else
				updatesMessage = string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_UpdateAvailable_Fmt),
					version);
			htmlError = false;

			// Получение обновлений Политики (ошибки игнорируются)
		policy:
			if (startupMode)
				{
				string adpRev = ExtractPolicyRevision (GetPolicy ());
				if (!string.IsNullOrWhiteSpace (adpRev) && (adpRev != adpRevision))
					RDGenerics.SetDPArrayRegistryValue (ADPRevisionKey, adpRev + newPolicyAlias);
				}

			// Не было проблем с загрузкой страницы
			if (!htmlError)
				{
				e.Result = 0;
				return;
				}

			// Есть проблема при загрузке страницы. Отмена
			updatesMessage = RDLocale.GetDefaultText (RDLDefaultTexts.Message_ServerUnavailable);
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
			// Контроль 
			if (string.IsNullOrWhiteSpace (updatesMessage))
				return;

			if (RDGenerics.StartedFromMSStore)
				{
				linkButtons[0].Enabled = !string.IsNullOrWhiteSpace (versionDescription);
				return;
				}

			// Включение текста кнопки
#if !DPMODULE
			if (linkButtons[0].Text.Contains ("..."))
				{
				linkButtons[0].Text = updatesMessage;

				// Включение кнопки и установка интервала
				if (!linkButtons[0].Enabled)
					{
					// Не запрещать загрузку вручную даже при отсутствии доступа к информации
					linkButtons[0].Enabled = true;
					linkButtons[1].Enabled = !string.IsNullOrWhiteSpace (versionDescription);

					UpdatesTimer.Interval = 2000;
					if (updatesMessage.Contains ("."))
						linkButtons[0].Font = new Font (linkButtons[0].Font, FontStyle.Bold);
					}
				}

			// Выключение
			else
				{
				linkButtons[0].Text = RDLocale.GetDefaultText (RDLDefaultTexts.Message_ManualDownload);
				}
#else
			linkButtons[0].Enabled = !string.IsNullOrWhiteSpace (versionDescription);
#endif
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
