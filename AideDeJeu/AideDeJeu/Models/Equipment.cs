﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AideDeJeu.Tools;
using Markdig.Syntax;

namespace AideDeJeuLib
{
    public class Equipment : Item
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }
        public string ArmorClass { get; set; }
        public string Discretion { get; set; }
        public string Weight { get; set; }
        public string Force { get; set; }
        public string Rarity { get; set; }
        public string Damages { get; set; }
        public string Properties { get; set; }
        public string Unity { get; set; }
        public string Capacity { get; set; }
        public string WeightCapacity { get; set; }
        public string Speed { get; set; }

        public override string Markdown
        {
            get
            {
                return
                    //$"# {Name}\n\n" +
                    //$"{AltName}\n\n" +
                    Text;
            }
        }

        public void ParseBlock(Block block)
        {
            if (block is HeadingBlock)
            {
                var headingBlock = block as HeadingBlock;
                if (this.Name == null)
                {
                    this.Name = headingBlock.Inline.ToMarkdownString();
                }
                this.Text += block.ToMarkdownString();
            }
            else if (block is ListBlock)
            {
                var listBlock = block as ListBlock;
                if (listBlock.BulletType == '-')
                {
                    foreach (var inblock in listBlock)
                    {
                        if (inblock is Markdig.Syntax.ListItemBlock)
                        {
                            var listItemBlock = inblock as Markdig.Syntax.ListItemBlock;
                            foreach (var ininblock in listItemBlock)
                            {
                                if (ininblock is Markdig.Syntax.ParagraphBlock)
                                {
                                    var paragraphBlock = ininblock as Markdig.Syntax.ParagraphBlock;
                                    var str = paragraphBlock.ToMarkdownString();

                                    var properties = new List<Tuple<string, Action<Equipment, string>>>()
                                    {
                                        new Tuple<string, Action<Equipment, string>>("AltName: ", (m, s) =>
                                        {
                                            this.Text += "- " + s; m.AltName = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Type** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Type = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Prix** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Price = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Classe d'armure** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.ArmorClass = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Discrétion** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Discretion = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Poids** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Weight = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Force** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Force = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Rareté** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Rarity = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Dégâts** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Damages = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Propriétés** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Properties = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Unité** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Unity = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Capacité** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Capacity = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Capacité de charge** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.WeightCapacity = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("**Vitesse** ", (m, s) =>
                                        {
                                            this.Text += "- " + str; m.Speed = s;
                                        }),
                                        new Tuple<string, Action<Equipment, string>>("", (m, s) =>
                                        {
                                            this.Text += str;
                                        }),
                                    };

                                    foreach (var property in properties)
                                    {
                                        if (str.StartsWith(property.Item1))
                                        {
                                            property.Item2.Invoke(this, str.Substring(property.Item1.Length));
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    this.Text += "\n";
                }
                else
                {
                    this.Text += block.ToMarkdownString();
                }
            }
            else
            {
                this.Text += block.ToMarkdownString();
            }
        }

        public override void Parse(ref ContainerBlock.Enumerator enumerator)
        {
            enumerator.MoveNext();
            while (enumerator.Current != null)
            {
                var block = enumerator.Current;
                if (block.IsNewItem())
                {
                    return;
                }
                ParseBlock(block);
                enumerator.MoveNext();

            }
        }
    }
}