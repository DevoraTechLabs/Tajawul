using System;

namespace Tajawul.Interfaces.User.Translation;

public interface IMTService
{
    Task<string?> GetMTResponseAsync(string text, string tgt_lang, string src_lang);
}
