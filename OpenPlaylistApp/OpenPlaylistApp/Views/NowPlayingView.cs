﻿using OpenPlaylistApp.Models;
using OpenPlaylistApp.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OpenPlaylistApp.Views
{
    class NowPlayingView : ContentView
    {
        private Label _nowPlayingLabel = new Label();
        private Label _trackTitleLabel = new Label();
        private Image _trackImage = new Image();
        NowPlayingViewModel _npvm;
        private RelativeLayout layout = new RelativeLayout();
        private ProgressBar progressBar = new ProgressBar();
        public NowPlayingView()
        {
            _nowPlayingLabel.Text = "Now Playing:";

            App.User.VenueChanged += GetNowPlaying;

            _trackImage.Opacity = 0.55f;
            _trackImage.Aspect = Aspect.AspectFill;

            _nowPlayingLabel.Font = Font.SystemFontOfSize(NamedSize.Micro, FontAttributes.Bold); // Maybe implement side scrolling text if clipped by parent
            _trackTitleLabel.Font = Font.SystemFontOfSize(NamedSize.Large);
           
            layout.MinimumHeightRequest = 80f;
            layout.IsClippedToBounds = true;
            layout.HeightRequest = 100f;

            layout.Children.Add(_trackImage, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) => parent.Width),Constraint.RelativeToParent((parent) => parent.Height));
            layout.Children.Add(_nowPlayingLabel, Constraint.Constant(0));
            layout.Children.Add(_trackTitleLabel, Constraint.Constant(0), Constraint.RelativeToParent((parent) => (parent.Height / 2)-(_trackTitleLabel.Height/2)));
            layout.Children.Add(progressBar, Constraint.Constant(0), Constraint.RelativeToParent((parent) => parent.Height - progressBar.Height), Constraint.RelativeToParent((parent) => parent.Width));
            Content = layout;
        }

        public void GetNowPlaying(Venue venue)
        {
            if (_npvm == null)
            {
                _npvm = new NowPlayingViewModel(venue);
                _npvm.LoadComplete += OnLoadComplete;
            }
            else
                _npvm.GetResult(venue);
        }

        void OnLoadComplete()
        {
            if (_trackTitleLabel != null)
            {
                if (_trackTitleLabel.Text == null || !_trackTitleLabel.Text.Equals(_npvm.Result.Name + " - " + _npvm.Result.Album.Artists[0].Name))
                {
                    try
                    {
                        _trackTitleLabel.Text = _npvm.Result.Name + " - " + _npvm.Result.Album.Artists[0].Name ?? "";
                        if (_npvm.Result.Album.Images != null && _npvm.Result.Album.Images[0] != null && _trackImage != null)
                        {
                            _trackImage.Source = _npvm.Result.Album.Images[0].URL ?? "";
                        }

                        progressBar.Progress = Convert.ToDouble(_npvm.Result.CurrentDurationStep) / Convert.ToDouble(_npvm.Result.Duration);

                        progressBar.ProgressTo(1, Convert.ToUInt32(Math.Abs(_npvm.Result.Duration - _npvm.Result.CurrentDurationStep)), Easing.Linear);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw e.InnerException;
                    }
                }
            }
        }
    }
}
