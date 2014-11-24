﻿using Newtonsoft.Json;
using OpenPlaylistApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WebAPI;
using Xamarin.Forms;

namespace OpenPlaylistApp.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Track> Results
        {
            get;
            set;
        }

        public PlaylistViewModel(Venue venue)
        {
            Results = new ObservableCollection<Track>();
            GetResults(venue);
        }


        async void GetResults(Venue venue)
        {
            Session session = Session.Instance();
            ObservableCollection<Track> returnValue = new ObservableCollection<Track>();
            try
            {
                var json = await session.GetPlaylist(venue);
                returnValue = (ObservableCollection<Track>)JsonConvert.DeserializeObject(json, typeof(ObservableCollection<Track>));
                foreach (Track t in returnValue)
                {
                    Results.Add(t);
                }
            }
            catch (Exception ex)
            {
                App.GetMainPage().DisplayAlert("Error", ex.Message, "OK", "Cancel");
            }
        }
    }
}
