using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Data.Entity;

namespace Task_1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DataBase dataBase = new DataBase();


            Application.Run(new Map(dataBase));
        }
    }
}
