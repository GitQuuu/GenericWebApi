using Mapster;

namespace Api.Configuration;

/// <summary>
/// Provides configuration settings for the Mapster type mapping library.
/// This static class is used to configure global mapping behavior.
/// </summary>
public static class MapsterConfiguration
{
	/// <summary>
	/// Configures global settings for the Mapster type mapping library.
	/// This method is used to define default behaviors and settings
	/// applied across all type mappings within the application.
	/// </summary>
	public static void Configure()
	{
		TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
	}
}