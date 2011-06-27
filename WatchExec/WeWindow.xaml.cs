using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using WatchMan;

namespace WatchExec
{
	/// <summary>
	/// Interaction logic for WeWindow.xaml
	/// </summary>
	public partial class WeWindow : Window
	{
		//************************************************
		System.Windows.Media.Brush brushStart = new SolidColorBrush(Colors.Red);
		System.Windows.Media.Brush brushStop = new SolidColorBrush(Colors.Lime);
		//************************************************
		private int width = 100;
		private int captionHeight = 19;
		private int x = 100;
		private int y = 100;
		private int contentHeight = 50;
		//************************************************
		WatchManager externalWatchMan;
		WatchManager selfWatchMan;
		bool doWork = false;
		object thisLock = new object();
		//************************************************
		int reuseDelay = 250;
		string keySeen = "null";
		string keyLost = "null";
		string color = "0";
		string windowTitle = "*";
		string regionType = "ownPush";
		string settingsFileName = "";
		List<string> settingsRaw = new List<string>();
		//************************************************
		BackgroundWorker backgroundWorkerAnalyzer = new BackgroundWorker();
		//public WeWindow(string settingsFile, WatchManager externalCpManager)
		
		private IRegionConfig _settings;
		public WeWindow(IRegionConfig settings, WatchManager externalCpManager)
		{
			InitializeComponent();
			
			this._settings = settings;
			this.buttonStartStop.Background = brushStop;

			//this.settingsFileName = settingsFile;
			//ReadSettingsFromFile(settingsFile);
			
			this.externalWatchMan = externalCpManager;
			
			this.labelCaption.Content = this._settings.WindowText;
			this.Title = this._settings.WindowText;
			
			this.windowTitle = this._settings.WatchTitle;
			this.reuseDelay = this._settings.WatchPediod;
			
			this.Left = this._settings.Left;
			this.Top = this._settings.Top;
			this.Width = this._settings.Width;
			this.Height = this.captionHeight + this._settings.Height;
			
			this.keyLost = this._settings.ActionLost;
			this.keySeen = this._settings.ActionSeen;
			
			bool regionIsTrigger = false;
			if (this._settings.InputType.ToLower() == "owntrig") regionIsTrigger = true;

			selfWatchMan = new WatchManager(regionIsTrigger);
			backgroundWorkerAnalyzer.DoWork +=new DoWorkEventHandler(backgroundWorkerAnalyzer_DoWork);
			backgroundWorkerAnalyzer.RunWorkerAsync();
		}
		
		#region MoveTo TextConfig
		/*
		private void ReadSettingsFromFile(string filename)
		{
			try
			{
				StreamReader sr = new StreamReader(filename);
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					settingsRaw.Add(line);
					string[] parts = line.Split('=');
					if (parts.Length == 2)
					{
						if (parts[0] == "title") this.labelCaption.Content = parts[1];

						else if (parts[0] == "w") this.width = int.Parse(parts[1]);
						else if (parts[0] == "h") this.contentHeight = int.Parse(parts[1]);
						else if (parts[0] == "x") this.x = int.Parse(parts[1]);
						else if (parts[0] == "y") this.y = int.Parse(parts[1]);

						else if (parts[0] == "type") this.regionType = parts[1];

						else if (parts[0] == "seen") this.keySeen = parts[1];
						else if (parts[0] == "lost") this.keyLost = parts[1];
						else if (parts[0] == "color") this.color = parts[1];

						else if (parts[0] == "delay") this.reuseDelay = int.Parse(parts[1]);
						else if (parts[0] == "window") this.windowTitle = parts[1];
					}
				}
				sr.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR with " + filename);
			}
		}
		private string ReadActionFromFile(string filename)
		{
			string result = "";
			try
			{
				StreamReader sr = new StreamReader(filename);
				result = sr.ReadToEnd();
				sr.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR with " + filename);
			}
			return result;
		}
		private void SaveSettingsToFile(string filename)
		{
			//List<string> settingsNew = new List<string>();
			//foreach(
			try
			{
				System.IO.StreamWriter sw = new StreamWriter(filename);
				for (int i = 0; i < settingsRaw.Count; i++)
				{
					if (settingsRaw[i].StartsWith("w="))
					{
						settingsRaw[i] = "w=" + ((int)this.Width).ToString();
					}
					else if (settingsRaw[i].StartsWith("h="))
					{
						settingsRaw[i] = "h=" + ((int)(this.Height - captionHeight)).ToString();
					}
					else if (settingsRaw[i].StartsWith("x="))
					{
						settingsRaw[i] = "x=" + ((int)(this.Left)).ToString();
					}
					else if (settingsRaw[i].StartsWith("y="))
					{
						settingsRaw[i] = "y=" + ((int)(this.Top)).ToString();
					}
					sw.WriteLine(settingsRaw[i]);
				}
				sw.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Cannot save settings");
			}
		}
		*/
		#endregion
		
