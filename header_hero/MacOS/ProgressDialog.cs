
namespace HeaderHero
{
    public class ProgressFeedback
    {
        public string Title = "";
        public int Item = 0;
        public int Count = 0;
        public string Message = "";
    }
	
	public class ProgressDialog
	{
		public string Text {get; set;}
		
		public delegate void ProgressDialogWork(ProgressFeedback feedback);
		public ProgressDialogWork Work;
		
		public void Start()
		{
			ProgressFeedback feedback = new ProgressFeedback();
			Work(feedback);
		}
	}
}
