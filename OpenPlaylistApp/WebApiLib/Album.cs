﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebAPILib {
    public class Album : SpotifyObject {
        private string _albumType;
        private List<Image> _images = new List<Image>();
        private List<Artist> _artists = new List<Artist>();
        private List<Track> _tracks = new List<Track>();
		private bool _tracksCached = false;
		private bool _artistsCached = false;

		public bool TracksCached { get { return _tracksCached; } }

		public bool ArtistsCached { get { return _artistsCached; } }

        public string AlbumType { get { return _albumType; } }

        public List<Image> Images { get { return new List<Image>(_images); } }

        public List<Artist> Artists {
            get {
				if (!_artistsCached) //If artists aren't loaded, load artists from spotify
					cache ();
                return new List<Artist>(_artists);
            }
        }

		public List<Track> Tracks { 
			get {
                if(!_tracksCached) //If artists aren't loaded, load artists from spotify
					cache ();
				return new List<Track>(_tracks);
			} 
		}

		public string Href{ get { return "https://api.spotify.com/v1/albums/" + ID; } }

		private void cache(){ //Loads artists and tracks from spotify
			JObject o = Search.getJobject(Href);
			if (!_artistsCached) { //Load artists
				List<Artist> artists = new List<Artist> ();
				foreach (JObject jsonArtist in o["artists"]) {
					string id = Convert.ToString (jsonArtist ["id"]);
					string name = Convert.ToString (jsonArtist ["name"]);
					if (SearchResult.Artists.Exists (a => id.Equals (a.ID))) { //If artist already exists, add album to artist
						SearchResult.Artists.Find (a => id.Equals (a.ID)).AddAlbum (this); 
						artists.Add (SearchResult.Artists.Find (a => id.Equals (a.ID)));
					} else { //If artist does not exist, create new artists
						Artist tmpArtist = new Artist (id, name, SearchResult);
						tmpArtist.AddAlbum (this);
						SearchResult.AddArtist (tmpArtist);
						artists.Add (tmpArtist);
					}
				}
				_artists = artists;
				_artistsCached = true;
			}
			if (!_tracksCached) { //Load Tracks
				List<Track> tracks = new List<Track> ();
				foreach (JObject jsonTrack in o["tracks"]["items"]) {
					string id = (string)(jsonTrack ["id"]);
					string name = (string)(jsonTrack ["name"]);
					int duration = (int)(jsonTrack ["duration_ms"]);
					bool isExplicit = (bool)jsonTrack ["explicit"];
					int trackNumber = (int)(jsonTrack ["track_number"]);
					if (SearchResult.Tracks.Exists (a => id.Equals (a.ID)))
						_tracks.Add (SearchResult.Tracks.Find (a => id.Equals (a.ID)));
					else { //
						Track tmpTrack = new Track (id, name, 0, duration, isExplicit, trackNumber, this, SearchResult); //TODO Spotify don't want to tell ud popularity
						SearchResult.AddTrack (tmpTrack);
						_tracks.Add (tmpTrack);
					}
				}
				_tracksCached = true;
			}
		}

        public override string URI { get { return "spotify:album:" + ID; } }

		public Album(string id, string name, string albumtype, IEnumerable<Image> images, Search searchResult, List<Artist> artists) : this(id,name,albumtype,images,searchResult){
			AddArtists(artists);
			foreach (Artist a in artists) {
				a.AddAlbum (this);
			}
		}

		public Album(string id, string name, string albumtype, IEnumerable<Image> images, Search searchResult) : base(id, name, searchResult) {
            _albumType = albumtype;
            _images = new List<Image>(images);
        }

		public void AddArtists(List<Artist> artists){
			if (_artists.Count == 0) {
				_artists = artists;
				foreach (Artist a in _artists) {
					a.AddAlbum (this);
				}
				_artistsCached = true;
			}
		}

        public void AddTrack(Track track) {
            if(!_tracks.Exists(a => track.ID.Equals(a.ID)))
                _tracks.Add(track);
        }
    }
}