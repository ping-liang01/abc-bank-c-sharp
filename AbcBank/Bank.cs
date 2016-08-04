using System;
using System.Collections.Generic;
using System.Linq;

namespace AbcBank
{
    public class Bank
    {
        private List<Customer> _customers;

        public Bank()
        {
            _customers = new List<Customer>();
        }

        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

        public String CustomerSummary()
        {
            String summary = "Customer Summary";
            foreach (Customer c in _customers)
                summary += "\n - " + c.Name + " (" + Format(c.GetNumberOfAccounts(), "account") + ")";
            return summary;
        }

        public double TotalInterestPaid()
        {
           return _customers.Select(c => c.TotalInterestEarned()).Sum();
        }

        //Make sure correct plural of word is created based on the number passed in:
        //If number passed in is 1 just return the word otherwise add an 's' at the end
        private String Format(int number, String word)
        {
            return number + " " + (number == 1 ? word : word + "s");
        }

    }
}
