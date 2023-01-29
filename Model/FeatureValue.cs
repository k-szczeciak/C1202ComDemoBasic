using System;
using System.Collections.Generic;
using System.Text;

namespace C1202ComDemoBasic.Model
{
    class FeatureValue
    {
        public String valueText { get; set; }
        public String unit { get; set; }
        public Boolean isToleranceEnabled { get; set; } = false;
        public Boolean isInTolerance { get; set; } = false;
        public Boolean isInWarning { get; set; } = false;

    }
}
