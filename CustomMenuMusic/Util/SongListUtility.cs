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
            _songBrowserMetaData = PluginManager.GetPlugin("Song Browser");
            SongBrowserPluginPresent = _songBrowserMetaData != null;

            _betterSonglistMetaData = PluginManager.GetPlugin("BetterSongList");
            BetterSongListPluginPresent = _betterSonglistMetaData != null;
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
                this.SelectCustomSongPack(2);
                this._levelFilteringNavigationController.UpdateCustomSongs();
                var tableView = this._annotatedBeatmapLevelCollectionsViewController.GetField<AnnotatedBeatmapLevelCollectionsTableView, AnnotatedBeatmapLevelCollectionsViewController>("_annotatedBeatmapLevelCollectionsTableView");
                tableView.SelectAndScrollToCellWithIdx(0);
                var customSong = tableView.GetField<IReadOnlyList<IAnnotatedBeatmapLevelCollection>, AnnotatedBeatmapLevelCollectionsTableView>("_annotatedBeatmapLevelCollections").FirstOrDefault();
                this._annotatedBeatmapLevelCollectionsViewController.HandleDidSelectAnnotatedBeatmapLevelCollection(tableView, customSong);
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

        void ClearFilter()
        {
            if (!BetterSongListPluginPresent) {
                return;
            }
            try {
                var filerUI = Type.GetType("BetterSongList.UI.FilterUI, BetterSongList");
                var filterUIInstance = filerUI.GetField("persistentNuts", (BindingFlags.NonPublic | BindingFlags.Static)).GetValue(filerUI);
                var filterDorpDown = (DropdownWithTableView)filerUI.GetField("_filterDropdown", (BindingFlags.NonPublic | BindingFlags.Instance)).GetValue(filterUIInstance);
                if (filterDorpDown.selectedIndex != 0) {
                    var setFilterMethod = filerUI.GetMethod("SetFilter", (BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
                    setFilterMethod.Invoke(filerUI, new object[] { null, true, true });
                }
            }
            catch (Exception e) {
                Logger.Log($"{e}");
            }
        }

        private void SongBrowserCancelFilter()
        {
            if (!SongBrowserPluginPresent) {
                return;
            }
            try {
                var configType = Type.GetType("SongBrowser.Configuration.PluginConfig, SongBrowser");
                var configInstance = configType.GetProperty("Instance", (BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)).GetValue(configType);
                var filterModeProp = configType.GetProperty("FilterMode");
                var sortModeProp = configType.GetProperty("SortMode");
                var sbAppInfo = Type.GetType("SongBrowser.SongBrowserApplication, SongBrowser");
                var sbAppInstance = sbAppInfo.GetField("Instance", (BindingFlags.Static | BindingFlags.Public)).GetValue(sbAppInfo);
                var songBrowserUIType = Type.GetType("SongBrowser.UI.SongBrowserUI, SongBrowser"); //SongBrowserApplication.Instance.Ui;
                var songBrowserUI = sbAppInfo.GetProperty("Ui", BindingFlags.Public | BindingFlags.Instance).GetValue(sbAppInstance);
                if (filterModeProp != null && sortModeProp != null && songBrowserUI != null) {
                    var filter = (int)filterModeProp.GetValue(configInstance);
                    var sortMode = (int)sortModeProp.GetValue(configInstance);
                    if (filter != 0 && sortMode != 2) {
                        songBrowserUIType.GetMethod("CancelFilter").Invoke(songBrowserUI, null);
                    }
                }
            }
            catch (Exception e) {
                Logger.Log($"{e}");
            }
        }
    }
}
