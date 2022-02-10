namespace InfoReader.Tools;

public static class DefaultFilePath
{
    public static string CurrentConfigFile => TomlConfigFile;
    public static string TomlConfigFile => "InfoReaderConfig.toml";
}