		private void backgroundWorkerAnalyzer_DoWork(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				try
				{
					bool needAction = false;
					lock (thisLock)
					{
						needAction = doWork;
					}
					if (needAction)
					{
						if ((WinAPI.GetWindowText(WinAPI.GetForegroundWindow()) == this.windowTitle) || (this.windowTitle == "*"))
						{
							System.Drawing.Rectangle bitmapRect = GetClientRectInScreenCoords();
							if (this.regionType.ToLower() == "extpush")//если есть необходимость работать с внешним WatchMan (casting for long time, usualy when many regions are trying to block input device)
							{
								//если внешний cpMan свободен - выполняется асинхронный анализ картинки:
								if (!this.externalWatchMan.AsyncProcessingInProgress)
									externalWatchMan.ProceedImageAsync(GetBitmapFromScreen(bitmapRect), bitmapRect, this.color, this.keySeen, this.keyLost);
							}
							else //если нет необходимости обработки во внешнем cpMan (например я знаю, что мое действие практически не блокирует систему ввода (attack, ss, next, etc))
							{
								selfWatchMan.ProceedImage(GetBitmapFromScreen(bitmapRect), bitmapRect, this.color, this.keySeen, this.keyLost);
							}
						}
					}
					//при использовании внешнего cpMan, reuseDelay будет фактически являться приоритетом действия
					System.Threading.Thread.Sleep(reuseDelay);
				}
				catch// (Exception ex)
				{
					//MessageBox.Show(ex.ToString());
				}
			}
		}

		public Bitmap GetBitmapFromScreen(System.Drawing.Rectangle clientRect)
		{
			System.Drawing.Image image = new Bitmap(clientRect.Width, clientRect.Height);
			Graphics grfx = Graphics.FromImage(image);
			grfx.CopyFromScreen(new System.Drawing.Point(clientRect.Left, clientRect.Top), new System.Drawing.Point(0, 0), clientRect.Size);
			grfx.Dispose();
			return (Bitmap)image;
		}

		public object GetThisPropertyThreadSafe(string propertyName)
		{
			object invokeResult = this.Dispatcher.Invoke(new GetProperty(GetThisProperty), propertyName);
			return invokeResult;
		}
		delegate object GetProperty(string propertyName);
		private object GetThisProperty(string propertyName)
		{
			object result = false;
			switch (propertyName)
			{
				case "Width":
					result = this.Width;
					break;
				case "Height":
					result = this.Height;
					break;
				case "Left":
					result = this.Left;
					break;
				case "Top":
					result = this.Top;
					break;
			}
			return result;
		}
		public System.Drawing.Rectangle GetClientRectInScreenCoords()
		{
			System.Drawing.Rectangle result = new System.Drawing.Rectangle();
			result.X = (int)((double)GetThisPropertyThreadSafe("Left"));
			result.Y = (int)((double)GetThisPropertyThreadSafe("Top"));
			result.Width = (int)((double)GetThisPropertyThreadSafe("Width")) - 4;
			result.Height = (int)((double)GetThisPropertyThreadSafe("Height")) - 2 - captionHeight;
			result.Offset(2, captionHeight);

			return result;
		}
		private void toolStripMenuItemExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		//*****************************************************************
		private void buttonClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void borderCaption_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released)
			{
				this.DragMove();
			}
		}

		private void StartStop()
		{
			if (this.buttonStartStop.Background == brushStop)
			{
				this.buttonStartStop.Background = brushStart;
				lock (thisLock)
				{
					doWork = true;
				}
			}
			else
			{
				this.buttonStartStop.Background = brushStop;
				lock (thisLock)
				{
					doWork = false;
				}
			}
		}
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			//this.SaveSettingsToFile(this.settingsFileName);
			this._settings.Left = (int)this.Left;
			this._settings.Top = (int)this.Top;
			this._settings.Width = (int)this.Width;
			this._settings.Height = (int)this.Height - this.captionHeight;
			
			this._settings.Save();
		}

		private void buttonStartStop_ToolTipOpening(object sender, ToolTipEventArgs e)
		{
			StartStop();
			e.Handled = true;
		}
	}
}
