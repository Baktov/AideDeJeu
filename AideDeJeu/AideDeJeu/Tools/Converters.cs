﻿using AideDeJeu.ViewModels;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AideDeJeu.Tools
{
    public class NullToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NullToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class HtmlNodeToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (str != null)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(str);

                var fs = new FormattedString();
                FormatedTextHelpers.HtmlNodeToFormatedString(doc.DocumentNode, fs);
                return fs;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class HtmlNodesToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strings = value as IEnumerable<string>;
            if (strings != null)
            {
                var fs = new FormattedString();
                foreach (var str in strings)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(str);

                    FormatedTextHelpers.HtmlNodeToFormatedString(doc.DocumentNode, fs);
                    fs.Spans.Add(new Span() { Text = "\r\n" });
                }
                return fs;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ItemsTypeTemplateConverter : IValueConverter
    {
        public ControlTemplate SpellsTemplate { get; set; }
        public ControlTemplate MonstersTemplate { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itemType = value as ItemType?;
            if (itemType == ItemType.Spell)
            {
                return SpellsTemplate;
            }
            if (itemType == ItemType.Monster)
            {
                return MonstersTemplate;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ItemTypeConverter<T> : IValueConverter
    {
        public T Spells { get; set; }
        public T Monsters { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itemType = value as ItemType?;
            if (itemType == ItemType.Spell)
            {
                return Spells;
            }
            if (itemType == ItemType.Monster)
            {
                return Monsters;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ItemTypeToStringConverter : ItemTypeConverter<string> { }


    public class ItemSourceTypeConverter<T> : IValueConverter
    {
        public T SpellVF { get; set; }
        public T SpellVO { get; set; }
        public T SpellHD { get; set; }
        public T MonsterVF { get; set; }
        public T MonsterVO { get; set; }
        public T MonsterHD { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var itemType = value as ItemSourceType?;
            //if (itemType == ItemSourceType.SpellVF)
            //{
            //    return SpellVF;
            //}
            //if (itemType == ItemSourceType.SpellVO)
            //{
            //    return SpellVO;
            //}
            //if (itemType == ItemSourceType.SpellHD)
            //{
            //    return SpellHD;
            //}
            //if (itemType == ItemSourceType.MonsterVF)
            //{
            //    return MonsterVF;
            //}
            //if (itemType == ItemSourceType.MonsterVO)
            //{
            //    return MonsterVO;
            //}
            //if (itemType == ItemSourceType.MonsterHD)
            //{
            //    return MonsterHD;
            //}
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ItemSourceTypeToItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = DependencyService.Get<MainViewModel>();
            var itemSourceType = vm.ItemSourceType;
            return vm.GetItemsViewModel(itemSourceType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ItemSourceTypeToFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = DependencyService.Get<MainViewModel>();
            var itemSourceType = vm.ItemSourceType;
            return vm.GetFilterViewModel(itemSourceType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
