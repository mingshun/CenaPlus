﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using CenaPlus.Entity;
namespace CenaPlus.Client.Remote.Contest
{
    /// <summary>
    /// Interaction logic for Problem_Submit.xaml
    /// </summary>
    public partial class Submit : UserControl, IContent
    {
        private int problemID;

        public Submit()
        {
            InitializeComponent();
            RichTextEditor.HighLightEdit.HighLight(txtCode);
        }

        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            var languages = Enum.GetNames(typeof(ProgrammingLanguage));
            lstLanguage.Items.Clear();
            foreach (var l in languages)
            {
                lstLanguage.Items.Add(l);
            }
            problemID = int.Parse(e.Fragment);
            var p = App.Server.GetProblem(problemID);
            var forbidden = p.ForbiddenLanguages.ToList();
            foreach (var l in forbidden)
            {
                var index = lstLanguage.Items.IndexOf(l.ToString());
                if (index >= 0)
                    lstLanguage.Items.RemoveAt(index);
            }
            lstLanguage.Items.Refresh();
            lstLanguage.SelectedIndex = 0;
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int recordID = App.Server.Submit(problemID,  new TextRange(txtCode.Document.ContentStart, txtCode.Document.ContentEnd).Text , (ProgrammingLanguage)Enum.Parse(typeof(ProgrammingLanguage), (string)lstLanguage.SelectedItem));
            var problem = App.Server.GetProblem(problemID);
            var frame = NavigationHelper.FindFrame(null, this);
            if (frame != null)
            {
                Thread.Sleep(200);
                frame.Source = new Uri("/Remote/Contest/ProblemGeneral.xaml#" + problem.ContestID, UriKind.Relative);
            }
        }

        private void imgSourcePath_DragEnter(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files.Length != 1)
            {
                imgSourcePath.Source = new BitmapImage(new Uri("/CenaPlus.Client;component/Resources/Box_Err.png", UriKind.Relative));
            }
            else
            {
                imgSourcePath.Source = new BitmapImage(new Uri("/CenaPlus.Client;component/Resources/Box_Hover.png", UriKind.Relative));
            }
        }

        private void imgSourcePath_DragLeave(object sender, DragEventArgs e)
        {
            imgSourcePath.Source = new BitmapImage(new Uri("/CenaPlus.Client;component/Resources/Box.png", UriKind.Relative));
        }

        private void imgSourcePath_Drop(object sender, DragEventArgs e)
        {
            imgSourcePath.Source = new BitmapImage(new Uri("/CenaPlus.Client;component/Resources/Box.png", UriKind.Relative));
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files.Length != 1)
            {
                ModernDialog.ShowMessage("You must select a single file.", "Error", MessageBoxButton.OK);
                return;
            }
            else
            {
                txtCode.Document.Blocks.Clear();
                txtCode.Document.Blocks.Add(new Paragraph(new Run(System.IO.File.ReadAllText(files[0]))));
                lstLanguage.SelectedItem = Entity.DetectLanguage.PathToLanguage(files[0]).ToString();
            }
        }
    }
}
