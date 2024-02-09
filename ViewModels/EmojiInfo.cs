using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Svg.Skia;

namespace FluentuiemojiPreview.ViewModels
{
    public class EmojiInfo
    {
        public string Title { get; set; }
        public Bitmap? Description_PNG { get; set; }
    }
}
