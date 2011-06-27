/*
 *
 * User: AJ01
 * Date: 09.05.2011
 * Time: 18:28
 * 
 */
namespace WatchMan
{
	using System;
	using System.Xml;
	using System.Xml.Linq;
	
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public class XmlConfig : IRegionConfig
	{
		/// <summary>
		/// Текст окна (заголовок)
		/// </summary>
		public string WindowText { get; private set; }
		
		/// <summary>
		/// Тип ввода
		/// </summary>
		public string InputType { get; private set; }
		
		/// <summary>
		/// Заголовок окна-цели
		/// </summary>
		public string WatchTitle { get; private set; }
		
		/// <summary>
		/// Периодичность слежения
		/// </summary>
		public int WatchPediod { get; private set; }
		
		/// <summary>
		/// Положение окна относительно левого края экрана
		/// </summary>
		public int Left { get; set; }
		
		/// <summary>
		/// Положение окна относительно верхнего края экрана
		/// </summary>
		public int Top { get; set; }
		
		/// <summary>
		/// Ширина окна
		/// </summary>
		public int Width { get; set; }
		
		/// <summary>
		/// Высота окна
		/// </summary>
		public int Height { get; set; }
		
		/// <summary>
		/// Цвет за которым необходимо следить
		/// </summary>
		public string WatchColor { get; private set; }
		
		/// <summary>
		/// Действие, которое действительно было считано из Xml файла
		/// </summary>
		private string _actionSeenReal = string.Empty;
		
		/// <summary>
		/// Действие при появлении цвета
		/// </summary>
		public string ActionSeen { get; private set; }
		
		private string _actionLostReal = string.Empty;
		
		/// <summary>
		/// Действие при исчезновении цвета
		/// </summary>
		public string ActionLost { get; private set; }
		
		private string _filename = string.Empty;
		
		public XmlConfig()
		{
			this.WindowText = "FreeWindow";
			this.InputType = "ownpush";
			this.WatchTitle = "*";
			this.WatchPediod = 1000;
			this.WatchColor = "#000000";
			this.ActionLost = "";
			this.ActionSeen = "";
			this.Left = 64;
			this.Top = 64;
			this.Width = 64;
			this.Height = 64;
		}
		
		public void Load(string filename)
		{
			this._filename = filename;
			XmlDocument xDoc = new XmlDocument();
			xDoc.Load(filename);
			foreach(XmlNode node0 in xDoc.ChildNodes)
			{
				if (node0.Name == "RegionConfig")
				{
					foreach(XmlNode node1 in node0.ChildNodes)
					{
						if (node1.Name == "Window")
						{
							this.WindowText = node1.Attributes["Title"].Value;
							this.InputType = node1.Attributes["Type"].Value;
							this.Left = int.Parse(node1.Attributes["Left"].Value);
							this.Top = int.Parse(node1.Attributes["Top"].Value);
							this.Width = int.Parse(node1.Attributes["Width"].Value);
							this.Height = int.Parse(node1.Attributes["Height"].Value);
						}
						foreach(XmlNode node2 in node1.ChildNodes)
						{
							if (node2.Name == "Watch")
							{
								this.WatchTitle = node2.Attributes["Title"].Value;
								this.WatchColor = node2.Attributes["Color"].Value;
								this.WatchPediod = int.Parse(node2.Attributes["Delay"].Value);
								foreach(XmlNode node3 in node2.ChildNodes)
								{
									if (node3.Name == "Seen")
									{
										this._actionSeenReal = node3.InnerText;
									}
									else if (node3.Name == "Lost")
									{
										this._actionLostReal = node3.InnerText;
									}
								}
							}
						}
					}
				}
			}
			this.ActionLost = this.ExtractActions(this._actionLostReal);
			this.ActionSeen = this.ExtractActions(this._actionSeenReal);
		}
		
		public void Save()
		{
			XDocument doc = new XDocument
				(
					new XElement
					(
						"RegionConfig",
						new XElement
						(
							"Window",
							new XAttribute("Title", this.WindowText),
							new XAttribute("Type", this.InputType),
							new XAttribute("Left", this.Left.ToString()),
							new XAttribute("Top", this.Top.ToString()),
							new XAttribute("Width", this.Width.ToString()),
							new XAttribute("Height", this.Height.ToString()),
							new XElement
							(
								"Watch",
								new XAttribute("Title", this.WatchTitle),
								new XAttribute("Color", this.WatchColor),
								new XAttribute("Delay", this.WatchPediod.ToString()),
								new XElement("Seen", this._actionSeenReal),
								new XElement("Lost", this._actionLostReal)
							)
						)
					)
				);
			doc.Save(this._filename);
		}
		
		private readonly string _preprocessorLineStart = "#";
		private readonly string _preprocessorCommandSplit = " ";
		private readonly string _preprocessorCommandInclude = "include";
		
		private string ExtractActions(string actionsText)
		{
			string result = string.Empty;
			var lines = actionsText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				var modifiedLine = line.TrimStart(' ', '\t');
				// I can preprocess line as I wish here
				if (modifiedLine.StartsWith(this._preprocessorLineStart))
				{
					result += this.PreprocessLine(modifiedLine);
				}
				else if (modifiedLine != string.Empty) // Could be that line was consisted of only spaces and tabs
				{
					result += modifiedLine + Environment.NewLine;
				}
			}
			
			return result;
		}
		
		private string PreprocessLine(string line)
		{
			string result = string.Empty;
			string expectedLineStart = this._preprocessorLineStart + this._preprocessorCommandInclude + this._preprocessorCommandSplit;
			if (line.StartsWith(expectedLineStart)) // like "#include actions.txt"
			{
				string openFileName = line.Substring(expectedLineStart.Length); // sets to actions.act
				System.IO.FileInfo fi = new System.IO.FileInfo(this._filename);
				openFileName = System.IO.Path.Combine(fi.DirectoryName, openFileName);
				if (System.IO.File.Exists(openFileName))
				{
					using (var sr = new System.IO.StreamReader(openFileName))
					{
						while(!sr.EndOfStream)
						{
							var readedLine = sr.ReadLine();
							var modifiedLine = readedLine.TrimStart(' ', '\t');
							if (modifiedLine != string.Empty)
							{
								result += modifiedLine + Environment.NewLine;
							}
						}
						sr.Close();
					}
				}
			}
			return result;
		}
	}
}