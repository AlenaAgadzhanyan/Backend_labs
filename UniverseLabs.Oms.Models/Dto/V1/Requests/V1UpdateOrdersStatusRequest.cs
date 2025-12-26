using UniverseLabs.Oms.Models.Enums;

namespace UniverseLabs.Oms.Models.Dto.V1.Requests;

public class V1UpdateOrdersStatusRequest
{
    public long[] OrderIds { get; set; }

    public OrderStatus NewStatus { get; set; }
}
