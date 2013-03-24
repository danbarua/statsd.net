﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statsd.net.Messages
{
  public class GraphiteLine
  {
    private string _name;
    private int _quantity;
    private long _epoc;

    public string Name { get { return _name; } }
    public int Quantity { get { return _quantity; } }

    public GraphiteLine(string name, int quantity)
    {
      _name = name;
      _quantity = quantity;
      _epoc = Utility.GetEpoch();
    }

    public GraphiteLine(string name, int quantity, long epoc)
    {
      _name = name;
      _quantity = quantity;
      _epoc = epoc;
    }

    public override string ToString()
    {
      return _name + " " + _quantity + " " + _epoc;
    }

    public static GraphiteLine Clone(GraphiteLine line)
    {
      return new GraphiteLine(line._name, line._quantity, line._epoc);
    }

    public static GraphiteLine[] CloneMany(GraphiteLine[] line)
    {
      return line.Select(p => GraphiteLine.Clone(p)).ToArray();
    }
  }
}