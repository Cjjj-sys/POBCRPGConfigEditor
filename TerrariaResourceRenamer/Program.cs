Console.WriteLine("请输入字典");
var dictItem2NamePath = Console.ReadLine();
var dictItem2NameTexts = File.ReadAllLines(dictItem2NamePath);
Dictionary<string, string> dictItem2Name = new Dictionary<string, string>();
Console.WriteLine("Item_\nProjectile_\nBuff_");
Console.WriteLine("输入前缀");
var prefix = Console.ReadLine();
foreach (var dictItem2NameText in dictItem2NameTexts)
{
    var itemId = dictItem2NameText.Split(' ')[0];
    var name = dictItem2NameText.Split(' ')[prefix == "Item_" ? 4 : 5];
    dictItem2Name.Add(itemId, name);
}

var path = AppDomain.CurrentDomain.BaseDirectory;
var filenames = Directory.EnumerateFiles(path);
foreach (var filename in filenames)
{
    var safeFileName = filename.Replace(path, "");
    Console.WriteLine(safeFileName);
    if (safeFileName.EndsWith(".png"))
    {
        try
        {
            var itemId = safeFileName.Replace($"{prefix}", "").Replace(".png", "");
            File.Move(safeFileName, $"{dictItem2Name[itemId]}.{itemId}.png");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}