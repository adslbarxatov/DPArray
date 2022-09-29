using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Форма обеспечивает возможность изменения языка интерфейса
	/// </summary>
	public partial class LanguageForm: Form
		{
		// Цветовая схема
		private Color backColor = Color.FromArgb (224, 224, 224);
		private Color foreColor = Color.FromArgb (32, 32, 32);

		/// <summary>
		/// Конструктор. Запускает форму выбора размера
		/// </summary>
		/// <param name="CurrentInterfaceLanguage">Текущий язык интерфейса</param>
		public LanguageForm (SupportedLanguages CurrentInterfaceLanguage)
			{
			// Инициализация
			InitializeComponent ();

			LanguagesCombo.Items.AddRange (Localization.LanguagesNames);
			try
				{
				LanguagesCombo.SelectedIndex = (int)CurrentInterfaceLanguage;
				}
			catch
				{
				LanguagesCombo.SelectedIndex = 0;
				}

			this.Text = ProgramDescription.AssemblyTitle;
			Label01.Text = string.Format (Localization.GetText ("LanguageSelectorMessage", CurrentInterfaceLanguage),
				LanguagesCombo.Text);
			OKButton.Text = Localization.GetText ("NextButtonText", CurrentInterfaceLanguage);
			AbortButton.Text = Localization.GetText ("AbortButtonText", CurrentInterfaceLanguage);

			this.BackColor = backColor;
			Label01.ForeColor = LanguagesCombo.ForeColor = OKButton.ForeColor = AbortButton.ForeColor = foreColor;
			LanguagesCombo.BackColor = OKButton.BackColor = AbortButton.BackColor = backColor;

			// Запуск отрисовки
			const int roundingSize = 20;
			Bitmap bm = new Bitmap (this.Width, this.Height);
			Graphics gr = Graphics.FromImage (bm);

			SolidBrush br = new SolidBrush (this.TransparencyKey);
			gr.FillRectangle (br, 0, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, this.Width - roundingSize / 2, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, 0, this.Height - roundingSize / 2, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, this.Width - roundingSize / 2, this.Height - roundingSize / 2, 
				roundingSize / 2, roundingSize / 2);
			br.Dispose ();

			br = new SolidBrush (this.BackColor);
			gr.FillEllipse (br, 0, 0, roundingSize, roundingSize);
			gr.FillEllipse (br, this.Width - roundingSize - 1, 0, roundingSize, roundingSize);
			gr.FillEllipse (br, 0, this.Height - roundingSize - 1, roundingSize, roundingSize);
			gr.FillEllipse (br, this.Width - roundingSize - 1, this.Height - roundingSize - 1, 
				roundingSize, roundingSize);
			br.Dispose ();
			gr.Dispose ();

			this.BackgroundImage = bm;

			// Запуск
			this.ShowDialog ();
			}

		// Выбор размера
		private void BOK_Click (object sender, EventArgs e)
			{
			Localization.CurrentLanguage = (SupportedLanguages)LanguagesCombo.SelectedIndex;
			this.Close ();
			}

		// Отмена
		private void BCancel_Click (object sender, EventArgs e)
			{
			this.Close ();
			}
		}
	}
