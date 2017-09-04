namespace AstmLib.Configuration
{
	public class AstmHighLevelSettings
	{
		public string SenderId { get; set; } = "";
        public string ReceiverId { get; set; } = "";
        public string Password { get; set; } = "";
        public bool TrimRecords { get; set; } = true;
        public bool CheckSN { get; set; } = true;
	}
}
