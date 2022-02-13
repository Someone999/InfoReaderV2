using System.Collections.Generic;
using System.Globalization;

namespace InfoReader.Tools.I8n
{

    public class TranslationDictionary : Dictionary<string, string>
    {
        public new string this[string key]
        {
            get => ContainsKey(key) ? base[key] : "null";
            set => base[key] = value;
        }
    }
    public class LocalizationInfo : ILocalizationInfo
    {
        private static Dictionary<string, LocalizationInfo> _localizationInfos = new();

        private LocalizationInfo(string cultureName, bool loadTranslations = true)
        {
            Culture = CultureInfo.GetCultureInfo(cultureName);
            CultureName = Culture.DisplayName;
            CultureShortName = Culture.Name;
            if (loadTranslations)
            {
                TryGetTranslation(new SqliteLanguageFileOperator(), CultureShortName);
            }
        }

        public static LocalizationInfo GetLocalizationInfo(string cultureName, bool loadTranslations = true)
        {
            if (!_localizationInfos.ContainsKey(cultureName.ToLower()))
                _localizationInfos.Add(cultureName.ToLower(), new LocalizationInfo(cultureName));
            return _localizationInfos[cultureName.ToLower()];
        }

        private LocalizationInfo(CultureInfo cultureInfo)
        {
            Culture = cultureInfo;
            CultureName = Culture.DisplayName;
            CultureShortName = Culture.Name;
            TryGetTranslation(new SqliteLanguageFileOperator(), CultureShortName.ToLower());
        }

        public static LocalizationInfo GetLocalizationInfo(CultureInfo cultureInfo)
        {
            if (!_localizationInfos.ContainsKey(cultureInfo.Name.ToLower()))
                _localizationInfos.Add(cultureInfo.Name.ToLower(), new LocalizationInfo(cultureInfo));
            return _localizationInfos[cultureInfo.Name.ToLower()];
        }

        void TryGetTranslation(ILanguageFileOperator fileOperator, string languageId)
        {
            Translations = fileOperator.ReadAll(languageId.ToLower());
            if (Translations.Count == 0)
            {
                Logger.LogError("No translation for this language now. English will be used.");
                TryGetTranslation(fileOperator, "en-us");
            }
        }
        public string CultureShortName { get; }
        public string CultureName { get; }
        public CultureInfo Culture { get; }
        public TranslationDictionary Translations { get; private set; } = new();
        public static LocalizationInfo Default { get; } = GetLocalizationInfo("en-us");
        public static LocalizationInfo SystemCulture { get; } = GetLocalizationInfo(CultureInfo.CurrentCulture);
        private static LocalizationInfo _current = SystemCulture;

        public static LocalizationInfo Current
        {
            get => _current;
            set
            {
                lock (_current)
                {
                    _current = value;
                }
            }
        }
    }
}