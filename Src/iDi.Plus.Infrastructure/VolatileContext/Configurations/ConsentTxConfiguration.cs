using iDi.Plus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iDi.Plus.Infrastructure.VolatileContext.Configurations;

public class ConsentTxConfiguration : IEntityTypeConfiguration<ConsentTx>
{
    public void Configure(EntityTypeBuilder<ConsentTx> builder)
    {
        builder.ToTable("Consents");
        builder.HasOne(p => p.Request);
    }
}