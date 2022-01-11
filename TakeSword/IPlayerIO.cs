namespace TakeSword
{
    public interface IPlayerIO
    {
        void Write(string message);
        void WriteLine(string message);
        string ReadLine();
    }
}

