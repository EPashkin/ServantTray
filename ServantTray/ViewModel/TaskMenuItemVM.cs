using GalaSoft.MvvmLight;

namespace ServantTray.ViewModel
{
    public class TaskMenuItemVM : ViewModelBase
    {
        private string m_text;

        public TaskMenuItemVM(string text)
        {
            m_text = text;
        }

        public string Title
        {
            get { return m_text; }
        }
    }
}
