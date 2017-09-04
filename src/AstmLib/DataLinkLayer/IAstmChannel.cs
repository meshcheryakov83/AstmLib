namespace AstmLib.DataLinkLayer
{
    public interface IAstmChannel
    {
        bool Reopen(int millisecondsTimeout = 100);
        bool IsInFaultState { get; }
        int ReadTimeout { get; set; }
        byte ReadByte();
        void WriteByte(byte b);
        void Write(byte[] buf, int offset, int length);
    }
}