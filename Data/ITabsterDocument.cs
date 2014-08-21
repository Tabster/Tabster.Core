﻿#region

using System;
using System.IO;

#endregion

namespace Tabster.Core.Data
{
    public interface ITabsterDocument
    {
        Version FileVersion { get; }
        FileInfo FileInfo { get; }
        void Load(string filename);
        void Save();
        void Save(string fileName);
        void Update();
    }
}