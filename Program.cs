using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

class Program {

    static void Main(string[] args) {
        CreateStartWindow();
    }

    public static void CreateStartWindow() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new StartupWindow());
    }
}