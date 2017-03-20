//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("2.6 MultiLineText_WithSuggestionPopupWin")]
    class Demo_MultiLineText_WithSuggestionPopupWin : DemoBase
    {
        LayoutFarm.CustomWidgets.TextBox textbox;
        SuggestionWindowMx sgBox;
        Point textBoxGlobalOffset;
        bool alreadyHasTextBoxGlobalOffset;
        Dictionary<char, List<string>> words = new Dictionary<char, List<string>>();
        protected override void OnStartDemo(SampleViewport viewport)
        {
            textbox = new LayoutFarm.CustomWidgets.TextBox(400, 300, true);
            textbox.SetLocation(20, 20);
            var textSplitter = new CustomWidgets.ContentTextSplitter();
            textbox.TextSplitter = textSplitter;
            sgBox = new SuggestionWindowMx(300, 200);
            sgBox.UserConfirmSelectedItem += new EventHandler(sgBox_UserConfirmSelectedItem);
            sgBox.ListItemKeyboardEvent += new EventHandler<UIKeyEventArgs>(sgBox_ListItemKeyboardEvent);
            sgBox.Hide();
            //------------------------------------
            //create special text surface listener
            var textSurfaceListener = new LayoutFarm.Text.TextSurfaceEventListener();
            textSurfaceListener.CharacterAdded += (s, e) => UpdateSuggestionList();
            textSurfaceListener.CharacterRemoved += (s, e) => UpdateSuggestionList();
            textSurfaceListener.PreviewArrowKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewArrowKeyDown);
            textSurfaceListener.PreviewEnterKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewEnterKeyDown);
            textbox.TextEventListener = textSurfaceListener;
            //------------------------------------ 

            viewport.AddContent(textbox);
            viewport.AddContent(sgBox.GetPrimaryUI());
            //------------------------------------ 
            BuildSampleCountryList();
        }

        void sgBox_ListItemKeyboardEvent(object sender, UIKeyEventArgs e)
        {
            //keyboard event occurs on list item in suggestion box
            //
            switch (e.UIEventName)
            {
                case UIEventName.KeyDown:
                    {
                        switch (e.KeyCode)
                        {
                            case UIKeys.Down:
                                sgBox.SelectedIndex++;
                                e.CancelBubbling = true;
                                break;
                            case UIKeys.Up:
                                sgBox.SelectedIndex--;
                                e.CancelBubbling = true;
                                break;
                            case UIKeys.Enter:
                                //use select some item
                                sgBox_UserConfirmSelectedItem(null, EventArgs.Empty);
                                e.CancelBubbling = true;
                                break;
                            default:
                                textbox.Focus();
                                break;
                        }
                    }
                    break;
            }
        }


        void textSurfaceListener_PreviewArrowKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //update selection in list box 
            switch (e.key)
            {
                case UIKeys.Down:
                    {
                        if (sgBox.Visible && sgBox.SelectedIndex < sgBox.ItemCount - 1)
                        {
                            sgBox.SelectedIndex++;
                            e.PreventDefault = true;
                        }
                    }
                    break;
                case UIKeys.Up:
                    {
                        if (sgBox.Visible && sgBox.SelectedIndex > 0)
                        {
                            sgBox.SelectedIndex--;
                            e.PreventDefault = true;
                        }
                    }
                    break;
            }
        }
        void textSurfaceListener_PreviewEnterKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //accept selected text
            if (!sgBox.Visible || sgBox.SelectedIndex < 0)
            {
                return;
            }
            sgBox_UserConfirmSelectedItem(null, EventArgs.Empty);
            e.PreventDefault = true;
        }
        void sgBox_UserConfirmSelectedItem(object sender, EventArgs e)
        {
            if (textbox.CurrentTextSpan != null)
            {
                textbox.ReplaceCurrentTextRunContent(currentLocalText.Length,
                    (string)sgBox.GetItem(sgBox.SelectedIndex).Tag);
                //------------------------------------- 
                //then hide suggestion list
                sgBox.ClearItems();
                sgBox.Hide();
                //-------------------------------------- 
            }

        }

        static string GetString(char[] buffer, LayoutFarm.Composers.TextSplitBound bound)
        {

            return new string(buffer, bound.startIndex, bound.length);
        }
        string currentLocalText = null;
        void UpdateSuggestionList()
        {
            //find suggestion words 
            this.currentLocalText = null;
            sgBox.ClearItems();
            if (textbox.CurrentTextSpan == null)
            {
                sgBox.Hide();
                return;
            }
            //-------------------------------------------------------------------------
            //sample parse ...
            //In this example  all country name start with Captial letter so ...
            string currentTextSpanText = textbox.CurrentTextSpan.Text.ToUpper();
            //analyze content
            var textBuffer = currentTextSpanText.ToCharArray();
            var results = new List<LayoutFarm.Composers.TextSplitBound>();
            results.AddRange(textbox.TextSplitter.ParseWordContent(textBuffer, 0, textBuffer.Length));
            //get last part of splited text
            int m = results.Count;
            if (m < 1)
            {
                return;
            }
            Composers.TextSplitBound lastSplitPart = results[m - 1];
            this.currentLocalText = GetString(textBuffer, lastSplitPart);
            //char firstChar = currentTextSpanText[0];
            char firstChar = currentLocalText[0];
            List<string> keywords;
            if (words.TryGetValue(firstChar, out keywords))
            {
                int j = keywords.Count;
                int listViewWidth = sgBox.Width;
                for (int i = 0; i < j; ++i)
                {
                    string choice = keywords[i].ToUpper();
                    if (choice.StartsWith(currentLocalText))
                    {
                        CustomWidgets.ListItem item = new CustomWidgets.ListItem(listViewWidth, 17);
                        item.BackColor = Color.LightGray;
                        item.Tag = item.Text = keywords[i];
                        sgBox.AddItem(item);
                    }
                }
            }
            if (sgBox.ItemCount > 0)
            {
                sgBox.Show();
                //TODO: implement selectedIndex suggestion hint here
                sgBox.SelectedIndex = 0;
                //move listview under caret position 
                var caretPos = textbox.CaretPosition;
                //temp fixed
                //TODO: review here
                if (!alreadyHasTextBoxGlobalOffset)
                {
                    this.textBoxGlobalOffset = textbox.GetGlobalLocation();
                    alreadyHasTextBoxGlobalOffset = true;
                }

                sgBox.SetLocation(textBoxGlobalOffset.X + caretPos.X, caretPos.Y + 70);
            }
            else
            {
                sgBox.Hide();
            }
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
            string[] seplist = keywordString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
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
    }

    class SuggestionWindowMx
    {
        LayoutFarm.CustomWidgets.ListView listView;
        LayoutFarm.CustomWidgets.UIFloatWindow floatWindow;

        public event EventHandler UserConfirmSelectedItem;
        public event EventHandler<UIKeyEventArgs> ListItemKeyboardEvent;

        public SuggestionWindowMx(int w, int h)
        {
            floatWindow = new CustomWidgets.UIFloatWindow(w, h);
            listView = new CustomWidgets.ListView(w, h);
            floatWindow.AddChild(listView);
            listView.ListItemMouseEvent += new CustomWidgets.ListView.ListItemMouseHandler(listView_ListItemMouseEvent);
            listView.ListItemKeyboardEvent += new CustomWidgets.ListView.ListItemKeyboardHandler(listView_ListItemKeyboardEvent);
        }

        void listView_ListItemKeyboardEvent(object sender, UIKeyEventArgs e)
        {
            if (ListItemKeyboardEvent != null)
            {
                ListItemKeyboardEvent(this, e);
            }
        }
        void listView_ListItemMouseEvent(object sender, UIMouseEventArgs e)
        {
            switch (e.UIEventName)
            {
                case UIEventName.DblClick:
                    if (UserConfirmSelectedItem != null)
                    {
                        UserConfirmSelectedItem(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        public void ClearItems()
        {
            this.listView.ClearItems();
        }

        public void SetLocation(int x, int y)
        {
            floatWindow.SetLocation(x, y);
        }
        public UIElement GetPrimaryUI()
        {
            return this.floatWindow;
        }
        public int ItemCount
        {
            get { return this.listView.ItemCount; }
        }
        public int SelectedIndex
        {
            get { return this.listView.SelectedIndex; }
            set { this.listView.SelectedIndex = value; }
        }
        public bool Visible
        {
            get
            {
                return this.floatWindow.Visible;
            }
        }
        public void Hide()
        {
            this.floatWindow.Visible = false;
        }
        public void Show()
        {
            floatWindow.Visible = true;
        }
        public CustomWidgets.ListItem GetItem(int index)
        {
            return this.listView.GetItem(index);
        }
        public void AddItem(CustomWidgets.ListItem item)
        {
            this.listView.AddItem(item);
        }
        public int Width
        {
            get { return this.listView.Width; }
        }
    }
}