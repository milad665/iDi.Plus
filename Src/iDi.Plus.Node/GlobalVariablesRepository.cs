using System;
using System.Linq;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Entities;
using iDi.Plus.Node.Context;

namespace iDi.Plus.Node;

public class GlobalVariablesRepository : IGlobalVariablesRepository
{
    private readonly IdPlusDbContext _context;

    public GlobalVariablesRepository(IdPlusDbContext context)
    {
        _context = context;
    }

    public (string Value, DateTime LastModified) Get(string key)
    {
        var config =
            _context.GlobalVariables.FirstOrDefault(v => v.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        return config == null ? (null, DateTime.MinValue) : (config.Value, config.LastModified);
    }

    public void Set(string key, string value)
    {
        var config =
            _context.GlobalVariables.FirstOrDefault(v => v.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (config != null)
            config.Set(value);
        else
            _context.GlobalVariables.Add(new GlobalVariable(key, value));

        _context.SaveChanges();
    }
}