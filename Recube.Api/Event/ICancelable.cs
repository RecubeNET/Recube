namespace Recube.Api.Event
{
    public interface ICancelable
    {
        public bool Canceled { get; set; }
    }
}