//---------------------------------------------------------------------------
// <summary>
// An extension for the Unity Editor to open recent scenes of projects.
// </summary>
// <copyright file="BosoniqToolsOpenRecentScene.cs" company="bosoniq.com">
// Copyright (c) 2014 Bosoniq (tools@bosoniq.com)
// </copyright>
//---------------------------------------------------------------------------

/// <summary>
/// Implements a Unity Editor menu item to load a scene of the project.
/// </summary>
/// <remarks>
/// This code is defined in the global namespace so that
/// EditorWindow.GetWindow can find the data type.
/// </remarks>
[global::UnityEditor.InitializeOnLoad]
internal partial class BosoniqToolsOpenRecentScene
	: global::Bosoniq.Tools.OpenRecentScene
{
	[global::UnityEditor.MenuItem(
		"File/Open Recent Scene... %`",
		false,
		(SeperatorMenuItemPriority + 1))]
	protected static void OpenRecentScene()
	{
		var window = global::UnityEditor.EditorWindow.GetWindow<BosoniqToolsOpenRecentScene>(
			true, "Open Recent Scene");
		window.autoRepaintOnSceneChange = true;
	}
}
