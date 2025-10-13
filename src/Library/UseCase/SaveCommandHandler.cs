using System.IO;
using System.Text.Json;

namespace Library.UseCase;

/// <summary>
/// 設定ファイルの作成コマンド処理部
/// </summary>
public class SaveCommandHandler
    : ICommandHandler<SaveCommand>
{
    public CommandResult Handle(SaveCommand command)
    {
        // フォルダの中を空にする
        if (Directory.Exists(command.FolderName))
        {
            Directory.Delete(command.FolderName, true); // true: サブディレクトリも含めて削除
        }

        // 設定フォルダを再作成
        Directory.CreateDirectory(command.FolderName);

        // 設定ファイルの作成
        foreach (var dto in command.TabModelDtos)
        {
            // 設定ファイル名の作成
            var fileName = command.FolderName + dto.Header + ".json";

            // JSONファイルの保存オプション
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // 見やすい整形
            };

            // 設定ファイル保存
            string json = JsonSerializer.Serialize(dto, options);
            File.WriteAllText(fileName, json);
        }

        return CommandResult.OK;
    }
}
