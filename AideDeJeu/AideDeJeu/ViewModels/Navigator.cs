﻿using AideDeJeu.Pdf;
using AideDeJeu.Tools;
using AideDeJeu.ViewModels.Library;
using AideDeJeu.Views;
using AideDeJeu.Views.Library;
using AideDeJeu.Views.PlayerCharacter;
using AideDeJeuLib;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AideDeJeu.ViewModels
{
    public interface INavigator
    {
        Task GotoAboutPageAsync();
        Task GotoItemDetailPageAsync(Item item);
    }
    public class Navigator : BaseViewModel, INavigator
    {
        public INavigation Navigation;

        public Navigator(INavigation navigation)
        {
            Navigation = navigation;
        }

        private Command _AboutCommand = null;
        public Command AboutCommand
        {
            get
            {
                return _AboutCommand ?? (_AboutCommand = new Command(async () => await GotoAboutPageAsync()));
            }
        }

        private ICommand _OpenWebCommand = null;
        public ICommand OpenWebCommand 
        { 
            get
            {
                return _OpenWebCommand ?? (_OpenWebCommand = new Command<string>((param) => Device.OpenUri(new Uri(param))));
            }
        }

        public async Task GotoAboutPageAsync()
        {
            //await Navigation.PushAsync(new Views.AboutPage(), true);
            await Shell.Current.GoToAsync("//about", true);
        }

        private Command _LibraryCommand = null;
        public Command LibraryCommand
        {
            get
            {
                return _LibraryCommand ?? (_LibraryCommand = new Command(async () => await GotoLibraryPageAsync()));
            }
        }

        //private NotifyTaskCompletion<bool> _TestNotify = null;
        //public NotifyTaskCompletion<bool> TestNotify
        //{
        //    get
        //    {
        //        return _TestNotify;
        //    }
        //    set
        //    {
        //        SetProperty(ref _TestNotify, value);
        //    }
        //}

        //public async Task<bool> TestGotoAsync()
        //{
        //    var page = new Views.MainTabbedPage();
        //    Device.BeginInvokeOnMainThread(async() => 
        //    await Navigation.PushAsync(page, true));
        //    return true;
        //}
        public async Task GotoLibraryPageAsync()
        {
            //await Navigation.PushAsync(new Views.ItemDetailPage(),true);
            //await Navigation.PushAsync(new Views.Library.MainTabbedPage(), true);
            //await Navigation.PushAsync(new Views.Library.ItemPage("index.md"), true);
            await Shell.Current.GoToAsync("//library", true);
            //TestNotify = new NotifyTaskCompletion<bool>(TestGotoAsync(), true);
        }

        private Command _BookmarksCommand = null;
        public Command BookmarksCommand
        {
            get
            {
                return _BookmarksCommand ?? (_BookmarksCommand = new Command(async () => await GotoBookmarksPageAsync()));
            }
        }

        public async Task GotoBookmarksPageAsync()
        {
            //await Navigation.PushAsync(new Views.Library.BookmarksPage(), true);
            await Shell.Current.GoToAsync("//bookmarks", true);
        }



        private Command _PlayerCharacterEditorCommand = null;
        public Command PlayerCharacterEditorCommand
        {
            get
            {
                return _PlayerCharacterEditorCommand ?? (_PlayerCharacterEditorCommand = new Command(async () => await GotoPlayerCharacterEditorPageAsync()));
            }
        }

        private Command _DicesCommand = null;
        public Command DicesCommand
        {
            get
            {
                return _DicesCommand ?? (_DicesCommand = new Command(async () => await GotoDicesPageAsync()));
            }
        }

        public async Task GotoPlayerCharacterEditorPageAsync()
        {
            //await Navigation.PushAsync(new Views.PlayerCharacter.PlayerCharacterEditorPage(), true);
            await Shell.Current.GoToAsync("//pceditor", true);
        }

        public async Task GotoDicesPageAsync()
        {
            //await Navigation.PushAsync(new Views.DicesPage(), true);
            await Shell.Current.GoToAsync("//dices", true);
        }

        private Command _DeepSearchCommand = null;
        public Command DeepSearchCommand
        {
            get
            {
                return _DeepSearchCommand ?? (_DeepSearchCommand = new Command(async () => await GotoDeepSearchPageAsync()));
            }
        }

        public async Task GotoDeepSearchPageAsync()
        {
            //await Navigation.PushAsync(new Views.Library.DeepSearchPage(), true);
            await Shell.Current.GoToAsync("//deepsearch", true);
        }

        private Command<ItemViewModel> _AddToFavoritesCommand = null;
        public Command<ItemViewModel> AddToFavoritesCommand
        {
            get
            {
                return _AddToFavoritesCommand ?? (_AddToFavoritesCommand = new Command<ItemViewModel>(async (item) => await ExecuteAddToFavoritesCommandAsync(item)));
            }
        }

        public async Task ExecuteAddToFavoritesCommandAsync(ItemViewModel itemVM)
        {
            //var page = Shell.Current;
            //var tabbedPage = App.Current.MainPage as MainTabbedPage;
            //var navigationPage = tabbedPage; //.MainNavigationPage;
            //var lastPage = navigationPage.Navigation.NavigationStack.LastOrDefault();
            //var context = lastPage.BindingContext;
            //Item item = (context as ItemViewModel)?.Item;
            ////if (context is ItemDetailViewModel)
            ////{
            ////    item = (context as ItemDetailViewModel).Item;
            ////}
            ////else if (context is ItemsViewModel)
            ////{
            ////    item = (context as ItemsViewModel).AllItems;
            ////}

            //await Application.Current.MainPage.DisplayAlert("Id", item.Id, "OK");
            var item = itemVM.Item;
            var repo = Main.Bookmarks;
            var vm = new BookmarksViewModel(); 
            var result = await Application.Current.MainPage.DisplayActionSheet("Ajouter à", "Annuler", "Nouvelle liste", repo.BookmarkCollectionNames.ToArray<string>());
            if (result != "Annuler" && result != null)
            {
                if (result == "Nouvelle liste")
                {
                    int i = 1;
                    while (repo.BookmarkCollectionNames.Contains(result = $"Nouvelle liste ({i})"))
                    {
                        i++;
                    }
                }
                await vm.AddBookmarkAsync(result, item);
            }
        }

        public async Task GotoItemDetailPageAsync(Item item)
        {
            if (item == null)
                return;

            
            var items = item as Item;
            var filterViewModel = items.GetNewFilterViewModel();
            var itemsViewModel = new ItemViewModel() { Item = item, AllItems = items, Filter = filterViewModel };
            itemsViewModel.LoadItemsCommand.Execute(null);

            await SwitchToMainTabAsync();

            if (filterViewModel == null)
            {
                await GotoItemsPageAsync(itemsViewModel);
            }
            else
            {
                await GotoFilteredItemsPageAsync(itemsViewModel);
            }
        }

        public async Task SwitchToMainTabAsync()
        {
            //await Shell.Current.GoToAsync("//library");
            //var tabbedPage = App.Current.MainPage as MainTabbedPage;
            //if (tabbedPage != null)
            //{
            //    tabbedPage.SelectedItem = null;
            //    tabbedPage.SelectedItem = tabbedPage; //.MainNavigationPage;
            //}
        }

        public async Task GotoItemsPageAsync(ItemViewModel itemsVM)
        {
            if (itemsVM == null)
                return;

            await Navigation.PushAsync(new ItemPage(itemsVM), true);
        }

        public async Task GotoFilteredItemsPageAsync(ItemViewModel itemsVM)
        {
            if (itemsVM == null)
                return;

            await Navigation.PushAsync(new ItemPage(itemsVM), true);
        }

        private ICommand _NavigateToLinkCommand = null;
        public ICommand NavigateToLinkCommand
        {
            get
            {
                return _NavigateToLinkCommand ?? (_NavigateToLinkCommand = new Command<string>(async (s) => await NavigateToLinkAsync(s)));
            }
        }

        public async Task NavigateToLinkAsync(string s)
        {
            await SwitchToMainTabAsync();
            await Navigation.PushAsync(new ItemPage(s), true);//.GoToAsync($"item?path={Uri.EscapeDataString(s)}");
            return;
            if (s != null)
            {
                var regex = new Regex("/?(?<file>.*?)(_with_(?<with>.*))?\\.md(#(?<anchor>.*))?");
                var match = regex.Match(s);
                var file = match.Groups["file"].Value;
                var anchor = match.Groups["anchor"].Value;
                var with = match.Groups["with"].Value;
                Item item = null;
                try
                {
                    Main.IsBusy = true;
                    Main.IsLoading = true;
                    item = await Task.Run(async () => await Store.GetItemFromDataAsync(file, anchor));
                }
                finally
                {
                    Main.IsBusy = false;
                    Main.IsLoading = false;
                }
                if (item != null)
                {
                    var items = item; // as Items;
                    var filterViewModel = items.GetNewFilterViewModel();
                    var itemsViewModel = new ItemViewModel() { AllItems = items, Filter = filterViewModel };
                    itemsViewModel.LoadItemsCommand.Execute(null);
                    if (!string.IsNullOrEmpty(with))
                    {
                        var swith = with.Split('_');
                        for (int i = 0; i < swith.Length / 2; i++)
                        {
                            var key = swith[i * 2 + 0];
                            var val = swith[i * 2 + 1];
                            filterViewModel.FilterWith(key, val);
                        }
                    }
                    await SwitchToMainTabAsync();
                    if (filterViewModel == null)
                    {
                        await GotoItemsPageAsync(itemsViewModel);
                    }
                    else
                    {
                        await GotoFilteredItemsPageAsync(itemsViewModel);
                    }

                }
                else
                {
                    //await App.Current.MainPage.DisplayAlert("Lien invalide", s, "OK");
                }
            }
        }



        public enum PopupResultEnum
        {
            Save,
            Cancel,
            Delete
        }
        public async Task<Tuple<string, PopupResultEnum>> OpenCancellableTextInputAlertDialog(string inputText)
        {
            // create the TextInputView
            var inputView = new TextInputCancellableView(
                "Nom de la liste ?", "Nouveau nom...", inputText, "Enregistrer", "Annuler", "Supprimer", "Le nom ne peut pas être vide.");

            // create the Transparent Popup Page
            // of type string since we need a string return
            var popup = new InputAlertDialogBase<Tuple<string, PopupResultEnum>>(inputView);


            // subscribe to the TextInputView's Button click event
            inputView.SaveButtonEventHandler +=
                (sender, obj) =>
                {
                    if (!string.IsNullOrEmpty(((TextInputCancellableView)sender).TextInputResult))
                    {
                        ((TextInputCancellableView)sender).IsValidationLabelVisible = false;
                        popup.PageClosedTaskCompletionSource.SetResult(new Tuple<string, PopupResultEnum>(((TextInputCancellableView)sender).TextInputResult, PopupResultEnum.Save));
                    }
                    else
                    {
                        ((TextInputCancellableView)sender).IsValidationLabelVisible = true;
                    }
                };

            // subscribe to the TextInputView's Button click event
            inputView.CancelButtonEventHandler +=
                (sender, obj) =>
                {
                    popup.PageClosedTaskCompletionSource.SetResult(new Tuple<string, PopupResultEnum>(null, PopupResultEnum.Cancel));
                };

            inputView.DeleteButtonEventHandler +=
                (sender, obj) =>
                {
                    popup.PageClosedTaskCompletionSource.SetResult(new Tuple<string, PopupResultEnum>(null, PopupResultEnum.Delete));
                };

            // Push the page to Navigation Stack
            await PopupNavigation.Instance.PushAsync(popup, true);

            // await for the user to enter the text input
            var result = await popup.PageClosedTask;

            // Pop the page from Navigation Stack
            await PopupNavigation.Instance.PopAsync();

            // return user inserted text value
            return result;
        }

        private Command<string> _GeneratePDFCommand = null;
        public Command<string> GeneratePDFCommand
        {
            get
            {
                return _GeneratePDFCommand ?? (_GeneratePDFCommand = new Command<string>(async (markdown) => await ExecuteGeneratePDFCommandAsync(markdown)));
            }
        }

        public async Task ExecuteGeneratePDFCommandAsync(string markdown)
        {
            var page = new PdfViewPage();
            page.PdfFile = new Tools.NotifyTaskCompletion<string>(Task.Run(async() =>
                { return await PdfService.Instance.MarkdownToPDF(new List<string>() { markdown }); }
            ));
            page.BindingContext = page;
            await Navigation.PushAsync(page, true);

        }

        private Command<IEnumerable<Item>> _GenerateItemsPDFCommand = null;
        public Command<IEnumerable<Item>> GenerateItemsPDFCommand
        {
            get
            {
                return _GenerateItemsPDFCommand ?? (_GenerateItemsPDFCommand = new Command<IEnumerable<Item>>(async (items) => await ExecuteGenerateItemsPDFCommandAsync(items)));
            }
        }

        public async Task ExecuteGenerateItemsPDFCommandAsync(IEnumerable<Item> items)
        {
            var page = new PdfViewPage();
            page.PdfFile = new Tools.NotifyTaskCompletion<string>(Task.Run(async () =>
            {
                return await PdfService.Instance.MarkdownToPDF(items.Select(item => item.Markdown)); }
            ));
            page.BindingContext = page;
            await Navigation.PushAsync(page, true);

        }

    }
}