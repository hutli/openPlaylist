﻿using Xamarin.Forms;

namespace OpenPlaylistApp.Views
{
    class VolumeView : ContentView
    {
        public VolumeView()
        {
            Slider slider = new Slider(0,100,50);
            Content = slider;
        }

    }
}