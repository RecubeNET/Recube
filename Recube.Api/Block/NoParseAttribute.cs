using System;

namespace Recube.Api.Block
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NoParseAttribute : Attribute
    {
    }
}