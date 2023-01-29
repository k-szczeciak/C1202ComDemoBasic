using System;
using System.Collections.Generic;
using System.Text;

namespace C1202ComDemoBasic.Model
{
    class dataDisplModel
    {
        public String ValueText { get; set; }
        public String BackgroundColor { get; set; }
        public String DateTime { get; set; }

        public dataDisplModel(String ValueText, String BackgroundColor, String DateTime)
        {
            this.ValueText = ValueText;
            this.BackgroundColor = BackgroundColor;
            this.DateTime = DateTime;
        }
    }
}
