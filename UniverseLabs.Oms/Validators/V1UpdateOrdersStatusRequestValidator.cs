using FluentValidation;
using UniverseLabs.Oms.Models.Dto.V1.Requests;

namespace UniverseLabs.Oms.Validators;

public class V1UpdateOrdersStatusRequestValidator : AbstractValidator<V1UpdateOrdersStatusRequest>
{
    public V1UpdateOrdersStatusRequestValidator()
    {
        RuleFor(x => x.OrderIds).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}