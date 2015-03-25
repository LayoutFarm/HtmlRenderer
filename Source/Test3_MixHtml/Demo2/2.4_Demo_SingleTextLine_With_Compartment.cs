// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{
    [DemoNote("2.4 Demo_SingleTextLine_With_Compartment")]
    class Demo_SingleTextLine_With_Compartment : DemoBase
    {
        LayoutFarm.CustomWidgets.TextBox textbox;
        LayoutFarm.CustomWidgets.ListView listView;
        Dictionary<char, List<string>> words = new Dictionary<char, List<string>>();

        UINinespaceBox ninespaceBox;
        protected override void OnStartDemo(SampleViewport viewport)
        {

            //--------------------------------
            {
                //background element
                var bgbox = new LayoutFarm.CustomWidgets.EaseBox(800, 600);
                bgbox.BackColor = Color.White;
                bgbox.SetLocation(0, 0);
                SetupBackgroundProperties(bgbox);
                viewport.AddContent(bgbox);
            }
            //--------------------------------
            //ninespace compartment
            ninespaceBox = new UINinespaceBox(800, 600);
            viewport.AddContent(ninespaceBox);
            ninespaceBox.SetSize(800, 600);
            //--------------------------------
            //test add some content to the ninespace box


            textbox = new LayoutFarm.CustomWidgets.TextBox(400, 30, false);
            listView = new CustomWidgets.ListView(300, 200);
            listView.SetLocation(0, 40);
            listView.Visible = false;
            //------------------------------------
            //create special text surface listener
            var textSurfaceListener = new LayoutFarm.Text.TextSurfaceEventListener();
            textSurfaceListener.CharacterAdded += (s, e) => UpdateSuggestionList();
            textSurfaceListener.CharacterRemoved += (s, e) => UpdateSuggestionList();
            textSurfaceListener.PreviewArrowKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewArrowKeyDown);
            textSurfaceListener.PreviewEnterKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewEnterKeyDown);
            textbox.TextEventListener = textSurfaceListener;
            //------------------------------------ 

            //------------------------------------ 
            BuildSampleCountryList();
            ninespaceBox.LeftSpace.AddChildBox(textbox);
            ninespaceBox.RightSpace.AddChildBox(listView);

        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.EaseBox backgroundBox)
        {

        }

        void textSurfaceListener_PreviewArrowKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //update selection in list box
            switch (e.key)
            {
                case UIKeys.Down:
                    {
                        if (listView.SelectedIndex < listView.ItemCount - 1)
                        {
                            listView.SelectedIndex++;
                        }

                    } break;
                case UIKeys.Up:
                    {
                        if (listView.SelectedIndex > 0)
                        {
                            listView.SelectedIndex--;
                        }

                    } break;
            }

        }
        void textSurfaceListener_PreviewEnterKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //accept selected text 
            if (textbox.CurrentTextSpan != null)
            {

                ListItem selectedItem = listView.GetItem(listView.SelectedIndex);
                if (selectedItem != null)
                {
                    textbox.ReplaceCurrentTextRunContent(textbox.CurrentTextSpan.CharacterCount,
                        (string)selectedItem.Tag);
                    //------------------------------------- 
                    //then hide suggestion list
                    listView.ClearItems();
                    listView.Visible = false;
                    //--------------------------------------
                }
                e.Canceled = true;
            }
        }
        void UpdateSuggestionList()
        {
            //find suggestion words 
            listView.ClearItems();
            if (textbox.CurrentTextSpan == null)
            {
                listView.Visible = false;
                return;
            }
            //-------------------------------------------------------------------------
            //In this example  all country name start with Captial letter so ...
            string currentTextSpanText = textbox.CurrentTextSpan.Text.ToUpper();
            char firstChar = currentTextSpanText[0];

            List<string> keywords;
            if (words.TryGetValue(firstChar, out keywords))
            {
                int j = keywords.Count;
                int listViewWidth = listView.Width;
                for (int i = 0; i < j; ++i)
                {
                    string choice = keywords[i].ToUpper();
                    if (choice.StartsWith(currentTextSpanText))
                    {
                        CustomWidgets.ListItem item = new CustomWidgets.ListItem(listViewWidth, 17);
                        item.BackColor = Color.LightGray;
                        item.Tag = item.Text = keywords[i];

                        listView.AddItem(item);
                    }
                }
            }
            listView.Visible = true;
            //-------------------------------------------------------------------------
        }

        void BuildSampleCountryList()
        {

            AddKeywordList(@"
Afghanistan
Albania
Algeria
American Samoa
Andorra
Angola
Anguilla
Antarctica
Antigua and Barbuda
Argentina
Armenia
Aruba
Australia
Austria
Azerbaijan
Bahamas
Bahrain
Bangladesh
Barbados
Belarus
Belgium
Belize
Benin
Bermuda
Bhutan
Bolivia
Bosnia and Herzegovina
Botswana
Brazil
Brunei Darussalam
Bulgaria
Burkina Faso
Burundi
Cambodia
Cameroon
Canada
Cape Verde
Cayman Islands
Central African Republic
Chad
Chile
China
Christmas Island
Cocos (Keeling) Islands
Colombia
Comoros
Democratic Republic of the Congo (Kinshasa)
'Congo, Republic of (Brazzaville)'
Cook Islands
Costa Rica
Ivory Coast
Croatia
Cuba
Cyprus
Czech Republic
Denmark
Djibouti
Dominica
Dominican Republic
East Timor (Timor-Leste)
Ecuador
Egypt
El Salvador
Equatorial Guinea
Eritrea
Estonia
Ethiopia
Falkland Islands
Faroe Islands
Fiji
Finland
France
French Guiana
French Polynesia
French Southern Territories
Gabon
Gambia
Georgia
Germany
Ghana
Gibraltar
Great Britain
Greece
Greenland
Grenada
Guadeloupe
Guam
Guatemala
Guinea
Guinea-Bissau
Guyana
Haiti
Holy See
Honduras
Hong Kong
Hungary
Iceland
India
Indonesia
Iran (Islamic Republic of)
Iraq
Ireland
Israel
Italy
Jamaica
Japan
Jordan
Kazakhstan
Kenya
Kiribati
'Korea, Democratic People's Rep. (North Korea)'
'Korea, Republic of (South Korea)'
Kuwait
Kyrgyzstan
'Lao, People's Democratic Republic'
Latvia
Lebanon
Lesotho
Liberia
Libya
Liechtenstein
Lithuania
Luxembourg
Macau
'Macedonia, Rep. of'
Madagascar
Malawi
Malaysia
Maldives
Mali
Malta
Marshall Islands
Martinique
Mauritania
Mauritius
Mayotte
Mexico
'Micronesia, Federal States of'
'Moldova, Republic of'
Monaco
Mongolia
Montenegro
Montserrat
Morocco
Mozambique
'Myanmar, Burma'
Namibia
Nauru
Nepal
Netherlands
Netherlands Antilles
New Caledonia
New Zealand
Nicaragua
Niger
Nigeria
Niue
Northern Mariana Islands
Norway
Oman
Pakistan
Palau
Palestinian territories
Panama
Papua New Guinea
Paraguay
Peru
Philippines
Pitcairn Island
Poland
Portugal
Puerto Rico
Qatar
Reunion Island
Romania
Russian Federation
Rwanda
Saint Kitts and Nevis
Saint Lucia
Saint Vincent and the Grenadines
Samoa
San Marino
Sao Tome and Principe
Saudi Arabia
Senegal
Serbia
Seychelles
Sierra Leone
Singapore
Slovakia (Slovak Republic)
Slovenia
Solomon Islands
Somalia
South Africa
South Sudan
Spain
Sri Lanka
Sudan
Suriname
Swaziland
Sweden
Switzerland
'Syria, Syrian Arab Republic'
Taiwan (Republic of China)
Tajikistan
Tanzania; officially the United Republic of Tanzania
Thailand
Tibet
Timor-Leste (East Timor)
Togo
Tokelau
Tonga
Trinidad and Tobago
Tunisia
Turkey
Turkmenistan
Turks and Caicos Islands
Tuvalu
Uganda
Ukraine
United Arab Emirates
United Kingdom
United States
Uruguay
Uzbekistan
Vanuatu
Vatican City State (Holy See)
Venezuela
Vietnam
Virgin Islands (British)
Virgin Islands (U.S.)
Wallis and Futuna Islands
Western Sahara
Yemen
Zambia
Zimbabwe");
        }
        void AddKeywordList(string keywordString)
        {

            string[] seplist = keywordString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int j = seplist.Length;
            for (int i = 0; i < j; ++i)
            {
                string sepWord = seplist[i];
                if (sepWord.StartsWith("'"))
                {
                    sepWord = sepWord.Substring(1, sepWord.Length - 2);
                }
                char firstChar = sepWord[0];
                List<string> list;
                if (!words.TryGetValue(firstChar, out list))
                {
                    list = new List<string>();
                    words.Add(firstChar, list);
                }
                list.Add(sepWord);
                list.Sort();
            }
        }


        class UINinespaceBox : LayoutFarm.CustomWidgets.EaseBox
        {
            Panel boxLeftTop;
            Panel boxRightTop;
            Panel boxLeftBottom;
            Panel boxRightBottom;
            //-------------------------------------
            Panel boxLeft;
            Panel boxTop;
            Panel boxRight;
            Panel boxBottom;
            //-------------------------------------
            Panel centerBox;



            EaseBox gripperLeft;
            EaseBox gripperRight;
            EaseBox gripperTop;
            EaseBox gripperBottom;


            DockSpacesController dockspaceController;
            NinespaceGrippers ninespaceGrippers;
            public UINinespaceBox(int w, int h)
                : base(w, h)
            {
                SetupDockSpaces();
            }
            void SetupDockSpaces()
            {
                //1. controller
                this.dockspaceController = new DockSpacesController(this, SpaceConcept.NineSpace);

                //2.  
                this.dockspaceController.LeftTopSpace.Content = boxLeftTop = CreateSpaceBox(SpaceName.LeftTop, Color.Red);
                this.dockspaceController.RightTopSpace.Content = boxRightTop = CreateSpaceBox(SpaceName.RightTop, Color.Red);
                this.dockspaceController.LeftBottomSpace.Content = boxLeftBottom = CreateSpaceBox(SpaceName.LeftBottom, Color.Red);
                this.dockspaceController.RightBottomSpace.Content = boxRightBottom = CreateSpaceBox(SpaceName.RightBottom, Color.Red);
                //3.
                this.dockspaceController.LeftSpace.Content = boxLeft = CreateSpaceBox(SpaceName.Left, Color.Blue);
                this.dockspaceController.TopSpace.Content = boxTop = CreateSpaceBox(SpaceName.Top, Color.Yellow);
                this.dockspaceController.RightSpace.Content = boxRight = CreateSpaceBox(SpaceName.Right, Color.Green);
                this.dockspaceController.BottomSpace.Content = boxBottom = CreateSpaceBox(SpaceName.Bottom, Color.Yellow);


                //--------------------------------
                //left and right space expansion
                dockspaceController.LeftSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
                dockspaceController.RightSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
                dockspaceController.SetRightSpaceWidth(200);
                dockspaceController.SetLeftSpaceWidth(200);

                //------------------------------------------------------------------------------------
                this.ninespaceGrippers = new NinespaceGrippers(this.dockspaceController);
                this.ninespaceGrippers.LeftGripper = gripperLeft = CreateGripper(Color.Red, false);
                this.ninespaceGrippers.RightGripper = gripperRight = CreateGripper(Color.Red, false);
                this.ninespaceGrippers.TopGripper = gripperTop = CreateGripper(Color.Red, true);
                this.ninespaceGrippers.BottomGripper = gripperBottom = CreateGripper(Color.Red, true);
                this.ninespaceGrippers.UpdateGripperPositions();
                //------------------------------------------------------------------------------------
            }

            CustomWidgets.EaseBox CreateGripper(PixelFarm.Drawing.Color bgcolor, bool isVertical)
            {
                int controllerBoxWH = 10;
                CustomWidgets.EaseBox gripperBox = new CustomWidgets.EaseBox(controllerBoxWH, controllerBoxWH);
                gripperBox.BackColor = bgcolor;
                ////---------------------------------------------------------------------
                gripperBox.MouseLeave += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        Point pos = gripperBox.Position;
                        if (isVertical)
                        {
                            gripperBox.SetLocation(pos.X, pos.Y + e.YDiff);
                        }
                        else
                        {
                            gripperBox.SetLocation(pos.X + e.XDiff, pos.Y);
                        }
                        this.ninespaceGrippers.UpdateNinespaces();
                        e.MouseCursorStyle = MouseCursorStyle.Pointer;
                        e.CancelBubbling = true;
                    }
                };
                gripperBox.MouseMove += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        Point pos = gripperBox.Position;
                        if (isVertical)
                        {
                            gripperBox.SetLocation(pos.X, pos.Y + e.YDiff);
                        }
                        else
                        {
                            gripperBox.SetLocation(pos.X + e.XDiff, pos.Y);
                        }

                        this.ninespaceGrippers.UpdateNinespaces();
                        e.MouseCursorStyle = MouseCursorStyle.Pointer;
                        e.CancelBubbling = true;
                    }
                };
                gripperBox.MouseUp += (s, e) =>
                {
                    e.MouseCursorStyle = MouseCursorStyle.Default;
                    e.CancelBubbling = true;
                };

                return gripperBox;
            }
            static CustomWidgets.Panel CreateSpaceBox(SpaceName name, Color bgcolor)
            {
                int controllerBoxWH = 10;
                CustomWidgets.Panel spaceBox = new CustomWidgets.Panel(controllerBoxWH, controllerBoxWH);
                spaceBox.BackColor = bgcolor;
                spaceBox.Tag = name;
                return spaceBox;
            }

            public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
            {
                if (!this.HasReadyRenderElement)
                {

                    var myRenderElement = base.GetPrimaryRenderElement(rootgfx) as LayoutFarm.CustomWidgets.CustomRenderBox;
                    PlainLayer plain0 = myRenderElement.GetDefaultLayer();
                     
                    //------------------------------------------------------
                    plain0.AddChild(boxLeftTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRightTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxLeftBottom.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRightBottom.GetPrimaryRenderElement(rootgfx));
                    //------------------------------------------------------
                    plain0.AddChild(boxLeft.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxRight.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(boxBottom.GetPrimaryRenderElement(rootgfx));

                    //grippers
                    plain0.AddChild(gripperLeft.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperRight.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperTop.GetPrimaryRenderElement(rootgfx));
                    plain0.AddChild(gripperBottom.GetPrimaryRenderElement(rootgfx));

                    //------------------------------------------------------
                }
                return base.GetPrimaryRenderElement(rootgfx);
            }

            public override void SetSize(int width, int height)
            {
                base.SetSize(width, height);
                dockspaceController.SetSize(width, height);

            }

            public Panel LeftSpace { get { return this.boxLeft; } }
            public Panel RightSpace { get { return this.boxRight; } }
            public Panel TopSpace { get { return this.boxTop; } }
            public Panel BottomSpace { get { return this.boxBottom; } }



        }


    }
}