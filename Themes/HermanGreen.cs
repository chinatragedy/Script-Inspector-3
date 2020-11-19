/* SCRIPT INSPECTOR 3
 * version 3.0.26, February 2020
 * Copyright © 2012-2020, Flipbook Games
 * 
 * Unity's legendary editor for C#, UnityScript, Boo, Shaders, and text,
 * now transformed into an advanced C# IDE!!!
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

namespace ScriptInspector.Themes
{
	using UnityEngine;
	using UnityEditor;
	
	[InitializeOnLoad]
	public class HermanGreen
	{
		private static string _themeName = "Herman Green";
		
		static HermanGreen()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
 
		public static Theme _colourTheme = new Theme
		{
			background				= new Color32(199,237,204,255),//绿豆沙
			text					= Color.black,
			hyperlinks				= Color.blue,
			
			keywords				= Color.blue, //关键词
			constants               = Color.black,
			strings					= new Color32(0x80, 0x00, 0x00, 0xff),
			builtInLiterals         = new Color32(0xff, 0x65, 0x01, 0xff), //FF8001
			operators               = Color.black,		

            referenceTypes          = new Color (0.58f, 0.04f, 0.0f),       // Dark Red
            valueTypes              = new Color (0.58f, 0.04f, 0.0f),       // Dark Red
            interfaceTypes          = new Color (0.58f, 0.04f, 0.0f),       // Dark Red
            enumTypes               = new Color (0.58f, 0.04f, 0.0f),       // Dark Red
            delegateTypes           = new Color (0.58f, 0.04f, 0.0f),       // Dark Red        
            
			builtInTypes            = Color.red, //string...
			
			namespaces              = Color.black,
			methods                 = Color.black,
			fields                  = Color.black,
			properties              = Color.black,
			events                  = Color.black,
			
			parameters              = Color.gray,
			variables               = Color.black,
			typeParameters          = new Color32(0x2b, 0x91, 0xaf, 0xff),
			enumMembers             = new Color32(111, 0, 138, 255),
			
			preprocessor            = Color.blue,
			defineSymbols           = new Color32(111, 0, 138, 255),
			inactiveCode            = Color.gray,
			comments				= new Color32(0x00, 0x80, 0x00, 0xff),
			xmlDocs                 = new Color32(0x00, 0x80, 0x00, 0xff),
			xmlDocsTags             = new Color32(0x80, 0x80, 0x80, 0xff),
			
			lineNumbers				= new Color32(0x2b, 0x91, 0xaf, 0xff),
			lineNumbersHighlight	= Color.blue,
			lineNumbersBackground	= Color.white,
			fold					= new Color32(165, 165, 165, 255),
			
			activeSelection			= new Color32(51, 153, 255, 102),
			passiveSelection		= new Color32(191, 205, 219, 102),
			searchResults			= new Color32(244, 167, 33, 255),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = new Color32(213, 213, 241, 255),
			currentLineInactive     = new Color32(228, 228, 228, 255),
			
			referenceHighlight      = new Color32(0xe0, 0xff, 0xff, 0xff),
			referenceModifyHighlight = new Color32(0xff, 0xdd, 0xdd, 0xff),
			
			tooltipBackground       = new Color32(253, 255, 153, 255),
			tooltipFrame            = new Color32(128, 128, 128, 255),
			tooltipText             = new Color32(22, 22, 22, 255),
			
			listPopupBackground				= Color.white,
		};
	}
}
