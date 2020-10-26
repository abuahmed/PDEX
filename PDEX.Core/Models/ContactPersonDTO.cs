using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PDEX.Core.Enumerations;

namespace PDEX.Core.Models
{
    public class ContactPersonDTO : CommonFieldsC
    {
        public ContactPersonDTO()
        {
            Sex = Sex.Male;
        }
        public Sex Sex
        {
            get { return GetValue(() => Sex); }
            set { SetValue(() => Sex, value); }
        }

        public DateTime? DateOfBirth
        {
            get { return GetValue(() => DateOfBirth); }
            set { SetValue(() => DateOfBirth, value); }
        }
        
        public string Work
        {
            get { return GetValue(() => Work); }
            set { SetValue(() => Work, value); }
        }

        [DataType(DataType.Currency)]
        public decimal? MonthlyIncome
        {
            get { return GetValue(() => MonthlyIncome); }
            set { SetValue(() => MonthlyIncome, value); }
        }

        [NotMapped]
        public int Age
        {
            get
            {
                if (DateOfBirth != null)
                {
                    int age = DateTime.Now.Subtract(DateOfBirth.Value).Days;
                    age = (int)(age / 365.25);
                    return age;
                }
                return 0;
            }
            set { SetValue(() => Age, value); }
        }
    }
}