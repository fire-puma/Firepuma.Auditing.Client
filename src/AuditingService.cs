using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firepuma.Api.Abstractions.Actor;
using Firepuma.Api.Abstractions.Errors;
using Firepuma.Auditing.Abstractions.Requests;
using Firepuma.Auditing.Abstractions.Responses;
using Firepuma.MicroServices.Auth;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Firepuma.Auditing.Client
{
    public class AuditingService<TActor> : IAuditingService where TActor : IActorIdentity
    {
        private readonly IActorProvider<TActor> _actorProvider;
        private readonly IRemoteIpProvider _remoteIpProvider;
        private readonly IErrorReportingService _errorReportingService;
        private readonly MicroServiceClient _microServiceClient;

        public AuditingService(
            IOptions<AuditingMicroServiceOptions> auditingOptions,
            IActorProvider<TActor> actorProvider,
            IRemoteIpProvider remoteIpProvider,
            IErrorReportingService errorReportingService,
            IMicroServiceTokenProvider microServiceTokenProvider)
        {
            _actorProvider = actorProvider;
            _remoteIpProvider = remoteIpProvider;
            _errorReportingService = errorReportingService;

            _microServiceClient = new MicroServiceClient(microServiceTokenProvider, new Uri(auditingOptions.Value.ServiceUrl));
        }

        public string SerializeToJson(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string str)
            {
                return str;
            }

            try
            {
                var json = JsonConvert.SerializeObject(obj);
                return json;
            }
            catch (Exception exception)
            {
                var wrappedException = new Exception(
                    $"Unable to serialize object to json in AuditingService.SerializeToJson, objectType={obj.GetType().FullName}, will use ToString()",
                    exception);
                _errorReportingService.CaptureException(wrappedException);

                return obj.ToString();
            }
        }

        public async Task<AuditRecordResponse> Add(IAudit addRequest)
        {
            var actor = await _actorProvider.GetActor();

            var remoteIp = _remoteIpProvider.GetRemoteIp();
            var oldString = SerializeToJson(addRequest.OldValue);
            var newString = SerializeToJson(addRequest.NewValue);
            var auditRecord = new AddAuditRequest(
                DateTime.Now,
                new AddAuditRequest.AuditActor(actor.Id, actor.Email, actor.FullName),
                remoteIp,
                actor.FullName,
                addRequest.Action,
                addRequest.EntityId,
                addRequest.EntityDescription,
                oldString,
                newString);

            var auditRecordResponse = await _microServiceClient.Post<AddAuditRequest, AuditRecordResponse>("api/v1/audit", auditRecord);

            return auditRecordResponse;
        }

        public async Task<IEnumerable<AuditRecordResponse>> ListAuditRecords(DateTime fromDate, DateTime toDate)
        {
            return await _microServiceClient.Get<IEnumerable<AuditRecordResponse>>($"api/v1/audit/within-dates/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}");
        }
    }
}
