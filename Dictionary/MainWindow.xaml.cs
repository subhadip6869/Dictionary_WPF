using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dictionary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread th1;
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.Bangla_DictionaryConnectionString);
        string query;
        OleDbCommand cmd;
        string wordid, meaning;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadList();
            data_listbox.SelectedIndex = 0;
            search_box.SelectedIndex = 0;
        }

        private void LoadList()
        {
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                query = "SELECT word FROM BengaliDictionary_36 ORDER BY word";
                cmd = new OleDbCommand(query, conn);
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string word = dr.GetString(dr.GetOrdinal("word"));
                    data_listbox.Items.Add(word);
                    search_box.Items.Add(word);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
        }

        private void search_box_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    int index = search_box.Items.IndexOf(search_box.Text);
                    data_listbox.ScrollIntoView(data_listbox.Items[index]);
                    data_listbox.SelectedIndex = index;
                    search_box.SelectedIndex = index;
                }
                catch (ArgumentOutOfRangeException bound_ex)
                {
                    MessageBox.Show(bound_ex.Message, "Item not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void search_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = search_box.SelectedIndex;
            data_listbox.SelectedIndex = (int)index;
            data_listbox.ScrollIntoView(data_listbox.Items[index]);
        }

        private void data_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //..........< Read Meaning >..........
                if (conn.State != ConnectionState.Open) conn.Open();
                query = "SELECT id, meaning FROM BengaliDictionary_36 WHERE word = '" + data_listbox.SelectedItem.ToString().Replace("'", "''") + "'";
                cmd = new OleDbCommand(query, conn);
                OleDbDataReader fetchdata = cmd.ExecuteReader();
                while (fetchdata.Read())
                {
                    wordid = fetchdata.GetValue(0).ToString();
                    meaning = fetchdata.GetValue(1).ToString();
                }
                cmd.Dispose();
                conn.Close();
                //..........</ Read Meaning >.........

                //..........< Print >..........
                id_label.Text = "ID: " + wordid;
                display_box.Text = meaning;

                search_box.SelectedIndex = data_listbox.SelectedIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
