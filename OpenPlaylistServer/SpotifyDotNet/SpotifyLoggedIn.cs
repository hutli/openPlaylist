﻿using System.Threading;
using libspotifydotnet;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SpotifyDotNet
{
    /// <summary>
    /// Interaction with spotify services. Must be logged in through Spotify class.
    /// </summary>
    public class SpotifyLoggedIn
    {
        private IntPtr _searchComplete;
        private object _sync;
        private IntPtr _sessionPtr;
        private static SpotifyLoggedIn _instance;
        private bool _isPlaying;

        private SpotifyLoggedIn() { }

        internal SpotifyLoggedIn(ref IntPtr sessionPtr, object sync, ref IntPtr searchComplete)
        {
            _instance = this;
            _searchComplete = searchComplete;
            _sync = sync;
            _sessionPtr = sessionPtr;
        }

        /// <summary>
        /// Get an instance of SpotifyLoggedIn. Throws exception if not logged in via Spotify class.
        /// </summary>
        public static SpotifyLoggedIn Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("Not logged into Spotify");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Starts a search. Will return results in the SearchComplete event on Spotify class.
        /// </summary>
        /// <param name="query">The query to search for</param>
        public void Search(string query)
        {
            IntPtr queryPointer = Marshal.StringToHGlobalAnsi(query);

            lock (_sync)
            {
                IntPtr searchPtr = libspotify.sp_search_create(_sessionPtr, queryPointer, 0, 10, 0, 10, 0, 10, 0, 10,
                sp_search_type.SP_SEARCH_STANDARD, _searchComplete, IntPtr.Zero); //TODO vi bruger aldrig searchPtr?
            }
        }

        /// <summary>
        /// Start receiving audio data on the given track. Data is delivered through the MusicDelivery event.
        /// </summary>
        /// <param name="track">The track to receive audio data on</param>
        public void Play(Track track)
        {
            if (_isPlaying)
            {
                Console.WriteLine("Ind i isplaying");
                Stop();
            }

            libspotify.sp_error err;
            // process events until all track is loaded
            do
            {
                err = track.Load(_sessionPtr);
                Console.WriteLine("player load " + err);
                Spotify.Instance.ProcessEvents();
            } while (err == libspotify.sp_error.IS_LOADING);


            Console.WriteLine(track.IsLoaded);

            var playErr = libspotify.sp_session_player_play(_sessionPtr, true);
            _isPlaying = true;
            Console.WriteLine("player play " + playErr);

            var availability = track.GetAvailability(_sessionPtr);
            Console.WriteLine(availability);

            Console.WriteLine("duration: " + track.Duration);
        }

        /// <summary>
        /// Stop receiving audio data.
        /// </summary>
        public void Stop()
        {
            
            try
            {
                //if( _isPlaying)
                //libspotify.sp_session_player_unload(_sessionPtr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            _isPlaying = false;
        }

        /// <summary>
        /// Get a track from a spotify URI. The uri must be a valid spotify track URI.
        /// </summary>
        /// <param name="uri">Valid Spotify track URI</param>
        /// <returns></returns>
        public Task<Track> TrackFromLink(String uri)
        {
            try
            {
                //lock (_sync)
                //{
                    var t = Task.Run(() =>
                    {
                        IntPtr linkPtr = Marshal.StringToHGlobalAnsi(uri);
                        IntPtr spLinkPtr = libspotify.sp_link_create_from_string(linkPtr);
                        if (spLinkPtr == IntPtr.Zero)
                        {
                            throw new ArgumentException("URI was not a track URI");
                        }
                        libspotify.sp_linktype linkType = libspotify.sp_link_type(spLinkPtr);

                        if (linkType == libspotify.sp_linktype.SP_LINKTYPE_TRACK)
                        {
                            IntPtr trackLinkPtr = libspotify.sp_link_as_track(spLinkPtr);
                            //libspotify.sp_link_release(spLinkPtr);

                            return new Track(trackLinkPtr);
                        }
                        //libspotify.sp_link_release(spLinkPtr);
                        throw new ArgumentException("URI was not a track URI");
                    });
                    

                    return t;
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        ///// <summary>
        ///// Creates a pointer to a track.
        ///// </summary>
        ///// <param name="uri">Valid Spotify track uri</param>
        ///// <returns>Pointer to a spotify track</returns>
        //internal IntPtr TrackUriToIntPtr(string uri)
        //{
        //    IntPtr linkPtr = Marshal.StringToHGlobalAnsi(uri);
        //    IntPtr spLinkPtr = libspotify.sp_link_create_from_string(linkPtr);

        //    libspotify.sp_linktype linkType = libspotify.sp_link_type(spLinkPtr);

        //    if (linkType == libspotify.sp_linktype.SP_LINKTYPE_TRACK)
        //    {
        //        return libspotify.sp_link_as_track(spLinkPtr);
        //    }
        //    throw new ArgumentException("URI was not a track URI");
            
        //}
    }
}
