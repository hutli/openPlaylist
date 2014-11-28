﻿using System.Collections.Concurrent;
using System.Windows.Threading;
using OpenPlaylistServer.Collections;
using OpenPlaylistServer.Models;
using OpenPlaylistServer.Services.Interfaces;
using WebAPI;

namespace OpenPlaylistServer.Services.Implementation
{
    class MainWindowViewModel : IMainWindowViewModel
    {
        private IPlaylistService _playlistService;
        private IUserService _userService;
        private IPlaybackService _playbackService;

        public MainWindowViewModel(IPlaylistService playlistService, IUserService userService, IPlaybackService playbackService)
        {
            _playlistService = playlistService;
            _userService = userService;
            _playbackService = playbackService;
        }

        public ConcurrentBagify<Track> Tracks
        {
            get
            {
                return _playlistService.Tracks;
            }
        }

        public ConcurrentBag<User> Users
        {
            get
            {
                return _userService.Users;
            }
        }

        public void TrackEnded()
        {
            // TrackEnded is called from libspotify running in a different thread than the UI thread.
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                // if there are no tracks to be played next, don't do anything
                // TODO: brug måske en algoritme til at beregne hvad vi skal spille næste gang
                if (Tracks.Count == 0)
                {
                    return;
                }

                _playbackService.Stop();
                Track next = _playlistService.NextTrack();
                if (next != null)
                {
                    _playbackService.Play(next);
                }
            });
        }

        public void PlayButtonClicked()
        {
            var nextTrack = _playlistService.NextTrack();
            if (nextTrack != null)
            {
                _playbackService.Play(nextTrack);
            }
        }

        public void StopButtonClicked()
        {
            _playbackService.Stop();
        }
    }
}
