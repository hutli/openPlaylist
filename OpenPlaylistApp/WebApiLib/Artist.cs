﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace WebAPILib {
	public class Artist: SpotifyObject {
		private List<string> _genres = new List<string> ();
		private List<Album> _albums = new List<Album> ();
		private int _popularity;

		public Artist (string id, string name, Search searchResult) : base (id, name, searchResult) {
		}

		public List<string> Genres{ get { return new List<string> (_genres); } }

		public List<Album> Albums { get { return new List<Album> (_albums); } }

		public int Popularity { get { return _popularity; } }

		public override string URI{ get { return "spotify:artist:" + ID; } }

		public void AddAlbum (Album album) {
			if (!_albums.Exists (a => a.ID.Equals (album.ID)))
				_albums.Add (album);
		}
	}
}