﻿using AideDeJeu.Tools;
using AideDeJeuLib;
using Markdig;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AideDeJeuCmd
{
    class Program
    {

        //static async Task<IEnumerable<Spell>> TestMarkdown(string filename)
        //{
        //    using (var sr = new StreamReader(filename))
        //    {
        //        var md = await sr.ReadToEndAsync();
        //        var document = Markdig.Parsers.MarkdownParser.Parse(md);
        //        //DumpMarkdownDocument(document);

        //        var spellss = document.ToSpells<SpellHD>();
        //        Console.WriteLine("ok");
        //        var md2 = spellss.ToMarkdownString();
        //        if (md.CompareTo(md2) != 0)
        //        {
        //            Debug.WriteLine("failed");
        //        }
        //        return spellss;
        //    }
        //}

        static async Task<IEnumerable<Monster>> TestMarkdownMonsters(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                var md = await sr.ReadToEndAsync();
                var pipeline = new MarkdownPipelineBuilder()
                    .UsePipeTables()
                    .Build();
                //var document = Markdig.Parsers.MarkdownParser.Parse(md, pipeline);
                //DumpMarkdownDocument(document);

                var monsters = AideDeJeu.Tools.MarkdownExtensions.ToItem(md) as IEnumerable<Monster>; // document.ToMonsters<MonsterHD>();
                //document.Dump();
                Console.WriteLine("ok");
                //var md2 = monsters.ToMarkdownString();
                //if (md.CompareTo(md2) != 0)
                //{
                //    Debug.WriteLine("failed");
                //}
                return monsters;
            }
        }

        static async Task CreateIndexes()
        {
            string dataDir = @"..\..\..\..\..\Data\";

            var result = string.Empty;
            var md = await LoadStringAsync(dataDir + "spells_hd.md");
            var items = AideDeJeu.Tools.MarkdownExtensions.ToItem(md) as IEnumerable<Spell>;

            var classes = new string[]
            {
                "Barde",
                "Clerc",
                "Druide",
                "Ensorceleur",
                "Magicien",
                "Paladin",
                "Rôdeur",
                "Sorcier"
            };
            var levels = new string[]
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                //"tour de magie",
                //"niveau 1",
                //"niveau 2",
                //"niveau 3",
                //"niveau 4",
                //"niveau 5",
                //"niveau 6",
                //"niveau 7",
                //"niveau 8",
                //"niveau 9"
            };
            foreach (var classe in classes)
            {
                //Console.WriteLine(classe);
                result += string.Format("## {0}\n\n", classe);
                foreach (var level in levels)
                {
                    //Console.WriteLine(level);
                    var spells = items.Where(s => s.Level == level && s.Source.Contains(classe)).OrderBy(s => s.Name).Select(s => string.Format("* [{0}](spells_hd.md#{1})", s.Name, Helpers.IdFromName(s.Name))).ToList();
                    if (spells.Count > 0)
                    {
                        result += string.Format("### {0}\n\n", level == "0" ? "Tours de magie" : "Niveau " + level);
                        result += spells.Aggregate((s1, s2) => s1 + "\n" + s2);
                        result += "\n\n";
                    }
                }
            }
            Console.WriteLine(result);
            await SaveStringAsync(dataDir + "spells_hd_by_class_level.md", result);
        }

        static async Task<IEnumerable<string>> GetAllAnchorsAsync()
        {
            var anchors = new List<string>();
            var allitems = new Dictionary<string, Item>();
            var names = Helpers.GetResourceNames();
            foreach(var name in names)
            {
                //if (name.Contains("_hd."))
                //{
                    var md = await Helpers.GetResourceStringAsync(name);
                    var item = AideDeJeu.Tools.MarkdownExtensions.ToItem(md);
                    allitems.Add(name, item);
                //}
            }
            foreach(var allitem in allitems)
            {
                if (allitem.Value is Items)
                {
                    var items = allitem.Value as Items;
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Name))
                        {
                            //Console.WriteLine(item.Name);
                            anchors.Add(item.Name);
                        }
                    }
                }
                else if(allitem.Value != null)
                {
                    if (!string.IsNullOrWhiteSpace(allitem.Value.Name))
                    {
                        //Console.WriteLine(allitem.Value.Name);
                        anchors.Add(allitem.Value.Name);
                    }
                }
            }
            return anchors;
        }

        static async Task SearchAsync(string anchor)
        {
            var first = true;
            var names = Helpers.GetResourceNames();
            foreach (var name in names)
            {
                if (name.EndsWith("_hd.md"))
                {
                    var md = await Helpers.GetResourceStringAsync(name);
                    using (var reader = new StringReader(md))
                    {
                        var line = await reader.ReadLineAsync();
                        while (line != null)
                        {
                            if (line.FirstOrDefault() != '#' &&
                                !line.StartsWith("- AltName") &&
                                line.Contains(anchor) &&
                                !line.Contains($"[{anchor}") &&
                                !line.Contains($"{anchor}]")
                                )
                            {
                                if (first)
                                {
                                    first = false;
                                    Console.WriteLine();
                                    Console.WriteLine(anchor);
                                    Console.WriteLine();
                                }
                                Console.WriteLine(line);
                                Console.WriteLine();
                            }
                            line = await reader.ReadLineAsync();
                        }
                    }
                }
            }
            //Console.WriteLine();
        }

        static async Task Main(string[] args)
        {
            string dataDir = @"..\..\..\..\..\Data\";
            await CheckAllLinks();
            var anchors = await GetAllAnchorsAsync();
            foreach (var anchor in anchors)
            {
                await SearchAsync(anchor);
            }
            return;
            var mdVO = await LoadStringAsync(dataDir + "monsters_vo.md");
            var mdVF = await LoadStringAsync(dataDir + "monsters_hd.md");

            //var regex = new Regex("# (?<namevo>.*?)\n- NameVO: \\[(?<namevf>.*?)\\]\n");
            //var matches = regex.Matches(mdVO);
            //foreach(Match match in matches)
            //{
            //    var nameVF = match.Groups["namevf"].Value;
            //    var nameVO = match.Groups["namevo"].Value;
            //    var replaceOld = string.Format("# {0}\n", nameVF);
            //    var replaceNew = string.Format("# {0}\n- NameVO: [{1}](monsters_vo.md#{2})\n", nameVF, nameVO, Helpers.IdFromName(nameVO));
            //    mdVF = mdVF.Replace(replaceOld, replaceNew);
            //}

            var regex = new Regex("_\\[(?<name>.*?)\\]_");
            var matches = regex.Matches(mdVF);
            var names = new List<string>();
            foreach (Match match in matches)
            {
                var name = match.Groups["name"].Value;
                if (!mdVF.Contains($"[{name}]:"))
                {
                    //Console.WriteLine(name);
                    names.Add(name);
                }
            }
            //names.Sort();
            names = names.OrderBy(n => n).Distinct().ToList();
            foreach (var name in names)
            {
                Console.WriteLine($"[{name}]: spells_hd.md#{Helpers.IdFromName(Helpers.Capitalize(name))}");
            }

            Console.WriteLine(mdVF);
            //await SaveStringAsync(dataDir + "monsters_hd_tmp.md", mdVF);

            return;

        }

        public static async Task CheckAllLinks()
        {
            // string dataDir = @"..\..\..\..\..\Data\";

            var allmds = new Dictionary<string, string>();
            var allanchors = new Dictionary<string, IEnumerable<string>>();
            var alllinks = new Dictionary<string, IEnumerable<Tuple<string, string>>>();
            var allnames = new Dictionary<string, IEnumerable<string>>();
            var resnames = Helpers.GetResourceNames();
            foreach (var resname in resnames)
            {
                if (resname.EndsWith(".md"))
                {
                    var name = resname.Substring(15, resname.Length - 18);
                    var md = await Helpers.GetResourceStringAsync(resname);
                    allmds.Add(name, md);
                    var anchors = GetMarkdownAnchors(md).ToList();
                    allanchors.Add(name, anchors);
                    var links = GetMarkdownLinks(md).ToList();
                    alllinks.Add(name, links);
                    var names = GetMarkdownAnchorNames(md).ToList();
                    allnames.Add(name, names);
                    var unlinkedrefs = GetMarkdownUnlinkedRefs(md).ToList();
                    foreach(var unlinkedref in unlinkedrefs)
                    {
                        Console.WriteLine($"{name} {unlinkedref}");
                    }
                }
            }

            foreach (var links in alllinks)
            {
                foreach (var link in links.Value)
                {
                    var file = link.Item1;
                    var anchor = link.Item2;
                    if (!allanchors[file].Contains(anchor))
                    {
                        Console.WriteLine($"{links.Key} => {file} {anchor}");
                    }
                }
            }

            //foreach (var names in allnames)
            //{
            //    foreach (var name in names.Value)
            //    {
            //        foreach(var mdkv in allmds)
            //        {
            //            FindName(mdkv.Value, name);
            //        }
            //    }
            //}
        }

        public static void FindName(string md, string name)
        {
            var index = md.IndexOf(name);
            while (index >= 0)
            {
                if ((md.Substring(index - 1, 1) != "[" ||
                    md.Substring(index + name.Length, 1) != "]") &&
                    md.Substring(index - 1, 1) != "#" &&
                    md.Substring(index - 2, 2) != "# ")
                {
                    Console.WriteLine(name);
                    Console.WriteLine(md.Substring(index - 10, name.Length + 20).Replace("\n", "\\n"));
                    Console.WriteLine();
                }
                index = md.IndexOf(name, index + 1);
            }
        }

        public static IEnumerable<Tuple<string, string>> GetMarkdownLinks(string md)
        {
            var regex = new Regex("[ \\(](?<file>[a-z_]*?)\\.md#(?<anchor>.*?)[\\n\\)]");
            var matches = regex.Matches(md);
            foreach (Match match in matches)
            {
                var file = match.Groups["file"].Value;
                var anchor = match.Groups["anchor"].Value;
                yield return new Tuple<string, string>(file, anchor);
            }
        }

        public static IEnumerable<string> GetMarkdownUnlinkedRefs(string md)
        {
            var regex = new Regex("\\[(?<ref>.+?)\\]", RegexOptions.IgnoreCase);
            var matches = regex.Matches(md);
            md = md.ToLower();
            foreach (Match match in matches)
            {
                var rref = match.Groups["ref"].Value;
                var lref = rref.ToLower();
                if (!md.Contains($"[{lref}]:") &&
                    !md.Contains($"[{lref}](") &&
                    !lref.Contains("]"))
                {
                    yield return rref;
                }
            }
        }

        public static IEnumerable<string> GetMarkdownAnchors(string md)
        {
            foreach (var name in GetMarkdownAnchorNames(md))
            {
                yield return Helpers.IdFromName(name);
            }
        }

        public static IEnumerable<string> GetMarkdownAnchorNames(string md)
        {
            var regex = new Regex($"\\n##? (?<name>.*?)\\n");
            var matches = regex.Matches(md);
            foreach (Match match in matches)
            {
                var name = match.Groups["name"].Value;
                yield return name;
            }
        }

        public static string Capitalize(string text)
        {
            return string.Concat(text.Take(1)).ToUpper() + string.Concat(text.Skip(1)).ToString().ToLower();
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        static string IdFromName(string name)
        {
            return RemoveDiacritics(name.ToLower().Replace(" ", "-"));
        }

        private static T LoadJSon<T>(string filename) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                return serializer.ReadObject(stream) as T;
            }
        }

        private static void SaveJSon<T>(string filename, T objT) where T : class
        {
            var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, true, true, "  "))
                {
                    serializer.WriteObject(writer, objT);
                }
            }
        }

        private static async Task SaveStringAsync(string filename, string text)
        {
            using (var sw = new StreamWriter(path: filename, append: false, encoding: Encoding.UTF8))
            {
                await sw.WriteAsync(text);
            }
        }

        private static async Task<string> LoadStringAsync(string filename)
        {
            using (var sr = new StreamReader(filename, Encoding.UTF8))
            {
                return await sr.ReadToEndAsync();
            }
        }

        private static async Task<IEnumerable<string>> LoadList(string filename)
        {
            using (var stream = new StreamReader(filename))
            {
                var lines = new List<string>();
                var line = await stream.ReadLineAsync();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line);
                    }
                    line = await stream.ReadLineAsync();
                }
                return lines;
            }
        }
    }
}
