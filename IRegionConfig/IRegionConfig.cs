/*
 *
 * User: AJ01
 * Date: 09.05.2011
 * Time: 18:15
 * 
 */
namespace WatchMan
{
	using System;
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public interface IRegionConfig
	{
		/// <summary>
		/// Текст окна (заголовок)
		/// </summary>
		string WindowText { get; }
		
		/// <summary>
		/// Тип ввода (OwnPush, OwnTrig, ExtPush)
		/// </summary>
		string InputType { get; }
		
		/// <summary>
		/// Заголовок окна слежения
		/// </summary>
		string WatchTitle { get; }
		
		/// <summary>
		/// Периодичность слежения
		/// </summary>
		int WatchPediod { get; }
		
		/// <summary>
		/// Положение окна относительно левого края экрана
		/// </summary>
		int Left { get; set; }
		
		/// <summary>
		/// Положение окна относительно верхнего края экрана
		/// </summary>
		int Top { get; set; }
		
		/// <summary>
		/// Ширина окна
		/// </summary>
		int Width { get; set; }
		
		/// <summary>
		/// Высота окна
		/// </summary>
		int Height { get; set; }
		
		/// <summary>
		/// Цвет за которым необходимо следить
		/// </summary>
		string WatchColor { get; }
		
		/// <summary>
		/// Действие при появлении цвета
		/// </summary>
		string ActionSeen { get; }
		
		/// <summary>
		/// Действие при исчезновении цвета
		/// </summary>
		string ActionLost { get; }
		
		/// <summary>
		/// Загружает конфиг
		/// </summary>
		/// <param name="filename">Имя файла для загрузки</param>
		void Load(string filename);
		
		/// <summary>
		/// Сохраняет конфиг
		/// </summary>
		void Save();
	}
}