using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
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
            this.InitializeComponent();
            var s = Strings.Resource["GetValue"]["GetValue"];
            var rd = ResourceLoader.GetForViewIndependentUse();
            var r1 = ResourceLoader.GetForViewIndependentUse("");
            var p = new DebuggerDisplay(Strings.Resource);
            //this.lv.ItemsSource = p.Items;
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
