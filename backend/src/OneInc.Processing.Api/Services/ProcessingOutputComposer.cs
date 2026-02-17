using System.Text;
using OneInc.Processing.Api.Domain;
using OneInc.Processing.Api.Interfaces;

namespace OneInc.Processing.Api.Services;

/// <summary>
/// Creates the processed output format required by the UI.
/// </summary>
public sealed class ProcessingOutputComposer(ICharacterAggregationService characterAggregationService) : IProcessingOutputComposer
{
    /// <inheritdoc />
    public ProcessingResult Compose(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var builder = new StringBuilder();
        var items = characterAggregationService.Aggregate(input);
        //We build our output string as foreach{char,count}/base64(inputString) 
        foreach (var item in items)
        {
            builder.Append(item.Character);
            builder.Append(item.Count);
        }

        builder.Append('/');
        builder.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(input)));

        var output = builder.ToString();
        var outputCharacterCount = output.EnumerateRunes().Count();

        return new ProcessingResult(output, outputCharacterCount);
    }
}
