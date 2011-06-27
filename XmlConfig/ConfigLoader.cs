/*
 *
 * User: AJ01
 * Date: 09.05.2011
 * Time: 18:36
 * 
 */
namespace WatchMan
{
	using System;
	
	/// <summary>
	/// Description of ConfigLoader.
	/// </summary>
	public class ConfigLoader : IConfigLoader
	{
		public ConfigLoader()
		{
			
		}
		public IRegionConfig GetFreeConfig()
		{
			return new XmlConfig();
		}
	}
}
