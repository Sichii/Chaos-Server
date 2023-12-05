using Silk.NET.Maths;
using Silk.NET.Windowing;

var windowOptions = WindowOptions.Default;
windowOptions.Size = new Vector2D<int>(640, 480);
windowOptions.Title = "Darkages";

var window = Window.Create(windowOptions);