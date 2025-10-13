namespace Library.UseCase;

/// <summary>
/// CSVファイルを作成するコマンド
/// </summary>
/// <param name="srcFilePath"></param>
/// <param name="dstFilePath"></param>
/// <param name="encode"></param>
/// <param name="contents"></param>
public class MakeCommand(string srcFilePath, string dstFilePath, string encode, IEnumerable<FieldModelDto> contents)
{
    /// <summary>
    /// ソース元パス
    /// </summary>
    public string SrcFilePath => srcFilePath;

    /// <summary>
    /// 出力先パス
    /// </summary>
    public string DstFilePath => dstFilePath;

    /// <summary>
    /// エンコード
    /// </summary>
    public string Encode => encode;

    public IEnumerable<FieldModelDto> Contents => contents;
}
