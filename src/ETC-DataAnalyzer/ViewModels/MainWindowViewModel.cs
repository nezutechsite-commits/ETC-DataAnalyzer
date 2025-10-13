using ETC_DataAnalyzer.Models;
using Library.UseCase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETC_DataAnalyzer.ViewModels;

internal class MainWindowViewModel : IEditableObject, INotifyPropertyChanged
{
    private const string SettingFilePath = @"..\..\..\settings\";

    private string srcFilePath = string.Empty;
    private string dstFilePath = string.Empty;

    private int selectedTabIndex = 0;
    private string selectedEncode = "shift-jis";

    private readonly LoadCommandHandler loadCommandHandler = new();
    private readonly MakeCommandHandler MakeCommandHandler = new();
    private readonly SaveCommandHandler SaveCommandHandler = new();

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// [ja-JP] ソース元ファイルパス
    /// </summary>
    public string SrcFilePath
    {
        get => this.srcFilePath;
        set
        {
            if (this.srcFilePath != value)
            {
                this.srcFilePath = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 出力先ファイルパス
    /// </summary>
    public string DstFilePath
    {
        get => this.dstFilePath;
        set
        {
            if (this.dstFilePath != value)
            {
                this.dstFilePath = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// タブのインデックス
    /// </summary>
    public int SelectedTabIndex
    {
        get => this.selectedTabIndex;
        set
        {
            if (this.selectedTabIndex != value)
            {
                this.selectedTabIndex = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 選択されたエンコード
    /// </summary>
    public string SelectedEncode
    {
        get => this.selectedEncode;
        set
        {
            if (this.selectedEncode != value)
            {
                this.selectedEncode = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// エンコードのコレクション
    /// </summary>
    public ObservableCollection<string> Encodes { get; } = ["shift-jis", "utf-8"];

    /// <summary>
    /// タブ項目
    /// </summary>
    public ObservableCollection<TabModel> TabItems { get; set; } = [];

    /// <summary>
    /// タブの追加
    /// </summary>
    public void AddTab()
    {
        this.TabItems.Add(new TabModel("カスタム", [new(1, string.Empty, 0)]));

        if (this.TabItems.Count <= 1)
        {
            this.SelectedTabIndex = 0;
        }
    }

    /// <summary>
    /// [ja-JP] タブの削除
    /// </summary>
    public bool DeleteTab()
    {
        // SelectedTabIndex番のタブが存在しない場合、バインドエラーが発生するため退避する
        var temp = this.selectedTabIndex;
        this.SelectedTabIndex = 0;

        // タブの削除
        this.TabItems.RemoveAt(temp);

        // 選択していたタブ番号に戻す
        var lastIndex = this.TabItems.Count - 1;
        this.SelectedTabIndex = TabItems.Count > temp
            ? temp
            : lastIndex;

        return true;
    }

    /// <summary>
    /// 現在のタブの表に行を挿入する
    /// </summary>
    /// <param name="no">挿入する位置</param>
    public void InsertRow(int no)
    {
        var contents = this.TabItems[this.selectedTabIndex].Contents;
        contents.Insert(no, new(no, string.Empty, 0));

        this.UpdateNo();
    }

    /// <summary>
    /// 現在のタブの表の選択行の削除
    /// </summary>
    /// <param name="selectedItems">選択行</param>
    public void DeleteRow(List<FieldModel> selectedItems)
    {
        foreach (var selectedItem in selectedItems)
        {
            this.TabItems[this.selectedTabIndex].Contents.Remove(selectedItem);
        }

        this.UpdateNo();
    }

    /// <summary>
    /// 設定保存
    /// </summary>
    /// <returns>結果</returns>
    public CommandResult OnSave()
    {
        // タブの内容をJSON形式で保存
        SaveCommand command = new(SettingFilePath, TabModel.GetTabModelDtos(this.TabItems));
        var result = this.SaveCommandHandler.Handle(command);

        return result;
    }

    /// <summary>
    /// ロード処理
    /// </summary>
    public CommandResult OnLoaded()
    {
        // ロード処理
        LoadCommand command = new(SettingFilePath);
        var result = this.loadCommandHandler.Handle(command);

        if (command.Result is not null)
        {
            // 設定内容を反映
            foreach (var dto in command.Result)
            {
                this.TabItems.Add(new TabModel(dto));
            }
        }

        return result;
    }

    /// <summary>
    /// CSVファイルの作成
    /// </summary>
    /// <param name="selectedIndex">タブのインデックス</param>
    /// <returns>結果</returns>
    public CommandResult MakeCsvFile(int selectedIndex)
    {
        // CSVファイルの作成処理
        MakeCommand command = new(
            this.srcFilePath,
            this.dstFilePath,
            this.selectedEncode,
            TabModel.GetFieldModelDto(this.TabItems[selectedIndex].Contents));
        var result = this.MakeCommandHandler.Handle(command);

        if (result is CommandResult.OK)
        {
            // 設定ファイルの保存
            this.OnSave();
        }

        return result;
    }

    /// <summary>
    /// 番号の振り直し
    /// </summary>
    private void UpdateNo()
    {
        int newNo = 1;
        foreach (var item in this.TabItems[this.selectedTabIndex].Contents)
        {
            item.No = newNo;
            newNo++;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void BeginEdit()
    {
    }

    public void CancelEdit()
    {
    }

    public void EndEdit()
    {
    }
}