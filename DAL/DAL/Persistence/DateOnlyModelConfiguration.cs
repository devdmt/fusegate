using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Persistence;

internal static class DateOnlyModelConfiguration
{
    private static readonly ValueConverter<DateOnly, DateTime> DateOnlyConverter = new(
        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime));

    private static readonly ValueConverter<DateOnly?, DateTime?> NullableDateOnlyConverter = new(
        dateOnly => dateOnly.HasValue ? dateOnly.Value.ToDateTime(TimeOnly.MinValue) : null,
        dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null);

    internal static PropertyBuilder<DateOnly> HasDateOnlyConversion(this PropertyBuilder<DateOnly> builder) =>
        builder.HasColumnType("date").HasConversion(DateOnlyConverter);

    internal static PropertyBuilder<DateOnly?> HasDateOnlyConversion(this PropertyBuilder<DateOnly?> builder) =>
        builder.HasColumnType("date").HasConversion(NullableDateOnlyConverter);

    internal static void ApplyDateOnlyConversions(ModelBuilder builder)
    {
        try
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType != typeof(DateOnly))
                        continue;

                    property.SetColumnType("date");
                    property.SetValueConverter(
                        property.IsNullable ? NullableDateOnlyConverter : DateOnlyConverter);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}
