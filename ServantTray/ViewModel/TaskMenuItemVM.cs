using GalaSoft.MvvmLight;

namespace ServantTray.ViewModel
{
    public class TaskMenuItemVM : ViewModelBase
    {
        private string m_text;
        private object m_code;

        public TaskMenuItemVM(string text, object code)
        {
            m_text = text.Replace("_", "__");
            m_code = code;
        }

        public string Title
        {
            get { return m_text; }
        }

        public object Code
        {
            get { return m_code; }
        }
    }
}
