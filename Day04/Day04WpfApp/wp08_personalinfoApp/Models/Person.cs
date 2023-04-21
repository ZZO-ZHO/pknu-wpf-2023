using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp08_personalinfoApp.Logics;

namespace wp08_personalinfoApp.Models
{
    internal class Person
    {

        private string firstname;
        private string lastname;
        private string email;
        private DateTime date;

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { 
            get => email;
            set 
            {
                if(Commons.IsValidEmail(value) != true)
                {
                    throw new Exception("우효하지않은 이메일형식");
                }
                else
                {
                    email = value;
                }
            }
            }    
        public DateTime Data { 
            get => date;
            set
            {
                var result = Commons.GetAge(value);
                if(result > 120 || result <= 0)
                {
                    throw new Exception("유효하지 않은 생일");
                }
                else
                {
                    date = value;
                }
            }
        }   

        public bool IsAdult
        {
            get => Commons.GetAge(date) > 18;   // 19살 이상이면 true
            
        }

        public bool IsBirthDay
        {
            get
            {
                return DateTime.Now.Month == date.Month &&
                    DateTime.Now.Day == date.Day;   // 오늘하고 월일이 같으면 생일
            }
        }

        public string Zodiac
        {
            get => Commons.GetZodiac(date);
        }

        public Person(string firstname, string lastname, string email, DateTime data)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Data = data;
        }   
    }
}
