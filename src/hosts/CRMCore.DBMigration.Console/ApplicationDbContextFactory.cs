﻿using CRMCore.Module.Data;
using CRMCore.Module.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace CRMCore.DBMigration.Console
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly IExtendDbContextOptionsBuilder _extendOptionsBuilder;
        private readonly IDatabaseConnectionStringFactory _dbConnectionStringFactory;
        private readonly IConfiguration _config;

        public ApplicationDbContextFactory()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Startup>()
                .Build();

            _extendOptionsBuilder = new SqlServerDbContextOptionsBuilderFactory();
            _dbConnectionStringFactory = new SqlServerDatabaseConnectionStringFactory(_config);
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var migrationAssembly = typeof(ApplicationDbContextFactory).GetTypeInfo().Assembly;

            var dbContextOptionBuilder = _extendOptionsBuilder.Extend(
                new DbContextOptionsBuilder<ApplicationDbContext>(),
                _dbConnectionStringFactory.Create(),
                migrationAssembly.GetName().Name);

            return (ApplicationDbContext)Activator.CreateInstance(
                typeof(ApplicationDbContext),
                dbContextOptionBuilder.Options);
        }
    }
}