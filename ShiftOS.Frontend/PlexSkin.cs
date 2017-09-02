﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plex.Engine;

namespace Plex.Frontend
{
    public class PlexSkin : Skin
    {

        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [Image("progressbarbg")]
        [ShifterName("Progress Bar Background Image")]
        [ShifterDescription("Set an image for the background of a progress bar.")]
        public byte[] ProgressBarBG = null;


        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        [Image("progress")]
        [ShifterName("Progress Image")]
        [ShifterDescription("Set the image for the progress inside a progress bar.")]
        public byte[] Progress = null;


        

        

        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        //[RequiresUpgrade("shift_progress_bar")]
        [ShifterName("Progress bar block size")]
        [ShifterDescription("If the progress bar style is set to Blocks, this determines how wide each block should be.")]
        public int ProgressBarBlockSize = 15;


        [ShifterMeta("System")]
        [ShifterCategory("Progress Bar")]
        //[RequiresUpgrade("shift_progress_bar")]
        [ShifterDescription("Set the style of a progress bar.\r\nMarquee: The progress bar will render a box that moves from the left to the right in a loop.\r\nContinuous: Progress is shown by a single, continuous box.\r\nBlocks: Just like Continuous, but the box is split into even smaller boxes of a set width.")]
        [ShifterName("Progress bar style")]
        public ProgressBarStyle ProgressBarStyle = ProgressBarStyle.Continuous;








        [Image("panelclockbg")]
        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel Clock")]
        [ShifterName("Panel Clock Background Image")]
        [ShifterDescription("Set the background image of the panel clock.")]
        //[RequiresUpgrade("skinning;shift_panel_clock")]
        public byte[] PanelClockBG = null;

        [ShifterMeta("System")]
        [ShifterCategory("Login Screen")]
        //[RequiresUpgrade("gui_based_login_screen")]
        [ShifterName("Login Screen Background Color")]
        [ShifterDescription("Change the background color of the login screen.")]
        public Color LoginScreenColor = Color.Black;

        [ShifterMeta("System")]
        [ShifterCategory("Login Screen")]
        //[RequiresUpgrade("skinning;gui_based_login_screen")]
        [ShifterName("Login Screen Background Image")]
        [ShifterDescription("Set an image as your login screen!")]
        [Image("login")]
        public byte[] LoginScreenBG = null;


        //[RequiresUpgrade("shift_screensaver")]
        [ShifterMeta("System")]
        [ShifterCategory("Screen saver")]
        [ShifterName("Screen saver wait (milliseconds)")]
        [ShifterDescription("How long do you have to stay idle before the screensaver activates?")]
        public int ScreensaverWait = 300000;

        //[RequiresUpgrade("skinning;shift_screensaver")]
        [ShifterMeta("System")]
        [ShifterCategory("Screen saver")]
        [ShifterName("Screen saver image")]
        [ShifterDescription("What image should appear on the screen saver?")]
        public byte[] ScreensaverImage = null;



        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_title_text")]
        [ShifterName("Title Font")]
        [ShifterDescription("The font style for the title text.")]
        public Font TitleFont = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);


