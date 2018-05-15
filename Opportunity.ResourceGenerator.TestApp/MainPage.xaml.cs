using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Opportunity.ResourceGenerator.TestApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            var r = ResourceLoader.GetForCurrentView();
            var dd = r.GetStringForUri(new Uri("ms-resource:ContentTextBox/ToolTipService/ToolTip"));
            var data = new { a = Math.PI, bs = "haha" };
            var dic = new Dictionary<string, double> { ["Count"] = Math.PI, ["v"] = 12 };
            var aa = new FormattableResourceString("Count: {Count} {_syncRoot} {buckets}").ToFormattableString(dic);
            var aao = new FormattableResourceString("Count: {Count} {_syncRoot} {buckets}").ToFormattableString((object)dic);
            var bb = new FormattableResourceString("{$FileNotFound}").ToFormattableString(Strings.Resources);
            var d = Test.Strings.Resources.FileNotFound().Format(new { line = 12, name = "Test.cs", path = "??" });
            dynamic resources = Test.Strings.Resources;
            Test.Strings.Resources.FileNotFound(1, 2, 3);
            string tooltip1 = (string)resources.ContentTextBox.ToolTipService.ToolTip();
            string tooltip2 = (string)resources.ContentTextBox["ToolTipService"].ToolTip();
            string tooltip3 = (string)resources.ContentTextBox["ToolTipService/ToolTip"]();
            FormattableString f = $"A = {1}";
            this.InitializeComponent();
            dynamic s0 = Strings.Resource;
            var ff = s0.FormatJson("P", null);
            var s1 = s0.GetValue;
            var s2t = s1.GetValue;
            var s2a = s2t();
            var s2 = s1.GetValue();
            var rd = ResourceLoader.GetForViewIndependentUse();
            var r1 = ResourceLoader.GetForViewIndependentUse("");
            var p = new DebuggerDisplay(Strings.Resource);
            this.lv.ItemsSource = p.Items;
            var ns1 = a.a1.ToDisplayNameString();
            var ns2 = a.a2.ToDisplayNameString();
            var ns3 = a.a3.ToDisplayNameString();
            var ns4 = a.a4.ToDisplayNameString();
            Debugger.Break();
        }
    }

    public enum a
    {
        [EnumDisplayName("a1disp")]
        a1,
        a2,
        [EnumDisplayName("ms-resource:AppName")]
        a3,
        [EnumDisplayName("ms-resource:///Resource/Plus+Test")]
        a4
    }
}
