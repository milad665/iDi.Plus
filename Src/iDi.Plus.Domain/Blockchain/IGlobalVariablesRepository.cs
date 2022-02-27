using System;

namespace iDi.Plus.Domain.Blockchain;

public interface IGlobalVariablesRepository
{
    (string Value, DateTime LastModified) Get(string key);
    void Set(string key, string value);
}