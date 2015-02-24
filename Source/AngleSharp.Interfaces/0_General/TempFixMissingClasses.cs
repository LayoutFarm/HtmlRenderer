//BSD, 2013-2015, Florian Rappl and collab
namespace AngleSharp
{
    public class DomException : System.Exception
    {
        ErrorCode errCode;
        public DomException(ErrorCode errCode)
        {
            this.errCode = errCode;
        }
        // public string Message { get; private set; }
    }
    public interface ICssStyleDeclaration { }
    public interface IMediaList { }
    public interface IPseudoElement { }

}