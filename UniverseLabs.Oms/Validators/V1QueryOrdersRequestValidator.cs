using FluentValidation;
using UniverseLabs.Oms.Models.Dto.V1.Requests;

namespace UniverseLabs.Oms.Validators;

public class V1QueryOrdersRequestValidator: AbstractValidator<V1QueryOrdersRequest>
{
    public V1QueryOrdersRequestValidator()
    {
        RuleFor(x => x.CustomerIds)
            .NotNull();
        
        RuleForEach(x => x.CustomerIds)
            .NotNull()
            .GreaterThan(0);
        
        RuleFor(x => x.Page)
            .GreaterThan(0);
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0);

        RuleFor(x => x.IncludeOrderItems)
            .NotNull();
    }
}