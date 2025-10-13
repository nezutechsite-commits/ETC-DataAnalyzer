using Library.UseCase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETC_DataAnalyzer.Models;

internal class TabModel : INotifyPropertyChanged
{
    private string header = string.Empty;
    private ObservableCollection<FieldModel> contents = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="TabModel"/> class.
    /// </summary>
    /// <param name="header">ヘッダー</param>
    /// <param name="cotnents">中身</param>
    public TabModel(string header, ObservableCollection<FieldModel> cotnents)
    {
        this.Header = header;
        this.Contents = cotnents;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TabModel"/> class.
    /// </summary>
    /// <param name="dto">タブモデルのDTO</param>
    public TabModel(TabModelDto dto)
    {
        this.Header = dto.Header;
        foreach (var content in dto.Contents)
        {
            var dt = new FieldModel(content.No, content.Name, content.Digit);
            this.Contents.Add(dt);
        }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// ヘッダー
    /// </summary>
    public string Header
    {
        get => this.header;
        set
        {
            if (this.header != value)
            {
                this.header = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 中身
    /// </summary>
    public ObservableCollection<FieldModel> Contents
    {
        get => this.contents;
        set
        {
            if (this.contents != value)
            {
                this.contents = value;
                this.OnPropertyChanged();
            }
        }
    }
    public static IEnumerable<TabModelDto> GetTabModelDtos(IEnumerable<TabModel> tabModels)
    {
        foreach (var tabModel in tabModels)
        {
            yield return GetTabModelDto(tabModel);
        }
    }

    /// <summary>
    /// タブモデルのDTOを取得する
    /// </summary>
    /// <param name="tabModel"></param>
    /// <returns></returns>
    public static TabModelDto GetTabModelDto(TabModel tabModel)
    {
        TabModelDto dto = new()
        {
            Header = tabModel.Header,
            Contents = GetFieldModelDto(tabModel.Contents)
        };

        return dto;
    }

    /// <summary>
    /// [ja-JP] フィールドモデルのコレクションを取得する
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    public static ObservableCollection<FieldModelDto> GetFieldModelDto(ObservableCollection<FieldModel> contents)
    {
        ObservableCollection<FieldModelDto> dtos = [];
        foreach (var content in contents)
        {
            FieldModelDto dto = new(content.No, content.Name, content.Digit);
            dtos.Add(dto);
        }

        return dtos;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
