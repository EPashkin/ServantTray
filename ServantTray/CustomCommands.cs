using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ServantTray
{
    public static class CustomCommands
    {
        static CustomCommands()
        {
            exitCommand = new RoutedCommand("Exit", typeof(CustomCommands));
        }
        public static RoutedCommand Exit
        {
            get
            {
                return (exitCommand);
            }
        }
        static RoutedCommand exitCommand;
    }
}
