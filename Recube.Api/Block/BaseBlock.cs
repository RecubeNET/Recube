using System.Collections.Generic;

namespace Recube.Api.Block
{
	public abstract class BaseBlock
	{
		public readonly List<BlockState> BlockStates;
		public readonly List<string> Properties;
	}
}