using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс предоставляет интерфейс визуализации прогресса установки/удаления программы
	/// </summary>
	public partial class RDWorkerForm: Form
		{
		// Запрет выхода из формы до окончания работы
		private bool allowClose = false;

		// Объекты-отрисовщики
		private Bitmap progress, frameGreenGrey, frameBack, frameDark;
		private Graphics g, gp;
		private int currentXOffset = 0, oldPercentage = 0, newPercentage = 0;

		// Параметры инициализации потока
		private object parameters;

		// Флаг принудительного размещения поверх всех окон
		private bool alwaysOnTop = false;

		/// <summary>
		/// Длина шкалы прогресса
		/// </summary>
		public const uint ProgressBarSize = 1000;

		/// <summary>
		/// Возвращает объект-обвязку исполняемого процесса
		/// </summary>
		public BackgroundWorker Worker
			{
			get
				{
				return bw;
				}
			}
		private BackgroundWorker bw = new BackgroundWorker ();

		/// <summary>
		/// Возвращает результат операции в виде строки
		/// </summary>
		public string WorkResultAsString
			{
			get
				{
				return exResult;
				}
			}
		private string exResult = "";

		// Инициализация ProgressBar
		private void InitializeProgressBar ()
			{
			// Настройка контролов
			InitializeComponent ();
			this.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.LightGrey);
			StateLabel.ForeColor = AbortButton.ForeColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
			AbortButton.BackColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultBackground);

			// Инициализация
			progress = new Bitmap (this.Width - 20, 30);
			g = Graphics.FromHwnd (this.Handle);
			gp = Graphics.FromImage (progress);

			// Формирование стрелок
			Point[] frame = [
				new Point (0, 0),
				new Point (this.Width / 4, 0),
				new Point (this.Width / 4 + progress.Height / 2, progress.Height / 2),
				new Point (this.Width / 4, progress.Height),
				new Point (0, progress.Height),
				new Point (progress.Height / 2, progress.Height / 2),
				];

			// Подготовка дескрипторов
			SolidBrush green = new SolidBrush (RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultEmerald)),
				grey = new SolidBrush (RDInterface.GetInterfaceColor (RDInterfaceColors.MediumGrey)),
				back = new SolidBrush (RDInterface.GetInterfaceColor (RDInterfaceColors.LightGrey)),
				dark = new SolidBrush (RDInterface.GetInterfaceColor (RDInterfaceColors.DarkGrey));

			frameGreenGrey = new Bitmap (10 * this.Width / 4, progress.Height);
			frameBack = new Bitmap (10 * this.Width / 4, progress.Height);
			frameDark = new Bitmap (10 * this.Width / 4, progress.Height);

			Graphics g1 = Graphics.FromImage (frameGreenGrey),
				g2 = Graphics.FromImage (frameBack),
				g3 = Graphics.FromImage (frameDark);

			// Сборка
			for (int i = 0; i < 8; i++)
				{
				for (int j = 0; j < frame.Length; j++)
					frame[j].X += this.Width / 4;

				g1.FillPolygon ((i % 2 == 0) ? green : grey, frame);
				g2.FillPolygon (back, frame);
				g3.FillPolygon (dark, frame);
				}

			// Объём
			for (int i = 0; i < frameGreenGrey.Height; i++)
				{
				Pen p = new Pen (Color.FromArgb (200 - (int)(185.0 * Math.Sin (Math.PI * (double)i /
					(double)frameGreenGrey.Height)), this.BackColor));
				g1.DrawLine (p, 0, i, frameGreenGrey.Width, i);
				g3.DrawLine (p, this.Width / 4 + frameGreenGrey.Height / 2 - Math.Abs (frameGreenGrey.Height / 2 - i),
					i, frameGreenGrey.Width, i);
				p.Dispose ();
				}

			// Освобождение ресурсов
			g1.Dispose ();
			g2.Dispose ();
			g3.Dispose ();

			green.Dispose ();
			grey.Dispose ();
			back.Dispose ();
			dark.Dispose ();

			// Запуск таймера
			DrawingTimer.Interval = 1;
			DrawingTimer.Enabled = true;
			}

