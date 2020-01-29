using Recube.Api.Entities;
using Recube.Api.Util;

namespace Recube.Api
{
    /// <summary>
    ///     Parent class of <see cref="IPlayer" />
    /// </summary>
    public interface IOfflinePlayer
    {
        Uuid Uuid { get; }
        bool Online { get; }

        IPlayer? GetPlayer();
    }
}