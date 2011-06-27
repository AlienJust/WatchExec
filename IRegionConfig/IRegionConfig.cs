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
		string WindowText {get; }
		
		/// <summary>
		/// Положение окна относительно левого края экрана
		/// </summary>
		int Left { get; }
		
		/// <summary>
		/// Положение окна относительно верхнего края экрана
		/// </summary>
		int Top { get; }
		
		/// <summary>
		/// Ширина окна
		/// </summary>
		int Width { get; }
		
		/// <summary>
		/// Высота окна
		/// </summary>
		int Height { get; }
		
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
	}
}