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
		public string WindowText {get; private set; }
		
		/// <summary>
		/// Положение окна относительно левого края экрана
		/// </summary>
		public int Left { get; private set; }
		
		/// <summary>
		/// Положение окна относительно верхнего края экрана
		/// </summary>
		public int Top { get; private set; }
		
		/// <summary>
		/// Ширина окна
		/// </summary>
		public int Width { get; private set; }
		
		/// <summary>
		/// Высота окна
		/// </summary>
		public int Height { get; private set; }
		
		/// <summary>
		/// Цвет за которым необходимо следить
		/// </summary>
		public string WatchColor { get; private set; }
		
		/// <summary>
		/// Действие при появлении цвета
		/// </summary>
		public string ActionSeen { get; private set; }
		
		/// <summary>
		/// Действие при исчезновении цвета
		/// </summary>
		public string ActionLost { get; private set; }
		
		public XmlConfig()
		{
			this.WindowText = "FreeWindow";
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
							this.Left = int.Parse(node1.Attributes["Left"].Value);
							this.Top = int.Parse(node1.Attributes["Top"].Value);
							this.Width = int.Parse(node1.Attributes["Width"].Value);
							this.Height = int.Parse(node1.Attributes["Height"].Value);
						}
						else if (node1.Name == "Watch")
						{
							this.WatchColor = node1.Attributes["Color"].Value;
							foreach(XmlNode node2 in node1.ChildNodes)
							{
								if (node2.Name == "Seen")
								{
									this.ActionSeen = node2.InnerText;
								}
								else if (node2.Name == "Lost")
								{
									this.ActionLost = node2.InnerText;
								}
							}
						}
					}
				}
			}
		}
		
		public void Save(string filename)
		{
			XDocument doc = new XDocument(new XElement("RegionConfig",
			                                           new XElement("Window",
			                                                        new XAttribute("Title", this.WindowText),
			                                                        new XAttribute("Left", this.Left.ToString()),
			                                                        new XAttribute("Top", this.Top.ToString()),
			                                                        new XAttribute("Width", this.Width.ToString()),
			                                                        new XAttribute("Height", this.Height.ToString())
			                                                       ),
			                                           new XElement("Watch",
			                                                        new XAttribute("Color", this.WatchColor),
			                                                        new XElement("Seen", this.ActionSeen),
			                                                        new XElement("Lost", this.ActionLost)
			                                                       )
			                                           )
			                             );
			//
		}
	}
}