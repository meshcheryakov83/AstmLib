namespace AstmLib.DataLinkLayer
{
    public enum DataLinkControlCodes : byte
    {
        STX = 0x02, // Start text
        ETX = 0x03, // End text
        EOT = 0x04, // End of transmission
        ENQ = 0x05, // Request of transmission
        ACK = 0x06, // Acknowledge
        NAK = 0x15, // Negative acknowledge
        ETB = 0x17, // End text briefly
        CR = 0x0D, // Carriage return
        LF = 0x0A, // Line feed
        SOH = 0x01,
        DLE = 0x10, // Data Link Escape
    }
}