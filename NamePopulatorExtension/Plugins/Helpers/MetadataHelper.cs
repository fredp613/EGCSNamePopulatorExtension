using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Helpers
{
    class MetadataHelper
    {
        
        Entity _entity;
        public IOrganizationService _service;

        public MetadataHelper(IOrganizationService service, Entity entity)
        {
            _entity = entity;
            _service = service;
        }

        public MetadataHelper()
        {
            // TODO: Complete member initialization
        }

        public int? getMaxLength(string stringFieldName) {
                        
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = _entity.LogicalName,
                LogicalName = stringFieldName,
                RetrieveAsIfPublished = true
            };

            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)_service.Execute(attributeRequest);
            if (attributeResponse != null)
            {
                if (attributeResponse.AttributeMetadata is StringAttributeMetadata)
                {
                    StringAttributeMetadata attributeMetadata = (StringAttributeMetadata)attributeResponse.AttributeMetadata;

                    if (attributeMetadata.MaxLength != null)
                    {
                        return attributeMetadata.MaxLength;
                    }
                }               
            }           
            return null;
        }

        public int? getObjectTypeCode()
        {
            RetrieveEntityRequest entityRequest = new RetrieveEntityRequest();
            entityRequest.LogicalName = _entity.LogicalName;

            RetrieveEntityResponse entityResponse = (RetrieveEntityResponse)_service.Execute(entityRequest);
            if (entityResponse != null)
            {
                return entityResponse.EntityMetadata.ObjectTypeCode.Value;
            }
            return null;
        }


        public static Boolean ValidateExistence(string entityToVerify, IOrganizationService service, string fieldNames = null)
        {

            var meta = new MetadataHelper
            {
                _service = service

            };

            if (!EntityExists(entityToVerify, meta._service))
            {                
                return false;
            }
            else
            {
                if (fieldNames == null)
                {
                    return true;
                }
                else
                {                   
                    char[] comma = { ',' };
                    var fieldNameArr = fieldNames.ToString().Split(comma).ToList();
                 
                    foreach (var fn in fieldNameArr)
                    {

                        if (FieldNameExists(entityToVerify, meta._service, fn.ToString()) == null)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
           

        }

        private static Boolean EntityExists(string entityToVerify, IOrganizationService service)
        {
            RetrieveEntityRequest entityRequest = new RetrieveEntityRequest();
            entityRequest.LogicalName = entityToVerify;
           
            RetrieveEntityResponse entityResponse = (RetrieveEntityResponse)service.Execute(entityRequest);
            if (entityResponse != null)
            {                   
                return true;
            }
            return false;
        }

        private static int? FieldNameExists(string entityToVerify, IOrganizationService service, string fieldName)
        {
            RetrieveEntityRequest entityRequest = new RetrieveEntityRequest();
            entityRequest.LogicalName = entityToVerify;

            RetrieveEntityResponse entityResponse = (RetrieveEntityResponse)service.Execute(entityRequest);
            if (entityResponse != null)
            {
                return entityResponse.EntityMetadata.ObjectTypeCode.Value;
            }
            return null;
        }

    }
}
