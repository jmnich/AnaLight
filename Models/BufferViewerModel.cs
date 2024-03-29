﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;
using System.Collections.ObjectModel;
using AnaLight.Models;
using System.Diagnostics;

namespace AnaLight.Models
{
    public class BufferViewerModel
    {
        public ObservableCollection<BasicSpectraContainer> ListOfSpectra { get; }

        public BufferViewerModel(ObservableCollection<BasicSpectraContainer> listOfSpectra)
        {
            ListOfSpectra = listOfSpectra;
        }

        public void EraseSpectraBuffer()
        {
            ListOfSpectra.Clear();
        }
    }
}
