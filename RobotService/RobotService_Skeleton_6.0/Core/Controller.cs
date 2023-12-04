using RobotService.Core.Contracts;
using RobotService.Models;
using RobotService.Models.Contracts;
using RobotService.Repositories;
using RobotService.Repositories.Contracts;
using RobotService.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Core
{
    public class Controller : IController
    {
        private IRepository<IRobot> robots;
        private IRepository<ISupplement> supplements;

        public Controller()
        {
            this.robots = new RobotRepository();
            this.supplements = new SupplementRepository();
        }

        public string CreateRobot(string model, string typeName)
        {
            IRobot robot;
            if (typeName != nameof(IndustrialAssistant) && typeName != nameof(DomesticAssistant))
            {
                return string.Format(OutputMessages.RobotCannotBeCreated, typeName);
            }
            else if (typeName == nameof(DomesticAssistant))
            {
                robot = new DomesticAssistant(model);
            }
            else
            {
                robot = new IndustrialAssistant(model);
            }
            robots.AddNew(robot);
            return string.Format(OutputMessages.RobotCreatedSuccessfully, typeName, model);

        }

        public string CreateSupplement(string typeName)
        {
            ISupplement supplement;
            if (typeName != nameof(SpecializedArm) && typeName != nameof(LaserRadar))
            {
                return string.Format(OutputMessages.SupplementCannotBeCreated, typeName);
            }
            else if (typeName == nameof(SpecializedArm))
            {
                supplement = new SpecializedArm();
            }
            else
            {
                supplement = new LaserRadar();
            }
            supplements.AddNew(supplement);
            return string.Format(OutputMessages.SupplementCreatedSuccessfully, typeName);

        }

        public string PerformService(string serviceName, int intefaceStandard, int totalPowerNeeded)
        {
            List <IRobot> robots = this.robots.Models()
                .Where(r => r.InterfaceStandards.Contains(intefaceStandard))
                .OrderByDescending(b => b.BatteryLevel)
                .ToList();
            if (robots.Count == 0)
            {
                return string.Format(OutputMessages.UnableToPerform, intefaceStandard);
            }
            int availablePower = robots.Sum(r => r.BatteryLevel);
            int robotCounter = 0;
            if (totalPowerNeeded > availablePower)
            {
                return string.Format(OutputMessages.MorePowerNeeded, serviceName, totalPowerNeeded - availablePower);
            }
            foreach (IRobot robot in robots)
            {
                if (robot.BatteryLevel >= totalPowerNeeded)
                {
                    robot.ExecuteService(totalPowerNeeded);
                    robotCounter++;
                    break;
                }
                else
                {
                    totalPowerNeeded -= robot.BatteryLevel;
                    robot.ExecuteService(robot.BatteryLevel);
                    robotCounter++;
                }
            }
            return string.Format(OutputMessages.PerformedSuccessfully, serviceName,robotCounter);
        }

        public string Report()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IRobot robot in robots.Models()
                .OrderByDescending(b=>b.BatteryLevel)
                .ThenBy(bc=>bc.BatteryCapacity))
            {
                sb.AppendLine(robot.ToString().TrimEnd());
            }
            return sb.ToString().TrimEnd();
        }

        public string RobotRecovery(string model, int minutes)
        {
            List<IRobot> robots = this.robots.Models()
                .Where(r=>r.Model == model)
                .Where(b=>b.BatteryCapacity / 2 > b.BatteryLevel)
                .ToList();
            int robotCounter = 0;
            foreach (IRobot robot in robots)
            {
                robot.Eating(minutes);
                robotCounter++;
            }
            return string.Format(OutputMessages.RobotsFed, robotCounter);
        }

        public string UpgradeRobot(string model, string supplementTypeName)
        {
            List <IRobot> robots = new List <IRobot>();
            int supplementValue = supplements.Models()
                .FirstOrDefault(s => s.GetType().Name == supplementTypeName).InterfaceStandard;
           
            foreach (var robot in this.robots.Models().Where(r=>r.Model == model))
            {
                if (!robot.InterfaceStandards.Contains(supplementValue))
                {
                    robots.Add(robot);
                }
            }
            
            if(robots.Count == 0)
            {
                return string.Format(OutputMessages.AllModelsUpgraded, model);
            }

            robots[0].InstallSupplement(supplements.FindByStandard(supplementValue));
            return string.Format(OutputMessages.UpgradeSuccessful, model, supplementTypeName);
        }
    }
}

