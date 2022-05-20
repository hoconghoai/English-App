using English_App.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace English_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Sentence> Sentences;
        public MainPage()
        {
            this.InitializeComponent();
            Sentences = new ObservableCollection<Sentence>();
            this.DataContext = this;
        }

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Sentences.Clear();
                string[] st = txtText.Text.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in st)
                {
                    Sentences.Add(new Sentence(s.Trim()));
                }

                new Thread(new ThreadStart(() =>
                {
                    foreach (var sentence in Sentences)
                    {
                        Translate(sentence.Text, sentence);
                        mainEvent.WaitOne();
                    }
                })).Start();
            }
            catch (Exception)
            {
                ShowDialog();
            }
        }

        private async void ShowDialog(string mess = "Lỗi xử lý")
        {
            var md = new MessageDialog(mess);
            md.Commands.Add(new UICommand("Close"));
            await md.ShowAsync();
        }
        private AutoResetEvent mainEvent = new AutoResetEvent(false);
        private async void Translate(string text, Sentence sentence)
        {
            try
            {
                string url = string.Format("https://translate.google.com/?sl=auto&tl=vi&text={0}&op=translate", HttpUtility.UrlEncode(text));
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    sentence.IsTranslating = true;
                    webView.Navigate(new Uri(url));
                });

                bool isSuccess = false;
                AutoResetEvent are = new AutoResetEvent(false);
                while (!isSuccess)
                {
                    are.WaitOne(1000);
                    if (isSuccess)
                    {
                        break;
                    }
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        try
                        {
                            var html = await webView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
                            Regex regex = new Regex("<span class=\"Q4iAWc\"(?:(?!>).)*>(?<content>((?!<\\/span>).)*)<\\/span>");
                            Match match = regex.Match(html);
                            if (match.Success)
                            {
                                sentence.VNText = match.Groups["content"].Value;
                                sentence.IsTranslating = false;
                                isSuccess = true;
                            }
                        }
                        catch (Exception)
                        {
                            //
                        }
                        finally
                        {
                            are.Set();
                        }
                    });
                }
                mainEvent.Set();
            }
            catch (Exception ex)
            {
                //
            }
        }

        private void log(string mess)
        { 
            Debug.WriteLine("{0}, {1}, {2}", DateTime.Now.ToString(), Thread.CurrentThread.ManagedThreadId.ToString(), mess);
        }

        //private void Translate(string text, Sentence sentence)
        //{
        //    Translation translation = new Translation();
        //    try
        //    {
        //        string url = string.Format("https://translate.google.com/?sl=auto&tl=vi&text={0}&op=translate", HttpUtility.UrlEncode(text));
        //        WebView webView = new WebView();
        //        wv.Children.Add(webView);
        //        webView.Navigate(new Uri(url));
        //        webView.LoadCompleted += (sender, e) =>
        //        {
        //            new Thread(new ThreadStart(async () =>
        //            {
        //                AutoResetEvent are = new AutoResetEvent(false);
        //                bool isSuccess = false;
        //                while (true)
        //                {
        //                    are.WaitOne(100);

        //                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
        //                    {
        //                        var html = await webView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
        //                        Regex regex = new Regex("<span class=\"Q4iAWc\"(?:(?!>).)*>(?<content>((?!<\\/span>).)*)<\\/span>");
        //                        Match match = regex.Match(html);
        //                        if (match.Success)
        //                        {
        //                            sentence.VNText = match.Groups["content"].Value;
        //                            sentence.IsTranslating = false;
        //                            isSuccess = true;
        //                        }
        //                        are.Set();
        //                    });

        //                    are.WaitOne();
        //                    if (isSuccess)
        //                    {
        //                        break;
        //                    }
        //                }
        //            })).Start();
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        //
        //    }
        //}
    }
}
