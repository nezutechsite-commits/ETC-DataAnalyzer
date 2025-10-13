using System.Collections.ObjectModel;

namespace Library.UseCase;

/// <summary>
/// タブモデルのDTO
/// </summary>
public class TabModelDto
{
    /// <summary>
    /// ヘッダー
    /// </summary>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// 中身
    /// </summary>
    public ObservableCollection<FieldModelDto> Contents { get; set; } = [];
}