        [ShifterEnumMask(new[] { "Right", "Left" })]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Title button position")]
        [ShifterDescription("Where should the title buttons be located?")]
        public int TitleButtonPosition = 0;

        


        
        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Text Color")]
        //[RequiresUpgrade("shift_title_text")]
        [ShifterDescription("The color of the title text.")]
        public Color TitleTextColor = Color.Black;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Background Color")]
        //[RequiresUpgrade("shift_titlebar")]
        [ShifterDescription("The color of the titlebar's background.")]
        public Color TitleBackgroundColor = Color.Gray;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Inactive Background Color")]
        //[RequiresUpgrade("shift_titlebar;shift_states")]
        [ShifterDescription("The color of the titlebar's background when the window isn't active.")]
        public Color TitleInactiveBackgroundColor = Color.White;



        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Left Border Background")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the left border.")]
        public Color BorderLeftBackground = Color.Gray;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Left Border Inactive Background")]
        //[RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the left border when the window is inactive.")]
        public Color BorderInactiveLeftBackground = Color.White;


        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Right Border Background")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the right border.")]
        public Color BorderRightBackground = Color.Gray;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Right Border Inactive Background")]
        //[RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the right border when the window is inactive.")]
        public Color BorderInactiveRightBackground = Color.White;


        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        //[RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel buttons from top")]
        [ShifterDescription("How far from the top should the panel buttons be?")]
        public int PanelButtonFromTop = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Border Background")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the bottom border.")]
        public Color BorderBottomBackground = Color.Gray;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Border Inactive Background")]
        //[RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the bottom border when the window is inactive.")]
        public Color BorderInactiveBottomBackground = Color.White;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Use Inactive Border Assets?")]
        //[RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("Do you want to use separate colors and images for inactive Window Borders?")]
        public bool RenderInactiveBorders = false;


        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        [ShifterName("Panel button holder from left")]
        [ShifterDescription("How far from the left should the panel button holder be?")]
        //[RequiresUpgrade("shift_panel_buttons")]
        public int PanelButtonHolderFromLeft = 100;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Left Border Inactive Background")]
        //[RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the bottom left border when the window is inactive.")]
        public Color BorderInactiveBottomLeftBackground = Color.White;


        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Left Border Background")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the bottom left border.")]
        public Color BorderBottomLeftBackground = Color.Gray;


        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Right Border Background")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterDescription("The background color for the bottom right border.")]
        public Color BorderBottomRightBackground = Color.Gray;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        [ShifterName("Bottom Right Border Inactive Background")]
        //[RequiresUpgrade("shift_window_borders;shift_states")]
        [ShifterDescription("The background color for the bottom right border when the window is inactive.")]
        public Color BorderInactiveBottomRightBackground = Color.White;



        #region Windows -> Title Buttons -> Idle -> Colors
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Color")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The close button color")]
        public Color CloseButtonColor = Color.Black;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Color")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The maximize button color")]
        public Color MaximizeButtonColor = Color.Black;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Color")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The minimize button color")]
        public Color MinimizeButtonColor = Color.Black;




        #endregion

        #region Windows -> Title Buttons -> Over -> Colors
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Over Color")]
        //[RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The close button color when the mouse hovers over it.")]
        public Color CloseButtonOverColor = Color.FromArgb(0x80, 0, 0);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Over Color")]
        //[RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The maximize button color when the mouse hovers over it.")]
        public Color MaximizeButtonOverColor = Color.Black;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Over Color")]
        //[RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The minimize button color when the mouse hovers over it")]
        public Color MinimizeButtonOverColor = Color.Black;




        #endregion

        #region Windows -> Title Buttons -> Down -> Colors
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Down Color")]
        //[RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The close button color when the mouse clicks it.")]
        public Color CloseButtonDownColor = Color.FromArgb(0x80, 0, 0);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Down Color")]
        //[RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The maximize button color when the mouse clicks it.")]
        public Color MaximizeButtonDownColor = Color.White;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Down Color")]
        //[RequiresUpgrade("shift_title_buttons;shift_states")]
        [ShifterDescription("The minimize button color when the mouse clicks it")]
        public Color MinimizeButtonDownColor = Color.Black;


        #endregion


        [ShifterHidden]
        public CommandParser CurrentParser = CommandParser.GenerateSample();


        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Background")]
        //[RequiresUpgrade("shift_desktop_panel")]
        [ShifterDescription("The background color used by the desktop panel")]
        public Color DesktopPanelColor = Color.Gray;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock Text Color")]
        //[RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The text color used by the desktop panel's clock.")]
        public Color DesktopPanelClockColor = Color.Black;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock Background Color")]
        //[RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The background color used by the desktop panel's clock.")]
        public Color DesktopPanelClockBackgroundColor = Color.Gray;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock Font")]
        //[RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The font used by the desktop panel's clock.")]
        public Font DesktopPanelClockFont = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Clock From Right")]
        //[RequiresUpgrade("shift_panel_clock")]
        [ShifterDescription("The position in pixels relative to the width of the desktop panel that the clock will sit at.")]
        public Point DesktopPanelClockFromRight = new Point(2, 2);


        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Height")]
        //[RequiresUpgrade("shift_desktop_panel")]
        [ShifterDescription("The height in pixels of the desktop panel.")]
        public int DesktopPanelHeight = 24;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel Position")]
        [ShifterEnumMask(new[] { "Top", "Bottom" })]
        //[RequiresUpgrade("shift_desktop_panel")]
        [ShifterDescription("The position of the desktop panel.")]
        public int DesktopPanelPosition = 0;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Titlebar Height")]
        //[RequiresUpgrade("shift_titlebar")]
        [ShifterDescription("The height of the titlebar.")]
        public int TitlebarHeight = 30;

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Size")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The close button size")]
        public Size CloseButtonSize = new Size(24, 24);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Size")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The maximize button size")]
        public Size MaximizeButtonSize = new Size(24, 24);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Size")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The minimize button size")]
        public Size MinimizeButtonSize = new Size(24, 24);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button From Right")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The close button location from the right of the titlebar.")]
        public Point CloseButtonFromSide = new Point(3, (30 - 24) / 2);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button From Right")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The maximize button location from the right of the titlebar.")]
        public Point MaximizeButtonFromSide = new Point(24 + 6, (30 - 24) / 2);

        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button From Right")]
        //[RequiresUpgrade("shift_title_buttons")]
        [ShifterDescription("The minimize button location from the right of the titlebar.")]
        public Point MinimizeButtonFromSide = new Point(48 + 9, (30 - 24) / 2);

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title text centered?")]
        //[RequiresUpgrade("shift_title_text")]
        [ShifterDescription("Is the title text centered?")]
        public bool TitleTextCentered = false;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Title Text From Left")]
        [ShifterFlag("TitleTextCentered", false)]
        //[RequiresUpgrade("shift_title_text")]
        [ShifterDescription("The title text location from the left of the titlebar.")]
        public Point TitleTextLeft = new Point(4, 4);

        


        #region Menus -> Toolbars
        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar Border")]
        public Color Menu_ToolStripBorder = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Dropdown background")]
        public Color Menu_ToolStripDropDownBackground = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient start")]
        public Color Menu_ToolStripGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient middle")]
        public Color Menu_ToolStripGradientMiddle = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Toolbar gradient end")]
        public Color Menu_ToolStripGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button select highlight")]
        public Color Menu_ButtonSelectedHighlight = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button select border")]
        public Color Menu_ButtonSelectedHighlightBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed highlight")]
        public Color Menu_ButtonPressedHighlight = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed border")]
        public Color Menu_ButtonPressedHighlightBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked highlight")]
        public Color Menu_ButtonCheckedHighlight = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked border")]
        public Color Menu_ButtonCheckedHighlightBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient border")]
        public Color Menu_ButtonPressedBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient border")]
        public Color Menu_ButtonSelectedBorder = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient start")]
        public Color Menu_ButtonCheckedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient middle")]
        public Color Menu_ButtonCheckedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button checked gradient end")]
        public Color Menu_ButtonCheckedGradientEnd = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient start")]
        public Color Menu_ButtonSelectedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient middle")]
        public Color Menu_ButtonSelectedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button selected gradient end")]
        public Color Menu_ButtonSelectedGradientEnd = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient start")]
        public Color Menu_ButtonPressedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient middle")]
        public Color Menu_ButtonPressedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Button pressed gradient end")]
        public Color Menu_ButtonPressedGradientEnd = Color.Black;




        #endregion

        #region Menus -> General
        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu text color")]
        public Color Menu_TextColor = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Menu selected text color")]
        public Color Menu_SelectedTextColor = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Rafter gradient start")]
        public Color Menu_RaftingContainerGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Rafter gradient end")]
        public Color Menu_RaftingContainerGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Separator Color 1")]
        public Color Menu_SeparatorDark = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Separator Color 2")]
        public Color Menu_SeparatorLight = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG")]
        public Color Menu_CheckBackground = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG (Selected)")]
        public Color Menu_CheckSelectedBackground = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("General")]
        [ShifterName("Check BG (Pressed)")]
        public Color Menu_CheckPressedBackground = Color.Black;




        #endregion

        #region Menus -> Menu Bars
        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient start")]
        public Color Menu_MenuItemPressedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient middle")]
        public Color Menu_MenuItemPressedGradientMiddle = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item pressed gradient end")]
        public Color Menu_MenuItemPressedGradientEnd = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient start")]
        public Color Menu_MenuItemSelectedGradientBegin = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Toolbars")]
        [ShifterName("Menu item selected gradient end")]
        public Color Menu_MenuItemSelectedGradientEnd = Color.Black;


        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient start")]
        public Color Menu_ImageMarginGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient middle")]
        public Color Menu_ImageMarginGradientMiddle = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Margin gradient end")]
        public Color Menu_ImageMarginGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu gradient start")]
        public Color Menu_MenuStripGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu gradient end")]
        public Color Menu_MenuStripGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item selected")]
        public Color Menu_MenuItemSelected = Color.Black;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu item border")]
        public Color Menu_MenuItemBorder = Color.White;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu bars")]
        [ShifterName("Menu border")]
        public Color Menu_MenuBorder = Color.White;




        #endregion

        #region Menus -> Status bars
        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient start")]
        public Color Menu_StatusStripGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Status bars")]
        [ShifterName("Status bar gradient end")]
        public Color Menu_StatusStripGradientEnd = Color.Gray;




        #endregion

        #region Menus -> Menu holders
        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Content panel gradient start")]
        public Color Menu_ToolStripContentPanelGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Content panel gradient end")]
        public Color Menu_ToolStripContentPanelGradientEnd = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Panel gradient start")]
        public Color Menu_ToolStripPanelGradientBegin = Color.Gray;

        [ShifterMeta("Menus")]
        [ShifterCategory("Menu holders")]
        [ShifterName("Panel gradient end")]
        public Color Menu_ToolStripPanelGradientEnd = Color.Gray;




        #endregion

        #region Windows -> Title Buttons -> Idle -> Images
        //Images
        [Image("closebutton")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning")]
        [ShifterDescription("Show an image on the Close Button using this setting.")]
        public byte[] CloseButtonImage = null;

        [Image("minimizebutton")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning")]
        [ShifterDescription("Show an image on the Minimize Button using this setting.")]
        public byte[] MinimizeButtonImage = null;

        [Image("maximizebutton")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning")]
        [ShifterDescription("Show an image on the Maximize Button using this setting.")]
        public byte[] MaximizeButtonImage = null;




        #endregion

        #region Windows -> Title Buttons -> Mouse Over -> Images
        //Images
        [Image("closebuttonover")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Over Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Close Button when the mouse hovers over it using this setting.")]
        public byte[] CloseButtonOverImage = null;

        [Image("minimizebuttonover")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Over Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Minimize Button when the mouse hovers over it using this setting.")]
        public byte[] MinimizeButtonOverImage = null;

        [Image("maximizebuttonover")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Over Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Maximize Button when the mouse hovers over it using this setting.")]
        public byte[] MaximizeButtonOverImage = null;




        #endregion

        #region Windows -> Title Buttons -> Mouse Down -> Images
        //Images
        [Image("closebuttondown")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Close Button Mouse Down Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Close Button when the mouse clicks it using this setting.")]
        public byte[] CloseButtonDownImage = null;

        [Image("minimizebuttondown")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Minimize Button Mouse Down Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Minimize Button when the mouse clicks it using this setting.")]
        public byte[] MinimizeButtonDownImage = null;

        [Image("maximizebuttondown")]
        [ShifterMeta("Windows")]
        [ShifterCategory("Title Buttons")]
        [ShifterName("Maximize Button Mouse Down Image")]
        //[RequiresUpgrade("shift_title_buttons;skinning;shift_states")]
        [ShifterDescription("Show an image on the Maximize Button when the mouse clicks it using this setting.")]
        public byte[] MaximizeButtonDownImage = null;




        #endregion


        #region Desktop -> Images
        [Image("desktopbackground")]
        [ShifterMeta("Desktop")]
        [ShifterCategory("General")]
        [ShifterName("Desktop Background Image")]
        //[RequiresUpgrade("skinning")]
        [ShifterDescription("Use an image as your desktop background.")]
        public byte[] DesktopBackgroundImage = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_app_launcher")]
        [ShifterName("App Launcher Text Color")]
        [ShifterDescription("Change the color of the App Launcher text.")]
        public Color AppLauncherTextColor = Color.Black;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_app_launcher")]
        [ShifterName("App Launcher Selected Text Color")]
        [ShifterDescription("Change the color of the app launcher's text while it is selected.")]
        public Color AppLauncherSelectedTextColor = Color.White;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_app_launcher")]
        [ShifterName("App Launcher Font")]
        [ShifterDescription("Change the font that the App Launcher text is displayed in.")]
        public Font AppLauncherFont = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);


        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher text")]
        [ShifterDescription("The text displayed on the app launcher.")]
        //[RequiresUpgrade("shift_app_launcher")]
        public string AppLauncherText = "Plex";

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher from left")]
        [ShifterDescription("The position of the app launcher from the left of the Desktop Panel.")]
        //[RequiresUpgrade("shift_app_launcher")]
        public Point AppLauncherFromLeft = new Point(0, 0);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher size")]
        [ShifterDescription("The size of the app launcher.")]
        //[RequiresUpgrade("shift_app_launcher")]
        public Size AppLauncherHolderSize = new Size(100, 24);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        [ShifterName("App launcher image")]
        [ShifterDescription("The image that will appear on the app launcher.")]
        [Image("applauncher")]
        //[RequiresUpgrade("skinning;shift_app_launcher")]
        public byte[] AppLauncherImage = null;




        #endregion



        [ShifterMeta("Desktop")]
        [ShifterCategory("Desktop Panel")]
        [ShifterName("Panel background image")]
        [Image("desktoppanel")]
        //[RequiresUpgrade("skinning;shift_desktop_panel")]
        public byte[] DesktopPanelBackground = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Titlebar background image")]
        [Image("titlebar")]
        //[RequiresUpgrade("skinning;shift_titlebar")]
        public byte[] TitleBarBackground = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("Titlebar inactive background image")]
        [Image("titlebarinactive")]
        //[RequiresUpgrade("skinning;shift_titlebar;shift_states")]
        public byte[] TitleBarInactiveBackground = null;


        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_titlebar")]
        [ShifterName("Show title corners?")]
        [ShifterDescription("If checked, a left and a right section will appear on the titlebar which is useful for rounded corners, padding, or other useful properties.")]
        public bool ShowTitleCorners = false;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left background color")]
        [ShifterDescription("What color should be used for the left title corner?")]
        public Color TitleLeftCornerBackground = Color.Gray;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left inactive background color")]
        [ShifterDescription("What color should be used for the left title corner when the window is inactive?")]
        public Color TitleInactiveLeftCornerBackground = Color.White;


        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right background color")]
        [ShifterDescription("What color should be used for the right title corner?")]
        public Color TitleRightCornerBackground = Color.Gray;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right inactive background color")]
        [ShifterDescription("What color should be used for the right title corner when the window is inactive?")]
        public Color TitleInactiveRightCornerBackground = Color.White;


        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left corner width")]
        [ShifterDescription("How wide should the left title corner be?")]
        public int TitleLeftCornerWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right corner width")]
        [ShifterDescription("How wide should the right title corner be?")]
        public int TitleRightCornerWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("skinning;shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left corner background image")]
        [ShifterDescription("Select an image to appear as the background texture for the left titlebar corner.")]
        [Image("titleleft")]
        public byte[] TitleLeftBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("skinning;shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title left corner inactive background image")]
        [ShifterDescription("Select an image to appear as the background texture for the left titlebar corner when the window is inactive.")]
        [Image("titleleftinactive")]
        public byte[] TitleLeftInactiveBG = null;


        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("skinning;shift_titlebar")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right corner background image")]
        [ShifterDescription("Select an image to appear as the background texture for the right titlebar corner.")]
        [Image("titleright")]
        public byte[] TitleRightBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        //[RequiresUpgrade("skinning;shift_titlebar;shift_states")]
        [ShifterFlag("ShowTitleCorners", true)]
        [ShifterName("Title right corner inactive background image")]
        [ShifterDescription("Select an image to appear as the background texture for the right titlebar corner when the window is inactive.")]
        [Image("titlerightinactive")]
        public byte[] TitleRightInactiveBG = null;


        [ShifterMeta("System")]
        [ShifterCategory("General")]
        [ShifterName("System color key-out")]
        [ShifterDescription("This is a color that will be represented as \"transparent\" in windows. This does not affect the desktop.")]
        public Color SystemKey = Color.FromArgb(1, 0, 1);

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders")]
        [Image("bottomborder")]
        [ShifterName("Bottom Border Image")]
        [ShifterDescription("Select an image to appear on the bottom border.")]
        public byte[] BottomBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("bottomborderinactive")]
        [ShifterName("Bottom Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the bottom border when the window is inactive. ")]
        public byte[] BottomBorderInactiveBG = null;


        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders")]
        [Image("bottomrborder")]
        [ShifterName("Bottom Right Border Image")]
        [ShifterDescription("Select an image to appear on the bottom right border.")]
        public byte[] BottomRBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders")]
        [Image("bottomlborder")]
        [ShifterName("Bottom Left Border Image")]
        [ShifterDescription("Select an image to appear on the bottom left border.")]
        public byte[] BottomLBorderBG = null;


        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("bottomrborderinactive")]
        [ShifterName("Bottom Right Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the bottom right border when the window is inactive.")]
        public byte[] BottomRBorderInactiveBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("bottomlborderinactive")]
        [ShifterName("Bottom Left Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the bottom left border when the window is inactive.")]
        public byte[] BottomLBorderInactiveBG = null;



        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders")]
        [Image("leftborder")]
        [ShifterName("Left Border Image")]
        [ShifterDescription("Select an image to appear on the left border.")]
        public byte[] LeftBorderBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders")]
        [Image("rightborder")]
        [ShifterName("Right Border Image")]
        [ShifterDescription("Select an image to appear on the right border.")]
        public byte[] RightBorderBG = null;


        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("leftborderinactive")]
        [ShifterName("Left Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the left border when the window is inactive.")]
        public byte[] LeftBorderInactiveBG = null;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("skinning;shift_window_borders;shift_states")]
        [Image("rightborderinactive")]
        [ShifterName("Right Border Inactive Image")]
        [ShifterDescription("Select an image to appear on the right border when the window is inactive.")]
        public byte[] RightBorderInactiveBG = null;



        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterName("Left border width")]
        [ShifterDescription("How wide should the left border be?")]
        public int LeftBorderWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterName("Right border width")]
        [ShifterDescription("How wide should the right border be?")]
        public int RightBorderWidth = 2;

        [ShifterMeta("Windows")]
        [ShifterCategory("Window border")]
        //[RequiresUpgrade("shift_window_borders")]
        [ShifterName("Bottom border height")]
        [ShifterDescription("How tall should the bottom border be?")]
        public int BottomBorderWidth = 2;

        [Image("panelbutton")]
        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        //[RequiresUpgrade("skinning;shift_panel_buttons")]
        [ShifterName("Panel button background image")]
        [ShifterDescription("Select a texture to display as the panel button background.")]
        public byte[] PanelButtonBG = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        //[RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button size")]
        [ShifterDescription("How big should the panel button be?")]
        public Size PanelButtonSize = new Size(185, 20);

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        //[RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button background color")]
        [ShifterDescription("Select a background color for the panel button.")]
        public Color PanelButtonColor = Color.Black;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        //[RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button text color")]
        [ShifterDescription("Select a text color for the panel button.")]
        public Color PanelButtonTextColor = Color.White;

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        //[RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button text from left")]
        [ShifterDescription("The position relative to the panel button left in pixels that the text is placed at.")]
        public Point PanelButtonFromLeft = new Point(2, 2);

        [ShifterMeta("Desktop")]
        [ShifterCategory("Panel buttons")]
        //[RequiresUpgrade("shift_panel_buttons")]
        [ShifterName("Panel button font")]
        [ShifterDescription("Select a font for the panel button.")]
        public Font PanelButtonFont = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);



        [ShifterMeta("Windows")]
        [ShifterCategory("Titlebar")]
        [ShifterName("App icon from side")]
        [ShifterDescription("How far from the side should the icon be?")]
        //[RequiresUpgrade("shift_titlebar;app_icons")]
        public Point TitlebarIconFromSide = new Point(4, 4);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Font")]
        [ShifterDescription("The font used by the status panel in the Advanced App Launcher.")]
        public Font ALStatusPanelFont = new Font("Microsoft Sans Serif", 9F);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Text Color")]
        [ShifterDescription("The text color for the AL status panel.")]
        public Color ALStatusPanelTextColor = Color.Black;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Background")]
        [ShifterDescription("The status panel's background color.")]
        public Color ALStatusPanelBackColor = Color.Gray;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Status Panel Text Alignment")]
        [ShifterDescription("What part of the panel should the status text stick to?")]
        public ContentAlignment ALStatusPanelAlignment = ContentAlignment.MiddleCenter;


        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL System Status Height")]
        [ShifterDescription("Set the height of the top system status bar in the App Launcher.")]
        public int ALSystemStatusHeight = 50;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL Size")]
        [ShifterDescription("Set the size of the App Launcher's container")]
        public Size AALSize = new Size(425, 500);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL Category View Width")]
        [ShifterDescription("Set the width of the left Category Listing on the app launcher.")]
        public int AALCategoryViewWidth = 237;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL Item List Width")]
        [ShifterDescription("Set the width of the item list in the app launcher.")]
        public int AALItemViewWidth = 237;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("AL System Actions Height")]
        [ShifterDescription("Set the height of the bottom system actions bar in the App Launcher.")]
        public int ALSystemActionHeight = 30;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("skinning;shift_advanced_app_launcher")]
        [ShifterName("Status Panel Background Image")]
        [ShifterDescription("Use an image for the App Launcher Status Panel")]
        [Image("al_bg_status")]
        public byte[] ALStatusPanelBG = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterEnumMask(new[] { "Button, bottom panel", "Button, top panel", "Category Item" })]
        [ShifterName("Shutdown Button position")]
        [ShifterDescription("Change the position and layout of the App Launcher Shutdown button.")]
        public int ShutdownButtonStyle = 0;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Shutdown Button from side")]
        [ShifterDescription("The location relative to the side of the system panel that the shutdown button should reside from.")]
        public Point ShutdownButtonFromSide = new Point(2, 2);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Align shutdown button to left?")]
        [ShifterDescription("Should the location of the shutdown button be calculated relative to the left of the system panel?")]
        public bool ShutdownOnLeft = false;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Shutdown Button Font")]
        [ShifterDescription("The font of the Shutdown Button")]
        public Font ShutdownFont = new Font("Microsoft Sans Serif", 9F);

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("Shutdown Text Color")]
        [ShifterDescription("The foreground color of the Shutdown button")]
        public Color ShutdownForeColor = Color.Black;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("System Panel background color")]
        [ShifterDescription("The background color of the App Launcher System Panel.")]
        public Color SystemPanelBackground = Color.Gray;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("skinning;shift_advanced_app_launcher")]
        [ShifterName("System Panel Background Image")]
        [ShifterDescription("The background image of the System Panel.")]
        [Image("al_bg_system")]
        public byte[] SystemPanelBG = null;

        [ShifterMeta("Desktop")]
        [ShifterCategory("App Launcher")]
        //[RequiresUpgrade("shift_advanced_app_launcher")]
        [ShifterName("App Launcher Item Font")]
        [ShifterDescription("Select the font to use for the items in the App Launcher.")]
        public Font AdvALItemFont = new Font("Microsoft Sans Serif", 9F);

    }
}
