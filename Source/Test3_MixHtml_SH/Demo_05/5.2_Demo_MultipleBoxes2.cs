//Apache2, 2014-present, WinterDev

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.2 MultpleBox2")]
    class Demo_MultipleBox2 : HtmlDemoBase
    {
        LayoutFarm.HtmlWidgets.ChoiceBox currentSingleCheckedBox;
        protected override void OnHtmlHostCreated()
        {
            int boxHeight = 35;
            //-------------------------------------------------------------------
            int boxX = 0;
            for (int i = 0; i < 2; ++i)
            {
                var button = new LayoutFarm.HtmlWidgets.Button(100, boxHeight);
                button.SetLocation(boxX, 20);
                button.Text = "button" + i;
                boxX += 100 + 2;
                AddToViewport(button);
            }
            int boxY = 70;
            for (int i = 0; i < 2; ++i)
            {
                var statedBox = new LayoutFarm.HtmlWidgets.ChoiceBox(100, boxHeight);
                //statedBox.Text = "chk" + i;
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;
                AddToViewport(statedBox);
                statedBox.CheckValueAssigned += (s, e) =>
                {
                    var selectedBox = (LayoutFarm.HtmlWidgets.ChoiceBox)s;
                    if (selectedBox != currentSingleCheckedBox)
                    {
                        if (currentSingleCheckedBox != null)
                        {
                            currentSingleCheckedBox.Checked = false;
                        }
                        currentSingleCheckedBox = selectedBox;
                    }
                };
            }
        }
    }
}