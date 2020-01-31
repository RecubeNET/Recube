using System.Collections.Generic;

namespace Recube.Api.Block
{
    public class BlockState
    {
        public bool Default { get; }

        public int NetworkId { get; }

        public string BaseName { get; }

        // PROPERTY NAME, PROPERTY CONDITION
        public Dictionary<string, string>? Properties { get; }

        public BlockState(string baseName, int networkId, bool @default, Dictionary<string, string>? properties)
        {
            BaseName = baseName;
            NetworkId = networkId;
            Properties = properties;
            Default = @default;
        }
    }
}