using System.IO;
using System.Text;

namespace Library.UseCase;

/// <summary>
/// CSVファイルを作成するコマンド処理部
/// </summary>
public class MakeCommandHandler
    : ICommandHandler<MakeCommand>
{
    private const string TempDirectory = @"..\..\..\temp\";
    private const string TempFilePath = TempDirectory + @"input.txt";

    public CommandResult Handle(MakeCommand command)
    {
        try
        {
            if (!File.Exists(command.SrcFilePath))
            {
                return CommandResult.File_Not_Exist_Src;
            }

            if (!Directory.Exists(command.DstFilePath))
            {
                return CommandResult.File_Not_Exist_Dst;
            }

            // 一時ファイルの作成
            if (!this.CreateTempFile(command.SrcFilePath, command.Encode))
            {
                return CommandResult.File_Create_Error_Temp;
            }

            // ファイル名の取得
            string srcPath = TempFilePath;
            string dstPath = Path.Combine(command.DstFilePath, Path.GetFileNameWithoutExtension(command.SrcFilePath) + ".csv");

            // 出力先ファイルの削除
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }

            // CSVファイルの作成
            if (!WriteCsvFile(srcPath, dstPath, command.Contents))
            {
                File.Delete(dstPath);
                return CommandResult.File_Create_Error_CSV;
            }

            return CommandResult.OK;
        }
        catch (Exception)
        {
            return CommandResult.Error;
        }
        finally
        {
            // 一時ファイルの削除
            if (Directory.Exists(TempDirectory))
            {
                Directory.Delete(TempDirectory, true);
            }
        }
    }

    /// <summary>
    /// 維持時ファイルを作成する
    /// </summary>
    /// <param name="srcFilePath"></param>
    /// <param name="encode"></param>
    /// <returns></returns>
    private bool CreateTempFile(string srcFilePath, string encode)
    {
        try
        {
            EncodingProvider provider = CodePagesEncodingProvider.Instance;
            var encoding = string.Equals(encode, "shift-jis") ? provider.GetEncoding(encode) : Encoding.UTF8;
            if (encoding is null)
            {
                return false;
            }

            // ファイルの読み込み
            string fileContent;
            using (StreamReader sr = new(srcFilePath, encoding))
            {
                fileContent = sr.ReadToEnd();
            }

            // 一時ファイルの作成
            Directory.CreateDirectory(TempDirectory);
            using (StreamWriter sw = new(TempFilePath, false, new UTF8Encoding(false)))
            {
                sw.Write(fileContent);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// CSVファイルを作成する
    /// </summary>
    /// <param name="srcFileName"></param>
    /// <param name="dstFileName"></param>
    /// <param name="contents"></param>
    /// <returns></returns>
    private static bool WriteCsvFile(string srcFileName, string dstFileName, IEnumerable<FieldModelDto> contents)
    {
        using (FileStream fs = File.Create(dstFileName))
        {
            // 必要に応じてファイルに書き込み処理を追加
        }

        using StreamWriter sw = new(dstFileName, true, new UTF8Encoding(true));

        // 行ヘッダ(フィールド名)の作成
        if (!contents.All(x => x.Name == string.Empty))
        {
            sw.WriteLine(string.Join(",", contents.Select(kvp => kvp.Name)));
        }

        using StreamReader sr = new(srcFileName, new UTF8Encoding(false));
        string? line;

        // 総桁数
        int totalDigit = contents.Sum(kvp => kvp.Digit);
        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (totalDigit > line.Length)
            {
                // 桁数が違う場合
                return false;
            }

            sw.WriteLine(FormatLine(line, contents));
        }

        return true;
    }

    /// <summary>
    /// 行をフォーマットするメソッド
    /// </summary>
    /// <param name="line"></param>
    /// <param name="contents"></param>
    /// <returns></returns>
    private static string FormatLine(string line, IEnumerable<FieldModelDto> contents)
    {
        var newLine = new StringBuilder();
        int startIndex = 0;

        foreach (var kvp in contents)
        {
            newLine.Append(line.Substring(startIndex, kvp.Digit)).Append(",");
            startIndex += kvp.Digit;
        }

        // 最後の ","を取り除く
        return newLine.ToString(0, newLine.Length - 1);
    }
}
