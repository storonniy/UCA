using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static UCA.ControlObjectSettings.ControlObjectSettings;

namespace UCA
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new SettingsForm());
            var settings = new Settings()
            {
                Comment = "Не указан",
                FactoryNumber = "Не указан",
                OperatorName = "Не указан"
            };
            Application.Run(new Form1(settings));
        }
    }
}
