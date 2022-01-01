using iDi.Plus.Application.Context;
using iDi.Plus.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using iDi.Blockchain.Framework.Communication;

namespace iDi.Plus.Application
{
    public class Process
    {
        private readonly Settings _settings;
        private readonly IBlockchainNodeServer _blockchainNodeServer;
        private readonly IdPlusDbContext _context;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public Process(Settings settings, IBlockchainNodeServer blockchainNodeServer, IdPlusDbContext context)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _settings = settings;
            _blockchainNodeServer = blockchainNodeServer;
            _context = context;
        }

        public void Run()
        {
            _context.ApplyMigrations(Seed);
            LoadDnsNodes();
            UpdateBlockchain();

            _blockchainNodeServer.Listen(_settings.Port, _cancellationTokenSource.Token);
        }

        private List<Node> LoadDnsNodes() => _context.Nodes.Where(n => n.IsDns && n.TrustedIpAddress != null).ToList();

        private void UpdateBlockchain()
        {
            var node = _context.Nodes
                .OrderByDescending(n => n.LastHeartbeat)
                .FirstOrDefault(n => n.IsVerifierNode && n.LastHeartbeat != null);

            //Send update message to node (get new blocks)
            //Busy-wait for response
            //return after update
            throw new NotImplementedException();
        }

        private void Seed(IdPlusDbContext context)
        {
            if (!context.Nodes.Any())
            {
                //context.Nodes.Add(new Node(Guid.Parse("d7849370-8ebf-4026-bd0a-011f9a72ed47"), true, true, IPAddress.Parse("127.0.0.1")));

                context.SaveChanges();
            }
        }
    }
}