using System;

namespace Recube.Api.Block
{
    /// <summary>
    /// This attribute can be used to skip parsing certain block classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NoParseAttribute : Attribute
    {
    }
}