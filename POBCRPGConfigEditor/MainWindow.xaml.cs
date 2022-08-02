using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;

namespace POBCRPGConfigEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Properties&Fields
        const string VERSION = "v0.1.0";
        public string SerializedPOBCRPGJson { get; set; }
        public POBCRPGJsonStructure.Rootobject DeserializedPOBCRPGJson { get; set; }
        private string _fileName;
        private bool _hasUnsavedChange = false;
        #endregion

        #region FuntionalMethods
        private void LoadPOBCRPGJson(string json)
        {
            SerializedPOBCRPGJson = json;
            DeserializedPOBCRPGJson = JsonSerializer.Deserialize<POBCRPGJsonStructure.Rootobject>(json);
            ListBoxRPGList.ItemsSource = null;
            ListBoxRPGList.Items.Clear();
            ListBoxRPGList.ItemsSource = DeserializedPOBCRPGJson.RPGList;
        }

        private async Task<bool> CheckSave()
        {
            if (_hasUnsavedChange)
            {
                var result = MessageBox.Show("您有未保存的更改! 是否立即保存?", "有未保存的更改", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    await Save();
                    _hasUnsavedChange = false;
                    return true;
                }
                else if (result == MessageBoxResult.No)
                {
                    return true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private async void ColdUpdate()
        {
            _hasUnsavedChange = false;
            string json = await File.ReadAllTextAsync(_fileName);
            LoadPOBCRPGJson(json);
        }

        private void HotUpdate()
        {
            SerializedPOBCRPGJson = JsonSerializer.Serialize<POBCRPGJsonStructure.Rootobject>(DeserializedPOBCRPGJson);
            ListBoxRPGList.ItemsSource = null;
            ListBoxRPGList.Items.Clear();
            ListBoxRPGList.ItemsSource = DeserializedPOBCRPGJson.RPGList;
            _hasUnsavedChange = true;
        }

        private async Task Save()
        {
            await File.WriteAllTextAsync(_fileName, SerializedPOBCRPGJson);
            MessageBox.Show("保存成功!", "保存", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddListBoxRPGItem()
        {
            var selectedItemIndex = ListBoxRPGList.SelectedIndex;
            if (selectedItemIndex == -1)
            {
                selectedItemIndex = DeserializedPOBCRPGJson.RPGList.Count - 1;
            }
            DeserializedPOBCRPGJson.RPGList.Insert(selectedItemIndex + 1, new POBCRPGJsonStructure.Rpglist()
            {
                Group = "Group",
                GoGroup = "GoGroup",
                Co = new List<string>(),
                C = 100
            });
        }

        private void DeleteListBoxRPGItem()
        {
            var selectedItem = ListBoxRPGList.SelectedItem as POBCRPGJsonStructure.Rpglist;
            if (selectedItem != null)
            {
                if (MessageBox.Show($"确定删除 {selectedItem.Group} -> {selectedItem.GoGroup} ?", "确认删除?", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    DeserializedPOBCRPGJson.RPGList.RemoveAt(ListBoxRPGList.SelectedIndex);
                }
            }
            else
            {
                MessageBox.Show("没有选中项", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        #endregion

        #region EventHandlerMethods
        private async void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            if(await CheckSave())
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "POBC-RPG.json 配置文件|*.json";
                ofd.Title = "选择 POBC-RPG.json 配置文件";
                ofd.FileName = "POBC-RPG.json";
                if ((bool)ofd.ShowDialog())
                {
                    _fileName = ofd.FileName;
                    string json = await File.ReadAllTextAsync(_fileName);
                    LoadPOBCRPGJson(json);
                }
            }
        }

        private async void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            await Save();
            ColdUpdate();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            ListBoxRPGList.ItemsSource = null;
            ListBoxRPGList.Items.Clear();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddListBoxRPGItem();
            HotUpdate();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteListBoxRPGItem();
            HotUpdate();
        }

        private void ListBoxRPGList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteListBoxRPGItem();
                HotUpdate();
            }
        }

        private void ListBoxRPGList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = ListBoxRPGList.SelectedItem as POBCRPGJsonStructure.Rpglist;
            if (selectedItem != null)
            {
                MessageBox.Show($"你双击了 {selectedItem.Group} -> {selectedItem.GoGroup} !\n但是还没有做内容编辑的部分!", "正在开发中...", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                MessageBox.Show("没有选中项", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            } 
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!await CheckSave())
            {
                e.Cancel = true;
            }
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"关于页面还没做好呢！不过可以告诉你现在的版本号哦！\n{VERSION}", $"{VERSION}", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void ListBoxMenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ListBoxRPGList.SelectedItem as POBCRPGJsonStructure.Rpglist;
            if (selectedItem != null)
            {
                MessageBox.Show($"你想要打开 {selectedItem.Group} -> {selectedItem.GoGroup} !\n但是还没有做内容编辑的部分!", "正在开发中...", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                MessageBox.Show("没有选中项", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ListBoxMenuItemAdd_Click(object sender, RoutedEventArgs e)
        {
            AddListBoxRPGItem();
            HotUpdate();
        }

        private void ListBoxMenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteListBoxRPGItem();
            HotUpdate();
        }
        #endregion
    }
}
