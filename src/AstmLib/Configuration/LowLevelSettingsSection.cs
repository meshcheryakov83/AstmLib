namespace AstmLib.Configuration
{
    public class AstmLowLevelSettings
	{
		public int MaxFrameLength { get; set; } = 240;
        public bool HavePriority { get; set; } = false;
        public bool CheckCrc { get; set; } = false;
		public int DelayUploadAfterDownload { get; set; } = 2000;
		public int DelayUploadAfterUpload { get; set; } = 2000;
		public int DelayUploadAfterUploadError { get; set; } = 2000;
		public int EnqWaitTimeout { get; set; } = 15000;
		public int WaitFrameTimeout { get; set; } = 30000;
	}
}
