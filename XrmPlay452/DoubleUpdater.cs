using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace XrmPlay452
{
    public class DoubleUpdater
    {
        private readonly IOrganizationService _service;
        public const string EntityLogicalName = "things";
        public const string AttributeName = "beepboop";

        public DoubleUpdater(IOrganizationService service)
        {
            _service = service;
        }

        public void UpdateWithTwoNumbers(int firstNumber, int second)
        {
            UpdateEntityContainingNumberToNumber(7, firstNumber);
            UpdateEntityContainingNumberToNumber(firstNumber, second);
        }

        private void UpdateEntityContainingNumberToNumber(int initialNumber, int updatedNumber)
        {
            using (var context = new OrganizationServiceContext(_service))
            {
                var returnedInitialEntities = from thing in context.CreateQuery(EntityLogicalName)
                    where (int) thing[AttributeName] == initialNumber
                    select thing;

                foreach (var entityToUpdate in returnedInitialEntities)
                {
                    UpdateEntityWithNumber(entityToUpdate, updatedNumber);
                }
            }
        }

        private void UpdateEntityWithNumber(Entity entity, int number)
        {
            entity[AttributeName] = number;
            _service.Update(entity);
        }
    }
}