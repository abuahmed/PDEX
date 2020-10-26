using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum NameTypes
    {
        [Description("Client Category")]
        ClientCategory = 0,
        [Description("City Place Names")]
        CityPlace = 1,
        [Description("Building Names")]
        CityBuilding = 2,
        [Description("Street Names/Numbers")]
        StreetNameNumbers = 3,
        [Description("Unit Of Measure")]
        UnitOfMeasure = 4,
        [Description("Category")]
        Category = 5,
        [Description("Document")]
        Document = 6,
        [Description("Task Process Types/Category")]
        TaskProcessType = 7,
        [Description("Tender Published On")]
        TenderPublishedOn = 8,
        [Description("Tender Item Category")]
        TenderItemCategory = 9,
        [Description("Tender Item Process Category")]
        TenderItemProcessCategory = 10,
        
    }
}
