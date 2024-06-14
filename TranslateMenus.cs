#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

public class TranslateMenus : Editor
{
    [MenuItem("CONTEXT/VRCExpressionsMenu/Translate to English")]
    private static async void TranslateToEnglish(MenuCommand command)
    {
        var RightClickedMenu = (VRCExpressionsMenu)command.context;

        var controls = new List<VRCExpressionsMenu.Control>();
        var submenus = new List<VRCExpressionsMenu>();

        void AddControls(VRCExpressionsMenu menu)
        {
            submenus.Add(menu);

            foreach (var control in menu.controls)
            {
                controls.Add(control);

                if (control.subMenu != null)
                {
                    AddControls(control.subMenu);
                }
            }
        }

        AddControls(RightClickedMenu);

        foreach (var control in controls)
        {
            if (control.name.Any(a => a > 127))
            {
                var translationResult = await DeepL.TranslateText(control.name, DeepL.Language.EnglishUS);
                control.name = translationResult.translated_text;
            }
        }

        foreach (var submenu in submenus)
        {
            EditorUtility.SetDirty(submenu);
        }
    }

    public class DeepL
    {
        internal class Language
        {
            internal static readonly string Bulgarian = "bg";
            internal static readonly string ChineseSimplified = "zh";
            internal static readonly string Czech = "cs";
            internal static readonly string Danish = "da";
            internal static readonly string Dutch = "nl";
            internal static readonly string EnglishUS = "en-US";
            internal static readonly string EnglishUK = "en-UK";
            internal static readonly string Estonian = "et";
            internal static readonly string Finnish = "fi";
            internal static readonly string French = "fr";
            internal static readonly string German = "de";

            internal static readonly string Greek = "el";
            internal static readonly string Hungarian = "hu";
            internal static readonly string Indonesian = "id";
            internal static readonly string Italian = "it";
            internal static readonly string Japanese = "ja";
            internal static readonly string Korean = "ko";
            internal static readonly string Latvian = "lv";
            internal static readonly string Lithuanian = "lt";
            internal static readonly string Norwegian = "nb";
            internal static readonly string Polish = "pl";
            internal static readonly string Portuguese = "pt-PT";

            internal static readonly string PortugueseBrazilian = "pt-BR";
            internal static readonly string Romanian = "ro";
            internal static readonly string Russian = "ru";
            internal static readonly string Slovak = "sk";
            internal static readonly string Slovenian = "sl";
            internal static readonly string Spanish = "es";
            internal static readonly string Swedish = "sv";
            internal static readonly string Turkish = "tr";
            internal static readonly string Ukrainian = "uk";
        }

        public class TranslationResult
        {
            public string translated_text;
            public string error;
        }

        public class TranslationRequest
        {
            public string text;
            public string lang;
        }

        internal static HttpClient client = new HttpClient();

        internal static async Task<TranslationResult> TranslateText(string Text, string Language)
        {
            var result = await client.PostAsync("http://127.0.0.1:5000/translate", new StringContent(JsonConvert.SerializeObject(new TranslationRequest { text = Text, lang = Language }), Encoding.UTF8, "application/json"));

            TranslationResult output = null;

            try
            {
                output = JsonConvert.DeserializeObject<TranslationResult>(await result.Content.ReadAsStringAsync());
            }
            catch
            {
            }

            return output;
        }
    }
}

#endif