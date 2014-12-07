﻿using System.Collections.ObjectModel;
using WebAPI;

namespace OpenPlaylistServer.Services.Interfaces
{
    public interface IPlaylistService
    {
        Track FindTrack(string trackUri);

        void Add(Track track);

        ObservableCollection<Track> Tracks { get; }
        int CalcTScore(Track track);

        Track NextTrack();
        void Remove(Track track);
    }
}
