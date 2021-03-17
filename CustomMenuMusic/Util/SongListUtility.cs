using HMUI;
using IPA.Loader;
using IPA.Utilities;
using SongCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

                // handle if song browser is present
                if (PluginManager.GetPlugin("SongBrowser") != null) {
                    this.SongBrowserCancelFilter();
                }

                // Make sure our custom songpack is selected
                this.SelectCustomSongPack(2);
                this._levelFilteringNavigationController.UpdateCustomSongs();
                var tableView = this._annotatedBeatmapLevelCollectionsViewController.GetField<AnnotatedBeatmapLevelCollectionsTableView, AnnotatedBeatmapLevelCollectionsViewController>("_annotatedBeatmapLevelCollectionsTableView");
                tableView.SelectAndScrollToCellWithIdx(0);
                var customSong = tableView.GetField<IReadOnlyList<IAnnotatedBeatmapLevelCollection>, AnnotatedBeatmapLevelCollectionsTableView>("_annotatedBeatmapLevelCollections").FirstOrDefault();
                this._levelFilteringNavigationController.HandleAnnotatedBeatmapLevelCollectionsViewControllerDidSelectAnnotatedBeatmapLevelCollection(customSong);


                var song = Loader.GetLevelById(levelID);
                if (song == null) {
                    yield break;
                }
                // get the table view
                var levelsTableView = this._levelCollectionViewController.GetField<LevelCollectionTableView, LevelCollectionViewController>("_levelCollectionTableView");
                levelsTableView.SelectLevel(song);

            }
            callback?.Invoke();
        }


        private void SongBrowserCancelFilter()
        {
            if (PluginManager.GetPlugin("SongBrowser") != null) {
                var songBrowserUI = SongBrowser.SongBrowserApplication.Instance.GetField<SongBrowser.UI.SongBrowserUI, SongBrowser.SongBrowserApplication>("_songBrowserUI");
                if (songBrowserUI) {
                    if (songBrowserUI.Model.Settings.filterMode != SongBrowser.DataAccess.SongFilterMode.None && songBrowserUI.Model.Settings.sortMode != SongBrowser.DataAccess.SongSortMode.Original) {
                        songBrowserUI.CancelFilter();
                    }
                }
            }
        }
    }
}
