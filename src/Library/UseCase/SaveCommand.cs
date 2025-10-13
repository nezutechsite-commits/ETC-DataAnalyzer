namespace Library.UseCase;

/// <summary>
/// 保存コマンド
/// </summary>
public class SaveCommand(string folderName, IEnumerable<TabModelDto> dtos)
{
    /// <summary>
    /// 設定ファイル保存先フォルダパス
    /// </summary>
    public string FolderName => folderName;

    /// <summary>
    /// タブモデルのDTO
    /// </summary>
    public IEnumerable<TabModelDto> TabModelDtos => dtos;
}
