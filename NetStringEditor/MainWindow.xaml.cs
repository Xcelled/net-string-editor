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
using Microsoft.Win32;

namespace NetStringEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private CompiledAssembly asm;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (asm != null && asm.Strings.Any(s => s.IsDirty))
			{
				if (MessageBox.Show("You have unsaved changes. Really open a new file?", Title, MessageBoxButton.YesNo) == MessageBoxResult.No)
					return;
			}

			var ofd = new OpenFileDialog();

			if (ofd.ShowDialog() != true)
				return;

			try
			{
				using (var f = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
				{
					DataContext = asm = CompiledAssembly.Load(f);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Could not open file: {ex}");
			}
		}

		private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (asm == null)
				return;

			var ofd = new SaveFileDialog();

			if (ofd.ShowDialog() != true)
				return;

			try
			{
				using (var f = File.Open(ofd.FileName, FileMode.Create, FileAccess.ReadWrite))
				{
					asm.Save(f);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Could not save file: {ex}");
			}

		}

		private void ResetContextItem_Click(object sender, RoutedEventArgs e)
		{
			var s = StringsTable.SelectedItem as CompiledString;
			s?.Reset();
		}

		private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
			{
				var tb = (TextBox)sender;
				var caret = tb.CaretIndex;
				tb.Text = tb.Text.Insert(caret, Environment.NewLine);
				tb.CaretIndex = caret + 1;
				e.Handled = true;
			}
		}
	}
}