#if DPMODULE

		/// <summary>
		/// Конструктор. Выполняет настройку и запуск процесса установки / удаления
		/// </summary>
		/// <param name="HardWorkProcess">Процесс, выполняющий установку / удаление</param>
		/// <param name="SetupPath">Путь установки/удаления</param>
		/// <param name="Flags">Флаги процедуры: b0 = режим удаления;
		///										 b1 = разрешено завершение работающих процессов;</param>
		///										 b2 = разрешён запуск приложения по завершении
		/// <param name="PackagePath">Путь к пакету развёртки</param>
		public RDWorkerForm (DoWorkEventHandler HardWorkProcess, string SetupPath, string PackagePath, uint Flags)
			{
			// Инициализация
			string[] arguments = [SetupPath, PackagePath, Flags.ToString ()];
			RDWorkerForm_Init (HardWorkProcess, arguments, " ", false, true);
			}

#endif

		/// <summary>
		/// Конструктор. Выполняет указанное действие с указанными параметрами
		/// </summary>
		/// <param name="HardWorkProcess">Выполняемый процесс</param>
		/// <param name="Parameters">Параметры, передаваемые в процесс; может быть null</param>
		/// <param name="WindowCaption">Строка, отображаемая при инициализации окна прогресса</param>
		/// <param name="Flags">Параметры работы процесса</param>
		public RDWorkerForm (DoWorkEventHandler HardWorkProcess, object Parameters, string WindowCaption,
			RDRunWorkFlags Flags)
			{
#if DPMODULE
			alwaysOnTop = (Flags & RDRunWorkFlags.AlwaysOnTop) != 0;
#endif
			bool middle = (Flags & RDRunWorkFlags.CaptionInTheMiddle) != 0;
			bool abort = (Flags & RDRunWorkFlags.AllowOperationAbort) != 0;

			string caption = "";
			if ((Flags & RDRunWorkFlags.DontSuspendExecution) == 0)
				{
				if (string.IsNullOrWhiteSpace (WindowCaption))
					caption = " ";
				else
					caption = WindowCaption;
				}
			else
				{
				abort = false;
				}

			RDWorkerForm_Init (HardWorkProcess, Parameters, caption, middle, abort);
			}

		/// <summary>
		/// Конструктор. Выполняет загрузку файла по URL
		/// </summary>
		/// <param name="HardWorkProcess">Выполняемый процесс</param>
		/// <param name="Length">Размер пакета</param>
		/// <param name="TargetPath">Путь создаваемого файла</param>
		/// <param name="URL">Ссылка для загрузки</param>
		/// <param name="PackagesList">Флаг, указывающий на загрузку списка пакетов</param>
		public RDWorkerForm (DoWorkEventHandler HardWorkProcess, string URL, string TargetPath,
			string Length, bool PackagesList)
			{
			// Инициализация
			string[] arguments = [URL, TargetPath, Length, PackagesList ? "1" : "0"];
			RDWorkerForm_Init (HardWorkProcess, arguments, " ", true, true);
			}

		// Общий метод подготовки исполнителя заданий
		private void RDWorkerForm_Init (DoWorkEventHandler HWProcess, object Parameters,
			string Caption, bool CaptionInTheCenter, bool AllowAbort)
			{
			// Настройка BackgroundWorker
			bw.WorkerReportsProgress = true;        // Разрешает возвраты изнутри процесса
			bw.WorkerSupportsCancellation = true;   // Разрешает завершение процесса

			bw.DoWork += ((HWProcess != null) ? HWProcess : DoWork);
			bw.RunWorkerCompleted += RunWorkerCompleted;

			// Донастройка окна (пробел должен считаться значением false)
			if (string.IsNullOrEmpty (Caption))
				{
				bw.RunWorkerAsync (Parameters);
				}
			else
				{
				bw.ProgressChanged += ProgressChanged;
				parameters = Parameters;

				InitializeProgressBar ();
				newPercentage = oldPercentage = (int)ProgressBarSize;
				RDInterface.SetTaskBarIndication (this, RDTaskbarModes.Indeterminate);

				AbortButton.Visible = AbortButton.Enabled = AllowAbort;
				if (AbortButton.Enabled)
					{
					AbortButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);
					AbortButton.FlatAppearance.MouseDownBackColor =
						RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultEmerald);
					}

				StateLabel.Text = Caption;
				if (CaptionInTheCenter)
					StateLabel.TextAlign = ContentAlignment.MiddleCenter;

				// Запуск
				this.Text = Caption;
				this.StartPosition = FormStartPosition.CenterParent;
				this.ShowDialog ();
				}
			}

		// Метод запускает выполнение процесса
		private void RDWorkerForm_Shown (object sender, EventArgs e)
			{
			// Перекрытие остальных окон
			if (alwaysOnTop)
				{
				this.Activate ();
				this.TopMost = true;
				}

			// Отмена центрирования на родительское окно, если это невозможно
			if (this.Left == 0)
				this.CenterToScreen ();

			// Запуск отрисовки
			RDMessageForm.CreateBackground (this);

			// Запуск
			bw.RunWorkerAsync (parameters);
			}

		// Метод обрабатывает изменение состояния процесса
		private void ProgressChanged (object sender, ProgressChangedEventArgs e)
			{
			// Обновление прогрессбара
			if (e.ProgressPercentage > ProgressBarSize)
				newPercentage = (int)ProgressBarSize;
			else if (e.ProgressPercentage < 0)
				newPercentage = oldPercentage = 0;  // Скрытие шкалы
			else
				newPercentage = e.ProgressPercentage;

			// Обновление вида окна на панели задач
			if (progress != null)
				{
				if (newPercentage != ProgressBarSize)
					RDInterface.SetProgressForTaskBarIndication (this, (uint)newPercentage);
				else
					RDInterface.SetTaskBarIndication (this, RDTaskbarModes.Indeterminate);
				}

			// Обновление текста над прогрессбаром
			if (progress != null)
				{
				StateLabel.Text = (string)e.UserState;
				this.Text = StateLabel.Text;
				}
			}

		// Метод обрабатывает завершение процесса
		private void RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
			{
			// Завершение работы исполнителя
			try
				{
				exResult = e.Result.ToString ();
				}
			catch
				{
				exResult = "";
				}
			bw.Dispose ();

			// Закрытие окна
			allowClose = true;
			if (progress != null)
				{
				RDInterface.DisableTaskBarIndication (this);
				this.Close ();
				}
			}

		// Кнопка инициирует остановку процесса
		private void AbortButton_Click (object sender, EventArgs e)
			{
			AbortButton.Enabled = false;
			bw.CancelAsync ();
			}

		// Образец метода, выполняющего длительные вычисления
		private void DoWork (object sender, DoWorkEventArgs e)
			{
			BackgroundWorker bw = ((BackgroundWorker)sender);

			// Собственно, выполняемый процесс
			for (int i = 0; i < ProgressBarSize; i++)
				{
				System.Threading.Thread.Sleep (500);
				bw.ReportProgress (i);  // Возврат прогресса

				// Завершение работы, если получено требование от диалога
				if (bw.CancellationPending)
					{
					e.Cancel = true;
					return;
					}
				}

			// Завершено
			e.Result = null;
			}

		// Закрытие формы
		private void RDWorkerForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			// Защита от ручного закрытия
			if (!allowClose)
				{
				e.Cancel = true;
				return;
				}

			// Обнуление
			DrawingTimer.Enabled = false;

			if (progress != null)
				progress.Dispose ();
			if (g != null)
				g.Dispose ();
			if (gp != null)
				gp.Dispose ();
			if (frameGreenGrey != null)
				frameGreenGrey.Dispose ();
			if (frameBack != null)
				frameBack.Dispose ();
			if (frameDark != null)
				frameDark.Dispose ();
			}

		// Отрисовка прогресс-бара
		private void DrawingTimer_Tick (object sender, EventArgs e)
			{
			// Отрисовка текущей позиции
			int recalcPercentage = (int)(oldPercentage + (newPercentage - oldPercentage) / 4);

			gp.DrawImage (frameGreenGrey, currentXOffset, 0);   // Полоса прогресса
			gp.DrawImage (frameBack, -9 * this.Width / 4, 0);   // Маска
			gp.DrawImage (frameDark, recalcPercentage *
				(progress.Width - progress.Height) / ProgressBarSize - this.Width / 4, 0);  // Фон
			gp.DrawImage (frameBack, progress.Width - progress.Height - this.Width / 4, 0); // Маска фона
			oldPercentage = recalcPercentage;

			g.DrawImage (progress, 18, StateLabel.Top + StateLabel.Height + 10);
			// Почему 18? Да хрен его знает. При ожидаемом x = 10 получается левое смещение

			// Смещение
			if (currentXOffset++ >= -2 * this.Width / 4)
				currentXOffset = -4 * this.Width / 4;
			}
		}
	}
