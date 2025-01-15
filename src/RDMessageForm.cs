using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Типы сообщений для пользователя
	/// </summary>
	public enum RDMessageTypes
		{
		/// <summary>
		/// Информационное (обычное) с текстом слева
		/// </summary>
		Information_Left = 0,

		/// <summary>
		/// Вопрос к пользователю с текстом слева
		/// </summary>
		Question_Left = 1,

		/// <summary>
		/// Предупреждение с текстом слева
		/// </summary>
		Warning_Left = 2,

		/// <summary>
		/// Ошибка с текстом слева
		/// </summary>
		Error_Left = 3,

		/// <summary>
		/// Сообщение об успешном результате с текстом слева
		/// </summary>
		Success_Left = 4,

		/// <summary>
		/// Информационное (обычное) с текстом по центру
		/// </summary>
		Information_Center = 10,

		/// <summary>
		/// Вопрос к пользователю с текстом по центру
		/// </summary>
		Question_Center = 11,

		/// <summary>
		/// Предупреждение с текстом по центру
		/// </summary>
		Warning_Center = 12,

		/// <summary>
		/// Ошибка с текстом по центру
		/// </summary>
		Error_Center = 13,

		/// <summary>
		/// Сообщение об успешном результате с текстом по центру
		/// </summary>
		Success_Center = 14,

		/// <summary>
		/// Сообщение об успешном результате с текстом по центру без звука
		/// </summary>
		Clipboard_Center = 15,
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
		/// <summary>
		/// Типы сообщений для пользователя
		/// </summary>
		private enum RDMessageInternalTypes
			{
			/// <summary>
			/// Информационное (обычное) с текстом слева
			/// </summary>
			Information_Left = 0,

			/// <summary>
			/// Вопрос к пользователю с текстом слева
			/// </summary>
			Question_Left = 1,

			/// <summary>
			/// Предупреждение с текстом слева
			/// </summary>
			Warning_Left = 2,

			/// <summary>
			/// Ошибка с текстом слева
			/// </summary>
			Error_Left = 3,

			/// <summary>
			/// Сообщение об успешном результате с текстом слева
			/// </summary>
			Success_Left = 4,

			/// <summary>
			/// Окно ввода с текстом слева
			/// </summary>
			Input_Left = 20,

			/// <summary>
			/// Информационное (обычное) с текстом по центру
			/// </summary>
			Information_Center = 10,

			/// <summary>
			/// Вопрос к пользователю с текстом по центру
			/// </summary>
			Question_Center = 11,

			/// <summary>
			/// Предупреждение с текстом по центру
			/// </summary>
			Warning_Center = 12,

			/// <summary>
			/// Ошибка с текстом по центру
			/// </summary>
			Error_Center = 13,

			/// <summary>
			/// Сообщение об успешном результате с текстом по центру
			/// </summary>
			Success_Center = 14,

			/// <summary>
			/// Сообщение об успешном результате с текстом по центру без звука
			/// </summary>
			Clipboard_Center = 15,

			/// <summary>
			/// Окно ввода с текстом по центру
			/// </summary>
			Input_Center = 30,

			/// <summary>
			/// Окно выбора языка интерфейса
			/// </summary>
			LanguageSelector = 40,
			}

		// Прочее
		private bool exitAllowed = false;
		private RDMessageInternalTypes windowType = RDMessageInternalTypes.Information_Left;

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
			RDMessageFormInit (Center ? RDMessageInternalTypes.Input_Center : RDMessageInternalTypes.Input_Left,
				Message,
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK),
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
				InitialText, RDLanguages.en_us, length);
			}

		/// <summary>
		/// Конструктор. Запускает простейшую форму сообщения для пользователя (с таймаутом автозакрытия)
		/// </summary>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		/// <param name="Timeout">Таймаут (в миллисекундах), по истечении которого
		/// сообщение автоматически закроется (от 100 до 60000 мс)</param>
		public RDMessageForm (RDMessageTypes Type, string Message, uint Timeout)
			{
			uint to = Timeout;
			if (to < 100)
				to = 100;
			if (to > 60000)
				to = 60000;

			RDMessageFormInit ((RDMessageInternalTypes)Type, Message, "-", null, null, RDLanguages.en_us, to);
			}

		/// <summary>
		/// Конструктор. Запускает форму выбора языка приложения
		/// </summary>
		/// <param name="CurrentInterfaceLanguage">Текущий язык интерфейса</param>
		public RDMessageForm (RDLanguages CurrentInterfaceLanguage)
			{
			RDMessageFormInit (RDMessageInternalTypes.LanguageSelector,
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
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message)
			{
			RDMessageFormInit ((RDMessageInternalTypes)Type, Message,
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK), null, null,
				RDLanguages.en_us, 0);
			}

		/*/// <summary>
		/// Конструктор. Запускает форму сообщения для пользователя с одной кнопкой
		/// </summary>
		/// <param name="ButtonOneName">Название кнопки</param>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message, string ButtonOneName)
			{
			RDMessageFormInit ((RDMessageInternalTypes)Type, Message,
				ButtonOneName, null, null, RDLanguages.en_us, 0);
			}*/

		/*/// <summary>
		/// Конструктор. Запускает форму сообщения для пользователя с двумя кнопками
		/// </summary>
		/// <param name="ButtonOneName">Название первой кнопки</param>
		/// <param name="ButtonTwoName">Название второй кнопки</param>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message, string ButtonOneName, string ButtonTwoName)
			{
			RDMessageFormInit ((RDMessageInternalTypes)Type, Message,
				ButtonOneName, ButtonTwoName, null, RDLanguages.en_us, 0);
			}*/

		/// <summary>
		/// Конструктор. Запускает форму сообщения для пользователя с тремя кнопками
		/// </summary>
		/// <param name="ButtonOneName">Название первой кнопки</param>
		/// <param name="ButtonTwoName">Название второй кнопки</param>
		/// <param name="ButtonThreeName">Название третьей кнопки</param>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message, string ButtonOneName, string ButtonTwoName,
			string ButtonThreeName)
			{
			RDMessageFormInit ((RDMessageInternalTypes)Type, Message,
				ButtonOneName, ButtonTwoName, ButtonThreeName, RDLanguages.en_us, 0);
			}

		// Основная инициализация формы
		private void RDMessageFormInit (RDMessageInternalTypes Type, string Message,
			string ButtonOneName, string ButtonTwoName, string ButtonThreeName,
			RDLanguages CurrentInterfaceLanguage, uint Timeout)
			{
			// Инициализация
			InitializeComponent ();

			// Запрет на побочное определение (не через собственный конструктор)
			windowType = Type;
			switch (windowType)
				{
				case RDMessageInternalTypes.LanguageSelector:
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
					break;

				case RDMessageInternalTypes.Input_Left:
				case RDMessageInternalTypes.Input_Center:
					InputTextBox.Visible = true;
					InputTextBox.MaxLength = (int)Timeout;
					break;
				}

			this.Text = ProgramDescription.AssemblyTitle;

			// Обработка текста
			if (!string.IsNullOrWhiteSpace (Message))
				{
				Label01.Text = Message.Replace ("\n", RDLocale.RN).Replace ("\r\r", "\r");
				Label01.SelectionLength = 0;

				switch (windowType)
					{
					case RDMessageInternalTypes.Information_Center:
					case RDMessageInternalTypes.Question_Center:
					case RDMessageInternalTypes.Success_Center:
					case RDMessageInternalTypes.Warning_Center:
					case RDMessageInternalTypes.Error_Center:
					case RDMessageInternalTypes.Input_Center:
					case RDMessageInternalTypes.Clipboard_Center:
						Label01.TextAlign = HorizontalAlignment.Center;
						break;
					}

				switch (windowType)
					{
					// Не требуют пересчёта размера
					case RDMessageInternalTypes.LanguageSelector:
					case RDMessageInternalTypes.Input_Center:
					case RDMessageInternalTypes.Input_Left:
						break;

					default:
						// Определение ширины
						if ((Message.Replace ("\n", "").Replace ("\r", "").Length > 150) &&
							(TextRenderer.MeasureText (Message, Label01.Font).Width > Label01.Width))
							{
							Label01.Width += this.Width;
							this.Width *= 2;
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
						break;
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
					switch (windowType)
						{
						case RDMessageInternalTypes.Input_Center:
						case RDMessageInternalTypes.Input_Left:
							InputTextBox.Text = ButtonThreeName;
							break;

						default:
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
							break;
						}
					}
				}

			// Системный звук и фоновый цвет
			switch (windowType)
				{
				case RDMessageInternalTypes.Information_Left:
				case RDMessageInternalTypes.Information_Center:
				default:
					SystemSounds.Asterisk.Play ();
					this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.LightGrey);
					break;

				case RDMessageInternalTypes.LanguageSelector:
				case RDMessageInternalTypes.Input_Center:
				case RDMessageInternalTypes.Input_Left:
					this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.LightGrey);
					break;

				case RDMessageInternalTypes.Question_Left:
				case RDMessageInternalTypes.Question_Center:
					SystemSounds.Question.Play ();
					this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.QuestionMessage);
					break;

				case RDMessageInternalTypes.Warning_Left:
				case RDMessageInternalTypes.Warning_Center:
					SystemSounds.Exclamation.Play ();
					this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.WarningMessage);
					break;

				case RDMessageInternalTypes.Error_Left:
				case RDMessageInternalTypes.Error_Center:
					SystemSounds.Hand.Play ();
					this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.ErrorMessage);
					break;

				case RDMessageInternalTypes.Success_Left:
				case RDMessageInternalTypes.Success_Center:
					SystemSounds.Asterisk.Play ();
					this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.SuccessMessage);
					break;

				case RDMessageInternalTypes.Clipboard_Center:
					this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.SuccessMessage);
					break;
				}

			// Окончательное выравнивание элементов, применение цветовой схемы
			AlignButtons ();
			Label01.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
			Label01.BackColor = this.BackColor;

			switch (windowType)
				{
				case RDMessageInternalTypes.LanguageSelector:
					LanguagesCombo.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
					LanguagesCombo.BackColor = this.BackColor;
					break;

				case RDMessageInternalTypes.Input_Left:
				case RDMessageInternalTypes.Input_Center:
					InputTextBox.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
					InputTextBox.BackColor = this.BackColor;
					break;

				default:
					// У двух полей ввода через таймаут передаётся ограничение длины текста
					if (Timeout > 0)
						MainTimer.Interval = (int)Timeout;
					break;
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
				switch (windowType)
					{
					case RDMessageInternalTypes.LanguageSelector:
						RDLocale.CurrentLanguage = (RDLanguages)LanguagesCombo.SelectedIndex;
						break;

					case RDMessageInternalTypes.Input_Center:
					case RDMessageInternalTypes.Input_Left:
						enteredText = InputTextBox.Text;
						break;
					}
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
			int w = DialogForm.Width;
			int h = DialogForm.Height;

			if (HeaderHeight > 0)
				{
				bre = new SolidBrush (RDInterface.GetInterfaceColor (RDInterfaceColors.LightEmerald));
				gr.FillRectangle (bre, 0, 0, w, HeaderHeight);
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
		}
	}
