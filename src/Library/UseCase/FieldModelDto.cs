namespace Library.UseCase;

public class FieldModelDto(int no, string name, int digit)
{
    /// <summary>
    /// 番号
    /// </summary>
    public int No { get; set; } = no;

    /// <summary>
    /// フィールド名
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 桁数
    /// </summary>
    public int Digit { get; set; } = digit;
}
