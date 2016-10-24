
namespace NamePopulatorExtension.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Discovery;
    using Microsoft.Xrm.Sdk.Messages;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using FredPearson;


    public class PostFieldNamePopulatorCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);
            OrganizationServiceContext ctx = new OrganizationServiceContext(service);
            FaultException ex1 = new FaultException();
            //throw new InvalidPluginExecutionException("asdfsd", ex1);


            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {

                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "fp_fieldnamepopulator")
                    return;

                try
                {
                    var entityForPlugin = entity.GetAttributeValue<string>("fp_name").ToString();

                    if (!new Populator(service, entityForPlugin).generatePluginSteps())
                    {
                        throw new InvalidPluginExecutionException("something went wrong", ex1);
                    }

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in the Field Name Populator plug-in.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("Field Name Populator Plugin: {0}", ex.ToString());
                    throw;
                }

            }


        }
    }
}
