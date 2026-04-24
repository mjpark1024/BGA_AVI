using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common;

namespace HDSInspector.SubWindow
{
    public partial class GoldenHistoryWindow : Window
    {
        public PCS.ELF.AVI.ModelManager GoldenModelManager = new PCS.ELF.AVI.ModelManager();

        public GoldenHistoryWindow()
        {
            InitializeComponent();

            this.KeyDown += GoldenHistoryWindow_KeyDown;

            string szMachineType = PCS.ELF.AVI.ModelManager.GetMachineType(MainWindow.Setting.General.MachineCode);
            if (szMachineType != "")
                GoldenModelManager.LoadGoldenHistory(szMachineType, MainWindow.CurrentModel.Name);
            lbHistory.ItemsSource = GoldenModelManager.History;         

            if (lbHistory.Items.Count > 0)
                lbHistory.SelectedIndex = lbHistory.Items.Count - 1;

            btnOK.Click += (s, e) =>
            {
                this.Close();
            };
        }

        void GoldenHistoryWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                this.Close();
        }
    }
}
