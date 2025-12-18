public static class MsgType
{
    public static string SimpleEvent = nameof(SimpleEvent);
    public static string IntEvent = nameof(IntEvent);

    // Dialog
    public static string AddTextToLog = nameof(AddTextToLog);
    public static string InterruptDialog = nameof(InterruptDialog);
    public static string AutoPlay = nameof(AutoPlay);
    public static string PlayerInputNext = nameof(PlayerInputNext);
    public static string DialogPlayStory = nameof(DialogPlayStory);
    public static string CloseDialog = nameof(CloseDialog);

    // Dialog choices
    public static string ShowChoices = nameof(ShowChoices);
    public static string PlayerChoosing = nameof(PlayerChoosing);
    public static string PlayerChoiceResult = nameof(PlayerChoiceResult);

    // Dialog Menu
    public static string ToggleDialogMenu = nameof(ToggleDialogMenu);
    public static string ChapterEnd = nameof(ChapterEnd);

    public static string ChatStart = nameof(ChatStart);
    public static string ChatEnd = nameof(ChatEnd);

    public static string DialogCharacterChanged = nameof(DialogCharacterChanged);

    // Map 最好改一个更具体的名字
    public static string State = nameof(State);
    public static string Chapter = nameof(Chapter);
    public static string CallDialog = nameof(CallDialog);
    public static string LoadTravelData = nameof(LoadTravelData);
    public static string CancelPreviewTravel = nameof(CancelPreviewTravel);
    public static string CloseMap = nameof(CloseMap);

    // System
    public static string SaveGame = nameof(SaveGame);
    public static string NewGameInit = nameof(NewGameInit);

    // Map
    public static string PreviewTravel = nameof(PreviewTravel);
    public static string PreviewTravelResult = nameof(PreviewTravelResult);
    public static string Travel = nameof(Travel);
    public static string Nap = nameof(Nap);

    // Intelligence
    public static string LoadIntelligenceDetail = nameof(LoadIntelligenceDetail);
    public static string LoadClueDetail = nameof(LoadClueDetail);

    public static string ShowSpreadPanel = nameof(ShowSpreadPanel);
    public static string HideSpreadPanel = nameof(HideSpreadPanel);
    public static string ShowSpreadDetail = nameof(ShowSpreadDetail);

    // World
    public static string OnWorldCharacterClicked = nameof(OnWorldCharacterClicked);
    public static string OnWorldClueClicked = nameof(OnWorldClueClicked);
}