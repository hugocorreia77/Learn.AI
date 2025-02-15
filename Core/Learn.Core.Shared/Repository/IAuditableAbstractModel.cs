using Learn.Core.Shared.Models.User;

namespace Learn.Core.Shared.Repository
{
    public interface IAuditableAbstractModel
    {
        UserReference CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        UserReference? UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
        UserReference? DeletedBy { get; set; }
        DateTime? DeletedOn { get; set; }
        bool IsDeleted { get; set; }
    }
}
