using System.Linq;
using FakeItEasy;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace XrmPlay452.Tests
{
    public class DoubleUpdaterTests
    {
        private readonly XrmFakedContext _fakeContext;
        private readonly DoubleUpdater _doubleUpdater;
        private const int InitialNumber = 7;
        private const int FirstUpdatedNumber = 4;
        private const int SecondUpdatedNumber = 3;

        public DoubleUpdaterTests()
        {
            _fakeContext = new XrmFakedContext();

            var fakeService = _fakeContext.GetOrganizationService();

            var entity = new Entity(DoubleUpdater.EntityLogicalName)
            {
                [DoubleUpdater.AttributeName] = InitialNumber
            };
            fakeService.Create(entity);

            _doubleUpdater = new DoubleUpdater(fakeService);
        }

        [Fact]
        public void DoesNotHaveOldEntity()
        {
            _doubleUpdater.UpdateWithTwoNumbers(FirstUpdatedNumber, SecondUpdatedNumber);

            var returnedInitialEntitiesAfterUpdate =
                from thing in _fakeContext.CreateQuery(DoubleUpdater.EntityLogicalName)
                where thing[DoubleUpdater.AttributeName] as int? == InitialNumber
                select thing;
            Assert.Equal(0, returnedInitialEntitiesAfterUpdate.Count());
        }

        [Fact]
        public void HasFinalNewEntity()
        {
            _doubleUpdater.UpdateWithTwoNumbers(FirstUpdatedNumber, SecondUpdatedNumber);

            var returnedUpdatedEntities = from thing in _fakeContext.CreateQuery(DoubleUpdater.EntityLogicalName)
                where thing[DoubleUpdater.AttributeName] as int? == SecondUpdatedNumber
                select thing;
            Assert.Equal(1, returnedUpdatedEntities.Count());
        }

        [Fact]
        public void CalledTwoUpdates()
        {
            _doubleUpdater.UpdateWithTwoNumbers(FirstUpdatedNumber, SecondUpdatedNumber);

            var fakeService = _fakeContext.GetOrganizationService();
            var calls = Fake.GetCalls(fakeService).ToList();
            Assert.Equal(2, calls.Count(call =>
                    call.Method.Name == "Update"
                )
            );
        }

        [Fact]
        public void CalledUpdateUsingFirstNumber()
        {
            var fakeService = _fakeContext.GetOrganizationService();
            Fake.ClearRecordedCalls(fakeService);

            _doubleUpdater.UpdateWithTwoNumbers(FirstUpdatedNumber, SecondUpdatedNumber);

            var calls = Fake.GetCalls(fakeService).ToList();
            Assert.Contains(calls, call =>
                call.Method.Name == "Update" &&
                call.GetArgument<Entity>(0)?[DoubleUpdater.AttributeName] as int? == FirstUpdatedNumber
            );
        }
    }
}