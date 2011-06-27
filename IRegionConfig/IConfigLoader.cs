/*
 *
 * User: AJ01
 * Date: 09.05.2011
 * Time: 18:26
 * 
 */
namespace WatchMan
{
	using System;
	
	/// <summary>
	/// Description of IConfigLoader.
	/// </summary>
	public interface IConfigLoader
	{
		IRegionConfig GetFreeConfig();
	}
}
