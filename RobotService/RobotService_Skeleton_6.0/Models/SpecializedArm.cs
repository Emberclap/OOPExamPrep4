using RobotService.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Models
{
    public class SpecializedArm : Supplement
    {
        private const int interfaceStandard = 10_045;
        private const int batteryUsage = 10_000;
        public SpecializedArm()
            : base(interfaceStandard, batteryUsage)
        {
        }

    }
}
