namespace Library.UseCase;

public class LoadCommand(string folderName)
{
    /// <summary>
    /// 設定ファイル保存先フォルダパス
    /// </summary>
    public string FolderName => folderName;

    /// <summary>
    /// 結果
    /// </summary>
    public IEnumerable<TabModelDto>? Result;
}
