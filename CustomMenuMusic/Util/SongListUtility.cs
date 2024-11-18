using HMUI;
using IPA.Loader;
using IPA.Utilities;
using SongCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace CustomMenuMusic.Util
{
    public class SongListUtility
    {
        private LevelCollectionViewController _levelCollectionViewController;
        private SelectLevelCategoryViewController _selectLevelCategoryViewController;
        private LevelFilteringNavigationController _levelFilteringNavigationController;
        private AnnotatedBeatmapLevelCollectionsViewController _annotatedBeatmapLevelCollectionsViewController;
        private readonly PluginMetadata _songBrowserMetaData;
        private readonly PluginMetadata _betterSonglistMetaData;
        public static bool SongBrowserPluginPresent { get; private set; }
        public static bool BetterSongListPluginPresent { get; private set; }

        public SongListUtility()
        {
            this._songBrowserMetaData = PluginManager.GetPlugin("Song Browser");
            SongBrowserPluginPresent = this._songBrowserMetaData != null;

            this._betterSonglistMetaData = PluginManager.GetPlugin("BetterSongList");
            BetterSongListPluginPresent = this._betterSonglistMetaData != null;
        }

        [Inject]
        public void Constractor(DiContainer container)
        {
            this._levelCollectionViewController = container.Resolve<LevelCollectionViewController>();
            this._levelFilteringNavigationController = container.Resolve<LevelFilteringNavigationController>();
            this._annotatedBeatmapLevelCollectionsViewController = container.Resolve<AnnotatedBeatmapLevelCollectionsViewController>();
            this._selectLevelCategoryViewController = container.Resolve<SelectLevelCategoryViewController>();
        }
        private void SelectCustomSongPack(int index)
        {
            var segcontrol = this._selectLevelCategoryViewController.GetField<IconSegmentedControl, SelectLevelCategoryViewController>("_levelFilterCategoryIconSegmentedControl");
            segcontrol.SelectCellWithNumber(index);
            this._selectLevelCategoryViewController.LevelFilterCategoryIconSegmentedControlDidSelectCell(segcontrol, index);
        }

        public IEnumerator ScrollToLevel(string levelID, Action callback)
        {
            if (this._levelCollectionViewController) {
                yield return new WaitWhile(() => !Loader.AreSongsLoaded || Loader.AreSongsLoading);
                // Make sure our custom songpack is selected
                this.SelectCustomSongPack(1);
                this._levelFilteringNavigationController.UpdateCustomSongs();
                var gridView = this._annotatedBeatmapLevelCollectionsViewController.GetField<AnnotatedBeatmapLevelCollectionsGridView, AnnotatedBeatmapLevelCollectionsViewController>("_annotatedBeatmapLevelCollectionsGridView");
                gridView.SelectAndScrollToCellWithIdx(0);
                var customSong = gridView._annotatedBeatmapLevelCollections.FirstOrDefault();
                this._annotatedBeatmapLevelCollectionsViewController.HandleDidSelectAnnotatedBeatmapLevelCollection(customSong);
                var song = Loader.GetLevelById(levelID);
                if (song == null) {
                    yield break;
                }
                // handle if song browser is present
                if (BetterSongListPluginPresent) {
                    this.ClearFilter();
                }
                else if (SongBrowserPluginPresent) {
                    this.SongBrowserCancelFilter();
                }
                yield return null;
                // get the table view
                var levelsTableView = this._levelCollectionViewController.GetField<LevelCollectionTableView, LevelCollectionViewController>("_levelCollectionTableView");
                levelsTableView.SelectLevel(song);
            }
            callback?.Invoke();
        }

        /// <summary>
        /// 通常のフィルタークリアと違って同期的に行うためきちんとリロードまで待ちます。
        /// </summary>
        private void ClearFilter()
        {
            try {
                if (!BetterSongListPluginPresent) {
                    return;
                }
                var filterUI = Type.GetType("BetterSongList.UI.FilterUI, BetterSongList");
                var filterUIInstance = filterUI.GetField("persistentNuts", BindingFlags.NonPublic | BindingFlags.Static).GetValue(filterUI);
                var filterDorpDown = (DropdownWithTableView)filterUI.GetField("_filterDropdown", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(filterUIInstance);
                var list = (List<object>)filterUI.GetField("_filterOptions", BindingFlags.Static | BindingFlags.NonPublic).GetValue(filterUI);
                if (filterDorpDown.selectedIndex != list.Count - 1) {
                    var setFilterMethod = filterUI.GetMethod("SetFilter", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    _ = setFilterMethod.Invoke(filterUI, new object[] { "All", true, false });
                    this.ResetLevelCollectionTableSet();
                }
            }
            catch (Exception e) {
                Logger.logger.Error(e);
            }
        }

        /// <summary>
        /// リセット用メソッド
        /// </summary>
        /// <param name="asyncProcess"></param>
        private void ResetLevelCollectionTableSet(bool asyncProcess = false, bool clearAsyncResult = true)
        {
            try {
                if (!BetterSongListPluginPresent) {
                    return;
                }
                var levelCollectionTableSet = Type.GetType("BetterSongList.HarmonyPatches.HookLevelCollectionTableSet, BetterSongList");
                var setFilterMethod = levelCollectionTableSet.GetMethod("Refresh", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                _ = setFilterMethod.Invoke(levelCollectionTableSet, new object[] { asyncProcess, clearAsyncResult });
            }
            catch (Exception e) {
                Logger.logger.Error(e);
            }
        }

        private void SongBrowserCancelFilter()
        {
            if (!SongBrowserPluginPresent) {
                return;
            }
            try {
                var configType = Type.GetType("SongBrowser.Configuration.PluginConfig, SongBrowser");
                var configInstance = configType.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).GetValue(configType);
                var filterModeProp = configType.GetProperty("FilterMode");
                var sortModeProp = configType.GetProperty("SortMode");
                var sbAppInfo = Type.GetType("SongBrowser.SongBrowserApplication, SongBrowser");
                var sbAppInstance = sbAppInfo.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(sbAppInfo);
                var songBrowserUIType = Type.GetType("SongBrowser.UI.SongBrowserUI, SongBrowser"); //SongBrowserApplication.Instance.Ui;
                var songBrowserUI = sbAppInfo.GetProperty("Ui", BindingFlags.Public | BindingFlags.Instance).GetValue(sbAppInstance);
                if (filterModeProp != null && sortModeProp != null && songBrowserUI != null) {
                    var filter = (int)filterModeProp.GetValue(configInstance);
                    var sortMode = (int)sortModeProp.GetValue(configInstance);
                    if (filter != 0 && sortMode != 2) {
                        _ = songBrowserUIType.GetMethod("CancelFilter").Invoke(songBrowserUI, null);
                    }
                }
            }
            catch (Exception e) {
                Logger.Log($"{e}");
            }
        }
    }
}
