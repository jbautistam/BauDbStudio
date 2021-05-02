using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.RestStudio.ViewModels.Explorers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.RestStudio.Views.Explorers
{
	/// <summary>
	///		Arbol del explorador de API Rest
	/// </summary>
	public partial class TreeRestApiExplorer : UserControl
	{
		public TreeRestApiExplorer(TreeRestApiViewModel treeViewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = treeViewModel;
		}

		/// <summary>
		///		ViewModel del árbol
		/// </summary>
		public TreeRestApiViewModel ViewModel { get; }

		private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{ 
			if (trvExplorer.DataContext is TreeRestApiViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
				ViewModel.SelectedNode = node;
		}

		private void trvExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{ 
			if (trvExplorer.DataContext is TreeRestApiViewModel && (sender as TreeView)?.SelectedItem is BaseTreeNodeViewModel node)
			{
				ViewModel.SelectedNode = node;
				ViewModel.OpenCommand.Execute(null);
			}
		}

		private void trvExplorer_MouseDown(object sender, MouseButtonEventArgs e)
		{ 
			if (e.ChangedButton == MouseButton.Left)
				ViewModel.SelectedNode = null;
		}
	}
}