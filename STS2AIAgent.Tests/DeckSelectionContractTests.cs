namespace STS2AIAgent.Tests;

/// <summary>
/// Source-level contracts for Godot deck-grid selections. These screens cannot be
/// instantiated in the lightweight test process, so the tests verify that the
/// native private selection state is exposed and consumed by the action settle path.
/// </summary>
internal static class DeckSelectionContractTests
{
    public static void DeckGridPayloadReportsNativeSelectionProgress()
    {
        var rawStateSource = AgentSourceFixture.Read(
            "STS2AIAgent/Game/GameStateService.cs");
        var stateSource = AgentSourceFixture.WithoutWhitespace(rawStateSource);
        var payloadBody = AgentSourceFixture.WithoutWhitespace(
            AgentSourceFixture.MethodBody(rawStateSource, "BuildSelectionPayload"));

        Assert.Contains("NDeckCardSelectScreen", stateSource, StringComparison.Ordinal);
        Assert.Contains("\"_prefs\"", stateSource, StringComparison.Ordinal);
        Assert.Contains("\"_selectedCards\"", stateSource, StringComparison.Ordinal);
        Assert.Contains("selectedCount++", stateSource, StringComparison.Ordinal);
        Assert.Contains(
            "selected_count=hasCombatHandSelection?combatHandSelection.SelectedCount:hasDeckCardSelection?deckCardSelection.SelectedCount:0",
            payloadBody,
            StringComparison.Ordinal);
        Assert.Contains(
            "min_select=hasCombatHandSelection?combatHandSelection.MinSelect:hasDeckCardSelection?deckCardSelection.MinSelect:1",
            payloadBody,
            StringComparison.Ordinal);
    }

    public static void IntermediateRequiredPickSettlesOnSelectionProgress()
    {
        var rawActionSource = AgentSourceFixture.Read(
            "STS2AIAgent/Game/GameActionService.cs");
        var selectBody = AgentSourceFixture.WithoutWhitespace(
            AgentSourceFixture.MethodBody(rawActionSource, "ExecuteSelectDeckCardAsync"));
        var progressBody = AgentSourceFixture.WithoutWhitespace(
            AgentSourceFixture.MethodBody(
                rawActionSource, "WaitForDeckSelectionProgressAsync"));

        Assert.Contains(
            "deckCardSelection.SelectedCount+1<deckCardSelection.MinSelect",
            selectBody,
            StringComparison.Ordinal);
        Assert.Contains("WaitForDeckSelectionProgressAsync", selectBody, StringComparison.Ordinal);
        Assert.Contains("ConfirmDeckSelectionAsync", selectBody, StringComparison.Ordinal);
        Assert.Contains(
            "metadata.SelectedCount>previousSelectedCount",
            progressBody,
            StringComparison.Ordinal);
        Assert.Contains(
            "ReferenceEquals(ActiveScreenContext.Instance.GetCurrentScreen(),screen)",
            progressBody,
            StringComparison.Ordinal);
    }
}
