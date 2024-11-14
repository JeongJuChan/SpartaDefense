public interface IAttribute
{
    AttributeAppliedData AttributeAppliedData { get; }
    void AddAttributeStats(AttributeAppliedData attributeAppliedData);
    void SubtractAttributeStats(AttributeAppliedData attributeAppliedData);
}