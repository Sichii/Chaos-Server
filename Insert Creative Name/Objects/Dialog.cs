namespace Insert_Creative_Name.Objects
{
    internal sealed class Dialog
    {
        internal byte DialogType { get; set; }
        internal byte GameObjectType { get; set; }
        internal int GameObjectId { get; set; }
        internal byte Unknown1 { get; set; }
        internal ushort Sprite1 { get; set; }
        internal byte Color1 { get; set; }
        internal byte Unknown2 { get; set; }
        internal ushort Sprite2 { get; set; }
        internal byte Color2 { get; set; }
        internal ushort PursuitId { get; set; }
        internal ushort DialogId { get; set; }
        internal bool PrevButton { get; set; }
        internal bool NextButton { get; set; }
        internal byte Unknown3 { get; set; }
        internal string GameObjectName { get; set; }
        internal string Message { get; set; }
        internal string[] Options { get; set; }
        internal string TopCaption { get; set; }
        internal byte InputLength { get; set; }
        internal string BottomCaption { get; set; }

        internal Dialog(byte dialogType, byte gameObjectType, int gameObjectId, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2,
            ushort pursuitId, ushort dialogId, bool prevButton, bool nextButton, byte unknown3, string gameObjectName, string message)
        {
            DialogType = dialogType;
            GameObjectType = gameObjectType;
            GameObjectId = gameObjectId;
            Unknown1 = unknown1;
            Sprite1 = sprite1;
            Color1 = color1;
            Unknown2 = unknown2;
            Sprite2 = sprite2;
            Color2 = color2;
            PursuitId = pursuitId;
            DialogId = dialogId;
            PrevButton = prevButton;
            NextButton = nextButton;
            Unknown3 = unknown3;
            GameObjectName = gameObjectName;
            Message = message;
            Options = new string[0];
            TopCaption = string.Empty;
            BottomCaption = string.Empty;
        }

        internal Dialog(byte dialogType, byte gameObjectType, int gameObjectId, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2,
            ushort pursuitId, ushort dialogId, bool prevButton, bool nextButton, byte unknown3, string gameObjectName, string message, string[] options)
          : this(dialogType, gameObjectType, gameObjectId, unknown1, sprite1, color1, unknown2, sprite2, color2, pursuitId, dialogId, prevButton, nextButton, unknown3, gameObjectName, message)
        {
            Options = options;
        }

        internal Dialog(byte dialogType, byte gameObjectType, int gameObjectId, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2,
            ushort pursuitId, ushort dialogId, bool prevButton, bool nextButton, byte unknown3, string gameObjectName, string message, string topCaption, byte inputLength, string bottomCaption)
          : this(dialogType, gameObjectType, gameObjectId, unknown1, sprite1, color1, unknown2, sprite2, color2, pursuitId, dialogId, prevButton, nextButton, unknown3, gameObjectName, message)
        {
            TopCaption = topCaption;
            InputLength = inputLength;
            BottomCaption = bottomCaption;
        }

        internal Dialog(byte dialogType, byte gameObjectType, int gameObjectId, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2,
            ushort pursuitId, ushort dialogId, bool prevButton, bool nextButton, byte unknown3, string gameObjectName, string message, string[] options, string topCaption, byte inputLength, string bottomCaption)
          : this(dialogType, gameObjectType, gameObjectId, unknown1, sprite1, color1, unknown2, sprite2, color2, pursuitId, dialogId, prevButton, nextButton, unknown3, gameObjectName, message)
        {
            Options = options;
            TopCaption = topCaption;
            InputLength = inputLength;
            BottomCaption = bottomCaption;
        }
    }
}
