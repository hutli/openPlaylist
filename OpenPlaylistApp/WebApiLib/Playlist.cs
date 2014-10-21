﻿using System;
using System.Collections.Generic;

namespace WebAPILib {
	public class Playlist : SpotifyObject {
		private List<Track> _tracks;

		public Playlist (string id, string name, search searchResult) : base(id, name, searchResult) {
		}
			
		public List<Track> Tracks{ get { return new List<Track> (_tracks); } }

		public void addTrack (Track track) {
			if (!_tracks.Exists (t => t.ID == track.ID))
				_tracks.Add (track); //TODO Vote when the track is already there
		}

		public void removeTrack (Track track) {
			if (_tracks.Contains (track))
				_tracks.Remove (track);
			else
				throw new Exception (); //TODO Make spotify exception
		}
	}
}

