using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Svg.Skia;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ShimSkiaSharp;
using SkiaSharp;
using Svg.Skia;
using SearchOption = System.IO.SearchOption;

namespace FluentuiemojiPreview.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string emoji_3d { get; init; } = "3D";
    private string emoji_color { get; init; } = "Color";
    private string emoji_flat { get; init; } = "Flat";
    private string emoji_highContrast { get; init; } = "High Contrast";
    private const int page_emojiCount = 30;
    private readonly string base_folderPath = AppDomain.CurrentDomain.BaseDirectory + "Assets";
    private readonly string[] emojifoder;
    private readonly List<string> emojifiles;
    [Reactive]
    public int SelectIndex { get; set; } = -1;
    [Reactive]
    public int CurrentPage { get; set; } = -1;
    [Reactive]
    public Bitmap CurrentEmoji { get; set; }
    [Reactive]
    public string CurrentEmojiName { get; set; }
    [Reactive]
    public string SerachPattern { get; set; }
    private ObservableCollection<EmojiInfo> _emojiList;

    public ObservableCollection<EmojiInfo> EmojiList
    {
        get { return _emojiList; }
        set { this.RaiseAndSetIfChanged(ref _emojiList, value); }
    }
    public ObservableCollection<EmojiInfo> SearchEmojiList
    {
        get { return _emojiList; }
        set { this.RaiseAndSetIfChanged(ref _emojiList, value); }
    }

    public ReactiveCommand<Unit, Unit> SwitchEmojiCommand { get; set; }
    public ReactiveCommand<Unit, Unit> NextPageCommand { get; set; }
    public ReactiveCommand<Unit, Unit> PreviousPageCommand { get; set; }
    public ReactiveCommand<Unit, Unit> GotoInitialPageCommand { get; set; }
    public ReactiveCommand<Unit, Unit> GotoLastPageCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SerachEmojiNameCommand { get; set; }


    public MainViewModel()
    {
        EmojiList = [];
        emojifoder = Directory.GetDirectories(base_folderPath, $"*", SearchOption.AllDirectories);
        emojifiles = [];
        for (int i = 0; i < emojifoder.Length; i++)
        {
            string sub_folderPath = $"{emojifoder[i]}\\3D";
            if (Directory.Exists(sub_folderPath))
            {
                var files = Directory.GetFiles(sub_folderPath, $"*.png", SearchOption.AllDirectories);
                emojifiles.Add(files[0]);
            }
        }
        SwitchEmojiCommand = ReactiveCommand.Create(SwitchEmoji);
        NextPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage + 1 > emojifiles.Count / page_emojiCount) return;
            CurrentPage += 1;
            RefreshEmojiList(CurrentPage);
        });
        PreviousPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage - 1 < 0) return;
            CurrentPage -= 1;
            RefreshEmojiList(CurrentPage);
        });
        GotoInitialPageCommand = ReactiveCommand.Create(() =>
        {
            CurrentPage = 0;
            RefreshEmojiList(CurrentPage);
        });
        GotoLastPageCommand = ReactiveCommand.Create(() =>
        {
            CurrentPage = emojifiles.Count / page_emojiCount;
            RefreshEmojiList(CurrentPage);
        });
        SerachEmojiNameCommand = ReactiveCommand.Create(() =>
        {
            RefreshEmojiList(SerachPattern);
        });
        CurrentPage = 0;
        RefreshEmojiList(CurrentPage);
    }
    //string baseurl = Directory.GetCurrentDirectory();
    //var imagepath = System.IO.Path.Combine(baseurl, @"assets\1st place medal\Color\1st_place_medal_color.svg");
    //using (var svg = new SKSvg())
    //{
    //    if (svg.Load(imagepath) is { })
    //    {
    //        using var stream = File.OpenWrite(System.IO.Path.Combine(baseurl, @"assets\1st place medal\Color\1st_place_medal_color.png"));
    //        svg.Picture.ToImage(stream, SKColors.Empty,
    //            SKEncodedImageFormat.Png,
    //            80, 5f, 5f,
    //            SKColorType.RgbaF16, SkiaSharp.SKAlphaType.Opaque, SkiaSharp.SKColorSpace.CreateSrgb());
    //    }
    //}
    private void SwitchEmoji()
    {
        if (SelectIndex < 0) return;
        int index = CurrentPage * page_emojiCount;
        CurrentEmojiName = Path.GetFileName(emojifiles[SelectIndex + index]);
        CurrentEmoji = new Bitmap(emojifiles[SelectIndex + index]);
    }
    private void RefreshEmojiList(int pagecount)
    {
        EmojiList.Clear();
        Task.Run(() =>
        {
            if (CurrentPage < 0) return;
            int index = page_emojiCount * pagecount;
            for (int i = 0; i < page_emojiCount; i++)
            {
                if (i + index < emojifiles.Count)
                {
                    EmojiList.Add(new EmojiInfo
                    {
                        Title = Path.GetFileName(emojifiles[i + index]),
                        Description_PNG = new Bitmap(emojifiles[i + index])
                    });
                }
            }
        });
    }

    private void RefreshEmojiList(string pattern)
    {
        SearchEmojiList.Clear();
        if (pattern == null || pattern == string.Empty || pattern?.Replace(" ", "") == string.Empty) return;
        Task.Run(() =>
        {
            var serachEmoji = emojifiles.Where(p => p.Contains(pattern)).ToList();
            if (serachEmoji.Count < 0) return;
            for (int i = 0; i < serachEmoji.Count; i++)
            {
                SearchEmojiList.Add(new EmojiInfo
                {
                    Title = Path.GetFileName(serachEmoji[i]),
                    Description_PNG = new Bitmap(serachEmoji[i])
                });
            }
        });
    }

}
