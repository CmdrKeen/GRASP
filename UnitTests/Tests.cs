using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Structures;
using Structures.Model;

namespace UnitTests
{
  [TestFixture]
  public class UnitTests
  {
    [Test]
    public void TestEuc2d()
    {
      var grasp = new GRASP();

      var cities1 = new List<City> { new City(0, 0), new City(0, 0) };
      var cities2 = new List<City> { new City(1, 1), new City(2, 2) };
      var cities3 = new List<City> { new City(-1, -1), new City(1, 1) };

      Assert.AreEqual(0, grasp.Euc2d(cities1[0], cities1[1]));
      Assert.AreEqual(1, grasp.Euc2d(cities2[0], cities2[1]));
      Assert.AreEqual(3, grasp.Euc2d(cities3[0], cities3[1]));
    }

    [Test]
    public void TestCost()
    {
      var grasp = new GRASP();
      var cities1 = new List<City> {new City(0, 0), new City(1, 1), new City(2, 2), new City(3, 3)};

      Assert.AreEqual(1 * 2, grasp.Cost(new List<int>() { 0, 1 }, cities1));
      Assert.AreEqual(3 + 4, grasp.Cost(new List<int>() { 0, 1, 2, 3 }, cities1));
      Assert.AreEqual(4 * 2, grasp.Cost(new List<int>() { 0, 3 }, cities1));
    }

    [Test]
    public void TestStochasticTwoOpt()
    {
      var grasp = new GRASP();

      List<int> perm = Enumerable.Range(1, 10).ToList();
      for (int i = 0; i < 200; i++)
      {
        var other = grasp.StochasticTwoOpt(perm);
        Assert.AreEqual(perm.Count, other.Count);
        Assert.AreNotEqual(perm, other);
        Assert.AreNotSame(perm, other);
        foreach (var x in other)
        {
          Assert.IsTrue(perm.Contains(x));
        }
      }
    }

    [Test]
    public void TestLocalSearch()
    {
      var grasp = new GRASP(20);

      var cities1 = new List<City> {new City(0, 0), new City(3, 3), new City(1, 1), new City(2, 2), new City(4, 4)};

      var best = new Candidate();
      best.Vector = new List<int>() {0, 1, 2, 3, 4};
      best.Cost = grasp.Cost(best.Vector, cities1);
      var rs = grasp.LocalSearch(best, cities1);

      Assert.IsNotNull(rs);
      Assert.IsNotNull(rs.Vector);
      Assert.IsNotNull(rs.Cost);
      Assert.AreNotSame(best, rs);
      Assert.AreNotEqual(best.Vector, rs.Vector);
      Assert.AreNotEqual(best.Cost, rs.Cost);

      // No improvement
      grasp = new GRASP(10);
      best = new Candidate();
      best.Vector = new List<int>() {0, 2, 3, 1, 4};
      best.Cost = grasp.Cost(best.Vector, cities1);

      rs = grasp.LocalSearch(best, cities1);
      Assert.IsNotNull(rs);
      Assert.AreEqual(best.Cost, rs.Cost);
    }

    [Test]
    public void TestConstructRandomizedGreedySolution()
    {
      var grasp = new GRASP(20);
      var cities = new List<City> { new City(0, 0), new City(3, 3), new City(1, 1), new City(2, 2), new City(4, 4) };

      var rs = grasp.ConstructRandomizedGreedySolution(cities);
      Assert.IsNotNull(rs);
      Assert.IsNotNull(rs.Vector);
      Assert.IsNotNull(rs.Cost);
      Assert.AreEqual(cities.Count, rs.Vector.Count);
      foreach (var index in rs.Vector)
      {
        Assert.IsTrue(index >= 0 && index <= 4);
      }
    }

    [Test]
    public void TestSearch()
    {
      var grasp = new GRASP(50, 70, 0.3);

      var berlinData = new List<City>
      {
        new City(565, 575), new City(25, 185), new City(345, 750), new City(945, 685), new City(845, 655),
        new City(880, 660), new City(25, 230), new City(525, 1000), new City(580, 1175), new City(650, 1130), new City(1605, 620),
        new City(1220, 580), new City(1465, 200), new City(1530, 5), new City(845, 680), new City(725, 370), new City(145, 665),
        new City(415, 635), new City(510, 875), new City(560, 365), new City(300, 465), new City(520, 585), new City(480, 415),
        new City(835, 625), new City(975, 580), new City(1215, 245), new City(1320, 315), new City(1250, 400), new City(660, 180),
        new City(410, 250), new City(420, 555), new City(575, 665), new City(1150, 1160), new City(700, 580), new City(685, 595),
        new City(685, 610), new City(770, 610), new City(795, 645), new City(720, 635), new City(760, 650), new City(475, 960),
        new City(95, 260), new City(875, 920), new City(700, 500), new City(555, 815), new City(830, 485), new City(1170, 65),
        new City(830, 610), new City(605, 625), new City(595, 360), new City(1340, 725), new City(1740, 245)
      };

      var best = grasp.Search(berlinData);

      Assert.IsNotNull(best.Cost);
      Assert.IsTrue(best.Cost < 11000);
    }
  }
}
