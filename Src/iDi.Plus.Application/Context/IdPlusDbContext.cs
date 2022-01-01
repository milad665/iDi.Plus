﻿using iDi.Plus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace iDi.Plus.Application.Context
{
    public class IdPlusDbContext : DbContext
    {
        private readonly string _dbPath;

        public DbSet<Node> Nodes { get; set; }
        public DbSet<KeyChange> KeyChangeList { get; set; }
        public DbSet<HotPoolRecord> HotPool { get; set; }

        public IdPlusDbContext()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _dbPath = Path.Join(path, "IdPlus.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={_dbPath}");

        public void ApplyMigrations(Action<IdPlusDbContext> seedMethod = null)
        {
            if (Database.GetPendingMigrations().Any())
                Database.Migrate();

            if (seedMethod != null)
                seedMethod(this);
        }
    }
}