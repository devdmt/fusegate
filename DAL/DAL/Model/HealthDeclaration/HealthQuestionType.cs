using System.Text.Json.Serialization;

namespace DAL.Model.HealthDeclaration;

/// <summary>
/// Supported health declaration question types. Only these exact values are accepted in the API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HealthQuestionType
{
    PriorDeclinedInsurance = 0,
    ExistingConditions = 1,
    DrugAlcoholAbuse = 2,
    Respiratory = 3,
    HeartCirculation = 4,
    ChronicConditions = 5,
    Wellness = 6,
    ImmuneViral = 7,
    Senses = 8
}
