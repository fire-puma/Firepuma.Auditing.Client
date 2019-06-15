using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firepuma.Auditing.Abstractions.Responses;

namespace Firepuma.Auditing.Client
{
    public interface IAuditingService
    {
        string SerializeToJson(object obj);
        Task<AuditRecordResponse> Add(IAudit addRequest);
        Task<IEnumerable<AuditRecordResponse>> ListAuditRecords(DateTime fromDate, DateTime toDate);
    }
}
