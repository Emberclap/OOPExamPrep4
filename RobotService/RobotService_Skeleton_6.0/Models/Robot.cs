using RobotService.Models.Contracts;
using RobotService.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Models
{

    public abstract class Robot : IRobot
    {
        private string model;
        private int batteryCapacity;
        private int batteryLevel;
        private int convertionCapacityIndex;
        private List<int> interfaceStandards;

        protected Robot(string model, int batteryCapacity, int convertionCapacityIndex)
        {
            Model = model;
            BatteryCapacity = batteryCapacity;
            BatteryLevel = this.batteryCapacity;
            ConvertionCapacityIndex = convertionCapacityIndex;
            this.interfaceStandards = new List<int>();
        }

        public string Model
        {
            get => model;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new AggregateException(ExceptionMessages.ModelNullOrWhitespace);
                }
                model = value;
            }
        }
        public int BatteryCapacity
        {
            get => batteryCapacity;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ExceptionMessages.BatteryCapacityBelowZero);
                }
                batteryCapacity = value;
            }
        }

        public int BatteryLevel { get; private set; }

        public int ConvertionCapacityIndex { get; private set; }

        public IReadOnlyCollection<int> InterfaceStandards => interfaceStandards;

        public void Eating(int minutes)
        {
            this.BatteryLevel += this.ConvertionCapacityIndex * minutes;
            if (batteryCapacity > this.BatteryCapacity)
            {
                this.BatteryLevel = this.BatteryCapacity;
            }
        }

        public bool ExecuteService(int consumedEnergy)
        {
            if (this.BatteryLevel >= consumedEnergy)
            {
                this.BatteryLevel -= consumedEnergy;
                return true;
            }
            else
            {
                return false;
            }

        }

        public void InstallSupplement(ISupplement supplement)
        {
            this.interfaceStandards.Add(supplement.InterfaceStandard);
            this.batteryCapacity -= supplement.BatteryUsage;
            this.BatteryLevel -= supplement.BatteryUsage;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetType().Name} {this.Model}:");
            sb.AppendLine($"--Maximum battery capacity: {this.BatteryCapacity}");
            sb.AppendLine($"--Current battery level: {this.BatteryLevel}");
            sb.Append($"--Supplements installed: ");
            if (interfaceStandards.Count > 0)
            {
                sb.AppendLine(string.Join(" ", interfaceStandards));
            }
            else
            {
                sb.AppendLine("none");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
