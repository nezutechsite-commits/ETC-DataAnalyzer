using ETC_DataAnalyzer.Models;
using ETC_DataAnalyzer.ViewModels;
using Library.UseCase;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ETC_DataAnalyzer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private FieldModel? selectedItem;
    private List<FieldModel>? selectedItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        // フォルダ / ファイルダイアログオープン
        this.CommandBindings.Add(new CommandBinding(OpenSrcFileCommand, this.OpenSrcFileCommand_Executed));
        this.CommandBindings.Add(new CommandBinding(OpenDstFileCommand, this.OpenDstFileCommand_Executed));

        // タブの追加・削除
        this.CommandBindings.Add(new CommandBinding(NewRegistCommand, this.NewRegistCommand_Executed, this.NewRegisterCommand_CanExecute));
        this.CommandBindings.Add(new CommandBinding(DeleteCommand, this.DeleteCommand_Executed, this.DeleteCommand_CanExecute));

        // 保存関連
        this.CommandBindings.Add(new CommandBinding(MakeCommand, this.MakeCommand_Executed, this.MakeCommand_CanExecute));
        this.CommandBindings.Add(new CommandBinding(SaveCommand, this.SaveCommand_Executed, this.SaveCommand_CanExecute));

        // データグリッド関連
        this.CommandBindings.Add(new CommandBinding(AddRowCommand, this.AddCommand_Executed));
        this.CommandBindings.Add(new CommandBinding(DeleteRowCommand, this.DeleteRowCommand_Executed, this.DeleteRowCommand_CanExecute));
        this.CommandBindings.Add(new CommandBinding(InsertAbobeCommand, this.InsertAbobeCommand_Executed, this.InsertCommand_CanExecute));
        this.CommandBindings.Add(new CommandBinding(InsertBelowCommand, this.InsertBelowCommand_Executed, this.InsertCommand_CanExecute));

        this.Loaded += this.MainWindow_Loaded;
    }

    /// <summary>
    /// ソース元のファイルダイアログオープンコマンド
    /// </summary>
    public static RoutedCommand OpenSrcFileCommand { get; } = new();

    /// <summary>
    /// 出力先パスのディレクトリオープンダイアログコマンド
    /// </summary>
    public static RoutedCommand OpenDstFileCommand { get; } = new();

    /// <summary>
    /// 新規登録コマンド
    /// </summary>
    public static RoutedCommand NewRegistCommand { get; } = new();

    /// <summary>
    /// 削除コマンド
    /// </summary>
    public static RoutedCommand DeleteCommand { get; } = new();

    /// <summary>
    /// CSV出力コマンド
    /// </summary>
    public static RoutedCommand MakeCommand { get; } = new();

    /// <summary>
    /// 保存コマンド
    /// </summary>
    public static RoutedCommand SaveCommand { get; } = new();

    /// <summary>
    /// 追加コマンド
    /// </summary>
    public static RoutedCommand AddRowCommand { get; } = new();

    /// <summary>
    /// 削除コマンド
    /// </summary>
    public static RoutedCommand DeleteRowCommand { get; } = new();

    /// <summary>
    /// 上へ挿入コマンド
    /// </summary>
    public static RoutedCommand InsertAbobeCommand { get; } = new();

    /// <summary>
    /// 下へ挿入コマンド
    /// </summary>
    public static RoutedCommand InsertBelowCommand { get; } = new();

    /// <summary>
    /// ファイルオープン処理 (ソース元ファイル名)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenSrcFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        OpenFileDialog openFileDialog = new()
        {
            // ファイルの種類をフィルターとして設定
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
        };

        if (openFileDialog.ShowDialog() == true)
        {
            // 選択されたファイルのパスをテキストボックスに表示
            viewModel.SrcFilePath = openFileDialog.FileName;
        }
    }

    /// <summary>
    /// フォルダオープン処理 (出力先パス)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDstFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        OpenFolderDialog openFolderDialog = new();

        if (openFolderDialog.ShowDialog() is true)
        {
            // 選択されたファイルのパスをテキストボックスに表示
            viewModel.DstFilePath = openFolderDialog.FolderName;
        }
    }

    /// <summary>
    /// 新規登録処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NewRegistCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is MainWindowViewModel viewModel)
        {
            // 新規タブの追加
            viewModel.AddTab();
        }
    }

    private void NewRegisterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (this.DataContext is MainWindowViewModel viewModel)
        {
            e.CanExecute = viewModel.TabItems.Count < 10;
        }
    }

    /// <summary>
    /// 削除処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var ret = MessageBox.Show("対象項目を削除します。よろしいですか。", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (ret is MessageBoxResult.Yes)
        {
            // タブの削除
            var result = viewModel.DeleteTab();
            if (!result)
            {
                // エラーメッセージ
                MessageBox.Show("削除に失敗しました。");
            }
        }
    }

    private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (viewModel.TabItems.Count >= 2)
        {
            e.CanExecute = true;
        }
    }

    /// <summary>
    /// CSVファイルの作成処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MakeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is MainWindowViewModel viewModel)
        {
            var result = viewModel.MakeCsvFile(this.tabControl.SelectedIndex);

            if (result is CommandResult.OK)
            {
                MessageBox.Show("ファイルの作成に成功しました。");
            }
            else if (result is CommandResult.File_Not_Exist_Src)
            {
                MessageBox.Show("ソース元のファイルが存在しません。");
            }
            else if (result is CommandResult.File_Not_Exist_Dst)
            {
                MessageBox.Show("出力先のフォルダが存在しません。");
            }
            else if (result is CommandResult.File_Create_Error_Temp)
            {
                MessageBox.Show("一時ファイルの作成に失敗しました。");
            }
            else if (result is CommandResult.File_Create_Error_CSV)
            {
                MessageBox.Show("CSVファイルの作成に失敗しました。");
            }
            else
            {
                MessageBox.Show($"ファイルの作成に失敗しました。");
            }
        }
    }

    private void MakeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        e.CanExecute = viewModel.TabItems.Count >= 1
            && viewModel.TabItems.All(x => !string.IsNullOrEmpty(x.Header))  // ラベルが空白でない
            && viewModel.TabItems.GroupBy(x => x.Header).All(g => g.Count() == 1);  // 同じラベル名がない
    }

    /// <summary>
    /// 設定ファイルの保存処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is MainWindowViewModel viewModel)
        {
            var result = viewModel.OnSave();
            if (result is CommandResult.OK)
            {
                // 成功
                MessageBox.Show("更新しました。");
            }
            else
            {
                // 失敗
                MessageBox.Show("更新に失敗しました。");
            }
        }
    }

    private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        e.CanExecute = viewModel.TabItems.Count >= 1
            && viewModel.TabItems.All(x => !string.IsNullOrEmpty(x.Header))   // ラベルが空白でない
            && viewModel.TabItems.GroupBy(x => x.Header).All(g => g.Count() == 1);  // 同じラベル名がない
    }

    /// <summary>
    /// 行の追加 (最後)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is MainWindowViewModel viewModel)
        {
            // 行の追加
            viewModel.InsertRow(viewModel.TabItems[viewModel.SelectedTabIndex].Contents.Count);
        }
    }

    /// <summary>
    /// 行の挿入 (選択行の上)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InsertAbobeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (this.selectedItem is not null)
        {
            // 行の挿入
            viewModel.InsertRow(this.selectedItem.No - 1);
        }
    }

    /// <summary>
    /// 行の挿入 (選択行の下)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InsertBelowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (this.selectedItem is not null)
        {
            // 行の挿入
            viewModel.InsertRow(this.selectedItem.No);
        }
    }

    private void InsertCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (this.selectedItems is null || this.selectedItems.Count > 1)
        {
            return;
        }

        if (this.selectedItem is not null)
        {
            e.CanExecute = true;
        }
    }

    /// <summary>
    /// 選択行の削除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteRowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (this.DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (this.selectedItems is not null)
        {
            // 行の削除
            viewModel.DeleteRow(this.selectedItems);
        }
    }

    private void DeleteRowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (this.selectedItems is not null)
        {
            e.CanExecute = true;
        }
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        {
            return;
        }

        if (dataGrid.SelectedItem is FieldModel fieldModel)
        {
            this.selectedItem = fieldModel;
        }

        this.selectedItems = dataGrid.SelectedItems.Cast<FieldModel>().ToList();
    }

    /// <summary>
    /// ロード処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is MainWindowViewModel viewModel)
        {
            // ロード処理
            var result = viewModel.OnLoaded();
            if (result is not CommandResult.OK)
            {
                // ロード失敗
                MessageBox.Show("設定ファイルの読み込みに失敗しました。");
            }
        }
    }

    /// <summary>
    /// セルの編集時の処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FieldDataGrid_CurrentCellChanged(object sender, EventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        {
            return;
        }

        dataGrid.CommitEdit();  // 行のコミット
        dataGrid.CommitEdit();  // 列のコミット
    }
}