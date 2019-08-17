using Recube.Api.Entities;
using Recube.Api.Util;

namespace Recube.Api
{
	/// <summary>
	///     Parent class of <see cref="Player" />
	/// </summary>
	public interface IOfflinePlayer
	{
		Uuid Uuid { get; }
		bool Online { get; }

		Player? GetPlayer();
	}
}