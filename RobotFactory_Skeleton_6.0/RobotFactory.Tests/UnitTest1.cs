using NUnit.Framework;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RobotFactory.Tests
{
    [TestFixture]
    public class Tests
    {
        private Factory factory;
        private string name = "test";
        private int capacity = 5;
        [SetUp]
        public void Setup()
        {
            this.factory = new Factory(this.name, capacity);
        }

        [Test]
        public void ConstructorShouldWorkProperly()
        {
            Assert.IsNotNull(this.factory);
            Assert.That(factory.Name == "test");
            Assert.That(factory.Robots != null);
            Assert.That(factory.Supplements != null);
            Assert.That(factory.Capacity == this.capacity);
        }
        [Test]
        public void ConstructorShouldInializeProperly()
        {
            Factory factory2 = new Factory(this.name, capacity);
            Assert.That(factory2.Robots.Count == 0);
            Assert.That(factory2.Supplements.Count == 0);
        }
        [Test]
        public void ProduceRobotShould_CheckRobotsCountAndReturnStringStatusIfThereisNoMoreSpace()
        {
            factory.ProduceRobot("Service", 5000, 2000);
            factory.ProduceRobot("Service2", 5002, 2002);
            factory.ProduceRobot("Service3", 5003, 2003);
            factory.ProduceRobot("Service4", 5004, 2004);
            factory.ProduceRobot("Service5", 5005, 2005); 
            string expectedOutput = factory.ProduceRobot("Service6", 5006, 2006);
            Assert.That(expectedOutput, Is.EqualTo($"The factory is unable to produce more robots for this production day!"));
        }
        [Test]
        public void ProduceRobotShould_AddNewRobotsToFactory()
        {
            factory.ProduceRobot("Service", 5000, 2000);
            factory.ProduceRobot("Service2", 5002, 2002);
            factory.ProduceRobot("Service3", 5003, 2003);
            // factory.ProduceRobot("Service4", 5004, 2004);
            Robot robot = new Robot("Service4", 5004, 2004);
            int expectedCount = 3;
            int actualRobotCount = factory.Robots.Count;
            Assert.That(actualRobotCount == expectedCount);
            string expected = $"Produced --> {robot}";
            Assert.AreEqual(expected, factory.ProduceRobot("Service4", 5004, 2004));
        }
        [Test]
        public void ProduceSupplementShould_AddSupplementToFactory()
        {
            factory.ProduceSupplement("Supplement1", 1);
            factory.ProduceSupplement("Supplement2", 2);
            factory.ProduceSupplement("Supplement3", 3);
            //factory.ProduceSupplement("Supplement4", 4);
            int expectedCount = 3;
            int actualCount = factory.Supplements.Count;
            Assert.That(actualCount == expectedCount);
            string expectedResult = "Supplement: Supplement4 IS: 4";
            string actualResult = factory.ProduceSupplement("Supplement4", 4);
            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void UpgradeRobotShould_CheckIfRobotAlreadyHaveCurrentSupplement()
        {
            factory.ProduceSupplement("Supplement1", 2000);
            factory.ProduceSupplement("Supplement1", 2000);
            factory.ProduceRobot("Service", 5000, 2000);
            Supplement supplement = factory.Supplements[0];
            Supplement supplement2 = factory.Supplements[1];
            Robot robot = factory.Robots[0];

            Assert.True(factory.UpgradeRobot(robot, supplement));
            Assert.True(factory.UpgradeRobot(robot, supplement2));
            Assert.False(factory.UpgradeRobot(robot, supplement2));
            Assert.That(robot.Supplements.Count > 0);
        }

        [Test]
        public void SellRobotShould_ReturnCorrectRobot()
        {
            factory.ProduceRobot("Service", 5000, 2000);
            factory.ProduceRobot("Service2", 5002, 2002);
            factory.ProduceRobot("Service3", 5003, 2003);
            factory.ProduceRobot("Service4", 5004, 2004);
            Assert.That(factory.SellRobot(5000) == factory.Robots.First(r => r.Price == 5000));
            Assert.That(factory.SellRobot(5004) == factory.Robots.First(r=>r.Price == 5004));
        }
        
    }

}