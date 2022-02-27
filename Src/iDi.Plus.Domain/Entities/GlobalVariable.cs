using System;
using iDi.Blockchain.Framework.Entities;

namespace iDi.Plus.Domain.Entities;

public class GlobalVariable : EntityBase
{
    protected GlobalVariable()
    {}
    public GlobalVariable(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; private set; }
    public string Value { get; private set; }
    public DateTime LastModified { get; private set; }

    public void Set(string value)
    {
        Value = value;
        LastModified = DateTime.UtcNow;
    }
}