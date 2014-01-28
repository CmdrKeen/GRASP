using System;
using System.Collections.Generic;
using System.Linq;
using Structures.Model;

namespace Structures
{

  public struct Candidate
  {
    public double Cost { get; set; }
    public List<int> Vector { get; set; }
  }

  public class GRASP
  {
    private readonly int _maxIterations = 500;
    private readonly int MaxNoImprovements = 50;
    private readonly double GreedinessFactor = 0.3;
    private readonly Random _random = new Random();


//    private int[,] _berlinData = new int[,]
//      {
//        {565, 575}, {25, 185}, {345, 750}, {945, 685}, {845, 655},
//        {880, 660}, {25, 230}, {525, 1000}, {580, 1175}, {650, 1130}, {1605, 620},
//        {1220, 580}, {1465, 200}, {1530, 5}, {845, 680}, {725, 370}, {145, 665},
//        {415, 635}, {510, 875}, {560, 365}, {300, 465}, {520, 585}, {480, 415},
//        {835, 625}, {975, 580}, {1215, 245}, {1320, 315}, {1250, 400}, {660, 180},
//        {410, 250}, {420, 555}, {575, 665}, {1150, 1160}, {700, 580}, {685, 595},
//        {685, 610}, {770, 610}, {795, 645}, {720, 635}, {760, 650}, {475, 960},
//        {95, 260}, {875, 920}, {700, 500}, {555, 815}, {830, 485}, {1170, 65},
//        {830, 610}, {605, 625}, {595, 360}, {1340, 725}, {1740, 245}
//      };

    private readonly List<City> _berlinData = new List<City>
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

    public GRASP(int maxIterations = 50, int maxNoImprovements = 50, double greedinessFactor = 0.3)
    {
      _maxIterations = maxIterations;
      MaxNoImprovements = maxNoImprovements;
      GreedinessFactor = greedinessFactor;
    }

    public void RunWithTestData()
    {
      var best = Search(_berlinData);
      Console.WriteLine("Done. Best solution: c=#{0}, v=#{1}", best.Cost, best.Vector);
    }

    /// <summary>
    /// Main call for this 
    /// </summary>
    public Candidate Search(List<City> cities)
    {
      // Create a cruddy dummy instace to immediately disregard
      // TODO: Refactor to something nicer
      var best = new Candidate {Cost = int.MaxValue};

      for (int i = 0; i < _maxIterations; i++)
      {
        var candidate = ConstructRandomizedGreedySolution(cities);
        candidate = LocalSearch(candidate, cities);
        best = (candidate.Cost < best.Cost) ? candidate : best;

       // if (i%100000 == 0)
       // {
          Console.WriteLine(" > iteration #{0}, best=#{1}", i + 1, best.Cost);
        //}
      }

      return best;
    }

    public Candidate ConstructRandomizedGreedySolution(List<City> cities)
    {
      var candidate = new Candidate();
     // int randomSize = _random.Next(cities.Length);
      candidate.Vector = new List<int>();
      candidate.Vector.Add(_random.Next(cities.Count));

      var allCities = new List<int>();
      for (int i = 0; i < cities.Count; i++)
      {
        allCities.Add(i);
      }

      // Build a path until we have all cities in the path
      while (candidate.Vector.Count < cities.Count)
      {
        var candidates = allCities.Where(x=>!candidate.Vector.Contains(x)).ToArray();
        var costs = new List<double>();
        for (int i = 0; i < candidates.Length; i++)
        {
          costs.Add(Euc2d(cities[candidate.Vector.Last()], cities[i]));
        }
        var rcl = new List<int>();
        var max = costs.Max();
        var min = costs.Min();

        for (int i = 0; i < costs.Count; i++)
        {
          if (costs[i] <= (min + GreedinessFactor*(max - min)))
          {
            rcl.Add(candidates[i]);
          }
        }

        candidate.Vector.Add(rcl[_random.Next(rcl.Count)]);
      }
      candidate.Cost = Cost(candidate.Vector, cities);
      return candidate;
    }

    public Candidate LocalSearch(Candidate best, List<City> cities)
    {
      // reference to max_no_improve
      int count = 0;
      do
      {
        var candidate = new Candidate();
        candidate.Vector = StochasticTwoOpt(best.Vector);
        candidate.Cost = Cost(candidate.Vector, cities);
        count = (candidate.Cost < best.Cost) ? 0 : count + 1;
        
        if (candidate.Cost < best.Cost)
        {
          best = candidate;
        }

      } while (count >= MaxNoImprovements);

      return best;
    }

    public List<int> StochasticTwoOpt(List<int> permutation)
    {
      // TODO: I don't think these are cities, just positions in the array to jumble up
      var exclude = new List<int>();
      var perm = new List<int>();

      perm.AddRange(permutation);

      var city1 = _random.Next(perm.Count);
      var city2 = _random.Next(perm.Count);

      exclude.Add(city1);
      exclude.Add(city1 == 0 ? perm.Count - 1 : city1 - 1);
      exclude.Add((city1 == perm.Count - 1) ? 0 : city1 + 1);

      // Get a new city2, while city2 isn't in the exclusions list
      while (exclude.Contains(city2))
      {
        city2 = _random.Next(perm.Count);
      }

      // Reverse order if city1 is larger than city2
      if (city2 < city1)
      {
        int holder = city1;
        city1 = city2;
        city2 = holder;
      }

      // Reverse a section of the array
      // NOTE: Check this is only reversing the part we want
      perm.Reverse(city1, city2 - city1);

      return perm;
    }

    public double Cost(List<int> permutation, List<City> cities)
    {
      // TODO: I think we can round to an int and be fine
      double distance = 0;
      for (int i = 0; i < permutation.Count; i++)
      {
        var city1 = permutation[i];
        var city2 = (i == permutation.Count - 1) ? permutation[0] : permutation[i + 1];
        distance += Euc2d(cities[city1], cities[city2]);
      }
      return distance;
    }

    public double Euc2d(City city1, City city2)
    {
      return Math.Round(Math.Sqrt(Math.Pow((city1.X - city2.X),2) + Math.Pow((city1.Y - city2.Y),2)));
    }
  }
}
