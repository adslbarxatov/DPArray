using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Флаги сообщений для пользователя
	/// </summary>
	public enum RDMessageFlags
		{
		/// <summary>
		/// Информационное (обычное) сообщение (по умолчанию)
		/// </summary>
		Information = 0x0000,

		/// <summary>
		/// Сообщение с вопросом к пользователю
		/// </summary>
		Question = 0x0001,

		/// <summary>
		/// Сообщение-предупреждение
		/// </summary>
		Warning = 0x0002,

		/// <summary>
		/// Сообщение об ошибке
		/// </summary>
		Error = 0x0004,

		/// <summary>
		/// Сообщение об успешном результате
		/// </summary>
		Success = 0x0008,

		/// <summary>
		/// Текст по левой стороне (по умолчанию)
		/// </summary>
		LeftText = 0x0000,

		/// <summary>
		/// Текст по центру
		/// </summary>
		CenterText = 0x0010,

		/// <summary>
		/// Сообщение фиксированной малой ширины
		/// </summary>
		LockSmallSize = 0x0020,

		/// <summary>
		/// Сообщение без звука
		/// </summary>
		NoSound = 0x0040,
		}

	/// <summary>
	/// Возможные результирующие кнопки сообщения для пользователя
	/// </summary>
	public enum RDMessageButtons
		{
		/// <summary>
		/// Кнопка не была нажата (окно закрыто иным способом)
		/// </summary>
		NotSelected = -1,

		/// <summary>
		/// Первая кнопка
		/// </summary>
		ButtonOne = 1,

		/// <summary>
		/// Вторая кнопка
		/// </summary>
		ButtonTwo = 2,

		/// <summary>
		/// Третья кнопка
		/// </summary>
		ButtonThree = 3
		}

	/// <summary>
	/// Форма является универсальным интерфейсом сообщений для пользователя
	/// </summary>
	public partial class RDMessageForm: Form
		{
		// Прочее
		private bool exitAllowed = false;
		private bool inputBox = false;
		private bool languageSelector = false;
		private RDTaskbarModes tbMode;

		/// <summary>
		/// Возвращает выбранную кнопку в сообщении
		/// </summary>
		public RDMessageButtons ResultButton
			{
			get
				{
				return resultButton;
				}
			}
		private RDMessageButtons resultButton = RDMessageButtons.NotSelected;

		/// <summary>
		/// Возвращает введённый пользователем текст
		/// </summary>
		public string EnteredText
			{
			get
				{
				return enteredText;
				}
			}
		private string enteredText = "";

		/// <summary>
		/// Конструктор. Запускает простейшую форму ввода текста для пользователя
		/// </summary>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="MaxLength">Максимальная длина вводимого текста</param>
		/// <param name="Center">Флаг, указывающий на расположение сообщения по центру</param>
		/// <param name="InitialText">Начальный текст, отображаемый в поле ввода</param>
		public RDMessageForm (string Message, bool Center, uint MaxLength, string InitialText)
			{
			uint length = MaxLength;
			if (length < 1)
				length = 1;
			if (length > 256)
				length = 256;

			// Начальный текст передаётся как текст третьей кнопки
			inputBox = true;
			RDMessageFormInit (Center ? RDMessageFlags.CenterText : RDMessageFlags.LeftText,
				Message,
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK),
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
				InitialText, RDLanguages.en_us, length);
			}

		/// <summary>
		/// Конструктор. Запускает простейшую форму сообщения для пользователя (с таймаутом автозакрытия)
		/// </summary>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Flags">Параметры создаваемого окна</param>
		/// <param name="Timeout">Таймаут (в миллисекундах), по истечении которого
		/// сообщение автоматически закроется (от 100 до 60000 мс)</param>
		public RDMessageForm (RDMessageFlags Flags, string Message, uint Timeout)
			{
			uint to = Timeout;
			if (to < 100)
				to = 100;
			if (to > 60000)
				to = 60000;

			RDMessageFormInit (Flags, Message, "-", null, null, RDLanguages.en_us, to);
			}

		/// <summary>
		/// Конструктор. Запускает форму выбора языка приложения
		/// </summary>
		/// <param name="CurrentInterfaceLanguage">Текущий язык интерфейса</param>
		public RDMessageForm (RDLanguages CurrentInterfaceLanguage)
			{
			languageSelector = true;
			RDMessageFormInit (RDMessageFlags.LeftText,
				string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_LanguageSelection_Fmt),
				RDLocale.LanguagesNames[(int)RDLocale.CurrentLanguage]),

				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Apply),
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),

				null, CurrentInterfaceLanguage, 0);
			}

		/// <summary>
		/// Конструктор. Запускает простейшую форму сообщения для пользователя (с одной кнопкой подтверждения)
		/// </summary>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Flags">Параметры создаваемого окна</param>
		public RDMessageForm (RDMessageFlags Flags, string Message)
			{
			RDMessageFormInit (Flags, Message,
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK), null, null,
				RDLanguages.en_us, 0);
			}

		/// <summary>
		/// Конструктор. Запускает форму сообщения для пользователя с тремя кнопками
		/// </summary>
		/// <param name="ButtonOneName">Название первой кнопки</param>
		/// <param name="ButtonTwoName">Название второй кнопки</param>
		/// <param name="ButtonThreeName">Название третьей кнопки</param>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Flags">Параметры создаваемого окна</param>
		public RDMessageForm (RDMessageFlags Flags, string Message, string ButtonOneName, string ButtonTwoName,
			string ButtonThreeName)
			{
			RDMessageFormInit (Flags, Message,
				ButtonOneName, ButtonTwoName, ButtonThreeName, RDLanguages.en_us, 0);
			}

		// Основная инициализация формы
		private void RDMessageFormInit (RDMessageFlags Flags, string Message,
			string ButtonOneName, string ButtonTwoName, string ButtonThreeName,
			RDLanguages CurrentInterfaceLanguage, uint Timeout)
			{
			// Инициализация
			InitializeComponent ();

			// Запрет на побочное определение (не через собственный конструктор)
			if (languageSelector)
				{
				LanguagesCombo.Visible = true;
				LanguagesCombo.Items.AddRange (RDLocale.LanguagesNames);
				try
					{
					LanguagesCombo.SelectedIndex = (int)CurrentInterfaceLanguage;
					}
				catch
					{
					LanguagesCombo.SelectedIndex = 0;
					}
				}
			else if (inputBox)
				{
				InputTextBox.Visible = true;
				InputTextBox.MaxLength = (int)Timeout;

				if (InputTextBox.MaxLength > 30)
					{
					Label01.Width += this.Width;
					InputTextBox.Width += this.Width;
					this.Width *= 2;
					}
				}

			this.Text = ProgramDescription.AssemblyTitle;

			// Обработка текста и подгонка ширины окна
			if (!string.IsNullOrWhiteSpace (Message))
				{
				Label01.Text = Message.Replace ("\n", RDLocale.RN).Replace ("\r\r", "\r");
				Label01.SelectionLength = 0;

				if (Flags.HasFlag (RDMessageFlags.CenterText))
					Label01.TextAlign = HorizontalAlignment.Center;

				if (!inputBox && !languageSelector)
					{
					// Определение ширины
					if ((Message.Replace ("\n", "").Replace ("\r", "").Length > 150) &&
						(TextRenderer.MeasureText (Message, Label01.Font).Width > Label01.Width))
						{
						if (!Flags.HasFlag (RDMessageFlags.LockSmallSize))
							{
							Label01.Width += this.Width;
							this.Width *= 2;
							}
						}

					// Определение высоты
					Label01.Height = (Label01.GetLineFromCharIndex (int.MaxValue) + 2) *
						TextRenderer.MeasureText ("X", Label01.Font).Height;

					this.Height = Label01.Height;
					if (Timeout > 0)
						{
						this.Height += 17;
						Label01.Top = (this.Height - Label01.Height) / 2 + 8;
						}
					else
						{
						this.Height += 57;
						Button01.Top = Button02.Top = Button03.Top = this.Height - 48;
						}
					}
				}

			// Настройка кнопок
			if (!string.IsNullOrWhiteSpace (ButtonOneName))
				{
				Button01.Text = ButtonOneName;
				if (Button01.Text.EndsWith (RDLocale.TabStopPreventor))
					{
					Button01.TabStop = false;
					Button01.Text = Button01.Text.Replace (RDLocale.TabStopPreventor, "");
					}
				else
					{
					AcceptButton = Button01;
					}
				}
			else
				{
				Button01.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK);
				AcceptButton = Button01;
				}

			if (!string.IsNullOrWhiteSpace (ButtonTwoName))
				{
				Button02.Text = ButtonTwoName;
				if (Button02.Text.EndsWith (RDLocale.TabStopPreventor))
					{
					Button02.TabStop = false;
					Button02.Text = Button02.Text.Replace (RDLocale.TabStopPreventor, "");
					}
				else
					{
					CancelButton = Button02;
					}

				if (!string.IsNullOrWhiteSpace (ButtonThreeName))
					{
					if (inputBox)
						{
						InputTextBox.Text = ButtonThreeName;
						}
					else
						{
						Button03.Text = ButtonThreeName;
						if (Button03.Text.EndsWith (RDLocale.TabStopPreventor))
							{
							Button03.TabStop = false;
							Button03.Text = Button03.Text.Replace (RDLocale.TabStopPreventor, "");
							}
						else
							{
							CancelButton = Button03;
							}
						}
					}
				}

			// Системный звук и фоновый цвет
			bool sound = !Flags.HasFlag (RDMessageFlags.NoSound);
			if (Flags.HasFlag (RDMessageFlags.Success))
				{
				if (sound)
					SystemSounds.Asterisk.Play ();

				this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.SuccessMessage);
				tbMode = RDTaskbarModes.Regular;
				}
			else if (Flags.HasFlag (RDMessageFlags.Error))
				{
				if (sound)
					SystemSounds.Hand.Play ();

				this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.ErrorMessage);
				tbMode = RDTaskbarModes.Error;
				}
			else if (Flags.HasFlag (RDMessageFlags.Warning))
				{
				if (sound)
					SystemSounds.Exclamation.Play ();

				this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.WarningMessage);
				tbMode = RDTaskbarModes.Warning;
				}
			else if (Flags.HasFlag (RDMessageFlags.Question))
				{
				if (sound)
					SystemSounds.Question.Play ();

				this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.QuestionMessage);
				tbMode = RDTaskbarModes.Indeterminate;
				}
			else
				{
				if (sound && !languageSelector && !inputBox)
					SystemSounds.Asterisk.Play ();

				this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.LightGrey);
				tbMode = (RDTaskbarModes)(-1);
				}

			// Окончательное выравнивание элементов, применение цветовой схемы
			AlignButtons ();
			Label01.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
			Label01.BackColor = this.BackColor;

			if (languageSelector)
				{
				LanguagesCombo.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
				LanguagesCombo.BackColor = this.BackColor;
				}
			else if (inputBox)
				{
				InputTextBox.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
				InputTextBox.BackColor = this.BackColor;
				}
			else
				{
				if (Timeout > 0)
					{
					MainTimer.Interval = (int)Timeout;

					// Досрочное закрытие
					Label01.Click += MainTimer_Tick;
					this.Click += MainTimer_Tick;
					Label01.KeyDown += MainTimer_Tick;
					this.KeyDown += MainTimer_Tick;
					}
				}

			// Запуск
			this.StartPosition = FormStartPosition.CenterParent;
			this.ShowDialog ();
			}

		private void RDMessageForm_Load (object sender, EventArgs e)
			{
			// Запуск отрисовки
			CreateBackground (this);
			}

		private void RDMessageForm_Shown (object sender, EventArgs e)
			{
			// Отмена центрирования на родительское окно, если это невозможно
			if (this.Left == 0)
				this.CenterToScreen ();

			// Принудительный фокус на поле ввода, если применимо
			if (InputTextBox.Visible)
				InputTextBox.Focus ();

			// Запуск таймера, если предусмотрен
			if (MainTimer.Interval > 75)
				MainTimer.Enabled = true;

			// Индикация на панели задач (если предусмотрена)
			if (tbMode >= 0)
				RDInterface.SetTaskBarIndication (this, tbMode);
			}

		// Выбор размера
		private void Button01_Click (object sender, EventArgs e)
			{
			// Извлечение номера
			string buttonName = ((Button)sender).Name;
			uint buttonNumber = uint.Parse (buttonName.Substring (buttonName.Length - 2, 2));
			resultButton = (RDMessageButtons)buttonNumber;

			// Возврат
			if (resultButton == RDMessageButtons.ButtonOne)
				{
				if (languageSelector)
					RDLocale.CurrentLanguage = (RDLanguages)LanguagesCombo.SelectedIndex;
				else if (inputBox)
					enteredText = InputTextBox.Text;
				}

			// Завершение
			exitAllowed = true;
			this.Close ();
			}

		// Выравнивание кнопок
		private void AlignButtons ()
			{
			// Настройка
			if (Button01.Text != "-")
				{
				Button01.Enabled = Button01.Visible = true;
				Button01.BackColor = this.BackColor;
				Button01.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
				}
			if (Button01.Enabled && (Button02.Text != "-"))
				{
				Button02.Enabled = Button02.Visible = true;
				Button02.BackColor = this.BackColor;
				Button02.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
				}
			if (Button02.Enabled && (Button03.Text != "-"))
				{
				Button03.Enabled = Button03.Visible = true;
				Button03.BackColor = this.BackColor;
				Button03.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
				}

			// Выравнивание
			if (!Button02.Enabled)
				{
				Button01.Left = (this.Width - Button01.Width) / 2;
				return;
				}

			if (!Button03.Enabled)
				{
				Button01.Left = this.Width / 2 - Button01.Width - 3;
				Button02.Left = this.Width / 2 + 3;
				return;
				}

			Button02.Left = (this.Width - Button02.Width) / 2;
			Button01.Left = Button02.Left - Button01.Width - 6;
			Button03.Left = Button02.Left + Button02.Width + 6;
			}

		// Запрет на закрытие окна "крестиком"
		private void RDMessageForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			e.Cancel = !exitAllowed;
			}

		/// <summary>
		/// Метод формирует фон диалогового окна для указанной формы
		/// </summary>
		/// <param name="DialogForm">Форма для создаия фона</param>
		public static void CreateBackground (Form DialogForm)
			{
			CreateBackground (DialogForm, 0);
			}

		/// <summary>
		/// Метод формирует фон диалогового окна для указанной формы
		/// </summary>
		/// <param name="DialogForm">Форма для создаия фона</param>
		/// <param name="HeaderHeight">Высота от верхней части диалогового окна,
		/// окрашиваемая в контрастный цвет</param>
		public static void CreateBackground (Form DialogForm, uint HeaderHeight)
			{
			// Начало
			const int roundingSize = 20;
			Bitmap bm = new Bitmap (DialogForm.Width, DialogForm.Height);
			Graphics gr = Graphics.FromImage (bm);

			// Отрисовка фона
			SolidBrush bre = null;
			SolidBrush brk = new SolidBrush (DialogForm.TransparencyKey);
			SolidBrush brb = new SolidBrush (DialogForm.BackColor);
			SolidBrush bra = null;
			int w = DialogForm.Width;
			int h = DialogForm.Height;

			if (HeaderHeight > 0)
				{
				bre = new SolidBrush (RDInterface.GetInterfaceColor (RDInterfaceColors.LightEmerald));
				gr.FillRectangle (bre, 0, 0, w, HeaderHeight);

				bra = new SolidBrush (RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText));
				gr.FillRectangle (bra, 0, HeaderHeight - 1, w, 2);
				bra.Dispose ();
				}

			gr.FillRectangle (brk, 0, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (brk, w - roundingSize / 2, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (brk, 0, h - roundingSize / 2, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (brk, w - roundingSize / 2, h - roundingSize / 2, roundingSize / 2, roundingSize / 2);
			brk.Dispose ();

			if (HeaderHeight > 0)
				{
				gr.FillEllipse (bre, 0, 0, roundingSize, roundingSize);
				gr.FillEllipse (bre, w - roundingSize - 1, 0, roundingSize, roundingSize);
				bre.Dispose ();
				}
			else
				{
				gr.FillEllipse (brb, 0, 0, roundingSize, roundingSize);
				gr.FillEllipse (brb, w - roundingSize - 1, 0, roundingSize, roundingSize);
				}
			gr.FillEllipse (brb, 0, h - roundingSize - 1, roundingSize, roundingSize);
			gr.FillEllipse (brb, w - roundingSize - 1, h - roundingSize - 1,
				roundingSize, roundingSize);
			brb.Dispose ();

			// Отрисовка контура
			Pen pDark = new Pen (Color.FromArgb (2 * DialogForm.BackColor.R / 3, 2 * DialogForm.BackColor.G / 3,
				2 * DialogForm.BackColor.B / 3), 1);
			w = DialogForm.Width - (int)pDark.Width;
			h = DialogForm.Height - (int)pDark.Width;

			gr.DrawLine (pDark, roundingSize / 2, 0, w - roundingSize / 2, 0);
			gr.DrawArc (pDark, w - roundingSize, 0, roundingSize, roundingSize, 0, -90);
			gr.DrawLine (pDark, w, roundingSize / 2, w, h - roundingSize / 2);
			gr.DrawArc (pDark, w - roundingSize, h - roundingSize, roundingSize, roundingSize, 0, 90);

			gr.DrawLine (pDark, roundingSize / 2, h, w - roundingSize / 2, h);
			gr.DrawArc (pDark, 0, h - roundingSize, roundingSize, roundingSize, 180, -90);
			gr.DrawLine (pDark, 0, roundingSize / 2, 0, h - roundingSize / 2);
			gr.DrawArc (pDark, 0, 0, roundingSize, roundingSize, 180, 90);

			// Финализация
			gr.Dispose ();
			pDark.Dispose ();
			DialogForm.BackgroundImage = bm;
			}

		// Автоматическое закрытие по таймеру
		private void MainTimer_Tick (object sender, EventArgs e)
			{
			resultButton = RDMessageButtons.ButtonOne;
			exitAllowed = true;
			this.Close ();
			}

		// Подавление дефекта интерфейса с выделением текста в Label01 при попадании фокуса
		protected override bool ProcessCmdKey (ref Message msg, Keys keyData)
			{
			if (InputTextBox.Focused)
				return base.ProcessCmdKey (ref msg, keyData);

			switch (keyData)
				{
				case Keys.Up:
				case Keys.Left:
					this.SelectNextControl (this.ActiveControl, false, true, false, true);
					return true;

				case Keys.Down:
				case Keys.Right:
					this.SelectNextControl (this.ActiveControl, true, true, false, true);
					return true;
				}

			return base.ProcessCmdKey (ref msg, keyData);
			}
		}
	}
