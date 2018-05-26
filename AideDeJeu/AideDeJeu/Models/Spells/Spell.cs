﻿using HtmlAgilityPack;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace AideDeJeuLib.Spells
{
    public class Spell : Item
    {
        public string LevelType { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public string Concentration { get; set; }
        public string Rituel { get; set; }
        public string CastingTime { get; set; }
        public string Range { get; set; }
        public string Components { get; set; }
        public string Duration { get; set; }
        public string DescriptionHtml { get; set; }
        public string DescriptionText
        {
            get
            {
                return DescriptionDiv?.InnerText?.Replace("\n", "\r\n\r\n");
            }
        }
        [IgnoreDataMember]
        public HtmlNode DescriptionDiv
        {
            get
            {
                if(DescriptionHtml != null)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(DescriptionHtml);
                    return doc.DocumentNode;
                }
                return null;
            }
            set
            {
                DescriptionHtml = value?.OuterHtml;
            }
        }

        public string Overflow { get; set; }
        public string NoOverflow { get; set; }
        public string Source { get; set; }

        public void ParseHtml()
        {
            var pack = new HtmlDocument();
            pack.LoadHtml(this.Html);
            var divSpell = pack.DocumentNode.SelectNodes("//div[contains(@class,'bloc')]").FirstOrDefault();
            ParseNode(divSpell);
        }

        public void ParseNode(HtmlNode nodeSpell)
        {
            this.Name = nodeSpell.SelectSingleNode("h1").InnerText;
            var divTrad = nodeSpell.SelectSingleNode("div[@class='trad']");
            
            var linkVO = divTrad.SelectSingleNode("a").GetAttributeValue("href", "");
            var matchIdVF = new Regex(@"\?vf=(?<idvf>.*)").Match(linkVO);
            this.IdVF = matchIdVF?.Groups["idvf"]?.Value;
            var matchIdVO = new Regex(@"\?vo=(?<idvo>.*)").Match(linkVO);
            this.IdVO = matchIdVO?.Groups["idvo"]?.Value;
            var altNames = divTrad?.InnerText;
            if (altNames != null)
            {
                var matchNames = new Regex(@"\[ (?<vo>.*?) \](?: \[ (?<alt>.*?) \])?").Match(altNames);
                this.NameVO = matchNames.Groups["vo"].Value;
                this.NamePHB = string.IsNullOrEmpty(matchNames.Groups["alt"].Value) ? this.Name : matchNames.Groups["alt"].Value;
            }
            else
            {
                this.NamePHB = this.Name;
            }
            this.LevelType = nodeSpell.SelectSingleNode("h2/em").InnerText;
            this.Level = this.LevelType.Split(new string[] { " - " }, StringSplitOptions.None)[0].Split(' ')[1];
            this.Type = this.LevelType.Split(new string[] { " - " }, StringSplitOptions.None)[1];
            this.CastingTime = nodeSpell.SelectSingleNode("div[@class='paragraphe']").InnerText.Split(new string[] { ": " }, StringSplitOptions.None)[1];
            this.Range = nodeSpell.SelectSingleNode("div[strong/text()='Portée' or strong/text()='Range']").InnerText.Split(new string[] { ": " }, StringSplitOptions.None)[1];
            this.Components = nodeSpell.SelectSingleNode("div[strong/text()='Composantes' or strong/text()='Components']")?.InnerText?.Split(new string[] { ": " }, StringSplitOptions.None)?[1];
            this.Duration = nodeSpell.SelectSingleNode("div[strong/text()='Durée' or strong/text()='Duration']").InnerText.Split(new string[] { ": " }, StringSplitOptions.None)[1];
            this.DescriptionDiv = nodeSpell.SelectSingleNode("div[contains(@class,'description')]");
            this.Overflow = nodeSpell.SelectSingleNode("div[@class='overflow']")?.InnerText;
            this.NoOverflow = nodeSpell.SelectSingleNode("div[@class='nooverflow']")?.InnerText;
            this.Source = nodeSpell.SelectSingleNode("div[@class='source']").InnerText;
        }

        public static Spell FromHtml(HtmlNode nodeSpell)
        {
            var spell = new Spell();
            spell.Html = nodeSpell.OuterHtml;
            spell.ParseNode(nodeSpell);
            return spell;
        }
    }
}