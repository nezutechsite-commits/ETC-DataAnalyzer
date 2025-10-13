namespace Library.UseCase;

public enum CommandResult
{
    OK = 0,

    Error,

    /// <summary>
    /// 送信元ファイルが存在しません
    /// </summary>
    File_Not_Exist_Src,

    /// <summary>
    /// 送信元ファイルが存在しません
    /// </summary>
    File_Not_Exist_Dst,

    /// <summary>
    /// ファイル作成エラー
    /// </summary>
    File_Create_Error_Temp,

    /// <summary>
    /// CSVファイル作成エラー
    /// </summary>
    File_Create_Error_CSV,
}
