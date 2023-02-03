﻿using Fastedit.Dialogs;
using Fastedit.Settings;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Fastedit.Helper
{
    public class DesignHelper
    {
        public static FasteditDesign CurrentDesign = null;

        public static async Task LoadDesign()
        {
            var designName = AppSettings.GetSettings(AppSettingsValues.Settings_DesignName, DefaultValues.DefaultDesignName);
            string path = Path.Combine(DefaultValues.DesignPath, designName);

            if (File.Exists(path))
            {
                CurrentDesign = GetDesignFromFile(path);
            }
            //When the design could not get loaded, load alternative design:
            if (CurrentDesign == null)
            {
                CurrentDesign = await LoadDefaultDesign();

                //When the design still could not get loaded, load some data
                if (CurrentDesign == null)
                {
                    CurrentDesign = new FasteditDesign
                    {
                        BackgroundColor = Color.FromArgb(100, 0, 0, 0),
                        BackgroundType = BackgroundType.Solid,
                        CursorColor = Color.FromArgb(255, 255, 255, 255),
                        DialogBackgroundColor = Color.FromArgb(30, 20, 20, 20),
                        DialogBackgroundType = ControlBackgroundType.Acrylic,
                        DialogTextColor = Color.FromArgb(255, 255, 255, 255),
                        LineHighlighterBackground = Color.FromArgb(50, 0, 0, 0),
                        LineNumberBackground = Color.FromArgb(0, 0, 0, 0),
                        LineNumberColor = Color.FromArgb(0, 0, 0, 0),
                        SearchHighlightColor = Color.FromArgb(0, 0, 0, 0),
                        SelectedTabPageHeaderBackground = Color.FromArgb(0, 0, 0, 0),
                        SelectedTabPageHeaderTextColor = Color.FromArgb(0, 0, 0, 0),
                        SelectionColor = Color.FromArgb(0, 0, 0, 0),
                        StatusbarBackgroundColor = Color.FromArgb(0, 0, 0, 0),
                        StatusbarBackgroundType = ControlBackgroundType.Acrylic,
                        StatusbarTextColor = Color.FromArgb(0, 0, 0, 0),
                        TextBoxBackground = Color.FromArgb(0, 0, 0, 0),
                        TextboxBackgroundType = ControlBackgroundType.Acrylic,
                        TextColor = Color.FromArgb(0, 0, 0, 0),
                        Theme = ElementTheme.Dark,
                        UnselectedTabPageHeaderBackground = Color.FromArgb(0, 0, 0, 0),
                        UnSelectedTabPageHeaderTextColor = Color.FromArgb(0, 0, 0, 0),
                    }; // await LoadDefaultDesign();
                }
            }
        }
        public static async Task CopyDefaultDesigns()
        {
            if (AppSettings.GetSettingsAsInt(AppSettingsValues.DesignLoaded) == 0)
            {
                //the designs are not loaded into the folder
                AppSettings.SaveSettings(AppSettingsValues.DesignLoaded, 1);

                try
                {
                    string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                    StorageFolder sourceFolder = await StorageFolder.GetFolderFromPathAsync($"{root}\\Assets\\Designs");
                    StorageFolder destinationFolder = await StorageFolder.GetFolderFromPathAsync(DefaultValues.DesignPath);

                    var files = await sourceFolder.GetFilesAsync();
                    foreach (var file in files)
                    {

                        if (file != null)
                        {
                            await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        }
                    }
                }
                catch
                {
                    AppSettings.SaveSettings(AppSettingsValues.DesignLoaded, 0);
                }
            }
        }

        public static string GetDesingNameFromPath(string path)
        {
            return Path.GetFileName(path);
        }
        public static FasteditDesign GetDesignFromFile(string path)
        {
            if (Path.GetExtension(path).Equals(".json") && File.Exists(path))
            {
                try
                {
                    return JsonConvert.DeserializeObject<FasteditDesign>(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    InfoMessages.DesignLoadError(GetDesingNameFromPath(path), ex);
                }
            }
            return null;
        }

        public static async Task<FasteditDesign> LoadDefaultDesign()
        {
            string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            var file = await StorageFile.GetFileFromPathAsync($"{root}\\Assets\\Designs" + DefaultValues.DefaultDesignName);

            return JsonConvert.DeserializeObject<FasteditDesign>(await FileIO.ReadTextAsync(file));
        }

        public static void SetBackground(Control element, Color color, BackgroundType type)
        {
            if (element == null)
                return;

            //remove mica
            if (BackdropMaterial.GetApplyToRootOrPageBackground(element) && type != BackgroundType.Mica)
                BackdropMaterial.SetApplyToRootOrPageBackground(element, false);

            int transparency = color.A;
            color.A = 255;
            if (type == BackgroundType.Null)
            {
                element.Background = null;
            }
            else if (type == BackgroundType.Acrylic)
            {
                element.Background = new Windows.UI.Xaml.Media.AcrylicBrush
                {
                    TintColor = color,
                    TintOpacity = transparency / 255.0,
                    FallbackColor = color,
                    BackgroundSource = Windows.UI.Xaml.Media.AcrylicBackgroundSource.HostBackdrop,
                };
            }
            else if (type == BackgroundType.Solid)
            {
                element.Background = new SolidColorBrush { Color = color };
            }
            else if (type == BackgroundType.Mica)
            {
                BackdropMaterial.SetApplyToRootOrPageBackground(element, true);
            }
        }
        public static Brush CreateBackgroundBrush(Color color, ControlBackgroundType type)
        {
            int transparency = color.A;
            color.A = 255;

            if (type == ControlBackgroundType.Acrylic)
            {
                return new Windows.UI.Xaml.Media.AcrylicBrush
                {
                    TintColor = color,
                    TintOpacity = transparency / 255.0,
                    FallbackColor = color,
                    BackgroundSource = Windows.UI.Xaml.Media.AcrylicBackgroundSource.Backdrop,
                };
            }
            else if (type == ControlBackgroundType.Solid)
            {
                return new SolidColorBrush { Color = color };
            }
            return null;
        }
    }
    public class FasteditDesign
    {
        public ElementTheme Theme { get; set; }
        public Color? BackgroundColor { get; set; }
        public Color? TextColor { get; set; }
        public Color? SelectionColor { get; set; }
        public BackgroundType BackgroundType { get; set; }
        public Color? LineNumberColor { get; set; }
        public Color? LineNumberBackground { get; set; }
        public Color? LineHighlighterBackground { get; set; }
        public Color? TextBoxBackground { get; set; }
        public Color? CursorColor { get; set; }
        public Color? SearchHighlightColor { get; set; }
        public ControlBackgroundType TextboxBackgroundType { get; set; }
        public Color? SelectedTabPageHeaderTextColor { get; set; }
        public Color? UnSelectedTabPageHeaderTextColor { get; set; }
        public Color? UnselectedTabPageHeaderBackground { get; set; }
        public Color? SelectedTabPageHeaderBackground { get; set; }
        public Color? StatusbarTextColor { get; set; }
        public ControlBackgroundType StatusbarBackgroundType { get; set; }
        public Color? StatusbarBackgroundColor { get; set; }
        public Color? DialogBackgroundColor { get; set; }
        public Color? DialogTextColor { get; set; }
        public ControlBackgroundType DialogBackgroundType { get; set; }
    }
    public enum BackgroundType
    {
        Acrylic, Solid, Mica, Null
    }
    public enum ControlBackgroundType
    {
        Acrylic, Solid, Null
    }
}
