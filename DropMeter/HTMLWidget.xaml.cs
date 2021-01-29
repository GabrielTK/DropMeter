﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp;

namespace DropMeter
{
    /// <summary>
    /// Interaction logic for HTMLWidget.xaml
    /// </summary>
    public partial class HTMLWidget : Window
    {
        /************ win32 interop stuff ****************/
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);


        private bool attachToDesktop = true;

        const int GWL_HWNDPARENT = -8;

        public HTMLWidget(bool attachToDesktop = true)
        {
            this.attachToDesktop = attachToDesktop;
            InitializeComponent();
            WebView.MenuHandler = new CloseMenuHandler(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Desktop Widget Inner Gears
            if (attachToDesktop)
            {
                

                

                
                //Init();

                // The window is set to the parent, and the parent window of the background window is set to the Program Manager window.
                IntPtr hwnd2 = new WindowInteropHelper(this).Handle;
                //W32.SetParent(hwnd2, programIntPtr);


                //IntPtr pWnd = FindWindow("Progman", null);
                //pWnd = FindWindowEx(pWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                //pWnd = FindWindowEx(pWnd, IntPtr.Zero, "SysListView32", null);
                //IntPtr tWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                //SetParent(tWnd, pWnd);
                W32.EnumWindows((hwnd, lParam) =>
                {
                    var shell = W32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", (IntPtr) null);
                    // Find the WorkerW that contains the SHELLDLL_DefView window handle
                    if (shell != IntPtr.Zero)
                    {
                        //hwnd is WorkerW
                        W32.SetParent(hwnd2, shell);
                        // Find the current WorkerW window, the next WorkerW window. 
                        //IntPtr tempHwnd = W32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", (IntPtr)null);

                        // hide this window
                        //W32.ShowWindow(tempHwnd, 0);
                    }

                    return true;
                }, IntPtr.Zero);
            }
            #endregion
            EnterWidgetMode(null, null);

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void WebView_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
            this.WebView.LoadHtml(@"<!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

<style>
    html, body {
    background: transparent;
    color: white;
}
</style>
</body>
</html>", "http://demoplugin.int/");
            this.WebView.ShowDevTools();
        }

        private void EnterWidgetMode(object sender, RoutedEventArgs e)
        {
            WidgetMove.Visibility = Visibility.Hidden;
            WebView.Visibility = Visibility.Visible;
        }

        public void EnterMoveMode()
        {
            Dispatcher.Invoke(() =>
            {
                WidgetMove.Visibility = Visibility.Visible;
                WebView.Visibility = Visibility.Hidden;
            });
        }
    }
}
