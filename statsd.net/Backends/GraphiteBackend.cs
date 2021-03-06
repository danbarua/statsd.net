﻿using statsd.net.shared.Messages;
using statsd.net.shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using statsd.net.shared;
using statsd.net.shared.Backends;
using log4net;

namespace statsd.net.Backends
{
  public class GraphiteBackend : IBackend
  {
    private UdpClient _client;
    private Task _completionTask;
    private bool _isActive;
    private ISystemMetricsService _systemMetrics;
    private ILog _log;
    
    public GraphiteBackend(string host, int port, ISystemMetricsService systemMetrics)
    {
      _log = SuperCheapIOC.Resolve<ILog>();
      var ipAddress = Utility.HostToIPv4Address(host);
      _client = new UdpClient();
      _client.Connect(ipAddress, port);
      _systemMetrics = systemMetrics;
      _completionTask = new Task(() => { _isActive = false; });
    }

    public bool IsActive
    {
      get { return _isActive; }
    }

    public int OutputCount
    {
      get { return 0; }
    }

    public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, GraphiteLine messageValue, ISourceBlock<GraphiteLine> source, bool consumeToAccept)
    {
      byte[] data = Encoding.ASCII.GetBytes(messageValue.ToString());
      try
      {
        _client.Send(data, data.Length);
        _systemMetrics.LogCount("backends.graphite.lines");
        _systemMetrics.LogCount("backends.graphite.bytes", data.Length);
      }
      catch (SocketException ex)
      {
        _log.Error("Failed to send packet to Graphite: " + ex.SocketErrorCode.ToString());
      }
      return DataflowMessageStatus.Accepted;
    }

    public void Complete()
    {
      _completionTask.Start();
    }

    public Task Completion
    {
      get { return _completionTask; }
    }

    public void Fault(Exception exception)
    {
      throw new NotImplementedException();
    }
  }
}
