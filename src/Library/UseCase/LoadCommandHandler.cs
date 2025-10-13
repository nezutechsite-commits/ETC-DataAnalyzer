using System.IO;
using System.Text.Json;

namespace Library.UseCase;

public class LoadCommandHandler
    : ICommandHandler<LoadCommand>
{
    public CommandResult Handle(LoadCommand command)
    {
        try
        {
            // 設定ファイル保存先フォルダ情報
            DirectoryInfo directoryInfo = new(command.FolderName);

            // 設定ファイル情報の取得
            var allFiles = directoryInfo.GetFiles();
            if (allFiles.Length <= 0)
            {
                return CommandResult.Error;
            }

            // 設定ファイルを1つずつ読み込み
            List<TabModelDto> dtos = [];
            foreach (var file in allFiles)
            {
                var fileName = command.FolderName + file.Name;

                // 設定ファイルを読み込む
                string json = File.ReadAllText(fileName);
                TabModelDto? tab = JsonSerializer.Deserialize<TabModelDto>(json);
                if (tab is not null)
                {
                    dtos.Add(tab);
                }
            }

            command.Result = dtos;
            return CommandResult.OK;
        }
        catch
        {
            return CommandResult.Error;
        }
    }
}
