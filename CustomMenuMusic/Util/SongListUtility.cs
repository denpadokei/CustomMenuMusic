using HMUI;
using IPA.Loader;
using IPA.Utilities;
using SongCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _levelCollectionViewController = container.Resolve<LevelCollectionViewController>();
            _levelFilteringNavigationController = container.Resolve<LevelFilteringNavigationController>();
            _annotatedBeatmapLevelCollectionsViewController = container.Resolve<AnnotatedBeatmapLevelCollectionsViewController>();
            _selectLevelCategoryViewController = container.Resolve<SelectLevelCategoryViewController>();
        }
        private void SelectCustomSongPack(int index)
        {
            var segcontrol = _selectLevelCategoryViewController.GetField<IconSegmentedControl, SelectLevelCategoryViewController>("_levelFilterCategoryIconSegmentedControl");
            segcontrol.SelectCellWithNumber(index);
            _selectLevelCategoryViewController.LevelFilterCategoryIconSegmentedControlDidSelectCell(segcontrol, index);
        }

        public IEnumerator ScrollToLevel(string levelID, Action<bool> callback, bool animated, bool isRetry = false)
        {
            if (_levelCollectionViewController) {
                yield return new WaitWhile(() => !Loader.AreSongsLoaded || Loader.AreSongsLoading);

                // handle if song browser is present
                if (PluginManager.GetPlugin("SongBrowser") != null) {
                    SongBrowserCancelFilter();
                }

                // Make sure our custom songpack is selected
                SelectCustomSongPack(2);
                _levelFilteringNavigationController.UpdateCustomSongs();
                var tableView = _annotatedBeatmapLevelCollectionsViewController.GetField<AnnotatedBeatmapLevelCollectionsTableView, AnnotatedBeatmapLevelCollectionsViewController>("_annotatedBeatmapLevelCollectionsTableView");
                tableView.SelectAndScrollToCellWithIdx(0);
                var customSong = tableView.GetField<IReadOnlyList<IAnnotatedBeatmapLevelCollection>, AnnotatedBeatmapLevelCollectionsTableView>("_annotatedBeatmapLevelCollections").FirstOrDefault();
                _levelFilteringNavigationController.HandleAnnotatedBeatmapLevelCollectionsViewControllerDidSelectAnnotatedBeatmapLevelCollection(customSong);


                var song = Loader.GetLevelByHash(levelID.Split('_').Last());
                if (song == null) {
                    yield break;
                }
                // get the table view
                var levelsTableView = _levelCollectionViewController.GetField<LevelCollectionTableView, LevelCollectionViewController>("_levelCollectionTableView");
                levelsTableView.SelectLevel(song);

            }
            callback?.Invoke(false);
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
