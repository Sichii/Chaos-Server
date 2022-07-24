﻿namespace Chaos.Geometry.Interfaces;

public interface ILocation : IPoint
{
    string Map { get; }

    static string ToString(ILocation location) => $"{location.Map}:{IPoint.ToString(location)}";
}