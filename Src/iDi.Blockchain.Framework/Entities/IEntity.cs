using System;

namespace iDi.Blockchain.Framework.Entities;

public interface IEntity
{
    public long Id { get; }
    public DateTime TimeStamp { get; }
}